# AZPIEGITURA DIGITALA (Makina Birtualak & Docker) · 1. FASEA: ANALISIA

> **Atala:** Sare birtualak, suebakia eta zerbitzarien orkestrazioa  
> **Fasearen Epea Orokorra:** 2026/11/03 - 2026/11/08  
> **Moduluaren Epe Zehatza:** 2026/11/03 - 2026/11/06  
> **Arduraduna / Rola:** Sistema administratzailea  
> **Lehentasuna (Planner):** Premiazkoa  

---

## 1. Testuingurua eta Helburua

**1. FASEA: ANALISIA** fasean, **AZPIEGITURA DIGITALA (Makina Birtualak & Docker)** moduluaren helburu nagusia honakoa da:
Eskakizunen bilketa sakona, erabilera-kasuen definizioa, plataformen bideragarritasuna eta oinarrizko arkitekturaren zehaztapena.

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> Ingurune birtualizatua: OPNsense suebakia (sareen segmentazioa, VLANak, DHCP), Windows Server 2025, Ubuntu Server eta zerbitzuak biltzen dituzten Docker edukiontziak.

- **Kodearen kokapena (Path):** [02_azpiegitura_birtuala](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/02_azpiegitura_birtuala)
- **Erabilitako Teknologia Nagusiak:** `VirtualBox, OPNsense, Docker & Docker Compose, Linux/Windows Server`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `1.12`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

- [ ] **Makina birtualen baliabideak (RAM, CPU, Diskoa) kalkulatzea (OPNsense, WinServer, Ubuntu).**
- [ ] **Sare birtualen segmentazioa (VLANak, DMZ, Lan, Bezeroak) definitzea.**
- [ ] **Docker edukiontzien arkitektura eta portuen mapeoa planifikatzea.**

---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

### 3.1. Makina birtualen baliabideak (RAM, CPU, Diskoa) kalkulatzea (OPNsense, WinServer, Ubuntu).
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.2. Sare birtualen segmentazioa (VLANak, DMZ, Lan, Bezeroak) definitzea.
- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.
- **Jarduera teknikoak:**
  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.
  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.
  - Frogak eta balidazio automatikoak gainditzea.

### 3.3. Docker edukiontzien arkitektura eta portuen mapeoa planifikatzea.
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
3. Teams Planner-eko dagokion kartaren (`1.12`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
