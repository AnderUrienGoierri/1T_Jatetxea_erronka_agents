# APP - Android Bezeroak (Flutter Mugikorra) · 6. FASEA: MANTENTZEA

> **Atala:** Mahaiko auto-eskaera eta auto-ordainketa mugikorretik  
> **Fasearen Epea Orokorra:** 2026/12/02 - 2026/12/05  
> **Moduluaren Epe Zehatza:** 2026/12/02 - 2026/12/05  
> **Arduraduna / Rola:** Mugikor garatzailea  
> **Lehentasuna (Planner):** Ertaina  

---

## 1. Testuingurua eta Helburua

**6. FASEA: MANTENTZEA** fasean, **APP - Android Bezeroak (Flutter Mugikorra)** moduluaren helburu nagusia honakoa da:
Sistemaren eguneroko jarraipena, segurtasun-kopiak (backupak), log-en kudeaketa, erabiltzaile-dokumentazioa eta akatsen prebentzioa.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Jatetxera datozen bezeroentzako mugikorreko aplikazioa. Mahaiko QR kodea eskaneatuz auto-eskaera egiteko, eramateko eskatzeko eta ordainketa birtuala burutzeko.

- **Kodearen kokapena (Path):** [01_aplikazioak/bezeroak/mobile_bezeroa_flutter](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/bezeroak/mobile_bezeroa_flutter)
- **Erabilitako Teknologia Nagusiak:** `Flutter 3.x, Dart, Mobile Camera (QR), REST Client`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `6.11`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Bezeroen app-aren bertsio berriak banatzeko prozedura idaztea.**
- [ ] **Akats-txostenen jasotzea eta erregistroa egitea.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Bezeroen app-aren bertsio berriak banatzeko prozedura idaztea.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Akats-txostenen jasotzea eta erregistroa egitea.
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
3. Teams Planner-eko dagokion kartaren (`6.11`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
