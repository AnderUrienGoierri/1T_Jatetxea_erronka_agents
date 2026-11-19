# AZPIEGITURA FISIKOA (Laborategia, Sareak eta Ekipoak) · 5. FASEA: MARTXAN JARTZEA

> **Atala:** Rack 19", Cisco bideratzea, kableatua eta Wi-Fi sareak  
> **Fasearen Epea Orokorra:** 2026/11/29 - 2026/12/01  
> **Moduluaren Epe Zehatza:** 2026/11/29 - 2026/11/30  
> **Arduraduna / Rola:** Sare teknikaria  
> **Lehentasuna (Planner):** Premiazkoa  

---

## 1. Testuingurua eta Helburua

**5. FASEA: MARTXAN JARTZEA** fasean, **AZPIEGITURA FISIKOA (Laborategia, Sareak eta Ekipoak)** moduluaren helburu nagusia honakoa da:
Ekoizpen-inguruneko hedapena (Deployment), Docker edukiontzien abiaraztea, ekipo eta tablet fisikoetan instalazioa eta benetako kaxa-irekiera.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Jatetxearen hardware eta sare-azpiegitura erreala: 19" Rack-a, Cisco switch eta routerrak, kableatu egituratua (TIA-568-B), Access Point-ak eta TPV ekipo fisikoak.

- **Kodearen kokapena (Path):** [03_laborategi_fisikoa](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/03_laborategi_fisikoa)
- **Erabilitako Teknologia Nagusiak:** `Cisco IOS (Switch/Router), 19" Rack, Patch Panel RJ45, Wi-Fi AP`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `5.13`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Rack-a itxi eta kableatu guztia etiketatu eta txukundu.**
- [ ] **Cisco switch eta routerrak martxan utzi eta konfigurazioa memorian gorde (`write memory`).**
- [ ] **Bezeroen eta langileen Wi-Fi sareak aktibatu eta funtzionamendua egiaztatu.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Rack-a itxi eta kableatu guztia etiketatu eta txukundu.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Cisco switch eta routerrak martxan utzi eta konfigurazioa memorian gorde (`write memory`).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Bezeroen eta langileen Wi-Fi sareak aktibatu eta funtzionamendua egiaztatu.
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
3. Teams Planner-eko dagokion kartaren (`5.13`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
