# 1T Jatetxea Erronka - Microsoft Teams Planner Planifikazio Integrala

**Proiektua:** 1T Jatetxea Erronka Agents  
**Taldea:** 1. TALDEA (2MG3 Ikasleak)  
**Erakundea:** Goierri Eskola  
**Plataforma:** Microsoft Teams > Planifikazioa (Planner)  
**Egitura:** 6 Zutabe (Faseak) x 13 Modulu/Atal = 78 Zeregin / Karta  

---

## Egutegi Orokorra (Faseen Epeak)

| Fasea (Zutabea) | Hasiera Data | Amaiera Data | Deskribapen Laburra |
| :--- | :--- | :--- | :--- |
| **1. ANALISIA** | 2026/11/03 | 2026/11/08 | Eskakizunen bilketa, erabilera-kasuak eta bideragarritasuna |
| **2. DISEINUA** | 2026/11/09 | 2026/11/14 | Datu-eredua, API kontratua, UI/UX (Figma) eta topologia |
| **3. GARAPENA** | 2026/11/15 | 2026/11/24 | Kodeketa, API endpointak, interfazeak eta zerbitzuak |
| **4. FROGAK** | 2026/11/25 | 2026/11/28 | Proba unitarioak, integrazio-probak, karga eta segurtasuna |
| **5. MARTXAN JARTZEA** | 2026/11/29 | 2026/12/01 | Ekoizpen-ingurunea, Docker hedatzea, Sare fisikoa eta konfigurazioa |
| **6. MANTENTZEA** | 2026/12/02 | 2026/12/05 | Eguneraketak, segurtasun-kopiak, dokumentazioa eta jarraipena |

---

## 13 Atal/Modulu Nagusiak

