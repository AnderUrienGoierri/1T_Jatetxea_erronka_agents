# E1 — Bideo Aurkezpenaren Gidoia (5–7 minutu) eta 10/10 Goi-Mailako Ezaugarriak

**Modulua:** Datuen Atzipena — E1 Entregagaia  
**Helburua:** 5–7 minutuko bideo-aurkezpenean (7 minutu pasa gabe) **REST APIa bera** eta **aplikazio ezberdinetatik konexioa nola egiten den** azaltzea, 7,5 oinarrizko mailatik **10/10 bikaintasun-mailara** iritsiz.

---

## 1. E1 Eskakizunen Analisia (`E1.png`)

- **Iraupena:** **5–7 minutu** (ez pasa 7 minututik — helburua: **6 min 15 seg**).
- **Bi ardatz nagusi derrigorrezkoak:**
  1. **APIa bera azaldu:** arkitektura, ORM (FluentNHibernate), endpoint konplexuak, transakzioak, konkurrentzia, segurtasuna eta Swagger UI.
  2. **Aplikazio ezberdinetatik konexioa nola egiten den azaldu:** HTTP bezeroak, header-ak, JSON deserializazioa eta API berera konektatutako 5 teknologia ezberdinak.
- **Kalifikazio-erreferentzia:**
  - **7,5 maila:** API REST osoa (CRUD estandarrak: `GET`, `POST`, `PUT`, `DELETE` eta oinarrizko konexioa).
  - **10/10 maila:** Zure APIan **dagoeneko inplementatuta eta martxan** dauden goi-mailako ezaugarri guztiak erakustea eta zuzenean frogatzea.

---

## 2. 10/10 Mailako Ezaugarri Guztiak (INPLEMENTATUTA ETA FROGATUTA)

| # | Ezaugarri aurreratua | Endpoint-a / Fitxategia | Zergatik balio du 10/10 |
|---|---|---|---|
| **1** | **FluentNHibernate ORM osoa** | `Data/NHibernateHelper.cs`, `Mappings/*.cs` (15 `ClassMap<T>`) | `ISessionFactory` Singleton, `MySqlConnectorDriver`, `MySQL8InnoDBDialect`, `CompositeId` (`PlaterOsagaiaMap`), 2 MySQL `VIEW` (`ReadOnly()`), `.Fetch()` N+1 prebentzioa. **0 SQL gordina** kontroladoreetan. |
| **2** | **Swagger UI / OpenAPI interaktiboa** | `GET /swagger` · `GET /openapi/v1.json` (`Program.cs`) | APIko 38 endpoint guztiak zuzenean nabigatzailean dokumentatuta eta probatzeko prest (Docker Production barne). |
| **3** | **JWT Bearer (HS256) + RBAC rol-baimenak** | `Helpers/JwtHelper.cs`, `POST /api/v1/langileak/login`, `GET /api/v1/langileak/nire-profila`, `GET /api/v1/txostenak/auditoria-segurua` | Login-ak HS256 token sinatua itzultzen du (`BCrypt` + auto-migrazioa). Tokenik gabe **`401 Unauthorized`**, `Zerbitzaria` rolarekin **`403 Forbidden`**, `Admin/AdminIT/Kudeatzailea` rolarekin **`200 OK`**. |
| **4** | **Analitika agregatu konplexua** | `GET /api/v1/txostenak/laburpena?hasiera=...&amaiera=...&top=5` (`TxostenakController.cs:24`) | NHibernate LINQ multi-taula agregazioak: Top N plater salduenak, ordainketak motaka, langileen fakturazioa, eguneko bilakaera, batez besteko tiketa eta biltegiko balio ekonomikoa (`7.426,03 €`). |
| **5** | **Bilaketa, iragazketa, ordenazio eta paginazio dinamikoa** | `GET /api/v1/txostenak/platerak-bilatu?search=...&minPrezioa=...&sortBy=prezioa&sortDir=desc&page=1&pageSize=5` (`TxostenakController.cs:142`) | SQL paginazio erreala (`Skip`/`Take` + `CountAsync`, `totalItems`/`totalPages`) + `v_plateren_stock_ahalmena` ikuspegitik anoa libreak bateratuta N+1 gabe. |
| **6** | **Batch transakzio atomikoa (`komanda-osoa`)** | `POST /api/v1/eskaerak/komanda-osoa` (`EskaerakController.cs:194-386`) | Mahai batean komanda ireki + hainbat plater batera gehitu eta stock metatua deskontatu. Azken platerak stockik ez badu, **Rollback osoa (`409 Conflict`)**: mahaia `Libre` geratzen da eta ez da ezer ukitzen. |
| **7** | **Ordainketa zatitua + Faktura automatikoa** | `POST /api/v1/ordainketak/zatikatu` (`OrdainketakController.cs:242-490`) | Kontu bera hainbat ordainketa-motatan zatitu (`Eskudirua` + `Txartela` + `Bizum`), azpi-tiketak gorde eta `fakturak` taulan salmenta-faktura ofiziala (`Sinplifikatua`/`Osoa`, %10 BEZ) sortu transakzio bakarrean. |
| **8** | **Konkurrentzia pesimista + Deadlock prebentzioa** | `POST /api/v1/erreserbak` (`itxaron=ms`), `POST /api/v1/eskaerak/{id}/lerroak` | `IsolationLevel.ReadCommitted` + `LockMode.Upgrade` (`SELECT ... FOR UPDATE`), lehengaiak ID gorakorrean blokeatuta (`OrderBy(LehengaiaId)`), eta `1062`/`1451`/`1213` kontrolatuta. |
| **9** | **5 plataforma ezberdin API berera + Monitorea** | `GET /api/v1/aplikazioak-linean`, `GET /health` | C# WinForms, JavaFX Core, Kotlin (Android TV & Tablet), Flutter (Dart) eta PHP (`cURL`) API berera konektatuta; denbora errealeko jarraipena + `503` tolerantzia DBa erortzean. |

