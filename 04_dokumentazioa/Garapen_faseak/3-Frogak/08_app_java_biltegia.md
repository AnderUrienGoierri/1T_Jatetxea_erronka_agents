# APP Java - Biltegia (Stock eta Inbentarioa) · 4. FASEA: FROGAK

> **Atala:** Biltegiko stockaren jarraipena eta alertak  
> **Fasearen Epea Orokorra:** 2026/11/25 - 2026/11/28  
> **Moduluaren Epe Zehatza:** 2026/11/26 - 2026/11/28  
> **Arduraduna / Rola:** Java garatzailea  
> **Lehentasuna (Planner):** Ertaina  

---

## 1. Testuingurua eta Helburua

**4. FASEA: FROGAK** fasean, **APP Java - Biltegia (Stock eta Inbentarioa)** moduluaren helburu nagusia honakoa da:
Proba unitarioak, integrazio-probak sistemen artean, erreserben talka eta karga-probak, eta zibersegurtasuneko ikuskapenak.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Biltegiko arduradunaren aplikazioa. Lehengaien uneko stocka, hornitzaileen albaranen sarrerak, inbentario-doikuntzak eta gutxieneko stock alertak gorriz bistaratzea.

- **Kodearen kokapena (Path):** [01_aplikazioak/biltegi_langileak/desktop_biltegia_stock](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/biltegi_langileak/desktop_biltegia_stock)
- **Erabilitako Teknologia Nagusiak:** `JavaFX 21, Maven, REST API Client`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `4.8`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Stock-aldaketak berehala islatzen direla probatu.**
- [ ] **Gutxieneko stock alertak gorriz behar bezala aktibatzen direla ziurtatu.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Stock-aldaketak berehala islatzen direla probatu.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Gutxieneko stock alertak gorriz behar bezala aktibatzen direla ziurtatu.
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
3. Teams Planner-eko dagokion kartaren (`4.8`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
