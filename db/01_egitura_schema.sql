-- =====================================================================
-- 1T JATETXEA - DATU-BASEAREN EGITURA (DDL - SCHEMA)
-- Motorra: MySQL 8.0 InnoDB | Karaktere-jokoa: utf8mb4
-- Bertsioa: 3.1 (Egitura eta Datuak banatuta: 01_egitura_schema.sql)
-- Barne: 17 taula (hornitzaileak barne), CHECK/UNIQUE/FK murrizketak, indizeak eta 3 VIEW
-- =====================================================================

SET NAMES utf8mb4;

DROP DATABASE IF EXISTS jatetxea_db;
CREATE DATABASE jatetxea_db
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE jatetxea_db;

-- =====================================================================
-- 1. KATALOGOA ETA BILTEGIA (HORNITZAILEAK, LEHENGAIAK, PLATERAK, ERREZETAK)
-- =====================================================================

-- 1.0 HORNITZAILEAK (PRODUKTU MOTA BAKOITZEKO HORNITZAILE BAT)
CREATE TABLE hornitzaileak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    izena VARCHAR(120) NOT NULL UNIQUE,
    cif VARCHAR(20) NOT NULL UNIQUE,
    produktu_mota VARCHAR(80) NOT NULL,
    bez_ehunekoa DECIMAL(4,2) NOT NULL DEFAULT 10.00,
    telefonoa VARCHAR(20) NULL,
    emaila VARCHAR(120) NULL,
    helbidea VARCHAR(200) NULL,
    aktibo TINYINT(1) NOT NULL DEFAULT 1,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT chk_hornitzaile_bez CHECK (bez_ehunekoa >= 0 AND bez_ehunekoa <= 100),
    INDEX idx_hornitzaile_mota (produktu_mota, ezabatua)
) ENGINE=InnoDB;

-- 1.1 LEHENGAIAK / OSAGAIAK
CREATE TABLE lehengaiak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    izena VARCHAR(100) NOT NULL UNIQUE,
    unitatea ENUM('kg', 'g', 'l', 'ml', 'unitatea') NOT NULL DEFAULT 'kg',
    uneko_stocka DECIMAL(10,3) NOT NULL DEFAULT 0.000,
    gutxieneko_stocka DECIMAL(10,3) NOT NULL DEFAULT 5.000,
    kostu_unitarioa DECIMAL(8,2) NOT NULL DEFAULT 0.00,
    hornitzailea_id INT NULL,
    hornitzailea VARCHAR(120) NULL,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT chk_lehengai_uneko_stocka CHECK (uneko_stocka >= 0),
    CONSTRAINT chk_lehengai_gutxieneko_stocka CHECK (gutxieneko_stocka >= 0),
    CONSTRAINT chk_lehengai_kostu_unitarioa CHECK (kostu_unitarioa >= 0),
    CONSTRAINT fk_lehengai_hornitzailea FOREIGN KEY (hornitzailea_id)
        REFERENCES hornitzaileak(id) ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_lehengai_stock (uneko_stocka, gutxieneko_stocka, ezabatua),
    INDEX idx_lehengai_hornitzailea (hornitzailea_id)
) ENGINE=InnoDB;

-- 1.2 PLATERAK ETA EDARIAK (KARTA / MENUA - 14 KATEGORIA)
CREATE TABLE platerak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    izena VARCHAR(100) NOT NULL,
    deskribapena TEXT NULL,
    kategoria ENUM(
        'Sarrerakoa', 'Lehena', 'Bigarrena', 'Postrea',
        'Urak', 'Freskagarriak', 'Garagardoak', 'Sagardoak', 'Ardoak',
        'Kafeak', 'Infusioak', 'Likoreak', 'Konbinatuak', 'Koktailak'
    ) NOT NULL,
    prezioa DECIMAL(6,2) NOT NULL,
    alergenoak VARCHAR(200) NULL,
    irudi_erreferentzia VARCHAR(255) NULL,
    aktibo TINYINT(1) NOT NULL DEFAULT 1,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT chk_plater_prezioa CHECK (prezioa >= 0),
    INDEX idx_plater_kategoria (kategoria, ezabatua)
) ENGINE=InnoDB;