---

## 3. Bideo Aurkezpenaren Gidoi Zehatza (Guztira: 6 min 20 seg — 7 min pasa gabe!)

### 1. BLOKEA — Sarrera, Arkitektura eta Swagger UI (`0:00 – 0:45` · 45 seg)
- **Pantailan:** `http://localhost:5050/` eta `http://localhost:5050/swagger` (Swagger UI interaktiboa).
- **Azalpena:**
  - *"Kaixo! 1T Jatetxea proiektuko ASP.NET Core (.NET 10) REST API zentrala aurkeztuko dut. 3 mailako arkitektura dugu: bezero bakar batek ere ez du zuzenean MySQL-ra konektatzen; 5 teknologia ezberdinetako aplikazioak API honen bidez komunikatzen dira, eta datu-atzipen guztia **NHibernate 5.7 + FluentNHibernate 3.5** bidez dago egina. Hemen ikusten duzue `/swagger` interfaze interaktiboa 38 endpoint-ekin."*

### 2. BLOKEA — FluentNHibernate ORM Barne-Egitura (`0:45 – 1:55` · 1 min 10 seg)
- **Pantailan (IDEan fitxategiak zabalik):**
  1. `Data/NHibernateHelper.cs` + `Data/MySqlConnectorDriver.cs`: `ISessionFactory` Singleton, `MySQL8InnoDBDialect`, `AddFromAssemblyOf<BezeroaMap>()`.
  2. `Models/DomeinuEreduak.cs` + `Mappings/PlaterOsagaiaMap.cs` + `Mappings/PlaterStockAhalmenaMap.cs`: propietate `public virtual`, `CompositeId` gako konposatua eta MySQL `VIEW` `ReadOnly()` mapeoa.
  3. `Controllers/BaseApiKontroladorea.cs`: `IrekiSaioa()`, `ApiResponse<T>` / `ApiError`, eta `LortuMySqlSalbuespena()` (`1062`, `1451`, `1213`).
- **Azalpena:**
  - *"Ez dago SQL gordin bakar bat ere kontroladoreetan. Saio bakoitza `using` blokean irekitzen da, `.Fetch()` erabiltzen dugu N+1 kontsultak saihesteko eta DTO-etara saio barruan bihurtzen dugu `LazyInitializationException` eragozteko."*

### 3. BLOKEA — API Dei Konplexuak, Transakzioak, Konkurrentzia eta JWT ZUZENEAN (`1:55 – 4:25` · 2 min 30 seg)
- **A) Analitika + Paginazio dinamikoa (`TxostenakController.cs`) — 35 seg:**
  - Exekutatu `GET /api/v1/txostenak/laburpena?top=5` (Top 5 platerak, ordainketak motaka, langileen salmentak, biltegiaren balioa) eta `GET /api/v1/txostenak/platerak-bilatu?sortBy=prezioa&sortDir=desc&page=1&pageSize=5` (`totalItems`, `totalPages`, `eginDaitezkeenAnoak`).
