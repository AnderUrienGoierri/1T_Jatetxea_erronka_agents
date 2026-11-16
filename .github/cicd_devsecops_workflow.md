# CI/CD eta DevSecOps Lan-Fluxuaren Dokumentazioa

Dokumentu honek **1T Jatetxea Kudeaketa Sistema Integralerako** GitHub Actions bidezko etengabeko integrazioa (CI), segurtasun-egiaztapenak (DevSecOps) eta kode-kalitatearen kontrol automatizatuak zehazten ditu.

---

## 1. Lan-Fluxuaren Arkitektura Orokorra

CI pipeline-a `.github/workflows/ci.yml` fitxategian definituta dago eta automatikoki exekutatzen da:
- `main` adarrera egindako `push` bakoitzean.
- `main` adarrera bideratutako edozein `pull_request`-etan.

Komit berriak jarraian igortzen badira, konkurrentzia-taldeek (`concurrency`) aurreko exekuzio erredundanteak automatikoki bertan behera uzten dituzte baliabideak optimizatzeko.

```text
Commit / Pull Request
   │
   ├── 1. Lana: DevSecOps & Isilpeko Datuen Auditoretza (Linux)
   │     ├── Zero Secret Leakage (giltzak, pasahitzak, tokenak)
   │     └── Komit-mezuen formatu-egiaztapena (Conventional Commits, zero emoji)
   │
   ├── 2. Lana: ASP.NET Core REST API (.NET 10, Linux)
   │     ├── NuGet mendekotasunak berreskuratu eta konpilatu
   │     └── Kontratuen eta kontroladoreen osotasun-proba
   │
   ├── 3. Lana: Admin IT Zentrala (.NET, Linux)
   │     ├── Kudeaketa-zerbitzaria konpilatu (Jatetxea.AdminZentrala)
   │     └── wwwroot estatikoen osotasun-egiaztapena
   │
   ├── 4. Lana: Desktop C# Windows Forms (.NET 10, Windows)
   │     ├── Jatetxea.TPV konpilazioa
   │     └── UI baliabideen eta inprimatze-moduluen egiaztapena
   │
   ├── 5. Lana: Desktop JavaFX Sukaldea KDS (Temurin JDK 21, Linux)
   │     ├── FXML fitxategien XML baliozkotzea
   │     └── Maven konpilazioa (mvn test-compile)
   │
   └── 6. Lana: Web PHP Frontend (PHP 8.4, Linux)
         └── PHP sintaxiaren linting automatikoa (php -l)
```

---

## 2. DevSecOps eta Segurtasun Jarraibideak

### 2.1 Zero Secret Leakage (Isilpeko Datuen Isurketarik Ez)
1. **Datu-baseko pasahitzak eta kredentzialak:**
   - Erabat debekatuta dago MySQL ekoizpeneko pasahitzak, OPNsense gakoak edo API tokenak git-era igotzea.
   - Ezarpen guztiak ingurune-aldagaien bidez (`ASPNETCORE_ENVIRONMENT`, `DB_PASSWORD`, `.env.example`) kudeatu behar dira.
2. **GitLeaks eta Scan Automatikoa:**
   - CI-ak fitxategiak miatzen ditu RSA gako pribatuak, pasahitz gogorrak edo konexio-kate arriskutsuak detektatzeko.

### 2.2 Eraikuntza Deterministak (Deterministic Builds)
- Proiektuko kudeatzaile guztiek mendekotasun-bertsio finkoak erabili behar dituzte:
  - .NET: `Directory.Build.props` edo `.csproj` bertsio esplizituekin.
  - Maven: `pom.xml` bertsio finkoekin, bertsio aldakorrak (`LATEST`, `RELEASE`) ekidinez.
  - PHP: `composer.lock` fitxategiaren bidez.

### 2.3 Sare eta Isolamendu Arkitektura (Least Privilege)
- Aplikazio bakoitzak dagokion VLAN gunean jardun behar du:
  - **VLAN 10 (LAN Pribatua):** Backend REST API (5050), MySQL (3308), Desktop TPV, KDS JavaFX.
  - **VLAN 20 (DMZ Publikoa):** Web PHP Frontend (8085).
  - **VLAN 99 (Kudeaketa):** OPNsense WebGUI (8443), SSH/RDP atakak.
- Web PHP aplikazioak ez dauka inoiz sarbide zuzenik MySQL datu-basera; REST API bidez soilik komunikatzen da.

---

## 3. Komit eta Garapen Estandarrak

1. **Conventional Commits Derrigorrezkoa:**
   - Komit-mezu guztiek `mota(esparrua): deskribapena` formatua errespetatu behar dute (`feat`, `fix`, `refactor`, `docs`, `style`, `test`, `ci`, `chore`).
2. **Zero Emoji Arau Zorrotza:**
   - Kodean, iruzkinetan, dokumentazioan, komit-mezuetan eta PR tituluetan ez da onartzen inolako emojirik.
3. **Commit Atomikoak:**
   - Pantaila edo funtzionalitate bakoitzeko komit atomiko bakar bat egiten da, probatu eta egiaztatu ondoren.