-- 1.3 PLATER_OSAGAIAK (ERREZETA / ESKANDALLOA - N:M GAKO KONPOSATUA)
CREATE TABLE plater_osagaiak (
    platera_id INT NOT NULL,
    lehengaia_id INT NOT NULL,
    kantitatea DECIMAL(10,3) NOT NULL,

    PRIMARY KEY (platera_id, lehengaia_id),
    CONSTRAINT chk_plater_osagai_kantitatea CHECK (kantitatea > 0),
    CONSTRAINT fk_plater_osagai_platera FOREIGN KEY (platera_id)
        REFERENCES platerak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_plater_osagai_lehengaia FOREIGN KEY (lehengaia_id)
        REFERENCES lehengaiak(id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB;

-- =====================================================================
-- 2. ARETOA, BEZEROAK ETA GIZA BALIABIDEAK (HR)
-- =====================================================================

-- 2.1 MAHAIAK
CREATE TABLE mahaiak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    zenbakia INT NOT NULL UNIQUE,
    edukiera INT NOT NULL DEFAULT 4,
    kokapena ENUM('Barran', 'Barra', 'Jantoki Nagusia', 'Terraza', 'VIP') NOT NULL DEFAULT 'Jantoki Nagusia',
    egoera ENUM('Libre', 'Okupatuta', 'Erreserbatuta', 'Garbitzen') NOT NULL DEFAULT 'Libre',
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT chk_mahai_zenbakia CHECK (zenbakia > 0),
    CONSTRAINT chk_mahai_edukiera CHECK (edukiera > 0),
    INDEX idx_mahai_egoera (egoera, ezabatua)
) ENGINE=InnoDB;

-- 2.2 BEZEROAK
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

-- 2.3 LANGILEAK (BCrypt pasahitzekin eta Odoo HR loturarekin)
CREATE TABLE langileak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    erabiltzailea VARCHAR(50) NOT NULL UNIQUE,
    pasahitza_hash VARCHAR(255) NOT NULL,
    izena VARCHAR(80) NOT NULL,
    abizena VARCHAR(100) NOT NULL DEFAULT '',
    nan_dni VARCHAR(20) NOT NULL UNIQUE,
    emaila VARCHAR(120) NOT NULL UNIQUE,
    telefonoa VARCHAR(20) NULL,
    rola ENUM('Zerbitzaria', 'Sukaldaria', 'Barrakoa', 'Biltegikoa', 'Kudeatzailea', 'AdminIT', 'Admin') NOT NULL DEFAULT 'Zerbitzaria',
    saila ENUM('Jantokia', 'Sukaldea', 'Barra', 'Biltegia', 'Sistemak_IT', 'Zuzendaritza') NOT NULL DEFAULT 'Jantokia',
    kargua VARCHAR(100) NOT NULL DEFAULT 'Langilea',
    odoo_employee_id INT NULL,
    aktibo TINYINT(1) NOT NULL DEFAULT 1,
    txata_aktibo TINYINT(1) NOT NULL DEFAULT 1,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_langile_rola (rola, aktibo),
    INDEX idx_langile_saila (saila)
) ENGINE=InnoDB;

-- 2.3.1 BARNE TXAT MEZUAK (GERENTE <-> TPV KOMUNIKAZIOA)
CREATE TABLE txat_mezuak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    langilea_id INT NOT NULL,
    gela VARCHAR(40) NOT NULL DEFAULT 'Orokorra',
    testua TEXT NOT NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_txat_mezu_langilea FOREIGN KEY (langilea_id)
        REFERENCES langileak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_txat_gela_data (gela, sortze_data)
) ENGINE=InnoDB;

-- 2.4 LANGILEEN KONTRATUAK (HR)
CREATE TABLE langile_kontratuak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    langilea_id INT NOT NULL,
    kontratu_zenbakia VARCHAR(50) NOT NULL UNIQUE,
    kontratu_mota ENUM('Mugagabea', 'Denboraldikoa', 'Praktikak', 'Formakuntza') NOT NULL DEFAULT 'Mugagabea',
    lanaldia ENUM('Osoa_40h', 'Partziala_30h', 'Partziala_20h') NOT NULL DEFAULT 'Osoa_40h',
    hasiera_data DATE NOT NULL,
    amaiera_data DATE NULL,
    hileroko_soldata_brutoa DECIMAL(8,2) NOT NULL,
    kategoria_profesionala VARCHAR(100) NOT NULL,
    egoera ENUM('Aktiboa', 'Amaitua', 'Etenda') NOT NULL DEFAULT 'Aktiboa',
    oharrak VARCHAR(255) NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_kontratua_langilea FOREIGN KEY (langilea_id)
        REFERENCES langileak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_kontratu_langilea (langilea_id, egoera)
) ENGINE=InnoDB;

