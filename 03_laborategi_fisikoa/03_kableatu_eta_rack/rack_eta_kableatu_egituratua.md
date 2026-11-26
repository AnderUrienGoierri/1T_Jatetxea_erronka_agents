# Rack Diseinua, Switch-ak eta Kableatu Egituratua - Goierri Eskola Laborategia

Laborategian benetan muntatuta dagoen kableatuaren, bi switch-en (mahai gainekoa eta rack-ekoa), pareteko hartunearen eta zerbitzarien arteko lotura fisikoen dokumentazio zehatza.

---

## 1. Kableatu Fisikoaren Katea (End-to-End)

```text
[ Goierri Eskola: SW Goierri (Fortinet 192.168.115.254) ]
                        |
         (Uplink Kablea -> VLAN 30 / WAN)
                        v
+---------------------------------------------------------------+
| GELAKO RACK-A (2. Switch-a: SW + Zerbitzariak + OPNsense)     |
|                                                               |
|  [ Rack Switch (SW) ]                                         |
|    ├── VLAN 30 (WAN) ───> OPNsense WAN (re2: 192.168.115.159) |
|    ├── VLAN 10 (LAN) ───> OPNsense LAN (re1: 192.168.10.1)    |
|    ├── VLAN 10 (LAN) ───> Windows Server 2025 (192.168.10.19) |
|    ├── VLAN 20 (DMZ) ───> OPNsense OPT1 (re0: 192.168.20.1)   |
|    ├── VLAN 20 (DMZ) ───> Ubuntu Server DMZ (192.168.20.10)   |
|    └── Gelako Kableatu Egituratua (Pareteko Hartunera)        |
+---------------------------------------------------------------+
                        |
          (Pareteko Ethernet Hartunea - Hembra)
                        |
              (Kable Beltza Cat 6)
                        v
+---------------------------------------------------------------+
| MAHAI GAINEKO SWITCH-A (1. Switch-a: Cisco SG200-50)          |
|   ├── LAN  (1–6 / 25–30)  ──> Portu 1 (Kable Urdina): Admin   |
|   │                           Portuak 2–6: Taldekideak        |
|   ├── DMZ  (7–12 / 31–36) ──> VLAN 20 (erreserban)            |
|   └── WAN  (13–20 / 37–43)──> Portu 18/42 (Kable Beltza):     |
|                               Pareteko Ethernet Hartunea      |
+---------------------------------------------------------------+
```

---

## 2. Erabilitako Kableak eta Portuen Esleipena

| Lotura / Kablea                     | Jatorria                                    | Helburua                           | Kable Mota / Kolorea             | VLAN / Sarea                |
| :---------------------------------- | :------------------------------------------ | :--------------------------------- | :------------------------------- | :-------------------------- |
| **Admin Portatila -> Mahaiko SW**   | `ANDER-SGB5PRO` (`Ethernet 5`, USB-GbE)     | Cisco SG200-50 **Portu 1 (`LAN`)** | Cat 5e/6 **Urdina**              | VLAN 10 (`LAN`)             |
| **Mahaiko SW -> Pareteko Hartunea** | Cisco SG200-50 **WAN taldea (Portu 18/42)** | Pareteko Ethernet Hembra           | Cat 6 **Beltza** (`KabelDirekt`) | Uplink / Gelako sarea       |
| **Pareteko Hartunea -> Rack SW**    | Pareteko kableatu egituratua                | Gelako Rack-eko Switch-a (`SW`)    | Kableatu egituratua              | Gelako lotura               |
| **Eskola -> Rack SW**               | Goierri Eskola (`SW Goierri`)               | Rack Switch (`VLAN 30` taldea)     | Ethernet Uplink                  | VLAN 30 (`WAN`)             |
| **Rack SW -> OPNsense WAN**         | Rack Switch (`VLAN 30` taldea)              | OPNsense `re2` (`WAN`)             | Ethernet                         | VLAN 30 (`192.168.115.159`) |
| **Rack SW -> OPNsense LAN**         | Rack Switch (`VLAN 10` taldea)              | OPNsense `re1` (`LAN`)             | Ethernet                         | VLAN 10 (`192.168.10.1`)    |
| **Rack SW -> OPNsense OPT1**        | Rack Switch (`VLAN 20` taldea)              | OPNsense `re0` (`OPT1/DMZ`)        | Ethernet                         | VLAN 20 (`192.168.20.1`)    |
| **Rack SW -> Windows Server**       | Rack Switch (`VLAN 10` taldea)              | `WIN-U1CN1H8S33S` (`Ethernet`)     | Ethernet                         | VLAN 10 (`192.168.10.19`)   |
| **Rack SW -> Ubuntu Server**        | Rack Switch (`VLAN 20` taldea)              | `1taldea-dmz` (`enp1s0`)           | Ethernet                         | VLAN 20 (`192.168.20.10`)   |

---

## 3. Arbeleko Kolore-Kodea eta Switch Etiketak (`irudiak/sareak.png`)

Laborategian erabilitako benetako VLAN eta kolore-banaketa:

| Eremua    | VLAN ID   | Azpisarea          | Etiketa (Cisco SG200-50)  | Funtzioa                                                |
| :-------- | :-------- | :----------------- | :------------------------ | :------------------------------------------------------ |
| **LAN**   | `VLAN 10` | `192.168.10.0/24`  | `LAN` (ezkerreko portuak) | Windows Server 2025, OPNsense WebGUI, Admin/Taldekideak |
| **DMZ**   | `VLAN 20` | `192.168.20.0/24`  | `DMZ` (erdiko portuak)    | Ubuntu Server (`1taldea-dmz`) web zerbitzari isolatua   |
| **WAN**   | `VLAN 30` | `192.168.115.0/24` | `WAN` (eskuineko portuak) | Goierri Eskolako sare nagusia eta Interneterako irteera |
| **Trunk** | `802.1Q`  | —                  | Rack Switch Trunk portua  | Switch arteko VLAN trafiko etiketatua                   |
