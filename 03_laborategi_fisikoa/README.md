# Goierri Eskola - Laborategi Fisikoa: 1T Jatetxea Muntaketa Erreala

Dokumentu honek Goierri Eskolako zibersegurtasun eta sare laborategian gauzatutako azpiegitura fisikoaren egoera erreala, kableatua, ekipoak, helbideratzea, suebaki-arauak eta sarbide-kredentzialak jasotzen ditu.

---

## 1. Helburua eta Egungo Egoera (2026-11-25)

Ingurune birtualeko arkitektura (OPNsense, Windows Server 2025 LAN eta Ubuntu DMZ) laborategiko hardware errealera eraman da eta **erabat operatibo** dago:

- **OPNsense 25.7 (amd64):** 3 NIC fisiko bereizirekin (`re2` WAN, `re1` LAN, `re0` OPT1/DMZ) bideratzea, NAT eta Zero-Trust segmentazioa kudeatzen.
- **Windows Server 2025 (`WIN-U1CN1H8S33S`):** VLAN 10 (`192.168.10.19/24`) barne-sarean konektatuta, Interneterako irteerarekin eta RDP bidez kudeagarri.
- **Ubuntu Server 26.04.1 LTS (`1taldea-dmz`):** VLAN 20 (`192.168.20.10/24`) DMZ sarean isolatuta (LAN blokeatuta, Internet baimenduta), SSH bidez kudeagarri.
- **Bi Switch Fisiko:** Gelako Rack-eko Switch nagusia (`SW`) eta mahai gaineko **Cisco SG200-50** Smart Switch-a (`LAN`, `DMZ`, `WAN` ataka-taldeekin).

---

## 2. Direktorioen Antolaketa

```text
03_laborategi_fisikoa/
├── README.md                                        # Gida nagusia eta laburpena (fitxategi hau)
├── sare_fisikoaren_azterketa.md                     # Sare osoaren azterketa, Mermaid diagrama eta diagnostikoa
├── 01_hardware_inbentarioa/                         # Ekipo fisikoen fitxa teknikoak, MACak eta OSak
│   └── inbentario_orokorra.md
├── 02_cisco_packet_tracer/                          # Simulazioa eta Cisco IOS erreferentzia-scriptak
│   ├── README.md
│   └── konfigurazio_scriptak_ios/
│       └── switch_cisco_catalyst_konfigurazioa.cfg
├── 03_kableatu_eta_rack/                            # Kableatua, switch-ak, portuak eta rack antolaketa
│   └── rack_eta_kableatu_egituratua.md
├── 04_ip_eta_vlan_plana/                            # IP/VLAN plana, suebaki-arauak, komandoak eta kredentzialak
│   └── helbideratze_eta_segmentazio_plana.md
└── irudiak/                                         # Arbeleko eskemak eta OPNsense pantaila-argazkiak
```

---

## 3. Muntaketa Errealaren Laburpen Taula

| Ekipoa / Gailua | Eremua / VLAN | IP Helbidea | Gateway | Sarbidea / Protokoloa |
| :--- | :--- | :--- | :--- | :--- |
| **Goierri Eskola Gateway** | WAN (`VLAN 30`) | `192.168.115.254/24` | `192.168.3.1` | Fortinet DHCP / Internet irteera |
| **OPNsense 25.7 (`OPNsense.internal`)** | WAN (`VLAN 30`) / LAN (`VLAN 10`) / DMZ (`VLAN 20`) | `192.168.115.159` (WAN)<br>`192.168.10.1` (LAN)<br>`192.168.20.1` (OPT1) | `192.168.115.254` | WebGUI: `https://192.168.10.1` (`root` / `opnsense`) |
| **Windows Server 2025 (`WIN-U1CN1H8S33S`)** | LAN (`VLAN 10`) | `192.168.10.19/24` | `192.168.10.1` | RDP (`3389`): `WIN-U1CN1H8S33S\Administrador` / `2MG31taldea` |
| **Ubuntu Server 26.04.1 (`1taldea-dmz`)** | DMZ (`VLAN 20`) | `192.168.20.10/24` | `192.168.20.1` | SSH (`22`): `ssh taldea1@192.168.20.10` (`taldea1` / `1Taldea1`) |
| **Cisco SG200-50 (Mahai Gaineko Switch)** | Kudeaketa (`VLAN 1`) + `VLAN 10/20/30` | `192.168.1.254` *(lehenetsia)* | — | WebGUI (`http://192.168.1.254`, `cisco` / `cisco`) |
| **Admin Portatila (`ANDER-SGB5PRO`)** | LAN (`VLAN 10`) | `192.168.10.99/24` *(+ ruta `192.168.20.0/24`)* | `192.168.115.254` *(edo `192.168.10.1`)* | OPNsense + RDP + SSH kudeaketa osoa |