-- 2.5 LANGILEEN FITXAKETAK (HR)
CREATE TABLE langile_fitxaketak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    langilea_id INT NOT NULL,
    sarrera_data DATETIME NOT NULL,
    irteera_data DATETIME NULL,
    lan_orduak DECIMAL(5,2) NULL,
    metodoa ENUM('PIN', 'QR', 'RFID', 'Aplikazioa', 'Eskuzkoa') NOT NULL DEFAULT 'Aplikazioa',
    egoera ENUM('Zuzena', 'Bete_gabea', 'Zuzendua') NOT NULL DEFAULT 'Zuzena',
    oharrak VARCHAR(255) NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_fitxaketa_langilea FOREIGN KEY (langilea_id)
        REFERENCES langileak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_fitxaketa_data (langilea_id, sarrera_data)
) ENGINE=InnoDB;

-- 2.6 LANGILEEN BAJAK ETA OPORRAK (HR)
CREATE TABLE langile_bajak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    langilea_id INT NOT NULL,
    mota ENUM('Gaixotasun_Arrunta', 'Lan_Istripua', 'Amatasun_Aitatasun', 'Oporrak', 'Baimen_Ordaindua', 'Baimen_Ordaindugabea') NOT NULL,
    hasiera_data DATE NOT NULL,
    amaiera_data DATE NULL,
    egun_kopurua INT NOT NULL DEFAULT 1,
    egoera ENUM('Eskatua', 'Onartua', 'Ukatua', 'Bukatua') NOT NULL DEFAULT 'Eskatua',
    justifikazio_agiria VARCHAR(255) NULL,
    oharrak TEXT NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_baja_langilea FOREIGN KEY (langilea_id)
        REFERENCES langileak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_baja_langilea (langilea_id, egoera)
) ENGINE=InnoDB;

-- =====================================================================
-- 3. ERAGIKETAK: ERRESERBAK, ESKAERAK, STOCK AUDITORIA ETA IRITZIAK
-- =====================================================================

-- 3.1 ERRESERBAK (UNIQUE gakoak erreserba bikoitzak eragozten ditu)
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

-- 3.2 ESKAERAK (KOMANDAK)
CREATE TABLE eskaerak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    mahaia_id INT NOT NULL,
    langilea_id INT NOT NULL,
    egoera ENUM('Irekia', 'Sukaldean', 'Prestatua', 'Zerbitzatua', 'Kobratua', 'Ezeztatua') NOT NULL DEFAULT 'Irekia',
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

-- 3.3 ESKAERA LERROAK (unitate_prezioa historikoa gordetzen du)
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

-- 3.4 STOCK MUGIMENDUAK (AUDITORIA ETA TRAZABILITATEA)
CREATE TABLE stock_mugimenduak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    lehengaia_id INT NOT NULL,
    mota ENUM('Sarrera_Albarana', 'Kontsumoa_Platera', 'Egokitzapena_Inbentarioa') NOT NULL,
    kantitatea DECIMAL(10,3) NOT NULL,
    eskaera_id INT NULL,
    albaran_zenbakia VARCHAR(50) NULL,
    faktura_id INT NULL,
    oharrak VARCHAR(200) NULL,
    data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT chk_mugimendu_kantitatea CHECK (kantitatea <> 0),
    CONSTRAINT fk_mugimendu_lehengaia FOREIGN KEY (lehengaia_id)
        REFERENCES lehengaiak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_mugimendu_eskaera FOREIGN KEY (eskaera_id)
        REFERENCES eskaerak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_mugimendu_faktura (faktura_id)
) ENGINE=InnoDB;

-- 3.5 IRITZIAK (BEZEROEN BALORAZIOAK, 0-5 IZAR)
CREATE TABLE iritziak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    bezeroa_id INT NOT NULL,
    balorazioa TINYINT NOT NULL,
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
-- 4. FINANTZAK: ORDAINKETAK (TPV TIKETAK) ETA FAKTURAK
-- =====================================================================

