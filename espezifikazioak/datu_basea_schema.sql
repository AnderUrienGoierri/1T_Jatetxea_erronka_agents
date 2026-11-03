-- =====================================================================
-- JATETXEAREN KUDEAKETA SISTEMA - DATU-BASE ESKEMA (MySQL 8.0 InnoDB)
-- Bertsioa: 2.1 (Lehengaien Stock, Errezeta eta CHECK Murrizketekin)
-- Integritatea: ON DELETE RESTRICT, UNIQUE eta CHECK murrizketak, Soft Delete
-- =====================================================================

DROP DATABASE IF EXISTS jatetxea_db;
CREATE DATABASE jatetxea_db
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE jatetxea_db;

-- 1. LEHENGAIAK / OSAGAIAK TAULA
-- Jatetxeko biltegian dauden lehengaiak (haragia, barazkiak, etab.)
CREATE TABLE lehengaiak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    izena VARCHAR(100) NOT NULL UNIQUE,
    unitatea ENUM('kg', 'g', 'l', 'ml', 'unitatea') NOT NULL DEFAULT 'kg',
    uneko_stocka DECIMAL(10,3) NOT NULL DEFAULT 0.000,
    gutxieneko_stocka DECIMAL(10,3) NOT NULL DEFAULT 5.000,
    kostu_unitarioa DECIMAL(8,2) NOT NULL DEFAULT 0.00,
    hornitzailea VARCHAR(100) NULL,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_lehengai_uneko_stocka CHECK (uneko_stocka >= 0),
    CONSTRAINT chk_lehengai_gutxieneko_stocka CHECK (gutxieneko_stocka >= 0),
    CONSTRAINT chk_lehengai_kostu_unitarioa CHECK (kostu_unitarioa >= 0),
    INDEX idx_lehengai_stock (uneko_stocka, gutxieneko_stocka, ezabatua)
) ENGINE=InnoDB;

-- 2. PLATERAK TAULA (KARTA / MENU)
CREATE TABLE platerak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    izena VARCHAR(100) NOT NULL,
    deskribapena TEXT NULL,
    kategoria ENUM('Sarrerakoa', 'Lehena', 'Bigarrena', 'Postrea', 'Edaria') NOT NULL,
    prezioa DECIMAL(6,2) NOT NULL,
    alergenoak VARCHAR(200) NULL,
    aktibo TINYINT(1) NOT NULL DEFAULT 1,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_plater_prezioa CHECK (prezioa >= 0),
    INDEX idx_plater_kategoria (kategoria, ezabatua)
) ENGINE=InnoDB;

