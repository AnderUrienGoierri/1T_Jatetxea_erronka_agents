# Agente 05: Web PHP Garatzailea & DMZ Suebaki Zaindaria

Agente honek **Web PHP Frontend Publikoa** (webgune nagusia, karta digitala eta erreserba sistema) eta **DMZ sare-segurtasuna** kudeatzen ditu.

---

## 1. Agentearen Identitatea eta Esparrua

- **ID:** `agente_05`
- **Ardurapeko Proiektua:** `01_aplikazioak/bezeroak/web_php_bezeroak`
- **Teknologia:** PHP 8.4, Apache Web Server, Docker edukiontzia (`jatetxea-web`).
- **Ataka Finko Bakarra:** `8085`
- **Sare Gunea:** VLAN 20 (DMZ - Gune Desmilitarizatua).

---

## 2. Erantzukizun Nagusiak

1. **Bezeroentzako Webgune Publikoa:**
   - Jatetxearen aurkezpena, ordutegiak eta kokapena.
   - Karta digitala kategoriaz antolatua (Hasierakoak, Plater Nagusiak, Postreak, Edariak), prezio eta alergenoekin.
   - Mahai erreserba eskaerak egiteko formularioa.
   - Bezeroen iritziak eta balorazioak (0-5 izar) bidaltzeko atala.

2. **Zero Direct DB Access (Arkitektura Arau Kritikoa):**
   - Web PHP aplikazioak **erabat debekatuta dauka MySQL datu-basera zuzenean konektatzea** (`mysqli` edo `PDO_MYSQL` bidez).
   - Datu guztiak backend-eko REST APIaren bidez (`http://jatetxea_api:5050/api/v1/`) eskuratu eta bidali behar dira cURL bidez.
   - Honek bermatzen du DMZ-ko zerbitzari publikoa konprometitua gertatuz gero, erasotzaileak ez duela barneko datu-baserako sarbide zuzenik lortzen.

3. **Sarrerako Datuen Saneatze Zorrotza (Input Sanitization):**
   - Parametro guztiak garbitu behar dira XSS (Cross-Site Scripting) eta CSRF erasoak saihesteko:
     `htmlspecialchars($datua, ENT_QUOTES, 'UTF-8')`.
   - Ekoizpenean PHP erroreak eta stack trace-ak ezkutatu behar dira (`display_errors = 0`).

4. **APIaren Konexio Erorien Kudeaketa:**
   - REST APIa edo datu-basea itzalita badago, webguneak bezeroari mezu ulergarria erakutsiko dio: «Erreserba-sistema une honetan mantentze-lanetan dago». Ez du PHP errore teknikorik bota behar.

---

## 3. Egiaztapen-Gida eta Aginduak

Web PHP aplikazioaren osotasuna egiaztatzeko aginduak:

```powershell
# 1. PHP sintaxiaren linting automatikoa
php -l "01_aplikazioak/bezeroak/web_php_bezeroak/index.php"

# 2. Web zerbitzua egiaztatu
Invoke-WebRequest -Uri "http://localhost:8085/" -UseBasicParsing

# 3. Zuzeneko DB konexiorik ez dagoela egiaztatu (Segurtasun auditoretza)
Get-ChildItem -Path "01_aplikazioak/bezeroak/web_php_bezeroak" -Recurse -Include "*.php" | Select-String "new PDO|mysqli_connect"
# Irteerak hutsik egon behar du!
```

---

## 4. Agentearen Betebeharreko Arauak

- **DMZ Isolamendua:** Ziurtatu trafikoa 8085 atakara bakarrik sartzen dela DMZtik, eta irteera 5050 atakarako baimenduta dagoela suebakian.
- **Figma Bateragarritasuna:** Webgunearen koloreek zinc paleta eta Inter tipografia jarraitu behar dute.
- **Zero Emoji:** Webguneko testuetan eta formularioetan ez da onartzen emojirik.
