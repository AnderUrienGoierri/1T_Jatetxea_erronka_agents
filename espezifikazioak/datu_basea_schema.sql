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
('Krema eta Hojaldrea', 'kg', 2.000, 3.000, 6.00, 'Okindegia'),
('Intxaurrak', 'kg', 5.000, 1.500, 12.00, 'Barazki Koop'),
('Sagardo Membrilloa', 'kg', 4.000, 1.000, 8.00, 'Gaztandegia'),
('Urdaiazpiko Iberikoa', 'kg', 6.000, 2.000, 48.00, 'Harategia Goierri'),
('Ogia eta Tomatea', 'kg', 10.000, 3.000, 2.50, 'Okindegia'),
('Txangurro Haragia', 'kg', 4.000, 1.500, 35.00, 'Arraindegia'),
('Esnea', 'l', 40.000, 15.000, 1.10, 'Esne Ekoizleak'),
('Irina', 'kg', 25.000, 10.000, 1.00, 'Okindegia'),
('Antxoak Kantaurikoak', 'kg', 5.000, 2.000, 28.00, 'Arraindegia'),
('Piparrak eta Olibak', 'kg', 6.000, 2.000, 9.50, 'Barazki Koop'),
('Puerroak eta Barazkiak', 'kg', 20.000, 5.000, 1.80, 'Barazki Koop'),
('Patatak', 'kg', 50.000, 15.000, 0.90, 'Barazki Koop'),
('Arroza', 'kg', 30.000, 10.000, 1.50, 'Janari Banatzailea'),
('Boletus Onddoak', 'kg', 8.000, 2.500, 26.00, 'Baso Ekoizleak'),
('Barazki Salda', 'l', 25.000, 8.000, 2.00, 'Sukalde Ekoizpena'),
('Legatza Freskoa', 'kg', 12.000, 4.000, 24.00, 'Arraindegia'),
('Baratxuria eta Gindilla', 'kg', 5.000, 1.500, 4.00, 'Barazki Koop'),
('Sagardo Ozpina', 'l', 10.000, 3.000, 3.50, 'Sagardotegia'),
('Txipiroiak', 'kg', 10.000, 3.000, 22.00, 'Arraindegia'),
('Txipiroi Tinta', 'kg', 2.000, 0.500, 18.00, 'Arraindegia'),
('Euskal Arkumea', 'kg', 15.000, 5.000, 19.00, 'Harategia Goierri'),
('Txerri Solomoa', 'kg', 12.000, 4.000, 11.00, 'Harategia Goierri'),
('Esnegaina', 'l', 15.000, 5.000, 3.80, 'Esne Ekoizleak'),
('Arrautzak', 'kg', 12.000, 4.000, 3.20, 'Baserri Tokikoa'),
('Eztia', 'kg', 6.000, 2.000, 12.00, 'Erlezain Tokikoa'),
('Ardi Esnea Mamia', 'l', 12.000, 4.000, 4.50, 'Gaztandegia'),
('Sagardo Botila', 'unitatea', 60.000, 20.000, 2.20, 'Bereziartua Sagardotegia'),
('Txakolin Botila', 'unitatea', 36.000, 12.000, 7.50, 'Ameztoi Upategia'),
('Ardo Beltza Crianza Botila', 'unitatea', 48.000, 15.000, 8.00, 'Errioxako Upategiak'),
('Ardo Beltza Erreserba Botila', 'unitatea', 24.000, 8.000, 14.00, 'Errioxako Upategiak'),
('Ardo Gorri Botila', 'unitatea', 30.000, 10.000, 6.50, 'Nafarroako Upategiak'),
('Etxeko Ardoa Upela', 'l', 80.000, 25.000, 2.50, 'Upategi Tokikoa'),
('Basqueland Garagardo Botila', 'unitatea', 48.000, 15.000, 1.80, 'Basqueland Brewing'),
('Garagardo Upela', 'l', 100.000, 30.000, 1.90, 'Garagardo Banatzailea'),
('Garagardo 00 Botila', 'unitatea', 36.000, 12.000, 1.20, 'Garagardo Banatzailea'),
('Ur Botila 1L', 'unitatea', 100.000, 30.000, 0.70, 'Alzola Urak'),
('Ur Gasdun Botila', 'unitatea', 48.000, 15.000, 0.80, 'Alzola Urak'),
('Freskagarri Lata', 'unitatea', 80.000, 25.000, 0.90, 'Edari Banatzailea'),
('Sagar Zuku Botila', 'unitatea', 30.000, 10.000, 1.40, 'Sagardotegi Tokikoa'),
('Kafe Aleak', 'kg', 8.000, 2.500, 16.00, 'Baque Kafeak'),
('Patxaran Botila', 'l', 10.000, 3.000, 12.00, 'Baines Patxarana');

