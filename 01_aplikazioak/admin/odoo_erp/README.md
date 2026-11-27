# 1T Jatetxea S.L. — Odoo 17 ERP eta Giza Baliabideak (`Employees / HR`)

**Goierri Eskola — 2. Mailako Erronka Integratua (2025-2026)**  
**IT Administrazio Taldea (1. Taldea):** Ander Urien Telleria (IT Lead), Julen Aguado Fernández, Iker Araujo Dominguez, Aratz Huegun Flores.  
**Dokumentazio Txosten Osoa (EKS Errubrika):** [ENPRESA_KUDEAKETA_SISTEMAK_ODOO_17_TXOSTENA.md](docs/ENPRESA_KUDEAKETA_SISTEMAK_ODOO_17_TXOSTENA.md)

---

## 1. Helburua eta Arkitektura

Odoo 17 Community ERP sistemak (**LGPL-3** lizentzia, **On-Premise Docker**) 1T Jatetxea S.L.-ren Giza Baliabideak eta barne kudeaketa zentralizatzen ditu **Langileak (`hr` — Employees)** modulu nagusiaren inguruan:

* **Langileen Fitxa Integralak (`hr.employee`):** 15 langileen profil-irudi korporatiboak, 6 sailen hierarkia zuzena, lanpostuak, datu pribatuak (helbidea, NAN, GS, IBAN, larrialdi-kontaktua, ikasketak), 4 digituko TPV/Kiosk PIN ofizialak eta Badge ID-ak.
* **Organigrama eta Gaitasunak (`hr_org_chart`, `hr_skills`):** CEO-tik sail bakoitzeko langileetarainoko organigrama interaktiboa, 37 CV lerro eta 79 gaitasun-erregistro.
* **Lan-Kontratuak (`hr_contract`):** 15 kontratu aktibo, soldata gordinak, lanaldi motak (40h / 20h) eta Gipuzkoako Ostalaritza Hitzarmeneko klausulak.
* **Fitxaketak (`hr_attendance`):** 53 sarrera-irteera erreal eta lan-orduen kalkulu automatikoa.
* **Oporrak eta Bajak (`hr_holidays`):** 2026ko 23 opor-egunen esleipena, prestakuntza-egunak, onartutako oporrak/bajak eta onartzeko zain dauden eskaerak.
* **1T Jatetxea v2.0 Marka Identitatea (`jatetxea_hr`):** Logo korporatiboa, favicona eta `http://localhost:8070/` Web Portala T1 v2.0 estilo diseinu gidarekin (*Chianti `#881337`, Antique Gold `#D4A359`, Warm Hospitality `#FAF7F2`, Evening Espresso `#0F0D0E`*).

---

## 2. Portuen Banaketa eta Sarbideak

| Zerbitzua | URL / Helbidea | Portua Host-ean | Portua Edukiontzian | Oharra |
| :--- | :--- | :---: | :---: | :--- |
| **1T Jatetxea Odoo 17 ERP** | `http://localhost:8070` | **8070** | 8069 | Docker bidez isolatua (8069 talkarik ez) |
| **Odoo PostgreSQL 15 DB** | `127.0.0.1:5432` | **5432** | 5432 | Datu-basea: `jatetxea_odoo` (`odoo`/`odoo`) |
| **Jatetxearen Web Publikoa** | `http://localhost:8085` | **8085** | 80 | Bezeroen karta eta erreserbak (MySQL) |

* **Admin Sarbidea:** `ander.urien@goierrieskola.eus` (edo `admin`) / Pasahitza: `admin`
* **Master Password (Database Manager):** `Errotagain169232$$`

### Mugikorretik: Wi-Fi eta edozein tokitatik

Odoo-k hosteko `8070` ataka erabiltzen du; PostgreSQL `5432` ataka host horretan bertan bakarrik dago eskuragarri.

1. **Wi-Fi sare berean:** zerbitzarian exekutatu `ipconfig` eta hartu zerbitzariaren IPv4 helbidea. Mugikorreko Odoo aplikazioan sartu `http://<zerbitzariaren-IP>:8070` (adibidez, `http://192.168.10.10:8070`). Bi gailuek sare berean egon behar dute.
2. **Windows Suebakia:** administratzaile gisa PowerShell ireki zerbitzarian eta baimendu 8070/TCP sare lokaletik:
   ```powershell
   New-NetFirewallRule -DisplayName "Odoo 8070 - LAN" -Direction Inbound -Protocol TCP -LocalPort 8070 -Action Allow -RemoteAddress LocalSubnet -Profile Any
   ```
