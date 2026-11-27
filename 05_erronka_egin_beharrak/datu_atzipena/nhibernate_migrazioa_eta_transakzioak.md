# Datu Atzipena — REST APIaren Migrazioa FluentNHibernate ORM-era, Transakzioak eta Konkurrentzia

**Proiektua:** 1T Jatetxea — Erronka Integrala (Goierri Eskola)  
**Osagaia:** `01_aplikazioak/admin/backend_rest_api_csharp` (`Jatetxea.Api`, ASP.NET Core `.NET 10`)  
**Datu-basea:** MySQL 8.0 InnoDB (`jatetxea_db`, 3308 portua)  
**ORM teknologia:** `NHibernate 5.7.1` + `FluentNHibernate 3.5.0` + `MySqlConnector 2.6.2`  
**Data:** 2026-11-27

---

## 1. Helburua eta Laburpen Exekutiboa

Dokumentu honek `backend_rest_api_csharp` REST APIaren datu-atzipen geruza SQL gordinetik (`MySqlCommand` / `MySqlDataReader`) **NHibernate + FluentNHibernate ORM** arkitekturara nola migratu den zehazten du, **Datuen Atzipena** moduluko ebaluazio-irizpide guztiak betez:

1. **Instalazioa eta konfigurazioa:** NuGet paketeak, `MySqlConnectorDriver`, `MySQL8InnoDBDialect`, `NHibernateHelper` (`ISessionFactory` Singleton) eta Dependency Injection integrazioa.
2. **Aldaketen zerrenda osoa:** Aurreko eta oraingo arkitekturaren alderaketa eta fitxategi guztien zerrenda.
3. **Modeloak eta Mapeoak:** 13 entitate nagusi + 2 MySQL ikuspegi (`VIEW`) FluentNHibernate `ClassMap<T>` bidez mapatuta.
4. **Transakzioen zerrenda osoa:** APIko transakzio guztiak, eragindako taulak, blokeoak eta kode-erreferentzia zehatzak.
5. **Isolamendu-maila:** Aukeratutako maila (`IsolationLevel.ReadCommitted`), noiz aplikatzen den eta justifikazio teknikoa.
6. **Konkurrentzia-egoerak:** Aldi bereko sarbide-gatazka guztiak (stock negatiboa, erreserba bikoitza, kobrantza bikoitza, deadlock-ak, katalogo-blokeoa...) eta kodeko konponbideak.
7. **Egiaztapen-probak:** Zuzenean `jatetxea_api` (Docker, 5050 portua) eta `jatetxea_mysql` (3308 portua) aurka exekutatutako emaitzak.

---

## 2. Instalazioa eta Konfigurazioa

### 2.1. NuGet paketeak (`Jatetxea.Api.csproj`)

NHibernate ez da sistema eragilean instalatzen den programa autonomo bat; .NET proiektuan NuGet pakete gisa integratzen den ORM liburutegia da. Ondorengo komandoekin instalatu da:

```powershell
cd 01_aplikazioak\admin\backend_rest_api_csharp
dotnet add package NHibernate
dotnet add package FluentNHibernate
```

Proiektuan aktibo dauden paketeak (`01_aplikazioak/admin/backend_rest_api_csharp/Jatetxea.Api.csproj`):

| Paketea | Bertsioa | Funtzioa |
|---|---|---|
| `NHibernate` | `5.7.1` | ORM nukleoa (`ISessionFactory`, `ISession`, `ITransaction`, LINQ, blokeoak) |
| `FluentNHibernate` | `3.5.0` | C# kode bidezko mapaketa tipatua (`ClassMap<T>`, `Fluently.Configure()`) |
| `MySqlConnector` | `2.6.2` | MySQL 8.0 driver asinkronoa eta errendimendu handikoa |
| `BCrypt.Net-Next` | `4.0.3` | Langileen PIN/pasahitzen hash zifraketa |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `10.0.0` | JWT Bearer (HS256) autentifikazioa eta Rol bidezko baimenak (`RBAC`) |
| `Swashbuckle.AspNetCore.SwaggerUI` | `11.0.1` | OpenAPI / Swagger UI dokumentazio interaktiboa (`/swagger`) |

### 2.2. Driver eta Dialektoaren konfigurazioa

- **`Data/MySqlConnectorDriver.cs`**: `NHibernate.Driver.ReflectionBasedDriver` hedatzen du proiektuan dagoeneko instalatuta dagoen `MySqlConnector` liburutegia erabiltzeko (`MySqlConnection` / `MySqlCommand`, `?` parametro-aurrizkia, `SupportsMultipleQueries = true`).
- **`Data/NHibernateHelper.cs`**:
  - `ISessionFactory` **behin bakarrik** eraikitzen du (Singleton patroia, `lock` bidez hari-segurua).
  - Konexio-katea `DB_CONNECTION_STRING` ingurune-aldagaitik (Docker) edo `appsettings.json`-eko `ConnectionStrings:Jatetxea` gakoetik hartzen du.
  - `MySQLConfiguration.Standard.Driver<MySqlConnectorDriver>().Dialect<MySQL8InnoDBDialect>()` konfiguratzen du.
  - `m.FluentMappings.AddFromAssemblyOf<BezeroaMap>()` bidez `Mappings/` karpetako `ClassMap<T>` guztiak automatikoki kargatzen ditu.
  - `Hbm2ddlKeyWords = "none"` ezartzen du APIa abiarazi dadin MySQL behin-behinean itzalita badago ere (`DatuBaseaMonitorZerbitzua`-rekin bateragarri).
  - Ez du `SchemaExport` edo `SchemaUpdate` automatikorik exekutatzen abiaraztean, `jatetxea_db`-k jada `ENUM`, `CHECK`, `FOREIGN KEY` eta `VIEW` propioak dituelako.
