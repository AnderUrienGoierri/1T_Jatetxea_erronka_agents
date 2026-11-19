# APP Java - Sukaldea (KDS Mahaigainekoa) · 6. FASEA: MANTENTZEA

> **Atala:** Sukaldeko lan-estazioko KDS aplikazioa  
> **Fasearen Epea Orokorra:** 2026/12/02 - 2026/12/05  
> **Moduluaren Epe Zehatza:** 2026/12/02 - 2026/12/05  
> **Arduraduna / Rola:** Java garatzailea  
> **Lehentasuna (Planner):** Ertaina  

---

## 1. Testuingurua eta Helburua

**6. FASEA: MANTENTZEA** fasean, **APP Java - Sukaldea (KDS Mahaigainekoa)** moduluaren helburu nagusia honakoa da:
Sistemaren eguneroko jarraipena, segurtasun-kopiak (backupak), log-en kudeaketa, erabiltzaile-dokumentazioa eta akatsen prebentzioa.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Sukaldeko pantaila taktiletarako edo ordenagailuetarako KDS mahaigaineko bertsioa. Komanden platerak kontrolatzea (Zai -> Martxan -> Eginda) eta sukaldarien PIN fitxaketa.

- **Kodearen kokapena (Path):** [01_aplikazioak/sukalde_langileak/desktop_sukaldea_kds](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/sukalde_langileak/desktop_sukaldea_kds)
- **Erabilitako Teknologia Nagusiak:** `JavaFX 21, Maven, REST API Client`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `6.7`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Sukaldeko KDS aplikazioaren erregistroak garbitzea.**
- [ ] **Sukaldarien aldaketa-proposamenak jasotzea.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Sukaldeko KDS aplikazioaren erregistroak garbitzea.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Sukaldarien aldaketa-proposamenak jasotzea.
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
3. Teams Planner-eko dagokion kartaren (`6.7`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
