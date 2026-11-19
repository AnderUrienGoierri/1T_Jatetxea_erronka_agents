# Datu-Basea (MySQL & PostgreSQL / Odoo) · 4. FASEA: FROGAK

> **Atala:** Datu erlazionalen eskema nagusia eta integrazio korporatiboa  
> **Fasearen Epea Orokorra:** 2026/11/25 - 2026/11/28  
> **Moduluaren Epe Zehatza:** 2026/11/25 - 2026/11/26  
> **Arduraduna / Rola:** Datu-base arduraduna  
> **Lehentasuna (Planner):** Premiazkoa  

---

## 1. Testuingurua eta Helburua

**4. FASEA: FROGAK** fasean, **Datu-Basea (MySQL & PostgreSQL / Odoo)** moduluaren helburu nagusia honakoa da:
Proba unitarioak, integrazio-probak sistemen artean, erreserben talka eta karga-probak, eta zibersegurtasuneko ikuskapenak.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Jatetxearen datu-eredu erlazionala (MySQL 8.0 InnoDB), ACID transakzioak, lehengaiak, stock kontrola, plateren eskandalloak eta Odoo ERP (PostgreSQL) HR integrazioa.

- **Kodearen kokapena (Path):** [db/datu_basea_schema.sql](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/db/datu_basea_schema.sql)
- **Erabilitako Teknologia Nagusiak:** `MySQL 8.0 (InnoDB), PostgreSQL 16 (Odoo 17), Docker`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `4.1`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Integritate erlazionalaren probak (Foreign Key murrizketak, ezabaketa kaskadan).**
- [ ] **Erreserben gako bakar konposatuaren talka-proba (`UNIQUE KEY uq_erreserba`).**
- [ ] **Datu-basearen erantzun-denborak eta indizeen eraginkortasuna ebaluatu.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Integritate erlazionalaren probak (Foreign Key murrizketak, ezabaketa kaskadan).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Erreserben gako bakar konposatuaren talka-proba (`UNIQUE KEY uq_erreserba`).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Datu-basearen erantzun-denborak eta indizeen eraginkortasuna ebaluatu.
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
3. Teams Planner-eko dagokion kartaren (`4.1`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
