# Proiektuaren Testuingurua eta Egoera Eguneratua

Dokumentu honek elkarrizketetan zehar hartutako erabaki, arkitektura, egiaztapen eta arau guztiak biltzen ditu.

## 1. Identifikazioa eta Errepositorioa
- Proiektua: `1T_Jatetxea_erronka_agents`
- GitHub Urruneko Biltegia: `https://github.com/AnderUrienGoierri/1T_Jatetxea_erronka_agents`
- CI/CD konfigurazioa: `.github/workflows/ci.yml` (C# .NET 10 eta JavaFX Maven eraikuntzak).

## 2. Arau Profesionalak (Derrigorrezkoak)
- **Emotikonorik Ez:** Proiektu osoan (README, kodea, iruzkinak, commit mezuak eta erantzunak) emotikonoen erabilera erabat debekatuta dago.
- **Aldaketa Txikien Commit eta Push:** Urrats, kontroladore edo funtzionalitate bakoitza amaitzean berehala egingo dira `git commit` eta `git push`.
- **Arkitektura Hirukoitza:** Bezeroek (`web_php/`, `desktop_csharp/`, `desktop_javafx/`) C# REST APIaren bidez soilik komunikatuko dira datu-basearekin.
- **Prezioen Segurtasuna:** Frontend-ek ez dute inoiz preziorik bidaliko; backend-eko datu-basetik bakarrik irakurriko dira.

## 3. Datu-Basea (MySQL 8.0 InnoDB Dockerren)
- Portua: `3308` (localhost:3308, jatetxea_db, root/root).
- Eskema eta taulak: `lehengaiak`, `platerak`, `plater_osagaiak`, `mahaiak`, `bezeroak`, `langileak`, `erreserbak`, `eskaerak`, `eskaera_lerroak`, `stock_mugimenduak`, `iritziak`.
- CHECK murrizketak zenbakizko eremu guztietan aktibo.
- Ikuspegiak: `v_erosi_beharreko_lehengaiak`, `v_plateren_stock_ahalmena`, `v_balorazio_orokorrak`.

## 4. REST APIa (backend_api/) - Garatuta eta Egiaztatuta
- Kontroladoreak: `MahaiakController`, `LehengaiakController`, `PlaterakController`, `ErreserbakController`, `EskaerakController`, `BezeroakController`, `LangileakController`, `IritziakController`.
- Transakzio pesimistak `SELECT ... FOR UPDATE` bidez.
- Gatazken kontrola: 1062 errorea 409 Conflict bihurtua.
- Stock deskontu automatikoa platerak eskatzean errezeta eta proporzioen arabera.

## 5. Web Frontend-a (web_php/) - Garatuta eta Egiaztatuta
- HTML5, CSS3 sentikorra eta JavaScript asinkronoa (Fetch) PHP ingurunean.
- Karta dinamikoa: lehengaien stockaren arabera anoa kalkulua eta agortutako platerak automatikoki desgaitzea.
- Mahai erreserba sistema interaktiboa: data, txanda, mahaia eta bezeroaren datuak, 409 Conflict gatazken kudeaketa atseginarekin.
- Bezeroen iritziak eta izar bidezko balorazio estatistikak.

## 6. Hurrengo Urratsak
- 4. Urratsa: `Desktop-CSharp-Agent` (`desktop_csharp/`)
- 5. Urratsa: `Desktop-JavaFX-Agent` (`desktop_javafx/`)
