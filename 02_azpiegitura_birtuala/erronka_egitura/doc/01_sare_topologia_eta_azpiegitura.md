# Erronka 1 - Jatetxea Sare Sistema eta Aplikazioen Hedapena
## 01. Sare Topologia eta Azpiegitura Plana

---

### 1. Testuinguru Grafikoa eta Diseinuaren Analisia

Proiektuaren sare-azpiegitura bi erreferentzia nagusitan oinarritzen da:
1. **Arbelaren zirriborroa (`img/eskema_zirriborroa.jpg`):**
   - Nukleo zentrala: OPNsense suebakia (`F.W Segurtasun`).
   - Hiru eremu nagusi:
     - **WAN / Internet (`I`):** Kanpoko sarea eta bezeroen gailuak.
     - **DMZ:** Ubuntu 26.04 zerbitzaria, non Apache 2.4 web zerbitzaria eta C# ASP.NET Core REST APIa kokatzen diren.
     - **LAN:** Windows Server 2025 zerbitzaria (DHCP, DNS, IIS eta Active Directory kudeatuz) eta langileen lanpostuak.
2. **Azpiegitura diagrama fisikoa (`img/proiektu_testuingurua.png`):**
   - **Router-on-a-stick arkitektura:** OPNsense eta kommutadorearen (Switch) artean kable fisiko bakarra erabiltzen da.
   - **Portu Tronkala (Trunk):** Switch-aren aldean 802.1Q portu tronkala konfiguratzen da; OPNsense-ren aldean subinterfaze birtualak (VLAN 10 eta VLAN 20).
   - **Kommutadoreen arteko Trunk-a:** Switch 1 eta Switch 2 arteko lotura tronkala trafiko guztia hedatzeko.

---

### 2. Sare Segmentazioa eta VLAN Egitura

Sarearen segurtasuna bermatzeko, trafikoa hiru broadcast domeinu bereizietan banatzen da:

```
+-----------------------------------------------------------------------------------+
|                                 WAN (Internet)                                    |
|                   DHCP / Publikoa - OPNsense em0 (edo em1)                        |
+-----------------------------------------------------------------------------------+
                                         |
                                         |
                        +----------------------------------+
                        |         OPNsense Suebakia        |
                        |      Router-on-a-stick (Trunk)   |
                        +----------------------------------+
                                         | (802.1Q Trunk: VLAN 10, VLAN 20)
                        +----------------------------------+
                        |        Kudeatutako Switch-a      |
                        +----------------------------------+
                               /                    \
     (Access VLAN 10)         /                      \ (Access VLAN 20)
                             /                        \
+--------------------------------------+    +--------------------------------------+
|       LAN Gunea (VLAN 10)            |    |       DMZ Gunea (VLAN 20)            |
|       Sare ID: 192.168.10.0/24       |    |       Sare ID: 192.168.20.0/24       |
|                                      |    |                                      |
| - Gateway: 192.168.10.1 (OPNsense)   |    | - Gateway: 192.168.20.1 (OPNsense)   |
| - WinServer: 192.168.10.10           |    | - Ubuntu Server: 192.168.20.10       |
|   (AD DS, DNS, DHCP, IIS)            |    |   (Apache, ASP.NET API, MySQL)       |
| - Bezeroak (TPV, KDS, BDS, Mugikorrak|    |                                      |
|   DHCP bidez: 192.168.10.100-200     |    |                                      |
+--------------------------------------+    +--------------------------------------+
```

---

### 3. IP Helbideratze Plana (Addressing Scheme)

| Sarea / Gunea | VLAN ID | Sare Helbidea | Sare-Maskara | Gateway (OPNsense) | Broadcast | Funtzio Nagusia |
| :--- | :---: | :--- | :--- | :--- | :--- | :--- |
| **WAN** | - | *ISP / NAT sarearen arabera* | 255.255.255.0 | *ISP Gateway* | - | Interneteko irteera eta kanpoko bezeroen sarrera |
| **LAN** | `10` | `192.168.10.0` | `255.255.255.0` (`/24`) | `192.168.10.1` | `192.168.10.255` | Barne kudeaketa, Active Directory eta langileen gailuak |
| **DMZ** | `20` | `192.168.20.0` | `255.255.255.0` (`/24`) | `192.168.20.1` | `192.168.20.255` | Zerbitzu publiko eta erdi-publikoak (Web, API, DB) |

