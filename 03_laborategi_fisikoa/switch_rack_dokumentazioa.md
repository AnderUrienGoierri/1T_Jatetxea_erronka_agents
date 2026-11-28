# Gelako Rack-eko Switch-aren Konfigurazio Dokumentazioa (Cisco SG200-50)

Dokumentu honek **1T Jatetxea Erronkako** laborategi fisikoan gelako rack-eko **Cisco SG200-50** switch-a berrikusi, Trunk portuetako VLAN etiketatzea (`802.1Q`) konpondu eta memorian iraunkorki gordetzeko egindako konfigurazio osoa jasotzen du.

---

## 1. Gailuaren Identifikazioa eta Kudeaketa Sarbidea

| Parametroa                               | Balioa                                                                                            |
| :--------------------------------------- | :------------------------------------------------------------------------------------------------ |
| **Eredua**                               | Cisco SG200-50 (_50-Port Gigabit Smart Switch_, 2014ko firmwarea)                                 |
| **Ostalari-izena (`sysName`)**           | `switchf81e1e`                                                                                    |
| **MAC Helbidea**                         | `ec-1d-8b-f8-1e-1e`                                                                               |
| **Kudeaketa IPa (VLAN 1)**               | `http://192.168.1.254` (`TCP 80 / 443`)                                                           |
| **Kudeaketa Interfazea**                 | WebGUI soilik (`bridg_vlan_Port_To_VLAN_m_jq.htm`, ez du SSH/Telnet CLIrik)                       |
| **Erabiltzailea**                        | `cisco`                                                                                           |
| **Pasahitza**                            | `Pvlbtnse1`                                                                                       |
| **Konfigurazioan erabilitako lanpostua** | `ANDER-SGB5PRO` (`Ethernet 5`, IP: `192.168.1.10/24`, Trunk/VLAN 1 portutik zuzenean konektatuta) |

---

## 2. Diagnostikoa eta Konpondutako Arazoa

Rack-eko switch-ean sartzean egoera hau aurkitu zen:

- **VLANak jada sortuta zeuden:** `VLAN 10 (LAN)`, `VLAN 20 (DMZ)` eta `VLAN 30 (WAN)`.
- **Sarbide-portuak (Untagged) ondo zeuden:** `GE1–GE6 / GE25–GE30` (`10UP`), `GE7–GE12 / GE31–GE36` (`20UP`) eta `GE13–GE18 / GE37–GE42` (`30UP`).
- **Detektatutako arazoa:** **Trunk blokean (`GE19–GE24` eta `GE43–GE50`) `1UP` bakarrik zegoen konfiguratuta**, hau da, VLAN 10, 20 eta 30 **ez zeuden `Tagged` moduan markatuta**. Horren ondorioz, rack-eko switch-ak ezin zituen VLAN 10/20/30 trafikoak pareteko hartunetik mahai gaineko Cisco SG200-50 switch-era pasatu.

**Aplikatutako konponketa:** `VLAN Management > Port to VLAN` atalean, `GE19–GE24` eta `GE43–GE50` portu guztiei `VLAN 10`, `VLAN 20` eta `VLAN 30` **`Tagged`** gehitu zaizkie (`1UP, 10T, 20T, 30T`), eta konfigurazioa `Startup Configuration`-ean gorde da.

---

## 3. Konfiguratutako VLANak (`VLAN Management > VLAN Settings`)

| VLAN ID | VLAN Izena  | Mota (`Originators`) | Azpisarea          | Gateway           | Funtzioa                                                                                                          |
| :------ | :---------- | :------------------- | :----------------- | :---------------- | :---------------------------------------------------------------------------------------------------------------- |
| **1**   | _(Default)_ | `Default`            | `192.168.1.0/24`   | —                 | Switch-aren WebGUI kudeaketa (`192.168.1.254`) eta Trunk Native VLAN                                              |
| **10**  | `LAN`       | `Static`             | `192.168.10.0/24`  | `192.168.10.1`    | Barne sarea: OPNsense `LAN (re1)`, Windows Server 2025 (`192.168.10.19`) eta Trunk bidez mahai gaineko lanpostuak |
| **20**  | `DMZ`       | `Static`             | `192.168.20.0/24`  | `192.168.20.1`    | Zerbitzari publikoen eremu isolatua: OPNsense `OPT1 (re0)` eta Ubuntu Server DMZ (`192.168.20.10`)                |
| **30**  | `WAN`       | `Static`             | `192.168.115.0/24` | `192.168.115.254` | Goierri Eskolako uplink-a eta OPNsense `WAN (re2)` (`192.168.115.159`)                                            |

---

## 4. 50 Portuen Esleipena (`VLAN Management > Port to VLAN`)

Rack-eko switch-ak mahai gaineko switch-aren portu-eskema bera jarraitzen du, laborategiko kudeaketa eta kableatua errazteko:

