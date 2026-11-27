-- =====================================================================
-- 1T JATETXEA - HASIERAKO DATUAK (DML - SEED)
-- Exekuzio ordena: 01_egitura_schema.sql ondoren (02_hasierako_datuak.sql)
-- Barne: 66 lehengai, 64 plater/edari (14 kategoria), errezeta osoak,
--        11 mahai, 15 langile (BCrypt), HR (kontratuak/fitxaketak/bajak),
--        bezeroak, iritziak eta 11 faktura.
-- =====================================================================

SET NAMES utf8mb4;
USE jatetxea_db;

-- =====================================================================
-- 1. LEHENGAIAK (66 OSAGAI: SUKALDEA + BARRAKO EDARIAK)
-- =====================================================================
INSERT INTO lehengaiak (izena, unitatea, uneko_stocka, gutxieneko_stocka, kostu_unitarioa, hornitzailea) VALUES
-- Sukaldeko lehengaiak
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
-- Barrako eta edarietako lehengaiak
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
('Patxaran Botila', 'l', 10.000, 3.000, 12.00, 'Baines Patxarana'),
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
('Whisky eta Vodka Botila', 'l', 12.000, 3.000, 18.50, 'Likore Banatzailea');

-- =====================================================================
-- 2. PLATERAK ETA EDARIAK (64 AUKERA — 14 KATEGORIA ZEHATZ)
-- =====================================================================
INSERT INTO platerak (izena, deskribapena, kategoria, prezioa, alergenoak) VALUES
-- Sarrerakoak (4)
('Idiazabal Gazta Dastaketa', 'Bertako gazta ondua intxaurrekin eta sagardo membrilloarekin', 'Sarrerakoa', 12.00, 'Laktosa, Fruitu lehorrak'),
('Iberiar Urdaiazpiko Ezkur-jana', 'Ezkurrez elikatutako 100% iberiarra ogi txigortu eta tomatearekin', 'Sarrerakoa', 19.50, 'Glutena'),
('Txangurro Kroketa Etxekoak (8 un.)', 'Itsas zapore sakoneko kroketa krematsuak', 'Sarrerakoa', 11.50, 'Krustazeoak, Glutena, Laktosa'),
('Gildak eta Antxoak Olio Epelekin', 'Kantauriko antxoak, Ibarrako piparrak eta oliba berdeak', 'Sarrerakoa', 9.50, 'Arraina'),
-- Lehen Platerak (4)
('Tolosako Babarrunak', 'Sakramentu guztiekin', 'Lehena', 14.00, NULL),
('Porrusalda Bakailao Miguekin', 'Tradizionala baratze-puerroekin eta bakailao mamitsuarekin', 'Lehena', 10.50, 'Arraina'),
('Sasoiko Barazki Menestra', 'Goierriko barazki freskoak birjina olioarekin erregosiak', 'Lehena', 12.50, NULL),
('Onddo eta Idiazabal Risotto Kremozua', 'Boletus edulis onddoak eta Idiazabal gazta urtua', 'Lehena', 15.00, 'Laktosa'),
-- Bigarren Platerak (6)
('Txuleta Errea (kg)', 'Euskal Okela plantxan gatz lodiarekin', 'Bigarrena', 45.00, NULL),
('Bakailaoa Bizkaitar Erara', 'Tradizionala piperrekin', 'Bigarrena', 18.50, 'Arraina'),
('Legatz Errea Orio Erara', 'Parrillan errea baratxuri, gindilla eta sagardo ozpin epelarekin', 'Bigarrena', 22.00, 'Arraina'),
('Txipiroiak Beren Tintan', 'Kostaldeko errezetan prestatutako txipiroi freskoak arroza zuriarekin', 'Bigarrena', 19.00, 'Moluskuak'),
('Arkume Errea Labean', 'Euskal arkume errea patata panadera gozoekin', 'Bigarrena', 24.50, NULL),
('Euskal Txerri Solomoa Gazta Saltsarekin', 'Bertako txerri solomoa plantxan Idiazabal saltsa leunarekin', 'Bigarrena', 16.50, 'Laktosa'),
-- Postreak (5)
('Pantxineta Etxekoa', 'Krema epelekin egina', 'Postrea', 6.00, 'Glutena, Arrautza, Laktosa'),
('Goxua Tradizionala', 'Bizkotxo bustia, pastel-krema, esnegaina eta karamelo txigortua', 'Postrea', 5.50, 'Glutena, Arrautza, Laktosa'),
('Idiazabal Gazta Tarta Eztiarekin', 'Etxeko tarta labean egina bertako baserriko eztiarekin', 'Postrea', 6.50, 'Laktosa, Arrautza'),
('Etxeko Mamia Intxaur eta Eztiarekin', 'Latxa ardi esnearekin egindako mamia intxaurrez lagunduta', 'Postrea', 5.00, 'Laktosa, Fruitu lehorrak'),
('Arroz-esnea Kanelarekin', 'Astiro egositako arroz-esne kremozua kanela hautseztatuta', 'Postrea', 4.50, 'Laktosa'),
-- Urak (4)
('Ur Mineral Naturala (1L)', 'Alzola euskal iturriko ur garbia', 'Urak', 2.50, NULL),
('Ur Mineral Gasduna (50cl)', 'Burbuila fin eta freskagarriak', 'Urak', 2.20, NULL),
('San Pellegrino Ur Gasduna (75cl)', 'Italiako ur gasdun fina, mahairako bikaina', 'Urak', 3.60, NULL),
('Alzola Ur Botilatxoa (50cl)', 'Euskal iturriko ur mineral naturala', 'Urak', 1.80, NULL),
-- Freskagarriak (5)
('Freskagarria (Kola, Laranja, Limoia 33cl)', 'Hautatutako freskagarri lata', 'Freskagarriak', 2.60, NULL),
('Etxeko Sagar Zuku Ekologikoa (25cl)', 'Sagar bertakoz egindako zuku naturala', 'Freskagarriak', 3.00, NULL),
('Tonika Premium (20cl)', 'Botila hotza limoi-azalarekin zerbitzatua', 'Freskagarriak', 2.80, NULL),
('Limonada Etxekoa Menta Freskoarekin', 'Limoi zukua, menta hostoak eta izotza', 'Freskagarriak', 3.40, NULL),
('Mosto Gorria (20cl)', 'Mahats zuku naturala, alkoholik gabea', 'Freskagarriak', 2.30, 'Sulfitoak'),
-- Garagardoak (5)
('Eskulangile Garagardoa Basqueland (33cl)', 'Euskal Herrian artisau moduan ekoitzitako IPA garagardoa', 'Garagardoak', 3.80, 'Glutena'),
('Garagardo Kaña Zirriborroa (33cl)', 'Iturriko garagardo hotza', 'Garagardoak', 2.80, 'Glutena'),
('Garagardo Zuritoa (20cl)', 'Iturriko garagardo trago laburra', 'Garagardoak', 1.80, 'Glutena'),
('Garagardo 0,0% Alkohol Gabea (33cl)', 'Alkohola kendutako garagardo freskagarria', 'Garagardoak', 2.80, 'Glutena'),
('Peroni Nastro Azzurro (33cl)', 'Italiako lager garagardo leun eta freskagarria', 'Garagardoak', 3.50, 'Glutena'),
-- Sagardoak (3)
('Euskal Sagardo Naturala (75cl)', 'Kupako sagardo fresko eta naturala', 'Sagardoak', 4.50, 'Sulfitoak'),
('Bereziartua Sagardo Ekologikoa (75cl)', 'Euskal Herriko sagar ekologiko hautatuekin egindako sagardoa', 'Sagardoak', 5.80, 'Sulfitoak'),
('Sagardo Aparduna Extra (75cl)', 'Metodo tradizionalean bigarren hartziduradun euskal sagardoa', 'Sagardoak', 12.50, 'Sulfitoak'),
-- Ardoak (7)
('Getariako Txakolina D.O. (75cl)', 'Hondarrabi Zuri mahatsez egindako txakolin txinpartatsua', 'Ardoak', 16.00, 'Sulfitoak'),
('Errioxako Ardo Beltza Crianza D.O. (75cl)', 'Haritz upelean ondua, gorputz handikoa', 'Ardoak', 18.00, 'Sulfitoak'),
('Errioxako Ardo Beltza Erreserba D.O. (75cl)', 'Hautespen berezia, dotorea eta orekatua', 'Ardoak', 26.00, 'Sulfitoak'),
('Nafarroako Ardo Gorria D.O. (75cl)', 'Garnatxa mahatsa, freskoa eta fruitutsua', 'Ardoak', 14.00, 'Sulfitoak'),
('Etxeko Ardoa Pitxerra (1L)', 'Upategiko ardo gaztea jantokirako', 'Ardoak', 7.00, 'Sulfitoak'),
('Chianti Classico D.O.C.G. (Kopa)', 'Toscana eskualdeko Sangiovese ardo beltz dotorea', 'Ardoak', 4.80, 'Sulfitoak'),
('Prosecco Valdobbiadene (Kopa)', 'Italiar apardun freskoa aperitiborako', 'Ardoak', 4.50, 'Sulfitoak'),
-- Kafeak (5)
('Kafe Ebakia / Esnearekin', 'Baque kalitate goreneko kafe aleak', 'Kafeak', 1.80, 'Laktosa'),
('Espresso Italiarra', 'Aroma intentsuko espresso laburra, krema lodiarekin', 'Kafeak', 1.60, NULL),
('Cappuccino Krematsua', 'Espressoa, esne aparra eta kakao ukitua', 'Kafeak', 2.50, 'Laktosa'),
('Kafe Amerikanoa', 'Espresso luzatua ur beroarekin', 'Kafeak', 1.90, NULL),
('Carajillo Patxaranarekin', 'Kafe beroa Nafarroako patxaran ukituarekin', 'Kafeak', 2.90, NULL),
-- Infusioak (4)
('Kamamila Infusio Naturala', 'Lasaigarria eta digestiboa bazkalondoan', 'Infusioak', 2.00, NULL),
('Te Berdea Menta Freskoarekin', 'Te berde ekologikoa menta ukitu freskoarekin', 'Infusioak', 2.20, NULL),
('Te Beltza Earl Grey', 'Bergamota usaineko te beltz klasikoa', 'Infusioak', 2.20, NULL),
('Poleo-Menta Digestiboa', 'Infusio freskoa eta arina', 'Infusioak', 2.00, NULL),
-- Likoreak (4)
('Patxaran Tradizionala (Kopa)', 'Nafarroako basaranez egindako patxaran digestiboa', 'Likoreak', 4.00, NULL),
('Limoncello Etxekoa (Txupitoa)', 'Sorrento erara egindako limoi likore izoztua', 'Likoreak', 3.50, NULL),
('Amaretto di Saronno (Kopa)', 'Almendra gozozko italiar likorea izotzarekin', 'Likoreak', 4.50, 'Fruitu lehorrak'),
('Orujo Belar Likorea (Txupitoa)', 'Belar aromatikoekin mazeratutako orujoa', 'Likoreak', 3.20, NULL),
-- Konbinatuak (4)
('Gin Tonic Premium', 'Ginebra aromatikoa, tonika premium eta limoi azala', 'Konbinatuak', 9.00, NULL),
('Ron-Kola (Barceló / Brugal)', 'Ron zaharra, kola freskagarria eta lima xerra', 'Konbinatuak', 7.50, NULL),
('Whisky-Kola / Ginger Ale', 'Whisky klasikoa izotzarekin eta aukeratutako freskagarriarekin', 'Konbinatuak', 8.00, NULL),
('Vodka-Limoi / Laranja', 'Vodka purua limoi edo laranja freskagarriarekin', 'Konbinatuak', 7.50, NULL),
-- Koktailak (4)
('Aperol Spritz Italiarra', 'Prosecco, Aperol, soda eta laranja xerra', 'Koktailak', 7.50, 'Sulfitoak'),
('Negroni Klasikoa', 'Ginebra, Campari/Aperol eta vermut gorria', 'Koktailak', 8.50, 'Sulfitoak'),
('Mojito Klasikoa', 'Ron zuria, lima, menta freskoa eta soda', 'Koktailak', 8.00, NULL),
('Espresso Martini', 'Vodka, espresso italiarra eta kafe likorea', 'Koktailak', 9.50, NULL);

