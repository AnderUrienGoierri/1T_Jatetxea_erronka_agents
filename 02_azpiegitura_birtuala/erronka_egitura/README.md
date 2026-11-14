# Erronka 1 - Jatetxea Sare Sistema eta Aplikazioen Hedapena

Zibersegurtasuneko 2. mailako 2. ebaluazioko erronka teknikoa: Jatetxe-kudeaketarako plataforma osoa (`1T_Jatetxea_erronka_agents`) hedatzea OPNsense suebakidun azpiegitura birtualizatu eta segmentatu batean (DMZ eta LAN).

---

## 1. Makina Birtualen Kredentzialak eta Sarbideak

| Makina | Sistema Eragilea | Gunea / Funtzioa | Sarbide Bidea | Kredentzialak |
| :--- | :--- | :--- | :--- | :--- |
| **Erronka-OPNsense** | OPNsense 26.7 (FreeBSD) | Suebakia, Router-on-a-stick, NAT | WebGUI: `http://127.0.0.1:8443`<br/>SSH: `ssh -p 2231 root@127.0.0.1` | `root` / `Errotagain169232$$`<br/>`ander` / `Errotagain169232$$` |
| **Erronka-WinServer2025** | Windows Server 2025 | LAN: Active Directory, DNS, DHCP, IIS | RDP: `mstsc /v:127.0.0.1:3391`<br/>SSH: `ssh -p 2232 ander@127.0.0.1` | `ander` / `Errotagain169232$$`<br/>(Domain: `jatetxea.local`) |
| **Erronka-Ubuntu** | Ubuntu Desktop 26.04 | DMZ: Apache (Web), .NET API, MySQL DB | SSH: `ssh -p 2230 ander@127.0.0.1`<br/>Web: `http://127.0.0.1:80` (DMZ bidez) | `ander` / `Errotagain169232$$`<br/>(Sudo baimenekin) |

> Xehetasun osoak: ikus [kredentzialak.md](file:///c:/Dev/GoierriEskola/2_maila/Zibersegurtasuna/2_ebaluaketa/erronka_sare_sistema/kredentzialak.md)

---

## 2. Kudeaketa Azkarreko Script-ak

* [hasi_makinak_headless.bat](file:///c:/Dev/GoierriEskola/2_maila/Zibersegurtasuna/2_ebaluaketa/erronka_sare_sistema/hasi_makinak_headless.bat): Hiru makina birtualak atzetik ixilpean abiarazten ditu (headless moduan, leihorik gabe) eta ataka guztiak prest egon arte egiaztatzen ditu.
* [gelditu_makinak.bat](file:///c:/Dev/GoierriEskola/2_maila/Zibersegurtasuna/2_ebaluaketa/erronka_sare_sistema/gelditu_makinak.bat): Hiru makinak modu seguruan eta ordenatuan itzaltzen ditu ACPI bidez.

---

## 3. Dokumentazio Tekniko Osoa (`doc/`)

Proiektuaren azterketa eta hedapen plana 7 dokumentu teknikotan garatu da sakon:

1. **[00. Aurkibidea eta Proiektuaren Ikuspegi Orokorra](file:///c:/Dev/GoierriEskola/2_maila/Zibersegurtasuna/2_ebaluaketa/erronka_sare_sistema/doc/00_aurkibidea_eta_proiektu_ikuspegia.md)**
2. **[01. Sare Topologia eta Azpiegitura Plana](file:///c:/Dev/GoierriEskola/2_maila/Zibersegurtasuna/2_ebaluaketa/erronka_sare_sistema/doc/01_sare_topologia_eta_azpiegitura.md)**
3. **[02. OPNsense Suebakia eta Segurtasun Matrizea](file:///c:/Dev/GoierriEskola/2_maila/Zibersegurtasuna/2_ebaluaketa/erronka_sare_sistema/doc/02_opnsense_suebakia_eta_segurtasun_matrizea.md)**
4. **[03. Zerbitzarien Arkitektura eta Rolak](file:///c:/Dev/GoierriEskola/2_maila/Zibersegurtasuna/2_ebaluaketa/erronka_sare_sistema/doc/03_zerbitzarien_arkitektura_eta_rolak.md)**
5. **[04. Datu-Basea eta Backend API Hedapena](file:///c:/Dev/GoierriEskola/2_maila/Zibersegurtasuna/2_ebaluaketa/erronka_sare_sistema/doc/04_datu_basea_eta_backend_api_hedapena.md)**
6. **[05. Bezero Aplikazioak eta Plataformak](file:///c:/Dev/GoierriEskola/2_maila/Zibersegurtasuna/2_ebaluaketa/erronka_sare_sistema/doc/05_bezero_aplikazioak_eta_plataformak.md)**
7. **[06. Hedapen Gida Urratsez Urrats (Runbook)](file:///c:/Dev/GoierriEskola/2_maila/Zibersegurtasuna/2_ebaluaketa/erronka_sare_sistema/doc/06_hedapen_gida_urratsez_urrats.md)**
