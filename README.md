# 1T Jatetxea Erronka - Proiektu Globalaren Egitura Nagusia

## Goierri Eskola - Zibersegurtasuna eta Sare Sistemen Kudeaketa (2. Maila)

Proiektu honek jatetxe-kudeaketarako sistema integral oso bat biltzen du: programazio anitzeko aplikazioak (C#, JavaFX, Flutter, PHP, Android TV Jetpack Compose), ingurune birtualeko sare eta zerbitzari azpiegitura (VirtualBox OPNsense suebakiarekin eta Docker ekosistemarekin), Odoo 17 HR ERP atala (PostgreSQL), IT Kontrol Zentrala eta Goierri Eskolako laborategian muntatutako hardware azpiegitura erreala Cisco sare-ekipoekin.

---

## 1. Proiektuaren Karpeta-Egitura Orokorra (Monorepo Masterplan)

```text
1T_Jatetxea_Erronka/
├── README.md                                    # Proiektu orokorraren aurkezpen nagusia (fitxategi hau)
├── Admin_IT_Zentrala.exe                        # Windows exekutagarri nagusia (Hasierako Konfigurazio Morroia eta Abiarazlea)
├── admin_it_zentrala.ico                        # Aplikazioaren ikono ofiziala
├── docker-compose.yml                           # Tokiko garapen eta zerbitzu orkestrazio orokorra (9 edukiontzi)
├── .gitignore                                   # Biltegi garbiko iragazketa arauak (ISO, VDI, bin/obj, tokenak baztertuta)
│
├── 01_aplikazioak/                              # Garatutako software ekosistema osoa
│   ├── admin_IT_zentrala/                       # IT Kontrol Zentrala (ASP.NET Core Web zerbitzaria + WinForms Morroia)
│   ├── backend_api/                             # ASP.NET Core (.NET 10, C#) REST API nagusia (MySQL 8.0)
│   ├── desktop_csharp/                          # Windows Forms (.NET 10, C#) TPV / Kutxa Mahaia (Zerbitzari/Barra + Erreserbak)
│   ├── desktop_javafx/                          # JavaFX (Java 17/21, Maven) Sukaldea (KDS), Barra (BDS) eta Stocka
│   ├── sukaldea_kds_android_tv/                 # Jetpack Compose for TV (Android TV 14, Kotlin) Sukaldeko Telebista Adimenduna
│   ├── web_php/                                 # Apache 2.4 + PHP 8.2 Webgune korporatiboa eta bezeroen karta publikoa
│   ├── mobile_bezeroa/                          # Flutter (Dart) Bezeroen mugikorrerako app-a (Karta, Erreserbak, Ordainketa)
│   ├── mobile_zerbitzaria/                      # Flutter (Dart) Zerbitzariaren PDA mugikorra (Eskaerak mahaitik hartu)
│   ├── odoo_erp/                                # Odoo 17 ERP & Web Portala (Giza Baliabideak, Langileak, Kontratuak)
│   └── txat_zerbitzaria/                        # Socket.IO / Node.js denbora errealeko barne txat zerbitzaria
│
├── 02_azpiegitura_birtuala/                     # Ingurune birtualeko baliabideak eta orkestrazioa
│   ├── 1T_Jatetxea_Entorno_Birtuala/            # Docker edukiontziak (MySQL, API, Odoo, Chat, Nginx, phpMyAdmin, Adminer)
│   └── erronka_egitura/                         # 3 Makina birtualak (OPNsense, WinServer2025, Ubuntu) eta headless scriptak
│
├── 03_laborategi_fisikoa/                       # Goierri Eskolako muntaketa fisiko errealaren dokumentazioa
│   ├── README.md                                # Laborategiko gida orokorra
│   ├── 01_hardware_inbentarioa/                 # Router, Switch, PC, TPV, Inprimagailu eta Pantailen fitxa teknikoak
│   ├── 02_cisco_packet_tracer/                  # Packet Tracer simulazioa (.pkt) eta Cisco IOS CLI scriptak (.cfg)
│   ├── 03_kableatu_eta_rack/                    # Rack 19" banaketa, patch panelak eta kolore-kodeak (TIA/EIA-568-B)
│   └── 04_ip_eta_vlan_plana/                    # Sare fisikoaren segmentazioa eta OPNsense suebaki matrizea
│
└── 04_dokumentazioa/                            # Txosten teknikoak, diagramak eta ebaluaziorako runbook-ak
    └── espezifikazioak/                         # SQL eskemak, diagrama arkitektonikoak, proba-protokoloak
```

---

## 2. Arkitektura Orokorra: 3 Mailako Eredu Integratua

```text
+---------------------------------------------------------------------------------------------------+
|                                 1. MAILA: AURRE-MAILA (BEZEROAK)                                  |
|                                                                                                   |
|  +--------------------+   +---------------------+   +--------------------+   +-----------------+  |
|  | Web Karta Publikoa |   | Desktop C# TPV      |   | JavaFX Sukalde/Bar |   | Android TV KDS  |  |
|  | (PHP 8.2 / Apache) |   | (WinForms .NET 10)  |   | (JavaFX / Maven)   |   | (Kotlin Compose)|  |
|  +---------+----------+   +----------+----------+   +---------+----------+   +--------+--------+  |
|            |                         |                        |                       |           |
|            |   +---------------------+   +--------------------+                       |           |
|            |   | Flutter Zerbitzaria |   | Flutter Bezeroa    |                       |           |
|            |   | (Android PDA)       |   | (Mugikorra)        |                       |           |
|            |   +----------+----------+   +---------+----------+                       |           |
+------------|--------------|------------------------|----------------------------------|-----------+
             |              |                        |                                  |
             +--------------+------------+-----------+----------------------------------+
                                         | HTTP / JSON REST (Portu 5050)
                                         v
+---------------------------------------------------------------------------------------------------+
|                                 2. MAILA: NEGOZIO LOGIKA & BACKEND                                |
|                                                                                                   |
|     +-------------------------------------------------+     +---------------------------------+   |
|     |     ASP.NET Core REST API Nagusia (.NET 10)     |     | Barne Txat Zerbitzaria (Node.js)|   |
|     |  32 Endpoint: Mahaiak, Platerak, Eskaerak,      |     | Socket.IO / WebSockets (3001)   |   |
|     |  Erreserbak, Stocka, Langileak, Iritziak        |     | Denbora errealeko komunikazioa  |   |
|     |  Transakzio Pesimistak (SELECT FOR UPDATE)      |     +---------------------------------+   |
|     +------------------------+------------------------+                                           |
+------------------------------|--------------------------------------------------------------------+
                               | TCP Socket Bitarra (Portu 3306 barnean / 3308 kanpoan)
                               v
+---------------------------------------------------------------------------------------------------+
|                               3. MAILA: DATU-BASEAK & KUDEAKETA ERP                               |
|                                                                                                   |
|     +-------------------------------------+         +-------------------------------------+       |
|     | MySQL 8.0 InnoDB (jatetxea_db)      |         | PostgreSQL 15 (odoo_db)             |       |
|     | Portua: 3308 (Raw DB Socket)        |         | Portua: 5432 (Odoo DB)              |       |
|     | Web Admin: phpMyAdmin (Portu 8081)  |         | Web Admin: Adminer (Portu 8082)     |       |
|     +-------------------------------------+         +------------------+------------------+       |
|                                                                        |                          |
|                                                     +------------------+------------------+       |
|                                                     | Odoo 17 HR ERP & Web Portala        |       |
|                                                     | Portua: 8070 (Werkzeug Python 3.11) |       |
|                                                     +-------------------------------------+       |
+---------------------------------------------------------------------------------------------------+
```

---

## 3. Aplikazio Guztien Laburpen Zehatza (Ecosystem Summary)

Proiektuak 12 software osagai nagusi biltzen ditu, bakoitzak bere ataka eta helburu espezifikoarekin:

| Aplikazioa | Mota | Teknologia | Portua / Sarbidea | Funtzio Nagusia |
|---|---|---|---|---|
| **IT Kontrol Zentrala** | Kudeaketa Orokorra | ASP.NET Core (.NET 11) | `http://localhost:5000` | Zerbitzari, Docker edukiontzi, VirtualBox makina eta sare profilen kontrol-panel grafiko osoa. |
| **Hasierako Morroia** | Desktop Launcher | C# WinForms (.NET 8/10/11) | [Admin_IT_Zentrala.exe](Admin_IT_Zentrala.exe) | Taldekideei proiektu osoa automatikoki instalatu, egiaztatu eta konfiguratzeko morroi eramangarria. |
| **Backend REST API** | REST API Nagusia | C# ASP.NET Core (.NET 10) | `http://localhost:5050` | Negozio logika zentralizatua, 32 endpoint, transakzio pesimistak (`SELECT ... FOR UPDATE`), aforoa eta stock murrizketa. |
| **Desktop TPV** | Salmenta-Puntua | C# WinForms (.NET 10) | Windows Lanpostua | Mahaiaren kudeaketa, kutxa, ordainketak, tiketak, zerbitzari/barrako rolen iragazpena eta **erreserbak hartzea**. |
| **Desktop JavaFX** | Sukaldea / Barra / Stock | Java 17/21 (JavaFX / Maven) | Ukipen-pantaila / PC | Sukaldeko KDS (plater egoerak), Barrako BDS (edariak soilik) eta Biltegiko Stock kontrola albaranekin. |
| **Android TV KDS** | Smart TV App | Kotlin / Compose for TV | Android TV (1080p/4K) | Sukaldeko hormako telebista: D-Pad bidez eskaeren egoerak aldatzeko pantaila espezifikoa. |
| **Sukaldea KDS TV Web** | Pantaila Bistaratzailea | Nginx Alpine / HTML5 | `http://localhost:8086` | Sukaldeko pantaila bistaratzaile arina web bidez telebistetara zabaltzeko. |
| **Web Publikoa** | Karta eta Erreserbak | Apache 2.4 / PHP 8.2 | `http://localhost:8085` | Jatetxeko webgune korporatiboa, karta interaktiboa, alergenoak, stockaren araberako plater erabilgarritasuna eta iritziak. |
| **Mobile Bezeroa** | Mugikorreko App-a | Dart (Flutter) | Android / iOS | Bezeroaren aplikazioa: karta ikusi, mahaiko QR bidez eskatu eta erreserbak kudeatu. |
| **Mobile Zerbitzaria** | Zerbitzariaren PDA | Dart (Flutter) | Android PDA | Zerbitzariaren eskuko gailua: mahaietan komandak hartu eta sukaldera berehala bidaltzeko. |
| **Odoo 17 HR ERP** | Enpresa Kudeaketa | Python 3.11 / Odoo 17 | `http://localhost:8070` | Giza baliabideen modulua: langileen fitxak, kontratuak, nominak, lan-ordutegiak eta fitxaketak. |
| **Barne Txat Zerbitzaria** | Komunikazioa | Node.js / Socket.IO | `http://localhost:3001` | Zerbitzarien, sukaldearen eta kudeatzaileen arteko denbora errealeko barne mezularitza. |
| **phpMyAdmin** | MySQL Web Admin | phpMyAdmin 5.2 | `http://localhost:8081` | MySQL datu-basearen taulak, datuak eta kontsultak nabigatzailetik kudeatzeko tresna grafikoa. |
| **Adminer** | Postgres Web Admin | Adminer Latest | `http://localhost:8082` | PostgreSQL datu-basearen (Odoo DB) kudeaketa grafikoa web bidez. |

---

## 4. Taldeak eta Rol Guztiak (Organizazio Egitura & Roster Ofiziala)

Jatetxeak eta IT azpiegiturak definitutako giza talde osoa ondorengo profesionalek osatzen dute (datu-basean eta Active Directory-n erregistratuak):

### 4.1. IT Zuzendaritza eta Sistemak (Admin IT Taldea)
Sistemaren azpiegitura birtuala, suebakia, segurtasuna, sareak eta kodearen mantentze-lanak bermatzen dituen talde teknikoa:

| Izena eta Abizena | Erabiltzailea | NAN / DNI | Saila | Kargua | Posta Elektronikoa |
|---|---|---|---|---|---|
| **Ander Urien Telleria** | `anderu` | `72548901A` | Sistemak_IT | **IT Lead / Arkitekto Nagusia** | `ander.urien@goierrieskola.eus` |
| **Julen Aguado Fernández** | `julen` | `72548902B` | Sistemak_IT | **Sare eta Suebaki Ingeniaria** | `julen.aguado@goierrieskola.eus` |
| **Iker Araujo Dominguez** | `iker` | `72548903C` | Sistemak_IT | **Sistema Eragileak eta Birtualizazioa** | `iker.araujo@goierrieskola.eus` |
| **Aratz Huegun Flores** | `aratz` | `72548904D` | Sistemak_IT | **Zibersegurtasun eta Auditoritza Burua** | `aratz.huegun@goierrieskola.eus` |

### 4.2. Zuzendaritza Kontseilua (3 Zuzendari)
Jatetxearen kudeaketa orokorra, finantzak, baimenak eta negozio-erabakiak hartzen dituen zuzendaritza-taldea:

| Izena eta Abizena | Erabiltzailea | NAN / DNI | Kontratua | Soldata Gordina | Kargua |
|---|---|---|---|---|---|
| **Ander Madinabeitia** | `ander_m_zuz` | `72548913N` | `KONT-2026-ZUZ01` | 3.200,00 € | Zuzendaritza Kontseilukoa / Zuzendaria |
| **Joseba Ramos** | `joseba_r_zuz` | `72548914P` | `KONT-2026-ZUZ02` | 3.200,00 € | Zuzendaritza Kontseilukoa / Zuzendaria |
| **Estebe Ormazabal** | `estebe_o_zuz` | `72548915Q` | `KONT-2026-ZUZ03` | 3.200,00 € | Zuzendaritza Kontseilukoa / Zuzendaria |

### 4.3. Sukaldeko Taldea (`Rola = Sukaldaria`)
Plateren prestaketa, errezeta eta eskandalloen jarraipena eta KDS pantailen kudeaketa:

| Izena eta Abizena | Erabiltzailea | NAN / DNI | Kargua | Telefonoa |
|---|---|---|---|---|
| **Ainhoa Goikoetxea** | `ainhoa_chef` | `72548905E` | Chef Burua / Sukaldari Nagusia | `677112233` |
| **Eneko Otegi** | `eneko_suk` | `72548906F` | Bigarren Sukaldaria / Parrilleroa | `677112234` |

### 4.4. Jantokia eta Zerbitzariak (`Rola = Zerbitzaria`)
Mahaien arreta, eskaeren sarrera (TPV eta PDA), zerbitzua eta bezeroen kobrantza:

| Izena eta Abizena | Erabiltzailea | NAN / DNI | Kargua | Telefonoa |
|---|---|---|---|---|
| **Mikel Arregi** | `mikel_zerb` | `72548907G` | Maître / Zerbitzari Burua | `666112233` |
| **Maite Zabaleta** | `maite_zerb` | `72548908H` | Jantokiko Zerbitzaria | `666112234` |
| **Unai Lasa** | `unai_zerb` | `72548909J` | Jantokiko Zerbitzaria | `666112235` |

### 4.5. Barrako Taldea (`Rola = Barrakoa`)
Kafetegi eta edarien zerbitzua, barrako eskaerak eta BDS pantailaren kudeaketa:

| Izena eta Abizena | Erabiltzailea | NAN / DNI | Kargua | Telefonoa |
|---|---|---|---|---|
| **Gorka Mendizabal** | `gorka_barra` | `72548910K` | Barman / Barrako Arduraduna | `655112233` |
| **Uxue Etxarri** | `uxue_barra` | `72548911L` | Barrako Zerbitzaria | `655112234` |

### 4.6. Biltegi eta Stock Taldea (`Rola = Biltegikoa`)
Hornitzaileen albaranak erregistratu, lehengaien sarrerak egin eta gutxieneko stock alertak kontrolatu:

| Izena eta Abizena | Erabiltzailea | NAN / DNI | Kargua | Telefonoa |
|---|---|---|---|---|
| **Kepa Aldasoro** | `kepa_biltegi` | `72548912M` | Biltegi eta Stock Burua | `644112233` |

### 4.7. Kanpoko Erabiltzaileak: Bezeroak (`Rola = Bezeroa`)
Web ataritik edo mugikorreko app-tik sartzen diren jatetxeko bezeroak:
- Karta kontsultatu, alergenoak iragazi eta stockaren araberako erabilgarritasuna ikusi.
- Mahai-erreserbak online egin dataren eta txandaren arabera (`Bazkaria_1`, `Bazkaria_2`, `Afaria_1`, `Afaria_2`).
- Bazkaldu/afaldu ostean iritziak eta 0-5 izarretako balorazioak utzi.

---

## 5. Rolen Baimenak eta Funtzionamendu Matrizea (RBAC)

Sistemak baimen-matrize zorrotza betetzen du:

| Funtzionalitatea | AdminIT | Kudeatzailea | Zerbitzaria | Barrakoa | Sukaldaria | Biltegikoa | Bezeroa |
|---|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| **IT Kontrol Zentrala (5000)** | Bai | Ez | Ez | Ez | Ez | Ez | Ez |
| **TPV Kutxa Osoa & Kobrantzak** | Bai | Bai | Bai | Soilik Barra | Ez | Ez | Ez |
| **Janari Eskaerak Sartu (Sukaldera)** | Bai | Bai | Bai | **EZ (Blokeatua)** | Ez | Ez | Ez |
| **Edariak / Kafeak Eskatu** | Bai | Bai | Bai | Bai | Ez | Ez | Ez |
| **Erreserbak Sartu (Telefonoz / Aurrez)**| Bai | Bai | Bai | Bai | Ez | Ez | Soilik Online |
| **KDS Sukaldeko Egoerak Aldatu** | Bai | Ez | Ez | Ez | Bai | Ez | Ez |
| **BDS Barrako Egoerak Aldatu** | Bai | Ez | Ez | Bai | Ez | Ez | Ez |
| **Albaranak / Stock Sarrerak** | Bai | Bai | Ez | Ez | Ez | Bai | Ez |
| **Langileak / Nominak / Odoo ERP** | Bai | Bai | Ez | Ez | Ez | Ez | Ez |
| **Karta Publikoa & Iritziak** | Bai | Bai | Bai | Bai | Bai | Bai | Bai |

---

## 6. Proiektua Martxan Jartzeko Gida (Hasierako Konfigurazio Morroia)

Proiektua edozein makinatan abiarazteko modurik seguru eta azkarrena erroko exekutagarria erabiltzea da:

1. Exekutatu **`Admin_IT_Zentrala.exe`** erroko karpetatik edo mahaigaineko [1T Jatetxea - Admin IT Zentrala.lnk](file:///C:/Users/anurt/Desktop/1T%20Jatetxea%20-%20Admin%20IT%20Zentrala.lnk) lasterbidetik.
2. Morroi grafikoa irekiko da:
   - **Lehen aldiz exekutatzen duten taldekideentzat:** Sakatu `INSTALATU ETA PRESTATU PROIEKTU OSOA` (sareak, bolumenak, Docker edukiontziak eta konpilazioak automatikoki gauzatuko ditu aurrerapen-barrarekin eta log monitorizatuarekin).
   - **Dagoeneko instalatuta badago:** Sakatu `AURRERA JARRAITU (IREKI 5000)`.
3. IT Kontrol Zentrala automatikoki irekiko da nabigatzailean: `http://localhost:5000/`. Handik zerbitzu guztiak klik bakar batean kontrola daitezke:
   - **1. Urratsa (Datu-baseak):** MySQL (3308), PostgreSQL (5432), phpMyAdmin (8081) eta Adminer (8082) abiarazi.
   - **2. Urratsa (Backend):** ASP.NET Core REST API (5050) eta Barne Txata (3001) abiarazi.
   - **3. Urratsa (Frontend):** Web Publikoa (8085), KDS TV (8086) eta Odoo ERP (8070) abiarazi.

---

## 7. Sare eta Zibersegurtasun Arkitektura (OPNsense + Cisco)

Sare azpiegitura osoa segmentazio sendo baten gainean eraikita dago:

- **Segmentazioa (VLAN):**
  - **VLAN 10 (LAN Barnea - 192.168.10.0/24):** Zerbitzari nagusiak (Active Directory, MySQL, REST API), TPVak eta sukaldeko KDSak.
  - **VLAN 20 (DMZ Publikoa - 192.168.20.0/24):** Apache Web Zerbitzaria. Kanpotik ikusgai dagoena baina ezin duena zuzenean LANeko datu-basea ikusi.
  - **VLAN 30 (WiFi Bezeroak - 192.168.30.0/24):** Jatetxeko bezeroen mugikorrak. Internet eta DMZ sarbidea soilik (isolatuta).
  - **VLAN 99 (Kudeaketa / IT - 192.168.99.0/24):** Switch, suebaki eta zerbitzarien urruneko kudeaketa segurua (SSH, HTTPS).
- **Suebakia (OPNsense Appliance):**
  - Router-on-a-Stick arkitektura Cisco Catalyst 2960 kommutadorearekin (802.1Q trunking).
  - Zero-Trust politika: DMZ -> LAN debekatuta (soilik APIaren 5050 atakarako baimen zehatza).
  - DMZ -> MySQL 3308 zuzenean guztiz blokeatua.
- **Laborategi Fisikoa (Goierri Eskola):**
  - Cisco Catalyst 2960 Switch konfigurazio erreala ([switch_cisco_catalyst_konfigurazioa.cfg](03_laborategi_fisikoa/02_cisco_packet_tracer/konfigurazio_scriptak_ios/switch_cisco_catalyst_konfigurazioa.cfg)).
  - Port Security gaituta (Sticky MAC helbideekin), BPDU Guard eta spanning-tree segurua.
  - Rack 19" kableatu egituratua TIA/EIA-568-B arauaren araberako patch panelekin.

---

## 8. Datu-Basearen Eredua eta Transakzio Pesimistak

Jatetxearen datu-base nagusiak (MySQL 8.0 InnoDB) transakzio pesimistak erabiltzen ditu konkurrentzia-arazoak ekiditeko:

- **Lehengaien Stock Deskontua:**
  Eskaera berri bat sartzen denean, plater bakoitzaren errezetaren araberako lehengaiak blokeatzen dira `SELECT ... FOR UPDATE` bidez, stock nahikorik ez badago transakzioa bertan behera utziz (`ROLLBACK`) eta stock negatiborik inoiz ez onartuz (`CHECK (uneko_stocka >= 0)`).
- **Mahai-Erreserben Aforoa:**
  Data eta txanda berean mahai bat bikoiztuta ez erreserbatzeko, gako bakar konposatua erabiltzen da (`UNIQUE KEY uq_erreserba (mahaia_id, data, txanda)`). Talka gertatuz gero, APIak `409 Conflict` erantzuten du bezeroari beste mahai edo txanda bat eskainiz.