| Blokea / Eremua                  | Goiko ilara | Beheko ilara | Portu kop. | VLAN Esleipena (`Port VLAN Membership`)                              | Zer konektatu Rack-ean                                                                                                                  |
| :------------------------------- | :---------- | :----------- | :--------: | :------------------------------------------------------------------- | :-------------------------------------------------------------------------------------------------------------------------------------- |
| **1. Blokea (`LAN`)**            | `GE1–GE6`   | `GE25–GE30`  |     12     | `10UP` (VLAN 10 Untagged + PVID; VLAN 1 Excluded)                    | • OPNsense `LAN (re1)` (`192.168.10.1`)<br>• Windows Server 2025 (`192.168.10.19`)                                                      |
| **2. Blokea (`DMZ`)**            | `GE7–GE12`  | `GE31–GE36`  |     12     | `20UP` (VLAN 20 Untagged + PVID; VLAN 1 Excluded)                    | • OPNsense `OPT1 / DMZ (re0)` (`192.168.20.1`)<br>• Ubuntu Server DMZ (`192.168.20.10`)                                                 |
| **3. Blokea (`WAN`)**            | `GE13–GE18` | `GE37–GE42`  |     12     | `30UP` (VLAN 30 Untagged + PVID; VLAN 1 Excluded)                    | • Goierri Eskolako Uplink-a (`192.168.115.254`)<br>• OPNsense `WAN (re2)` (`192.168.115.159`)                                           |
| **4. Blokea (`TRUNK` / `MGMT`)** | `GE19–GE24` | `GE43–GE48`  |     12     | `1UP, 10T, 20T, 30T` (VLAN 1 Untagged/PVID + VLAN 10, 20, 30 Tagged) | • **802.1Q Trunk** pareteko kableatu egituratura ↔ Mahai gaineko Switch (`GE19–24` / `GE43–48`)<br>• WebGUI kudeaketa (`192.168.1.254`) |
| **SFP Combo Portuak**            | `GE49–GE50` | —            |     2      | `1UP, 10T, 20T, 30T`                                                 | Trunk erreserba / zuntz-lotura                                                                                                          |

---

## 5. Kableatu Fisikoaren Konexio Gida (Rack-a ↔ Mahaia)

1. **Goierri Eskola (WAN Uplink) → Rack Switch:** konektatu **`WAN`** blokean (`GE13–GE18` edo `GE37–GE42`).
2. **OPNsense `WAN (re2)` → Rack Switch:** konektatu **`WAN`** blokean (`GE13–GE18` edo `GE37–GE42`).
3. **OPNsense `LAN (re1)` eta Windows Server 2025 → Rack Switch:** konektatu **`LAN`** blokean (`GE1–GE6` edo `GE25–GE30`).
4. **OPNsense `OPT1 (re0)` eta Ubuntu Server DMZ → Rack Switch:** konektatu **`DMZ`** blokean (`GE7–GE12` edo `GE31–GE36`).
5. **Rack Switch ↔ Mahai Gaineko Switch (pareteko hartunearen bidez):** bi switch-etan **`TRUNK`** blokean konektatu (`GE19–GE24` edo `GE43–GE48`).

---

## 6. Konfigurazioa Memorian Gordetzea eta Egiaztapena

Aldaketak aplikatu ondoren, bi egiaztapen egin dira:

1. **`VLAN Management > Port VLAN Membership`:**
   - `GE1–GE6`, `GE25–GE30` → `10UP` (Administrative & Operational VLANs)
   - `GE7–GE12`, `GE31–GE36` → `20UP`
   - `GE13–GE18`, `GE37–GE42` → `30UP`
   - `GE19–GE24`, `GE43–GE50` → `1UP, 10T, 20T, 30T`
2. **`Administration > File Management > Copy/Save Configuration`:**
   - **Source File Name:** `Running configuration`
   - **Destination File Name:** `Startup configuration`
   - **`Apply`** sakatuta eta egiaztatuta goiko barrako **`Save`** ikono gorria desagertu dela (konfigurazioa flash memorian iraunkorki gordeta).

---

## 7. Lotutako Dokumentazioa

- Mahai gaineko switch-aren dokumentazioa: [switch_mahaia_dokumentazioa.md](file:///c:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/03_laborategi_fisikoa/switch_mahaia_dokumentazioa.md)
- Sare fisikoaren topologia eta helbideratzea: [sare_fisikoaren_azterketa.md](file:///c:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/03_laborategi_fisikoa/sare_fisikoaren_azterketa.md)
- Kableatua eta rack antolaketa: [rack_eta_kableatu_egituratua.md](file:///c:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/03_laborategi_fisikoa/03_kableatu_eta_rack/rack_eta_kableatu_egituratua.md)
