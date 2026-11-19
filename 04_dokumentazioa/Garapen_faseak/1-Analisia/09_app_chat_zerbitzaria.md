# APP - Zerbitzaria / Chat Internoa · 1. FASEA: ANALISIA

> **Atala:** Sailen arteko denbora errealeko komunikazio arina  
> **Fasearen Epea Orokorra:** 2026/11/03 - 2026/11/08  
> **Moduluaren Epe Zehatza:** 2026/11/05 - 2026/11/08  
> **Arduraduna / Rola:** Backend / Sareak  
> **Lehentasuna (Planner):** Baxua  

---

## 1. Testuingurua eta Helburua

**1. FASEA: ANALISIA** fasean, **APP - Zerbitzaria / Chat Internoa** moduluaren helburu nagusia honakoa da:
Eskakizunen bilketa sakona, erabilera-kasuen definizioa, plataformen bideragarritasuna eta oinarrizko arkitekturaren zehaztapena.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Jatetxeko langileen arteko berehalako barne-komunikazio sistema. Areto, sukalde eta barraren arteko gelak (rooms), premiazko abisuak eta denbora errealeko mezularitza.

- **Kodearen kokapena (Path):** [01_aplikazioak/admin/admin_IT_zentrala/server](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/admin/admin_IT_zentrala/server)
- **Erabilitako Teknologia Nagusiak:** `Node.js, Socket.IO, WebSockets`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `1.9`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Barne komunikazioaren beharrak aztertzea (zerbitzariak <-> sukaldea <-> barra).**
- [ ] **Denbora errealeko protokoloaren hautaketa (WebSockets / Socket.IO vs polling).**
- [ ] **Kanalak eta mezu azkarren txantiloiak definitzea ("Premiazkoa mahai 4", "Ogia falta da").**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Barne komunikazioaren beharrak aztertzea (zerbitzariak <-> sukaldea <-> barra).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Denbora errealeko protokoloaren hautaketa (WebSockets / Socket.IO vs polling).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Kanalak eta mezu azkarren txantiloiak definitzea ("Premiazkoa mahai 4", "Ogia falta da").
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
3. Teams Planner-eko dagokion kartaren (`1.9`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
