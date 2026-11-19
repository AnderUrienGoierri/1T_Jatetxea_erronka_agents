# REST API Backend-a (ASP.NET Core C#) · 6. FASEA: MANTENTZEA

> **Atala:** Negozio-logika eta aplikazio guztien komunikazio zentralizatua  
> **Fasearen Epea Orokorra:** 2026/12/02 - 2026/12/05  
> **Moduluaren Epe Zehatza:** 2026/12/02 - 2026/12/05  
> **Arduraduna / Rola:** Backend garatzailea  
> **Lehentasuna (Planner):** Premiazkoa  

---

## 1. Testuingurua eta Helburua

**6. FASEA: MANTENTZEA** fasean, **REST API Backend-a (ASP.NET Core C#)** moduluaren helburu nagusia honakoa da:
Sistemaren eguneroko jarraipena, segurtasun-kopiak (backupak), log-en kudeaketa, erabiltzaile-dokumentazioa eta akatsen prebentzioa.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Aplikazio guztien komunikazio-zentroa. CRUD eragiketak, negozio-logika (erreserben talka-kontrola, stockaren deskontu pesimista, ordainketen zibersegurtasuna eta fitxaketak).

- **Kodearen kokapena (Path):** [01_aplikazioak/admin/backend_rest_api_csharp](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/admin/backend_rest_api_csharp)
- **Erabilitako Teknologia Nagusiak:** `C# ASP.NET Core (.NET 10), Dapper / ADO.NET, JWT, BCrypt`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `6.2`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Erroreen log-sistema aktibatzea (Serilog / fitxategietara logeatzea).**
- [ ] **API bertsio-berritzeen eta adabakien kudeaketa-plana idaztea.**
- [ ] **API-ko eskaeren latentzia eta trafiko-analisia egitea.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Erroreen log-sistema aktibatzea (Serilog / fitxategietara logeatzea).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. API bertsio-berritzeen eta adabakien kudeaketa-plana idaztea.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. API-ko eskaeren latentzia eta trafiko-analisia egitea.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

---

## 4. Loturak eta Menpekotasunak

- **Sarrerako Menpekotasunak:** Fase honek aurreko faseko emaitzak eta azpiegituraren egoera behar ditu martxan egoteko.
- **Irteerako Ekarpenak:** Modulu honek sortutako emaitzak hurrengo faseetarako eta beste bezero-aplikazioentzat oinarri dira.
- **Lotutako Modulu Nagusiak:**
  - `01_datu_basea.md` (Datuak atzitzeko eta gordetzeko).
  - `02_rest_api.md` (Negozio-logika eta endpoint-ak kontsumitzeko).

---

## 5. Fase Amaierako Entregagaiak (Deliverables)

Fase hau bukatutzat emateko entregatu behar diren elementuak:
1. Moduluari dagokion iturburu-kodea edo konfigurazio-fitxategi eguneratuak.
2. Frogak eta balidazio-txostenak (errore gabe exekutatzen dela egiaztatuta).
3. Teams Planner-eko dagokion kartaren (`6.2`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
