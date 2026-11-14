-- =====================================================================
-- 1T JATETXEA - HR ETA LANGILEEN EGITURA EGUNERATZEA (MySQL 8.0)
-- Egileak: Ander Urien, Julen Aguado, Iker Araujo, Aratz Huegun
-- Xedea: Langileen taula zabaltzea, Odoo ERP-arekiko zutabeak eta
--        kontratu, fitxaketa eta bajen taulak sortzea.
-- =====================================================================

USE jatetxea_db;

-- 1. Langileak taulari zutabe berriak gehitzea (lehendik badago)
-- Kontuan hartuta MySQL-k ez duela 'IF NOT EXISTS' zutabeetan modu zuzenean, prozedura segurua erabiltzen dugu:
DROP PROCEDURE IF EXISTS EguneratuLangileakTaula;

DELIMITER $$
CREATE PROCEDURE EguneratuLangileakTaula()
BEGIN
    -- abizena
    IF NOT EXISTS (SELECT * FROM information_schema.COLUMNS WHERE TABLE_SCHEMA='jatetxea_db' AND TABLE_NAME='langileak' AND COLUMN_NAME='abizena') THEN
        ALTER TABLE langileak ADD COLUMN abizena VARCHAR(100) NOT NULL DEFAULT '' AFTER izena;
    END IF;

    -- nan_dni
    IF NOT EXISTS (SELECT * FROM information_schema.COLUMNS WHERE TABLE_SCHEMA='jatetxea_db' AND TABLE_NAME='langileak' AND COLUMN_NAME='nan_dni') THEN
        ALTER TABLE langileak ADD COLUMN nan_dni VARCHAR(20) NULL UNIQUE AFTER abizena;
    END IF;

    -- emaila
    IF NOT EXISTS (SELECT * FROM information_schema.COLUMNS WHERE TABLE_SCHEMA='jatetxea_db' AND TABLE_NAME='langileak' AND COLUMN_NAME='emaila') THEN
        ALTER TABLE langileak ADD COLUMN emaila VARCHAR(120) NULL UNIQUE AFTER nan_dni;
    END IF;

    -- telefonoa
    IF NOT EXISTS (SELECT * FROM information_schema.COLUMNS WHERE TABLE_SCHEMA='jatetxea_db' AND TABLE_NAME='langileak' AND COLUMN_NAME='telefonoa') THEN
        ALTER TABLE langileak ADD COLUMN telefonoa VARCHAR(20) NULL AFTER emaila;
    END IF;

    -- rola ENUM eguneratu
    ALTER TABLE langileak MODIFY COLUMN rola ENUM('Zerbitzaria', 'Sukaldaria', 'Barrakoa', 'Biltegikoa', 'Kudeatzailea', 'AdminIT', 'Admin') NOT NULL DEFAULT 'Zerbitzaria';

    -- saila
    IF NOT EXISTS (SELECT * FROM information_schema.COLUMNS WHERE TABLE_SCHEMA='jatetxea_db' AND TABLE_NAME='langileak' AND COLUMN_NAME='saila') THEN
        ALTER TABLE langileak ADD COLUMN saila ENUM('Jantokia', 'Sukaldea', 'Barra', 'Biltegia', 'Sistemak_IT', 'Zuzendaritza') NOT NULL DEFAULT 'Jantokia' AFTER rola;
    END IF;

    -- kargua
    IF NOT EXISTS (SELECT * FROM information_schema.COLUMNS WHERE TABLE_SCHEMA='jatetxea_db' AND TABLE_NAME='langileak' AND COLUMN_NAME='kargua') THEN
        ALTER TABLE langileak ADD COLUMN kargua VARCHAR(100) NOT NULL DEFAULT 'Langilea' AFTER saila;
    END IF;

    -- odoo_employee_id
    IF NOT EXISTS (SELECT * FROM information_schema.COLUMNS WHERE TABLE_SCHEMA='jatetxea_db' AND TABLE_NAME='langileak' AND COLUMN_NAME='odoo_employee_id') THEN
        ALTER TABLE langileak ADD COLUMN odoo_employee_id INT NULL AFTER kargua;
    END IF;
END$$
DELIMITER ;

CALL EguneratuLangileakTaula();
DROP PROCEDURE IF EXISTS EguneratuLangileakTaula;