-- Platerak (Karta Zabala)
INSERT INTO platerak (izena, deskribapena, kategoria, prezioa, alergenoak) VALUES
-- Sarrerakoak
('Idiazabal Gazta Dastaketa', 'Bertako gazta ondua intxaurrekin eta sagardo membrilloarekin', 'Sarrerakoa', 12.00, 'Laktosa, Fruitu lehorrak'),
('Iberiar Urdaiazpiko Ezkur-jana', 'Ezkurrez elikatutako 100% iberiarra ogi txigortu eta tomatearekin', 'Sarrerakoa', 19.50, 'Glutena'),
('Txangurro Kroketa Etxekoak (8 un.)', 'Itsas zapore sakoneko kroketa krematsuak', 'Sarrerakoa', 11.50, 'Krustazeoak, Glutena, Laktosa'),
('Gildak eta Antxoak Olio Epelekin', 'Kantauriko antxoak, Ibarrako piparrak eta oliba berdeak', 'Sarrerakoa', 9.50, 'Arraina'),
-- Lehen Platerak
('Tolosako Babarrunak', 'Sakramentu guztiekin', 'Lehena', 14.00, NULL),
('Porrusalda Bakailao Miguekin', 'Tradizionala baratze-puerroekin eta bakailao mamitsuarekin', 'Lehena', 10.50, 'Arraina'),
('Sasoiko Barazki Menestra', 'Goierriko barazki freskoak birjina olioarekin erregosiak', 'Lehena', 12.50, NULL),
('Onddo eta Idiazabal Risotto Kremozua', 'Boletus edulis onddoak eta Idiazabal gazta urtua', 'Lehena', 15.00, 'Laktosa'),
-- Bigarren Platerak
('Txuleta Errea (kg)', 'Euskal Okela plantxan gatz lodiarekin', 'Bigarrena', 45.00, NULL),
('Bakailaoa Bizkaitar Erara', 'Tradizionala piperrekin', 'Bigarrena', 18.50, 'Arraina'),
('Legatz Errea Orio Erara', 'Parrillan errea baratxuri, gindilla eta sagardo ozpin epelarekin', 'Bigarrena', 22.00, 'Arraina'),
('Txipiroiak Beren Tintan', 'Kostaldeko errezetan prestatutako txipiroi freskoak arroza zuriarekin', 'Bigarrena', 19.00, 'Moluskuak'),
('Arkume Errea Labean', 'Euskal arkume errea patata panadera gozoekin', 'Bigarrena', 24.50, NULL),
('Euskal Txerri Solomoa Gazta Saltsarekin', 'Bertako txerri solomoa plantxan Idiazabal saltsa leunarekin', 'Bigarrena', 16.50, 'Laktosa'),
-- Postreak
('Pantxineta Etxekoa', 'Krema epelekin egina', 'Postrea', 6.00, 'Glutena, Arrautza, Laktosa'),
('Goxua Tradizionala', 'Bizkotxo bustia, pastel-krema, esnegaina eta karamelo txigortua', 'Postrea', 5.50, 'Glutena, Arrautza, Laktosa'),
('Idiazabal Gazta Tarta Eztiarekin', 'Etxeko tarta labean egina bertako baserriko eztiarekin', 'Postrea', 6.50, 'Laktosa, Arrautza'),
('Etxeko Mamia Intxaur eta Eztiarekin', 'Latxa ardi esnearekin egindako mamia intxaurrez lagunduta', 'Postrea', 5.00, 'Laktosa, Fruitu lehorrak'),
('Arroz-esnea Kanelarekin', 'Astiro egositako arroz-esne kremozua kanela hautseztatuta', 'Postrea', 4.50, 'Laktosa'),
-- Edariak
('Euskal Sagardo Naturala (75cl)', 'Kupako sagardo fresko eta naturala', 'Edaria', 4.50, 'Sulfitoak'),
('Getariako Txakolina D.O. (75cl)', 'Hondarrabi Zuri mahatsez egindako txakolin txinpartatsua', 'Edaria', 16.00, 'Sulfitoak'),
('Errioxako Ardo Beltza Crianza D.O. (75cl)', 'Haritz upelean ondua, gorputz handikoa', 'Edaria', 18.00, 'Sulfitoak'),
('Errioxako Ardo Beltza Erreserba D.O. (75cl)', 'Hautespen berezia, dotorea eta orekatua', 'Edaria', 26.00, 'Sulfitoak'),
('Nafarroako Ardo Gorria D.O. (75cl)', 'Garnatxa mahatsa, freskoa eta fruitutsua', 'Edaria', 14.00, 'Sulfitoak'),
('Etxeko Ardoa Pitxerra (1L)', 'Upategiko ardo gaztea jantokirako', 'Edaria', 7.00, 'Sulfitoak'),
('Eskulangile Garagardoa Basqueland (33cl)', 'Euskal Herrian artisau moduan ekoitzitako IPA garagardoa', 'Edaria', 3.80, 'Glutena'),
('Garagardo Kaña Zirriborroa (33cl)', 'Iturriko garagardo hotza', 'Edaria', 2.80, 'Glutena'),
('Garagardo Zuritoa (20cl)', 'Iturriko garagardo trago laburra', 'Edaria', 1.80, 'Glutena'),
('Garagardo 0,0% Alkohol Gabea (33cl)', 'Alkohola kendutako garagardo freskagarria', 'Edaria', 2.80, 'Glutena'),
('Ur Mineral Naturala (1L)', 'Alzola euskal iturriko ur garbia', 'Edaria', 2.50, NULL),
('Ur Mineral Gasduna (50cl)', 'Burbuila fin eta freskagarriak', 'Edaria', 2.20, NULL),
('Freskagarria (Kola, Laranja, Limoia 33cl)', 'Hautatutako freskagarri lata', 'Edaria', 2.60, NULL),
('Etxeko Sagar Zuku Ekologikoa (25cl)', 'Sagar bertakoz egindako zuku naturala', 'Edaria', 3.00, NULL),
('Kafe Ebakia / Esnearekin', 'Baque kalitate goreneko kafe aleak', 'Edaria', 1.80, 'Laktosa'),
('Patxaran Tradizionala (Kopa)', 'Nafarroako basaranez egindako patxaran digestiboa', 'Edaria', 4.00, NULL);

