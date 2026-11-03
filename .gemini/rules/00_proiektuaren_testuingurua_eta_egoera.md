# Proiektuaren Testuingurua, Historia eta Uneko Egoera

Dokumentu honek elkarrizketetan zehar hartutako erabaki, arkitektura, egiaztapen eta hurrengo urrats guztiak biltzen ditu, edozein txat berritan agenteak testuinguru osoa berehala eskura izan dezan.

---

## 1. Proiektuaren Oinarrizko Datuak
- **Izen Ofiziala:** `1T_Jatetxea_erronka_agents`
- **Tokiko Bidea:** `C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents`
- **GitHub Biltegia:** `https://github.com/AnderUrienGoierri/1T_Jatetxea_erronka_agents` (Publikoa)
- **CI/CD:** GitHub Actions lan-fluxua martxan (`.github/workflows/ci.yml`), .NET 10 eta JavaFX eraikuntzak automatikoki egiaztatuz.

---

## 2. Derrigorrezko Arauak (Arau Maila)
1. **Emotikonoen Erabateko Debekua:** Ez erabili emotikonorik kodean, iruzkinetan, dokumentazioan, commit mezuetan ezta txateko erantzunetan ere. Lana profesionala da.
2. **Aldaketa Txikien Commit eta Push Jarraitua:** Egindako urrats, kontroladore, klase edo aldaketa bakoitzeko, berehala egingo da `git commit` eta `git push` mezu esanguratsu batekin (emotikonorik gabe).
3. **3 Mailako Arkitektura:** Aurre-mailak (Web, WinForms, JavaFX) ez du inoiz datu-basearekin zuzenean hitz egiten; dena C# REST API bidez doa.
4. **Segurtasuna:** Prezioak ez dira inoiz frontend-etik onartzen (datu-basetik bakarrik). Zerbitzariaren PINa bcrypt bidez egiaztatzen da. Parametrizatutako kontsultak soilik erabili.

---

## 3. Datu-Basearen Uneko Egoera (MySQL 8.0 InnoDB)
- **Docker Edukiontzia:** `jatetxea_mysql` martxan dago `3308` portuan (`3308:3306`).
- **Datu-basea:** `jatetxea_db` (Erabiltzailea: `root`, Pasahitza: `root`).
- **Eskema (v2.1):** 11 taula prest:
  `lehengaiak`, `platerak`, `plater_osagaiak`, `mahaiak`, `bezeroak`, `langileak`, `erreserbak`, `eskaerak`, `eskaera_lerroak`, `stock_mugimenduak`, eta bezeroen `iritziak` taula (0-5 izarren balorazioa).
- **CHECK Murrizketak:** MySQL 8.0 InnoDB mailan ezarrita (prezioak >= 0, stockak >= 0, edukiera > 0, kantitatea > 0, balorazioa 0..5, mugimenduak <> 0).
- **Ikuspegiak (Views):**
  - `v_erosi_beharreko_lehengaiak`: Gutxieneko stocketik behera dauden lehengaiak.
  - `v_plateren_stock_ahalmena`: Uneko stockaren arabera plater bakoitzetik zenbat anoa prestatu daitezkeen.
  - `v_balorazio_orokorrak`: Iritzien batez bestekoa eta izarren banaketa.
- **Hasierako Datuak:** Txertatuta eta egiaztatuta daude.

---

## 4. Zehaztapen Dokumentu Nagusiak
- `espezifikazioak/T1_jatetxea_erronka_balidazioak_transakzioak.pdf`: Zerbitzariaren tableta-interfazearen diseinua (PIN login, mahaien mapa berdea/gorria/horia, pertsona kopurua hautatzeko modala, karrito interaktiboa, ohar pertsonalizatuak, transakzio laburrak eta blokeo pesimistak `SELECT ... FOR UPDATE`).
- `espezifikazioak/datu_basea_schema.sql`: Datu-basearen eskema osoa.
- `espezifikazioak/rest_api_kontratua.md`: Endpoint-en definizioa eta 409 Conflict kontrola.
- `inplementazio_plana.md`: Garapen faseak (Monorepo eredua).

---

## 5. Uneko Egoera eta Hurrengo Zeregina (2. URRATSA)
- **Eginda:**
  - Karpetak eta eskeletoak sortuta: `backend_api/`, `desktop_csharp/`, `web_php/`, `desktop_javafx/`, `Jatetxea.slnx`.
  - Datu-basea Dockerren martxan eta baliozkotuta (Portua: 3308).
  - GitHub biltegia sinkronizatuta eta CI aktibo.
- **Orain egitekoa:**
  - **`Backend-API-Agent` abiaraztea `backend_api/` karpetan:**
    - `MySqlConnector` liburutegia gehitu.
    - `appsettings.json` fitxategian konexio-katea ezarri (`localhost:3308`).
    - Kontroladoreak eta transakzio pesimistak eraiki: `ErreserbakController`, `MahaiakController`, `PlaterakController`, `LehengaiakController`, `EskaerakController` (`SELECT ... FOR UPDATE` lehengaietan eta errezeta bidezko deskontu automatikoa), `IritziakController` eta `LangileakController` (PIN bidezko saio-hasiera).
    - Aldaketa bakoitzeko: `git commit` eta `git push` (emotikonorik gabe).