-- 3. PLATER_OSAGAIAK TAULA (ERREZETA / ESKANDALLOA - N:M)
-- Plater bakoitzak behar duen lehengai bakoitzaren proportzioa
CREATE TABLE plater_osagaiak (
    platera_id INT NOT NULL,
    lehengaia_id INT NOT NULL,
    kantitatea DECIMAL(10,3) NOT NULL, -- Adib. 0.350 kg haragi, 0.050 l olio
    
    PRIMARY KEY (platera_id, lehengaia_id),
    CONSTRAINT chk_plater_osagai_kantitatea CHECK (kantitatea > 0),
    CONSTRAINT fk_plater_osagai_platera FOREIGN KEY (platera_id)
        REFERENCES platerak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_plater_osagai_lehengaia FOREIGN KEY (lehengaia_id)
        REFERENCES lehengaiak(id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- 4. MAHAIAK TAULA
CREATE TABLE mahaiak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    zenbakia INT NOT NULL UNIQUE,
    edukiera INT NOT NULL DEFAULT 4,
    kokapena ENUM('Barran', 'Jantoki Nagusia', 'Terraza', 'VIP') NOT NULL DEFAULT 'Jantoki Nagusia',
    egoera ENUM('Libre', 'Okupatuta', 'Erreserbatuta', 'Garbitzen') NOT NULL DEFAULT 'Libre',
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_mahai_zenbakia CHECK (zenbakia > 0),
    CONSTRAINT chk_mahai_edukiera CHECK (edukiera > 0),
    INDEX idx_mahai_egoera (egoera, ezabatua)
) ENGINE=InnoDB;

-- 5. BEZEROAK TAULA
CREATE TABLE bezeroak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    izena VARCHAR(80) NOT NULL,
    abizena VARCHAR(100) NOT NULL,
    telefonoa VARCHAR(20) NOT NULL,
    emaila VARCHAR(120) NOT NULL UNIQUE,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 6. LANGILEAK TAULA
CREATE TABLE langileak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    erabiltzailea VARCHAR(50) NOT NULL UNIQUE,
    pasahitza_hash VARCHAR(255) NOT NULL,
    izena VARCHAR(80) NOT NULL,
    rola ENUM('Zerbitzaria', 'Sukaldaria', 'Kudeatzailea', 'Admin') NOT NULL DEFAULT 'Zerbitzaria',
    aktibo TINYINT(1) NOT NULL DEFAULT 1,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 7. ERRESERBAK TAULA
-- UNIQUE gakoak bermatzen du mahai bera ezin dela txanda berean bi aldiz erreserbatu
CREATE TABLE erreserbak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    bezeroa_id INT NOT NULL,
    mahaia_id INT NOT NULL,
    data DATE NOT NULL,
    txanda ENUM('Bazkaria_1', 'Bazkaria_2', 'Afaria_1', 'Afaria_2') NOT NULL,
    pertsona_kopurua INT NOT NULL,
    egoera ENUM('Baieztatua', 'Eserita', 'Ezeztatua', 'Bukatua') NOT NULL DEFAULT 'Baieztatua',
    oharrak VARCHAR(255) NULL,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT chk_erreserba_pertsona_kopurua CHECK (pertsona_kopurua > 0),
    CONSTRAINT uq_erreserba_mahaia_data_txanda UNIQUE (mahaia_id, data, txanda),
    CONSTRAINT fk_erreserbak_bezeroak FOREIGN KEY (bezeroa_id)
        REFERENCES bezeroak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_erreserbak_mahaiak FOREIGN KEY (mahaia_id)
        REFERENCES mahaiak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
        
    INDEX idx_erreserba_bilaketa (data, txanda, egoera)
) ENGINE=InnoDB;

-- 8. ESKAERAK TAULA (KOMANDAK)
CREATE TABLE eskaerak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    mahaia_id INT NOT NULL,
    langilea_id INT NOT NULL,
    egoera ENUM('Irekia', 'Sukaldean', 'Zerbitzatua', 'Kobratua', 'Ezeztatua') NOT NULL DEFAULT 'Irekia',
    guztira DECIMAL(8,2) NOT NULL DEFAULT 0.00,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT chk_eskaera_guztira CHECK (guztira >= 0),
    CONSTRAINT fk_eskaerak_mahaiak FOREIGN KEY (mahaia_id)
        REFERENCES mahaiak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_eskaerak_langileak FOREIGN KEY (langilea_id)
        REFERENCES langileak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
        
    INDEX idx_eskaera_mahaia (mahaia_id, egoera)
) ENGINE=InnoDB;