-- Errezetak (Plater bakoitzak zein osagai eta zein proportziotan kontsumitzen duen)
-- Txuleta
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Txuleta Errea (kg)' AND l.izena = 'Euskal Okela Txuleta';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Txuleta Errea (kg)' AND l.izena = 'Oliba Olio Birjina';

-- Tolosako Babarrunak
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Tolosako Babarrunak' AND l.izena = 'Tolosako Babarruna';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Tolosako Babarrunak' AND l.izena = 'Tipula';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Tolosako Babarrunak' AND l.izena = 'Oliba Olio Birjina';

-- Bakailaoa Bizkaitar Erara
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.250 FROM platerak p, lehengaiak l WHERE p.izena = 'Bakailaoa Bizkaitar Erara' AND l.izena = 'Bakailao Gazitua';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Bakailaoa Bizkaitar Erara' AND l.izena = 'Tipula';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Bakailaoa Bizkaitar Erara' AND l.izena = 'Oliba Olio Birjina';

-- Pantxineta Etxekoa
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Pantxineta Etxekoa' AND l.izena = 'Krema eta Hojaldrea';

-- Idiazabal Gazta Dastaketa
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Dastaketa' AND l.izena = 'Idiazabal Gazta';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.040 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Dastaketa' AND l.izena = 'Intxaurrak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Dastaketa' AND l.izena = 'Sagardo Membrilloa';

-- Iberiar Urdaiazpiko Ezkur-jana
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.100 FROM platerak p, lehengaiak l WHERE p.izena = 'Iberiar Urdaiazpiko Ezkur-jana' AND l.izena = 'Urdaiazpiko Iberikoa';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Iberiar Urdaiazpiko Ezkur-jana' AND l.izena = 'Ogia eta Tomatea';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Iberiar Urdaiazpiko Ezkur-jana' AND l.izena = 'Oliba Olio Birjina';