-- =====================================================================
-- 3. ERREZETAK / ESKANDALLOAK (PLATER_OSAGAIAK)
-- =====================================================================
-- Sukaldeko platerak
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Txuleta Errea (kg)' AND l.izena = 'Euskal Okela Txuleta'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Txuleta Errea (kg)' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Tolosako Babarrunak' AND l.izena = 'Tolosako Babarruna'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Tolosako Babarrunak' AND l.izena = 'Tipula'
UNION ALL SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Tolosako Babarrunak' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.250 FROM platerak p, lehengaiak l WHERE p.izena = 'Bakailaoa Bizkaitar Erara' AND l.izena = 'Bakailao Gazitua'
UNION ALL SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Bakailaoa Bizkaitar Erara' AND l.izena = 'Tipula'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Bakailaoa Bizkaitar Erara' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Pantxineta Etxekoa' AND l.izena = 'Krema eta Hojaldrea'
UNION ALL SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Dastaketa' AND l.izena = 'Idiazabal Gazta'
UNION ALL SELECT p.id, l.id, 0.040 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Dastaketa' AND l.izena = 'Intxaurrak'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Dastaketa' AND l.izena = 'Sagardo Membrilloa'
UNION ALL SELECT p.id, l.id, 0.100 FROM platerak p, lehengaiak l WHERE p.izena = 'Iberiar Urdaiazpiko Ezkur-jana' AND l.izena = 'Urdaiazpiko Iberikoa'
UNION ALL SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Iberiar Urdaiazpiko Ezkur-jana' AND l.izena = 'Ogia eta Tomatea'
UNION ALL SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Iberiar Urdaiazpiko Ezkur-jana' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.120 FROM platerak p, lehengaiak l WHERE p.izena = 'Txangurro Kroketa Etxekoak (8 un.)' AND l.izena = 'Txangurro Haragia'
UNION ALL SELECT p.id, l.id, 0.200 FROM platerak p, lehengaiak l WHERE p.izena = 'Txangurro Kroketa Etxekoak (8 un.)' AND l.izena = 'Esnea'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Txangurro Kroketa Etxekoak (8 un.)' AND l.izena = 'Irina'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Txangurro Kroketa Etxekoak (8 un.)' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Gildak eta Antxoak Olio Epelekin' AND l.izena = 'Antxoak Kantaurikoak'
UNION ALL SELECT p.id, l.id, 0.060 FROM platerak p, lehengaiak l WHERE p.izena = 'Gildak eta Antxoak Olio Epelekin' AND l.izena = 'Piparrak eta Olibak'
UNION ALL SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Gildak eta Antxoak Olio Epelekin' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.200 FROM platerak p, lehengaiak l WHERE p.izena = 'Porrusalda Bakailao Miguekin' AND l.izena = 'Puerroak eta Barazkiak'
UNION ALL SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Porrusalda Bakailao Miguekin' AND l.izena = 'Patatak'
UNION ALL SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Porrusalda Bakailao Miguekin' AND l.izena = 'Bakailao Gazitua'
UNION ALL SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Porrusalda Bakailao Miguekin' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.350 FROM platerak p, lehengaiak l WHERE p.izena = 'Sasoiko Barazki Menestra' AND l.izena = 'Puerroak eta Barazkiak'
UNION ALL SELECT p.id, l.id, 0.025 FROM platerak p, lehengaiak l WHERE p.izena = 'Sasoiko Barazki Menestra' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.120 FROM platerak p, lehengaiak l WHERE p.izena = 'Onddo eta Idiazabal Risotto Kremozua' AND l.izena = 'Arroza'
UNION ALL SELECT p.id, l.id, 0.100 FROM platerak p, lehengaiak l WHERE p.izena = 'Onddo eta Idiazabal Risotto Kremozua' AND l.izena = 'Boletus Onddoak'
UNION ALL SELECT p.id, l.id, 0.040 FROM platerak p, lehengaiak l WHERE p.izena = 'Onddo eta Idiazabal Risotto Kremozua' AND l.izena = 'Idiazabal Gazta'
UNION ALL SELECT p.id, l.id, 0.250 FROM platerak p, lehengaiak l WHERE p.izena = 'Onddo eta Idiazabal Risotto Kremozua' AND l.izena = 'Barazki Salda'
UNION ALL SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Onddo eta Idiazabal Risotto Kremozua' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.350 FROM platerak p, lehengaiak l WHERE p.izena = 'Legatz Errea Orio Erara' AND l.izena = 'Legatza Freskoa'
UNION ALL SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Legatz Errea Orio Erara' AND l.izena = 'Baratxuria eta Gindilla'
UNION ALL SELECT p.id, l.id, 0.015 FROM platerak p, lehengaiak l WHERE p.izena = 'Legatz Errea Orio Erara' AND l.izena = 'Sagardo Ozpina'
UNION ALL SELECT p.id, l.id, 0.040 FROM platerak p, lehengaiak l WHERE p.izena = 'Legatz Errea Orio Erara' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.300 FROM platerak p, lehengaiak l WHERE p.izena = 'Txipiroiak Beren Tintan' AND l.izena = 'Txipiroiak'
UNION ALL SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Txipiroiak Beren Tintan' AND l.izena = 'Tipula'
UNION ALL SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Txipiroiak Beren Tintan' AND l.izena = 'Txipiroi Tinta'
UNION ALL SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Txipiroiak Beren Tintan' AND l.izena = 'Arroza'
UNION ALL SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Txipiroiak Beren Tintan' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.500 FROM platerak p, lehengaiak l WHERE p.izena = 'Arkume Errea Labean' AND l.izena = 'Euskal Arkumea'
UNION ALL SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Arkume Errea Labean' AND l.izena = 'Patatak'
UNION ALL SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Arkume Errea Labean' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.250 FROM platerak p, lehengaiak l WHERE p.izena = 'Euskal Txerri Solomoa Gazta Saltsarekin' AND l.izena = 'Txerri Solomoa'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Euskal Txerri Solomoa Gazta Saltsarekin' AND l.izena = 'Idiazabal Gazta'
UNION ALL SELECT p.id, l.id, 0.060 FROM platerak p, lehengaiak l WHERE p.izena = 'Euskal Txerri Solomoa Gazta Saltsarekin' AND l.izena = 'Esnegaina'
UNION ALL SELECT p.id, l.id, 0.020 FROM platerak p, lehengaiak l WHERE p.izena = 'Euskal Txerri Solomoa Gazta Saltsarekin' AND l.izena = 'Oliba Olio Birjina'
UNION ALL SELECT p.id, l.id, 0.120 FROM platerak p, lehengaiak l WHERE p.izena = 'Goxua Tradizionala' AND l.izena = 'Krema eta Hojaldrea'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Goxua Tradizionala' AND l.izena = 'Esnegaina'
UNION ALL SELECT p.id, l.id, 0.100 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Tarta Eztiarekin' AND l.izena = 'Idiazabal Gazta'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Tarta Eztiarekin' AND l.izena = 'Arrautzak'
UNION ALL SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Idiazabal Gazta Tarta Eztiarekin' AND l.izena = 'Eztia'
UNION ALL SELECT p.id, l.id, 0.180 FROM platerak p, lehengaiak l WHERE p.izena = 'Etxeko Mamia Intxaur eta Eztiarekin' AND l.izena = 'Ardi Esnea Mamia'
UNION ALL SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Etxeko Mamia Intxaur eta Eztiarekin' AND l.izena = 'Intxaurrak'
UNION ALL SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Etxeko Mamia Intxaur eta Eztiarekin' AND l.izena = 'Eztia'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Arroz-esnea Kanelarekin' AND l.izena = 'Arroza'
UNION ALL SELECT p.id, l.id, 0.200 FROM platerak p, lehengaiak l WHERE p.izena = 'Arroz-esnea Kanelarekin' AND l.izena = 'Esnea';