1. **DATU-BASEA** (MySQL 8.0 & PostgreSQL / Odoo)
2. **REST API** (ASP.NET Core C#)
3. **APP - Java Admin - Kudeatzailea** (JavaFX IT/Admin mahaigainekoa)
4. **APP - TPV - Zerbitzariak (C#)** (C# WinForms TPV Salmenta Puntua)
5. **APP - Android - Zerbitzariak** (Jetpack Compose Komandera mugikorra/tableta)
6. **APP - Android TV - Sukaldariak** (Android TV KDS Jetpack Compose)
7. **APP Java - Sukaldea** (JavaFX KDS mahaigainekoa)
8. **APP Java - Biltegia** (JavaFX Stock / Inbentarioa)
9. **APP - Zerbitzaria / Chat Internoa** (Socket.IO txat zerbitzaria eta bezeroak)
10. **APP - Web (Apache/PHP) - Bezeroak** (Webgune korporatiboa, karta eta erreserbak)
11. **APP - Android Bezeroak** (Jetpack Compose / Android bezeroen app mugikorra)
12. **AZPIEGITURA DIGITALA (Makina Birtualak)** (VirtualBox VMs, Docker, OPNsense, Sare birtualak)
13. **AZPIEGITURA FISIKOA** (Laborategia, Cisco switch/router, Rack 19", kableatua, APak)

---

# FASEZ FASEKO ZEREGIN GUZTIAK (PLANNER KARTAK)

---

## 1. FASEA: ANALISIA (Hasiera: 2026/11/03 - Amaiera: 2026/11/08)

### 1.1. DATU-BASEA - Analisia
- **Epea:** 2026/11/03 - 2026/11/06 | **Lehentasuna:** Premiazkoa | **Rola:** Datu-base arduraduna
- **Egitekoak (Checklist):**
  - [ ] Jatetxearen entitate nagusiak identifikatzea (mahaiak, langileak, platerak, erreserbak, eskaerak, fakturak, stocka).
  - [ ] Odoo ERP (PostgreSQL) eta MySQL arteko datuen banaketa eta bereizketa aztertzea.
  - [ ] Erreserben gainjartzeak eta transakzioen eskakizunak zehaztea (ACID propietateak).
  - [ ] Datu-motak, eremu derrigorrezkoak eta gako primario/arrotzak zerrendatzea.

### 1.2. REST API - Analisia
- **Epea:** 2026/11/04 - 2026/11/07 | **Lehentasuna:** Premiazkoa | **Rola:** Backend garatzailea
- **Egitekoak (Checklist):**
  - [ ] Beharrezko endpoint-en zerrenda definitzea (Auth, Platerak, Mahaiak, Eskaerak, Erreserbak, Fakturak, Fitxaketak).
  - [ ] Autentifikazio eta baimen-estrategia erabakitzea (JWT tokenak edo BCrypt PINak).
  - [ ] Datuen balidazio-arauak eta errore-kode estandarrak finkatzea (400, 401, 404, 409 Conflict).
  - [ ] Aplikazio bezero bakoitzaren eskakizun teknikoak aztertzea (.NET, Java, Android, Web).

### 1.3. APP - Java Admin - Kudeatzailea - Analisia
- **Epea:** 2026/11/05 - 2026/11/08 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Kudeatzailearen funtzionalitateak definitzea (langileen kudeaketa, zerbitzuen egoera, txostenak).
  - [ ] Mahaigaineko arkitektura aztertzea (JavaFX 21 + Maven arkitektura modularra).
  - [ ] Erabiltzaile-rolen baimen-mailak identifikatzea (Admin vs Arduraduna).

### 1.4. APP - TPV - Zerbitzariak (C#) - Analisia
- **Epea:** 2026/11/04 - 2026/11/07 | **Lehentasuna:** Premiazkoa | **Rola:** .NET garatzailea
- **Egitekoak (Checklist):**
  - [ ] TPV-ren lan-fluxuak aztertzea (mahaien irekiera, komandak gehitzea, kobraketa, tiketa inprimatzea).
  - [ ] Langileen txanda-hasiera eta amaierako fitxaketa sistemaren eskakizunak finkatzea.
  - [ ] Offline/online portaera eta APIarekiko konexio-galeren kudeaketa aztertzea.

### 1.5. APP - Android - Zerbitzariak - Analisia
- **Epea:** 2026/11/05 - 2026/11/08 | **Lehentasuna:** Premiazkoa | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] Zerbitzariak mahaian komandak azkar hartzeko eskakizunak definitzea (ukipen erraza, plateren oharrak).
  - [ ] Mugikor eta tablet formatuetarako eskakizun ergonomikoak aztertzea.
  - [ ] PIN bidezko saio-hasiera azkarra eta fitxaketa integratua aztertzea.

### 1.6. APP - Android TV - Sukaldariak - Analisia
- **Epea:** 2026/11/05 - 2026/11/08 | **Lehentasuna:** Ertaina | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] Urruneko aginte bidezko (D-Pad) nabigazio eskakizunak aztertzea.
  - [ ] Pantaila handian komandak urrutitik ikusteko tipografia eta kolore-kontrasteak aztertzea.
  - [ ] Denbora errealeko komanden sarrera eta iraungitze-denboren alertak definitzea.

### 1.7. APP Java - Sukaldea - Analisia
- **Epea:** 2026/11/05 - 2026/11/08 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Sukaldeko KDS mahaigaineko bertsioaren beharrak aztertzea (ukipen-pantaila edo sagua).
  - [ ] Plateren egoera aldaketak (Zai -> Martxan -> Eginda) definitzea.
  - [ ] Sukaldarien PIN bidezko fitxaketa erregistroaren integrazioa aztertzea.

### 1.8. APP Java - Biltegia - Analisia
- **Epea:** 2026/11/05 - 2026/11/08 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Stockaren sarrera eta irteeren kudeaketa-beharrak aztertzea.
  - [ ] Gutxieneko stock alerten logika definitzea (gorriz markatzea kritikoak).
  - [ ] Odoo ERP-ko erosketa-eskaerekin duen erlazioa aztertzea.

### 1.9. APP - Zerbitzaria / Chat Internoa - Analisia
- **Epea:** 2026/11/05 - 2026/11/08 | **Lehentasuna:** Baxua | **Rola:** Backend / Sareak
- **Egitekoak (Checklist):**
  - [ ] Barne komunikazioaren beharrak aztertzea (zerbitzariak <-> sukaldea <-> barra).
  - [ ] Denbora errealeko protokoloaren hautaketa (WebSockets / Socket.IO vs polling).
  - [ ] Kanalak eta mezu azkarren txantiloiak definitzea ("Premiazkoa mahai 4", "Ogia falta da").

### 1.10. APP - Web (Apache/PHP) - Bezeroak - Analisia
- **Epea:** 2026/11/04 - 2026/11/07 | **Lehentasuna:** Premiazkoa | **Rola:** Web garatzailea
- **Egitekoak (Checklist):**
  - [ ] Atari publikoaren eskakizunak zehaztea (karta ikusi, alergenoen iragazkia, erreserba formularioa).
  - [ ] Erreserben egutegia eta txanden eskuragarritasunaren kontsulta definitzea.
  - [ ] Errendimendu eta responsive (mugikor/ordenagailu) eskakizunak aztertzea.

### 1.11. APP - Android Bezeroak - Analisia
- **Epea:** 2026/11/05 - 2026/11/08 | **Lehentasuna:** Ertaina | **Rola:** Mugikor garatzailea
- **Egitekoak (Checklist):**
  - [ ] Mahaiko QR kodeen bidezko auto-eskaera lan-fluxua aztertzea.
  - [ ] Eramateko eskaerak eta ordainketa simulatuaren eskakizunak zehaztea.
  - [ ] Bezeroen esperientzia (UX) eta karta nabigazio azkarra definitzea.

### 1.12. AZPIEGITURA DIGITALA (Makina Birtualak) - Analisia
- **Epea:** 2026/11/03 - 2026/11/06 | **Lehentasuna:** Premiazkoa | **Rola:** Sistema administratzailea
- **Egitekoak (Checklist):**
  - [ ] Makina birtualen baliabideak (RAM, CPU, Diskoa) kalkulatzea (OPNsense, WinServer, Ubuntu).
  - [ ] Sare birtualen segmentazioa (VLANak, DMZ, Lan, Bezeroak) definitzea.
  - [ ] Docker edukiontzien arkitektura eta portuen mapeoa planifikatzea.

### 1.13. AZPIEGITURA FISIKOA - Analisia
- **Epea:** 2026/11/03 - 2026/11/06 | **Lehentasuna:** Premiazkoa | **Rola:** Sare teknikaria
- **Egitekoak (Checklist):**
  - [ ] Laborategiko ekipoen inbentarioa egitea (Cisco routerrak, switxak, TPVak, APak).
  - [ ] Kableatu-egituratuaren beharrak neurtzea (RJ45 patch kableak, patch panelak).
  - [ ] Jatetxearen plano fisikoaren arabera gailuen kokapena identifikatzea.

---

## 2. FASEA: DISEINUA (Hasiera: 2026/11/09 - Amaiera: 2026/11/14)

### 2.1. DATU-BASEA - Diseinua (MySQL)
- **Epea:** 2026/11/09 - 2026/11/12 | **Lehentasuna:** Premiazkoa | **Rola:** Datu-base arduraduna
- **Egitekoak (Checklist):**
  - [ ] Entitate-Erlazio diagrama (ER) osatzea.
  - [ ] Datu-basearen eskema erlazionala diseinatzea (SQL taulak, foreign key-ak, indizeak).
  - [ ] `uq_erreserba (mahaia_id, data, txanda)` murrizketa bakarra definitzea.
  - [ ] Hasierako datuen txertaketa (seeding) diseinatzea (karta zabaldua, langileak, mahaiak).

### 2.2. REST API - Diseinua (C#)
- **Epea:** 2026/11/10 - 2026/11/13 | **Lehentasuna:** Premiazkoa | **Rola:** Backend garatzailea
- **Egitekoak (Checklist):**
  - [ ] OpenAPI / Swagger kontratua diseinatzea (eskaera eta erantzun JSON egiturak).
  - [ ] DTO klaseak eta Controller egitura planifikatzea ASP.NET Core-n.
  - [ ] Datu-baseko sarbide-geruza diseinatzea (Dapper / Entity Framework Core).
  - [ ] Postman bildumaren egitura diseinatzea endpoint-ak probatzeko.

### 2.3. APP - Java Admin - Kudeatzailea - Diseinua
- **Epea:** 2026/11/11 - 2026/11/14 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Admin panelaren Figma wireframeak eta maketak sortzea.
  - [ ] FXML fitxategien eta CSS estilo orrien egitura diseinatzea.
  - [ ] Zerbitzuen monitorizazio-txartelen diseinu bisuala egitea.

### 2.4. APP - TPV - Zerbitzariak (C#) - Diseinua
- **Epea:** 2026/11/09 - 2026/11/12 | **Lehentasuna:** Premiazkoa | **Rola:** .NET garatzailea
- **Egitekoak (Checklist):**
  - [ ] Aretoaren plano interaktiboa diseinatzea (Jangela Nagusia, Terraza, Barra, VIP).
  - [ ] Kobraketa leihoaren eta tiketen formatu fiskalaren diseinua.
  - [ ] Zenbakizko teklatu birtualaren diseinua PIN bidez azkar fitxatzeko.

### 2.5. APP - Android - Zerbitzariak - Diseinua
- **Epea:** 2026/11/10 - 2026/11/13 | **Lehentasuna:** Premiazkoa | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] Jetpack Compose bidezko UI/UX pantailen maketak Figman diseinatzea.
  - [ ] Mahai-aukeraketa eta kategoria/plater zerrenden nabigazioa diseinatzea.
  - [ ] Eskaera-saskiaren eta ohar berezien txertaketa interfazea definitzea.

