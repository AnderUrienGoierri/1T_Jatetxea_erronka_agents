# E2 — Datu-Basearen Diseinua, Aplikazioaren Aukeraketa eta 5 Minutuko Bideo-Gidoia

**Modulua:** Datuen Atzipena — E2 Entregagaia  
**Helburua:** **5 minutuko** bideoan `jatetxea_db` (MySQL 8.0 InnoDB) datu-basearen diseinua azaltzea, **aplikazio baten funtzionamenduarekin zuzenean erlazionatuz**, hartutako **diseinu-erabakiak justifikatuz** eta **transakzioak non aplikatu diren** erakutsiz.

---

## 1. Zein aplikazio aukeratu? (Konparazioa eta Gomendioa)

Proiektuko aplikazioen artean transakzioak eta datu-basearen muina hobekien erakusten dituzten hautagaiak aztertu dira:

| Aplikazioa | Ukitutako taulak + VIEW-ak | Transakzio kritikoak | Balorazioa E2-rako |
|---|---|---|---|
| **1. `C# TPV Areto & Kutxazaina`** (`desktop_tpv_csharp`) | **10 taula + 1 VIEW** (`langileak`, `mahaiak`, `erreserbak`, `eskaerak`, `eskaera_lerroak`, `platerak`, `plater_osagaiak`, `lehengaiak`, `stock_mugimenduak`, `ordainketak`, `v_plateren_stock_ahalmena`) | **4 transakzio nagusi:** Komanda ireki, Platera gehitu + Stock deskontatu (6 taula + Rollback), Lerroa ezabatu + Stock itzuli, Kobrantza + Tiket fiskala | **AUKERARIK APROPOSENA (% 100 gomendatua)** |
| **2. `Web PHP Bezeroak`** (`web_php_bezeroak`, `:8085`) | 6 taula + 2 VIEW (`platerak`, `mahaiak`, `bezeroak`, `erreserbak`, `iritziak`, `v_plateren_stock_ahalmena`, `v_balorazio_orokorrak`) | 1 transakzio kritiko nagusi (`POST /erreserbak` blokeo pesimista + `UNIQUE`) | Oso ona taldeko 2. kide batek erreserbak azaltzeko, baina stocka eta kobrantzak kanpoan geratzen dira. |
| **3. `JavaFX Admin / Biltegia`** (`admin_desktop_java` / `desktop_biltegia_stock`) | `platerak`, `plater_osagaiak`, `lehengaiak`, `stock_mugimenduak`, `fakturak`, `v_erosi_beharreko_lehengaiak` | 2 transakzio (`PUT /platerak/{id}` errezeta, `POST /lehengaiak/sarrera` albarana) | Egokia taldeko 3. kidearentzat (backoffice eta biltegia). |

### Zergatik da **`C# TPV Areto & Kutxazaina`** aukerarik onena?

1. **Datu-basearen % 75+ erakusten du fluxu natural bakar batean:** *Login → Mahaien mapa → Komanda ireki → Platera eskatu (errezeta + stock deskontua + auditoria) → Stock gabezia / Rollback → Lerroa ezabatu (stock itzulera) → Kobrantza + Tiket fiskala*.
2. **Transakzio konplexuenak bertan daude:** Batez ere `POST /api/v1/eskaerak/{id}/lerroak` (**6 taula** transakzio pesimista bakarrean) eta `POST /api/v1/ordainketak` (kobrantza bikoitza eragoztea + tiket fiskala).
3. **Diseinu-erabaki guztiak zuzenean lotzen dira UI-arekin:** Gako konposatua (`plater_osagaiak`), prezio historikoaren kopia (`eskaera_lerroak.unitate_prezioa`), `VIEW` dinamikoa (`v_plateren_stock_ahalmena`), `CHECK (uneko_stocka >= 0)`, `ENUM` egoera-makina, `Soft Delete` eta `BCrypt`/PCI-DSS maskaratzea.
4. **Bisualki oso indartsua da:** TPV-an klik egin eta segidan `phpMyAdmin`-en (`http://localhost:8081`) errenkada berriak eta stockaren jaitsiera zuzenean erakuts daitezke.

---

## 2. Datu-Basearen Diseinua ↔ C# TPV Funtzionamenduaren Lotura