#### Ekipoen eta Zerbitzarien IP Banaketa Zehatza:

| Ekipoa | Gunea | IP Helbidea | Mota | DNS Zerbitzaria | Deskribapena |
| :--- | :--- | :--- | :---: | :--- | :--- |
| **OPNsense (WAN)** | WAN | Dinamikoa / NAT | DHCP | 1.1.1.1, 8.8.8.8 | Interneteko sarbidea |
| **OPNsense (LAN)** | LAN | `192.168.10.1` | Estatikoa | - | LANeko lehenetsitako atebidea (Gateway) |
| **OPNsense (DMZ)** | DMZ | `192.168.20.1` | Estatikoa | - | DMZko lehenetsitako atebidea (Gateway) |
| **Erronka-WinServer2025** | LAN | `192.168.10.10` | Estatikoa | `127.0.0.1` | AD DS (`jatetxea.local`), DNS eta DHCP zerbitzaria |
| **Erronka-Ubuntu** | DMZ | `192.168.20.10` | Estatikoa | `192.168.10.10` / `1.1.1.1` | Apache (Web), ASP.NET Core (API) eta MySQL DB |
| **TPV 1 (C# WinForms)** | LAN | `192.168.10.101` | DHCP | `192.168.10.10` | Salmenta Puntuko Terminala (Zerbitzariak) |
| **Sukaldea KDS (JavaFX)** | LAN | `192.168.10.102` | DHCP | `192.168.10.10` | Sukaldeko eskaera-pantaila (KDS) |
| **Barra BDS (JavaFX)** | LAN | `192.168.10.103` | DHCP | `192.168.10.10` | Barrako edari-pantaila (BDS) |
| **Mugikorrak (Flutter)** | LAN/Wi-Fi | `192.168.10.104+` | DHCP | `192.168.10.10` | Zerbitzarien PDA mugikorrak |

---

### 4. VirtualBox Ingurune Birtualeko Ezarpena

VirtualBox-en azpiegitura hau modu fidagarrian gauzatzeko bi aukera tekniko daude:

#### 1. Aukera (Gomendatua Laborategirako): Barne Sare Bereiziak (Internal Networks)
VirtualBox-ek barne kommutadore birtual gisa jokatzen du `intnet` aukerarekin:
* **OPNsense:**
  * Sare-txartela 1: `NAT` (WAN sarbidea eta Host administrazioa port forwarding bidez: 2231, 8443).
  * Sare-txartela 2: `Barne sarea (Internal Network)` -> Izena: `intnet-lan` (LAN sarerako).
  * Sare-txartela 3: `Barne sarea (Internal Network)` -> Izena: `intnet-dmz` (DMZ sarerako).
* **Erronka-WinServer2025:**
  * Sare-txartela 1: `Barne sarea (Internal Network)` -> Izena: `intnet-lan`.
  * Sare-txartela 2 (Aukerakoa administraziorako): `NAT` (RDP 3391 eta SSH 2232 portuak Host-etik sartzeko).
* **Erronka-Ubuntu:**
  * Sare-txartela 1: `Barne sarea (Internal Network)` -> Izena: `intnet-dmz`.
  * Sare-txartela 2 (Aukerakoa administraziorako): `NAT` (SSH 2230 portua Host-etik sartzeko).

#### 2. Aukera (Eskema Fisikoaren Kopia Zuzena): 802.1Q VLAN Trunk bidez
* OPNsense-n Sare-txartela 2 `intnet-trunk` moduan jartzen da modu promiskuoan (`Modo promiscuo: Permitir todo`).
* OPNsense-n `vlan10` eta `vlan20` subinterfazeak sortzen dira txartel horren gainean.
* Windows Server eta Ubuntu makinetan VLAN ID bidezko etiketazioa egiten da sare-txarteletan.

Laborategi honetarako, **1. Aukera** da egonkorrena eta konfiguratzeko garbienez funtzionatzen duena, suebakiaren bideratze eta segurtasun erregelak modu berean aplikatzen baitira txartel fisiko/birtual bakoitzean.
