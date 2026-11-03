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

## 4. Zehaztapen Giltzarriak (PDF eta Ereduak)

* `espezifikazioak/T1_jatetxea_erronka_balidazioak_transakzioak.pdf`: Zerbitzariaren tableta interfazea (PIN bidezko saio-hasiera, mahaien zerrenda eta mapa koloreekin, mahaikide kopuruaren hautatzaile modala, produktuen katalogoa eta karrito interaktiboa, blokeo pesimistak `SELECT ... FOR UPDATE` eta transakzio laburrak).
* `espezifikazioak/datu_basea_schema.sql`: Eskema osoa.
* `espezifikazioak/rest_api_kontratua.md`: Endpoint-en kontratua eta salbuespenen kudeaketa.

---

## 5. Hurrengo Ekintza: REST APIaren Garapena (2. Urratsa)

* Agentea: `Backend-API-Agent`
* Karpetan: `backend_api/`
* Zereginak:
  1. `MySqlConnector` liburutegia txertatu.
  2. `appsettings.json` konfiguratuta (`localhost:3308`).
  3. Kontroladoreak eta transakzio pesimistak inplementatzea (`ErreserbakController`, `MahaiakController`, `PlaterakController`, `LehengaiakController`, `EskaerakController`, `IritziakController`, `LangileakController`).
  4. Git commit eta push aldaketa bakoitzeko.
