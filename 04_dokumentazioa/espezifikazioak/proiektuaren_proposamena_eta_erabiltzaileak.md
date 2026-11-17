# Jatetxearen Kudeaketa Integrala: Proiektuaren Proposamena eta Eragileak

**Egileak:** Ander Urien, Aratz Huegun, Iker Araujo, Julen Aguado  
**Data:** 2026ko Azaroa  
**Erakundea:** Goierri Eskola - Informatikako Zikloa  

---

## 1. Proiektuaren Helburua eta Ikuspegi Orokorra

Dokumentu honek Jatetxearen Erronka Integraleko proposamen arkitektonikoa eta funtzionala laburbiltzen du. Soluzio honen helburua jatetxe baten eguneroko jardun osoa digitalizatzea eta automatizatzea da, bezeroak erreserba egiten duen unetik hasi eta hornitzaileen stocka kudeatu, langileen lanaldiaren fitxaketa erregistratu edota fakturazioa burutu arte.

Informazio operatibo guztia zentralizatuta dago backend API baten bidez (ASP.NET Core REST API), datu-base sendo baten gainean (**MySQL** jatetxearen datu-base operatibo nagusirako, eta **PostgreSQL** esklusiboki Odoo ERP sistemak kudeatzen duena). Sistema honetan arkitektura sendo, moderno eta plataforma bakoitzera optimizatua aukeratu da: mugikor eta tablet gailuetan Android natiboa erabiltzen da (Flutter erabili beharrean), eta eragiketa-fluxu bakoitzari bere zereginetarako interfaze aproposena eskaintzen zaio.

