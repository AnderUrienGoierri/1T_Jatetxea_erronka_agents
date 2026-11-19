# APP Java - Sukaldea (KDS Mahaigainekoa) · 3. FASEA: GARAPENA

> **Atala:** Sukaldeko lan-estazioko KDS aplikazioa  
> **Fasearen Epea Orokorra:** 2026/11/15 - 2026/11/24  
> **Moduluaren Epe Zehatza:** 2026/11/17 - 2026/11/21  
> **Arduraduna / Rola:** Java garatzailea  
> **Lehentasuna (Planner):** Ertaina  

---

## 1. Testuingurua eta Helburua

**3. FASEA: GARAPENA** fasean, **APP Java - Sukaldea (KDS Mahaigainekoa)** moduluaren helburu nagusia honakoa da:
Iturburu-kodearen garapen osoa, zerbitzarien programazioa, negozio-logika, interfaze grafikoak, scriptak eta datu-baseen inplementazioa.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Sukaldeko pantaila taktiletarako edo ordenagailuetarako KDS mahaigaineko bertsioa. Komanden platerak kontrolatzea (Zai -> Martxan -> Eginda) eta sukaldarien PIN fitxaketa.

- **Kodearen kokapena (Path):** [01_aplikazioak/sukalde_langileak/desktop_sukaldea_kds](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/sukalde_langileak/desktop_sukaldea_kds)
- **Erabilitako Teknologia Nagusiak:** `JavaFX 21, Maven, REST API Client`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `3.7`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **JavaFX KDS mahaigaineko aplikazioa garatu (`desktop_sukaldea_kds`).**
- [ ] **Plateren egoera aldaketen logika (Zai -> Martxan -> Eginda) APIarekin lotu.**
- [ ] **Sukaldarien PIN bidezko fitxaketa erregistroa inplementatu.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. JavaFX KDS mahaigaineko aplikazioa garatu (`desktop_sukaldea_kds`).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Plateren egoera aldaketen logika (Zai -> Martxan -> Eginda) APIarekin lotu.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Sukaldarien PIN bidezko fitxaketa erregistroa inplementatu.
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
3. Teams Planner-eko dagokion kartaren (`3.7`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