### 2.6. APP - Android TV - Sukaldariak - Diseinua
- **Epea:** 2026/11/11 - 2026/11/14 | **Lehentasuna:** Ertaina | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] Leanback / Compose for TV layout diseinua prestatzea (txartel handiak).
  - [ ] Komanden egoera kolore-kodeak definitzea (Horia = Zai, Urdina = Martxan, Berdea = Eginda).
  - [ ] Fokuaren nabigazioa eta D-Pad kontrolen portaera diseinatzea.

### 2.7. APP Java - Sukaldea - Diseinua
- **Epea:** 2026/11/10 - 2026/11/13 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] JavaFX KDS mahaigaineko leihoaren layout-a diseinatzea (GridPane / TilePane).
  - [ ] Plater bakoitzaren txartelaren osagai bisualak diseinatzea.
  - [ ] Egoera aldaketa botoi handiak eta fitxaketa goiburua diseinatzea.

### 2.8. APP Java - Biltegia - Diseinua
- **Epea:** 2026/11/11 - 2026/11/14 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Stock taularen interfazea diseinatzea (TableView, bilaketa barra, iragazkiak).
  - [ ] Gutxieneko stock alerten estiloak (Badge gorriak / abisuak) diseinatzea.
  - [ ] Inbentarioaren doikuntza modalaren diseinua egitea.

### 2.9. APP - Zerbitzaria / Chat Internoa - Diseinua
- **Epea:** 2026/11/11 - 2026/11/14 | **Lehentasuna:** Baxua | **Rola:** Backend garatzailea
- **Egitekoak (Checklist):**
  - [ ] WebSocket gertaeren protokoloa diseinatzea (`join_room`, `send_message`, `receive_message`).
  - [ ] Txat leihoaren diseinua bezero aplikazioetarako (widget mugikorra edo panela).
  - [ ] Jakinarazpen bisual eta akustikoen portaera diseinatzea.

### 2.10. APP - Web (Apache/PHP) - Bezeroak - Diseinua
- **Epea:** 2026/11/09 - 2026/11/12 | **Lehentasuna:** Premiazkoa | **Rola:** Web garatzailea
- **Egitekoak (Checklist):**
  - [ ] Webgunearen wireframeak eta responsive maketak diseinatzea (HTML/Tailwind/CSS).
  - [ ] Jatetxearen marka eta kolore-paleta aplikatzea (Italiar-Euskaldun estetika).
  - [ ] Erreserben formulario pausoak (Data -> Txanda -> Mahaikideak -> Datu pertsonalak) diseinatzea.

