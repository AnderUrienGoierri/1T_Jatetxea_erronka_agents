# APP - Android TV - Sukaldariak (KDS) · 2. FASEA: DISEINUA

> **Atala:** Sukaldeko pantaila handirako Kitchen Display System  
> **Fasearen Epea Orokorra:** 2026/11/09 - 2026/11/14  
> **Moduluaren Epe Zehatza:** 2026/11/11 - 2026/11/14  
> **Arduraduna / Rola:** Android garatzailea  
> **Lehentasuna (Planner):** Ertaina  

---

## 1. Testuingurua eta Helburua

**2. FASEA: DISEINUA** fasean, **APP - Android TV - Sukaldariak (KDS)** moduluaren helburu nagusia honakoa da:
Datu-ereduen definizio zehatza, API kontratuen eta JSON eskemen egituraketa, Figma bidezko UI/UX diseinuak eta sare-topologien modelaketa.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Sukaldeko telebista eta pantaila handietarako Kitchen Display System (KDS). Urruneko agintearekin (D-Pad) kontrolatua, komandak denbora errealean erakustea eta prestatze-egoeren kudeaketa.

- **Kodearen kokapena (Path):** [01_aplikazioak/sukalde_langileak/sukaldea_kds_android_tv](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/sukalde_langileak/sukaldea_kds_android_tv)
- **Erabilitako Teknologia Nagusiak:** `Android TV, Jetpack Compose for TV / Leanback, REST Polling`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `2.6`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Leanback / Compose for TV layout diseinua prestatzea (txartel handiak).**
- [ ] **Komanden egoera kolore-kodeak definitzea (Horia = Zai, Urdina = Martxan, Berdea = Eginda).**
- [ ] **Fokuaren nabigazioa eta D-Pad kontrolen portaera diseinatzea.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Leanback / Compose for TV layout diseinua prestatzea (txartel handiak).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Komanden egoera kolore-kodeak definitzea (Horia = Zai, Urdina = Martxan, Berdea = Eginda).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Fokuaren nabigazioa eta D-Pad kontrolen portaera diseinatzea.
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
3. Teams Planner-eko dagokion kartaren (`2.6`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