-- Edarien errezetak (45 edari guztiak)
INSERT INTO plater_osagaiak (platera_id, lehengaia_id, kantitatea)
SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Euskal Sagardo Naturala (75cl)' AND l.izena = 'Sagardo Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Getariako Txakolina D.O. (75cl)' AND l.izena = 'Txakolin Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Errioxako Ardo Beltza Crianza D.O. (75cl)' AND l.izena = 'Ardo Beltza Crianza Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Errioxako Ardo Beltza Erreserba D.O. (75cl)' AND l.izena = 'Ardo Beltza Erreserba Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Nafarroako Ardo Gorria D.O. (75cl)' AND l.izena = 'Ardo Gorri Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Etxeko Ardoa Pitxerra (1L)' AND l.izena = 'Etxeko Ardoa Upela'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Eskulangile Garagardoa Basqueland (33cl)' AND l.izena = 'Basqueland Garagardo Botila'
UNION ALL SELECT p.id, l.id, 0.330 FROM platerak p, lehengaiak l WHERE p.izena = 'Garagardo Kaña Zirriborroa (33cl)' AND l.izena = 'Garagardo Upela'
UNION ALL SELECT p.id, l.id, 0.200 FROM platerak p, lehengaiak l WHERE p.izena = 'Garagardo Zuritoa (20cl)' AND l.izena = 'Garagardo Upela'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Garagardo 0,0% Alkohol Gabea (33cl)' AND l.izena = 'Garagardo 00 Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Ur Mineral Naturala (1L)' AND l.izena = 'Ur Botila 1L'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Ur Mineral Gasduna (50cl)' AND l.izena = 'Ur Gasdun Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Freskagarria (Kola, Laranja, Limoia 33cl)' AND l.izena = 'Freskagarri Lata'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Etxeko Sagar Zuku Ekologikoa (25cl)' AND l.izena = 'Sagar Zuku Botila'
UNION ALL SELECT p.id, l.id, 0.010 FROM platerak p, lehengaiak l WHERE p.izena = 'Kafe Ebakia / Esnearekin' AND l.izena = 'Kafe Aleak'
UNION ALL SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Kafe Ebakia / Esnearekin' AND l.izena = 'Esnea'
UNION ALL SELECT p.id, l.id, 0.060 FROM platerak p, lehengaiak l WHERE p.izena = 'Patxaran Tradizionala (Kopa)' AND l.izena = 'Patxaran Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'San Pellegrino Ur Gasduna (75cl)' AND l.izena = 'San Pellegrino Ur Botila 75cl'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Alzola Ur Botilatxoa (50cl)' AND l.izena = 'Alzola Ur Botila 50cl'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Bereziartua Sagardo Ekologikoa (75cl)' AND l.izena = 'Sagardo Ekologiko Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Sagardo Aparduna Extra (75cl)' AND l.izena = 'Sagardo Aparduna Botila'
UNION ALL SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Chianti Classico D.O.C.G. (Kopa)' AND l.izena = 'Chianti Classico Botila'
UNION ALL SELECT p.id, l.id, 0.150 FROM platerak p, lehengaiak l WHERE p.izena = 'Prosecco Valdobbiadene (Kopa)' AND l.izena = 'Prosecco Valdobbiadene Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Tonika Premium (20cl)' AND l.izena = 'Tonika Premium Botila'
UNION ALL SELECT p.id, l.id, 0.120 FROM platerak p, lehengaiak l WHERE p.izena = 'Limonada Etxekoa Menta Freskoarekin' AND l.izena = 'Limoia eta Menta Freskoa'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Mosto Gorria (20cl)' AND l.izena = 'Mosto Gorria Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Peroni Nastro Azzurro (33cl)' AND l.izena = 'Peroni Garagardo Botila'
UNION ALL SELECT p.id, l.id, 0.010 FROM platerak p, lehengaiak l WHERE p.izena IN ('Espresso Italiarra', 'Cappuccino Krematsua', 'Kafe Amerikanoa', 'Carajillo Patxaranarekin', 'Espresso Martini') AND l.izena = 'Kafe Aleak'
UNION ALL SELECT p.id, l.id, 0.120 FROM platerak p, lehengaiak l WHERE p.izena = 'Cappuccino Krematsua' AND l.izena = 'Esnea'
UNION ALL SELECT p.id, l.id, 0.030 FROM platerak p, lehengaiak l WHERE p.izena = 'Carajillo Patxaranarekin' AND l.izena = 'Patxaran Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena IN ('Kamamila Infusio Naturala', 'Te Berdea Menta Freskoarekin', 'Te Beltza Earl Grey', 'Poleo-Menta Digestiboa') AND l.izena = 'Infusio Poltsak (Kamamila/Teak)'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Limoncello Etxekoa (Txupitoa)' AND l.izena = 'Limoncello Botila'
UNION ALL SELECT p.id, l.id, 0.060 FROM platerak p, lehengaiak l WHERE p.izena = 'Amaretto di Saronno (Kopa)' AND l.izena = 'Amaretto Botila'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena = 'Orujo Belar Likorea (Txupitoa)' AND l.izena = 'Orujo Belar Botila'
UNION ALL SELECT p.id, l.id, 0.060 FROM platerak p, lehengaiak l WHERE p.izena = 'Aperol Spritz Italiarra' AND l.izena = 'Aperol Botila'
UNION ALL SELECT p.id, l.id, 0.090 FROM platerak p, lehengaiak l WHERE p.izena = 'Aperol Spritz Italiarra' AND l.izena = 'Prosecco Valdobbiadene Botila'
UNION ALL SELECT p.id, l.id, 0.060 FROM platerak p, lehengaiak l WHERE p.izena IN ('Negroni Klasikoa', 'Gin Tonic Premium') AND l.izena = 'Ginebra Premium eta Vermut'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena = 'Gin Tonic Premium' AND l.izena = 'Tonika Premium Botila'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena IN ('Mojito Klasikoa', 'Ron-Kola (Barceló / Brugal)') AND l.izena = 'Ron Zuria eta Zaharra Botila'
UNION ALL SELECT p.id, l.id, 0.080 FROM platerak p, lehengaiak l WHERE p.izena = 'Mojito Klasikoa' AND l.izena = 'Limoia eta Menta Freskoa'
UNION ALL SELECT p.id, l.id, 0.050 FROM platerak p, lehengaiak l WHERE p.izena IN ('Whisky-Kola / Ginger Ale', 'Vodka-Limoi / Laranja', 'Espresso Martini') AND l.izena = 'Whisky eta Vodka Botila'
UNION ALL SELECT p.id, l.id, 1.000 FROM platerak p, lehengaiak l WHERE p.izena IN ('Ron-Kola (Barceló / Brugal)', 'Whisky-Kola / Ginger Ale', 'Vodka-Limoi / Laranja') AND l.izena = 'Freskagarri Lata';

