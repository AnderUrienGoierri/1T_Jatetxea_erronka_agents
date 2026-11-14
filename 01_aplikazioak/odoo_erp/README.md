# 1T Jatetxea - Odoo 17 ERP eta Giza Baliabideak (HR)

Goierri Eskola - 2. Mailako Erronka Integratua  
**IT Administrazio Taldea:** Ander Urien Telleria (IT Lead), Julen Aguado Fernández, Iker Araujo Dominguez, Aratz Huegun Flores.

---

## 1. Helburua eta Arkitektura

Odoo 17 ERP sistemak jatetxearen barne kudeaketa administratiboa eta Giza Baliabideak (HR) zentralizatzen ditu:
* **Langileen Fitxa Nagusiak** (`hr.employee`): Sailak, karguak, NAN/DNI, kontaktua eta arduradun hierarkikoa.
* **Fitxaketak eta Ordutegi Kontrola** (`hr_attendance`): Sarrera-irteeren erregistroa, orduen kalkulua.
* **Bajak eta Oporrak** (`hr_holidays`): Gaixotasun-bajak, lan-istripuak, baimenak eta opor egunak.
* **Lan-Kontratuak** (`hr_contract`): Mugagabeak, denboraldikoak, soldata gordinak eta lanaldiak.

### Zergatik PostgreSQL eta ez MySQL?
Odoo ERP-ak arkitektura propioan **PostgreSQL** datu-basea soilik onartzen du bere ORM indartsuarentzat. Jatetxearen aplikazio bezeroek (Web PHP, JavaFX KDS, Flutter, C# TPV) **MySQL (`jatetxea_db`)** erabiltzen dute denbora errealeko erreserba, karta eta eskaeretarako. Bi sistemak paraleloan modu koordinatuan funtzionatzen dute.

---

## 2. Portuen Banaketa eta Talkak Ekiditea

| Zerbitzua | Portua Host-ean | Portua Edukiontzian | Oharra |
|---|---|---|---|
| **1T Jatetxea Odoo 17** | **8070** | 8069 | Docker bidez isolatua (talkarik ez) |
| **Odoo PostgreSQL DB** | **5432** | 5432 | Docker barruko datu-basea |
| *Probetako Odoo Lokala* | *8069* | *Native Windows* | Host sisteman instalatutako aurreko proba zerbitzua (`odoo-server-17.0`) |
| *Probetako Postgres Lokala* | *5433* | *Native Windows* | Host sisteman instalatutako PostgreSQL zerbitzua |

> **Garrantzitsua:** IT Kontrol Zentralak portu 8069an aurretik "ONLINE" erakusten zuen arrazoia Windows Host-eko zerbitzu lokal bat zelako zen (`odoo-server-17.0`). Jatetxearen edukiontzi ofiziala **8070 portuan** hedatu dugu garbi eta seguru lan egiteko.

---

## 3. Langileen Plantilla eta Sailak

### A. IT Administrazio Taldea
1. **Ander Urien Telleria** - IT Lead / FullStack Arkitektoa (`ander.urien@goierrieskola.eus`)
2. **Julen Aguado Fernández** - Sare eta Suebaki Ingeniaria (`julen.aguado@goierrieskola.eus`)
3. **Iker Araujo Dominguez** - Sistemen eta Birtualizazio Ingeniaria (`iker.araujo@goierrieskola.eus`)
4. **Aratz Huegun Flores** - Zibersegurtasun eta Ebaluazio Burua (`aratz.huegun@goierrieskola.eus`)

### B. Jatetxeko Langileak
5. **Ainhoa Goikoetxea** - Chef Burua / Sukaldari Nagusia (Saila: Sukaldea)
6. **Eneko Otegi** - Bigarren Sukaldaria / Parrilleroa (Saila: Sukaldea)
7. **Mikel Arregi** - Maître / Zerbitzari Burua (Saila: Jantokia)
8. **Maite Zabaleta** - Jantokiko Zerbitzaria (Saila: Jantokia)
9. **Unai Lasa** - Jantokiko Zerbitzaria (Saila: Jantokia)
10. **Gorka Mendizabal** - Barman / Barrako Arduraduna (Saila: Barra)
11. **Uxue Etxarri** - Barrako Zerbitzaria (Saila: Barra)
12. **Kepa Aldasoro** - Biltegi eta Hornidura Arduraduna (Saila: Biltegia)
13. **Ander Madinabeitia** - Jatetxearen Zuzendaria (Saila: Zuzendaritza)
14. **Joseba Ramos** - Jatetxearen Zuzendaria (Saila: Zuzendaritza)
15. **Estebe Ormazabal** - Jatetxearen Zuzendaria (Saila: Zuzendaritza)

---

## 4. Karpetaren Egitura

```
1T_Jatetxea_ODOO/
├── docker-compose.yml          # Odoo 17 + PostgreSQL 15 zerbitzuen definizioa
├── config/
│   └── odoo.conf               # Odoo zerbitzariaren konfigurazio fitxategia
├── addons/
│   └── jatetxea_hr/            # Odoo 17 HR modulu pertsonalizatua
│       ├── __init__.py
│       ├── __manifest__.py
│       └── data/
│           ├── hr_department_data.xml
│           ├── hr_job_data.xml
│           ├── hr_employee_data.xml
│           └── hr_contract_data.xml
├── data/                       # CSV fitxategiak (zuzeneko inportaziorako)
│   ├── 01_departamentuak.csv
│   ├── 02_lanpostuak.csv
│   ├── 03_langileak.csv
│   ├── 04_kontratuak.csv
│   ├── 05_fitxaketak.csv
│   └── 06_bajak.csv
├── scripts/
│   ├── abiarazi_odoo.ps1       # Zerbitzua altxatzeko script laguntzailea
│   └── sync_mysql_to_odoo.py   # MySQL -> Odoo XML-RPC sinkronizatzailea
└── README.md
```

---

## 5. Martxan Jartzeko Gida

1. **Edukiontziak abiarazi:**
   ```powershell
   cd C:\Dev\GoierriEskola\2_maila\1T_Jatxetxea_Erronka\1T_Jatetxea_ODOO
   docker compose up -d
   ```
2. **Nabigatzailean ireki:**
   * URL: `http://localhost:8070`
   * Master Password: `Errotagain169232$$`
   * Datu-base berria sortu: `jatetxea_odoo_db`
3. **Modulua instalatu:**
   * Aplikazioak (Apps) atalean `jatetxea_hr` aktibatu, edo CSV datuak inportatu.