Taula honek erakusten du TPV-ko ekintza bakoitzak zein taula eta erlazio aktibatzen dituen ([01_egitura_schema.sql](../../../db/01_egitura_schema.sql), [02_hasierako_datuak.sql](../../../db/02_hasierako_datuak.sql) eta [EER_diagarama_datu_basea.pdf](../../../04_dokumentazioa/garapen_diagramak/EER_diagarama_datu_basea.pdf)):

| TPV-ko urratsa (Zer egiten du zerbitzariak) | Ukitutako taulak / VIEW-ak | Erlazioa (E/E) | Zer gertatzen da DBan |
|---|---|---|---|
| **1. PIN bidez saioa hasi** | `langileak` | — | `erabiltzailea` (`UNIQUE`) eta `pasahitza_hash` (`BCrypt`) egiaztatzen dira (`aktibo = 1`, `ezabatua = 0`). |
| **2. Mahaien egoera ikusi** | `mahaiak`, `eskaerak`, `erreserbak` | `mahaiak 1:N eskaerak` · `mahaiak 1:N erreserbak` | Mahai bakoitzaren `egoera` (`Libre`, `Okupatuta`, `Erreserbatuta`, `Garbitzen`) eta edukiera kolorez margotzen dira. |
| **3. Mahai batean komanda ireki** | `mahaiak`, `eskaerak`, `langileak` | `langileak 1:N eskaerak` · `mahaiak 1:N eskaerak` | **Transakzioa:** `mahaiak.egoera` → `Okupatuta`, eta `eskaerak` taulan erregistro berria (`egoera = 'Irekia'`, `guztira = 0.00`). |
| **4. Karta eta anoa libreak ikusi** | `platerak`, `v_plateren_stock_ahalmena` | `platerak N:M lehengaiak` (`plater_osagaiak` bidez) | `VIEW`-ak denbora errealean kalkulatzen du osagai mugatzailearen arabera zenbat anoa egin daitezkeen (`FLOOR(MIN(uneko_stocka / kantitatea))`). |
| **5. Platera komandara gehitu** | `eskaerak`, `platerak`, `plater_osagaiak`, `lehengaiak`, `stock_mugimenduak`, `eskaera_lerroak` | `eskaerak 1:N eskaera_lerroak N:1 platerak` · `lehengaiak 1:N stock_mugimenduak` | **Gako-transakzioa:** Errezeta irakurri, lehengaiak blokeatu eta deskontatu, `Kontsumoa_Platera` auditoria idatzi, lerroa gehitu eta `eskaerak.guztira` eguneratu. |
| **6. Platera kendu (sukaldera bidali gabe)** | `eskaera_lerroak`, `eskaerak`, `plater_osagaiak`, `lehengaiak`, `stock_mugimenduak` | Berdin | **Transakzioa:** Lerroa ezabatu, deskontatutako stocka lehengaietara itzuli (`Egokitzapena_Inbentarioa`) eta `guztira` murriztu. |
| **7. Kontua kobratu eta tiketa inprimatu** | `ordainketak`, `eskaerak`, `mahaiak` | `eskaerak 1:N ordainketak` | **Transakzioa:** `ordainketak` sortu (`tiket_zenbakia UNIQUE`, `tiket_testua` 80mm), `eskaerak.egoera` → `Kobratua`, eta mahai berean beste konturik ez badago `mahaiak.egoera` → `Garbitzen`. |

---

## 3. Diseinuan Hartutako Erabaki Nagusiak eta Arrazoiak (Bideoan justifikatzekoak)

Hauek dira 10/10 nota lortzeko argi eta labur azaldu behar diren **8 diseinu-erabaki teknikoak**:

1. **Motorra `InnoDB` eta `utf8mb4`:**
   - *Zergatik:* `InnoDB` derrigorrezkoa da ACID transakzioak, `FOREIGN KEY` osotasuna eta errenkada-mailako `SELECT ... FOR UPDATE` blokeoak edukitzeko (MyISAM-ek ez ditu onartzen). `utf8mb4`-k euskarazko karaktereak eta `€` sinboloa arazorik gabe gordetzen ditu.
