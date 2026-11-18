# 02. Docker Ekosistema eta Edukiontzien Banaketa

Dokumentu honek sistema osoa simulatzeko eta ekoizpenerako prestatutako Docker edukiontzi guztiak zehazten ditu. Helburua da aplikazio bakoitzak bere edukiontzi isolatua izatea, banaka aztertu, abiarazi, gelditu eta gainbegiratu ahal izateko.

---

## 1. Edukiontzien Taula Orokorra

| Edukiontzia | Irudia | Portua (Host:Kont.) | Funtzioa / Deskribapena | Kokapena Arkitekturan |
| :--- | :--- | :--- | :--- | :--- |
| **jatetxea_mysql** | `mysql:8.0` | `3308:3306` | MySQL Datu-base nagusia (`jatetxea_db`, 11 taula, demo datuak) | Windows Server (LAN) |
| **jatetxea_api** | `mcr.microsoft.com/dotnet/aspnet:10.0` | `5050:5000` | ASP.NET Core C# REST API nagusia (.NET 10) | Windows Server (LAN) |
| **jatetxea_web_php** | `php:8.4-apache` | `8085:80` | Jatetxearen webgune publikoa (Karta, Erreserbak, Iritziak) | Ubuntu Server (DMZ) |
| **jatetxea_chat** | `node:20-alpine` | `3001:3001` | Socket.IO barne txat zerbitzaria (sukaldea, barra, zerbitzariak) | Windows Server (LAN) |
| **jatetxea_kds_tv** | `nginx:alpine` | `8086:80` | Sukaldeko Android TVrako optimizatutako KDS web panela | Windows Server (LAN) |
| **jatetxea_odoo_postgres** | `postgres:15-alpine` | `5432:5432` | Odoo ERParentzako PostgreSQL datu-basea | Windows Server (LAN) |
| **jatetxea_odoo_erp** | `odoo:17` | `8070:8069` | Odoo ERP (Langileen baimenak, fitxaketak, bajak eta kudeaketa) | Windows Server (LAN) |
| **jatetxea_phpmyadmin** | `phpmyadmin:5.2` | `8081:80` | MySQL kudeaketa-panela (phpMyAdmin) | Windows Server (LAN) |
| **jatetxea_postgreSQL_adminer** | `adminer:latest` | `8082:8080` | PostgreSQL eta MySQL kudeaketa arina (Adminer) | Windows Server (LAN) |

---

## 2. Edukiontzi Bakoitzaren Xehetasunak

### 1. `jatetxea-db` (MySQL 8.0)

- **Karpeta:** `02_azpiegitura_birtuala/1T_Jatetxea_Entorno_Birtuala/docker/db/`
- **Hasieratzea:** Lehenengo aldian edukiontzia altxatzean, automatikoki exekutatzen du `01_schema.sql` fitxategia, 11 taulak, integritate-murrizketak (CHECK) eta hasierako produktu, langile eta mahai guztiak kargatuz.
- **Iraunkortasuna:** `jatetxea_db_data` Docker bolumen dedikatuan gordetzen dira datuak, edukiontzia berrabiarazi arren daturik ez galtzeko.

### 2. `jatetxea-api` (C# ASP.NET Core .NET 10)

- **Karpeta:** `02_azpiegitura_birtuala/1T_Jatetxea_Entorno_Birtuala/docker/api/Dockerfile`
- **Iturburua:** `01_aplikazioak/admin/backend_rest_api_csharp/`
- **Eraikuntza:** Multi-stage build eredu profesionala (SDK-arekin eraiki eta garbi argitaratu runtime arin batean).
- **Segurtasuna:** `jatetxea_user` erabiltzaile ez-pribilegiatuarekin exekutatzen da edukiontzi barruan.
- **Osasun-kontrola:** `/health` endpoint-a integratuta, MySQL konexioaren egoera denbora errealean egiaztatzeko.

### 3. `jatetxea-web` (Apache / PHP 8.4)

- **Karpeta:** `02_azpiegitura_birtuala/1T_Jatetxea_Entorno_Birtuala/docker/web/Dockerfile`
- **Iturburua:** `01_aplikazioak/bezeroak/web_php_bezeroak/`
- **Integrazioa:** `01_aplikazioak/bezeroak/web_php_bezeroak/` karpetako fitxategiak zuzenean mapeatzen dira. `API_BASE_URL` ingurune-aldagaiaren bidez komunikatzen da REST APIarekin (`http://jatetxea-api:5000/api/v1`).

### 4. `jatetxea-chat` (Barne Txat Zerbitzaria)

- **Karpeta:** `02_azpiegitura_birtuala/1T_Jatetxea_Entorno_Birtuala/docker/chat/`
- **Iturburua:** `01_aplikazioak/chat_zerbitzaria/`
- **Protokoloa:** WebSockets eta Socket.IO.
- **Gelak (Rooms):**
  - `sukaldea`: Sukaldeko eskaerak eta abisuak.
  - `barra`: Edarien komandak.
  - `zerbitzariak`: Mahaietako deiak eta jakinarazpenak.
  - `orokorra`: Jatetxe osoko oharrak.
- **API integrazioa:** `POST /api/notify` bidez REST APIak jakinarazpen automatikoak txertatu ditzake gelan.

### 5. `jatetxea-kds-tv` (Sukaldeko Android TV)

- **Karpeta:** `02_azpiegitura_birtuala/1T_Jatetxea_Entorno_Birtuala/docker/sukaldea_tv/`
- **Iturburua:** `01_aplikazioak/sukaldea_kds_android_tv/`
- **Xede Gailua:** Sukaldean horman kokatutako Smart TV edo Android TV telebistak.
- **Ezaugarriak:** Kontraste handia, txartel handiak (3 metrotara argi irakurtzeko moduan), denbora neurgailuak eta telebistako urruneko agintearekin nabigatzeko aukera.

### 6. `jatetxea-odoo` (Odoo 17 ERP eta PostgreSQL)

- **Karpeta:** `02_azpiegitura_birtuala/1T_Jatetxea_Entorno_Birtuala/docker/odoo/`
- **Iturburua:** `01_aplikazioak/odoo_erp/`
- **Helburua Erronkan:**
  - Jatetxeko langileen baimenak eta rolak kudeatzea (Zerbitzaria, Sukaldaria, Kudeatzailea).
  - Fitxaketak erregistratzea (sarrera/irteera orduak).
  - Bajak eta oporraldiak onartzea.
  - Hornitzaileen fakturak eta orokorreko administrazioa bateratzea.

---

## 3. Banakako Kontrola eta Aginduak

Docker Compose bidez zerbitzu guztiak batera zein banaka kudeatu daitezke:

- Edukiontziak ez dira Docker Desktop abiaraztean automatikoki piztuko; eskuz abiarazi behar dira.

- **Zerbitzu guztiak altxatzea:**

  ```powershell
  cd 02_azpiegitura_birtuala\1T_Jatetxea_Entorno_Birtuala
  docker compose up -d
  ```

- **Edukiontzi bakarra berreraiki eta altxatzea (adibidez, APIa soilik):**

  ```powershell
  docker compose up -d --build jatetxea-api
  ```

- **Edukiontzi baten log-ak jarraitzea:**

  ```powershell
  docker compose logs -f jatetxea-api
  docker compose logs -f jatetxea-chat
  ```

- **Edukiontzi guztiak gelditzea:**

  ```powershell
  docker compose down
  ```
