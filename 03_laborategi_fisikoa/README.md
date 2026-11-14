# Goierri Eskola - Laborategi Fisikoa: 1T Jatetxea Muntaketa Erreala

Dokumentu honek Goierri Eskolako zibersegurtasun eta sare laborategian gauzatuko den azpiegitura fisikoaren hedapena zehazten du. Proiektu honek ingurune birtualeko arkitektura (OPNsense, Ubuntu DMZ eta Windows Server LAN) laborategiko ekipo eta kableatu errealetara eramaten du.

---

## 1. Helburua eta Ikuspegi Orokorra

Simulazio birtualean (VirtualBox eta Docker bidez) garatutako eta balioztatutako jatetxe-sistema osoa laborategiko ekipo fisikoetan hedatzea:
- **Router-on-a-Stick eta Segmentazio Fisikoa:** Kudeatutako Cisco switch eta suebaki fisiko baten bidez VLANen trunking-a eta sarbide-portuak konfiguratzea.
- **Puntu Amaierako Gailu Errealak (Endpoints):**
  - Zerbitzari fisikoak / Laborategiko PCak.
  - TPV lanpostu fisikoa (C# WinForms aplikazioa + Ticket inprimagailua).
  - Sukaldeko pantaila / TV KDS (Android TV edo HDMI pantaila).
  - PDA / Mugikor errealak zerbitzarientzat (Samsung gailua ADB bidez probatua).
  - Bezeroen WiFi sarbidea (AP fisikoa) erreserbak eta eskaerak mugikorretik egiteko.

---

## 2. Direktorioen Antolaketa

```text
laborategi_fisikoa/
├── README.md                                    # Gida nagusia (fitxategi hau)
├── 01_hardware_inbentarioa/                     # Ekipo fisikoen fitxa teknikoak eta serialak
│   └── inbentario_orokorra.md
├── 02_cisco_packet_tracer/                      # Simulazioa eta gailuen CLI scriptak
│   ├── README.md                                # Packet Tracer gida eta topologia
│   └── konfigurazio_scriptak_ios/               # Cisco Switch eta Router konfigurazioak (.cfg / .txt)
│       ├── switch_cisco_catalyst_konfigurazioa.cfg
│       └── router_opnsense_interfazeak.txt
├── 03_kableatu_eta_rack/                        # Kableatua, patch panelak eta rack antolaketa
│   └── rack_eta_kableatu_egituratua.md
└── 04_ip_eta_vlan_plana/                        # Sare fisikoaren segmentazioa eta segurtasuna
    └── helbideratze_eta_segmentazio_plana.md
```

---

## 3. Ekipoen Banaketa Eremuka (Laborategia vs. Jatetxe Erreala)

| Eremua | Ekipo Fisikoak | VLAN | Funtzioa |
| :--- | :--- | :--- | :--- |
| **Sare Nukleoa (Rack)** | Cisco Catalyst Switch + Suebaki Fisikoa (MiniPC / APU OPNsenserekin) | VLAN 99 (Mgmt), Trunk | Bideratzea, suebakia, VLAN etiketatzea (802.1Q) |
| **Zerbitzarien Gunea (LAN)** | PC 1 (Windows Server 2025) + PC 2 (Ubuntu Server) | VLAN 10 (LAN), VLAN 20 (DMZ) | Datu-baseak, APIak, Active Directory, Odoo |
| **Barra eta Kutxa (LAN)** | PC 3 (C# TPV) + Ticket inprimagailu termikoa + Bar-kode irakurgailua | VLAN 10 (LAN) | Mahaiaren kudeaketa, fakturazioa, ordainketak |
| **Sukaldea (LAN/BDS)** | PC 4 / NUC / Android TV pantaila (JavaFX KDS) | VLAN 10 (LAN) | Plateren prestaketa, sukaldeko eta barrako bistaratzailea |
| **Jantokia eta Zerbitzariak** | WiFi Access Point 1 (SSID: `Jatetxea_Langileak`) + PDA/Tablet mugikorrak | VLAN 10 (LAN) | Zerbitzarien eskaerak mahaietan hartzeko |
| **Bezero Publikoak** | WiFi Access Point 2 edo VAP (SSID: `Jatetxea_Bezeroak_Free`) | VLAN 30 (Guest/WiFi) | Karta digitala ikusi, mahaitik eskatu eta ordaindu |