- **`Program.cs`**:
  - `builder.Services.AddSingleton<ISessionFactory>(_ => NHibernateHelper.SortuSessionFactory(builder.Configuration));` bidez DI edukiontzian erregistratzen da.
  - Middleware globalak `GenericADOException` (NHibernate) eta `MySqlException` harrapatzen ditu, DBa erorita badago `503 Service Unavailable` JSON garbia itzuliz.

---

## 3. Aplikazioan Egindako Aldaketen Laburpena

### 3.1. Aurretik vs. Ondoren

| Arloa | Aurretik (ADO.NET / SQL gordina) | Ondoren (NHibernate + FluentNHibernate) |
|---|---|---|
| Konexioa | `await SortuKonexioaAsync()` (`MySqlConnection`) | `using var session = IrekiSaioa();` (`ISessionFactory.OpenSession()`) |
| Irakurketak | `MySqlCommand` + `MySqlDataReader` eskuzko mapaketa | `session.Query<T>()` (LINQ) eta `session.GetAsync<T>(id)` |
| JOIN eta N+1 prebentzioa | SQL `JOIN` manualak | `.Fetch(x => x.Entitatea)` (Eager loading saio barruan) |
| Sortu / Eguneratu / Ezabatu | `INSERT`, `UPDATE`, `DELETE` SQL kateak | `SaveAsync(obj)`, `UpdateAsync(obj)`, `DeleteAsync(obj)` + dirty checking |
| Transakzioak | `MySqlTransaction` | `using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);` |
| Blokeo pesimistak | `SELECT ... FOR UPDATE` SQL eskuz | `session.GetAsync<T>(id, LockMode.Upgrade)` / `.WithLock(LockMode.Upgrade)` |
| Erroreen kudeaketa | `catch (MySqlException ex)` | `LortuMySqlSalbuespena(ex)` (`GenericADOException` -> `MySqlException`) |
| Kontroletako SQL gordina | 10 kontroladoretan sakabanatuta | **0** `MySqlCommand` / `MySqlConnection` kontroladore guztietan |