- **B) JWT Bearer + Rol bidezko baimenak (`401` / `403` / `200`) — 35 seg:**
  - Exekutatu 4.1 ataleko komandoa: `GET /api/v1/txostenak/auditoria-segurua` tokenik gabe (**`401`**) → login `Zerbitzaria` rolarekin (**`403`**) → login `AdminIT` rolarekin (**`200 OK`**, azken stock mugimenduak).
- **C) Batch Transakzio Atomikoa (`POST /api/v1/eskaerak/komanda-osoa`) + Rollback — 40 seg:**
  - Erakutsi `EskaerakController.cs:194-295` (`IsolationLevel.ReadCommitted`, `LockMode.Upgrade` ID gorakorrean deadlock-ak saihesteko) eta exekutatu 4.2 ataleko demoa: 2 plater eskatu (2.ak stocka gainditzen du) → **`409 Conflict` + Rollback osoa**, mahaia `Libre` geratzen da!
- **D) Konkurrentzia-proba ZUZENEAN (`POST /api/v1/erreserbak`) + Ordainketa Zatitua (`zatikatu`) — 40 seg:**
  - Exekutatu 4.3 ataleko PowerShell komando paraleloa (`itxaron=1500`): bi eskaera aldi berean → bata **`201 Created`** eta bestea **`409 Conflict`**.
  - Aipatu labur `POST /api/v1/ordainketak/zatikatu` (`OrdainketakController.cs:242`): kontua eskudiru + txartelean zatitu eta `fakturak` taulan faktura ofiziala sortzen du transakzio bakarrean.

### 4. BLOKEA — 5 Aplikazio Ezberdinetatik Konexioa Nola Egiten Den (`4:25 – 5:50` · 1 min 25 seg)
- **Pantailan (fitxa bakoitza 10-15 segundoz + `/api/v1/aplikazioak-linean`):**
  1. **C# WinForms TPV** — [ApiService.cs](../../../01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp/Services/ApiService.cs): `HttpClient`, `X-App-Id: csharp_tpv`, `X-Langilea` header-ak eta `System.Text.Json`.
  2. **Java / JavaFX Core** — [ApiZerbitzua.java](../../../01_aplikazioak/admin/komunak/desktop_javafx_core/src/main/java/eus/goierri/jatetxea/core/api/ApiZerbitzua.java): `java.net.http.HttpClient` + `api.properties` (Admin, Sukaldea, Barra eta Biltegia aplikazioek modulu bera partekatzen dute).
  3. **Flutter Mugikorra (Dart)** — [api_service.dart](../../../01_aplikazioak/bezeroak/mobile_bezeroa_flutter/lib/services/api_service.dart) eta **Android TV KDS (Kotlin)** — [JatetxeaApiService.kt](../../../01_aplikazioak/sukalde_langileak/sukaldea_kds_android_tv/app/src/main/java/eus/goierri/jatetxea/sukaldea/data/api/JatetxeaApiService.kt): `package:http` + `SharedPreferences` IP dinamikoa; Kotlin KDS-ak eskaeren egoerak eguneratzen ditu.
  4. **Web Publikoa (PHP)** — [api.php](../../../01_aplikazioak/bezeroak/web_php_bezeroak/assets/php/api.php): `cURL` bidezko `call_api()`, Docker sareko `jatetxea-api:5000` eta fallback automatikoa `host.docker.internal:5050`.
  5. **Zuzeneko froga:** Ireki `http://localhost:5050/api/v1/aplikazioak-linean` eta erakutsi nola APIak denbora errealean identifikatzen dituen konektatutako bezero guztiak.

### 5. BLOKEA — Osasun-Monitorea eta Itxiera (`5:50 – 6:20` · 30 seg)
- **Pantailan:** `http://localhost:5050/health` eta `Program.cs` 503 middleware-a.
- **Azalpena:**
  - *"`DatuBaseaMonitorZerbitzua`-k 3 segundoro egiaztatzen du MySQL eta eroriz gero `503 Service Unavailable` JSON garbia itzultzen du. Dokumentazioan 33 transakzioen zerrenda, `ReadCommitted` isolamendu-maila eta 9 konkurrentzia-egoerak daude azalduta. Eskerrik asko!"*

---