-- =====================================================================
-- 4. MAHAIAK (11 MAHAI)
-- =====================================================================
INSERT INTO mahaiak (zenbakia, edukiera, kokapena, egoera) VALUES
(1, 2, 'Jantoki Nagusia', 'Libre'),
(2, 4, 'Jantoki Nagusia', 'Libre'),
(3, 4, 'Jantoki Nagusia', 'Libre'),
(4, 6, 'Jantoki Nagusia', 'Libre'),
(5, 8, 'VIP', 'Libre'),
(6, 4, 'Terraza', 'Libre'),
(7, 2, 'Barra', 'Libre'),
(8, 2, 'Barra', 'Libre'),
(9, 4, 'Terraza', 'Libre'),
(10, 4, 'Terraza', 'Libre'),
(11, 6, 'VIP', 'Libre');

-- =====================================================================
-- 5. LANGILEAK ETA GIZA BALIABIDEAK (HR — 15 LANGILE BCrypt HASH-EKIN)
-- =====================================================================
INSERT INTO langileak (erabiltzailea, pasahitza_hash, izena, abizena, nan_dni, emaila, telefonoa, rola, saila, kargua) VALUES
('anderu', '$2a$11$WjOP2N6biVYO1HrvAjYKyOmX5zpj5PZ2U16rjklM/tzSiUwNg6odG', 'Ander', 'Urien Telleria', '72548901A', 'ander.urien@goierrieskola.eus', '688112233', 'AdminIT', 'Sistemak_IT', 'IT Lead / Arkitekto Nagusia'),
('julen', '$2a$11$RqNGdk.gmKgvB5c4btG9leU8PMSBc782S1kS8PSXKsbt0D/HYVBAK', 'Julen', 'Aguado Fernández', '72548902B', 'julen.aguado@goierrieskola.eus', '688112234', 'AdminIT', 'Sistemak_IT', 'Sare eta Suebaki Ingeniaria'),
('iker', '$2a$11$qJ8PWi0Cy4a4Cu.nNPwL0eDbGCPqI8M4C6c0JeqpnVmY9XmWttWUO', 'Iker', 'Araujo Dominguez', '72548903C', 'iker.araujo@goierrieskola.eus', '688112235', 'AdminIT', 'Sistemak_IT', 'Sistema Eragileak eta Birtualizazioa'),
('aratz', '$2a$11$xikVH/hjNWd7KUpGt6w11Oyv4XAWbj3zzfYvUs4LXuVGkOCeUCsLC', 'Aratz', 'Huegun Flores', '72548904D', 'aratz.huegun@goierrieskola.eus', '688112236', 'AdminIT', 'Sistemak_IT', 'Zibersegurtasun eta Auditoritza Burua'),
('ainhoa_chef', '$2a$11$QznQKDnUuNCokupqxQ6RouhWUQkhtAcu6FiXjt0xxGPmNFTPoGjAG', 'Ainhoa', 'Goikoetxea', '72548905E', 'ainhoa.goikoetxea@jatetxea.eus', '677112233', 'Sukaldaria', 'Sukaldea', 'Chef Burua / Sukaldari Nagusia'),
('eneko_suk', '$2a$11$TTG1qxhdXBNemtYawaY7OO.BKHvNx7QbzcOPHUl827LQ4QlySp0T2', 'Eneko', 'Otegi', '72548906F', 'eneko.otegi@jatetxea.eus', '677112234', 'Sukaldaria', 'Sukaldea', 'Bigarren Sukaldaria / Parrilleroa'),
('mikel_zerb', '$2a$11$qpinuX7AxybylayHyOrV9eA8WCDDBAkgwa5IxwG9bB/tFbtF8EiRS', 'Mikel', 'Arregi', '72548907G', 'mikel.arregi@jatetxea.eus', '666112233', 'Zerbitzaria', 'Jantokia', 'Maître / Zerbitzari Burua'),
('maite_zerb', '$2a$11$Wv/Yf.tCrQBwRSB5TGg82ez8.P/PRRQjdznEPcVao12YgaxqKjpoS', 'Maite', 'Zabaleta', '72548908H', 'maite.zabaleta@jatetxea.eus', '666112234', 'Zerbitzaria', 'Jantokia', 'Jantokiko Zerbitzaria'),
('unai_zerb', '$2a$11$2t0Np6A5k7zkDlzh8Z6dreXnGyPU45VrelaqK0M44F2VrWqXIel.O', 'Unai', 'Lasa', '72548909J', 'unai.lasa@jatetxea.eus', '666112235', 'Zerbitzaria', 'Jantokia', 'Jantokiko Zerbitzaria'),
('gorka_barra', '$2a$11$40DAjzi765gK1kjGKNbzZuuPEqpS4vg3TROpZ969ODsXytQ5GxgvO', 'Gorka', 'Mendizabal', '72548910K', 'gorka.mendizabal@jatetxea.eus', '655112233', 'Barrakoa', 'Barra', 'Barman / Barrako Arduraduna'),
('uxue_barra', '$2a$11$ihyQpYEQBDmHDXNoAdy/Geuahgg6.WyCh4Yu.An2QvDSrXEwZrc16', 'Uxue', 'Etxarri', '72548911L', 'uxue.etxarri@jatetxea.eus', '655112234', 'Barrakoa', 'Barra', 'Barrako Zerbitzaria'),
('kepa_biltegi', '$2a$11$xwax0r.nwsIPIwwEdSr.nO3hlwxCe5TIvj21OFFeRmCRd/vk3Z2WW', 'Kepa', 'Aldasoro', '72548912M', 'kepa.aldasoro@jatetxea.eus', '644112233', 'Biltegikoa', 'Biltegia', 'Biltegi eta Stock Burua'),
('ander_m_zuz', '$2a$11$Fo4WievPUE35eElCleMJjusW.ENeraAMmOm9T6XDgPyGR2nKjB6zm', 'Ander', 'Madinabeitia', '72548913N', 'ander.madinabeitia@jatetxea.eus', '633112233', 'Kudeatzailea', 'Zuzendaritza', 'Zuzendaritza Kontseilukoa / Zuzendaria'),
('joseba_r_zuz', '$2a$11$S7r5PkmmbwSvI2RySPVA1OKpEu/kmIxCdJhw9ZeXmCqY6tSSzhJYy', 'Joseba', 'Ramos', '72548914P', 'joseba.ramos@jatetxea.eus', '633112234', 'Kudeatzailea', 'Zuzendaritza', 'Zuzendaritza Kontseilukoa / Zuzendaria'),
('estebe_o_zuz', '$2a$11$maFEcb7ZQIzRSKjH0Rm.nu/2O7YLioEgLF0KPTyyXeqS47n6aJMjK', 'Estebe', 'Ormazabal', '72548915Q', 'estebe.ormazabal@jatetxea.eus', '633112235', 'Kudeatzailea', 'Zuzendaritza', 'Zuzendaritza Kontseilukoa / Zuzendaria');

