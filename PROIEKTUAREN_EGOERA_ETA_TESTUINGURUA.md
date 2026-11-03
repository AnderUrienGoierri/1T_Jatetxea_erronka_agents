# Proiektuaren Egoera eta Testuinguru Osoa (1T_Jatetxea_erronka_agents)

Dokumentu honek orain arte egindako aurrerapen, erabaki tekniko, arau eta hurrengo urrats guztiak biltzen ditu, edozein unetan edo saio berritan lana jarraitzeko.

---

## 1. Identifikazioa eta Repositorioa

* **Proiektuaren Izena:** `1T_Jatetxea_erronka_agents`
* **Tokiko Kokapena:** `C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents`
* **GitHub Biltegia:** [https://github.com/AnderUrienGoierri/1T_Jatetxea_erronka_agents](https://github.com/AnderUrienGoierri/1T_Jatetxea_erronka_agents)
* **CI/CD:** `.github/workflows/ci.yml` bidezko etengabeko integrazioa (C# .NET 10 eta JavaFX eraikuntzak egiaztatzen).

---

## 2. Arau Profesionalak (Derrigorrezkoak)

1. **Emotikonorik Ez:** Proiektu osoan (README, kodea, iruzkinak, commit mezuak eta txat erantzunak) ez da emotikonorik erabiliko.
2. **Aldaketa Txikien Commit eta Push:** Funtzionalitate edo fitxategi bakoitza amaitu bezain laster, `git commit` eta `git push` egingo dira berehala GitHub-era.
3. **Arkitektura Hirukoitza:** Bezeroek (`web_php/`, `desktop_csharp/`, `desktop_javafx/`) ez dute datu-basearekin zuzenean hitz egiten; komunikazio guztia C# REST APIaren bidez egingo da.
4. **Prezioen Segurtasuna:** Frontend-etik bidalitako prezio bakar bat ere ez da onartuko; backend-eko datu-basetik bakarrik irakurriko dira.

---

## 3. Datu-Basea (MySQL 8.0 InnoDB Dockerren)

* **Portua:** `3308` (3306 Windows zerbitzuak eta 3307 beste edukiontzi batek hartuta dutelako).
* **Kredentzialak:** `localhost:3308`, DB: `jatetxea_db`, Erabiltzailea: `root`, Pasahitza: `root`.
* **Taulak (11):** `lehengaiak`, `platerak`, `plater_osagaiak`, `mahaiak`, `bezeroak`, `langileak`, `erreserbak`, `eskaerak`, `eskaera_lerroak`, `stock_mugimenduak`, eta `iritziak` (0-5 izar arteko balorazioarekin).
* **CHECK Murrizketak:** Aktibo MySQL 8.0 mailan eremu zenbakizko guztietan.
* **Ikuspegiak:** `v_erosi_beharreko_lehengaiak`, `v_plateren_stock_ahalmena`, `v_balorazio_orokorrak`.

---

## 4. REST APIa (backend_api/) - EGOERA: EGINDA ETA EGIAZTATUTA

* **Teknologia:** C# ASP.NET Core (.NET 10).
* **Arkitektura:** Controllers, DTO, Models, Responses (`ApiResponse<T>`, `ApiError`), `BaseApiKontroladorea`.
* **Giltzarriak eta Transakzio Pesimistak:**
  - `POST /api/v1/erreserbak`: `SELECT ... FOR UPDATE` mahaia blokeatuz, txanda/data gatazkak egiaztatuz eta 1062 errorea 409 Conflict bihurtuz.
  - `POST /api/v1/eskaerak/{id}/lerroak`: `SELECT ... FOR UPDATE` lehengaiak blokeatuz, errezeta bidezko stock deskontua, prezioa datu-basetik soilik eskuratzea eta `stock_mugimenduak` taulan auditoria erregistratzea.
  - `POST /api/v1/lehengaiak/sarrera`: Hornitzailearen albarana erregistratzea transakzioan eta stocka gehitzea.
  - `GET /api/v1/lehengaiak/alertak`: Gutxieneko stocketik behera dauden lehengaien erosketa-zerrenda.
  - `GET /api/v1/platerak/erabilgarritasuna`: Lehengaien uneko stockaren arabera zenbat plater prestatu daitezkeen kalkulatuta.
  - `POST /api/v1/langileak/login`: PIN edo erabiltzaile bidezko saio-hasiera.
  - `GET /api/v1/iritziak/estatistikak`: 0-5 izarren banaketa eta batez besteko balorazio orokorra.

---

## 5. Web Frontend-a (web_php/) - EGOERA: EGINDA ETA EGIAZTATUTA

* **Teknologia:** HTML5, CSS3 modernoa eta sentikorra, JavaScript asinkronoa (Fetch) eta PHP.
* **Fitxategiak:**
  - `web_php/index.php`: Interfaze orokorra (Goiburua, Hero, Karta kategoria iragazkiekin, Erreserba panela, Iritziak estatistikekin eta Kontaktua).
  - `web_php/assets/css/style.css`: Estilo garbiak, paleta profesionala, txartel sentikorrak, erreserba inprimakia eta jakinarazpen alertak.
  - `web_php/assets/js/app.js`: REST APIarekiko konexio asinkronoa, stock erabilgarritasunaren arabera agortutako platerak automatikoki desgaitzea, 409 Conflict gatazken kudeaketa atsegina eta balorazio sistema.

---

## 6. Hurrengo Urratsak (Desktop Aplikazioak)

1. **4. Urratsa (Desktop-CSharp-Agent):** `desktop_csharp/` garatzea jantokiko zerbitzariaren tableta/TPV interfazerako (.NET 10 WinForms):
   - PIN bidezko saio-hasiera (`POST /api/v1/langileak/login`).
   - Mahaien mapa koloreen arabera (`Libre`, `Okupatuta`, `Erreserbatuta`, `Garbitzen`).
   - Mahaikide kopuruaren hautatzaile modala.
   - Produktuen katalogoa eta komandaren karrito interaktiboa (`POST /api/v1/eskaerak/{id}/lerroak`).
2. **5. Urratsa (Desktop-JavaFX-Agent):** `desktop_javafx/` garatzea sukaldeko KDS pantailarako eta biltegiko erosketa-zerrendarako (Java 21 JavaFX MVC).
