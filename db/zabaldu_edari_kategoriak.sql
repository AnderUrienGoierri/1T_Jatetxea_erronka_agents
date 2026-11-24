-- =====================================================================
-- BARRAKO EDARI KATEGORIA ZEHATZAK ETA PRODUKTU BERRIAK
-- 1T Jatetxea (Osteria & Parrilla) - MySQL 8.0 UTF-8 (utf8mb4)
-- 10 kategoria zehatz: Urak, Freskagarriak, Garagardoak, Sagardoak,
-- Ardoak, Kafeak, Infusioak, Likoreak, Konbinatuak, Koktailak
-- Idempotentea: hainbat aldiz exekuta daiteke bikoizturik sortu gabe.
-- =====================================================================

SET NAMES utf8mb4;
USE jatetxea_db;

-- 1. BEHIN-BEHINEKO ENUM ZABALKUNTZA (datu zaharrak migratu ahal izateko)
ALTER TABLE platerak
    MODIFY COLUMN kategoria ENUM(
        'Sarrerakoa', 'Lehena', 'Bigarrena', 'Postrea', 'Edaria',
        'Urak', 'Freskagarriak', 'Garagardoak', 'Sagardoak', 'Ardoak',
        'Kafeak', 'Infusioak', 'Likoreak', 'Konbinatuak', 'Koktailak'
    ) NOT NULL;

-- 2. LEHENGAIAK: BARRAKO OSAGAI BERRIAK TXERTATU
INSERT INTO lehengaiak (izena, unitatea, uneko_stocka, gutxieneko_stocka, kostu_unitarioa, hornitzailea) VALUES
('San Pellegrino Ur Botila 75cl', 'unitatea', 40.000, 12.000, 1.30, 'Italiar Inportazioak'),
('Alzola Ur Botila 50cl', 'unitatea', 90.000, 25.000, 0.45, 'Alzola Urak'),
('Sagardo Ekologiko Botila', 'unitatea', 42.000, 12.000, 3.10, 'Bereziartua Sagardotegia'),
('Sagardo Aparduna Botila', 'unitatea', 24.000, 8.000, 5.20, 'Gipuzkoako Sagardotegiak'),
('Chianti Classico Botila', 'l', 24.000, 6.000, 14.00, 'Toscana Upategiak'),
('Prosecco Valdobbiadene Botila', 'l', 18.000, 5.000, 11.50, 'Veneto Upategiak'),
('Tonika Premium Botila', 'unitatea', 60.000, 20.000, 1.10, 'Edari Banatzailea'),
('Limoia eta Menta Freskoa', 'kg', 12.000, 3.000, 3.20, 'Barazki Koop'),
('Mosto Gorria Botila', 'unitatea', 45.000, 15.000, 0.95, 'Errioxako Upategiak'),
('Peroni Garagardo Botila', 'unitatea', 48.000, 15.000, 1.35, 'Italiar Inportazioak'),
('Infusio Poltsak (Kamamila/Teak)', 'unitatea', 150.000, 40.000, 0.25, 'Te eta Infusioak'),
('Limoncello Botila', 'l', 8.000, 2.000, 14.50, 'Sorrento Likoreak'),
('Amaretto Botila', 'l', 6.000, 2.000, 16.00, 'Italiar Inportazioak'),
('Orujo Belar Botila', 'l', 8.000, 2.000, 11.00, 'Galiziako Destilategia'),
('Aperol Botila', 'l', 12.000, 3.000, 13.50, 'Italiar Inportazioak'),
('Ginebra Premium eta Vermut', 'l', 14.000, 4.000, 22.00, 'Likore Banatzailea'),
('Ron Zuria eta Zaharra Botila', 'l', 12.000, 3.000, 16.00, 'Likore Banatzailea'),
('Whisky eta Vodka Botila', 'l', 12.000, 3.000, 18.50, 'Likore Banatzailea')
ON DUPLICATE KEY UPDATE uneko_stocka = GREATEST(uneko_stocka, VALUES(uneko_stocka));

-- 3. LEHENDIK ZEUDEN PRODUKTUAK KATEGORIA ZEHATZETARA BIRKATEGORIZATU
UPDATE platerak SET izena = 'Garagardo Kaña Zirriborroa (33cl)' WHERE id = 27;

