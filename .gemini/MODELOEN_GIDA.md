# Antigravity & Gemini Modeloen Gida: 1T Jatetxea Proiektua

Dokumentu honek Antigravity IDEko modelo hautatzailean agertzen diren adimen artifizialeko ereduen ezaugarriak, indarguneak eta **1T Jatetxea Erronkan** duten erabilera praktiko zehatza azaltzen du.

---

## 1. Modeloen Ikuspegi Orokorra eta Konparaketa

Antigravity IDEk hainbat arrazoibide-maila, abiadura eta orkestrazio-ahalmen eskaintzen ditu garapen-fase bakoitzerako tresna egokiena hautatzeko:

| Eredua | Mota | Arrazoibide Maila | Latentzia / Abiadura | Erabilera Nagusia Proiektuan |
| :--- | :--- | :--- | :--- | :--- |
| **Gemini 4 Neon (Medium)** | Bakarkakoa | Gorena (Frontier Deep Reasoning) | Ertaina | Arkitektura konplexua, segurtasuna, debug sakona, C# & JavaFX integrazioa |
| **Gemini 4 Argon (Medium)** | Bakarkakoa | Altua (Balanced Performance) | Azkarra | Eguneroko garapena: CRUD, FXML kontroladoreak, DTOak, probak |
| **Gemini 4 Hydrogen (Medium)** | Bakarkakoa | Oinarrizkoa / Zehatza | Ultra-azkarra | Commit mezuak, script txikiak, formatu aldaketak, sintaxi konponketa azkarrak |
| **Orchestrator (Neon)** | Multi-Agent / Swarm | Estrategikoa & Sakona | Dinamikoa | Modulu anitzeko epikak (DB -> API -> UI -> TV), 8 agenteen koordinazioa |
| **Orchestrator (Argon)** | Multi-Agent / Swarm | Eraginkorra & Egituratua | Bizkorra | Build & test pipeline-ak, egiaztapen masiboak, bateratze atazak |
| **Gemini 3.8 Flash (Medium)** | Bakarkakoa | Arina (High-Throughput) | Oso azkarra | Log erraldoiak aztertzea (Docker, adb logcat), bilaketak eta laburpenak |

---

## 2. Modelo Bakoitzaren Azterketa eta Erabilera Proiektuan

### 2.1. Gemini 4 Neon (Medium)
*Arrazoibide sakoneko motor nagusia.*

- **Ezaugarriak:**
  - Kode-sorkuntzan eta arazo abstraktuen ebazpenean kalitate goreneko emaitzak.
  - Testuinguru erraldoia (1M+ token) eta fitxategi anitzen arteko erlazioak ulertzeko gaitasun aparta.
  - Multimodalitate aurreratua: Arkitektura-diagramak, eskuzko zirriborroak eta UI pantaila-argazkiak xehetasun handiz interpretatzen ditu.

- **Nola erabili 1T Jatetxea proiektuan:**
  1. **Arkitektura eta Datu-baseko Transakzio Kritikoak:** MySQL InnoDB-n eskaeren eta sukaldeko egoeren arteko konkurrentzia-arazoak (pessimistic locking, deadlocks, isolamendu-mailak) modelatzeko.
  2. **ASP.NET Core REST APIren Nukleoa:** JWT bidezko autentifikazioa, rol-oinarritutako baimenak (RBAC) eta middleware pertsonalizatuak diseinatzeko.
  3. **Android TV & JavaFX Arazo Egiturazkoak:** Gradle / AGP (Android Gradle Plugin) bateragarritasun korapilatsuak, ProGuard arauak eta JavaFX harpidetza/multithreading errore konplexuak ebazteko.
  4. **Segurtasun eta Azpiegitura Diseinua:** OPNsense suebakia, DMZ isolamendua eta Cisco switch VLAN konfigurazioen baliozkotzea.
  5. **Gomendatutako Agenteak:** `agente_01_arkitektura_eta_datubasea`, `agente_02_backend_eta_segurtasuna`, `agente_06_sareak_eta_it_azpiegitura`.

---

### 2.2. Gemini 4 Argon (Medium)
*Eguneroko kodegintza eta inplementaziorako lan-zaldi orekatua.*

