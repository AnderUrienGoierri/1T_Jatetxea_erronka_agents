# Jatetxearen Kudeaketa Integrala: Proiektuaren Proposamena eta Eragileak

## 1. Proiektuaren Helburua eta Ikuspegi Orokorra

Dokumentu honek Jatetxearen Erronka Integraleko proposamen arkitektonikoa eta funtzionala laburbiltzen du. Soluzio honen helburua jatetxe baten eguneroko jardun osoa digitalizatzea eta automatizatzea da, bezeroak erreserba egiten duen unetik hasi eta hornitzaileen stocka kudeatu edota fakturazioa burutu arte.

Informazio guztia zentralizatuta dago后端 API baten bidez (ASP.NET Core REST API), datu-base sendo baten gainean (PostgreSQL), eragiketa-fluxu bakoitzari dagokion interfaze espezifikoa eskainiz.

---

## 2. Eragileak (Rolak) eta Bakoitzaren Aplikazioak

Jatetxearen ekosisteman 6 erabiltzaile-rol nagusi identifikatu dira:

| Rola / Erabiltzailea | Erabilitako Aplikazioak | Plataforma / Teknologia | Funtzio Nagusiak |
| :--- | :--- | :--- | :--- |
| **Bezeroak** | Web-App (`web_php` / HTML-JS)<br/>Mugikorreko App-a (`mobile_bezeroa`) | Web (HTML5/JS/PHP)<br/>Android / iOS (Flutter) | Karta kontsultatu, alergenoen iragazketa, mahai erreserbak egin, auto-eskaerak bideratu eta faktura ikusi. |
| **Zerbitzariak (Jantokia)** | Komandera Mugikorra (`zerbitzaria_komandera_android`)<br/>Komandera Flutter (`mobile_zerbitzaria`) | Android Natiboa (Jetpack Compose)<br/>Multiplataforma (Flutter) | Mahaien mapa eta okupazioa kudeatu, komandak jaso mahai ondoan, eskaerak sukaldera bidali eta kobraketak bideratu. |
| **Sukaldariak** | Sukaldeko KDS Telebista (`sukaldea_kds_android_tv`)<br/>KDS Mahaigainekoa (`desktop_javafx`) | Android TV (Leanback / Compose)<br/>Desktop (Java 21, JavaFX) | Sukaldeko pantaila handian komandak denbora errealean jaso, plateren egoerak eguneratu (Prestatzen, Prest) eta lehengaiak gastatu. |
| **Barrakoa (Kutxazaina)** | TPV Salmenta Puntua (`desktop_csharp`) | Windows Desktop (.NET 10 WinForms/WPF) | Barran zuzeneko salmentak erregistratu, tiketak eta fakturak inprimatu, kontuak kobratu eta mahaien egoera globala ikusi. |
| **Biltegikoak** | Biltegi Kudeatzailea (`desktop_javafx`)<br/>ERP Kudeaketa (`odoo_erp`) | Desktop (JavaFX MVC)<br/>Web ERP (Odoo) | Lehengaien stocka kontrolatu, gutxieneko stock alertak aztertu, hornitzaileen albaranak jaso eta erosketa-zerrendak sortu. |
| **Admin TI / Zuzendaritza** | TI Panela (`admin_IT_zentrala`)<br/>ERP / Finantzak (`odoo_erp`)<br/>Zentral API (`backend_api`) | Web Dashboard (Angular / React / HTML)<br/>Odoo ERP<br/>ASP.NET Core REST API | Langileen altak/bajak, baimenak (RBAC), azpiegitura birtualaren monitorizazioa, salmenta txostenak eta segurtasun-kopiak. |

---

## 3. Aplikazioen Deskribapen Zehatza

```mermaid
graph TD
    subgraph KANPOKOAK["Bezeroak"]
        BEZ_WEB["Web Karta & Erreserbak<br/>(web_php)"]
        BEZ_MOB["Bezeroaren Mugikorra<br/>(mobile_bezeroa - Flutter)"]
    end

    subgraph JANTOKIA_BARRA["Zerbitzariak eta Barra"]
        ZERB_AND["Android Komandera<br/>(zerbitzaria_komandera_android - Jetpack Compose)"]
        BAR_TPV["TPV Kutxa Zentrala<br/>(desktop_csharp - .NET 10)"]
    end

    subgraph SUKALDEA_BILTEGIA["Sukaldea eta Biltegia"]
        SUK_TV["Sukaldeko KDS Pantaila<br/>(sukaldea_kds_android_tv)"]
        BIL_JFX["KDS & Biltegi Kudeaketa<br/>(desktop_javafx - Java 21)"]
    end

    subgraph ADMIN_ERP["Kudeaketa eta Administrazioa"]
        ADM_TI["Admin IT Panela<br/>(admin_IT_zentrala)"]
        ERP_OD["Odoo ERP<br/>(odoo_erp)"]
    end

    API["REST API Zentrala<br/>(backend_api - ASP.NET Core)"]
    DB[("PostgreSQL Datu-Basea")]

    BEZ_WEB -->|HTTP/JSON| API
    BEZ_MOB -->|HTTP/JSON| API
    ZERB_AND -->|HTTP/JSON| API
    BAR_TPV -->|HTTP/JSON| API
    SUK_TV -->|HTTP/WebSockets| API
    BIL_JFX -->|HTTP/JSON| API
    ADM_TI -->|HTTP/JSON| API
    ERP_OD <-->|Sinkronizazioa| API

    API <--> DB
```

