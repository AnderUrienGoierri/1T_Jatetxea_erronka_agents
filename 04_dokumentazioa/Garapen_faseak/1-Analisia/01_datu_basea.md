# Datu-Basea (MySQL & PostgreSQL / Odoo) · 1. FASEA: ANALISIA

> **Atala:** Datu erlazionalen eskema nagusia eta integrazio korporatiboa  
> **Fasearen Epea Orokorra:** 2026/11/03 - 2026/11/08  
> **Moduluaren Epe Zehatza:** 2026/11/03 - 2026/11/06  
> **Arduraduna / Rola:** Datu-base arduraduna  
> **Lehentasuna (Planner):** Premiazkoa  

---

## 1. Testuingurua eta Helburua

**1. FASEA: ANALISIA** fasean, **Datu-Basea (MySQL & PostgreSQL / Odoo)** moduluaren helburu nagusia honakoa da:
Eskakizunen bilketa sakona, erabilera-kasuen definizioa, plataformen bideragarritasuna eta oinarrizko arkitekturaren zehaztapena.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Jatetxearen datu-eredu erlazionala (MySQL 8.0 InnoDB), ACID transakzioak, lehengaiak, stock kontrola, plateren eskandalloak eta Odoo ERP (PostgreSQL) HR integrazioa.

- **Kodearen kokapena (Path):** [db/datu_basea_schema.sql](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/db/datu_basea_schema.sql)
- **Erabilitako Teknologia Nagusiak:** `MySQL 8.0 (InnoDB), PostgreSQL 16 (Odoo 17), Docker`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `1.1`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Jatetxearen entitate nagusiak identifikatzea (mahaiak, langileak, platerak, erreserbak, eskaerak, fakturak, stocka).**
- [ ] **Odoo ERP (PostgreSQL) eta MySQL arteko datuen banaketa eta bereizketa aztertzea.**
- [ ] **Erreserben gainjartzeak eta transakzioen eskakizunak zehaztea (ACID propietateak).**
- [ ] **Datu-motak, eremu derrigorrezkoak eta gako primario/arrotzak zerrendatzea.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Jatetxearen entitate nagusiak identifikatzea (mahaiak, langileak, platerak, erreserbak, eskaerak, fakturak, stocka).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Odoo ERP (PostgreSQL) eta MySQL arteko datuen banaketa eta bereizketa aztertzea.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Erreserben gainjartzeak eta transakzioen eskakizunak zehaztea (ACID propietateak).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.4. Datu-motak, eremu derrigorrezkoak eta gako primario/arrotzak zerrendatzea.
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
3. Teams Planner-eko dagokion kartaren (`1.1`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
