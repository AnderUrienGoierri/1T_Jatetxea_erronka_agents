# APP - Android TV - Sukaldariak (KDS) · 5. FASEA: MARTXAN JARTZEA

> **Atala:** Sukaldeko pantaila handirako Kitchen Display System  
> **Fasearen Epea Orokorra:** 2026/11/29 - 2026/12/01  
> **Moduluaren Epe Zehatza:** 2026/11/30 - 2026/12/01  
> **Arduraduna / Rola:** Android garatzailea  
> **Lehentasuna (Planner):** Ertaina  

---

## 1. Testuingurua eta Helburua

**5. FASEA: MARTXAN JARTZEA** fasean, **APP - Android TV - Sukaldariak (KDS)** moduluaren helburu nagusia honakoa da:
Ekoizpen-inguruneko hedapena (Deployment), Docker edukiontzien abiaraztea, ekipo eta tablet fisikoetan instalazioa eta benetako kaxa-irekiera.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Sukaldeko telebista eta pantaila handietarako Kitchen Display System (KDS). Urruneko agintearekin (D-Pad) kontrolatua, komandak denbora errealean erakustea eta prestatze-egoeren kudeaketa.

- **Kodearen kokapena (Path):** [01_aplikazioak/sukalde_langileak/sukaldea_kds_android_tv](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/sukalde_langileak/sukaldea_kds_android_tv)
- **Erabilitako Teknologia Nagusiak:** `Android TV, Jetpack Compose for TV / Leanback, REST Polling`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `5.6`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **APK-a Android TV gailuan / telebistan instalatu (ADB edo USB bidez).**
- [ ] **Pantaila finkoan abiarazte automatikoa konfiguratu gailua piztean.**
- [ ] **Sukaldeko horman kokatu eta kable bidezko sare-konexioa bermatu.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. APK-a Android TV gailuan / telebistan instalatu (ADB edo USB bidez).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Pantaila finkoan abiarazte automatikoa konfiguratu gailua piztean.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Sukaldeko horman kokatu eta kable bidezko sare-konexioa bermatu.
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
3. Teams Planner-eko dagokion kartaren (`5.6`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