INSERT INTO langile_kontratuak (langilea_id, kontratu_zenbakia, kontratu_mota, lanaldia, hasiera_data, hileroko_soldata_brutoa, kategoria_profesionala) VALUES
(1, 'KONT-2026-IT01', 'Mugagabea', 'Osoa_40h', '2026-09-01', 2850.00, 'Ingeniari Informatikoa'),
(2, 'KONT-2026-IT02', 'Mugagabea', 'Osoa_40h', '2026-09-01', 2700.00, 'Sare Administratzailea'),
(3, 'KONT-2026-IT03', 'Mugagabea', 'Osoa_40h', '2026-09-01', 2700.00, 'Sistema Teknikaria'),
(4, 'KONT-2026-IT04', 'Mugagabea', 'Osoa_40h', '2026-09-01', 2750.00, 'Zibersegurtasun Espezialista'),
(5, 'KONT-2026-SUK01', 'Mugagabea', 'Osoa_40h', '2025-01-15', 2500.00, 'Sukaldari Nagusia'),
(6, 'KONT-2026-SUK02', 'Mugagabea', 'Osoa_40h', '2025-03-01', 1950.00, 'Sukaldaria'),
(7, 'KONT-2026-JAN01', 'Mugagabea', 'Osoa_40h', '2025-01-15', 2100.00, 'Zerbitzari Burua'),
(8, 'KONT-2026-JAN02', 'Denboraldikoa', 'Partziala_30h', '2026-06-01', 1400.00, 'Zerbitzaria'),
(9, 'KONT-2026-JAN03', 'Denboraldikoa', 'Partziala_20h', '2026-06-01', 950.00, 'Zerbitzaria'),
(10, 'KONT-2026-BAR01', 'Mugagabea', 'Osoa_40h', '2025-02-01', 1850.00, 'Barman Nagusia'),
(11, 'KONT-2026-BAR02', 'Mugagabea', 'Osoa_40h', '2025-05-10', 1600.00, 'Barrako Laguntzailea'),
(12, 'KONT-2026-BIL01', 'Mugagabea', 'Osoa_40h', '2025-01-15', 1800.00, 'Biltegizaina'),
(13, 'KONT-2026-ZUZ01', 'Mugagabea', 'Osoa_40h', '2024-10-01', 3200.00, 'Zuzendari Kudeatzailea'),
(14, 'KONT-2026-ZUZ02', 'Mugagabea', 'Osoa_40h', '2024-10-01', 3200.00, 'Zuzendari Kudeatzailea'),
(15, 'KONT-2026-ZUZ03', 'Mugagabea', 'Osoa_40h', '2024-10-01', 3200.00, 'Zuzendari Kudeatzailea');