### 2.11. APP - Android Bezeroak - Diseinua
- **Epea:** 2026/11/11 - 2026/11/14 | **Lehentasuna:** Ertaina | **Rola:** Mugikor garatzailea
- **Egitekoak (Checklist):**
  - [ ] Android bezeroen app-aren pantailen prototipoa diseinatzea (Jetpack Compose) (Onboarding, Karta, Ordainketa, QR).
  - [ ] Txartel birtualaren animazioa eta ordainketa fluxuaren diseinua egitea.
  - [ ] Erreserben baieztapen pantaila eta QR sortzailearen interfazea diseinatzea.

### 2.12. AZPIEGITURA DIGITALA - Diseinua
- **Epea:** 2026/11/09 - 2026/11/12 | **Lehentasuna:** Premiazkoa | **Rola:** Sistema administratzailea
- **Egitekoak (Checklist):**
  - [ ] Sare birtualaren arkitektura diagrama osatzea (OPNsense WAN, LAN, DMZ, VLANak).
  - [ ] Firewall erregela-matrizea diseinatzea (sarbide baimenduak eta blokeatuak).
  - [ ] `docker-compose.yml` konfigurazio-fitxategiaren egitura diseinatzea.

### 2.13. AZPIEGITURA FISIKOA - Diseinua
- **Epea:** 2026/11/09 - 2026/11/12 | **Lehentasuna:** Premiazkoa | **Rola:** Sare teknikaria
- **Egitekoak (Checklist):**
  - [ ] Cisco Packet Tracer simulazioa diseinatzea eta balioztatzea (.pkt).
  - [ ] 19" Rack-aren antolaketa plano fisikoa diseinatzea (Patch panel, switch, router kokapena).
  - [ ] Sare-kableen kolore-kodeen eskema diseinatzea (TIA/EIA-568-B estandarra).

---

## 3. FASEA: GARAPENA (Hasiera: 2026/11/15 - Amaiera: 2026/11/24)

### 3.1. DATU-BASEA - Garapena (MySQL)
- **Epea:** 2026/11/15 - 2026/11/18 | **Lehentasuna:** Premiazkoa | **Rola:** Datu-base arduraduna
- **Egitekoak (Checklist):**
  - [ ] `datu_basea_schema.sql` sortu eta exekutatu MySQL 8.0 edukiontzian.
  - [ ] Karta zabalduaren eta prezioen seeding-a txertatu (`gehitu_karta_zabaldua.sql`).
  - [ ] Langileen taulak eta BCrypt hash enkriptatuak sortu (`gehitu_langileak_eta_hr_egitura.sql`).
  - [ ] Odoo 17 PostgreSQL datu-basearen integrazioa konfiguratu.

### 3.2. REST API - Garapena (C#)
- **Epea:** 2026/11/16 - 2026/11/20 | **Lehentasuna:** Premiazkoa | **Rola:** Backend garatzailea
- **Egitekoak (Checklist):**
  - [ ] ASP.NET Core (.NET 10) proiektua sortu eta Dapper / MySQL konexioa programatu.
  - [ ] CRUD kontroladoreak garatu: Platerak, Mahaiak, Eskaerak, Erreserbak, Fitxaketak.
  - [ ] Erreserben talka-kontrol logika programatu (409 Conflict erantzuna).
  - [ ] Langileen PIN autentifikazioa eta BCrypt balidazioa inplementatu.

### 3.3. APP - Java Admin - Kudeatzailea - Garapena
- **Epea:** 2026/11/18 - 2026/11/22 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] JavaFX Admin proiektua garatu (Maven, Java 21).
  - [ ] Langileen altak, bajak eta rolen esleipen modulua kodetu.
  - [ ] Fitxaketa-erregistroen kontsulta eta iragazte taula garatu.

### 3.4. APP - TPV - Zerbitzariak (C#) - Garapena
- **Epea:** 2026/11/16 - 2026/11/21 | **Lehentasuna:** Premiazkoa | **Rola:** .NET garatzailea
- **Egitekoak (Checklist):**
  - [ ] WinForms TPV aplikazio nagusia kodetu (.NET 10).
  - [ ] Mahaien mapa interaktiboa eta egoera koloreak (Okupatua, Librea, Erreserbatua) garatu.
  - [ ] Komanden sarrera, ordainketa kudeaketa eta txanda-fitxaketa PIN bidez programatu.

### 3.5. APP - Android - Zerbitzariak - Garapena
- **Epea:** 2026/11/17 - 2026/11/22 | **Lehentasuna:** Premiazkoa | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] Jetpack Compose komandera aplikazioa garatu (Retrofit API bezeroarekin).
  - [ ] Mahaien kudeaketa eta komanden zuzeneko bidalketa sukaldera kodetu.
  - [ ] Plateren ohar bereziak (puntua, alergiak) eta fitxaketa modulua inplementatu.

### 3.6. APP - Android TV - Sukaldariak - Garapena
- **Epea:** 2026/11/18 - 2026/11/22 | **Lehentasuna:** Ertaina | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] Android TV aplikazioa garatu Jetpack Compose for TV erabiliz.
  - [ ] Sukaldeko komandak denbora errealean erakusteko API polling / socket harpidetza programatu.
  - [ ] D-Pad urrutiko agintearen nabigazioa eta egoera eguneratzea kodetu.