-- 4.1 ORDAINKETAK ETA TIKET FISKALAK (PCI-DSS maskaratua + token zifratua)
CREATE TABLE ordainketak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    eskaera_id INT NOT NULL,
    tiket_zenbakia VARCHAR(50) NOT NULL UNIQUE,
    mota ENUM('Eskudirua', 'Txartela', 'Bizum', 'GooglePay', 'ApplePay') NOT NULL,
    zenbatekoa DECIMAL(8,2) NOT NULL,
    jasotakoa DECIMAL(8,2) NULL,
    itzulitakoa DECIMAL(8,2) NULL,
    txartel_maskaratua VARCHAR(30) NULL,
    txartel_marka VARCHAR(30) NULL,
    operazio_kodea VARCHAR(60) NOT NULL,
    token_zifratua VARCHAR(120) NULL,
    egoera ENUM('Arrakastatsua', 'Ezeztatua', 'Itzulita') NOT NULL DEFAULT 'Arrakastatsua',
    langilea_id INT NULL,
    tiket_testua MEDIUMTEXT NULL,
    data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT chk_ordainketa_zenbatekoa CHECK (zenbatekoa > 0),
    CONSTRAINT fk_ordainketak_eskaerak FOREIGN KEY (eskaera_id)
        REFERENCES eskaerak(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_ordainketa_eskaera (eskaera_id),
    INDEX idx_ordainketa_tiket (tiket_zenbakia)
) ENGINE=InnoDB;

-- 4.2 FAKTURAK (SALMENTA ETA HORNITZAILEEN EROSKETA / GASTU FAKTURAK)
CREATE TABLE fakturak (
    id INT AUTO_INCREMENT PRIMARY KEY,
    eskaera_id INT NULL,
    hornitzailea_id INT NULL,
    faktura_zenbakia VARCHAR(50) NOT NULL UNIQUE,
    mota ENUM('Salmenta', 'Erosketa') NOT NULL DEFAULT 'Salmenta',
    azpimota ENUM('Sinplifikatua', 'Osoa') NOT NULL DEFAULT 'Sinplifikatua',
    kontzeptua VARCHAR(255) NOT NULL,
    hornitzailea VARCHAR(120) NULL,
    hornitzaile_cif VARCHAR(20) NULL,
    bezero_nif VARCHAR(20) NULL,
    bezero_izena VARCHAR(120) NULL,
    bezero_helbidea VARCHAR(200) NULL,
    oinarri_zergagarria DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    bez_ehunekoa DECIMAL(4,2) NOT NULL DEFAULT 10.00,
    bez_zenbatekoa DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    guztira DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    ordainketa_metodoa VARCHAR(50) NOT NULL DEFAULT 'Txartela',
    egoera ENUM('Ordaindua', 'Zain', 'Ezeztatua') NOT NULL DEFAULT 'Ordaindua',
    data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    epemuga_data DATE NULL,
    oharrak TEXT NULL,
    ezabatua TINYINT(1) NOT NULL DEFAULT 0,
    ezabatze_data DATETIME NULL,
    sortze_data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_faktura_eskaera FOREIGN KEY (eskaera_id)
        REFERENCES eskaerak(id) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT fk_faktura_hornitzailea FOREIGN KEY (hornitzailea_id)
        REFERENCES hornitzaileak(id) ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_faktura_mota (mota, egoera, data),
    INDEX idx_faktura_hornitzailea (hornitzailea_id),
    INDEX idx_faktura_zenbakia (faktura_zenbakia),
    INDEX idx_faktura_data (data)
) ENGINE=InnoDB;

ALTER TABLE stock_mugimenduak
    ADD CONSTRAINT fk_mugimendu_faktura FOREIGN KEY (faktura_id)
        REFERENCES fakturak(id) ON DELETE SET NULL ON UPDATE CASCADE;

-- =====================================================================
-- 5. IKUSPEGIAK (VIEWS) — STOCK ETA BALORAZIO KALKULU DINAMIKOAK
-- =====================================================================

-- 5.1 Erosi beharreko lehengaiak (stocka gutxienekoaren azpitik edo berdin)
CREATE OR REPLACE VIEW v_erosi_beharreko_lehengaiak AS
SELECT
    l.id AS lehengaia_id,
    l.izena,
    l.unitatea,
    l.uneko_stocka,
    l.gutxieneko_stocka,
    (l.gutxieneko_stocka * 2 - l.uneko_stocka) AS proposatutako_erosketa,
    COALESCE(h.izena, l.hornitzailea) AS hornitzailea
FROM lehengaiak l
LEFT JOIN hornitzaileak h ON l.hornitzailea_id = h.id
WHERE l.uneko_stocka <= l.gutxieneko_stocka AND l.ezabatua = 0;

-- 5.2 Plater bakoitzaren ekoizpen-ahalmena (osagai mugatzailearen arabera)
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

-- 5.3 Iritzien estatistika orokorrak (bataz bestekoa eta izar banaketa)
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
