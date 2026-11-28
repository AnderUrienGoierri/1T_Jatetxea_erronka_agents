# Mahai Gaineko Switch-aren Konfigurazio Dokumentazioa (Cisco SG200-50)

Dokumentu honek **1T Jatetxea Erronkako** laborategi fisikoan mahai gaineko **Cisco SG200-50** switch-a berrezarri, VLAN bidez segmentatu eta memorian iraunkorki gordetzeko egindako konfigurazio osoa jasotzen du.

---

## 1. Gailuaren Identifikazioa eta Kudeaketa Sarbidea

| Parametroa                               | Balioa                                                                                             |
| :--------------------------------------- | :------------------------------------------------------------------------------------------------- |
| **Eredua**                               | Cisco SG200-50 (_50-Port Gigabit Smart Switch_)                                                    |
| **Ostalari-izena (`sysName`)**           | `switch0249ea`                                                                                     |
| **MAC Helbidea**                         | `bc-f1-f2-02-49-ea`                                                                                |
| **Kudeaketa IPa (VLAN 1)**               | `http://192.168.1.254` (`TCP 80 / 443`)                                                            |
| **Kudeaketa Interfazea**                 | WebGUI soilik (ez du SSH/Telnet CLIrik)                                                            |
| **Erabiltzailea**                        | `cisco`                                                                                            |
| **Pasahitza**                            | `Pvlbtnse1`                                                                                        |
| **Konfigurazioan erabilitako lanpostua** | `ANDER-SGB5PRO` (`Ethernet 5`, IP: `192.168.1.10/24`, **Portu `GE20`** bidez zuzenean konektatuta) |

---

## 2. Gertaera eta Berrezarpenaren Arrazoia

Switch-a nahi gabe berrabiaraztean (_reboot_), aurreko konfigurazioa `Startup Configuration`-ean gorde gabe zegoenez, switch-a fabrikako VLAN egoerara itzuli zen (`VLAN 1` soilik, portu guztiak `1UP` moduan).

Konfigurazioa berriz hutsetik ezarri eta oraingoan **Running Configuration → Startup Configuration** memorian iraunkorki gorde da, etorkizunean argindarra joan edo berrabiarazten bada ere konfigurazioa ez galtzeko.

---

## 3. Sortutako VLANak (`VLAN Management > VLAN Settings`)

| VLAN ID | VLAN Izena  | Mota (`Originators`) | Azpisarea          | Gateway           | Funtzioa                                                                     |
| :------ | :---------- | :------------------- | :----------------- | :---------------- | :--------------------------------------------------------------------------- |
| **1**   | _(Default)_ | `Default`            | `192.168.1.0/24`   | —                 | Switch-aren WebGUI kudeaketa (`192.168.1.254`) eta Trunk Native VLAN         |
| **10**  | `LAN`       | `Static`             | `192.168.10.0/24`  | `192.168.10.1`    | Barne sarea: Admin/taldekideen portatilak, Windows Server 2025, OPNsense LAN |
| **20**  | `DMZ`       | `Static`             | `192.168.20.0/24`  | `192.168.20.1`    | Zerbitzari publikoen eremu isolatua (Ubuntu Server `1taldea-dmz`)            |
| **30**  | `WAN`       | `Static`             | `192.168.115.0/24` | `192.168.115.254` | Goierri Eskolako sare nagusia eta Interneterako irteera                      |

---

## 4. 50 Portuen Esleipena (`VLAN Management > Port to VLAN`)

Switch-aren aurrealdeko etiketen (`LAN`, `DMZ`, `WAN`, `1 TALDEA`) eta laborategiko topologiaren arabera, 50 portuak honela banatu dira:

| Blokea / Etiketa                 | Goiko ilara | Beheko ilara | Portu kop. | VLAN Esleipena (`Port VLAN Membership`)                              | Deskribapena                                                                              |
| :------------------------------- | :---------- | :----------- | :--------: | :------------------------------------------------------------------- | :---------------------------------------------------------------------------------------- |
| **1. Blokea (`LAN`)**            | `GE1–GE6`   | `GE25–GE30`  |     12     | `10UP` (VLAN 10 Untagged + PVID; VLAN 1 Excluded)                    | Admin eta taldekideen portatilak (`192.168.10.x`)                                         |
| **2. Blokea (`DMZ`)**            | `GE7–GE12`  | `GE31–GE36`  |     12     | `20UP` (VLAN 20 Untagged + PVID; VLAN 1 Excluded)                    | DMZ ekipoak (`192.168.20.x`)                                                              |
| **3. Blokea (`WAN`)**            | `GE13–GE18` | `GE37–GE42`  |     12     | `30UP` (VLAN 30 Untagged + PVID; VLAN 1 Excluded)                    | Goierri Eskola / WAN (`192.168.115.x`)                                                    |
| **4. Blokea (`TRUNK` / `MGMT`)** | `GE19–GE24` | `GE43–GE48`  |     12     | `1UP, 10T, 20T, 30T` (VLAN 1 Untagged/PVID + VLAN 10, 20, 30 Tagged) | **802.1Q Trunk** (pareteko hartunea → Rack Switch) eta WebGUI kudeaketa (`192.168.1.254`) |
| **SFP Combo Portuak**            | `GE49–GE50` | —            |     2      | `1UP, 10T, 20T, 30T`                                                 | Trunk erreserba / zuntz-lotura                                                            |

