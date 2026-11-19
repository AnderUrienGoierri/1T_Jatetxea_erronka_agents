# APP - TPV - Zerbitzariak (C# WinForms) · 4. FASEA: FROGAK

> **Atala:** Barrako Salmenta Puntuaren Terminal Nagusia (POS)  
> **Fasearen Epea Orokorra:** 2026/11/25 - 2026/11/28  
> **Moduluaren Epe Zehatza:** 2026/11/25 - 2026/11/27  
> **Arduraduna / Rola:** .NET garatzailea  
> **Lehentasuna (Planner):** Premiazkoa  

---

## 1. Testuingurua eta Helburua

**4. FASEA: FROGAK** fasean, **APP - TPV - Zerbitzariak (C# WinForms)** moduluaren helburu nagusia honakoa da:
Proba unitarioak, integrazio-probak sistemen artean, erreserben talka eta karga-probak, eta zibersegurtasuneko ikuskapenak.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Barrako Salmenta Puntuaren Terminala (TPV/POS). Mahaien mapa interaktiboa, komanden sarrera, kontuen kobraketa (eskudirua, txartela, Bizum), tiket fiskalen inprimaketa eta langileen txanda-fitxaketa.

- **Kodearen kokapena (Path):** [01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp)
- **Erabilitako Teknologia Nagusiak:** `C# WinForms (.NET 10), ESC/POS Termikoa, REST API Bezeroa`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `4.4`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Mahaien fluxu osoaren proba: Ireki -> Komanda gehitu -> Ordaindu -> Tiketa atera.**
- [ ] **PIN bidezko fitxaketa probatu eta APIan erregistro zuzena egiaztatu.**
- [ ] **Sare-mozketaren simulazioa (offline alertak).**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Mahaien fluxu osoaren proba: Ireki -> Komanda gehitu -> Ordaindu -> Tiketa atera.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. PIN bidezko fitxaketa probatu eta APIan erregistro zuzena egiaztatu.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Sare-mozketaren simulazioa (offline alertak).
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
3. Teams Planner-eko dagokion kartaren (`4.4`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
