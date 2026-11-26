# Cisco Packet Tracer - 1T Jatetxea Sare Fisikoaren Simulazio Gida

Gida honek gure laborategi fisiko errealeko topologia osoa (**Goierri Eskola -> Rack Switch -> OPNsense 3 NIC + WinServer 2025 + Ubuntu DMZ -> Mahai Gaineko Switch -> Portatilak**) Cisco Packet Tracer-en 5 minututan muntatzeko urrats zehatzak eta CLI scriptak jasotzen ditu.

---

## 0. Zuzenean Sortutako Proiektua (.PKT)

> [!NOTE]
> Proiektua dagoeneko **zuzenean sortuta eta enkriptatuta** dago `repacket` tresnaren bidez!
> Fitxategi hau Cisco Packet Tracer-en zuzenean ireki dezakezu:
> - **PKT Fitxategia:** [topologia_orokorra.pkt](file:///c:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/03_laborategi_fisikoa/02_cisco_packet_tracer/topologia_orokorra.pkt)
> - **XML Iturria:** [topologia_orokorra.xml](file:///c:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/03_laborategi_fisikoa/02_cisco_packet_tracer/topologia_orokorra.xml)
> - **Eraikitzeko Scripta:** [build_jatetxea_pkt.py](file:///c:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/03_laborategi_fisikoa/02_cisco_packet_tracer/build_jatetxea_pkt.py)

---

## 1. Behar Diren Gailuak (Packet Tracer-en jarri)

| Gailu Izena | Packet Tracer Eredua | IP Helbidea / Maskara | Gateway | Funtzioa |
| :--- | :--- | :--- | :--- | :--- |
| **`Internet-DNS`** | `Server-PT` | `8.8.8.8 / 24` | `8.8.8.1` | Internet proba (`ping 8.8.8.8`) |
| **`GOIERRI-GW`** | `Router 2911` | `Gig0/0`: `192.168.115.254/24`<br>`Gig0/1`: `8.8.8.1/24` | — | Goierri Eskolako Fortinet Gateway + DHCP |
| **`OPNSENSE-FW`** | `Router 2911` *(3 Gig portuak)* | `Gig0/0` (WAN): `192.168.115.159/24`<br>`Gig0/1` (LAN): `192.168.10.1/24`<br>`Gig0/2` (DMZ): `192.168.20.1/24` | `192.168.115.254` | OPNsense Suebakia + NAT + ACL (`3306` salbuespena) |
| **`SW-RACK`** | `Switch 2960-24TT` | VLAN 10, 20, 30 | — | Gelako Rack-eko Switch nagusia |
| **`SW-MAHAIA`** | `Switch 2960-24TT` | VLAN 10, 20, 30 | — | Mahai gaineko Cisco SG200-50 switch-a |
| **`WinServer2025`** | `Server-PT` | `192.168.10.19 / 24` | `192.168.10.1` | MySQL DB (`3306`) + Desktop TPV/KDS |
| **`Ubuntu-DMZ`** | `Server-PT` | `192.168.20.10 / 24` | `192.168.20.1` | Web Publikoa (`HTTP 80`) + REST API (`5050`) |
| **`Admin-Portatila`** | `Laptop-PT` | `192.168.10.99 / 24` | `192.168.10.1` | `ANDER-SGB5PRO` admin lanpostua (LAN) |
| **`Taldekide-1`** | `Laptop-PT` | `192.168.10.101 / 24` | `192.168.10.1` | Taldekidearen portatila (LAN) |
| **`WAN-Bezeroa`** | `PC-PT` | `DHCP` (`192.168.115.x/24`) | `192.168.115.254` | Eskolako sareko bezeroa (`http://192.168.115.159` probatzeko) |

---

## 2. Kableatu Taula (Copper Straight-Through / Crossover)

| Jatorrizko Gailua eta Portua | Kable Mota | Helburuko Gailua eta Portua | VLAN / Sarea |
| :--- | :--- | :--- | :--- |
| `Internet-DNS` (`Fa0`) | Crossover | `GOIERRI-GW` (`Gig0/1`) | Internet (`8.8.8.0/24`) |
| `GOIERRI-GW` (`Gig0/0`) | Straight-Through | `SW-RACK` (`Fa0/20`) | **VLAN 30 (WAN)** |
| `OPNSENSE-FW` (`Gig0/0` - WAN) | Straight-Through | `SW-RACK` (`Fa0/13`) | **VLAN 30 (WAN)** |
| `OPNSENSE-FW` (`Gig0/1` - LAN) | Straight-Through | `SW-RACK` (`Fa0/1`) | **VLAN 10 (LAN)** |
| `OPNSENSE-FW` (`Gig0/2` - DMZ) | Straight-Through | `SW-RACK` (`Fa0/7`) | **VLAN 20 (DMZ)** |
| `WinServer2025` (`Fa0`) | Straight-Through | `SW-RACK` (`Fa0/2`) | **VLAN 10 (LAN)** |
| `Ubuntu-DMZ` (`Fa0`) | Straight-Through | `SW-RACK` (`Fa0/8`) | **VLAN 20 (DMZ)** |
| `SW-RACK` (`Gig0/1`) | Crossover | `SW-MAHAIA` (`Gig0/1`) | **Trunk (VLAN 10,20,30)** |
| `Admin-Portatila` (`Fa0`) | Straight-Through | `SW-MAHAIA` (`Fa0/1`) | **VLAN 10 (LAN)** |
| `Taldekide-1` (`Fa0`) | Straight-Through | `SW-MAHAIA` (`Fa0/2`) | **VLAN 10 (LAN)** |
| `WAN-Bezeroa` (`Fa0`) | Straight-Through | `SW-MAHAIA` (`Fa0/13`) | **VLAN 30 (WAN)** |

---

## 3. CLI Konfigurazio Scriptak (Kopiatu eta Itsatsi)

Gailu bakoitzaren **CLI** fitxan sartu (`no` erantzun hasierako morroian) eta itsatsi dagokion fitxategia:
1. `GOIERRI-GW` -> [01_goierri_router.cfg](konfigurazio_scriptak_ios/01_goierri_router.cfg)
2. `OPNSENSE-FW` -> [02_opnsense_router_2911.cfg](konfigurazio_scriptak_ios/02_opnsense_router_2911.cfg)
3. `SW-RACK` -> [03_sw_rack_2960.cfg](konfigurazio_scriptak_ios/03_sw_rack_2960.cfg)
4. `SW-MAHAIA` -> [04_sw_mahaia_2960.cfg](konfigurazio_scriptak_ios/04_sw_mahaia_2960.cfg)

---

## 4. Egiaztapen Probak Packet Tracer-en

1. **`Admin-Portatila` (`192.168.10.99`) -> `ping 192.168.10.19`**, **`ping 192.168.20.10`** eta **`ping 8.8.8.8`**: Guztiek erantzun behar dute.
2. **`Ubuntu-DMZ` (`192.168.20.10`) -> `ping 8.8.8.8`**: Erantzun behar du (Internet baimenduta).
3. **`Ubuntu-DMZ` (`192.168.20.10`) -> `ping 192.168.10.19`**: **Destination host unreachable / Blokeatuta** (ACL-ak DMZ -> LAN blokeatzen du, `3306` izan ezik!).
4. **`WAN-Bezeroa` (`192.168.115.x`) -> Web Browser `http://192.168.115.159`**: `Ubuntu-DMZ` webgunea ireki behar da NAT Port Forwarding bidez!
5. Gorde proiektua hemen: `02_cisco_packet_tracer/topologia_orokorra.pkt`.