> **API kontratua % 100 mantenduta:** Endpoint bideak, HTTP kodeak, JSON egiturak (`ApiResponse<T>` / `ApiError`), bezero guztiekin bateragarritasuna (C# TPV WinForms, JavaFX, Android TV KDS, Flutter, Web PHP) ez dira apurtu.

### 3.2. Fitxategi berriak eta aldatuak

**Fitxategi berriak (20 fitxategi):**
- `Data/MySqlConnectorDriver.cs` — NHibernate driverra `MySqlConnector`-erako.
- `Data/NHibernateHelper.cs` — `ISessionFactory` Singleton eta konfigurazio zentrala.
- `Helpers/JwtHelper.cs` — HS256 JWT token sorkuntza eta balidazioa (`sub`, `name`, `role`, `saila`).
- `DTO/TxostenaDto.cs` — Analitika agregatua, paginazioa eta auditoria seguruaren DTOak.
- `Controllers/TxostenakController.cs` — Analitika (`laburpena`), bilaketa paginatua (`platerak-bilatu`) eta RBAC auditoria (`auditoria-segurua`).
- `Mappings/BezeroaMap.cs` (`bezeroak`)
- `Mappings/MahaiaMap.cs` (`mahaiak`)
- `Mappings/LangileaMap.cs` (`langileak`)
- `Mappings/LehengaiaMap.cs` (`lehengaiak`)
- `Mappings/PlateraMap.cs` (`platerak`)
- `Mappings/PlaterOsagaiaMap.cs` (`plater_osagaiak`, gako konposatua)
- `Mappings/ErreserbaMap.cs` (`erreserbak`)
- `Mappings/EskaeraMap.cs` (`eskaerak`)
- `Mappings/EskaeraLerroaMap.cs` (`eskaera_lerroak`)
- `Mappings/StockMugimenduaMap.cs` (`stock_mugimenduak`)
- `Mappings/IritziaMap.cs` (`iritziak`)
- `Mappings/OrdainketaMap.cs` (`ordainketak`)
- `Mappings/FakturaMap.cs` (`fakturak`)
- `Mappings/ErosiBeharrekoLehengaiaMap.cs` (`v_erosi_beharreko_lehengaiak` VIEW, `ReadOnly()`)
- `Mappings/PlaterStockAhalmenaMap.cs` (`v_plateren_stock_ahalmena` VIEW, `ReadOnly()`)

**Aldatutako fitxategiak (13 fitxategi):**
- `Jatetxea.Api.csproj` — `NHibernate 5.7.1` eta `FluentNHibernate 3.5.0` gehituta.
- `Program.cs` — `ISessionFactory` DI erregistroa eta `GenericADOException` kudeaketa.
- `Models/DomeinuEreduak.cs` — Propietate guztiak `public virtual`, erlazio-propietateak (`References` / `IList<T>`), `Ordainketa`, `Faktura` eta ikuspegien modeloak gehituta, `PlaterOsagaia` `Equals`/`GetHashCode` gako konposaturako.
- `Controllers/BaseApiKontroladorea.cs` — `ISessionFactory`, `IrekiSaioa()`, `LortuMySqlSalbuespena()`, `RollbackSeguruaAsync()`, `LortuZerbitzuEgoeraAsync()` (LINQ).
- `Controllers/BezeroakController.cs`
- `Controllers/MahaiakController.cs`
- `Controllers/LangileakController.cs`
- `Controllers/LehengaiakController.cs`
- `Controllers/PlaterakController.cs`
- `Controllers/ErreserbakController.cs`
- `Controllers/EskaerakController.cs`
- `Controllers/OrdainketakController.cs`
- `Controllers/IritziakController.cs`
- `Controllers/FakturakController.cs`
- `Controllers/SystemController.cs`

---

## 4. Modeloak eta FluentNHibernate Mapeoak

### 4.1. Entitateen eta taulen korrespondentzia

| Entitatea (`Models/DomeinuEreduak.cs`) | Mapeoa (`Mappings/`) | MySQL Taula / Ikuspegia | Gako Nagusia | Erlazio Nagusiak |
|---|---|---|---|---|
| `Bezeroa` | `BezeroaMap` | `bezeroak` | `id` (Identity) | `HasMany`: `Erreserbak`, `Iritziak` (`Inverse`, `LazyLoad`) |
| `Mahaia` | `MahaiaMap` | `mahaiak` | `id` (Identity) | `HasMany`: `Erreserbak`, `Eskaerak` (`Inverse`, `LazyLoad`) |
| `Langilea` | `LangileaMap` | `langileak` | `id` (Identity) | `HasMany`: `Eskaerak` (`Inverse`, `LazyLoad`) |
| `Lehengaia` | `LehengaiaMap` | `lehengaiak` | `id` (Identity) | `HasMany`: `PlaterOsagaiak`, `StockMugimenduak` |
| `Platera` | `PlateraMap` | `platerak` | `id` (Identity) | `HasMany`: `Osagaiak`, `EskaeraLerroak` |
| `PlaterOsagaia` | `PlaterOsagaiaMap` | `plater_osagaiak` | `CompositeId(platera_id, lehengaia_id)` | `References`: `Platera`, `Lehengaia` (read-only FK) |
| `Erreserba` | `ErreserbaMap` | `erreserbak` | `id` (Identity) | `References`: `Bezeroa`, `Mahaia` |
| `Eskaera` | `EskaeraMap` | `eskaerak` | `id` (Identity) | `References`: `Mahaia`, `Langilea`; `HasMany`: `Lerroak`, `Ordainketak` |
| `EskaeraLerroa` | `EskaeraLerroaMap` | `eskaera_lerroak` | `id` (Identity) | `References`: `Eskaera`, `Platera` |
| `StockMugimendua` | `StockMugimenduaMap` | `stock_mugimenduak` | `id` (Identity) | `References`: `Lehengaia`, `Eskaera` (nullable) |
| `Iritzia` | `IritziaMap` | `iritziak` | `id` (Identity) | `References`: `Bezeroa` |
| `Ordainketa` | `OrdainketaMap` | `ordainketak` | `id` (Identity) | `References`: `Eskaera` |
| `Faktura` | `FakturaMap` | `fakturak` | `id` (Identity) | `References`: `Eskaera` (nullable) |
| `ErosiBeharrekoLehengaia` | `ErosiBeharrekoLehengaiaMap` | `v_erosi_beharreko_lehengaiak` | `lehengaia_id` (Assigned) | `ReadOnly()`, `SchemaAction.None()` |
| `PlaterStockAhalmena` | `PlaterStockAhalmenaMap` | `v_plateren_stock_ahalmena` | `platera_id` (Assigned) | `ReadOnly()`, `SchemaAction.None()` |

### 4.2. NHibernate kode-ordena eta praktika onak

- **Propietate birtualak:** Entitate guztien propietateak `public virtual` dira, NHibernate-k proxy dinamikoak sortu eta lazy loading-a ondo exekuta dezan.
- **Saioaren bizi-zikloa:** Kontroladore-metodo bakoitzak `using var session = IrekiSaioa();` irekitzen du, datuak saio barruan DTO-etara bihurtzen ditu (`ToListAsync()` + `Select(...)`), eta saioa itxi aurretik materializatzen du **`LazyInitializationException`** guztiz saihestuz.
- **N+1 arazoaren prebentzioa:** Zerrendatze-kontsultetan `.Fetch(x => x.Mahaia).Fetch(x => x.Langilea)` erabiltzen da (`ErreserbakController`, `EskaerakController`, `IritziakController`, `PlaterakController`, `OrdainketakController`), `LEFT JOIN` bakar batean datu guztiak ekartzeko.

---

## 5. Transakzioen Zerrenda Osoa (Kode-erreferentziekin)

Jarraian API osoan kudeatzen diren **33 transakzio esplizituak** ageri dira, fitxategi eta lerro-zenbaki zehatzekin:

### 5.1. Eskaerak eta Stocka (`EskaerakController.cs`)

| # | Endpoint-a | Metodoa | Kode-erreferentzia | Isolamendua | Blokeoak (`LockMode.Upgrade`) | Eragindako taulak eta helburua |
|---|---|---|---|---|---|---|
| 1 | `POST /api/v1/eskaerak` | `Sortu` | `EskaerakController.cs:114-184` | `ReadCommitted` | `Mahaia` (`L119`) | Mahaia egiaztatu/blokeatu, egoera `Okupatuta` jarri eta eskaera sortu atomikoki. |
| 1b | `POST /api/v1/eskaerak/komanda-osoa` | `SortuKomandaOsoa` | `EskaerakController.cs:194-386` | `ReadCommitted` | `Mahaia` + beharrezko `Lehengaia` guztiak **ID gorakorrean** | **BATCH TRANSAKZIO KONPOSATUA:** Komanda ireki + hainbat plater batera gehitu eta stock metatua deskontatu. Osagai bakar bat falta bada, **komanda osoa eta stock guztiak Rollback** (`409 Conflict`). |
| 2 | `POST /api/v1/eskaerak/{id}/lerroak` | `GehituPlateraEtaDeskontatuStock` | `EskaerakController.cs:201-323` | `ReadCommitted` | `Eskaera` (`L206`), `Lehengaia` guztiak ID gorakorrean (`L245`) | **GAKO TRANSAKZIOA:** Stock nahikoa egiaztatu, lehengaiak deskontatu, `stock_mugimenduak` auditoria erregistratu, `eskaera_lerroak` gehitu eta `eskaerak.guztira` eguneratu. Stockik ez badago `Rollback`. |
| 3 | `PUT /api/v1/eskaerak/{id}/egoera` | `EguneratuEgoera` | `EskaerakController.cs:342-409` | `ReadCommitted` | `Eskaera` (`L344`), `Mahaia` (`L400`) | Eskaera eta sukaldeko/zerbitzatutako lerroen egoerak sinkronizatu; kobratzean mahaia `Garbitzen` jarri beste konturik ez badago. |
| 4 | `POST /api/v1/eskaerak/{id}/sukaldera` | `BidaliSukaldera` | `EskaerakController.cs:420-458` | `ReadCommitted` | `Eskaera` (`L422`), `Mahaia` (`L443`) | Eskaera `Sukaldean`, mahaia `Okupatuta` eta sukaldeko lerroak (edariak izan ezik) `Sukaldean` jarri batera. |
| 5 | `PUT /api/v1/eskaerak/lerroak/{lerroId}/egoera` | `EguneratuLerroEgoera` | `EskaerakController.cs:482-542` | `ReadCommitted` | `EskaeraLerroa` (`L484`), `Eskaera` (`L495`), `Mahaia` (`L506`) | KDS/BDS lerro-egoera aldatu eta automatikoki eskaera gurasoaren egoera (`Sukaldean`, `Prestatua`, `Zerbitzatua`) eguneratu. |
| 6 | `DELETE /api/v1/eskaerak/lerroak/{lerroId}` | `EzabatuLerroa` | `EskaerakController.cs:554-631` | `ReadCommitted` | `EskaeraLerroa` (`L559`), `Eskaera` (`L579`), `Lehengaia` ID gorakorrean (`L589`) | Sukaldera bidali gabeko lerroa ezabatu, deskontatutako stocka lehengaietara itzuli, auditoria erregistratu eta `guztira` murriztu. |
| 7 | `DELETE /api/v1/eskaerak/{id}` | `Ezabatu` | `EskaerakController.cs:642-656` | `ReadCommitted` | `Eskaera` (`L644`) | Eskaeraren soft delete (`ezabatua = 1`, `egoera = 'Ezeztatua'`). |

### 5.2. Ordainketak eta Tiketak (`OrdainketakController.cs`)

| # | Endpoint-a | Metodoa | Kode-erreferentzia | Isolamendua | Blokeoak (`LockMode.Upgrade`) | Eragindako taulak eta helburua |
|---|---|---|---|---|---|---|
| 8 | `POST /api/v1/ordainketak` | `ProzesatuOrdainketa` | `OrdainketakController.cs:32-233` | `ReadCommitted` | `Eskaera` (`L37`), `Mahaia` (`L59`) | **KOBRANTZA TRANSAKZIOA:** Ordainketa balidatu, 80mm tiket fiskala gorde (`ordainketak`), eskaera `Kobratua` jarri eta mahai bereko azken kontua bada `Garbitzen` egoerara pasatu. |
| 8b | `POST /api/v1/ordainketak/zatikatu` | `ProzesatuOrdainketaZatitua` | `OrdainketakController.cs:242-490` | `ReadCommitted` | `Eskaera`, `Mahaia` (`LockMode.Upgrade`) | **ORDAINKETA ZATITUA + FAKTURA:** Kontu bera hainbat metodotan zatitu (`Eskudirua` + `Txartela` + `Bizum`), tiket azpi-zenbakiak gorde, `fakturak` taulan salmenta-faktura ofiziala (`Sinplifikatua`/`Osoa`) sortu eta eskaera/mahaia itxi atomikoki. |

### 5.3. Erreserbak (`ErreserbakController.cs`)

| # | Endpoint-a | Metodoa | Kode-erreferentzia | Isolamendua | Blokeoak (`LockMode.Upgrade`) | Eragindako taulak eta helburua |
|---|---|---|---|---|---|---|
| 9 | `POST /api/v1/erreserbak` | `Sortu` | `ErreserbakController.cs:120-227` | `ReadCommitted` | `Mahaia` (`L158`), `Erreserba` (`L180`) | **ERRESERBA PESIMISTA:** Bezeroa sortu/lotu, mahaia eta data+txanda blokeatu, aforoa eta eskuragarritasuna egiaztatu eta erreserba sortu (`1062`/`1213` kontrolatuta). |
| 10 | `DELETE /api/v1/erreserbak/{id}` | `Ezabatu` | `ErreserbakController.cs:238-267` | `ReadCommitted` | `Erreserba` (`L240`) | Erreserba behin betiko (`hard`) edo logikoki (`soft`) ezeztatu. |
| 11 | `PUT /api/v1/erreserbak/{id}/restore` | `Berreskuratu` | `ErreserbakController.cs:277-317` | `ReadCommitted` | `Erreserba` (`L281`), `Mahaia` (`L289`) | Mahaia blokeatu, hutsunea oraindik libre dagoela egiaztatu eta ezeztatutako erreserba berreskuratu. |

### 5.4. Platerak eta Errezetak (`PlaterakController.cs`)

| # | Endpoint-a | Metodoa | Kode-erreferentzia | Isolamendua | Blokeoak (`LockMode.Upgrade`) | Eragindako taulak eta helburua |
|---|---|---|---|---|---|---|
| 12 | `POST /api/v1/platerak` | `Sortu` | `PlaterakController.cs:144-198` | `ReadCommitted` | — (zerbitzua itxita aurrez egiaztatua) | Platera (`platerak`) eta bere errezetako osagai guztiak (`plater_osagaiak`) atomikoki sortu. |
| 13 | `PUT /api/v1/platerak/{id}` | `Eguneratu` | `PlaterakController.cs:212-305` | `ReadCommitted` | `Mahaia` aktiboak (`L220`), `Platera` (`L232`), `Lehengaia` ID gorakorrean (`L246`) | Zerbitzua itxita dagoela bermatuz plateraren datuak eta errezeta osoa atomikoki ordezkatu. |
| 14 | `DELETE /api/v1/platerak/{id}` | `Ezabatu` | `PlaterakController.cs:320-358` | `ReadCommitted` | `Platera` (`L326` / `L346`) | Platera soft/hard delete (`1451` FK kontrolarekin). |
| 15 | `PUT /api/v1/platerak/{id}/restore` | `Berreskuratu` | `PlaterakController.cs:373-388` | `ReadCommitted` | `Platera` (`L375`) | Desgaitutako platera berreskuratu. |

### 5.5. Lehengaiak eta Biltegia (`LehengaiakController.cs`)

| # | Endpoint-a | Metodoa | Kode-erreferentzia | Isolamendua | Blokeoak (`LockMode.Upgrade`) | Eragindako taulak eta helburua |
|---|---|---|---|---|---|---|
| 16 | `POST /api/v1/lehengaiak` | `Sortu` | `LehengaiakController.cs:109-160` | `ReadCommitted` | — | Lehengaia sortu eta hasierako stocka > 0 bada `stock_mugimenduak` erregistroa sortu batera. |
| 17 | `PUT /api/v1/lehengaiak/{id}` | `Eguneratu` | `LehengaiakController.cs:175-225` | `ReadCommitted` | `Lehengaia` (`L179`) | Lehengaiaren datuak eguneratu, unitate-aldaketa errezeta/historiarik badu blokeatuz. |
| 18 | `POST /api/v1/lehengaiak/sarrera` | `ErregistratuSarrera` | `LehengaiakController.cs:238-291` | `ReadCommitted` | `Lehengaia` (`L243`) | Albaran sarrera: stocka gehitu (`uneko_stocka`) eta `Sarrera_Albarana` auditoria sortu atomikoki. |
| 19 | `DELETE /api/v1/lehengaiak/{id}` | `Ezabatu` | `LehengaiakController.cs:301-348` | `ReadCommitted` | `Lehengaia` (`L307` / `L327`) | Errezeta aktiboetan erabiltzen ez dela egiaztatuz soft delete edo `hard` delete (`1451`). |
| 20 | `PUT /api/v1/lehengaiak/{id}/restore` | `Berreskuratu` | `LehengaiakController.cs:358-373` | `ReadCommitted` | `Lehengaia` (`L360`) | Desgaitutako lehengaia berreskuratu. |

### 5.6. Mahaiak (`MahaiakController.cs`)

| # | Endpoint-a | Metodoa | Kode-erreferentzia | Isolamendua | Blokeoak (`LockMode.Upgrade`) | Eragindako taulak eta helburua |
|---|---|---|---|---|---|---|
| 21 | `POST /api/v1/mahaiak` | `Sortu` | `MahaiakController.cs:160-192` | `ReadCommitted` | — | Mahai berria sortu (`1062` zenbaki bikoiztua kontrolatuz). |
| 22 | `PUT /api/v1/mahaiak/{id}` | `Eguneratu` | `MahaiakController.cs:209-269` | `ReadCommitted` | `Mahaia` (`L213`) | Mahaia libre, eskaerarik gabe eta erreserba aktiborik gabe dagoela egiaztatuz eguneratu. |
| 23 | `PUT /api/v1/mahaiak/{id}/egoera` | `EguneratuEgoera` | `MahaiakController.cs:283-313` | `ReadCommitted` | `Mahaia` (`L285`) | `Libre`/`Garbitzen` jarri aurretik kobratu gabeko lerro aktiborik ez dagoela egiaztatu. |
| 24 | `DELETE /api/v1/mahaiak/{id}` | `Ezabatu` | `MahaiakController.cs:327-365` | `ReadCommitted` | `Mahaia` (`L333` / `L353`) | Mahaia soft/hard delete (`1451` FK kontrola). |
| 25 | `PUT /api/v1/mahaiak/{id}/restore` | `Berreskuratu` | `MahaiakController.cs:380-395` | `ReadCommitted` | `Mahaia` (`L382`) | Desgaitutako mahaia berreskuratu. |

### 5.7. Langileak, Bezeroak, Iritziak eta Fakturak

| # | Kontroladorea eta Metodoa | Endpoint-a | Kode-erreferentzia | Blokeoak | Helburua |
|---|---|---|---|---|---|
| 26 | `LangileakController.SaioaHasi` | `POST /api/v1/langileak/login` | `LangileakController.cs:113-116` | — | Testu lauko PIN zaharrak automatikoki BCrypt hash-era migratzea saio-hasieran. |
| 27 | `LangileakController.Sortu / Eguneratu / Desaktibatu / MigratuHashak` | `POST/PUT/DELETE /api/v1/langileak` | `LangileakController.cs:163, 220, 272, 297` | `Langilea` (`L224`, `L274`) | Langileen CRUD osoa (`1062` unique) eta BCrypt migrazio masiboa transakzio bakarrean. |
| 28 | `BezeroakController.Sortu / Eguneratu / Ezabatu` | `POST/PUT/DELETE /api/v1/bezeroak` | `BezeroakController.cs:79, 122, 157` | `Bezeroa` (`L126`, `L163`, `L183`) | Bezeroen CRUD atomikoa (`1062` emaila, `1451` FK). |
| 29 | `IritziakController.Sortu` | `POST /api/v1/iritziak` | `IritziakController.cs:88-157` | — | Bezero berria (behar bada) eta iritzia transakzio berean gordetzea. |
| 30 | `IritziakController.Ezabatu` | `DELETE /api/v1/iritziak/{id}` | `IritziakController.cs:166-196` | `Iritzia` (`L168`) | Iritziaren soft/hard delete. |
| 31 | `FakturakController.Sortu / EguneratuEgoera / Ezabatu` | `POST/PUT/DELETE /api/v1/fakturak` | `FakturakController.cs:103, 154, 177` | `Faktura` (`L156`, `L179`) | Salmenta/erosketa fakturen sorrera (`1062`), egoera-aldaketa eta ezabaketa. |

---

## 6. Isolamendu-mailaren Aukeraketa eta Justifikazioa

### 6.1. Zein isolamendu-maila aukeratu da?

Idazketa- eta blokeo-transakzio guztietan **`IsolationLevel.ReadCommitted`** aukeratu da, eragiketa kritikoetan **blokeo pesimistarekin** (`LockMode.Upgrade` -> `SELECT ... FOR UPDATE`) konbinatuta:

```csharp
using var session = IrekiSaioa();
using var tx = session.BeginTransaction(IsolationLevel.ReadCommitted);
```

### 6.2. Noiz erabiltzen da?

- **Irakurketa soilak (`GET` endpoint-ak):** `ISession` irekitzen da transakzio idazlerik gabe (InnoDB-ren snapshot irakurketa ez-blokeatzailea), KDS pantailek, Web kartak eta TPV-ak elkarri trabarik egin gabe irakur dezaten.
- **Idazketa sinpleak (`POST` / `PUT` / `DELETE`):** `BeginTransaction(IsolationLevel.ReadCommitted)` erabiltzen da atomikotasuna (dena edo ezer ez) eta rollback segurua bermatzeko.
- **Idazketa kritiko konkurrenteak (Stocka, Erreserbak, Ordainketak, Katalogoa):** `BeginTransaction(IsolationLevel.ReadCommitted)` + `LockMode.Upgrade` (`SELECT ... FOR UPDATE`) erabiltzen da irakurri-egiaztatu-idatzi zikloa serializatzeko ukitutako errenkadetan soilik.

### 6.3. Zergatik `ReadCommitted` + `LockMode.Upgrade`? (Alternatiben konparazioa)

| Isolamendu-maila | Dirty Read | Non-Repeatable Read | Phantom Read | Zergatik aukeratu / baztertu den T1 Jatetxean |
|---|---|---|---|---|
| `ReadUncommitted` | Bai | Bai | Bai | **Baztertua:** Beste TPV batek rollback egin dezakeen stocka edo kobrantza irakurriko luke (datu faltsuak). |
| **`ReadCommitted` + `LockMode.Upgrade`** | **Ez** | **Kontrolatua** (`FOR UPDATE`) | **Kontrolatua** (`UNIQUE` + `FOR UPDATE`) | **AUKERATUA:** Irakurketa zikinak eragozten ditu, `FOR UPDATE` blokeoak errenkada zehatza soilik blokeatzen du (InnoDB-n gap-lock deadlock-ak drastikoki murriztuz), eta beti datu baieztatu freskoena irakurtzen du blokeoa hartzean. |
| `RepeatableRead` (MySQL lehenetsia) | Ez | Ez | Ia ez (InnoDB) | **Baztertua transakzio kritikoetan:** MVCC snapshot zaharra irakur dezake transakzio barruko aurreko SELECT batean, eta `FOR UPDATE` egitean InnoDB-k *gap lock* zabalak jartzen ditu, aldi bereko TPV eskaeretan deadlock arriskua handituz. |
| `Serializable` | Ez | Ez | Ez | **Baztertua:** Irakurketa guztiak blokeatzen ditu (`LOCK IN SHARE MODE`), jatetxeko puntako orduetan (hainbat TPV, KDS, Web eta Flutter aldi berean) errendimendu-botila-lepoa eta timeout-ak sortuz. |

---

## 7. Konkurrentzia-egoera Guztiak (Kode-erreferentziekin)

Jatetxe batean hainbat bezero-aplikazio ari dira aldi berean API beraren aurka (TPV C# terminalak, Sukaldeko JavaFX / Android TV KDS pantailak, Web PHP publikoa, Flutter mugikorra eta Biltegiko kudeaketa). Ondorengo **9 konkurrentzia-egoera** identifikatu eta konpondu dira kodean:

### 1. egoera: Aldi bereko plater-eskaerak eta stock negatiboa (*Lost Update*)
- **Arazoa:** Bi zerbitzarik aldi berean azken txuleta (0,5 kg stock geratzen da) bi mahaitarako eskatzen dute. Bi transakzioek `0,5 kg` irakurriko balute blokeorik gabe, biek onartuko lukete eskaera eta stocka `-0,5 kg` (edo eguneraketa galdua) geratuko litzateke.
- **Konponbidea:** `EskaerakController.GehituPlateraEtaDeskontatuStock` (`EskaerakController.cs:201-278`):
  - `session.GetAsync<Eskaera>(id, LockMode.Upgrade)` (`L206`) eskaera blokeatzen du.
  - `session.GetAsync<Lehengaia>(po.LehengaiaId, LockMode.Upgrade)` (`L245`) errezetako lehengai bakoitza blokeatzen du (`SELECT ... FOR UPDATE`).
  - Bigarren zerbitzaria itxaroten geratzen da lehenengoak `CommitAsync()` egin arte; ondoren stock eguneratua (`0,0 kg`) irakurtzen du, egiaztapenak huts egiten du (`L255-263`), `RollbackSeguruaAsync(tx)` exekutatzen da eta `409 Conflict` itzultzen da.

### 2. egoera: Deadlock prebentzioa hainbat osagai aldi berean blokeatzean
- **Arazoa:** A platerak 1. eta 2. lehengaiak behar ditu; B platerak 2. eta 1. lehengaiak. Bi eskaerak kontrako ordenean blokeatuz gero, blokeo gurutzatua (*Deadlock*, MySQL `1213`) gertatuko litzateke.
- **Konponbidea:**
  - **Blokeo-ordena determinista:** Lehengai guztiak `OrderBy(po => po.LehengaiaId)` ordenean (ID gorakorrean) blokeatzen dira hiru lekuetan:
    - `EskaerakController.cs:239` (`GehituPlateraEtaDeskontatuStock`)
    - `EskaerakController.cs:584` (`EzabatuLerroa`)
    - `PlaterakController.cs:241` (`Eguneratu`)
  - **Deadlock salbuespenaren kudeaketa:** Hala ere MySQL `1213` gertatuko balitz, `catch (Exception ex) when (LortuMySqlSalbuespena(ex)?.Number == 1213)` blokeak rollback garbia egin eta `409 Conflict` mezua itzultzen du (`EskaerakController.cs:313`, `EskaerakController.cs:622`, `ErreserbakController.cs:218`, `OrdainketakController.cs:224`, `PlaterakController.cs:301`).

### 3. egoera: Mahai beraren erreserba bikoitza (*Double Booking*)
- **Arazoa:** Bi bezerok webetik edo telefonoz aldi berean egun eta txanda bererako (adib. `2027-06-15`, `Afaria_2`) 1. mahaia erreserbatzen dute.
- **Konponbidea:** `ErreserbakController.Sortu` (`ErreserbakController.cs:120-227`):
  - `session.GetAsync<Mahaia>(mahaiaId, LockMode.Upgrade)` (`L158`) mahaia blokeatzen du.
  - `session.Query<Erreserba>().Where(...).WithLock(LockMode.Upgrade)` (`L178-181`) txanda horretako erreserba egiaztatzen du.
  - Bigarren defentsa-geruza gisa, datu-baseko `UNIQUE KEY uq_erreserba_mahaia_data_txanda (mahaia_id, data, txanda)` murrizketak `1062` salbuespena jaurtitzen du, eta APIak `409 Conflict` itzultzen du (`L213-217`).
  - Gainera, `dto.Itxaron` parametroa (`L190-193`) mantendu da konkurrentzia-probak artifizialki simulatu ahal izateko.

### 4. egoera: Ezeztatutako erreserba berreskuratzea erreserba berri batekin batera
- **Arazoa:** Langile batek ezeztatutako erreserba berreskuratzen duen segundo berean, bezero batek mahai bera erreserbatzen du.
- **Konponbidea:** `ErreserbakController.Berreskuratu` (`ErreserbakController.cs:277-317`) — `LockMode.Upgrade` aplikatzen du bai `Erreserba` (`L281`) bai `Mahaia` (`L289`) errenkadetan, eta hutsunea libre dagoela egiaztatzen du commit aurretik.

### 5. egoera: Eskaera beraren kobrantza bikoitza (*Double Charge*)
- **Arazoa:** Bi zerbitzarik TPV ezberdinetatik mahai beraren ordainketa botoia sakatzen dute aldi berean (edo klik bikoitza egiten da).
- **Konponbidea:** `OrdainketakController.ProzesatuOrdainketa` (`OrdainketakController.cs:32-233`):
  - `session.GetAsync<Eskaera>(dto.EskaeraId, LockMode.Upgrade)` (`L37`) eskaera blokeatzen du; bigarren transakzioak `eskaera.Egoera == "Kobratua"` (`L48-52`) ikusiko du eta `409 Conflict` jasoko du.
  - Bigarren defentsa-geruza gisa, `ordainketak.tiket_zenbakia` zutabeak `UNIQUE` murrizketa du (`1062` -> `409 Conflict`, `L219-223`).

### 6. egoera: Lerro bat ezabatzea sukaldeko KDS egoera-aldaketarekin batera
- **Arazoa:** Zerbitzariak plater bat ezabatu nahi du TPV-tik, sukaldariak KDS pantailatik plater hori `Sukaldean` egoerara pasatzen duen une berean.
- **Konponbidea:** Bi metodoek `EskaeraLerroa` errenkada bera blokeatzen dute `LockMode.Upgrade` bidez:
  - `EskaerakController.EguneratuLerroEgoera` (`EskaerakController.cs:484`)
  - `EskaerakController.EzabatuLerroa` (`EskaerakController.cs:559`)
  - Sukaldaria lehenago sartzen bada, zerbitzariak `lerroa.Egoera == "Sukaldean"` ikusiko du eta `409 Conflict` jasoko du (`L569-575`), stocka oker itzultzea eragotziz.

### 7. egoera: Albaran sarrera eta plater-kontsumoa aldi berean biltegian
- **Arazoa:** Biltegikoak hornitzailearen albarana sartzen du (+10 kg) TPV-ak osagai bera deskontatzen duen bitartean (-0,5 kg). Blokeorik gabe, azken idazketak bestearen aldaketa zapalduko luke (*Lost Update*).
- **Konponbidea:** `LehengaiakController.ErregistratuSarrera` (`LehengaiakController.cs:243`) eta `EskaerakController.GehituPlateraEtaDeskontatuStock` (`EskaerakController.cs:245`) biek `LockMode.Upgrade` eskatzen dute `Lehengaia` errenkadan; horrela bi eguneraketak sekuentzialki eta zehaztasun osoz metatzen dira (+9,5 kg).

### 8. egoera: Katalogoa edo mahaiak aldatzea zerbitzua irekitzearekin batera
- **Arazoa:** Kudeatzaileak plater baten errezeta/prezioa editatzen duen bitartean, zerbitzari batek mahai batean komanda berria irekitzen du.
- **Konponbidea:** `PlaterakController.Eguneratu` (`PlaterakController.cs:216-232`) metodoak mahai aktibo guztiak blokeatzen ditu (`WithLock(LockMode.Upgrade)` ID ordenean) eta zerbitzua itxita dagoela egiaztatzen du (`LortuZerbitzuEgoeraAsync`). Aldi berean `EskaerakController.Sortu` (`L119`) metodoak `Mahaia` blokeatzen duenez, bi eragiketak elkarren artean serializatzen dira eta ez da egoera inkonsistenterik sortzen.

### 9. egoera: Erregistro beraren edizio/ezabaketa konkurrentea eta UNIQUE/FK murrizketak
- **Arazoa:** Bi administratzailek aldi berean bezero, langile, lehengai edo mahai bera editatzen/ezabatzen dute, edo balio errepikatuak (`emaila`, `nan_dni`, `zenbakia`, `faktura_zenbakia`) sartzen dituzte.
- **Konponbidea:**
  - `PUT` eta `DELETE` eragiketa guztietan `session.GetAsync<T>(id, LockMode.Upgrade)` erabiltzen da (`BezeroakController`, `LangileakController`, `LehengaiakController`, `MahaiakController`, `PlaterakController`, `FakturakController`, `IritziakController`).
  - `BaseApiKontroladorea.LortuMySqlSalbuespena(ex)` (`BaseApiKontroladorea.cs:37-45`) metodoak NHibernate-ren `GenericADOException` barruko MySQL kodeak bereizten ditu:
    - `1062` (Unique bikoiztua) -> `409 Conflict`
    - `1451` / `1452` (Foreign Key murrizketa) -> `409 Conflict`
    - `1213` (Deadlock) -> `409 Conflict`

---

## 8. Egiaztapen Probak eta Emaitzak

Migrazioaren ondoren ondorengo egiaztapenak burutu dira `Jatetxea.Api` proiektuan eta Docker ingurunean (`jatetxea_api` 5050 portuan, `jatetxea_mysql` 3308 portuan):

1. **Konpilazioa (`dotnet build -c Release`):** **0 errore, 0 abisu**.
2. **Docker irudiaren berreraikuntza (`docker compose up -d --build jatetxea-api`):** Irudia garbi eraiki eta abiarazi da.
3. **Kontroladoreetako SQL gordinaren auditoria:** `Controllers/*.cs` fitxategietan `MySqlCommand` / `MySqlConnection` agerpenak = **0**.
4. **Endpoint guztien probak (18 GET endpoint + CRUD + Konkurrentzia):**
   - `GET /`, `/health`, `/api/v1/system/service-status` -> `200 OK` (`orm: "NHibernate 5.7 + FluentNHibernate 3.5 (MySQL8InnoDBDialect)"`).
   - `GET` zerrendak eta ikuspegiak (`bezeroak`, `mahaiak`, `mahaiak/disponibleak`, `langileak`, `lehengaiak`, `lehengaiak/alertak`, `platerak`, `platerak/erabilgarritasuna`, `platerak/1/eskandalloa`, `erreserbak`, `eskaerak`, `eskaerak/52`, `iritziak`, `iritziak/estatistikak`, `fakturak`, `fakturak/laburpena`) -> **guztiak `200 OK`**.
   - **CRUD ziklo osoa (`Bezeroak`):** `POST` (`201`) -> `GET/{id}` (`200`) -> `PUT/{id}` (`200`) -> `DELETE` soft (`200`) -> `DELETE?hard=true` (`200`).
   - **Konkurrentzia / Erreserba bikoitzaren proba (`Erreserbak`):** 1. `POST` (`201 Created`) -> 2. `POST` berbera (`409 Conflict`, blokeo pesimista + `UNIQUE` aktibo) -> `DELETE?hard=true` (`200 OK`, garbituta).
5. **10/10 Goi-mailako endpoint berrien probak (guztiak egiaztatuta):**
   - **Swagger UI + OpenAPI:** `GET /swagger/index.html` (`200 OK`) eta `GET /openapi/v1.json` (`200 OK`, 72 KB).
   - **Analitika agregatua:** `GET /api/v1/txostenak/laburpena?top=5` -> `200 OK` (28 eskaera, 1930,60 € fakturazioa, 7426,03 € biltegi-balioa, Top 5 platerak eta 3 ordainketa-mota).
   - **Bilaketa eta paginazio dinamikoa:** `GET /api/v1/txostenak/platerak-bilatu?sortBy=prezioa&sortDir=desc&page=1&pageSize=5` -> `200 OK` (`totalItems: 64`, `totalPages: 13`, `eginDaitezkeenAnoak` bateratuta).
   - **JWT Bearer + RBAC segurtasuna:** `GET /api/v1/txostenak/auditoria-segurua` -> tokenik gabe **`401 Unauthorized`**, `Zerbitzaria` rolarekin **`403 Forbidden`**, `Admin` rolarekin **`200 OK`**; `GET /api/v1/langileak/nire-profila` -> **`200 OK`**.
   - **Batch transakzio atomikoa (`POST /api/v1/eskaerak/komanda-osoa`):** Stock nahikorik gabe -> **`409 Conflict`** + Rollback osoa (mahaia `Libre` geratzen da); stockarekin -> **`201 Created`**.
   - **Ordainketa zatitua + Faktura automatikoa (`POST /api/v1/ordainketak/zatikatu`):** 2 zati (`Eskudirua` + `Txartela`) + `Osoa` faktura automatikoa (`F2026-S00053-92`) -> **`201 Created`** transakzio bakar batean.