INSERT INTO langile_fitxaketak (langilea_id, sarrera_data, irteera_data, lan_orduak, metodoa, egoera) VALUES
(1, '2026-11-14 08:30:00', '2026-11-14 16:30:00', 8.00, 'Aplikazioa', 'Zuzena'),
(5, '2026-11-14 09:00:00', '2026-11-14 17:00:00', 8.00, 'RFID', 'Zuzena'),
(7, '2026-11-14 11:30:00', '2026-11-14 16:30:00', 5.00, 'PIN', 'Zuzena'),
(10, '2026-11-14 10:00:00', '2026-11-14 18:00:00', 8.00, 'QR', 'Zuzena');

INSERT INTO langile_bajak (langilea_id, mota, hasiera_data, amaiera_data, egun_kopurua, egoera, oharrak) VALUES
(8, 'Oporrak', '2026-11-10', '2026-11-15', 5, 'Onartua', 'Urteko opor egunetatik hartutakoak'),
(6, 'Gaixotasun_Arrunta', '2026-11-02', '2026-11-05', 3, 'Bukatua', 'Gripe arrunta, mediku agiria aurkeztua');

-- =====================================================================
-- 6. BEZEROAK, IRITZIAK ETA FAKTURAK
-- =====================================================================
INSERT INTO bezeroak (izena, abizena, telefonoa, emaila) VALUES
('Ane', 'Iparragirre', '600112233', 'ane@adibidea.eus'),
('Ion', 'Zubizarreta', '611223344', 'ion@adibidea.eus');

