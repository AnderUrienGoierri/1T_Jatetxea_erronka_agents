# REST API Backend-a (ASP.NET Core C#) · 3. FASEA: GARAPENA

> **Atala:** Negozio-logika eta aplikazio guztien komunikazio zentralizatua  
> **Fasearen Epea Orokorra:** 2026/11/15 - 2026/11/24  
> **Moduluaren Epe Zehatza:** 2026/11/16 - 2026/11/20  
> **Arduraduna / Rola:** Backend garatzailea  
> **Lehentasuna (Planner):** Premiazkoa  

---

## 1. Testuingurua eta Helburua

**3. FASEA: GARAPENA** fasean, **REST API Backend-a (ASP.NET Core C#)** moduluaren helburu nagusia honakoa da:
Iturburu-kodearen garapen osoa, zerbitzarien programazioa, negozio-logika, interfaze grafikoak, scriptak eta datu-baseen inplementazioa.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Aplikazio guztien komunikazio-zentroa. CRUD eragiketak, negozio-logika (erreserben talka-kontrola, stockaren deskontu pesimista, ordainketen zibersegurtasuna eta fitxaketak).

- **Kodearen kokapena (Path):** [01_aplikazioak/admin/backend_rest_api_csharp](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/admin/backend_rest_api_csharp)
- **Erabilitako Teknologia Nagusiak:** `C# ASP.NET Core (.NET 10), Dapper / ADO.NET, JWT, BCrypt`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `3.2`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **ASP.NET Core (.NET 10) proiektua sortu eta Dapper / MySQL konexioa programatu.**
- [ ] **CRUD kontroladoreak garatu: Platerak, Mahaiak, Eskaerak, Erreserbak, Fitxaketak.**
- [ ] **Erreserben talka-kontrol logika programatu (409 Conflict erantzuna).**
- [ ] **Langileen PIN autentifikazioa eta BCrypt balidazioa inplementatu.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. ASP.NET Core (.NET 10) proiektua sortu eta Dapper / MySQL konexioa programatu.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. CRUD kontroladoreak garatu: Platerak, Mahaiak, Eskaerak, Erreserbak, Fitxaketak.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Erreserben talka-kontrol logika programatu (409 Conflict erantzuna).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.4. Langileen PIN autentifikazioa eta BCrypt balidazioa inplementatu.
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
3. Teams Planner-eko dagokion kartaren (`3.2`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