### 3.1. Bezeroen Interfazeak
* **Web-App (`01_aplikazioak/web_php`):** Jatetxearen atari publikoa. Bezeroak mahai-erreserbak egiteko aukera du, eguneko menua eta karta ikusi eta alergenoen araberako iragazketa egiteko. Stockik ez duten platerak automatikoki agortu gisa agertzen dira.
* **Mugikorreko Aplikazioa (`01_aplikazioak/mobile_bezeroa`):** Flutter bidez garatua, bezeroak bere gailutik mahai gaineko QR kodea eskaneatu eta eskaerak zuzenean bideratzeko aukera ematen du.

### 3.2. Zerbitzarien eta Jantokiaren Interfazeak
* **Komandera Android Natiboa (`01_aplikazioak/zerbitzaria_komandera_android`):** Jetpack Compose bidez sortua, telefono zein tablet tamainetara egokitzen den interfaze modernoa. Zerbitzariak mahaien egoera ikus dezake (Libre, Okupatua, Eskaerarekin), komandak platerka jaso, oharrak gehitu (adib. haragia puntuan) eta eskaera sukaldera igorri.
* **Multiplataforma Komandera (`01_aplikazioak/mobile_zerbitzaria`):** Flutter bidez egindako komandera alternatiboa.

### 3.3. Barrako eta Salmenta Puntuko Interfazea
* **C# .NET 10 Desktop TPV (`01_aplikazioak/desktop_csharp`):** Barran kokatutako ukipen-pantailarako terminal sendoa. Mahai bakoitzeko kontua ixtea, faktura zatituak kudeatzea, txartel/eskudiruzko kobraketak egitea eta eguneko itxiera ekonomikoa (Z kaxa) burutzea ahalbidetzen du.

### 3.4. Sukaldeko Interfazeak (KDS - Kitchen Display System)
* **Android TV KDS (`01_aplikazioak/sukaldea_kds_android_tv`):** Sukaldeko horman kokatutako telebista edo pantaila handirako pentsatua. Sartzen diren komandak txartel moduan bistaratzen ditu, lehentasunaren arabera ordenatuta eta denbora-markekin.
* **JavaFX KDS eta Biltegia (`01_aplikazioak/desktop_javafx`):** MVC arkitekturako mahaigaineko aplikazioa. Sukaldariek komandak ikusi eta "Prestatzen" edo "Bukatua" markatu ditzakete bertatik.

### 3.5. Biltegiko Interfazea
* **JavaFX Biltegi Modulua (`01_aplikazioak/desktop_javafx`):** Jatetxeko despentsa eta hozkailuetako stock errealaren jarraipena. Gutxieneko stocketik behera dauden lehengaiak gorriz nabarmentzen ditu eta hornitzaileentzako erosketa-proposamena automatikoki kalkulatzen du.
* **Odoo ERP integrazioa (`01_aplikazioak/odoo_erp`):** Erosketa-eskaera ofizialak, albaranak eta hornitzaileen fakturen kontabilitatea kudeatzeko.

### 3.6. Administrazioa eta TI Kudeaketa
* **Admin IT Zentrala (`01_aplikazioak/admin_IT_zentrala`):** Sistemaren administratzailearentzako kontrol-panela. Langileen kudeaketa (rolak, baimenak, PINak), jatetxeko gailuen monitorizazioa eta azpiegitura-zerbitzuen ikuskaritza ahalbidetzen ditu.
* **Backend REST API (`01_aplikazioak/backend_api`):** Aplikazio guztiei zerbitzua ematen dien nukleoa. Datu-basearekiko transakzio seguruak bermatzen ditu (stock deskontua, eskaera egoerak, balidazioak).

---

## 4. Azpiegitura Birtuala eta Segurtasuna

Proiektu honek bere azpiegitura birtual propioa du (`02_azpiegitura_birtuala/`), sare mailan isolamendu zorrotza bermatzen duena:
* **OPNsense Firewall:** Sareak segmentatzen ditu (Bezeroen Wi-Fi publikoa, Langileen sarea, eta Zerbitzarien DMZ / IT kudeaketa).
* **Docker Edukiontziak:** Sukaldeko zerbitzuak, APIa eta datu-basea modu arin eta errepikagarrian hedatzeko.
* **Active Directory / LDAP / RBAC:** Erabiltzaile bakoitzak bere profilari dagozkion datuak bakarrik ikus ditzakeela ziurtatuz.