UPDATE platerak SET kategoria = 'Urak'
WHERE izena IN ('Ur Mineral Naturala (1L)', 'Ur Mineral Gasduna (50cl)');

UPDATE platerak SET kategoria = 'Sagardoak'
WHERE izena IN ('Euskal Sagardo Naturala (75cl)');

UPDATE platerak SET kategoria = 'Ardoak'
WHERE izena IN (
    'Getariako Txakolina D.O. (75cl)',
    'Errioxako Ardo Beltza Crianza D.O. (75cl)',
    'Errioxako Ardo Beltza Erreserba D.O. (75cl)',
    'Nafarroako Ardo Gorria D.O. (75cl)',
    'Etxeko Ardoa Pitxerra (1L)'
);

UPDATE platerak SET kategoria = 'Freskagarriak'
WHERE izena IN (
    'Freskagarria (Kola, Laranja, Limoia 33cl)',
    'Etxeko Sagar Zuku Ekologikoa (25cl)'
);

UPDATE platerak SET kategoria = 'Garagardoak'
WHERE izena IN (
    'Eskulangile Garagardoa Basqueland (33cl)',
    'Garagardo Kaña Zirriborroa (33cl)',
    'Garagardo Zuritoa (20cl)',
    'Garagardo 0,0% Alkohol Gabea (33cl)'
);

UPDATE platerak SET kategoria = 'Kafeak'
WHERE izena IN ('Kafe Ebakia / Esnearekin');

UPDATE platerak SET kategoria = 'Likoreak'
WHERE izena IN ('Patxaran Tradizionala (Kopa)');

UPDATE platerak SET kategoria = 'Konbinatuak'
WHERE izena IN ('Gin Tonic Premium');