Gainera, langileen asistentzia eta ordutegi kontrola betetzeko, aplikazio nagusiek (C# TPV, JavaFX eta Android Komandera) **fitxatzeko funtzio integratua** eskaintzen dute, lanaldiaren hasiera eta amaiera PIN bidez erregistratuz eta Odoo ERP-ko nomin/asistentzia moduluekin sinkronizatuz.

---

## 2. Eragileak (Rolak) eta Bakoitzaren Aplikazioak

Jatetxearen ekosisteman 6 erabiltzaile-rol nagusi identifikatu dira:

| Rola / Erabiltzailea | Erabilitako Aplikazioak | Plataforma / Teknologia | Funtzio Nagusiak |
| :--- | :--- | :--- | :--- |
| **Bezeroak** | Web-App (`web_php` / HTML-JS) | Web (HTML5/JS/PHP) | Karta kontsultatu, alergenoen iragazketa, mahai erreserbak egin eta eguneko menua ikusi. |
| **Zerbitzariak (Jantokia)** | TPV Salmenta Puntua (`desktop_csharp`)<br>Komandera Android (`zerbitzaria_komandera_android`) | Windows Desktop (.NET 10 WinForms)<br>Android (Mugikorra + Tableta) | Mahaien mapa eta okupazioa kudeatu, erreserben egutegia ikusi, komandak mahai ondoan jaso, sukaldera bidali, kobraketak bideratu eta **lanaldia fitxatu**. |
| **Sukaldariak** | Sukaldeko KDS Telebista (`sukaldea_kds_android_tv`)<br>JavaFX Aplikazioa (`desktop_javafx` - KDS Modulua) | Android TV (Jetpack Compose)<br>Desktop (Java 17/21, JavaFX) | Sukaldeko pantailan komandak denbora errealean jaso, plateren egoerak eguneratu (Zai, Martxan, Eginda), lehengaiak kontrolatu eta **lanaldia fitxatu**. |
| **Barrakoa (Kutxazaina)** | TPV Salmenta Puntua (`desktop_csharp`)<br>JavaFX Aplikazioa (`desktop_javafx` - BDS)<br>Android App (`zerbitzaria_komandera_android`) | Windows Desktop (.NET 10)<br>Desktop (JavaFX BDS)<br>Android (Mugikorra + Tableta) | Barran salmentak erregistratu, edarien komandak prestatu (BDS), tiketak/fakturak inprimatu, kontuak kobratu, mugikorrean komandak jaso eta **lanaldia fitxatu**. |
| **Biltegikoak** | JavaFX Aplikazioa (`desktop_javafx` - Biltegia/Stock)<br>ERP Kudeaketa (`odoo_erp`) | Desktop (JavaFX)<br>Web ERP (Odoo 17 / PostgreSQL) | Lehengaien stocka kontrolatu, gutxieneko stock alertak aztertu, hornitzaileen albaranak jaso, erosketa-zerrendak kudeatu eta **lanaldia fitxatu**. |
| **Admin IT / Zuzendaritza** | TI Panela (`admin_IT_zentrala`)<br>JavaFX Aplikazioa (`desktop_javafx`)<br>ERP / Finantzak (`odoo_erp`)<br>Zentral API (`backend_api`) | Web Dashboard<br>Desktop (JavaFX)<br>Odoo ERP (PostgreSQL)<br>ASP.NET Core REST API | Zerbitzuen eta azpiegituraren monitorizazioa, langileen login eta baimenak (RBAC/PINak), asistentzia eta fitxaketen ikuskaritza, txostenak eta segurtasun-kopiak. |

---

## 3. Aplikazioen Deskribapena

![Arkitektura Diagrama](arkitektura_diagrama.svg)

### 3.1. Bezeroen Interfazea

- **Web-App (`01_aplikazioak/web_php`):** Jatetxearen atari publikoa. Bezeroak mahai-erreserbak egiteko aukera du, eguneko menua eta karta ikusi eta alergenoen araberako iragazketa egiteko. Stockik ez duten platerak automatikoki agortu gisa agertzen dira.

### 3.2. Zerbitzarien eta Jantokiaren Interfazeak

- **TPV Salmenta Puntua Aretoa & Kutxazaina (`01_aplikazioak/desktop_csharp`):** Jantokiko zerbitzariek eta arduradunek erabilia. Mahaien banaketa interaktiboa (Jangela Nagusia, Terraza, Barra, VIP), erreserben 5 hilabeteko egutegi integrala, kobraketa azkarrak eta txanda hasiera/amaierako **fitxaketa sistema** bideratzen ditu.
- **Komandera Android Natiboa (`01_aplikazioak/zerbitzaria_komandera_android`):** Jetpack Compose bidez garatua, telefono zein tablet formatuetara guztiz egokitua. Zerbitzariak esku-gailutik mahai bakoitzeko komandak jaso ditzake, ohar bereziak jarri (alergiak, haragi-puntua), berehala sukaldera edo barrara bidali eta fitxaketa erregistratu.

### 3.3. Sukaldeko Interfazeak (KDS - Kitchen Display System)

- **Android TV KDS (`01_aplikazioak/sukaldea_kds_android_tv`):** Sukaldeko hormako telebista edo pantaila nagusirako diseinatua (Android TV / Jetpack Compose). Sartzen diren aginduak denbora errealean erakusten ditu, txanden eta prestaketa-denboren arabera ordenatuta.
- **JavaFX Mahaigaineko Aplikazioa (`01_aplikazioak/desktop_javafx` - Sukaldea KDS):** Sukaldari buruak eta laguntzaileek plateren egoerak ("Zai", "Martxan", "Eginda") klik bakarrean kudeatzeko eta txandaren fitxaketa egiteko aukera dute.

### 3.4. Barrako eta Salmenta Puntuko Interfazeak

- **C# .NET 10 Desktop TPV (`01_aplikazioak/desktop_csharp`):** Barran kokatutako terminal nagusia. Tiketak eta faktura fiskalak inprimatzea, zatitutako ordainketak (eskudirua, txartela), eguneko kaxa-itxiera eta fitxaketa ahalbidetzen ditu.
- **JavaFX Barra BDS Modulua (`01_aplikazioak/desktop_javafx`):** Barran prestatzeko diren edari, kafe eta aperitiboen komandak bereizita kudeatzeko pantaila dedikatua.
- **Android Komandera Mugikorra eta Tableta (`01_aplikazioak/zerbitzaria_komandera_android`):** Barrako langileek ere erabil dezakete mugikor edo tabletan eskaerak azkar hartzeko, egoerak ikusteko eta fitxatzeko.

### 3.5. Biltegiko Interfazea

- **JavaFX Biltegia eta Stock Modulua (`01_aplikazioak/desktop_javafx`):** Jatetxeko biltegi, despentsa eta ganbarako stock errealaren jarraipena. Gutxieneko stocketik behera dauden osagaiak gorriz bistaratzen ditu hornidura bermatuz, eta biltegiko langileen fitxaketa ahalbidetzen du.
- **Odoo 17 ERP Integrazioa (`01_aplikazioak/odoo_erp`):** Hornitzaileen erosketa-eskaera ofizialak, albaranen sarrera eta hornitzaileen fakturen kontabilitatea kudeatzeko (PostgreSQL gainean).

### 3.6. Administrazioa, TI eta Zuzendaritza

- **Admin IT Zentrala (`01_aplikazioak/admin_IT_zentrala`):** Sistemaren administratzaileentzako aginte-zentroa. Docker zerbitzuen, ataken, sare-interfazearen, aplikazioen abiarazleen, langileen fitxaketen ikuskaritzaren eta VirtualBox makina birtualen kontrol osoa eskaintzen du.
- **JavaFX Aplikazio Integratua (`01_aplikazioak/desktop_javafx`):** Admin IT eta Zuzendaritzak sarbide osoa dute Sukaldea (KDS), Barra (BDS) eta Biltegia (Stock) moduluetara saio-hasiera bakarretik PIN bidez.
- **Odoo ERP (`01_aplikazioak/odoo_erp`):** Zuzendaritzarentzat finantzak, langileen nominak, asistentzia-erregistroak (fitxaketak) eta txosten ekonomiko globalak aztertzeko tresna.
- **Backend REST API Zentrala (`01_aplikazioak/backend_api`):** ASP.NET Core bidez sortua, MySQL datu-base operatibo nagusiarekin komunikatzen da eta aplikazio guztiei (Desktop, Android, Web) zerbitzua ematen die, fitxaketen erregistroa barne.

---

## 4. Azpiegitura Birtuala eta Datu-Baseak

- **Datu-Baseen Banaketa:**
  - **MySQL (`jatetxea_db`):** Jatetxearen aplikazio operatibo guztien datu-base nagusia (langileak, fitxaketak, mahaiak, platerak, eskaerak, erreserbak, fakturak eta stock eragiketak).
  - **PostgreSQL (`jatetxea_odoo`):** Odoo 17 ERP sistemak esklusiboki erabiltzen duen datu-basea (kontabilitatea, erosketak eta nominak kudeatzeko).
- **OPNsense Firewall:** Sareak segmentatzen ditu (Bezeroen Wi-Fi publikoa, Langileen sarea, eta Zerbitzarien DMZ / IT kudeaketa).
- **Docker Edukiontziak:** Backend APIa, datu-baseak eta web zerbitzuak modu arin, isolatu eta fidagarrian hedatzeko.
- **Active Directory / LDAP / RBAC:** Erabiltzaile bakoitzak bere profilari dagozkion datuak bakarrik ikus ditzakeela ziurtatuz (BCrypt PIN eta rolen araberako sarbide-kontrola).
