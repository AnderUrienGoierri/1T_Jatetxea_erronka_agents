# Abio Azkarra - Proiektua Martxan Jartzeko Gida
## T1 Jatetxea (Italiar-Euskaldun Jatetxe Sistema Osoa)

Dokumentu honek azaltzen du nola jarri martxan proiektu hau hutsetik edozein ikaskideren ordenagailutan (ZIP fitxategia erauzi ondoren).

---

## 1. Aurre-baldintzak (Instalatuta egon beharrekoak)

* **.NET 10 SDK:** Backend APIa eta C# TPV aplikazioa konpilatu eta exekutatzeko.
* **Java JDK 17 edo 21:** JavaFX aplikazioa (Sukaldeko KDS, Barrako BDS eta Biltegia) exekutatzeko.
* **IntelliJ IDEA edo Eclipse/VSCode:** JavaFX proiektua irekitzeko (gomendatua: IntelliJ).
* **Visual Studio 2022 edo VSCode:** C# proiektuak arazteko (`Jatetxea.sln`).
* **MySQL / MariaDB:** Datu-basea ostatatzeko (Docker Desktop BIDEZ edo XAMPP bidez).
* **PHP:** (Aukerakoa) Web ataria nabigatzailean probatzeko.

---

## 2. Martxan Jartzeko Urratsak (Ordena honetan)

### 1. URRATSA: Datu-Basea Martxan Jarri (MySQL)

Hautatu zure ingurunerako modurik errazena:

#### A Aukera: Docker Desktop bidez (Gomendatua eta Azkarrena)
Ireki terminal bat proiektuaren erroan eta exekutatu:
```bash
docker compose up -d
```
* **Portua:** `3308`
* **Datu-basea:** `jatetxea_db` (erabiltzailea: `root`, pasahitza: `root`)
* Docker-ek lehen aldian automatikoki sortuko ditu taula guztiak, langileak, mahaiak eta platerak (`espezifikazioak/datu_basea_schema.sql` erabiliz).

#### B Aukera: XAMPP edo Bertako MySQL Zerbitzaria
1. Ziurtatu MySQL zerbitzua martxan dagoela.
2. MySQL Workbench edo phpMyAdmin bidez, exekutatu bi SQL script hauek:
   * `espezifikazioak/datu_basea_schema.sql` (taulak eta hasierako datuak).
   * `espezifikazioak/gehitu_karta_zabaldua.sql` (karta zabaldua).
3. Zure MySQL zerbitzaria `3306` portuan badago (`3308` beharrean), aldatu portua `backend_api/appsettings.json` fitxategian (`Port=3306;`).

---

### 2. URRATSA: Backend APIa Abiarazi (.NET 10)

Aurre-mailako bezero guztiek (C# TPV, JavaFX eta Web) API hau behar dute datuak trukatzeko.

Terminal batean:
```bash
cd backend_api
dotnet run
```
*(Edo Visual Studio 2022-n `Jatetxea.sln` ireki eta `Jatetxea.Api` proiektua abiarazi).*

* **APIaren helbidea:** `http://localhost:5000`
* **Swagger dokumentazioa ikusteko:** Nabigatzailean ireki `http://localhost:5000/swagger`

---

### 3. URRATSA: Bezeroak Abiarazi

APIa martxan dagoenean, edozein bezero erabil dezakezue:

#### A) C# Desktop TPV Zerbitzaria (Tablet POS)
Zerbitzariek mahaiak kontrolatzeko, eskaerak hartzeko, sukaldera bidaltzeko eta kobratzeko:

Terminal batean:
```bash
cd desktop_csharp
dotnet run
```
*(Edo Visual Studio-tik `Jatetxea.Desktop` proiektua exekutatu).*

* **Sartzeko langileak (PIN kodeak):**
  * `Ainhoa Goikoetxea` (Zerbitzaria) -> PIN: `1234`
  * `Mikel Arruabarrena` (Zerbitzaria) -> PIN: `1234`
  * `Jon Mendizabal` (Sukaldariburua) -> PIN: `1234`
  * `Maite Urrutia` (Kudeatzailea) -> PIN: `1234`
* **Funtzionalitate nagusiak:**
  * Mahaien kudeaketa guneen arabera (Jangela Nagusia, Terraza, Barra, VIP).
  * Komanda berria sortu eta produktuak hautatu kategoriaz (Edariak, Hastekoak, Haragiak, Postreak).
  * **Oharrak / Adierazpenak:** Plater bakoitzari oharrak jartzeko aukera (Gazta gutxi, Tipularik gabe, Puntuan, etab.).
  * **Kontu Banatuak:** Mahai berean `+ KONTU BERRIA (Mahaikide Banatua)` botoia erabilita eskaera anitzak kudeatu.
  * **Sukaldera Bidali:** Platerak sukaldera bidali prestaketa martxan jartzeko.
  * **Kontua Eskatu / Kobratu:** Ticket xehatua ikusi eta mahaia libre utzi garbitzeko.

---

#### B) JavaFX KDS, BDS eta Biltegia (Desktop Java)
Sukaldeko eta barrako pantailetan eskaerak denbora errealean kudeatzeko eta biltegiko stocka kontrolatzeko:

* **IntelliJ IDEA bidez (Gomendatua):**
  1. Ireki IntelliJ IDEA.
  2. Egin klik **Open** botoian eta hautatu `desktop_javafx` karpeta.
  3. Maven dependentziak kargatu ondoren, ireki fitxategi hau eta eman **Run**:
     `src/main/java/eus/goierri/jatetxea/App.java`
* **Terminal bidez:**
  ```bash
  cd desktop_javafx
  .\mvnw.cmd javafx:run
  ```
  *(Linux edo Mac-en: `./mvnw javafx:run`)*.

* **Pantailak (Goiko fitxak):**
  1. **Sukaldea (KDS):** Janari platerak bakarrik bistaratzen ditu (Sarrerakoak, Haragiak, Arrainak, Postreak). Botoiak: `Zai`, `Martxan` eta `Eginda markatu`.
  2. **Barra (BDS):** Bar Display System, edariak eta kafeak bakarrik bistaratzen ditu. Botoiak: `Zai`, `Martxan` eta `Zerbitzatuta markatu`.
  3. **Biltegia (Stocka):** Lehengaien uneko stocka, gutxieneko stock alertak gorriz eta albaranen bidezko sarrerak erregistratzeko gunea.

---

#### C) Web Ataria (PHP - Karta eta Erreserbak)
Bezeroentzat kanpoko webgunea:

Terminal batean:
```bash
cd web_php
php -S localhost:8000
```
* Nabigatzailean ireki: `http://localhost:8000`
* Bertan jatetxearen karta kontsultatu eta online erreserbak egin daitezke.

---

## 3. ZIP Fitxategia Sortzeko Gomendioa

Karpeta hau ZIP batean konprimitu aurretik:
1. Itxi Visual Studio eta martxan dauden terminal guztiak.
2. PowerShell bidez ZIP garbia sortu nahi baduzu:
   ```powershell
   Compress-Archive -Path "C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\*" -DestinationPath "C:\Dev\GoierriEskola\1T_Jatetxea_Proiektua.zip" -Force
   ```
3. Zure ikaskideek ZIPa erauzi eta dokumentu honetako pausoak jarraitu besterik ez dute!