-- 9. ESKAERA LERROAK TAULA
CREATE TABLE eskaera_lerroak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    eskaera_id INT NOT NULL,
    platera_id INT NOT NULL,
    kantitatea INT NOT NULL DEFAULT 1,
    unitate_prezioa DECIMAL(6,2) NOT NULL,
    egoera ENUM('Eskatua', 'Sukaldean', 'Prestatua', 'Zerbitzatua') NOT NULL DEFAULT 'Eskatua',
    oharrak VARCHAR(150) NULL,
    
    CONSTRAINT chk_lerro_kantitatea CHECK (kantitatea > 0),
    CONSTRAINT chk_lerro_unitate_prezioa CHECK (unitate_prezioa >= 0),
    CONSTRAINT fk_lerroak_eskaerak FOREIGN KEY (eskaera_id)
        REFERENCES eskaerak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_lerroak_platerak FOREIGN KEY (platera_id)
        REFERENCES platerak(id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- 10. STOCK MUGIMENDUAK (HISTORIKOA / AUDITORIA)
CREATE TABLE stock_mugimenduak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    lehengaia_id INT NOT NULL,
    mota ENUM('Sarrera_Albarana', 'Kontsumoa_Platera', 'Egokitzapena_Inbentarioa') NOT NULL,
    kantitatea DECIMAL(10,3) NOT NULL, -- Positiboa sarreretan, negatiboa kontsumoan
    eskaera_id INT NULL,
    oharrak VARCHAR(200) NULL,
    data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT chk_mugimendu_kantitatea CHECK (kantitatea <> 0),
    CONSTRAINT fk_mugimendu_lehengaia FOREIGN KEY (lehengaia_id)
        REFERENCES lehengaiak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_mugimendu_eskaera FOREIGN KEY (eskaera_id)
        REFERENCES eskaerak(id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- 11. IRITZIAK TAULA (BEZEROEN BALORAZIOAK ETA IRUZKINAK)
-- Bezeroek iritziak (0-5 izar) eta iruzkinak uzteko aukera
CREATE TABLE iritziak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    bezeroa_id INT NOT NULL,
    balorazioa TINYINT NOT NULL, -- 0 eta 5 izar artean
    testua TEXT NULL,
    aktibo TINYINT(1) NOT NULL DEFAULT 1,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT chk_iritzia_balorazioa CHECK (balorazioa >= 0 AND balorazioa <= 5),
    CONSTRAINT fk_iritziak_bezeroak FOREIGN KEY (bezeroa_id)
        REFERENCES bezeroak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
        
    INDEX idx_iritzia_balorazioa (balorazioa),
    INDEX idx_iritzia_bezeroa (bezeroa_id, ezabatua)
) ENGINE=InnoDB;

-- =====================================================================
-- IKUSPEGIAK (VIEWS) STOCK KONTROLERAKO
-- =====================================================================

-- 1. Erosi beharreko lehengaiak (Alerta: stocka gutxienekoaren azpitik)
CREATE OR REPLACE VIEW v_erosi_beharreko_lehengaiak AS
SELECT 
    id AS lehengaia_id,
    izena,
    unitatea,
    uneko_stocka,
    gutxieneko_stocka,
    (gutxieneko_stocka * 2 - uneko_stocka) AS proposatutako_erosketa,
    hornitzailea
FROM lehengaiak
WHERE uneko_stocka <= gutxieneko_stocka AND ezabatua = 0;

-- 2. Plater bakoitzaren ekoizpen-ahalmena (zenbat plater egin daitezkeen lehengaien arabera)
CREATE OR REPLACE VIEW v_plateren_stock_ahalmena AS
SELECT 
    p.id AS platera_id,
    p.izena AS plater_izena,
    COALESCE(FLOOR(MIN(l.uneko_stocka / po.kantitatea)), 0) AS egin_daitezkeen_platerak
FROM platerak p
JOIN plater_osagaiak po ON p.id = po.platera_id
JOIN lehengaiak l ON po.lehengaia_id = l.id
WHERE p.ezabatua = 0 AND l.ezabatua = 0
GROUP BY p.id, p.izena;

-- 3. Iritzien estatistika orokorrak (bataz besteko puntuazioa eta izar banaketa)
CREATE OR REPLACE VIEW v_balorazio_orokorrak AS
SELECT 
    COUNT(*) AS guztira_iritziak,
    ROUND(AVG(balorazioa), 1) AS bataz_besteko_balorazioa,
    COUNT(CASE WHEN balorazioa = 5 THEN 1 END) AS bost_izarrak,
    COUNT(CASE WHEN balorazioa = 4 THEN 1 END) AS lau_izarrak,
    COUNT(CASE WHEN balorazioa = 3 THEN 1 END) AS hiru_izarrak,
    COUNT(CASE WHEN balorazioa = 2 THEN 1 END) AS bi_izarrak,
    COUNT(CASE WHEN balorazioa = 1 THEN 1 END) AS izar_bat,
    COUNT(CASE WHEN balorazioa = 0 THEN 1 END) AS zero_izar
FROM iritziak
WHERE ezabatua = 0 AND aktibo = 1;

-- =====================================================================
-- DATUAK TXERTATU DB-an 
-- =====================================================================

-- Lehengaiak
INSERT INTO lehengaiak (izena, unitatea, uneko_stocka, gutxieneko_stocka, kostu_unitarioa, hornitzailea) VALUES
('Euskal Okela Txuleta', 'kg', 12.000, 5.000, 22.00, 'Harategia Goierri'),
('Idiazabal Gazta', 'kg', 4.500, 2.000, 18.00, 'Gaztandegia'),
('Tolosako Babarruna', 'kg', 15.000, 5.000, 9.00, 'Barazki Koop'),
('Bakailao Gazitua', 'kg', 8.000, 3.000, 16.50, 'Arraindegia'),
('Oliba Olio Birjina', 'l', 25.000, 10.000, 8.50, 'Olio Ekoizleak'),
('Tipula', 'kg', 20.000, 5.000, 1.20, 'Barazki Koop'),
('Krema eta Hojaldrea', 'kg', 2.000, 3.000, 6.00, 'Okindegia'); -- Gutxienekoaren azpitik!

-- Platerak
INSERT INTO platerak (izena, deskribapena, kategoria, prezioa, alergenoak) VALUES
('Txuleta Errea (kg)', 'Euskal Okela plantxan gatz lodiarekin', 'Bigarrena', 45.00, NULL),
('Tolosako Babarrunak', 'Sakramentu guztiekin', 'Lehena', 14.00, NULL),
('Bakailaoa Bizkaitar Erara', 'Tradizionala piperrekin', 'Bigarrena', 18.50, 'Arraina'),
('Pantxineta Etxekoa', 'Krema epelekin egina', 'Postrea', 6.00, 'Glutena, Arrautza, Laktosa');

-- Errezetak (Plater bakoitzak zein osagai eta zein proportziotan kontsumitzen duen)
-- Txuleta: 1.000 kg Okela + 0.050 l Olio
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea) VALUES
(1, 1, 1.000),
(1, 5, 0.050);