-- Txangurro Kroketa Etxekoak
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.120 FROM platerak p, lehengaiak l WHERE p.izena = 'Txangurro Kroketa Etxekoak (8 un.)' AND l.izena = 'Txangurro Haragia';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.200 FROM platerak p, lehengaiak l WHERE p.izena = 'Txangurro Kroketa Etxekoak (8 un.)' AND l.izena = 'Esnea';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Txangurro Kroketa Etxekoak (8 un.)' AND l.izena = 'Irina';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Txangurro Kroketa Etxekoak (8 un.)' AND l.izena = 'Oliba Olio Birjina';

-- Gildak eta Antxoak Olio Epelekin
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Gildak eta Antxoak Olio Epelekin' AND l.izena = 'Antxoak Kantaurikoak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.060 FROM platerak p, lehengaiak l WHERE p.izena = 'Gildak eta Antxoak Olio Epelekin' AND l.izena = 'Piparrak eta Olibak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Gildak eta Antxoak Olio Epelekin' AND l.izena = 'Oliba Olio Birjina';

-- Porrusalda Bakailao Miguekin
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.200 FROM platerak p, lehengaiak l WHERE p.izena = 'Porrusalda Bakailao Miguekin' AND l.izena = 'Puerroak eta Barazkiak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Porrusalda Bakailao Miguekin' AND l.izena = 'Patatak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Porrusalda Bakailao Miguekin' AND l.izena = 'Bakailao Gazitua';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Porrusalda Bakailao Miguekin' AND l.izena = 'Oliba Olio Birjina';

-- Sasoiko Barazki Menestra
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.350 FROM platerak p, lehengaiak l WHERE p.izena = 'Sasoiko Barazki Menestra' AND l.izena = 'Puerroak eta Barazkiak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.025 FROM platerak p, lehengaiak l WHERE p.izena = 'Sasoiko Barazki Menestra' AND l.izena = 'Oliba Olio Birjina';

-- Onddo eta Idiazabal Risotto Kremozua
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.120 FROM platerak p, lehengaiak l WHERE p.izena = 'Onddo eta Idiazabal Risotto Kremozua' AND l.izena = 'Arroza';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.100 FROM platerak p, lehengaiak l WHERE p.izena = 'Onddo eta Idiazabal Risotto Kremozua' AND l.izena = 'Boletus Onddoak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.040 FROM platerak p, lehengaiak l WHERE p.izena = 'Onddo eta Idiazabal Risotto Kremozua' AND l.izena = 'Idiazabal Gazta';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.250 FROM platerak p, lehengaiak l WHERE p.izena = 'Onddo eta Idiazabal Risotto Kremozua' AND l.izena = 'Barazki Salda';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Onddo eta Idiazabal Risotto Kremozua' AND l.izena = 'Oliba Olio Birjina';

-- Legatz Errea Orio Erara
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.350 FROM platerak p, lehengaiak l WHERE p.izena = 'Legatz Errea Orio Erara' AND l.izena = 'Legatza Freskoa';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Legatz Errea Orio Erara' AND l.izena = 'Baratxuria eta Gindilla';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.015 FROM platerak p, lehengaiak l WHERE p.izena = 'Legatz Errea Orio Erara' AND l.izena = 'Sagardo Ozpina';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.040 FROM platerak p, lehengaiak l WHERE p.izena = 'Legatz Errea Orio Erara' AND l.izena = 'Oliba Olio Birjina';

-- Txipiroiak Beren Tintan
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.300 FROM platerak p, lehengaiak l WHERE p.izena = 'Txipiroiak Beren Tintan' AND l.izena = 'Txipiroiak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Txipiroiak Beren Tintan' AND l.izena = 'Tipula';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Txipiroiak Beren Tintan' AND l.izena = 'Txipiroi Tinta';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Txipiroiak Beren Tintan' AND l.izena = 'Arroza';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Txipiroiak Beren Tintan' AND l.izena = 'Oliba Olio Birjina';

