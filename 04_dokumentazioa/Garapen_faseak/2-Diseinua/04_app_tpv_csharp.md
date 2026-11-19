# APP - TPV - Zerbitzariak (C# WinForms) · 2. FASEA: DISEINUA

> **Atala:** Barrako Salmenta Puntuaren Terminal Nagusia (POS)  
> **Fasearen Epea Orokorra:** 2026/11/09 - 2026/11/14  
> **Moduluaren Epe Zehatza:** 2026/11/09 - 2026/11/12  
> **Arduraduna / Rola:** .NET garatzailea  
> **Lehentasuna (Planner):** Premiazkoa  

---

## 1. Testuingurua eta Helburua

**2. FASEA: DISEINUA** fasean, **APP - TPV - Zerbitzariak (C# WinForms)** moduluaren helburu nagusia honakoa da:
Datu-ereduen definizio zehatza, API kontratuen eta JSON eskemen egituraketa, Figma bidezko UI/UX diseinuak eta sare-topologien modelaketa.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Barrako Salmenta Puntuaren Terminala (TPV/POS). Mahaien mapa interaktiboa, komanden sarrera, kontuen kobraketa (eskudirua, txartela, Bizum), tiket fiskalen inprimaketa eta langileen txanda-fitxaketa.

- **Kodearen kokapena (Path):** [01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp)
- **Erabilitako Teknologia Nagusiak:** `C# WinForms (.NET 10), ESC/POS Termikoa, REST API Bezeroa`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `2.4`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Aretoaren plano interaktiboa diseinatzea (Jangela Nagusia, Terraza, Barra, VIP).**
- [ ] **Kobraketa leihoaren eta tiketen formatu fiskalaren diseinua.**
- [ ] **Zenbakizko teklatu birtualaren diseinua PIN bidez azkar fitxatzeko.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Aretoaren plano interaktiboa diseinatzea (Jangela Nagusia, Terraza, Barra, VIP).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Kobraketa leihoaren eta tiketen formatu fiskalaren diseinua.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Zenbakizko teklatu birtualaren diseinua PIN bidez azkar fitxatzeko.
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
3. Teams Planner-eko dagokion kartaren (`2.4`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