> **Oharra (`Portu 19–24 / 43–48`):** Bloke honetan `VLAN 1` _Untagged (PVID 1)_ mantentzeak bi abantaila ditu:
>
> 1. Pareteko hartunetik rack-eko switch-era **VLAN 10, 20 eta 30** trafiko guztia etiketatuta (_Tagged_, 802.1Q Trunk) pasatzen da kable bakarretik.
> 2. Portatila bloke honetako portu libre batera (adib. `GE20`) konektatuz, edozein unetan sar daiteke berriz switch-aren WebGUI-ra (`http://192.168.1.254`), konexioa galdu gabe.

---

## 5. Kableatu Fisikoaren Konexio Gida

Switch-a martxan jartzeko kableak honela konektatu behar dira:

1. **Pareteko Ethernet hartunea (Rack-eko Switch-era doan kable beltza):** konektatu **Trunk** blokean (`GE19–GE24` edo `GE43–GE48`, adib. `GE19` edo `GE20`).
2. **Admin portatila (`ANDER-SGB5PRO`, `192.168.10.99`) eta taldekideen portatilak (`192.168.10.101–.109`):** konektatu **`LAN`** blokean (`GE1–GE6` edo `GE25–GE30`, adib. `GE1`).
3. **DMZ ekipoak (behar izanez gero mahaitik zuzenean):** konektatu **`DMZ`** blokean (`GE7–GE12` edo `GE31–GE36`).
4. **WAN / Eskolako sare zuzena (behar izanez gero):** konektatu **`WAN`** blokean (`GE13–GE18` edo `GE37–GE42`).

---

## 6. Konfigurazioa Memorian Gordetzea eta Egiaztapena

Aldaketa guztiak aplikatu ondoren, bi egiaztapen egin dira:

1. **`VLAN Management > Port VLAN Membership`:**
   - `GE1–GE6`, `GE25–GE30` → `10UP` (Administrative & Operational VLANs)
   - `GE7–GE12`, `GE31–GE36` → `20UP`
   - `GE13–GE18`, `GE37–GE42` → `30UP`
   - `GE19–GE24`, `GE43–GE50` → `1UP, 10T, 20T, 30T`
2. **`Administration > File Management > Copy/Save Configuration`:**
   - **Source File Name:** `Running configuration`
   - **Destination File Name:** `Startup configuration`
   - **`Apply`** sakatuta eta egiaztatuta goiko barrako **`Save`** ikono gorri kliskaria desagertu dela (konfigurazioa flash memorian gordeta).

---

## 7. Eskuz Berrezartzeko Laburpena (Erreferentzia)

Etorkizunean mahai gaineko edo rack-eko **Cisco SG200-50** bat berriz konfiguratu behar bada:

1. Konektatu portatila zuzenean switch-era (`192.168.1.10/24` IParekin `Ethernet 5`-en) eta ireki `http://192.168.1.254` (`cisco` / `Pvlbtnse1`).
2. Joan `VLAN Management > VLAN Settings` atalera eta sortu:
   - `10` → `LAN`
   - `20` → `DMZ`
   - `30` → `WAN`
3. Joan `VLAN Management > Port to VLAN` atalera:
   - **VLAN 10:** `GE1–6, GE25–30` = `Untagged (PVID)`, `GE19–24, GE43–50` = `Tagged` → **Apply**.
   - **VLAN 20:** `GE7–12, GE31–36` = `Untagged (PVID)`, `GE19–24, GE43–50` = `Tagged` → **Apply**.
   - **VLAN 30:** `GE13–18, GE37–42` = `Untagged (PVID)`, `GE19–24, GE43–50` = `Tagged` → **Apply**.
   - **VLAN 1:** egiaztatu `GE19–24, GE43–50` bakarrik daudela `Untagged (PVID)`.
4. Egin klik goiko **`Save`** botoian (`Running configuration → Startup configuration` → **Apply**) aldaketak iraunkor egiteko.
