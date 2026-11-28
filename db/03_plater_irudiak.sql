-- Aplikatu lehendik dagoen jatetxea_db datu-basean, babes-kopia egin ondoren.
-- Ez da daturik ezabatzen eta lehendik ezarritako irudiak errespetatzen dira.
USE jatetxea_db;

SET @zutabea = (
    SELECT COUNT(*) FROM information_schema.columns
    WHERE table_schema = DATABASE()
        AND table_name = 'platerak'
        AND column_name = 'irudi_erreferentzia'
);
SET @sql = IF(@zutabea = 0,
    'ALTER TABLE platerak ADD COLUMN irudi_erreferentzia VARCHAR(255) NULL',
    'SELECT 1');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

UPDATE platerak SET irudi_erreferentzia = 'platerak/freskagarria.jpg'
WHERE izena = 'Freskagarria (Kola, Laranja, Limoia 33cl)' AND irudi_erreferentzia IS NULL;
UPDATE platerak SET irudi_erreferentzia = 'platerak/sagar-zukua.jpg'
WHERE izena = 'Etxeko Sagar Zuku Ekologikoa (25cl)' AND irudi_erreferentzia IS NULL;
UPDATE platerak SET irudi_erreferentzia = 'platerak/tonika.jpg'
WHERE izena = 'Tonika Premium (20cl)' AND irudi_erreferentzia IS NULL;
UPDATE platerak SET irudi_erreferentzia = 'platerak/limonada-menta.jpg'
WHERE izena = 'Limonada Etxekoa Menta Freskoarekin' AND irudi_erreferentzia IS NULL;
UPDATE platerak SET irudi_erreferentzia = 'platerak/mosto-gorria.jpg'
WHERE izena = 'Mosto Gorria (20cl)' AND irudi_erreferentzia IS NULL;
