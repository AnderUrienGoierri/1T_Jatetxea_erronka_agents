# AZPIEGITURA DIGITALA (Makina Birtualak & Docker) · 2. FASEA: DISEINUA

> **Atala:** Sare birtualak, suebakia eta zerbitzarien orkestrazioa  
> **Fasearen Epea Orokorra:** 2026/11/09 - 2026/11/14  
> **Moduluaren Epe Zehatza:** 2026/11/09 - 2026/11/12  
> **Arduraduna / Rola:** Sistema administratzailea  
> **Lehentasuna (Planner):** Premiazkoa  

---

## 1. Testuingurua eta Helburua

**2. FASEA: DISEINUA** fasean, **AZPIEGITURA DIGITALA (Makina Birtualak & Docker)** moduluaren helburu nagusia honakoa da:
Datu-ereduen definizio zehatza, API kontratuen eta JSON eskemen egituraketa, Figma bidezko UI/UX diseinuak eta sare-topologien modelaketa.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Ingurune birtualizatua: OPNsense suebakia (sareen segmentazioa, VLANak, DHCP), Windows Server 2025, Ubuntu Server eta zerbitzuak biltzen dituzten Docker edukiontziak.

- **Kodearen kokapena (Path):** [02_azpiegitura_birtuala](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/02_azpiegitura_birtuala)
- **Erabilitako Teknologia Nagusiak:** `VirtualBox, OPNsense, Docker & Docker Compose, Linux/Windows Server`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `2.11`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Sare birtualaren arkitektura diagrama osatzea (OPNsense WAN, LAN, DMZ, VLANak).**
- [ ] **Firewall erregela-matrizea diseinatzea (sarbide baimenduak eta blokeatuak).**
- [ ] **`docker-compose.yml` konfigurazio-fitxategiaren egitura diseinatzea.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Sare birtualaren arkitektura diagrama osatzea (OPNsense WAN, LAN, DMZ, VLANak).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Firewall erregela-matrizea diseinatzea (sarbide baimenduak eta blokeatuak).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. `docker-compose.yml` konfigurazio-fitxategiaren egitura diseinatzea.
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
3. Teams Planner-eko dagokion kartaren (`2.11`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
