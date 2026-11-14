# Rack Diseinua eta Kableatu Egituratua - Goierri Eskola Laborategia

Fisikoki laborategian muntatuko den rack-aren banaketa, patch panelen esleipena eta kableatuaren kolore-kodeen araudia.

---

## 1. Rack-aren Unitateen Banaketa (19" Rack - 12U/24U)

```text
+--------------------------------------------------------------+
| U12: Pasaharra / PDU (Kableen kudeatzailea)                 |
+--------------------------------------------------------------+
| U11: OPNsense Firewall Hardware / Router Nagusia             |
+--------------------------------------------------------------+
| U10: Pasaharra horizontal (Kable antolatzailea)              |
+--------------------------------------------------------------+
| U09: Cisco Catalyst 2960 Kudeatutako Switch-a                |
+--------------------------------------------------------------+
| U08: Patch Panel Nagusia (24 portu RJ45 Cat 6)               |
+--------------------------------------------------------------+
| U07: Pasaharra horizontal                                    |
+--------------------------------------------------------------+
| U05-U06: Apalategia / Shelf (Mini PCak / NUCak)              |
|          - PC 2: Ubuntu Server (DMZ)                         |
|          - WiFi PoE Injektorea                               |
+--------------------------------------------------------------+
| U01-U04: Dorrea / Zerbitzari Nagusia                         |
|          - PC 1: Windows Server 2025 (LAN Core)              |
|          - UPS / SAI (Elikatze-iturri etengabea)             |
+--------------------------------------------------------------+
```

---

## 2. Kableatuaren Kolore-Kodeak (TIA/EIA-568-B Araua)

Laborategian bisualki erraz identifikatu eta akatsak saihesteko, kable koloreen estandarra finkatu da:

| Kolorea | Erabilera / VLAN | Deskribapena |
| :--- | :--- | :--- |
| **Horia** | WAN / Uplink | Interneteko sarbidea (Eskolako saretik OPNsensera) |
| **Gorria** | 802.1Q Trunk | OPNsense eta Cisco Switch arteko trunking nagusia |
| **Urdina** | VLAN 10 (LAN) | Barne zerbitzariak, TPV kutxa, Sukaldeko KDS |
| **Laranja** | VLAN 20 (DMZ) | Ubuntu Web zerbitzaria (Apache / PHP) |
| **Berdea** | VLAN 30 (WiFi/Guest)| Haririk gabeko Access Point-era doan lotura |
| **Grisa** | VLAN 99 (Kudeaketa)| Kontsolako kableak eta kudeaketa ataka dedikatuak |

---

## 3. Patch Panel Portuen Esleipen Matrizea

| Patch Portua | Konektatutako Ekipoa | Switch Portua | VLAN Esleitua |
| :--- | :--- | :--- | :--- |
| **P-01** | OPNsense LAN NIC (Trunk) | GigabitEthernet 0/1 | Trunk (VLAN 10,20,30,99) |
| **P-02** | Windows Server 2025 (NIC 1) | FastEthernet 0/1 | VLAN 10 (LAN) |
| **P-03** | TPV Kutxa (Barra) | FastEthernet 0/2 | VLAN 10 (LAN) |
| **P-04** | Sukaldeko KDS Pantaila | FastEthernet 0/3 | VLAN 10 (LAN) |
| **P-05** | Ticket Inprimagailu Ethernet | FastEthernet 0/4 | VLAN 10 (LAN) |
| **P-09** | Ubuntu Server (DMZ Apache) | FastEthernet 0/9 | VLAN 20 (DMZ) |
| **P-17** | WiFi Access Point (PoE) | FastEthernet 0/17 | Trunk (VLAN 10,30) |
| **P-24** | Kudeaketa Lanpostua (Admin) | FastEthernet 0/24 | VLAN 99 (Kudeaketa) |
