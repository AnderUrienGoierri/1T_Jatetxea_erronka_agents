# APP - Zerbitzaria / Chat Internoa · 2. FASEA: DISEINUA

> **Atala:** Sailen arteko denbora errealeko komunikazio arina  
> **Fasearen Epea Orokorra:** 2026/11/09 - 2026/11/14  
> **Moduluaren Epe Zehatza:** 2026/11/11 - 2026/11/14  
> **Arduraduna / Rola:** Backend garatzailea  
> **Lehentasuna (Planner):** Baxua  

---

## 1. Testuingurua eta Helburua

**2. FASEA: DISEINUA** fasean, **APP - Zerbitzaria / Chat Internoa** moduluaren helburu nagusia honakoa da:
Datu-ereduen definizio zehatza, API kontratuen eta JSON eskemen egituraketa, Figma bidezko UI/UX diseinuak eta sare-topologien modelaketa.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Jatetxeko langileen arteko berehalako barne-komunikazio sistema. Areto, sukalde eta barraren arteko gelak (rooms), premiazko abisuak eta denbora errealeko mezularitza.

- **Kodearen kokapena (Path):** [01_aplikazioak/admin/admin_IT_zentrala/server](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/01_aplikazioak/admin/admin_IT_zentrala/server)
- **Erabilitako Teknologia Nagusiak:** `Node.js, Socket.IO, WebSockets`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `2.9`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **WebSocket gertaeren protokoloa diseinatzea (`join_room`, `send_message`, `receive_message`).**
- [ ] **Txat leihoaren diseinua bezero aplikazioetarako (widget mugikorra edo panela).**
- [ ] **Jakinarazpen bisual eta akustikoen portaera diseinatzea.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. WebSocket gertaeren protokoloa diseinatzea (`join_room`, `send_message`, `receive_message`).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Txat leihoaren diseinua bezero aplikazioetarako (widget mugikorra edo panela).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Jakinarazpen bisual eta akustikoen portaera diseinatzea.
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
3. Teams Planner-eko dagokion kartaren (`2.9`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