-- Tolosako Babarrunak: 0.150 kg Babarrun + 0.050 kg Tipula + 0.030 l Olio
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea) VALUES
(2, 3, 0.150),
(2, 6, 0.050),
(2, 5, 0.030);

-- Bakailaoa: 0.250 kg Bakailao + 0.080 kg Tipula + 0.050 l Olio
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea) VALUES
(3, 4, 0.250),
(3, 6, 0.080),
(3, 5, 0.050);

-- Pantxineta: 0.150 kg Krema eta Hojaldre
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea) VALUES
(4, 7, 0.150);

-- Mahaiak
INSERT INTO mahaiak (zenbakia, edukiera, kokapena, egoera) VALUES
(1, 2, 'Jantoki Nagusia', 'Libre'),
(2, 4, 'Jantoki Nagusia', 'Libre'),
(3, 4, 'Jantoki Nagusia', 'Libre'),
(4, 6, 'Jantoki Nagusia', 'Libre'),
(5, 8, 'VIP', 'Libre'),
(6, 4, 'Terraza', 'Libre');

-- Langileak
INSERT INTO langileak (erabiltzailea, pasahitza_hash, izena, rola) VALUES
('zerbitzari1', '$2a$11$eXampleHashedPassword1', 'Mikel Arregi', 'Zerbitzaria'),
('sukaldari1', '$2a$11$eXampleHashedPassword2', 'Ainhoa Goikoetxea', 'Sukaldaria'),
('admin1', '$2a$11$eXampleHashedPassword3', 'Koldo Etxeberria', 'Admin');

-- Bezeroak
INSERT INTO bezeroak (izena, abizena, telefonoa, emaila) VALUES
('Ane', 'Iparragirre', '600112233', 'ane@adibidea.eus'),
('Ion', 'Zubizarreta', '611223344', 'ion@adibidea.eus');

-- Iritziak (0-5 izar)
INSERT INTO iritziak (bezeroa_id, balorazioa, testua) VALUES
(1, 5, 'Txuleta zoragarria eta zerbitzu bikaina! Seguru itzuliko garela.'),
(2, 4, 'Babarrun goxoak eta giro atsegina, baina postreetan aukera zabalagoa eskertuko litzateke.');