-- Arkume Errea Labean
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.500 FROM platerak p, lehengaiak l WHERE p.izena = 'Arkume Errea Labean' AND l.izena = 'Euskal Arkumea';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Arkume Errea Labean' AND l.izena = 'Patatak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Arkume Errea Labean' AND l.izena = 'Oliba Olio Birjina';

-- Euskal Txerri Solomoa Gazta Saltsarekin
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.250 FROM platerak p, lehengaiak l WHERE p.izena = 'Euskal Txerri Solomoa Gazta Saltsarekin' AND l.izena = 'Txerri Solomoa';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Euskal Txerri Solomoa Gazta Saltsarekin' AND l.izena = 'Idiazabal Gazta';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.060 FROM platerak p, lehengaiak l WHERE p.izena = 'Euskal Txerri Solomoa Gazta Saltsarekin' AND l.izena = 'Esnegaina';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Euskal Txerri Solomoa Gazta Saltsarekin' AND l.izena = 'Oliba Olio Birjina';

-- Goxua Tradizionala
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.120 FROM platerak p, lehengaiak l WHERE p.izena = 'Goxua Tradizionala' AND l.izena = 'Krema eta Hojaldrea';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Goxua Tradizionala' AND l.izena = 'Esnegaina';

-- Idiazabal Gazta Tarta Eztiarekin
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.100 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Tarta Eztiarekin' AND l.izena = 'Idiazabal Gazta';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Tarta Eztiarekin' AND l.izena = 'Arrautzak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Tarta Eztiarekin' AND l.izena = 'Eztia';

-- Etxeko Mamia Intxaur eta Eztiarekin
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.180 FROM platerak p, lehengaiak l WHERE p.izena = 'Etxeko Mamia Intxaur eta Eztiarekin' AND l.izena = 'Ardi Esnea Mamia';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Etxeko Mamia Intxaur eta Eztiarekin' AND l.izena = 'Intxaurrak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Etxeko Mamia Intxaur eta Eztiarekin' AND l.izena = 'Eztia';

-- Arroz-esnea Kanelarekin
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Arroz-esnea Kanelarekin' AND l.izena = 'Arroza';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.200 FROM platerak p, lehengaiak l WHERE p.izena = 'Arroz-esnea Kanelarekin' AND l.izena = 'Esnea';

-- Edariak
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Euskal Sagardo Naturala (75cl)' AND l.izena = 'Sagardo Botila';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Getariako Txakolina D.O. (75cl)' AND l.izena = 'Txakolin Botila';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Errioxako Ardo Beltza Crianza D.O. (75cl)' AND l.izena = 'Ardo Beltza Crianza Botila';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Errioxako Ardo Beltza Erreserba D.O. (75cl)' AND l.izena = 'Ardo Beltza Erreserba Botila';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Nafarroako Ardo Gorria D.O. (75cl)' AND l.izena = 'Ardo Gorri Botila';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Etxeko Ardoa Pitxerra (1L)' AND l.izena = 'Etxeko Ardoa Upela';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Eskulangile Garagardoa Basqueland (33cl)' AND l.izena = 'Basqueland Garagardo Botila';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.330 FROM platerak p, lehengaiak l WHERE p.izena = 'Garagardo Kaña Zirriborroa (33cl)' AND l.izena = 'Garagardo Upela';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.200 FROM platerak p, lehengaiak l WHERE p.izena = 'Garagardo Zuritoa (20cl)' AND l.izena = 'Garagardo Upela';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Garagardo 0,0% Alkohol Gabea (33cl)' AND l.izena = 'Garagardo 00 Botila';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Ur Mineral Naturala (1L)' AND l.izena = 'Ur Botila 1L';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Ur Mineral Gasduna (50cl)' AND l.izena = 'Ur Gasdun Botila';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Freskagarria (Kola, Laranja, Limoia 33cl)' AND l.izena = 'Freskagarri Lata';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Etxeko Sagar Zuku Ekologikoa (25cl)' AND l.izena = 'Sagar Zuku Botila';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.010 FROM platerak p, lehengaiak l WHERE p.izena = 'Kafe Ebakia / Esnearekin' AND l.izena = 'Kafe Aleak';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Kafe Ebakia / Esnearekin' AND l.izena = 'Esnea';
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.060 FROM platerak p, lehengaiak l WHERE p.izena = 'Patxaran Tradizionala (Kopa)' AND l.izena = 'Patxaran Botila';

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