- **Ezaugarriak:**
  - Abiaduraren eta arrazoibide-kalitatearen arteko oreka ezin hobea.
  - Egituratutako kodea (JSON, DTO, SQL DDL/DML, FXML) sortzeko oso fidagarria.
  - Erantzun-denbora azkarra, garapen iteratiborako aproposa.

- **Nola erabili 1T Jatetxea proiektuan:**
  1. **JavaFX Mahaigaineko Aplikazioak:** `stock_kudeaketa`, `langileak_kudeaketa`, `jantokia_kudeaketa`, `sukaldea_kds` eta `barra_bds` FXML fitxategiak eta haien kontroladoreak idazteko.
  2. **API Endpoint eta Kontroladore Estandarrak:** C# ASP.NET Core-n CRUD eragiketak (Mahaiak, Platerak, Lehengaiak, Kategoriak) eta DTO bihurketak egiteko.
  3. **Unitate Probak eta Integrazio Probak:** JUnit 5 (Java) eta xUnit (C#) proba kasuak automatikoki sortzeko.
  4. **Gomendatutako Agenteak:** `agente_03_sukaldea_kds`, `agente_04_barra_bds`, `agente_05_kudeaketa_mahaigaina`, `agente_08_qa_eta_probak`.

---

### 2.3. Gemini 4 Hydrogen (Medium)
*Latentzia ultra-baxuko modelo arina.*

- **Ezaugarriak:**
  - Erantzun ia berehalakoak (segundo gutxitan).
  - Testuinguru murriztu edo zehatzetan eraginkortasun handia.
  - Baliabide-kontsumo baxua.

- **Nola erabili 1T Jatetxea proiektuan:**
  1. **Git Commit Mezuak:** `.github/COMMIT_CONVENTION.md` arauak errespetatuz commit mezu garbiak sortzeko (adib. `fix(kds): konpondu tenporizadorearen kolorea`).
  2. **Script Txikiak eta Automatizazioa:** `.bat`, PowerShell edo Bash scriptetan lerro zehatzak aldatzeko (adib. `instalatu_android_tv.bat` egokitzea).
  3. **Sintaxi eta Formatatze Zuzenketak:** Kompilazio-errore txikiak (faltan den kakotx bat, import ahaztu bat, CSS propietate bat) konpontzeko.
  4. **Kontsulta Azkarrak:** Komando baten sintaxia gogoratzeko (adib. `adb connect IP:PORT` edo `docker compose restart`).

---

### 2.4. Orchestrator (Neon)
*Arrazoibide estrategikoko orkestratzaile adimenduna.*

- **Ezaugarriak:**
  - Hainbat azpi-agenteren (`subagents` / `swarm`) koordinazio hierarkikoa kudeatzen du.
  - Problemak urrats estrategikoetan deskonposatzeko ahalmen handia.
  - Lan-fluxu paraleloak sinkronizatzen ditu eta agente bakoitzaren emaitzak bateratzen ditu.

- **Nola erabili 1T Jatetxea proiektuan:**
  1. **Modulu Anitzeko Epikak (End-to-End Features):**
     - Adibidez: *«Eskaera berri bat sortu eta sukaldera bidali»*.
     - Orchestrator (Neon)-ek lana banatzen du:
       - Agente 01: MySQL taulak prestatu (`Eskaera`, `EskaeraLerroa`).
       - Agente 02: C# REST API endpointak sortu (`POST /api/eskaerak`).
       - Agente 04: Barra BDS interfazea eguneratu.
       - Agente 03: Sukaldea KDS Android TV-n jakinarazpena jaso.
       - Agente 08: Integrazio probak egiaztatu.
  2. **Proiektuaren Egitura Nagusiaren Eraldaketak:** Karpeten berrantolaketa, moduluen arteko mendekotasun aldaketak eta bertsio-askapen planak.

---

### 2.5. Orchestrator (Argon)
*Pipeline eta eragiketa anitzeko orkestratzaile azkarra.*

- **Ezaugarriak:**
  - Multi-agente ataza errepikakorretan eta pipeline estandarretan optimizatua.
  - Abiadura handiagoa Neon orkestratzailea baino, errutina ezagunak azkar exekutatzeko.

- **Nola erabili 1T Jatetxea proiektuan:**
  1. **Proiektu Osoaren Konpilazio eta Proba Kateak:**
     - Modulu guztiak sekuentzian eraikitzea (`mvn clean package` JavaFX moduluetan, `dotnet build` C# APIan, Gradle Android TVn).
  2. **Fitxategi-egituraren eta Dokumentazioaren Auditoria:**
     - `.agents/` karpetako fitxategi guztien egiaztapena eta txostenak eguneratzea.
  3. **Kode-estiloaren eta Linting-aren Bateratzea:**
     - Modulu guztietako izendapen-hitzarmenen berrikusketa masiboa.

---

### 2.6. Gemini 3.8 Flash (Medium)
*Testuinguru handiko eta kostu baxuko modelo arina.*

- **Ezaugarriak:**
  - Token bolumen handiak oso denbora laburrean prozesatzeko gaitasuna.
  - Bilaketa eta laburpen gaitasun azkarra.

- **Nola erabili 1T Jatetxea proiektuan:**
  1. **Log Luzeen Analisia:**
     - Docker edukiontzien log-ak (`docker compose logs`) aztertzea erroreak bilatzeko.
     - Android TV-ren `adb logcat` irteera erraldoian crash-ak eta salbuespenak iragaztea.
     - MySQL query log luzeetan kontsulta motelenak identifikatzea.
  2. **Dokumentazio Luzeen Sintesia:**
     - PDF eskuliburuak, erronkaren oinarri-dokumentuak edo sareko diagramak azkar irakurri eta laburtzea.

---

## 3. Erabilera Matrizea: 1T Jatetxea Azpisistemak

Hona hemen proiektu honetako atal bakoitzari dagokion modelo gomendatua:

| Azpisistema / Modulua | Teknologia | Gomendatutako Eredua | Arrazoia |
| :--- | :--- | :--- | :--- |
| **REST API & Backend** | C# ASP.NET Core 8 | **Gemini 4 Argon** (CRUD) / **Neon** (Arkitektura/Segurtasuna) | Egitura sendoa eta API errendimendua bermatzeko |
| **Datu-basea (MySQL)** | MySQL 8 / Docker | **Gemini 4 Neon** | Datu-osotasuna, gako arrotzak, indizeak eta transakzioak |
| **KDS Android TV** | Kotlin / Jetpack Compose | **Gemini 4 Neon** | AGP, SDK bertsioak eta hardware kudeaketa zorrotza |
| **Mahaigaineko App-ak** | Java 21 / JavaFX | **Gemini 4 Argon** | FXML eta kontroladore egituratuak azkar sortzeko |
| **IT Zerbitzaria & Admin** | C# WinForms / JavaFX | **Gemini 4 Argon** | Administrazio pantailak eta instalatzaileak eraikitzeko |
| **Sareak & OPNsense** | OPNsense / Cisco Packet Tracer | **Gemini 4 Neon** | Sare-segmentazioa, VLANak eta suebaki-arauak |
| **Git & Commit-ak** | Git / GitHub | **Gemini 4 Hydrogen** | Conventional Commits mezu azkar eta garbiak sortzeko |
| **Log Analisia & Debug** | Logcat / Docker logs | **Gemini 3.8 Flash** | Testuinguru handiko testuak azkar irakurtzeko |
| **Modulu arteko fluxuak** | Sistema Osoa | **Orchestrator (Neon)** | 8 agenteak koordinatu eta integratzeko |

---

## 4. Lan-fluxu Gomendatua (Nola aldatu eredua une bakoitzean)

Garapen-saio eraginkor batean, komeni da unean uneko atazaren arabera eredua aldatzea:

1. **Hasieran (Plangintza orokorra):** Hautatu **Gemini 4 Neon** edo **Orchestrator (Neon)** ataza handia definitzeko eta arkitektura finkatzeko.
2. **Inplementazioan (Kodegintza erreala):** Aldatu **Gemini 4 Argon** eredura JavaFX, C# kontroladoreak edo FXML pantailak banan-banan eraikitzeko.
3. **Debug eta Errore azterketan:**
   - Log fitxategi luze bat baduzu: erabili **Gemini 3.8 Flash**.
   - Errore sakon bat baduzu (konkurrentzia, AGP, sare-blokeoa): erabili **Gemini 4 Neon**.
4. **Bukaeran (Commit eta itxiera):** Aldatu **Gemini 4 Hydrogen** eredura commit mezuak sortu, formatua garbitu eta ataza ixteko.