-- 4. BARRAKO PRODUKTU BERRIAK TXERTATU (bikoiztu gabe)
INSERT INTO platerak (izena, deskribapena, kategoria, prezioa, alergenoak)
SELECT * FROM (
    SELECT 'San Pellegrino Ur Gasduna (75cl)' AS izena, 'Italiako ur gasdun fina, mahairako bikaina' AS deskribapena, 'Urak' AS kategoria, 3.60 AS prezioa, NULL AS alergenoak
    UNION ALL SELECT 'Alzola Ur Botilatxoa (50cl)', 'Euskal iturriko ur mineral naturala', 'Urak', 1.80, NULL
    UNION ALL SELECT 'Bereziartua Sagardo Ekologikoa (75cl)', 'Euskal Herriko sagar ekologiko hautatuekin egindako sagardoa', 'Sagardoak', 5.80, 'Sulfitoak'
    UNION ALL SELECT 'Sagardo Aparduna Extra (75cl)', 'Metodo tradizionalean bigarren hartziduradun euskal sagardoa', 'Sagardoak', 12.50, 'Sulfitoak'
    UNION ALL SELECT 'Chianti Classico D.O.C.G. (Kopa)', 'Toscana eskualdeko Sangiovese ardo beltz dotorea', 'Ardoak', 4.80, 'Sulfitoak'
    UNION ALL SELECT 'Prosecco Valdobbiadene (Kopa)', 'Italiar apardun freskoa aperitiborako', 'Ardoak', 4.50, 'Sulfitoak'
    UNION ALL SELECT 'Tonika Premium (20cl)', 'Botila hotza limoi-azalarekin zerbitzatua', 'Freskagarriak', 2.80, NULL
    UNION ALL SELECT 'Limonada Etxekoa Menta Freskoarekin', 'Limoi zukua, menta hostoak eta izotza', 'Freskagarriak', 3.40, NULL
    UNION ALL SELECT 'Mosto Gorria (20cl)', 'Mahats zuku naturala, alkoholik gabea', 'Freskagarriak', 2.30, 'Sulfitoak'
    UNION ALL SELECT 'Peroni Nastro Azzurro (33cl)', 'Italiako lager garagardo leun eta freskagarria', 'Garagardoak', 3.50, 'Glutena'
    UNION ALL SELECT 'Espresso Italiarra', 'Aroma intentsuko espresso laburra, krema lodiarekin', 'Kafeak', 1.60, NULL
    UNION ALL SELECT 'Cappuccino Krematsua', 'Espressoa, esne aparra eta kakao ukitua', 'Kafeak', 2.50, 'Laktosa'
    UNION ALL SELECT 'Kafe Amerikanoa', 'Espresso luzatua ur beroarekin', 'Kafeak', 1.90, NULL
    UNION ALL SELECT 'Carajillo Patxaranarekin', 'Kafe beroa Nafarroako patxaran ukituarekin', 'Kafeak', 2.90, NULL
    UNION ALL SELECT 'Kamamila Infusio Naturala', 'Lasaigarria eta digestiboa bazkalondoan', 'Infusioak', 2.00, NULL
    UNION ALL SELECT 'Te Berdea Menta Freskoarekin', 'Te berde ekologikoa menta ukitu freskoarekin', 'Infusioak', 2.20, NULL
    UNION ALL SELECT 'Te Beltza Earl Grey', 'Bergamota usaineko te beltz klasikoa', 'Infusioak', 2.20, NULL
    UNION ALL SELECT 'Poleo-Menta Digestiboa', 'Infusio freskoa eta arina', 'Infusioak', 2.00, NULL
    UNION ALL SELECT 'Limoncello Etxekoa (Txupitoa)', 'Sorrento erara egindako limoi likore izoztua', 'Likoreak', 3.50, NULL
    UNION ALL SELECT 'Amaretto di Saronno (Kopa)', 'Almendra gozozko italiar likorea izotzarekin', 'Likoreak', 4.50, 'Fruitu lehorrak'
    UNION ALL SELECT 'Orujo Belar Likorea (Txupitoa)', 'Belar aromatikoekin mazeratutako orujoa', 'Likoreak', 3.20, NULL
    UNION ALL SELECT 'Gin Tonic Premium', 'Ginebra aromatikoa, tonika premium eta limoi azala', 'Konbinatuak', 9.00, NULL
    UNION ALL SELECT 'Ron-Kola (Barceló / Brugal)', 'Ron zaharra, kola freskagarria eta lima xerra', 'Konbinatuak', 7.50, NULL
    UNION ALL SELECT 'Whisky-Kola / Ginger Ale', 'Whisky klasikoa izotzarekin eta aukeratutako freskagarriarekin', 'Konbinatuak', 8.00, NULL
    UNION ALL SELECT 'Vodka-Limoi / Laranja', 'Vodka purua limoi edo laranja freskagarriarekin', 'Konbinatuak', 7.50, NULL
    UNION ALL SELECT 'Aperol Spritz Italiarra', 'Prosecco, Aperol, soda eta laranja xerra', 'Koktailak', 7.50, 'Sulfitoak'
    UNION ALL SELECT 'Negroni Klasikoa', 'Ginebra, Campari/Aperol eta vermut gorria', 'Koktailak', 8.50, 'Sulfitoak'
    UNION ALL SELECT 'Mojito Klasikoa', 'Ron zuria, lima, menta freskoa eta soda', 'Koktailak', 8.00, NULL
    UNION ALL SELECT 'Espresso Martini', 'Vodka, espresso italiarra eta kafe likorea', 'Koktailak', 9.50, NULL
) AS berriak
WHERE NOT EXISTS (
    SELECT 1 FROM platerak p WHERE p.izena = berriak.izena AND p.ezabatua = 0
);

-- 5. KENDU 'Edaria' GENERIKOA ENUM-ETIK (jada produktu guztiek kategoria zehatza dute)
ALTER TABLE platerak
    MODIFY COLUMN kategoria ENUM(
        'Sarrerakoa', 'Lehena', 'Bigarrena', 'Postrea',
        'Urak', 'Freskagarriak', 'Garagardoak', 'Sagardoak', 'Ardoak',
        'Kafeak', 'Infusioak', 'Likoreak', 'Konbinatuak', 'Koktailak'
    ) NOT NULL;