### 3.7. APP Java - Sukaldea - Garapena
- **Epea:** 2026/11/17 - 2026/11/21 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] JavaFX KDS mahaigaineko aplikazioa garatu (`desktop_sukaldea_kds`).
  - [ ] Plateren egoera aldaketen logika (Zai -> Martxan -> Eginda) APIarekin lotu.
  - [ ] Sukaldarien PIN bidezko fitxaketa erregistroa inplementatu.

### 3.8. APP Java - Biltegia - Garapena
- **Epea:** 2026/11/18 - 2026/11/22 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] JavaFX Stock aplikazioa garatu (`desktop_biltegia_stock`).
  - [ ] Inbentarioaren eguneraketak eta stock mugimenduak APIarekin sinkronizatu.
  - [ ] Gutxieneko stock alertak gorriz bistaratzeko logika programatu.

### 3.9. APP - Zerbitzaria / Chat Internoa - Garapena
- **Epea:** 2026/11/19 - 2026/11/23 | **Lehentasuna:** Baxua | **Rola:** Backend garatzailea
- **Egitekoak (Checklist):**
  - [ ] Node.js + Socket.IO txat zerbitzaria kodetu.
  - [ ] Aretoa, Sukaldea eta Barraren arteko gela banaketak (rooms) garatu.
  - [ ] Bezeroentzako txat interfaze arina inplementatu.

### 3.10. APP - Web (Apache/PHP) - Bezeroak - Garapena
- **Epea:** 2026/11/16 - 2026/11/20 | **Lehentasuna:** Premiazkoa | **Rola:** Web garatzailea
- **Egitekoak (Checklist):**
  - [ ] PHP 8.2 backend-a eta HTML5/JavaScript bezero ataria kodetu.
  - [ ] Karta dinamikoa API bidez kargatu eta alergenoen araberako iragazkia programatu.
  - [ ] Mahai-erreserben formulario interaktiboa garatu (data, txanda, pertsona kopurua).

### 3.11. APP - Android Bezeroak - Garapena
- **Epea:** 2026/11/18 - 2026/11/23 | **Lehentasuna:** Ertaina | **Rola:** Mugikor garatzailea
- **Egitekoak (Checklist):**
  - [ ] Android mugikorreko bezeroen aplikazioa kodetu (Jetpack Compose / Kotlin).
  - [ ] QR kode irakurlea integratu mahaiko auto-eskaerarako.
  - [ ] Txartel birtualaren bidezko ordainketa simulatuaren logika programatu.

### 3.12. AZPIEGITURA DIGITALA - Garapena
- **Epea:** 2026/11/15 - 2026/11/19 | **Lehentasuna:** Premiazkoa | **Rola:** Sistema administratzailea
- **Egitekoak (Checklist):**
  - [ ] VirtualBox makina birtualak konfiguratu (OPNsense, WinServer2025, Ubuntu).
  - [ ] Docker Compose zerbitzuak abiarazi (MySQL, API, Odoo, Nginx, Adminer).
  - [ ] OPNsense suebakian VLANak, DHCP zerbitzariak eta NAT erregelak konfiguratzea.

### 3.13. AZPIEGITURA FISIKOA - Garapena
- **Epea:** 2026/11/15 - 2026/11/19 | **Lehentasuna:** Premiazkoa | **Rola:** Sare teknikaria
- **Egitekoak (Checklist):**
  - [ ] Laborategiko kableatu fisikoa crimpatu eta patch panelean konektatu.
  - [ ] Cisco Switch eta Router konfigurazioak kargatu CLI bidez (VLANak, Trunk, OSPF).
  - [ ] Wi-Fi Access Point-ak muntatu eta jatetxeko sare publiko/pribatuak igorri.

---

## 4. FASEA: FROGAK (Hasiera: 2026/11/25 - Amaiera: 2026/11/28)

### 4.1. DATU-BASEA - Frogak
- **Epea:** 2026/11/25 - 2026/11/26 | **Lehentasuna:** Premiazkoa | **Rola:** Datu-base arduraduna
- **Egitekoak (Checklist):**
  - [ ] Integritate erlazionalaren probak (Foreign Key murrizketak, ezabaketa kaskadan).
  - [ ] Erreserben gako bakar konposatuaren talka-proba (`UNIQUE KEY uq_erreserba`).
  - [ ] Datu-basearen erantzun-denborak eta indizeen eraginkortasuna ebaluatu.

### 4.2. REST API - Frogak
- **Epea:** 2026/11/25 - 2026/11/27 | **Lehentasuna:** Premiazkoa | **Rola:** Backend garatzailea
- **Egitekoak (Checklist):**
  - [ ] Postman bilduma osoa exekutatu (Newman CLI / Postman Runner bidez).
  - [ ] Endpoint bakoitzeko errore-kudeaketa egiaztatu (400, 401, 404, 409).
  - [ ] Karga arineko probak egin aldibereko eskaerak ondo kudeatzen direla bermatzeko.

### 4.3. APP - Java Admin - Kudeatzailea - Frogak
- **Epea:** 2026/11/26 - 2026/11/28 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Erabiltzaile eta rolen baimen-probak egitea (Admin sarbide mugatuak).
  - [ ] Fitxaketa-zerrendak datu errealekin kontsultatu eta egiaztatzea.