INSERT INTO iritziak (bezeroa_id, balorazioa, testua) VALUES
(1, 5, 'Txuleta zoragarria eta zerbitzu bikaina! Seguru itzuliko garela.'),
(2, 4, 'Babarrun goxoak eta giro atsegina, baina postreetan aukera zabalagoa eskertuko litzateke.');

INSERT INTO fakturak
(faktura_zenbakia, mota, azpimota, kontzeptua, hornitzailea, hornitzaile_cif, bezero_nif, bezero_izena, bezero_helbidea, oinarri_zergagarria, bez_ehunekoa, bez_zenbatekoa, guztira, ordainketa_metodoa, egoera, data)
VALUES
('F2026-S001', 'Salmenta', 'Sinplifikatua', 'Jatetxe zerbitzua - Mahaia 1', NULL, NULL, NULL, 'Bezero Arrunta', NULL, 74.55, 10.00, 7.45, 82.00, 'Txartela', 'Ordaindua', '2026-11-17 14:30:00'),
('F2026-S002', 'Salmenta', 'Sinplifikatua', 'Jatetxe zerbitzua - Mahaia 3', NULL, NULL, NULL, 'Bezero Arrunta', NULL, 122.73, 10.00, 12.27, 135.00, 'Txartela', 'Ordaindua', '2026-11-17 15:15:00'),
('F2026-S003', 'Salmenta', 'Osoa', 'Enpresa bazkaria (12 mahaikide)', NULL, NULL, 'B75123987', 'CAF S.A. Enpresa', 'J.M. Iturrioz 26, Beasain', 436.36, 10.00, 43.64, 480.00, 'Transferentzia', 'Ordaindua', '2026-11-18 15:45:00'),
('F2026-S004', 'Salmenta', 'Sinplifikatua', 'Barra eta menu kontsumoa', NULL, NULL, NULL, 'Bezero Arrunta', NULL, 45.45, 10.00, 4.55, 50.00, 'Eskudirua', 'Ordaindua', '2026-11-19 13:50:00'),
('F2026-S005', 'Salmenta', 'Sinplifikatua', 'Jatetxe zerbitzua - Mahaia 2', NULL, NULL, NULL, 'Bezero Arrunta', NULL, 89.09, 10.00, 8.91, 98.00, 'Bizum', 'Ordaindua', '2026-11-19 22:10:00'),
('F2026-S006', 'Salmenta', 'Osoa', 'Batzordearen afaria (8 lagun)', NULL, NULL, 'A20112233', 'Goierri Valley Elkartea', 'Arranbide 2, Ordizia', 290.91, 10.00, 29.09, 320.00, 'Txartela', 'Ordaindua', '2026-11-19 22:45:00'),
('F2026-G001', 'Erosketa', 'Osoa', 'Txuleta eta haragi freskoen hornidura', 'Goierri Okela S.L.', 'B20123456', NULL, NULL, NULL, 450.00, 10.00, 45.00, 495.00, 'Transferentzia', 'Ordaindua', '2026-11-15 08:30:00'),
('F2026-G002', 'Erosketa', 'Osoa', 'Legatz eta itsaski horniketa', 'Kantauri Arrainak S.A.', 'A20987654', NULL, NULL, NULL, 320.00, 10.00, 32.00, 352.00, 'Transferentzia', 'Ordaindua', '2026-11-16 09:15:00'),
('F2026-G003', 'Erosketa', 'Osoa', 'Garaiko barazkiak, patatak eta entsaladak', 'Ordiziako Barazki Koop.', 'F20345678', NULL, NULL, NULL, 180.00, 10.00, 18.00, 198.00, 'Eskudirua', 'Ordaindua', '2026-11-17 07:45:00'),
('F2026-G004', 'Erosketa', 'Osoa', 'Ardoak, sagardoa eta likoreak', 'Euskal Upeltegiak & Edariak', 'B20778899', NULL, NULL, NULL, 520.00, 21.00, 109.20, 629.20, 'Transferentzia', 'Ordaindua', '2026-11-18 11:00:00'),
('F2026-G005', 'Erosketa', 'Osoa', 'Jatetxeko argindarra eta gas hornidura (Urria)', 'Iberdrola Energia', 'A48010615', NULL, NULL, NULL, 680.00, 21.00, 142.80, 822.80, 'Helbideratua', 'Ordaindua', '2026-11-10 12:00:00');
