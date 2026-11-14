# 1T Jatetxea Erronka - Proiektu Globalaren Egitura Nagusia

## Goierri Eskola - Zibersegurtasuna eta Sare Sistemen Kudeaketa (2. Maila)

Proiektu honek jatetxe-kudeaketarako sistema integral oso bat biltzen du: programazio anitzeko aplikazioak (C#, JavaFX, Flutter, PHP, Android TV Jetpack Compose), ingurune birtualeko sare eta zerbitzari azpiegitura (VirtualBox OPNsense suebakiarekin eta Docker ekosistemarekin), Odoo 17 HR ERP atala (PostgreSQL), IT Kontrol Zentrala eta Goierri Eskolako laborategian muntatutako hardware azpiegitura erreala Cisco sare-ekipoekin.

---

## 1. Proiektuaren Karpeta-Egitura Orokorra (Monorepo Masterplan)

```text
1T_Jatetxea_Erronka/
├── README.md                                    # Proiektu orokorraren aurkezpen nagusia (fitxategi hau)
├── docker-compose.yml                           # Tokiko garapen eta zerbitzu orkestrazio orokorra (7 edukiontzi)
│
├── 01_aplikazioak/                              # Garatutako software ekosistema (Lengoaia eta motaren arabera)
│   ├── backend_api/                             # ASP.NET Core (.NET 10, C#) REST API nagusia (MySQL 8.0)
│   ├── desktop_csharp/                          # Windows Forms (.NET 10, C#) TPV / Kutxa Mahaia (Zerbitzari/Barra + Erreserbak)
│   ├── desktop_javafx/                          # JavaFX (Java 17/21, Maven) Sukaldea (KDS), Barra (BDS) eta Stocka
│   ├── mobile_bezeroa/                          # Flutter (Dart) Bezeroen mugikorrerako app-a (Karta, Erreserbak, Ordainketa)
│   ├── mobile_zerbitzaria/                      # Flutter (Dart) Zerbitzariaren PDA mugikorra (Eskaerak, Mahaiak)
│   ├── sukaldea_kds_android_tv/                 # Jetpack Compose for TV (Android TV 14, Kotlin) Sukaldeko Telebista Adimenduna
│   ├── web_php/                                 # Apache 2.4 + PHP 8.2 Webgune korporatiboa eta bezeroen ataria
│   ├── odoo_erp/                                # Odoo 17 ERP & Web Portala (Giza Baliabideak, Langileak, Kontratuak)
│   └── admin_IT_zentrala/                       # IT Kontrol Zentrala (Docker, VM eta Sare Kudeaketa Integrala)
│
├── 02_azpiegitura_birtuala/                     # Ingurune birtualeko baliabideak eta orkestrazioa
│   ├── 1T_Jatetxea_Entorno_Birtuala/            # Docker edukiontziak (MySQL, API, Odoo, Chat, Nginx) eta switch-env.ps1
│   └── erronka_egitura/                         # 3 Makina birtualak (OPNsense, WinServer2025, Ubuntu) eta headless scriptak
│
├── 03_laborategi_fisikoa/                       # Goierri Eskolako muntaketa fisiko errealaren dokumentazioa
│   ├── README.md                                # Laborategiko gida orokorra
│   ├── 01_hardware_inbentarioa/                 # Router, Switch, PC, TPV, Inprimagailu eta Pantailen fitxa teknikoak
│   ├── 02_cisco_packet_tracer/                  # Packet Tracer simulazioa (.pkt) eta Cisco IOS CLI scriptak (.cfg)
│   ├── 03_kableatu_eta_rack/                    # Rack 19" banaketa, patch panelak eta kolore-kodeak
│   └── 04_ip_eta_vlan_plana/                    # Sare fisikoaren segmentazioa eta OPNsense suebaki matrizea
│
└── 04_dokumentazioa/                            # Txosten teknikoak, diagramak eta ebaluaziorako runbook-ak
    └── espezifikazioak/                         # SQL eskemak, diagrama arkitektonikoak, proba-protokoloak
```

---

## 2. Aplikazioen Zerrenda eta Teknologia Matrizea

| Aplikazioa | Mota | Lengoaia / Framework | Funtzio Nagusia | Portua / Sarbidea |
|---|---|---|---|---|
| **IT Kontrol Zentrala** | Admin Web/Desktop | ASP.NET Core (.NET 11) | Docker edukiontziak, VirtualBox makinak eta zerbitzu guztien kontrola | `http://localhost:5000` |
| **Backend API** | REST API | C# (.NET 10) | Datuen zentralizazioa, negozio logika, transakzio pesimistak | `http://localhost:5050` |
| **Desktop TPV** | Desktop UI | C# WinForms (.NET 10) | Kutxa nagusia, mahai kudeaketa, zerbitzari/barrako rolen bereizketa eta **telefonoz/pertsonan erreserbak egitea** | Windows Lanpostua |
| **Sukaldea / KDS** | Desktop UI | Java 17/21 (JavaFX) | Sukaldeko plateren egoerak, barrako edariak, biltegi stocka | Ukipen-pantaila / PC |
| **Mobile Bezeroa** | Mugikorra | Dart (Flutter) | Karta digitala, mahai erreserbak, ordainketa eta tiketa | Android / iOS |
| **Mobile Zerbitzaria** | Mugikorra | Dart (Flutter) | Zerbitzariaren PDA: eskaerak mahaitik hartu eta sukaldera bidali | Android PDA |
| **Android TV KDS** | Smart TV App | Kotlin / Jetpack Compose for TV | Sukaldeko hormako telebista: D-Pad urruneko agintearekin platerak kontrolatu | Android TV (1080p/4K) |
| **Web Ataria** | Web | PHP 8.2 / Apache | Jatetxearen webgune publikoa, karta bistaratzea eta online erreserbak | `http://localhost:8085` |
| **Odoo 17 ERP** | ERP Kudeaketa | Python / Odoo 17 | Erabiltzaileak, ordutegiak, fitxaketak, bajak | `http://localhost:8070` |
| **PostgreSQL DB** | Datu-Basea | PostgreSQL 15 | Odoo ERP-aren taulak eta transakzioak gordetzeko datu-basea | `127.0.0.1:5432` |

---

## 3. Proiektua Martxan Jartzeko Gida (Hasierako Konfigurazio Morroia)

Proiektua edozein makinatan abiarazteko modurik errazena erroko exekutagarria erabiltzea da:

1. Exekutatu **`Admin_IT_Zentrala.exe`** erroko karpetatik edo mahaigaineko lasterbidetik.
2. Morroi grafikoa irekiko da:
   - **Lehen aldiz exekutatzen duten taldekideentzat:** Sakatu `INSTALATU ETA PRESTATU PROIEKTU OSOA` (sareak, bolumenak, irudiak eta konpilazioak automatikoki gauzatuko ditu).
   - **Dagoeneko instalatuta baduzu:** Sakatu `AURRERA JARRAITU (IREKI 5000)`.
3. IT Kontrol Zentrala automatikoki irekiko da nabigatzailean: `http://localhost:5000/`. Handik zerbitzu guztiak klik bakar batean kontrola daitezke.

---

## 4. Desktop C# TPV: Zerbitzari eta Barrako Rolen Kudeaketa & Erreserbak

Aplikazio honek jatetxearen kutxa eta mahaiak kudeatzen ditu:
* **Zerbitzariak (`Rola = Zerbitzaria` edo `AdminIT`):**
  - Kategoria guztietako eskaerak egin ditzakete: Hastekoak, Haragiak, Arrainak, Postreak eta Edariak.
* **Barrakoak (`Rola = Barrakoa`):**
  - **Ez dituzte janari eskariak egingo:** Sukaldeko janari kategoriak (Hastekoak, Haragiak, etab.) ezkutatu edo blokeatu egiten dira.
  - **Soilik edariak, kafeak eta barrako produktuak** eskatzeko baimena dute.
* **Erreserbak Egiteko Funtzionalitatea (`ErreserbakForm` & `ErreserbaBerriaModal`):**
  - Zerbitzariek eta barrakoek bezeroen erreserbak zuzenean sartu ditzakete TPVtik, bezeroak telefonoz deitzen badu edo pertsonan barrara/sarrerara hurbiltzen bada.
  - Erreserben zerrenda dataren eta txandaren arabera iragazten da.
  - Erreserba berria sartzean: bezeroaren izena, abizena, telefonoa, data, txanda (`Bazkaria_1`, `Bazkaria_2`, `Afaria_1`, `Afaria_2`), pertsona kopurua (pax), mahaia eta oharrak biltzen dira, eta backend APIra transakzionalki bidaltzen da.

---

## 5. Zuzendaritza Taldea (3 Zuzendari)

Jatetxearen kudeaketa orokorra 3 zuzendarik osatzen dute:
1. **Ander Madinabeitia** (NAN: `72548913N`, Erabiltzailea: `ander_m_zuz`, Kontratua: `KONT-2026-ZUZ01`, Soldata: 3.200,00 €)
2. **Joseba Ramos** (NAN: `72548914P`, Erabiltzailea: `joseba_r_zuz`, Kontratua: `KONT-2026-ZUZ02`, Soldata: 3.200,00 €)
3. **Estebe Ormazabal** (NAN: `72548915Q`, Erabiltzailea: `estebe_o_zuz`, Kontratua: `KONT-2026-ZUZ03`, Soldata: 3.200,00 €)

---

## 6. Sare eta Zibersegurtasun Arkitektura (OPNsense + Cisco)

- **Segmentazioa (VLAN):**
  - **VLAN 10 (LAN Barnea - 192.168.10.0/24):** Zerbitzari nagusiak (Active Directory, MySQL, API), TPVak eta sukaldeko KDSak.
  - **VLAN 20 (DMZ Publikoa - 192.168.20.0/24):** Apache Web Zerbitzaria. Kanpotik ikusgai dagoena baina ezin duena zuzenean LANeko datu-basea ikusi.
  - **VLAN 30 (WiFi Bezeroak - 192.168.30.0/24):** Jatetxeko bezeroen mugikorrak. Internet eta DMZ sarbidea soilik.
  - **VLAN 99 (Kudeaketa - 192.168.99.0/24):** Switch, suebaki eta zerbitzarien urruneko kudeaketa segurua (SSH, HTTPS).
- **Router-on-a-Stick:** OPNsense suebakia eta Cisco Catalyst 2960 kommutadorearen arteko lotura 802.1Q trunk bidez.