### 4.4. APP - TPV - Zerbitzariak (C#) - Frogak
- **Epea:** 2026/11/25 - 2026/11/27 | **Lehentasuna:** Premiazkoa | **Rola:** .NET garatzailea
- **Egitekoak (Checklist):**
  - [ ] Mahaien fluxu osoaren proba: Ireki -> Komanda gehitu -> Ordaindu -> Tiketa atera.
  - [ ] PIN bidezko fitxaketa probatu eta APIan erregistro zuzena egiaztatu.
  - [ ] Sare-mozketaren simulazioa (offline alertak).

### 4.5. APP - Android - Zerbitzariak - Frogak
- **Epea:** 2026/11/26 - 2026/11/28 | **Lehentasuna:** Premiazkoa | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] Mugikor eta tabletan erantzun grafikoa eta ukipen-zehaztasuna probatu.
  - [ ] Komanda bat bidaltzean KDS eta TPV-an berehala agertzen dela ziurtatu.
  - [ ] Bateria-kontsumoa eta sare-aldaketak (Wi-Fi roaming) testatzea.

### 4.6. APP - Android TV - Sukaldariak - Frogak
- **Epea:** 2026/11/26 - 2026/11/28 | **Lehentasuna:** Ertaina | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] D-Pad kontrolarekin nabigazioaren erosotasuna eta abiadura probatu.
  - [ ] Komanda berriak sartzean pantaila automatikoki eguneratzen dela egiaztatu.
  - [ ] Sukaldeko distira eta ikuspen-angelu egokiak testatzea.

### 4.7. APP Java - Sukaldea - Frogak
- **Epea:** 2026/11/26 - 2026/11/28 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Egoera-trantsizio guztiak probatu (Zai -> Martxan -> Eginda).
  - [ ] Sukaldariaren fitxaketa PIN bidez probatu eta balioztatu.

### 4.8. APP Java - Biltegia - Frogak
- **Epea:** 2026/11/26 - 2026/11/28 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Stock-aldaketak berehala islatzen direla probatu.
  - [ ] Gutxieneko stock alertak gorriz behar bezala aktibatzen direla ziurtatu.

### 4.9. APP - Zerbitzaria / Chat Internoa - Frogak
- **Epea:** 2026/11/26 - 2026/11/28 | **Lehentasuna:** Baxua | **Rola:** Backend garatzailea
- **Egitekoak (Checklist):**
  - [ ] Aldibereko mezu-bidalketak eta atzerapena (latency) neurtu.
  - [ ] Gela ezberdinen arteko isolamendua egiaztatu (sukaldea ez nahastea bezeroekin).

### 4.10. APP - Web (Apache/PHP) - Bezeroak - Frogak
- **Epea:** 2026/11/25 - 2026/11/27 | **Lehentasuna:** Premiazkoa | **Rola:** Web garatzailea
- **Egitekoak (Checklist):**
  - [ ] Nabigatzaile anitzeko probak (Chrome, Firefox, Safari, Edge).
  - [ ] Erreserba bikoiztuen talka-proba (HTTP 409 mezua erabiltzaileari egoki erakustea).
  - [ ] Alergenoen iragazkia plater guztietan behar bezala dabilela testatzea.

### 4.11. APP - Android Bezeroak - Frogak
- **Epea:** 2026/11/26 - 2026/11/28 | **Lehentasuna:** Ertaina | **Rola:** Mugikor garatzailea
- **Egitekoak (Checklist):**
  - [ ] QR kodeen eskaneatze azkarra egiaztatu kamerarekin.
  - [ ] Ordainketa-pasabide simulatuaren balidazio guztiak probatu.

### 4.12. AZPIEGITURA DIGITALA - Frogak
- **Epea:** 2026/11/25 - 2026/11/27 | **Lehentasuna:** Premiazkoa | **Rola:** Sistema administratzailea
- **Egitekoak (Checklist):**
  - [ ] Firewall erregela-probak (segurtasun-eskaneoa Nmap bidez).
  - [ ] Docker edukiontzien berrabiarazte automatikoa probatu (`restart: always`).
  - [ ] Segurtasun-kopiak eta leheneratze simulazioa burutzea.

### 4.13. AZPIEGITURA FISIKOA - Frogak
- **Epea:** 2026/11/25 - 2026/11/27 | **Lehentasuna:** Premiazkoa | **Rola:** Sare teknikaria
- **Egitekoak (Checklist):**
  - [ ] Kableatuaren jarraitutasun-testa kable-testagailuarekin.
  - [ ] Sare fisikoaren abiadura eta pakete-galeren neurketa (Ping, Iperf).
  - [ ] Wi-Fi estalduraren eta seinalearen intentsitatearen neurketa laborategian.

---

## 5. FASEA: MARTXAN JARTZEA (Hasiera: 2026/11/29 - Amaiera: 2026/12/01)

### 5.1. DATU-BASEA - Martxan Jartzea
- **Epea:** 2026/11/29 - 2026/11/30 | **Lehentasuna:** Premiazkoa | **Rola:** Datu-base arduraduna
- **Egitekoak (Checklist):**
  - [ ] Ekoizpeneko MySQL eta PostgreSQL datu-baseak hasieratu eta babestu.
  - [ ] Ekoizpeneko erabiltzaile eta baimen estuak konfiguratu (root urruneko sarbidea desgaitu).
  - [ ] Hasierako datu errealak inportatu eta egiaztatu.