-- 6. PRODUKTU BERRIEN ERREZETAK (STOCK KALKULURAKO v_plateren_stock_ahalmena)
INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p JOIN lehengaiak l ON l.izena = 'San Pellegrino Ur Botila 75cl' WHERE p.izena = 'San Pellegrino Ur Gasduna (75cl)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p JOIN lehengaiak l ON l.izena = 'Alzola Ur Botila 50cl' WHERE p.izena = 'Alzola Ur Botilatxoa (50cl)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p JOIN lehengaiak l ON l.izena = 'Sagardo Ekologiko Botila' WHERE p.izena = 'Bereziartua Sagardo Ekologikoa (75cl)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p JOIN lehengaiak l ON l.izena = 'Sagardo Aparduna Botila' WHERE p.izena = 'Sagardo Aparduna Extra (75cl)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.150 FROM platerak p JOIN lehengaiak l ON l.izena = 'Chianti Classico Botila' WHERE p.izena = 'Chianti Classico D.O.C.G. (Kopa)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.150 FROM platerak p JOIN lehengaiak l ON l.izena = 'Prosecco Valdobbiadene Botila' WHERE p.izena = 'Prosecco Valdobbiadene (Kopa)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p JOIN lehengaiak l ON l.izena = 'Tonika Premium Botila' WHERE p.izena = 'Tonika Premium (20cl)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.120 FROM platerak p JOIN lehengaiak l ON l.izena = 'Limoia eta Menta Freskoa' WHERE p.izena = 'Limonada Etxekoa Menta Freskoarekin';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p JOIN lehengaiak l ON l.izena = 'Mosto Gorria Botila' WHERE p.izena = 'Mosto Gorria (20cl)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p JOIN lehengaiak l ON l.izena = 'Peroni Garagardo Botila' WHERE p.izena = 'Peroni Nastro Azzurro (33cl)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.010 FROM platerak p JOIN lehengaiak l ON l.izena = 'Kafe Aleak' WHERE p.izena IN ('Espresso Italiarra', 'Cappuccino Krematsua', 'Kafe Amerikanoa', 'Carajillo Patxaranarekin', 'Espresso Martini');

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.120 FROM platerak p JOIN lehengaiak l ON l.izena = 'Esnea' WHERE p.izena = 'Cappuccino Krematsua';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.030 FROM platerak p JOIN lehengaiak l ON l.izena = 'Patxaran Botila' WHERE p.izena = 'Carajillo Patxaranarekin';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p JOIN lehengaiak l ON l.izena = 'Infusio Poltsak (Kamamila/Teak)' WHERE p.izena IN ('Kamamila Infusio Naturala', 'Te Berdea Menta Freskoarekin', 'Te Beltza Earl Grey', 'Poleo-Menta Digestiboa');

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p JOIN lehengaiak l ON l.izena = 'Limoncello Botila' WHERE p.izena = 'Limoncello Etxekoa (Txupitoa)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.060 FROM platerak p JOIN lehengaiak l ON l.izena = 'Amaretto Botila' WHERE p.izena = 'Amaretto di Saronno (Kopa)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p JOIN lehengaiak l ON l.izena = 'Orujo Belar Botila' WHERE p.izena = 'Orujo Belar Likorea (Txupitoa)';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.060 FROM platerak p JOIN lehengaiak l ON l.izena = 'Aperol Botila' WHERE p.izena = 'Aperol Spritz Italiarra';
INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.090 FROM platerak p JOIN lehengaiak l ON l.izena = 'Prosecco Valdobbiadene Botila' WHERE p.izena = 'Aperol Spritz Italiarra';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.060 FROM platerak p JOIN lehengaiak l ON l.izena = 'Ginebra Premium eta Vermut' WHERE p.izena IN ('Negroni Klasikoa', 'Gin Tonic Premium');
INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p JOIN lehengaiak l ON l.izena = 'Tonika Premium Botila' WHERE p.izena = 'Gin Tonic Premium';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p JOIN lehengaiak l ON l.izena = 'Ron Zuria eta Zaharra Botila' WHERE p.izena IN ('Mojito Klasikoa', 'Ron-Kola (Barceló / Brugal)');
INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.080 FROM platerak p JOIN lehengaiak l ON l.izena = 'Limoia eta Menta Freskoa' WHERE p.izena = 'Mojito Klasikoa';

INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 0.050 FROM platerak p JOIN lehengaiak l ON l.izena = 'Whisky eta Vodka Botila' WHERE p.izena IN ('Whisky-Kola / Ginger Ale', 'Vodka-Limoi / Laranja', 'Espresso Martini');
INSERT IGNORE INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p JOIN lehengaiak l ON l.izena = 'Freskagarri Lata' WHERE p.izena IN ('Ron-Kola (Barceló / Brugal)', 'Whisky-Kola / Ginger Ale', 'Vodka-Limoi / Laranja');
