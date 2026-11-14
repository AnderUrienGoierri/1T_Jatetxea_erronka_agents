# 1T Jatetxea Entornoa: Sistema Integratua, Docker Edukiontziak eta Sare Zentralita

Ongi etorri **1T Jatetxea Entornoa** biltegira. Karpeta nagusi honek jatetxearen sistema osoa kudeatzeko beharrezko Docker konfigurazioak, zerbitzu anitzeko orkestrazioa, sare zentralita eta 3 makina birtualen simulazio-ingurunea biltzen ditu.

---

## 1. Sistema Orokorraren Laburpena

Soluzio honek jatetxe baten behar teknologiko guztiak estaltzen ditu:
1. **Sare eta Zibersegurtasuna (3 Makina Birtual):**
   * **OPNsense Firewall:** Sare segmentazioa (WAN, LAN, DMZ) eta Zero-Trust trafiko kontrola.
   * **Ubuntu DMZ (`192.168.20.10`):** Bezeroen webgune publikoa (Apache 2.4 / PHP 8.2).
   * **Windows Server 2025 (`192.168.10.10`):** Barneko zerbitzari nagusia (MySQL, ASP.NET Core API, Odoo ERP, Chat Server, Active Directory, DNS eta DHCP).
2. **Docker Edukiontzien Ekosistema:**
   * Aplikazio bakoitzak bere edukiontzi independentea du, banaka aztertu eta kontrolatzeko.
3. **Sare Zentralita Dinamikoa (`switch-env.ps1`):**
   * WiFi sare ezberdinetan (Goierri Eskolan, Tolosako etxean, etxean edo lokalean) aplikazio bezero guztiak klik bakarrean birbideratzeko automatizazioa.
4. **Bezero Anitzeko Interfazeak:**
   * C# Desktop TPV (Kutxak eta barra).
   * JavaFX Desktop KDS / BDS / Biltegia (Sukaldea eta stocka).
   * Flutter Zerbitzaria (Mugikorrak eta tabletak).
   * Flutter Bezeroa (Erreserbak eta eskaerak).
   * Android TV Sukaldea (KDS telebista adimendunaren pantaila).
   * Odoo 17 ERP (Erabiltzaileen baimenak, fitxaketak eta bajak).

---

## 2. Direktorioen Egitura

```text
1T_Jatetxea_Entornoa/
├── docker/
│   ├── api/             # ASP.NET Core (.NET 10) REST API Dockerfile
│   ├── web/             # PHP 8.2 Apache Dockerfile
│   ├── db/              # MySQL 8.0 Datu-base Dockerfile
│   ├── odoo/            # Odoo 17 Community eta konfigurazioa (odoo.conf)
│   ├── chat/            # Barne Txat Zerbitzaria (Node.js / Socket.IO)
│   └── sukaldea_tv/     # Sukaldeko Android TVrako KDS panela (Nginx)
├── config/
│   └── environments.json # Sare profilak (localhost, tolosako_etxea, eskola, etxean, vbox)
├── scripts/
│   ├── switch-env.ps1          # WiFi sareen zentralita automatikoa
│   ├── start-all.ps1           # Docker edukiontzi guztiak abiaraztea
│   ├── stop-all.ps1            # Edukiontzi guztiak gelditzea
│   └── test-connectivity.ps1   # Zerbitzuen osasuna egiaztatzea
├── doc/
│   ├── 01_arkitektura_eta_makina_birtualak.md
│   ├── 02_docker_ekosistema_eta_edukiontziak.md
│   ├── 03_wifi_sareak_eta_zentralita.md
│   └── 04_bezeroen_konexio_gida.md
├── docker-compose.yml              # Tokiko simulazio osoa (7 edukiontzi)
├── docker-compose.vbox-windows.yml # Windows Server VMrako Docker fitxategia
├── docker-compose.vbox-ubuntu.yml  # Ubuntu DMZ VMrako Docker fitxategia
└── README.md
```

---

## 3. Abio Azkarreko Gida

### 1. Sare Profila Hautatzea (Zentralita)
Uneko WiFi sarera egokitzeko, exekutatu:

```powershell
.\scripts\switch-env.ps1
```
Aukeratu zure uneko kokapena (adibidez: `tolosako_etxea`, `eskola` edo `localhost`).

### 2. Docker Zerbitzu Guztiak Abiaraztea
Garatzailearen ordenagailuan edukiontzi guztiak altxatzeko:

```powershell
.\scripts\start-all.ps1
```

### 3. Zerbitzuen Osasuna Egiaztatzea
Zerbitzu guztiak erantzuten ari direla egiaztatzeko:

```powershell
.\scripts\test-connectivity.ps1
```

---

## 4. Portuen eta Sarbide-Puntuen Erreferentzia

| Zerbitzua | Portua | Helbidea (Localhost) | Helbidea (Sare Lokalean) |
| :--- | :--- | :--- | :--- |
| **MySQL Datu-Basea** | `3308` | `localhost:3308` | `<IP>:3308` |
| **ASP.NET Core REST API** | `5000` | `http://localhost:5000/api/v1` | `http://<IP>:5000/api/v1` |
| **Web PHP Publikoa** | `8085` | `http://localhost:8085` | `http://<IP>:8085` |
| **Barne Txat Zerbitzaria** | `3001` | `http://localhost:3001` | `http://<IP>:3001` |
| **Sukaldea KDS Android TV** | `8086` | `http://localhost:8086` | `http://<IP>:8086` |
| **Odoo 17 ERP (Kudeaketa)** | `8069` | `http://localhost:8069` | `http://<IP>:8069` |
