# Cisco Packet Tracer - 1T Jatetxea Sare Diseinua eta Simulazioa

Karpeta honetan Cisco Packet Tracer erabiliz sortutako topologiaren fitxategiak, eskemak eta kommutadore zein bideratzaileen konfigurazio-scriptak gordetzen dira.

---

## 1. Topologiaren Egitura (Packet Tracer Eskema)

```
                       [ INTERNET / WAN ]
                                |
                         (DHCP / Eskola)
                                |
                     [ OPNsense / Router ]
                      (Router-on-a-Stick)
                                |  Gig0/0 (Trunk: Subinterfazeak: .10, .20, .30, .99)
                                |
                     [ Cisco Catalyst 2960 ]
                                |
      +-------------------------+-------------------------+-------------------------+
      |                         |                         |                         |
Fa0/1 - Fa0/8             Fa0/9 - Fa0/16            Fa0/17 - Fa0/22               Fa0/24
(Access VLAN 10)          (Access VLAN 20)          (Trunk / Access VLAN 30)   (Access VLAN 99)
 [ LAN Gunea ]             [ DMZ Gunea ]             [ WiFi AP / Bezeroak ]     [ Kudeaketa ]
      |                         |                         |                         |
  - WinServer 2025          - Ubuntu Web              - Access Point             - Admin PC
  - TPV C# Kutxa              (Apache/PHP)            - Bezeroen Mugikorrak
  - KDS JavaFX Sukaldea                               - Zerbitzarien PDAk (VLAN 10)
  - Ticket Inprimagailua
```

---

## 2. Cisco Packet Tracer Topologia Fitxategiak

| Fitxategia | Deskribapena |
| :--- | :--- |
| `topologia_orokorra.pkt` | Cisco Packet Tracer bidez irekitzeko proiektu osoa (Cisco 2960 Switch + 2811/2911 Router + PCak eta Zerbitzariak). |
| `topologia_eskema.png` | Packet Tracer-etik esportatutako sare-eskema grafikoa dokumentazioan txertatzeko. |
| `konfigurazio_scriptak_ios/` | Ekipoetan kopiatu eta itsasteko komando sekuentziak. |

---

## 3. Zibersegurtasun Neurriak Switch-ean (Cisco IOS)

Laborategian zein ekoizpenean aplikatu beharreko segurtasun konfigurazioak:
1. **Port Security:** Portu bakoitzean baimendutako MAC helbide kopurua mugatu (gehienez 1-2). Eraso bat detektatzean portua itzali (`violation shutdown`).
2. **DHCP Snooping:** Bezeroek baimenik gabeko DHCP zerbitzari faltsurik ez jartzeko (Rogue DHCP). Switch-eko `Fa0/1` (edo trunk portua) soilik markatu `trust` gisa.
3. **Erabili gabeko portuak itzali:** Konektatu gabeko portu guztiak `shutdown` jarri eta zabor-VLAN batera esleitu (VLAN 999).
4. **Trunking segurua:** DTP (Dynamic Trunking Protocol) desgaitu eta Native VLAN aldatu (ez erabili VLAN 1 defektuzkoa).
