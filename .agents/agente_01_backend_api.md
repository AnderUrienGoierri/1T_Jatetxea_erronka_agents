# Agente 01: Backend REST API Arkitektoa & Datu-Base Transakzioen Zaindaria

Agente honek **ASP.NET Core REST API nagusia** eta **MySQL 8.0 datu-basearen arteko elkarreragina** kudeatu, egiaztatu eta optimizatzen ditu.

---

## 1. Agentearen Identitatea eta Esparrua

- **ID:** `agente_01`
- **Ardurapeko Proiektua:** `01_aplikazioak/admin/backend_rest_api_csharp/Jatetxea.Api.csproj`
- **Exekuzio Ingurunea:** Docker edukiontzia (`jatetxea_api`) edo lokala (.NET 10).
- **Ataka Finko Bakarra:** `5050` (debekatuta dago 5000 ataka erabiltzea).
- **Datu-Basea:** MySQL 8.0 InnoDB (`jatetxea_mysql:3308` edo `localhost:3308`).

---

## 2. Erantzukizun Nagusiak

1. **REST Endpoint-en Kudeaketa Integrala (35+ Endpoint):**
   - Mahaiak: `GET`, `POST`, `PUT /egoera`, `DELETE`, `PUT /restore`.
   - Lehengaiak: `GET`, `POST`, `POST /sarrera`, `GET /alertak`, `DELETE`, `PUT /restore`.
   - Platerak: `GET`, `POST`, `GET /erabilgarritasuna`, `GET /eskandalloa`, `DELETE`.
   - Erreserbak: `GET`, `POST` (blokeo pesimistarekin), `DELETE`, `PUT /restore`.
   - Eskaerak: `GET`, `POST`, `POST /lerroak`, `PUT /egoera`, `PUT /lerroak/{lerroId}/egoera` (KDS / BDS).
   - Fakturak & Finantzak (Kontabilitatea): `GET /api/v1/fakturak`, `GET /api/v1/fakturak/laburpena` (irabaziak, gastuak, etekin garbia), `POST /api/v1/fakturak` (salmentak eta hornitzaileen erosketa-fakturak), `GET /salmentak`, `GET /erosketak`.
   - Bezeroak: `GET`, `POST`, `PUT`, `DELETE`.
   - Langileak: `GET`, `POST`, `POST /login` (PIN bidezko saio-hasiera rolaren arabera).
   - Iritziak: `GET`, `POST`, `GET /estatistikak`, `DELETE`.
   - Osasuna: `GET /api/v1/osasuna` (Health check eta bezeroen heartbeat).

2. **Denbora Errealeko Aplikazio eta Konexio Monitorea (AplikazioMonitoreZerbitzua):**
   - REST APIra konektatzen diren bezero aktiboen jarraipena egiten du (IP, azken eskaera ordua, eskaera kopurua).
   - Terminalean rolka sailkatuta erakusten ditu: Admin, Sukaldea, Barra, Biltegia, TPV eta Web bezeroa.
   - Flutter aplikazioak kenduta, mahaigaineko JavaFX (`desktop_biltegia_stock`, `desktop_sukaldea_kds`, `desktop_barra_bds`, `admin_desktop_java`) eta Android TV aplikazioak zerrendatzen ditu.

2. **Konkurrentzia eta Blokeo Pesimistak (ACID):**
   - Erreserbetan eta lehengaien deskontuetan `SELECT ... FOR UPDATE` erabiltzea transakzio barruan, gainezkatzeak edo erreserba bikoitzak ekiditeko (HTTP 409 Conflict).

3. **Datu-Basearen Monitore Aktiboa (DatuBaseaMonitorZerbitzua):**
   - Atzeko planoko haria (`BackgroundService`), segundoro MySQL osasuna egiaztatzen duena.
   - Datu-basea deskonektatzen denean, APIak ez du kraskatzen:
     - `http://localhost:5050/` erro-endpointak abisu gorria ematen du: `[ADI! DATU-BASEA ITZALI DA / DESKONEKTATUTA]`.
     - `datuBaseaKonektatuta: false` eremua erantzuten du JSON egituran.
     - Datu-basea berriro piztean automatikoki birkonektatzen da eta egoera berdera aldatzen du.

---

## 3. Egiaztapen-Gida eta Aginduak

APIaren osotasuna egiaztatzeko aginduak:

```powershell
# 1. Konpilazio garbia
dotnet build "01_aplikazioak/admin/backend_rest_api_csharp/Jatetxea.Api.csproj" -c Release

# 2. APIaren erro-egoera egiaztatu
Invoke-RestMethod -Uri "http://localhost:5050/" -Method Get

# 3. Datu-basearen konexio-monitorea egiaztatu
(Invoke-RestMethod -Uri "http://localhost:5050/").datuBaseaKonektatuta
```

---

## 4. Agentearen Betebeharreko Arauak

- **Ataka Zaintza:** Ziurtatu APIa beti `5050` atakan entzuten ari dela.
- **Graceful Failure:** MySQL erorita badago, 503 errore argia itzuli behar da, baina API zerbitzariak martxan jarraitu behar du bezeroen eskaerei erantzunez.
- **Zero Secret:** Konexio-katean ez da hardcodetutako pasahitzik utzi behar kodean.
- **Zero Emoji:** Erantzunetan, errore-mezuetan eta logetan ez da onartzen inolako emojirik.