-- 2. Kontratuak Taula
CREATE TABLE IF NOT EXISTS langile_kontratuak (
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

-- 3. Fitxaketak Taula
CREATE TABLE IF NOT EXISTS langile_fitxaketak (
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

-- 4. Bajak eta Oporrak Taula
CREATE TABLE IF NOT EXISTS langile_bajak (
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

-- 5. Langile berriak txertatzea edo eguneratzea (ON DUPLICATE KEY UPDATE)
INSERT INTO langileak (erabiltzailea, pasahitza_hash, izena, abizena, nan_dni, emaila, telefonoa, rola, saila, kargua) VALUES
('anderu', '$2a$11$eXampleHashedPasswordAdmin', 'Ander', 'Urien Telleria', '72548901A', 'ander.urien@goierrieskola.eus', '688112233', 'AdminIT', 'Sistemak_IT', 'IT Lead / Arkitekto Nagusia'),
('julen', '$2a$11$eXampleHashedPasswordAdmin', 'Julen', 'Aguado Fernández', '72548902B', 'julen.aguado@goierrieskola.eus', '688112234', 'AdminIT', 'Sistemak_IT', 'Sare eta Suebaki Ingeniaria'),
('iker', '$2a$11$eXampleHashedPasswordAdmin', 'Iker', 'Araujo Dominguez', '72548903C', 'iker.araujo@goierrieskola.eus', '688112235', 'AdminIT', 'Sistemak_IT', 'Sistema Eragileak eta Birtualizazioa'),
('aratz', '$2a$11$eXampleHashedPasswordAdmin', 'Aratz', 'Huegun Flores', '72548904D', 'aratz.huegun@goierrieskola.eus', '688112236', 'AdminIT', 'Sistemak_IT', 'Zibersegurtasun eta Auditoritza Burua'),
('ainhoa_chef', '$2a$11$eXampleHashedPassword2', 'Ainhoa', 'Goikoetxea', '72548905E', 'ainhoa.goikoetxea@jatetxea.eus', '677112233', 'Sukaldaria', 'Sukaldea', 'Chef Burua / Sukaldari Nagusia'),
('eneko_suk', '$2a$11$eXampleHashedPassword2', 'Eneko', 'Otegi', '72548906F', 'eneko.otegi@jatetxea.eus', '677112234', 'Sukaldaria', 'Sukaldea', 'Bigarren Sukaldaria / Parrilleroa'),
('mikel_zerb', '$2a$11$eXampleHashedPassword1', 'Mikel', 'Arregi', '72548907G', 'mikel.arregi@jatetxea.eus', '666112233', 'Zerbitzaria', 'Jantokia', 'Maître / Zerbitzari Burua'),
('maite_zerb', '$2a$11$eXampleHashedPassword1', 'Maite', 'Zabaleta', '72548908H', 'maite.zabaleta@jatetxea.eus', '666112234', 'Zerbitzaria', 'Jantokia', 'Jantokiko Zerbitzaria'),
('unai_zerb', '$2a$11$eXampleHashedPassword1', 'Unai', 'Lasa', '72548909J', 'unai.lasa@jatetxea.eus', '666112235', 'Zerbitzaria', 'Jantokia', 'Jantokiko Zerbitzaria'),
('gorka_barra', '$2a$11$eXampleHashedPassword1', 'Gorka', 'Mendizabal', '72548910K', 'gorka.mendizabal@jatetxea.eus', '655112233', 'Barrakoa', 'Barra', 'Barman / Barrako Arduraduna'),
('uxue_barra', '$2a$11$eXampleHashedPassword1', 'Uxue', 'Etxarri', '72548911L', 'uxue.etxarri@jatetxea.eus', '655112234', 'Barrakoa', 'Barra', 'Barrako Zerbitzaria'),
('kepa_biltegi', '$2a$11$eXampleHashedPassword1', 'Kepa', 'Aldasoro', '72548912M', 'kepa.aldasoro@jatetxea.eus', '644112233', 'Biltegikoa', 'Biltegia', 'Biltegi eta Stock Burua'),
('ander_m_zuz', '$2a$11$eXampleHashedPassword3', 'Ander', 'Madinabeitia', '72548913N', 'ander.madinabeitia@jatetxea.eus', '633112233', 'Kudeatzailea', 'Zuzendaritza', 'Zuzendaritza Kontseilukoa / Zuzendaria'),
('joseba_r_zuz', '$2a$11$eXampleHashedPassword3', 'Joseba', 'Ramos', '72548914P', 'joseba.ramos@jatetxea.eus', '633112234', 'Kudeatzailea', 'Zuzendaritza', 'Zuzendaritza Kontseilukoa / Zuzendaria'),
('estebe_o_zuz', '$2a$11$eXampleHashedPassword3', 'Estebe', 'Ormazabal', '72548915Q', 'estebe.ormazabal@jatetxea.eus', '633112235', 'Kudeatzailea', 'Zuzendaritza', 'Zuzendaritza Kontseilukoa / Zuzendaria')
ON DUPLICATE KEY UPDATE 
    izena = VALUES(izena),
    abizena = VALUES(abizena),
    nan_dni = VALUES(nan_dni),
    emaila = VALUES(emaila),
    telefonoa = VALUES(telefonoa),
    rola = VALUES(rola),
    saila = VALUES(saila),
    kargua = VALUES(kargua);