## 4. Bideoan Zuzenean Exekutatzeko Demo Komandoak (PowerShell — Kopiatu eta Itsatsi)

### 4.1. Analitika + Paginazioa + JWT RBAC (`401` / `403` / `200`)

```powershell
$base = "http://localhost:5050"

# 1. Analitika agregatua (NHibernate LINQ)
(Invoke-RestMethod "$base/api/v1/txostenak/laburpena?top=5").data | Select-Object eskaeraKopurua, fakturazioGuztira, batezBestekoTiketa, biltegiBalioGuztira, stockAlertaKopurua | Format-List

# 2. Bilaketa eta paginazio dinamikoa
(Invoke-RestMethod "$base/api/v1/txostenak/platerak-bilatu?sortBy=prezioa&sortDir=desc&page=1&pageSize=5").data.items | Format-Table id, izena, kategoria, prezioa, eginDaitezkeenAnoak

# 3. JWT Segurtasuna: tokenik gabe -> 401 Unauthorized
try { Invoke-WebRequest "$base/api/v1/txostenak/auditoria-segurua" -UseBasicParsing } catch { Write-Host "Tokenik gabe HTTP:" $_.Exception.Response.StatusCode.value__ }
```

### 4.2. Batch Transakzio Atomikoa (`komanda-osoa` — Rollback `409 Conflict`)

```powershell
$base = "http://localhost:5050"
$m = ((Invoke-RestMethod "$base/api/v1/mahaiak").datalist | Where-Object { $_.egoera -eq "Libre" } | Select-Object -First 1)
$body = @{ mahaiaId=$m.id; langileaId=1; pertsonaKopurua=2; lerroak=@(@{plateraId=1;kantitatea=1}, @{plateraId=1;kantitatea=100}) } | ConvertTo-Json -Depth 4
try {
  Invoke-RestMethod -Method Post -Uri "$base/api/v1/eskaerak/komanda-osoa" -ContentType "application/json" -Body $body
} catch {
  Write-Host "Batch Rollback HTTP:" $_.Exception.Response.StatusCode.value__
  (New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())).ReadToEnd()
}
Write-Host "Mahaiaren egoera Rollback ondoren:" (Invoke-RestMethod "$base/api/v1/mahaiak/$($m.id)").data.egoera
```

### 4.3. Konkurrentzia-proba ZUZENEAN: Bi erreserba aldi berean (`201` vs `409`)

```powershell
$body = '{"bezeroaId":1,"mahaiaId":1,"data":"2027-07-20","txanda":"Afaria_1","pertsonaKopurua":2,"itxaron":1500,"oharrak":"Konkurrentzia demo"}'
$j1 = Start-Job { param($b) try { (Invoke-WebRequest -Method Post -Uri "http://localhost:5050/api/v1/erreserbak" -ContentType "application/json" -Body $b -UseBasicParsing).StatusCode } catch { $_.Exception.Response.StatusCode.value__ } } -ArgumentList $body
$j2 = Start-Job { param($b) try { (Invoke-WebRequest -Method Post -Uri "http://localhost:5050/api/v1/erreserbak" -ContentType "application/json" -Body $b -UseBasicParsing).StatusCode } catch { $_.Exception.Response.StatusCode.value__ } } -ArgumentList $body
Wait-Job $j1, $j2 | Out-Null
Write-Host "1. Bezeroa HTTP:" (Receive-Job $j1)
Write-Host "2. Bezeroa HTTP:" (Receive-Job $j2)
```
*(Demoaren ondoren garbitu proba-erreserba `DELETE /api/v1/erreserbak/{id}?hard=true` bidez).*

---

## 5. Grabatu Aurreko Checklist Azkarra

- [ ] Martxan: `jatetxea_mysql` (`:3308`), `jatetxea_api` (`:5050`), `jatetxea_web_php` (`:8085`).
- [ ] Nabigatzailean prest: `http://localhost:5050/swagger`, `http://localhost:5050/health`, `http://localhost:5050/api/v1/aplikazioak-linean`.
- [ ] IDEan fitxak prest: `NHibernateHelper.cs`, `PlaterOsagaiaMap.cs`, `TxostenakController.cs`, `EskaerakController.cs`, `OrdainketakController.cs`, `ApiService.cs` (C#), `ApiZerbitzua.java` (JavaFX), `api_service.dart` (Flutter), `api.php` (PHP).
- [ ] Kronometroa bistan: **6:00–6:30 min** (inoiz ez 7:00 gainditu!).
