# 1T Jatetxea · Web PHP Bezeroak (web_php_bezeroak)

Jatetxearen webgune publikoa, karta interaktiboa, online mahai-erreserbak eta bezeroen iritziak kudeatzen dituen PHP aplikazioa.

Kartako alergenoak ikurrekin erakusten dira; ikurraren gainean jarrita edo teklatuarekin hautatuta izena ikus daiteke. Ikur ezagunik ez duten alergenoak testuz agertzen dira.

Kartako «Guztiak» iragazkiak platerak erakusten ditu; edariak «Edariak» iragazkian eta haren azpikategorietan daude.

Goiburuko ezarpenetatik gaia alda daiteke. Konexio-puntua berdea da APIa eta datu-basea ondo daudenean, laranja APIak erantzun baina egoera-egiaztapenak huts egiten duenean, eta gorria APIa eskuragarri ez dagoenean. Azken kasuan datu-basearen egoera ezezaguna da; puntuaren azalpenak xehetasuna erakusten du.

Tolosako babarrunen argazkia: [Xaverius, Wikimedia Commons](https://commons.wikimedia.org/wiki/File:Tolosako_babarrunak.jpeg), domeinu publikoa.

---

## 1. Informazio Orokorra

- **Kokapena:** `01_aplikazioak/bezeroak/web_php_bezeroak`
- **Teknologia:** PHP 8.4, Apache 2.4, HTML5, JavaScript, Tailwind CSS.
- **Portua:** `8085` (Docker barruan 80, kanpora 8085 mapeatua).
- **URL Lokala:** `http://localhost:8085`

---

## 2. Arkitektura eta Fitxategien Banaketa

```
web_php_bezeroak/
├── index.php                         # Sarrera-orri nagusia (Single Page Application egituran antolatua)
├── .env                              # APIaren oinarrizko helbidea (API_BASE_URL)
├── assets/
│   ├── css/                          # Estilo-orriak (base, nav, karta, erreserbak, iritziak, kontaktua)
│   ├── js/                           # JavaScript bezero-logika (app, config, erreserbak, iritziak, karta)
│   ├── php/                          # PHP backend moduluak eta API bitartekaria (api.php, erreserbak.php, etab.)
│   └── img/                          # Jatetxearen ikonoak eta logoak (SVG)
```

---

## 3. Exekuzioa eta Hedapena

Aplikazioa Docker Compose bidez abiarazten da:

```bash
# Proiektuaren erroan:
docker compose up -d jatetxea-web
```

Nabigatzailean ireki: `http://localhost:8085`.
