-- =====================================================================
-- KARTA ZABALDUA ETA LEHENGAIEN EGUNERAKETA
-- Jatetxearen Kudeaketa Sistema (MySQL 8.0 InnoDB)
-- Arau profesionala: emotikonorik gabe
-- =====================================================================

USE jatetxea_db;

-- 1. LEHENGAI BERRIAK TXERTATU
INSERT INTO lehengaiak (izena, unitatea, uneko_stocka, gutxieneko_stocka, kostu_unitarioa, hornitzailea) VALUES
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
('Patxaran Botila', 'l', 10.000, 3.000, 12.00, 'Baines Patxarana')
ON DUPLICATE KEY UPDATE uneko_stocka = VALUES(uneko_stocka);

-- 2. PLATER BERRIAK TXERTATU

-- A. Sarrerakoak
INSERT INTO platerak (izena, deskribapena, kategoria, prezioa, alergenoak) VALUES
('Idiazabal Gazta Dastaketa', 'Bertako gazta ondua intxaurrekin eta sagardo membrilloarekin', 'Sarrerakoa', 12.00, 'Laktosa, Fruitu lehorrak'),
('Iberiar Urdaiazpiko Ezkur-jana', 'Ezkurrez elikatutako 100% iberiarra ogi txigortu eta tomatearekin', 'Sarrerakoa', 19.50, 'Glutena'),
('Txangurro Kroketa Etxekoak (8 un.)', 'Itsas zapore sakoneko kroketa krematsuak', 'Sarrerakoa', 11.50, 'Krustazeoak, Glutena, Laktosa'),
('Gildak eta Antxoak Olio Epelekin', 'Kantauriko antxoak, Ibarrako piparrak eta oliba berdeak', 'Sarrerakoa', 9.50, 'Arraina');

-- B. Lehen Platerak
INSERT INTO platerak (izena, deskribapena, kategoria, prezioa, alergenoak) VALUES
('Porrusalda Bakailao Miguekin', 'Tradizionala baratze-puerroekin eta bakailao mamitsuarekin', 'Lehena', 10.50, 'Arraina'),
('Sasoiko Barazki Menestra', 'Goierriko barazki freskoak birjina olioarekin erregosiak', 'Lehena', 12.50, NULL),
('Onddo eta Idiazabal Risotto Kremozua', 'Boletus edulis onddoak eta Idiazabal gazta urtua', 'Lehena', 15.00, 'Laktosa');

-- C. Bigarren Platerak
INSERT INTO platerak (izena, deskribapena, kategoria, prezioa, alergenoak) VALUES
('Legatz Errea Orio Erara', 'Parrillan errea baratxuri, gindilla eta sagardo ozpin epelarekin', 'Bigarrena', 22.00, 'Arraina'),
('Txipiroiak Beren Tintan', 'Kostaldeko errezetan prestatutako txipiroi freskoak arroza zuriarekin', 'Bigarrena', 19.00, 'Moluskuak'),
('Arkume Errea Labean', 'Euskal arkume errea patata panadera gozoekin', 'Bigarrena', 24.50, NULL),
('Euskal Txerri Solomoa Gazta Saltsarekin', 'Bertako txerri solomoa plantxan Idiazabal saltsa leunarekin', 'Bigarrena', 16.50, 'Laktosa');

-- D. Postreak
INSERT INTO platerak (izena, deskribapena, kategoria, prezioa, alergenoak) VALUES
('Goxua Tradizionala', 'Bizkotxo bustia, pastel-krema, esnegaina eta karamelo txigortua', 'Postrea', 5.50, 'Glutena, Arrautza, Laktosa'),
('Idiazabal Gazta Tarta Eztiarekin', 'Etxeko tarta labean egina bertako baserriko eztiarekin', 'Postrea', 6.50, 'Laktosa, Arrautza'),
('Etxeko Mamia Intxaur eta Eztiarekin', 'Latxa ardi esnearekin egindako mamia intxaurrez lagunduta', 'Postrea', 5.00, 'Laktosa, Fruitu lehorrak'),
('Arroz-esnea Kanelarekin', 'Astiro egositako arroz-esne kremozua kanela hautseztatuta', 'Postrea', 4.50, 'Laktosa');

-- E. Edariak (Aukera zabala)
INSERT INTO platerak (izena, deskribapena, kategoria, prezioa, alergenoak) VALUES
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

-- 3. ERREZETAK (PLATER OSAGAIAK - ESKANDALLOA)
-- Proportzio errealistak plater bakoitzeko lehengaien kontsumoa bermatzeko:

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

-- Edariak (Botilaka / Portzioka):
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