### 5.2. REST API - Martxan Jartzea
- **Epea:** 2026/11/29 - 2026/11/30 | **Lehentasuna:** Premiazkoa | **Rola:** Backend garatzailea
- **Egitekoak (Checklist):**
  - [ ] Ekoizpeneko Docker ingurunean APIa hedatu.
  - [ ] `appsettings.Production.json` kredentzial seguruekin konfiguratu.
  - [ ] Nginx Reverse Proxy eta SSL/TLS ziurtagiriak aktibatu (HTTPS).

### 5.3. APP - Java Admin - Kudeatzailea - Martxan Jartzea
- **Epea:** 2026/11/30 - 2026/12/01 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Exekutagarri autonomoa (JAR / jpackage instalatzailea) sortu eta zabaldu.
  - [ ] Administratzailearen ordenagailuan instalatu eta ekoizpeneko APIarekin konektatu.

### 5.4. APP - TPV - Zerbitzariak (C#) - Martxan Jartzea
- **Epea:** 2026/11/29 - 2026/11/30 | **Lehentasuna:** Premiazkoa | **Rola:** .NET garatzailea
- **Egitekoak (Checklist):**
  - [ ] TPV ordenagailu fisikoan C# aplikazioa instalatu eta konfiguratu.
  - [ ] Tiket-inprimagailu termikoa eta diru-kaxatxoa konektatu eta probatu.
  - [ ] Kaxa-irekiera ofiziala egin eta langileei erabiltzeko baimenak eman.

### 5.5. APP - Android - Zerbitzariak - Martxan Jartzea
- **Epea:** 2026/11/30 - 2026/12/01 | **Lehentasuna:** Premiazkoa | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] APK ekoizpen bertsioa sinatu eta sortu.
  - [ ] Jatetxeko tablet eta komandera gailu mugikorretan instalatu.
  - [ ] Zerbitzarien Wi-Fi pribatura lotu eta lehen proba erreala egin.

### 5.6. APP - Android TV - Sukaldariak - Martxan Jartzea
- **Epea:** 2026/11/30 - 2026/12/01 | **Lehentasuna:** Ertaina | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] APK-a Android TV gailuan / telebistan instalatu (ADB edo USB bidez).
  - [ ] Pantaila finkoan abiarazte automatikoa konfiguratu gailua piztean.
  - [ ] Sukaldeko horman kokatu eta kable bidezko sare-konexioa bermatu.

### 5.7. APP Java - Sukaldea - Martxan Jartzea
- **Epea:** 2026/11/30 - 2026/12/01 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Sukaldeko kontrol pantailan Java aplikazioa instalatu.
  - [ ] Sukaldari-buruaren profilarekin sarbidea egiaztatu.

### 5.8. APP Java - Biltegia - Martxan Jartzea
- **Epea:** 2026/11/30 - 2026/12/01 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Biltegiko terminalean aplikazioa hedatu eta stock hasierako balioak egiaztatu.
  - [ ] Barra-kode irakurgailua konektatu eta probatu.

### 5.9. APP - Zerbitzaria / Chat Internoa - Martxan Jartzea
- **Epea:** 2026/11/30 - 2026/12/01 | **Lehentasuna:** Baxua | **Rola:** Backend garatzailea
- **Egitekoak (Checklist):**
  - [ ] Txat zerbitzaria atzeko planoko zerbitzu gisa abiarazi (systemd / Docker).
  - [ ] Aplikazio bezero guztietan txat konexio automatikoa aktibatu.

### 5.10. APP - Web (Apache/PHP) - Bezeroak - Martxan Jartzea
- **Epea:** 2026/11/29 - 2026/11/30 | **Lehentasuna:** Premiazkoa | **Rola:** Web garatzailea
- **Egitekoak (Checklist):**
  - [ ] Ekoizpeneko Apache web zerbitzarian fitxategiak hedatu.
  - [ ] Bezeroentzako domeinua / IP publikoa eta SSL ziurtagiria lotu.
  - [ ] Erreserben sistema jendaurrean ireki.

### 5.11. APP - Android Bezeroak - Martxan Jartzea
- **Epea:** 2026/11/30 - 2026/12/01 | **Lehentasuna:** Ertaina | **Rola:** Mugikor garatzailea
- **Egitekoak (Checklist):**
  - [ ] Bezeroen app-a probako ingurune irekian jarri (edo QR bidezko deskarga zuzena prestatu).
  - [ ] Mahaiko QR kode fisikoak inprimatu eta mahaietan itsatsi.

### 5.12. AZPIEGITURA DIGITALA - Martxan Jartzea
- **Epea:** 2026/11/29 - 2026/11/30 | **Lehentasuna:** Premiazkoa | **Rola:** Sistema administratzailea
- **Egitekoak (Checklist):**
  - [ ] Zerbitzari birtual guztiak ekoizpen moduan martxan jarri.
  - [ ] Sare birtualak eta OPNsense suebakiaren defentsa-mekanismoak gaitu.
  - [ ] Auto-abiaraztea (startup scripts) konfiguratzea zerbitzari nagusia berrabiarazten denerako.

