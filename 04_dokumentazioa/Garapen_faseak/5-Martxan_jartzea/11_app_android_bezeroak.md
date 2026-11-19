# APP - Android Bezeroak (Flutter Mugikorra) · 5. FASEA: MARTXAN JARTZEA

> **Atala:** Mahaiko auto-eskaera eta auto-ordainketa mugikorretik  
> **Fasearen Epea Orokorra:** 2026/11/29 - 2026/12/01  
> **Moduluaren Epe Zehatza:** 2026/11/30 - 2026/12/01  
> **Arduraduna / Rola:** Mugikor garatzailea  
> **Lehentasuna (Planner):** Ertaina  

---

## 1. Testuingurua eta Helburua

**5. FASEA: MARTXAN JARTZEA** fasean, **APP - Android Bezeroak (Flutter Mugikorra)** moduluaren helburu nagusia honakoa da:
Ekoizpen-inguruneko hedapena (Deployment), Docker edukiontzien abiaraztea, ekipo eta tablet fisikoetan instalazioa eta benetako kaxa-irekiera.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Jatetxera datozen bezeroentzako mugikorreko aplikazioa. Mahaiko QR kodea eskaneatuz auto-eskaera egiteko, eramateko eskatzeko eta ordainketa birtuala burutzeko.

- **Kodearen kokapena (Path):** [01_aplikazioak/bezeroak/mobile_bezeroa_flutter](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/bezeroak/mobile_bezeroa_flutter)
- **Erabilitako Teknologia Nagusiak:** `Flutter 3.x, Dart, Mobile Camera (QR), REST Client`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `5.11`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Bezeroen app-a probako ingurune irekian jarri (edo QR bidezko deskarga zuzena prestatu).**
- [ ] **Mahaiko QR kode fisikoak inprimatu eta mahaietan itsatsi.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Bezeroen app-a probako ingurune irekian jarri (edo QR bidezko deskarga zuzena prestatu).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Mahaiko QR kode fisikoak inprimatu eta mahaietan itsatsi.
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
3. Teams Planner-eko dagokion kartaren (`5.11`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
