# 01. Sare Arkitektura eta 3 Makina Birtualen Eredua

Dokumentu honek erronkan zehaztutako 3 makina birtualen eta aplikazioen banaketa arkitektonikoa azaltzen du, zibersegurtasuneko eta sistema-ingeniaritzako irizpide profesionalak jarraituz.

---

## 1. Helburua eta Testuingurua

Erronkaren helburua jatetxe baten informatika eta sare sistema osoa simulatzea da:
1. Kanpoko bezeroek Internetetik jatetxearen webgunera sartu eta platerak ikusi edo mahaiak erreserbatu ahal izatea.
2. Barneko langileek (zerbitzariak, sukaldariak, barrakoak, administratzaileak) beren aplikazio espezifikoak erabiltzea (TPV, tabletak, sukaldeko KDS pantaila, Odoo ERP kudeaketa).
3. Segurtasuna bermatzea: kanpoko webgunea DMZ gune batean kokatzea eta barneko datu-basea, APIa eta kudeaketa-tresnak sare seguru batean babestea.

---

## 2. Hiru Makina Birtualen Banaketa

```
           +-------------------------------------------------------+
           |                     WAN / INTERNET                    |
           |             (Host / Garatzailearen Sarea)             |
           +-------------------------------------------------------+
                                      |
                                      | (em0: NAT 10.0.2.15)
           +-------------------------------------------------------+
           |             1. MAKINA: OPNsense FIREWALL              |
           |           Bideratzea, NAT eta Suhesi Arauak           |
           +-------------------------------------------------------+
                    /                                   \
                   / (em2: 192.168.20.1)                 \ (em1: 192.168.10.1)
                  /                                       \
  +--------------------------------+     +-----------------------------------+
  |   2. MAKINA: UBUNTU SERVER     |     |   3. MAKINA: WINDOWS SERVER 2025  |
  |         (DMZ SAREA)            |     |            (LAN SAREA)            |
  |       IP: 192.168.20.10        |     |         IP: 192.168.10.10         |
  +--------------------------------+     +-----------------------------------+
  | - Apache 2.4 / PHP 8.2         |     | - MySQL 8.0/8.4 (jatetxea_db)     |
  | - Web Bezero Publikoa (web_php)|     | - ASP.NET Core REST API (.NET 10) |
  | - Ataka: 80                    |     | - TPV C# Desktop aplikazioa       |
  |                                |     | - Odoo 17/18 ERP (HR & Baimenak)  |
  |                                |     | - Jatetxearen Barne Txat Zerb.    |
  |                                |     | - Sukaldeko Android TV Zerbitzua  |
  |                                |     | - Active Directory, DNS eta DHCP  |
  +--------------------------------+     +-----------------------------------+
```

### 1. Makina: Erronka-OPNsense (Firewall & Router)
* **Eginkizuna:** Sarearen kontrolgune nagusia, trafikoaren banaketa eta segurtasun-perimetroa.
* **Interfazeak:**
  * `em0` (WAN): Host-arekin eta kanpoko sarearekin komunikazioa.
  * `em1` (LAN - `192.168.10.1/24`): Barneko zerbitzari segurua eta kudeaketa.
  * `em2` (DMZ - `192.168.20.1/24`): Eremu desmilitarizatua, web zerbitzari publikoarentzat.
* **Segurtasun-Politikak:**
  * WAN -> DMZ: 80 portua baimenduta (Webgunera sartzeko).
  * DMZ -> LAN: **Zero-Trust blokeo osoa**. DMZko webguneak LANeko APIko 5000 portura soilik bidali ditzake eskaerak. Datu-baseko (3306), SMB (445), RDP (3389) edo gainerako zerbitzuetara sarbidea erabat debekatuta dago.
  * LAN -> DMZ: Administratzaile eta zerbitzariek DMZrako sarbide osoa dute mantenu lanetarako.

### 2. Makina: Erronka-Ubuntu (DMZ Web Zerbitzaria)
* **IP Helbidea:** `192.168.20.10`
* **Eginkizuna:** Jatetxearen webgune publikoa (`web_php`) ostatu ematea.
* **Teknologia:** Apache 2.4 eta PHP 8.2.
* **Komunikazio Bidea:** Webgune honek ez du datu-baserik lokalean; bezero batek erreserba bat egiten duenean, PHPak HTTP deia egiten dio Windows Server-eko APIari (`http://192.168.10.10:5000/api/v1`).

### 3. Makina: Erronka-WinServer2025 (LAN Zerbitzari Integratua)
* **IP Helbidea:** `192.168.10.10`
* **Eginkizuna:** Jatetxearen datu, logika eta kudeaketa-aplikazio guztiak zentralizatzea:
  * **MySQL Zerbitzaria:** `jatetxea_db` datu-basea 11 taulekin, InnoDB transakzio pesimistekin.
  * **ASP.NET Core REST API:** C# bidez garatutako negozio-logika nagusia.
  * **TPV C# Desktop:** Zerbitzariek eta barrakoek erabiltzeko Windows Forms mahaigaineko programa.
  * **Odoo ERP:** Langileen alta/bajak, txandak, fitxaketak eta baimenak administratzeko.
  * **Barne Txat Zerbitzaria:** Sukaldea, barra eta zerbitzarien arteko denbora errealeko komunikazioa.
  * **Android TV Sukaldea Zerbitzua:** Sukaldean pantaila handi batean eskaerak bistaratzeko KDS sistema.
  * **Sare Zerbitzuak:** Active Directory (`jatetxea.local`), DNS eta DHCP.

---

## 3. Zibersegurtasun Abantailak

1. **Erasoen Isolatzea:** Erasotzaile batek web zerbitzaria (Ubuntu DMZ) arriskuan jarriko balu ere, ezin izango litzateke zuzenean datu-base nagusira sartu, ezta Windows domeinuko erabiltzaileetara ere, OPNsense suhesiak blokeatu egingo lukeelako.
2. **Kredentzialen Babesa:** Datu-baseko pasahitzak eta erabiltzaileak Windows barne-sarean bakarrik existitzen dira, ez dira inoiz kanpora azaltzen.
3. **Auditoria eta Erregistroak:** OPNsense-k trafiko bidegabe guztia erregistratzen du (Drop logs).
