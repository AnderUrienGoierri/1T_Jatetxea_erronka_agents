# Proiektuaren Egoera eta Testuinguru Osoa (1T_Jatetxea_erronka_agents)

Dokumentu honek proiektuaren egoera osoa, arkitektura erabakiak, inplementatutako modulu guztiak eta arau profesionalak biltzen ditu, edozein unetan edo saio berritan lana jarraitzeko.

---

## 1. Identifikazioa eta Repositorioa

* **Proiektuaren Izena:** `1T_Jatetxea_erronka_agents`
* **Tokiko Kokapena:** `C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents`
* **GitHub Biltegia:** [https://github.com/AnderUrienGoierri/1T_Jatetxea_erronka_agents](https://github.com/AnderUrienGoierri/1T_Jatetxea_erronka_agents)
* **CI/CD:** `.github/workflows/ci.yml` bidezko etengabeko integrazioa (JavaFX 17 eta .NET 10 eraikuntzak eta probak egiaztatzen, berdez).

---

## 2. Arau Profesionalak (Derrigorrezkoak)

1. **Emotikonorik Ez:** Proiektu osoan (README, kodea, iruzkinak, commit mezuak eta txat erantzunak) ez da emotikonorik erabiliko.
2. **Aldaketa Txikien Commit eta Push:** Funtzionalitate edo fitxategi bakoitza amaitu bezain laster, `git commit` eta `git push` egingo dira berehala GitHub-era.
3. **Arkitektura Hirukoitza:** Bezeroek (`web_php/`, `desktop_csharp/`, `desktop_javafx/`) ez dute datu-basearekin zuzenean hitz egiten; komunikazio guztia C# REST APIaren bidez egingo da.
4. **Prezioen Segurtasuna:** Frontend-etik bidalitako prezio bakar bat ere ez da onartuko; backend-eko datu-basetik bakarrik irakurriko dira transakzio seguruetan.
5. **Datu Errealak Soilik:** API zerbitzaria deskonektatuta badago, bezeroek konexio eza jakinaraziko dute; ez da datu faltsurik (mock) erabiliko modu isilean.

---

## 3. Datu-Basea (MySQL 8.0 InnoDB Dockerren)

* **Portua:** `3308` (3306 Windows zerbitzuak eta 3307 beste edukiontzi batek hartuta dutelako).
* **Kredentzialak:** `localhost:3308`, DB: `jatetxea_db`, Erabiltzailea: `root`, Pasahitza: `root`.
* **Taulak (11):**
  * `lehengaiak`: 47 lehengai orotara biltegian (unitateak, gutxieneko stocka eta uneko stocka).
  * `platerak`: 35 plater eta edari zabalduta (Kategoriak: Sarrerakoa, Lehena, Bigarrena, Postrea, Edaria).
  * `plater_osagaiak`: Errezeta eta eskandallo proportzio osoak plater bakoitzeko.
  * `mahaiak`: Jatetxeko mahaiak (zenbakia, edukiera, kokapena eta egoera: Librea, Okupatuta, Eskaera bidean, Garbitzen).
  * `bezeroak`: Bezeroen erregistroa.
  * `langileak`: Langileen erregistroa eta PIN zenbakiak saio-hasierarako.
  * `erreserbak`: Erreserben kudeaketa.
  * `eskaerak`: Eskaeren / komanden buruak (mahai bakoitzeko eskaera bat edo gehiago egon daitezke kontu banatuentzat).
  * `eskaera_lerroak`: Eskaeren plater lerroak, kantitateak, unitate prezioak eta egoerak (Eskatua, Sukaldean, Prestatua, Zerbitzatua).
  * `stock_mugimenduak`: Stock deskontuen eta albaranen sarreren auditoria osoa.
  * `iritziak`: Bezeroen balorazioak (0-5 izar) eta iruzkinak.
* **CHECK Murrizketak:** Aktibo MySQL 8.0 mailan eremu zenbakizko guztietan.
* **Ikuspegiak:** `v_erosi_beharreko_lehengaiak`, `v_plateren_stock_ahalmena`, `v_balorazio_orokorrak`.

---

## 4. REST APIa (backend_api/) - EGOERA: EGINDA ETA EGIAZTATUTA

* **Teknologia:** C# ASP.NET Core (.NET 10).
* **Arkitektura:** Controllers, DTO, Models, Responses (`ApiResponse<T>`, `ApiError`), `BaseApiKontroladorea`.
* **Giltzarriak eta Transakzio Pesimistak:**
  * `POST /api/v1/erreserbak`: `SELECT ... FOR UPDATE` mahaia blokeatuz, txanda/data gatazkak egiaztatuz eta 1062 errorea 409 Conflict bihurtuz.
  * `POST /api/v1/eskaerak`: Mahai batean eskaera irekitzea. Mahai okupatuetan ere baimentzen du kontu berriak sortzea mahaikide banatuen kasuan (`Okupatuta` zein `Eskaera bidean` egoeretan).
  * `POST /api/v1/eskaerak/{id}/lerroak`: `SELECT ... FOR UPDATE` lehengaiak blokeatuz, errezeta bidezko stock deskontu automatikoa, prezioa datu-basetik soilik eskuratzea eta `stock_mugimenduak` taulan auditoria erregistratzea.
  * `POST /api/v1/eskaerak/{id}/sukaldera`: Zai dauden platerak sukaldera bidaltzea (`Sukaldean` egoerara pasaz).
  * `PUT /api/v1/eskaerak/{id}/egoera`: Eskaeraren egoera aldatzea (`Irekia`, `Sukaldean`, `Zerbitzatua`, `Kobratua`, `Ezeztatua`). `Kobratua` ezartzean, mahaia `Garbitzen` egoerara pasatzen da bakarrik mahai horretan beste kontu aktiborik geratzen ez bada.
  * `PUT /api/v1/eskaerak/lerroak/{lerroId}/egoera`: Plater lerro baten egoera aldatzea (`Eskatua`, `Sukaldean`, `Prestatua`, `Zerbitzatua`).
  * `POST /api/v1/lehengaiak/sarrera`: Hornitzailearen albarana erregistratzea transakzioan eta stocka gehitzea.
  * `GET /api/v1/lehengaiak/alertak`: Gutxieneko stocketik behera dauden lehengaien zerrenda.
  * `GET /api/v1/platerak/erabilgarritasuna`: Lehengaien uneko stockaren arabera zenbat plater prestatu daitezkeen kalkulatuta.
  * `POST /api/v1/langileak/login`: PIN edo erabiltzaile bidezko saio-hasiera.
  * `GET /api/v1/iritziak/estatistikak`: 0-5 izarren banaketa eta batez besteko balorazio orokorra.

---

## 5. Web Frontend-a (web_php/) - EGOERA: EGINDA ETA EGIAZTATUTA

* **Teknologia:** HTML5, CSS3 modernoa eta sentikorra, JavaScript asinkronoa (Fetch) eta PHP.
* **Fitxategiak:**
  * `web_php/index.php`: Interfaze orokorra (Goiburua, Hero, Karta kategoria iragazkiekin, Erreserba panela, Iritziak estatistikekin eta Kontaktua).
  * `web_php/assets/css/style.css`: Estilo garbiak, paleta profesionala, txartel sentikorrak, erreserba inprimakia eta jakinarazpen alertak.
  * `web_php/assets/js/app.js`: REST APIarekiko konexio asinkronoa, stock erabilgarritasunaren arabera agortutako platerak automatikoki desgaitzea, 409 Conflict gatazken kudeaketa atsegina eta balorazio sistema.

---

## 6. Desktop TPV Zerbitzaria (desktop_csharp/) - EGOERA: EGINDA ETA EGIAZTATUTA

* **Teknologia:** C# .NET 10 Windows Forms, Tablet POS diseinu sentikorra (1366x850 bereizmena).
* **Fitxategiak eta Egitura:**
  * `desktop_csharp/Forms/LoginForm.cs`: PIN bidezko sarbide azkarra zerbitzarien avatarrekin. Lineaz kanpo badago APIaren konexio eza argi adierazten du.
  * `desktop_csharp/Forms/MahaiakForm.cs`: Mahaien mapa guneen arabera (Jangela Nagusia, Terraza, Barra, VIP, Guztiak) eta egoeren araberako koloreekin (Librea, Okupatuta, Eskaera bidean, Garbitzen). 15 segundoro automatikoki freskatzen da.
  * `desktop_csharp/Forms/MahaiaXehetasunakForm.cs`: Mahaiaren xehetasun osoak:
    * Mahai berean hainbat kontu kudeatzea (`1. Kontua`, `2. Kontua`, etab.).
    * Sukaldera bidali gabeko platerak bidaltzea ("SUKALDERA BIDALI" botoia).
    * Kontu berria gehitzea mahaikide banatuentzat ("+ KONTU BERRIA (Mahaikide Banatua)").
    * Eskaerari plater gehiago gehitzea ("+ PRODUKTUAK GEHITU").
    * Kontua kobratzea ("KONTUA ESKATU / KOBRATU"); azken kontua ordaintzean mahaia automatikoki "Garbitzen" egoeran uzten du.
  * `desktop_csharp/Forms/EskaeraForm.cs`: Komandaren produktuen hautaketa (kategoriak, plater txartelak, karrito interaktiboa ohar bereziekin eta laburpen ekonomikoarekin).
  * `desktop_csharp/Services/ApiService.cs`: REST APIarekiko komunikazio asinkronoa, errore-mezu zehatzen tratamendua eta JSON serializazio zorrotza.
* **UX/UI eta High-DPI:**
  * `ApplicationHighDpiMode = DpiUnaware` eta `UseCompatibleTextRendering = false` bidez, Windows-eko eskalan (%125 edo %150 DPI) testuak eta zenbakiak moztu gabe eta txukun bistaratzen dira.

---

## 7. Desktop Sukaldea, Barra eta Biltegia (desktop_javafx/) - EGOERA: EGINDA ETA EGIAZTATUTA

* **Teknologia:** Java 17/21, JavaFX 21, FXML, CSS eta Maven.
* **MVC Arkitektura:**
  * `model/`: `Eskaera`, `EskaeraLerroa`, `Lehengaia`, `Mahaia` (JavaFX propietate beagarriekin).
  * `api/`: `ApiClient` (Java 11 `HttpClient` asinkronoa, Jackson JSON analisia, errore kudeaketa eta konexio egoera).
  * `controller/`:
    * `SukaldeaController`: Sukaldeko KDS eskaera aktiboak eta platerak kudeatzeko (Sarrerakoak, Lehenak, Bigarrenak, Postreak). Plater bakoitzaren egoera aldatzeko botoiak (Zai, Martxan, Eginda) eta eskaera osoa eginda markatzeko botoia.
    * `BarraController`: Barrako BDS pantaila bereizia (Bar Display System), edariak eta kafeak bakarrik bistaratzeko eta prestatzeko (`Kategoria == 'Edaria'`).
    * `BiltegiaController`: Biltegiko lehengaien zerrenda osoa, uneko stocka, gutxieneko stocka, stock alertak gorriz eta albaranen bidezko sarrerak erregistratzeko inprimakia.
    * `PantailaAldatzailea`: Goiko nabigazio-fitxak aldatzeko zerbitzua (`Sukaldea (KDS)`, `Barra (BDS)`, `Biltegia (Stocka)`).
  * `resources/`: `sukaldea-view.fxml`, `barra-view.fxml`, `biltegia-view.fxml`, `styles.css`.
* **Irisgarritasuna eta UX:** Kontraste handiko TableView estiloak hautatutako lerroan (`#881337` hondo ardo-iluna eta letra zuri lodiak) datuak argi irakurtzeko. Auto-freskatze automatikoa (10 segundoro).

---

## 8. Proiektuaren Uneko Egoera Orokorra

Proiektuko hiru mailak (MySQL DB, ASP.NET Core REST API, eta 3 Aurre-Mailako bezeroak: Web PHP, C# Desktop POS eta JavaFX KDS/Stock) guztiz osatuta, egiaztatuta eta elkarrekin konektatuta daude.
CI eraikuntza automatikoa (`CI - Build & Quality Gate`) gainditu da bi plataformetan (.NET 10 eta Java 17).
