# 02. Docker Ekosistema eta Edukiontzien Banaketa

Dokumentu honek sistema osoa simulatzeko eta ekoizpenerako prestatutako Docker edukiontzi guztiak zehazten ditu. Helburua da aplikazio bakoitzak bere edukiontzi isolatua izatea, banaka aztertu, abiarazi, gelditu eta gainbegiratu ahal izateko.

---

## 1. Edukiontzien Taula Orokorra

| Edukiontzia | Irudia | Portua (Host:Kont.) | Funtzioa / Deskribapena | Kokapena Arkitekturan |
| :--- | :--- | :--- | :--- | :--- |
| **jatetxea_mysql** | `mysql:8.0` | `3308:3306` | MySQL Datu-base nagusia (`jatetxea_db`, 11 taula, demo datuak) | Windows Server (LAN) |
| **jatetxea_api** | `mcr.microsoft.com/dotnet/aspnet:10.0` | `5000:5000` | ASP.NET Core C# REST API nagusia | Windows Server (LAN) |
| **jatetxea_web_php** | `php:8.2-apache` | `8085:80` | Jatetxearen webgune publikoa (Karta, Erreserbak, Iritziak) | Ubuntu Server (DMZ) |
| **jatetxea_chat** | `node:20-alpine` | `3001:3001` | Socket.IO barne txat zerbitzaria (sukaldea, barra, zerbitzariak) | Windows Server (LAN) |
| **jatetxea_kds_tv** | `nginx:alpine` | `8086:80` | Sukaldeko Android TVrako optimizatutako KDS web panela | Windows Server (LAN) |
| **jatetxea_odoo_postgres** | `postgres:15-alpine` | `5432:5432` | Odoo ERParentzako PostgreSQL datu-basea | Windows Server (LAN) |
| **jatetxea_odoo_erp** | `odoo:17` | `8069:8069` | Odoo ERP (Langileen baimenak, fitxaketak, bajak eta kudeaketa) | Windows Server (LAN) |

---

## 2. Edukiontzi Bakoitzaren Xehetasunak

### 1. `jatetxea-db` (MySQL 8.0)
* **Karpeta:** `docker/db/`
* **Hasieratzea:** Lehenengo aldian edukiontzia altxatzean, automatikoki exekutatzen du `01_schema.sql` fitxategia, 11 taulak, integritate-murrizketak (CHECK) eta hasierako produktu, langile eta mahai guztiak kargatuz.
* **Iraunkortasuna:** `jatetxea_db_data` Docker bolumen dedikatuan gordetzen dira datuak, edukiontzia berrabiarazi arren daturik ez galtzeko.

### 2. `jatetxea-api` (C# ASP.NET Core .NET 10)
* **Karpeta:** `docker/api/Dockerfile`
* **Eraikuntza:** Multi-stage build eredu profesionala (SDK-arekin eraiki eta garbi argitaratu runtime arin batean).
* **Segurtasuna:** `jatetxea_user` erabiltzaile ez-pribilegiatuarekin exekutatzen da edukiontzi barruan.
* **Osasun-kontrola:** `/health` endpoint-a integratuta, MySQL konexioaren egoera denbora errealean egiaztatzeko.

### 3. `jatetxea-web` (Apache / PHP 8.2)
* **Karpeta:** `docker/web/Dockerfile`
* **Integrazioa:** `web_php/` karpetako fitxategiak zuzenean mapeatzen dira. `API_BASE_URL` ingurune-aldagaiaren bidez komunikatzen da REST APIarekin.

### 4. `jatetxea-chat` (Barne Txat Zerbitzaria)
* **Karpeta:** `docker/chat/`
* **Protokoloa:** WebSockets eta Socket.IO.
* **Gelak (Rooms):**
  * `sukaldea`: Sukaldeko eskaerak eta abisuak.
  * `barra`: Edarien komandak.
  * `zerbitzariak`: Mahaietako deiak eta jakinarazpenak.
  * `orokorra`: Jatetxe osoko oharrak.
* **API integrazioa:** `POST /api/notify` bidez REST APIak jakinarazpen automatikoak txertatu ditzake gelan (adibidez, sukaldera eskaera bidaltzean).

### 5. `jatetxea-kds-tv` (Sukaldeko Android TV)
* **Karpeta:** `docker/sukaldea_tv/`
* **Xede Gailua:** Sukaldean horman kokatutako Smart TV edo Android TV telebistak.
* **Ezaugarriak:** Kontraste handia, txartel handiak (3 metrotara argi irakurtzeko moduan), denbora neurgailuak eta telebistako urruneko agintearekin nabigatzeko aukera.

### 6. `jatetxea-odoo` (Odoo 17 ERP eta PostgreSQL)
* **Karpeta:** `docker/odoo/`
* **Helburua Erronkan:**
  * Jatetxeko langileen baimenak eta rolak kudeatzea (Zerbitzaria, Sukaldaria, Kudeatzailea).
  * Fitxaketak erregistratzea (sarrera/irteera orduak).
  * Bajak eta oporraldiak onartzea.
  * Hornitzaileen fakturak eta orokorreko administrazioa bateratzea.

---

## 3. Banakako Kontrola eta Aginduak

Docker Compose bidez zerbitzu guztiak batera zein banaka kudeatu daitezke:

* **Zerbitzu guztiak altxatzea:**
  ```powershell
  docker compose up -d
  ```

* **Edukiontzi bakarra berreraiki eta altxatzea (adibidez, APIa soilik):**
  ```powershell
  docker compose up -d --build jatetxea-api
  ```

* **Edukiontzi baten log-ak jarraitzea:**
  ```powershell
  docker compose logs -f jatetxea-api
  docker compose logs -f jatetxea-chat
  ```

* **Edukiontzi guztiak gelditzea:**
  ```powershell
  docker compose down
  ```