2. **N:M erlazioa gako nagusi konposatuarekin (`plater_osagaiak`):**
   - `PRIMARY KEY (platera_id, lehengaia_id)` + `kantitatea DECIMAL(10,3)`.
   - *Zergatik:* Plater batek lehengai asko ditu eta lehengai bera plater askotan erabiltzen da. Gako konposatuak osagai bera plater berean bikoiztea eragozten du; `DECIMAL(10,3)`-k gramoak/ml-ak zehaztasunez gordetzen ditu (`0.350 kg`).
3. **Prezio historikoa gordetzea (`eskaera_lerroak.unitate_prezioa`):**
   - *Zergatik:* Nahiz eta `platerak.prezioa` existitu, lerroa sortzean uneko prezioa kopiatzen da `eskaera_lerroak`-era (beti backend-etik irakurrita, inoiz ez bezeroaren eskaeratik). Horrela, bihar kartako prezioa igotzen bada, **iraganeko eskaera eta tiket fiskalen totala ez da inoiz aldatzen**.
4. **Diru-balioetan `DECIMAL` (`DECIMAL(6,2)`, `DECIMAL(8,2)`, `DECIMAL(10,2)`):**
   - *Zergatik:* `FLOAT`/`DOUBLE` motek biribiltze-errore bitarrak ematen dituzte kontabilitatean eta BEZ kalkuluetan.
5. **Soft Delete (`ezabatua`, `ezabatze_data`) + `ON DELETE RESTRICT`:**
   - *Zergatik:* Jatetxeko lege- eta kontabilitate-trazabilitatea mantentzeko ez dira benetan ezabatzen platerak, mahaiak, langileak edo bezeroak; `ezabatua = 1` markatzen dira. Gainera, FK guztiek `ON DELETE RESTRICT` dute istripuz historiala ez apurtzeko.
6. **Defentsa bikoitza datu-baseko murrizketekin (`CHECK`, `UNIQUE`, `ENUM`):**
   - `CHECK (uneko_stocka >= 0)`, `CHECK (kantitatea > 0)`, `CHECK (edukiera > 0)` → aplikazioan akatsik balego ere, MySQL-k berak eragozten du stock negatiboa.
   - `UNIQUE (tiket_zenbakia)` (`ordainketak`), `UNIQUE (mahaia_id, data, txanda)` (`erreserbak`), `UNIQUE (zenbakia)` (`mahaiak`) → bikoizketak DB mailan blokeatuta.
   - `ENUM(...)` egoera-fluxu kontrolatuetarako (`Irekia → Sukaldean → Prestatua → Zerbitzatua → Kobratua`).
7. **Stock auditoria-taula bereizia (`stock_mugimenduak`):**
   - *Zergatik:* `lehengaiak.uneko_stocka` gainidazteaz gain, aldaketa bakoitza (`Sarrera_Albarana`, `Kontsumoa_Platera`, `Egokitzapena_Inbentarioa`) data eta `eskaera_id`-arekin erregistratzen da, desfaseak edo merma auditatzeko.
8. **MySQL `VIEW` (`v_plateren_stock_ahalmena`) + Indizeak (`INDEX`) + Segurtasuna:**
   - `VIEW`-ak kalkulu logika DBan zentralizatzen du (TPV, Web PHP eta Flutter-ek formula bera erabiltzen dute, eta FluentNHibernate-k `ReadOnly()` gisa mapatzen du).
   - Indize konposatuek (`idx_eskaera_mahaia`, `idx_lehengai_stock`, `idx_mahai_egoera`) TPV-aren karga bizkortzen dute.
   - Segurtasuna: `langileak.pasahitza_hash` (`BCrypt`), `ordainketak.txartel_maskaratua` (`**** **** **** 1234`, inoiz ez txartel osoa).

---

## 4. Transakzioak Non Aplikatu Diren (C# TPV Fluxuan)

Transakzio guztiak **NHibernate `ITransaction` (`IsolationLevel.ReadCommitted`)** eta **blokeo pesimista (`LockMode.Upgrade` → `SELECT ... FOR UPDATE`)** bidez kudeatzen dira:

| # | Non aplikatzen da (TPV ekintza) | Endpoint-a eta Kodea | Blokeatutako errenkadak | Zer bermatzen du (Atomikotasuna + Konkurrentzia) |
|---|---|---|---|---|
| **T1** | **Komanda berria irekitzea** | `POST /api/v1/eskaerak` (`EskaerakController.cs:114-184`) | `Mahaia` (`LockMode.Upgrade`) | Mahaia ez dagoela `Garbitzen` egiaztatu, `Okupatuta` jarri eta `eskaerak` sortu batera; errorea bada rollback. |
| **T2** | **Platera gehitzea eta stocka deskontatzea (GAKOA)** | `POST /api/v1/eskaerak/{id}/lerroak` (`EskaerakController.cs:201-323`) | `Eskaera` + `Lehengaia` guztiak **ID gorakorrean** (`OrderBy(LehengaiaId)`) | Bi zerbitzarik aldi berean azken anoa eskatzen badute, lehenengoak blokeatzen du; bigarrenak stock eguneratua ikusi eta `409 Conflict` + `Rollback` jasotzen du. ID gorakorreko blokeoak **deadlock-ak** prebenitzen ditu. |
| **T3** | **Sukaldera bidali gabeko lerroa ezabatzea** | `DELETE /api/v1/eskaerak/lerroak/{lerroId}` (`EskaerakController.cs:554-631`) | `EskaeraLerroa` + `Eskaera` + `Lehengaia` (ID gorakorrean) | Lerroa oraindik `Eskatua` dela egiaztatu (sukaldariak KDS-n `Sukaldean` jarri badu, `409`), stocka itzuli, auditoria erregistratu, `guztira` jaitsi eta lerroa ezabatu atomikoki. |
| **T4** | **Kontua kobratzea eta tiketa sortzea** | `POST /api/v1/ordainketak` (`OrdainketakController.cs:32-233`) | `Eskaera` + `Mahaia` (`LockMode.Upgrade`) | Kobrantza bikoitza eragozten du (`egoera == 'Kobratua'` + `UNIQUE tiket_zenbakia`), tiketa gorde, eskaera itxi eta mahai berean beste eskaerarik ez badago mahaia `Garbitzen` jartzen du. |

> **Isolamendu-maila (`ReadCommitted` + `LockMode.Upgrade`):** Irakurketa zikinak (*Dirty Reads*) eragozten ditu, `FOR UPDATE` blokeoak errenkada zehatza soilik blokeatzen du (InnoDB-ko *gap-lock* deadlock-ak saihestuz), eta blokeoa hartzean beti azken balio baieztatua irakurtzen du.

---

## 5. 5 Minutuko Bideo-Gidoi Zehatza (Helburua: 4 min 50 seg — 5:00 pasa gabe!)