3. **Edozein tokitatik:** instalatu Tailscale zerbitzarian eta mugikorrean, eta hasi saioa tailnet berean. Zerbitzarian `tailscale ip -4` exekutatu, eta mugikorrean erabili `http://<Tailscale-IP>:8070`. Windows Suebakiak konexioa blokeatzen badu, baimendu tailneteko helbide-tartea:
   ```powershell
   New-NetFirewallRule -DisplayName "Odoo 8070 - Tailscale" -Direction Inbound -Protocol TCP -LocalPort 8070 -Action Allow -RemoteAddress 100.64.0.0/10 -Profile Any
   ```

Ez ireki `8070` routerrean Internetera, eta ez argitaratu PostgreSQL `5432` ataka. Tailscale bidezko sarbideak VPN pribatuan mantentzen du konexioa; erabiltzaile eta pasahitz seguruak erabili.

---

## 3. Karpetaren Egitura

```text
01_aplikazioak/admin/odoo_erp/
├── docker-compose.yml                         # Odoo 17 + PostgreSQL 15 definizioa
├── config/
│   └── odoo.conf                              # Odoo zerbitzariaren konfigurazioa (jatetxea_odoo)
├── addons/
│   └── jatetxea_hr/                           # 1T Jatetxea modulu pertsonalizatua (v17.0.2.0.0)
│       ├── __init__.py
│       ├── __manifest__.py
│       ├── data/                              # XML hasierako datuak (sailak, lanpostuak, langileak, kontratuak)
│       ├── views/
│       │   └── website_templates.xml          # Google Fonts (Cinzel/Plus Jakarta Sans) eta Login marka
│       └── static/
│           ├── description/icon.png           # T1 Jatetxea aplikazio-ikonoa
│           └── src/
│               ├── img/                       # Logo ofizialak, favicona eta 15 langileen avatarrak
│               └── scss/                      # jatetxea_theme.scss (Web) eta jatetxea_backend.scss (ERP)
├── data/                                      # CSV babes-kopiak (01_departamentuak .. 06_bajak)
├── scripts/
│   ├── abiarazi_odoo.ps1                      # Zerbitzua abiarazteko PowerShell scripta
│   ├── konfiguratu_odoo_osoa.py               # ORM bidezko konfigurazio integral idempotentea
│   ├── konfiguratu_odoo_portal.py             # Portal konfigurazio esteka
│   └── sync_mysql_to_odoo.py                  # MySQL -> Odoo XML-RPC sinkronizatzailea
├── docs/
│   ├── ENPRESA_KUDEAKETA_SISTEMAK_ODOO_17_TXOSTENA.md  # Erronkako dokumentazio osoa
│   └── img/                                   # 15 pantaila-argazki erreal (00 .. 14)
└── README.md
```

---

## 4. Martxan Jartzea eta Konfigurazio Osoa Aplikatzea

1. **Edukiontziak abiarazi:**
   ```powershell
   docker start jatetxea_odoo_postgres jatetxea_odoo_erp
   ```
   *(Edo garbi sortzeko: `.\scripts\abiarazi_odoo.ps1`)*
2. **`jatetxea_hr` modulua eguneratu eta datu osoak kargatu (idempotentea):**
   ```powershell
   docker exec jatetxea_odoo_erp odoo -c /etc/odoo/odoo.conf -d jatetxea_odoo -u jatetxea_hr --stop-after-init --no-http
   docker cp .\scripts\konfiguratu_odoo_osoa.py jatetxea_odoo_erp:/tmp/konfiguratu_odoo_osoa.py
   docker exec jatetxea_odoo_erp sh -c "odoo shell -c /etc/odoo/odoo.conf -d jatetxea_odoo --no-http < /tmp/konfiguratu_odoo_osoa.py"
   docker restart jatetxea_odoo_erp
   ```
3. **Nabigatzailean ireki:** `http://localhost:8070`