### 5.13. AZPIEGITURA FISIKOA - Martxan Jartzea
- **Epea:** 2026/11/29 - 2026/11/30 | **Lehentasuna:** Premiazkoa | **Rola:** Sare teknikaria
- **Egitekoak (Checklist):**
  - [ ] Rack-a itxi eta kableatu guztia etiketatu eta txukundu.
  - [ ] Cisco switch eta routerrak martxan utzi eta konfigurazioa memorian gorde (`write memory`).
  - [ ] Bezeroen eta langileen Wi-Fi sareak aktibatu eta funtzionamendua egiaztatu.

---

## 6. FASEA: MANTENTZEA (Hasiera: 2026/12/02 - Amaiera: 2026/12/05)

### 6.1. DATU-BASEA - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Premiazkoa | **Rola:** Datu-base arduraduna
- **Egitekoak (Checklist):**
  - [ ] Eguneroko backup automatikoen cron job-a konfiguratzea (MySQL dump).
  - [ ] Taulen tamainaren eta disko-espazioaren monitorizazioa ezartzea.
  - [ ] Erreserba eta transakzio zaharren artxibatze-politika zehaztea.

### 6.2. REST API - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Premiazkoa | **Rola:** Backend garatzailea
- **Egitekoak (Checklist):**
  - [ ] Erroreen log-sistema aktibatzea (Serilog / fitxategietara logeatzea).
  - [ ] API bertsio-berritzeen eta adabakien kudeaketa-plana idaztea.
  - [ ] API-ko eskaeren latentzia eta trafiko-analisia egitea.

### 6.3. APP - Java Admin - Kudeatzailea - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Erabiltzaileen eskaeren araberako hobekuntza txikiak txertatzea.
  - [ ] Erabiltzaile-eskuliburua eta ohiko akatsen gida entregatzea.

### 6.4. APP - TPV - Zerbitzariak (C#) - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Premiazkoa | **Rola:** .NET garatzailea
- **Egitekoak (Checklist):**
  - [ ] Kaxa-itxieren auditoretza eta gorabeheren analisia egitea.
  - [ ] TPV-ren erantzun-abiaduraren jarraipena eta memoria-ihesen ikuskapena.

### 6.5. APP - Android - Zerbitzariak - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Premiazkoa | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] Gailuen karga-zikloak eta deskonexio-gorabeherak aztertzea.
  - [ ] Zerbitzarien iritzia jaso eta interfazearen doikuntzak planifikatzea.

### 6.6. APP - Android TV - Sukaldariak - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Ertaina | **Rola:** Android garatzailea
- **Egitekoak (Checklist):**
  - [ ] Pantaila luzaroan piztuta egotean sortutako memoria-kudeaketa gainbegiratzea.
  - [ ] Jakinarazpen soinudun berriak edo tenporizadoreen doikuntzak aztertzea.

### 6.7. APP Java - Sukaldea - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Sukaldeko KDS aplikazioaren erregistroak garbitzea.
  - [ ] Sukaldarien aldaketa-proposamenak jasotzea.

### 6.8. APP Java - Biltegia - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Ertaina | **Rola:** Java garatzailea
- **Egitekoak (Checklist):**
  - [ ] Inbentario errealaren eta datu-baseko stockaren arteko desbideraketak berrikustea.
  - [ ] Hornitzaileen albaranen erregistro-fitxategiak artxibatzea.

### 6.9. APP - Zerbitzaria / Chat Internoa - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Baxua | **Rola:** Backend garatzailea
- **Egitekoak (Checklist):**
  - [ ] Txat mezuen historia garbitzeko script periodikoa konfiguratzea.
  - [ ] WebSocket konexio aktiboen monitorizazioa egitea.

### 6.10. APP - Web (Apache/PHP) - Bezeroak - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Premiazkoa | **Rola:** Web garatzailea
- **Egitekoak (Checklist):**
  - [ ] Sarbide eta erroreen log-ak aztertzea (`access.log` eta `error.log`).
  - [ ] Karta-aldaketak eta sasoiko plateren eguneratze-prozedura definitzea.

### 6.11. APP - Android Bezeroak - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Ertaina | **Rola:** Mugikor garatzailea
- **Egitekoak (Checklist):**
  - [ ] Bezeroen app-aren bertsio berriak banatzeko prozedura idaztea.
  - [ ] Akats-txostenen jasotzea eta erregistroa egitea.

### 6.12. AZPIEGITURA DIGITALA - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Premiazkoa | **Rola:** Sistema administratzailea
- **Egitekoak (Checklist):**
  - [ ] Docker irudien eta sistema eragileen segurtasun-eguneraketak aplikatzea.
  - [ ] Disko-erabilera, RAM kontsumoa eta tenperaturak monitorizatzea.
  - [ ] Hondamenditik berreskuratzeko plana (Disaster Recovery Plan) dokumentatzea.

### 6.13. AZPIEGITURA FISIKOA - Mantentzea
- **Epea:** 2026/12/02 - 2026/12/05 | **Lehentasuna:** Premiazkoa | **Rola:** Sare teknikaria
- **Egitekoak (Checklist):**
  - [ ] Cisco sare-gailuen konfigurazioen segurtasun-kopiak ateratzea TFTP bidez.
  - [ ] Hardwarearen egoera fisikoa, haizagailuak eta tenperaturak gainbegiratzea.
  - [ ] Laborategiko inbentario fisikoaren amaierako ikuskapena egitea.