| Denbora | Atala | Zer erakutsi pantailan | Zer azaldu (ideia gakoak) |
|---|---|---|---|
| **0:00 – 0:45** *(45s)* | **1. E/E Diagrama eta Ikuspegi Orokorra** | Ireki [EER_diagarama_datu_basea.pdf](../../../04_dokumentazioa/garapen_diagramak/EER_diagarama_datu_basea.pdf) (edo phpMyAdmin Designer). | *"Kaixo! `jatetxea_db` (MySQL 8.0 InnoDB) datu-basearen diseinua **C# TPV Areto & Kutxazaina** aplikazioarekin erlazionatuta azalduko dut, TPV-ak ukitzen baititu datu-baseko taula eta transakzio kritikoenak. 13 taula nagusi eta 3 ikuspegi ditugu, 3. Forma Normalean."* Seinalatu 3 blokeak: Katalogoa (`platerak`–`plater_osagaiak`–`lehengaiak`), Zerbitzua (`mahaiak`–`eskaerak`–`eskaera_lerroak`–`langileak`) eta Finantzak/Auditoria (`ordainketak`, `stock_mugimenduak`). |
| **0:45 – 1:45** *(1m)* | **2. Diseinu-erabaki Gakoak** | Erakutsi labur [01_egitura_schema.sql](../../../db/01_egitura_schema.sql) (4 puntu azkar): `lehengaiak`, `plater_osagaiak`, `eskaera_lerroak`, `v_plateren_stock_ahalmena`. | Justifikatu 5 erabaki nagusiak: (1) **`InnoDB`** ACID transakzio eta `FOR UPDATE`-rako; (2) **`plater_osagaiak` gako konposatua** `(platera_id, lehengaia_id)` N:M errezetarako + `DECIMAL(10,3)`; (3) **`eskaera_lerroak.unitate_prezioa`** prezio historikoa mantentzeko; (4) **`CHECK (uneko_stocka >= 0)`**, `UNIQUE` eta **`Soft Delete` (`ezabatua`) + `ON DELETE RESTRICT`** datu historikorik ez galtzeko; (5) **`v_plateren_stock_ahalmena` VIEW-a** stock erabilgarria kalkulatzeko. |
| **1:45 – 3:15** *(1m 30s)* | **3. Zuzeneko Demoa TPV-an + DBa Aldatzen Ikusi** | Alde batean **C# TPV** irekita, bestean **phpMyAdmin (`:8081`)**. | 1) **PIN Login** → `langileak` (`BCrypt`).<br>2) **Mahai bat aukeratu eta komanda ireki** → **T1 Transakzioa** (`mahaiak.egoera = 'Okupatuta'` + `eskaerak` sortu).<br>3) **Plater bat gehitu** → **T2 Gako-transakzioa**: erakutsi phpMyAdmin-en nola jaitsi den `lehengaiak.uneko_stocka`, sortu den `stock_mugimenduak` (`Kontsumoa_Platera`), gehitu den `eskaera_lerroak` eta eguneratu den `eskaerak.guztira`.<br>4) **Stockik gabeko platera (edo kantitate handiegia) probatu** → TPV-ak `409 Conflict` abisua ematen du eta DBan **Rollback** egin denez stocka ez da ukitu! |
| **3:15 – 4:15** *(1m)* | **4. Lerroa Ezabatu + Kobrantza eta Tiketa** | **C# TPV** + **phpMyAdmin** (`ordainketak`, `eskaerak`, `mahaiak`). | 1) **Sukaldera bidali gabeko lerroa ezabatu** → **T3 Transakzioa**: stocka automatikoki itzultzen da `lehengaiak`-era (`Egokitzapena_Inbentarioa`).<br>2) **Kontua kobratu (Txartela/Eskudirua)** → **T4 Transakzioa**: `ordainketak` errenkada berria (`tiket_zenbakia UNIQUE`, txartela maskaratuta `****`, %10 BEZ eta 80mm tiketa `MEDIUMTEXT`-en), `eskaerak.egoera = 'Kobratua'` eta `mahaiak.egoera = 'Garbitzen'`. |
| **4:15 – 4:50** *(35s)* | **5. Transakzioen Kodea eta Isolamendu-maila** | Ireki 15 segundoz `EskaerakController.cs:201-278` (FluentNHibernate). | *"Transakzio guztiak APIan **NHibernate** bidez daude inplementatuta, **`IsolationLevel.ReadCommitted`** isolamendu-mailarekin eta **`LockMode.Upgrade` (`SELECT ... FOR UPDATE`)** blokeo pesimistarekin. Lehengaiak **ID gorakorrean** blokeatzen ditugu deadlock-ak saihesteko. Horrela, hainbat TPV aldi berean ari direnean ere ez da inoiz stock negatiborik, kobrantza bikoitzik edo datu inkonsistenterik sortzen. Eskerrik asko!"* |

---

## 6. Grabatu Aurreko Checklist Azkarra (5 min ez gainditzeko)

- [ ] Martxan edukitzea: `jatetxea_mysql` (`:3308`), `jatetxea_api` (`:5050`), `jatetxea_phpmyadmin` (`:8081`) eta **C# TPV** (`Jatetxea.Desktop.exe`).
- [ ] Irekita edukitzea:
  1. [EER_diagarama_datu_basea.pdf](../../../04_dokumentazioa/garapen_diagramak/EER_diagarama_datu_basea.pdf)
  2. [01_egitura_schema.sql](../../../db/01_egitura_schema.sql) eta [02_hasierako_datuak.sql](../../../db/02_hasierako_datuak.sql)
  3. `http://localhost:8081` (`phpMyAdmin` → `eskaerak`, `lehengaiak`, `stock_mugimenduak`, `ordainketak` fitxak prest)
  4. `EskaerakController.cs` (`L201-L278`)
- [ ] **Aholku garrantzitsua:** Ez galdu denborarik phpMyAdmin-en saioa hasten bideoan — sartu aurretik egon dadila irekita, eta bakarrik sakatu *Refresh* taulan aldaketak berehala ikusteko!
- [ ] Kronometroa bistan: **4:45–4:55** helburu, **5:00** mugaren barruan lasai geratzeko.
