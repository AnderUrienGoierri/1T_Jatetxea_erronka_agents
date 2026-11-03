# Antigravity CLI: Agente Anitzekin Lan Egiteko Gida (Multi-Agent Workflows)

Dokumentu honek azaltzen du nola antolatu eta erabili daitezkeen hainbat agente paraleloan **Antigravity CLI**-n, proiektu bakar baten gainean modu eraginkorrean eta koordinatuan lan egiteko.

---

## 1. Nola funtzionatzen du agente anitzen sistemak?

Antigravity-k zeregin konplexuak banatzeko eta aldi berean hainbat frontetan lan egiteko azpiegitura aurreratua eskaintzen du. Bi zutabe nagusi ditu:

1. **Azpi-agente espezializatuak (Subagents):**
   * Agente nagusiak (*orchestrator*) azpi-agente autonomoak abiaraz ditzake atzeko planoan.
   * Agente bakoitzari eginkizun, testuinguru eta tresna zehatzak esleitzen zaizkio (adibidez: fitxategiak soilik irakurtzeko baimena ikerketarako, edo komandoak exekutatzeko baimena garapenerako).

2. **Lan-eremu isolatuak (Isolated Workspaces / Branches):**
   * Agente bakoitzak lan-eremu independente batean lan egin dezake (`Workspace: 'branch'` edo `'share'`).
   * Horri esker, hainbat agentek ez dute proiektuko fitxategi bera aldi berean gainidatziko edo gatazkarik sortuko. Bakoitzak bere adarrean garatzen du, eta amaitutakoan aldaketak aztertu eta bateratzen dira (*merge*).

---

## 2. Agenteen rolak eta zereginen banaketa

Proiektu bat ataletan banatzeko, agente bakoitzari rol espezifiko bat ematea da estrategiarik onena:

| Rola | Ardura nagusia | Adibidea |
| :--- | :--- | :--- |
| **Codebase Researcher** | Kodea, mendekotasunak eta dokumentazioa aztertu | Liburutegien bateragarritasuna edo APIen funtzionamendua ikertu |
| **Backend Developer** | Zerbitzariaren logika, APIak eta datu-baseak garatu | REST/GraphQL endpoint-ak eta datu-ereduak sortu |
| **Frontend / UI Developer** | Erabiltzaile-interfazea eta osagai bisualak prestatu | Web orriak, estiloak eta bezeroaren logika garatu |
| **QA / Tester** | Probak diseinatu, idatzi eta exekutatu | Unitate-probak (`unit tests`) eta integrazio-probak garatu |

---

## 3. Nola jarri martxan (bi bide nagusi)

### A Bidea: `/teamwork` slash komandoa (Gomendatua)
Antigravity-k komando berezi bat du talde-lan autonomoa aktibatzeko. Zure txatean idatzi besterik ez duzu:
```text
/teamwork
```
Komando honek automatikoki aktibatzen du agente talde koordinatuaren fluxua: proiektua analizatu, atazak bereizi eta agente autonomoak esleituko ditu paraleloan lan egiteko.

---

### B Bidea: Agente nagusiari agindu zuzenak ematea
Txatean bertan, zehaztu agente nagusiari zein agente eta zein zeregin banatu nahi dituzun.

**Adibideko eskaera (Prompt-a):**
> *"Sortu azpi-agenteak paraleloan proiektu hau garatzeko:*
> * **1. Agentea (Backend):** Garatu `/api/ikasleak` endpoint-ak eta datuen balidazioa adar isolatu batean.
> * **2. Agentea (Frontend):** Prestatu ikasleen zerrenda eta inprimakiak bistaratuko dituen interfazea.
> * **3. Agentea (QA):** Idatzi unitate-probak backend-eko logikarentzat.
> *Bakoitzak bere lana amaitzean, aurkeztu laburpen bateratua aldaketak integratzeko."*

---

## 4. Praktika onak eta gomendioak

1. **Plangintza argia hasieran (`/plan`):**
   * Garatzen hasi aurretik, erabili `/plan` komandoa. Honek modulu bakoitzaren mugak eta interfazeak aldez aurretik zehaztea ahalbidetzen du, agenteek elkarren mendekotasunekin arazorik izan ez dezaten.
2. **Direktorio edo atalen banaketa garbia:**
   * Komeni da agente bakoitzak direktorio independenteetan lan egitea (adibidez: `/src/backend` eta `/src/frontend`).
3. **Erreaktibotasuna eta jakinarazpenak:**
   * Ez duzu agente bakoitzari etengabe itxaron beharrik; agente nagusiak atzeko planoko azpi-agenteen emaitzak jasotzen dituenean automatikoki jakinaraziko dizu.
4. **Berrikuspena eta bateratzea:**
   * Agenteek lana bukatzean, aldaketak berrikusi eta probak pasa ondoren integratzen da kode guztia adar nagusira.

---

## 5. Slash komando erabilgarriak

* **/teamwork**: Agente talde baten koordinazio autonomoa aktibatzeko.
* **/plan**: Zeregin konplexu bat urratsez urrats planifikatzeko agenteak banatu aurretik.
* **/goal**: Zeregin luzeetan agenteek helburua erabat lortu arte etengabe eta sakon lan egin dezaten.
* **/boost**: Pentsamendu sakona, ikuspegi anitzak eta egiaztapen zorrotza behar duten proiektuetarako.
