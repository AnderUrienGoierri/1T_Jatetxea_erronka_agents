# T1 Jatetxea · Komandera Android — UI/UX eta Figma Diseinu Hobekuntza Plana (v2.0)

> **Data:** 2026-11-24  
> **Proiektua:** `zerbitzaria_komandera_android` (T1 Jatetxea · Osteria & Parrilla)  
> **Helburua:** Aplikazioaren UI/UX maila **"garatzaile/IA batek sortutako interfaze funtzional"** batetik **ostalaritzako produktu profesional, moderno eta pertsonalizatu** batera igotzea, **Figma** diseinu-iturri bakar (*Single Source of Truth*) gisa erabiliz eta pantailaz pantaila **Jetpack Compose**-ra eramanez.  
> **Oinarri teoriko eta metodologikoak:**
> - `docs/INTERFAZEEN_GARAPENA_TEORIA_LABURPENA.pdf` (Norman, Nielsen H1–H10, Fitts, Gestalt, 60-30-10, WCAG 2.2 POUR, Constraints)
> - `docs/Claude-rekin_garatzen_b.pdf` (5 urratseko Figma + AI metodologoa, Layer Naming Protocol, pantailaz pantailako migrazioa)
> - `T1_Jatetxea_Diseinu_Sistema_eta_Koloreak.pdf` (*"Kultura Bi, Sukalde Bat"* — Italo-Euskaldun marka identitatea)
> - `design/01_UI_UX_ANDROID_ERROMA_EUSKAL_DISEINUA.md` & `design/02_FIGMA_GIDA_ETA_KOMPONENTEAK.md`
> - `.gemini/FIGMA_DISEINU_ARAUAK.md` (Zero Emoji araua, Auto-Layout derrigorrezkoa, geruza-izendapena)

---

## 1. Erantzun Zuzena: Ondo geratuko da?

**Bai, sekulako jauzi kualitatiboa egingo du — eta oso nabaria izango da emaitza.**

Gaur egungo aplikazioa **funtzionalki oso sendoa** da (API erreala, rolak, 10 barra-kategoria, ahots bidezko bilaketa, hileko egutegia, 3 hizkuntza, 0 warning), baina bisualki oraindik baditu **"IA batek edo garatzaile batek azkar egina"** dela salatzen duten 6 seinale klasiko. Hona hemen zehazki zein diren eta nola kenduko ditugun:

| # | Gaur egungo "IA-itxurako" seinalea | Zergatik nabaritzen da? | Soluzio profesionala (v2.0) |
|---|---|---|---|
| **1** | **Android-eko `Roboto` lehenetsia nonahi** | Ez dago `res/font/` karpetarik ezta `Typography.kt` pertsonalizaturik; prezioak, tituluak eta marka-izena letra-tipo berean daude. | **3 letra-tipo profesional** paketean sartuta (`res/font/`): Editorial Serif (**Cinzel / Playfair Display**) markarako, Humanist Sans (**Inter / Plus Jakarta Sans**) UIrako, eta **JetBrains Mono** prezio, mahai-zenbaki eta tiketetarako (`tnum`). |
| **2** | **"Kutxa kutxaren barruan" (Card-in-Card)** | Adib. `LoginScreen`-en erabiltzaile-txartelak beste txartel handi baten barruan daude; `MahaiaXehetasunak`-en lerroak txartel handi baten barruan. Karga bisuala handitzen du (**Gestalt** urratuz). | Hierarkia lau eta garbia: txartel bakarra maila bakoitzean, **espazio zuri eskuzabala**, 1dp-ko banatzaile finak (`Hairline Dividers`) eta *Eskualde Komuna* neurrian erabilia. |
| **3** | **Gris/Zuri hotza (`#F8FAFC` + `#FFFFFF`)** | Ospitale edo formulario korporatibo baten sentsazioa ematen du, ez goi-mailako Euskal-Italiar jatetxe bero batena. | **Warm Hospitality Light** paleta: **Liho Epela (`#FAF7F2`)** atzealde gisa, **Marmol Zuria (`#FFFFFF`)** txartel goratuetan, **Ertz Harri Epela (`#E6DFD5`)** eta **Arbel Epela (`#1C1917`)** testuetan — **60-30-10 araua** zorrotz aplikatuta (eta `#000000` / `#FFFFFF` mutur hotzak saihestuta). |
| **4** | **Logo sinpleegia (`ic_t1_logo.xml`)** | Gaur egungo logoa borobil gorri bat da, lerro zuzeneko `T1` batekin eta behean "irribarre" hori bat + 2 puntu berderekin. | **T1 Marka-Zigilu Bektore Berria** (Figman sortua, SVG garbia): *Osteria & Parrilla* nortasun modernoa, `ic_t1_logo.xml` eta **Android Adaptive App Icon** (`ic_launcher_foreground.xml`) berrituta. |
| **5** | **Informazio-dentsitate uniformea** | Mahai-txartel guztiek pisu bisual bera dute eta goiburuek/botoiek ez dute erritmo bisual argirik. | **Eskala tipografiko kontrastatua** (zenbaki handi garbiak, etiketa txiki maiuskula finak), **Fitts-en legea (`≥ 48dp`)** erpuru-eremuan (`Bottom Dock`), eta okupatutako mahaietan adierazle azkarrak (egoera ikonoa + testua, inoiz ez kolore hutsez — **WCAG 2.2 POUR**). |
| **6** | **Ikono eta itzal estilo mistoak** | Material3 lehenetsiak itzal gogorrekin edo trazu ezberdinekin. | **Zero Emoji** araua zorrotz beteta; ikono bektorial koherenteak (*Rounded* / *Outline* trazu uniformea) eta 1px-ko ertz dotoreak itzal astunen ordez. |

> **Ondorioa:** Emaitza **Toast POS, Lightspeed edo boutique jatetxe moderno baten** itxurako aplikazioa izango da — garbia, azkarra zerbitzariarentzat (esku bakarrarekin erabiltzeko egokitua), eta 100% koherentea T1 Jatetxearen marka-gidarekin.

---

## 2. Figma MCP Konexioaren Egiaztapena (Dagoeneko Prest!)

Plana prestatu aurretik **Figma MCP (`figma_figma`)** konexioa egiaztatu dugu (`whoami` bidez):

- **Erabiltzailea:** `Ander Urien Telleria` (`and_uri_tel@goierrieskola.org`) — **Saioa hasita eta konektatuta!**
- **Detektatutako Figma Planak / Taldeak:**
  1. **`2MG3`** — `team::1698983143399415470` (*tier: `student`*, Hezkuntza kontua — zure Chrome-eko pantaila-argazkian agertzen den bera)
  2. **`El equipo de and_uri_tel`** — `team::1700183322369263540` (*tier: `starter`*, talde pertsonala)
- **Ohar tekniko garrantzitsua (`seat: "View"`):** `whoami`-k bi taldeetan `seat: "View"` itzultzen du. Fitxategi berria **Drafts**-en (`create_new_file`) sortzean, lehen pausoan egiaztatuko dugu `use_figma` bidez zuzenean editatu dezakegun. Zure kontuko taldearen baimenengatik MCP-k zuzenean sortu ezin badu, zuk Figman fitxategi huts bat sortu eta bere URL-a ematea nahikoa da (`use_figma`-k bertan eraiki dezan). Horrela ez dugu inongo blokeorik izango.
- **Zuk Figmatik kudeatzeko abantaila:** Sortuko dugun fitxategiak **Figma Variables (Light/Dark)**, **Auto-Layout**, **Master Components** eta **Layer Naming Protocol** (`btn*`, `card*`, `lbl*`, `txt*`) izango ditu — beraz, zuk Figman kolore, tarte edo testu bat aldatzean pantaila guztiak automatikoki eguneratuko dira.

---

## 3. Marka Identitatea eta Logo Berriaren Proposamenak

Gaur egungo `ic_t1_logo.xml` logoa ordezkatuko dugu Figman bertan diseinatutako **bektore profesional** batekin, eta 3 bertsio sortuko ditugu (*Marka Nagusia argirako*, *Marka Ilunerako*, eta *Android Launcher Icon*).

Hona hemen aukeratu dezakezun **3 norabide estetiko**:

### A Aukera (Gomendatua): *"Zigilu Gastronomiko Modernoa"* (Artisan Crest)
- **Kontzeptua:** Borobil edo ezkutu geometrikoki orekatu fina, **Chianti (`#881337`)** atzealde aberatsarekin (gradiente leuna `#9F1239 → #6B0F2B`), barruan **serif dotorezko `T1` monograma** (`Cinzel` inspirazioa, `T`-aren besoa eta `1`-aren errematea lotura geometriko sotilarekin) eta behean **urrezko olibondo-hosto / parrillako su-trazu minimalista** (marra bakar jarraitua, 2 puntu solterik gabe).
- **Non nabarmentzen da:** Oso ondo funtzionatzen du bai `192dp` Android ikonoan, bai Login pantailako goiburuan, bai tiketean (monokromoan).

### B Aukera: *"Monograma Arkitektoniko Minimalista"* (Boutique Bistro)
- **Kontzeptua:** Ez dago borobil astunik; **karratu biribildu fina (`Squircle`)** edo ertz bikoitz mehea (`1.5px`), atzealde **Liho Epela (`#FAF7F2`)** argian eta **Chianti (`#881337`)** tipografia + urrezko lerro horizontal txikia azpian (`OSTERIA · PARRILLA`).
- **Non nabarmentzen da:** Goi-mailako jatetxe garaikide / Michelin estiloko estetika ultragarbia.

### C Aukera: *"Gaur Egungo T1 Zigiluaren Bilakaera Findua"*
- **Kontzeptua:** Gaur egungo borobil gorriaren silueta ezaguna mantentzen du (proiektuko JavaFX/Web atalekin jarraitutasun handiena), baina "irribarrea" eta puntu berdeak kendu, `T1` benetako serif bektorial bihurtu, eta urrezko eraztun arkitektoniko fin batekin dotoretzen du.

---

## 4. Diseinu Sistema v2.0 (Estilo Gidaren 7 Atalak)

`INTERFAZEEN_GARAPENA_TEORIA_LABURPENA.pdf` dokumentuko **Estilo Gidaren 7 atalei** eta `T1_Jatetxea_Diseinu_Sistema_eta_Koloreak.pdf` gidari jarraituz:

### 4.1 Kolore-Paleta (60-30-10 Araua + WCAG 2.2 AA/AAA)

> **Kontraesana argituta:** Errepositorioan bi estilo ageri dira — Admin/IT sistemarako *Zinc Dark* (`.gemini/FIGMA_DISEINU_ARAUAK.md`) eta jatetxerako *Italo-Euskaldun Gastronomia* (`T1_Jatetxea_Diseinu_Sistema_eta_Koloreak.pdf`). Zerbitzariaren komanderak **Italo-Euskaldun Gastronomia paleta** erabiliko du (Modu Argia lehenetsita + Modu Iluna aukeran), baina Admin dokumentuko **Zero Emoji, Auto-Layout, Layer Naming eta Monospace zenbakien** zorroztasuna hartuz.

#### Modu Argia — *Warm Hospitality* (Lehenetsia)
| Rola (60-30-10) | Token izena (Figma & Compose) | Hex | Erabilera eta Kontrastea |
|---|---|---|---|
| **%60 Oinarria** | `bg/canvas` (`WarmLinen`) | `#FAF7F2` | Pantailen atzealde beroa (ez `#FFFFFF` itsugarria) |
| **%60 Gainazala** | `bg/surface` (`MarbleWhite`) | `#FFFFFF` | Txartelak, goiburuak, beheko ekintza-barra |
| **%60 Gainazal 2.** | `bg/surface-subtle` (`StoneMist`) | `#F3EFE8` | Bilatzaile-eremuak, chip inaktiboak, input fondoak |
| **%30 Ertzak** | `border/subtle` (`WarmTaupe`) | `#E6DFD5` | 1dp-ko txartel-ertz dotoreak eta banatzaileak |
| **%30 Testua 1.** | `text/primary` (`EspressoInk`) | `#1C1917` | Tituluak eta testu nagusia (Kontrastea **> 16:1 AAA**) |
| **%30 Testua 2.** | `text/secondary` (`WarmSlate`) | `#57534E` | Azpitituluak, guneak, kategoriak (**> 7:1 AAA**) |
| **%30 Testua 3.** | `text/muted` (`StoneMuted`) | `#78716C` | Orduak, ohar lagungarriak (**> 4.6:1 AA**) |
| **%10 Marka** | `brand/primary` (`ChiantiPrimary`) | `#881337` | CTA nagusiak, hautatutako tab-ak, logoa (**> 9.4:1 AAA** zuriarekin) |
| **%10 Marka Hover** | `brand/hover` (`PomodoroHover`) | `#9F1239` | Pultsatze-egoerak eta marka-gradienteak |
| **%10 Azentua** | `brand/gold` (`AntiqueGold`) | `#B45309` | VIP, faktura, erreserbak, detaile gastronomikoak |

#### Modu Iluna — *Evening Service OLED* (Ezarpenetan aukeragarria)
| Token izena | Hex | Erabilera |
|---|---|---|
| `bg/canvas` (`EveningEspresso`) | `#0F0D0E` | Atzealde ilun beroa (ez `#000000` purua) |
| `bg/surface` (`CharcoalWine`) | `#1A1618` | Txartelak eta dialogoak |
| `bg/surface-subtle` (`WarmGraphite`) | `#262023` | Inputak eta elementu sekundarioak |
| `border/subtle` | `#352D31` | 1dp-ko ertz finak |
| `text/primary` | `#F5F2EE` | Testu nagusia (**> 15:1 AAA**) |
| `brand/primary` | `#BE123C` | Chianti argixeagoa modu ilunean irakurgarritasuna bermatzeko |

#### Egoera Semantikoak (Beti **Ikonoa + Testua + Kolorea** — WCAG 2.2 POUR)
| Egoera | Kolore nagusia | Atzealde leuna (Light) | Ikonoa |
|---|---|---|---|
| **Libre / Zerbitzatua** | `#15803D` (Albahaka) | `#DCFCE7` | `CheckCircleOutline` |
| **Okupatuta / Errorea** | `#B91C1C` (Gorri sakona) | `#FEE2E2` | `Restaurant` / `ErrorOutline` |
| **Sukaldean / Zain** | `#C2410C` (Terrakota) | `#FFEDD5` | `Schedule` / `LocalFireDepartment` |
| **Garbitzen / Info** | `#0369A1` (Mediterraneo) | `#E0F2FE` | `CleaningServices` |
| **Erreserbatuta / Kontua** | `#B45309` (Urre zahartua) | `#FEF3C7` | `EventAvailable` / `ReceiptLong` |

### 4.2 Tipografia Sistema (`res/font/` + `Typography.kt` + Figma Text Styles)

Aplikazioak **lineaz kanpo (`res/font/*.ttf`)** txertatutako 3 letra-tipo erabiliko ditu:

| Estiloa | Letra-tipoa | Pisua / Tamaina | Non erabili |
|---|---|---|---|
| `displayBrand` | **Cinzel** (edo **Playfair Display**) | Bold · `24sp` / `28sp` | `T1 JATETXEA` marka-goiburua, splash/login titulua |
| `headlineScreen` | **Plus Jakarta Sans** (edo **Inter**) | Bold · `20sp` (`-0.3sp` tracking) | Pantaila-tituluak (`Aretoko Mahaiak`, `Mahaia 1`) |
| `titleCard` | **Plus Jakarta Sans** / **Inter** | SemiBold · `16sp` | Mahai-zenbakiak, plater-izenak, sekzio-izenburuak |
| `bodyDefault` | **Plus Jakarta Sans** / **Inter** | Regular / Medium · `14sp` | Deskribapenak, guneak, ohar-testuak |
| `labelMicro` | **Plus Jakarta Sans** / **Inter** | SemiBold · `11sp–12sp` (+4% tracking) | Txapak (`OKUPATUTA`), tab-ak, goiburu txikiak |
| `monoPrice` | **JetBrains Mono** | SemiBold / Bold · `15sp–22sp` (`tnum`) | Prezioak (`59.10 €`), tiketa, eskaera `#38`, IP/portuak |

### 4.3 Espaziatzea, Ukipen-Eremuak eta Ergonomia (Fitts + Gestalt)
- **8pt Grid zorrotza:** `4dp`, `8dp`, `12dp`, `16dp`, `20dp`, `24dp`, `32dp`.
- **Fitts-en Legea:** Botoi interaktibo guztiak gutxienez **`48dp` altuera** (`52–56dp` CTA nagusietan eta PIN teklatuan).
- **Erpuru-eremua (*Thumb Zone*):** Zerbitzariak esku bakarrarekin erabiltzen du mugikorra jantokian; horregatik ekintza nagusiak (`Bidali Sukaldera`, `Gehitu Platerak`, `Kobratu`, `Baieztatu`) **beheko dock finkoan** kokatuko dira.
- **Akatsen Prebentzioa (*Slips vs Mistakes*):**
  - Ekintza suntsitzaileak (`Ezabatu lerroa`, `Ezabatu eskaera`, `Saioa itxi`) beti ekintza positiboetatik (`Zerbitzatu`, `Kobratu`) fisikoki bananduta eta berrespen-dialogoarekin.
  - Egoera blokeatuak (`Libre`/`Garbitzen` kobratu gabe) bisualki ezgaituta edo azalpen argiarekin (*Murrizketa fisikoa/logikoa*).

---

## 5. Figma Fitxategiaren Arkitektura (Zuk Figmatik Kudeatzeko)

`Claude-rekin_garatzen_b.pdf` eta `design/02_FIGMA_GIDA_ETA_KOMPONENTEAK.md`-ri jarraituz, fitxategi bakar eta oso antolatua sortuko dugu:  
**Fitxategiaren izena:** `T1 Jatetxea - Komandera Android UI v2.0`

| Orria (Figma Page) | Edukia | Helburua |
|---|---|---|
| **`00_Cover & Brand`** | Proiektu-azala + **Logo berriaren** bertsioak (Light, Dark, Monokromo, Android App Icon 192px) | Zure marka-identitatea eta logoa erabaki eta SVG-ra esportatzeko |
| **`01_Design Tokens`** | **Figma Variables** (`Light` & `Dark` moduak), kolore-taula WCAG ratioekin, tipografia-eskala, erradio eta tarte-gida | Kolore edo letra-tamaina bat ukituta pantaila guztiak eguneratzeko |
| **`02_Components`** | **Auto-Layout Master Components & Variants**: `btnPrimary`, `btnSecondary`, `btnDanger`, `cardTable` (4 egoera), `cardStaff`, `cardOrderLine`, `cardProduct`, `chipFilter`, `badgeStatus`, `searchBarVoice`, `topAppBar`, `bottomActionDock`, `pinKeypad` | Berrerabilgarritasuna eta koherentzia %100 |
| **`03_Screens - Light (390×844)`** | **8 pantaila osoak** erabiltzaile-fluxuan ordenatuta (`01_Login` → `08_Ezarpenak`) | Diseinu nagusia (Modu Argia lehenetsia) berrikusteko |
| **`04_Screens - Dark (390×844)`** | Pantaila nagusiak **Modu Ilunean** (Figma Variable Mode = `Dark` bidez) | Gaueko zerbitzuko itxura egiaztatzeko |
| **`05_Prototype Flow`** | Pantailen arteko fluxu klikagarria (Login → PIN → Mahaiak → Mahaia → Katalogoa → Ordainketa) | Aurkezpenean edo bezeroarekin probatzeko |

**Geruzen izendapen-protokoloa (`Claude-rekin_garatzen_b.pdf`):**
- `btn*` (botoiak: `btnBidaliSukaldera`, `btnKobratu`, `btnEzabatuLerroa`)
- `txt*` (testu-eremuak: `txtBilaketa`, `txtBezeroIzena`)
- `lbl*` (etiketak: `lblGuztira`, `lblMahaiEgoera`)
- `card*` (txartelak: `cardMahaia`, `cardLangilea`, `cardProduktua`)
- `badge*` (egoera-txapak: `badgeOkupatuta`, `badgeBarraModua`)
- `dlg*` (dialogoak/sheet-ak: `dlgPin`, `dlgErreserbak`, `dlgEzarpenak`)

---

## 6. Pantailaz Pantailako Hobekuntza Zehatza (Figma → Jetpack Compose)

### 6.1 `01_Login` + `02_PIN_Modal` (Hasierako Inpresioa)
- **Goiburu editoriala:** Logo berria goi-ezkerrean edo zentratuta tamaina dotorean, `T1 JATETXEA` **Cinzel/Playfair** tipografian + `OSTERIA · PARRILLA` azpititulu fina (ez kutxa laranja astun bat), eta ezarpenen ikono garbia goi-eskuinean.
- **Card-in-Card kentzea:** Erabiltzaile-zerrenda ez da beste kutxa erraldoi baten barruan sartuko; `Guztiak | Jantokia | Barra | Admin` tab-barra garbi baten azpian sekzio-izenburu finak (`JANTOKIA · 4`, `BARRA · 2`, `ADMIN · 1`) eta txartel zuri garbiak ertz sotilarekin + rol-txapa + abatar zirkularra (inicial eleganteekin).
- **Konexio-egoera diskretua:** Behealdean pill trinko eta garbi bat (puntu berde delikatua + `API Konektatuta · 10.0.2.2:5050` edo gorria sakatuta ezarpenetara joateko).
- **PIN Modala (`02_PIN_Modal`):** Behetik igotzen den *Bottom Sheet* / modal dotorea: erabiltzailearen abatarra, 4 puntu animatu (`4px` mugimendua errorea dagoenean), `64dp` borobil minimalistak teklatuan (**Fitts** bikaina).

### 6.2 `03_Mahaiak` (Aretoko Kontrol-Panela)
- **Goiburu trinkoa:** `Aretoko Mahaiak` + `Maite · Jantokia` (edo `Gorka · Barra` txapa bereziarekin); eskuinean 3 ikono garbi (`Erreserbak`, `Ezarpenak`, `Freskatu`).
- **Gune-iragazkiak (`Guztiak`, `Jantoki Nagusia`, `Barra`, `Terraza`, `VIP`):** Pill garbiak, korritze horizontal leunarekin.
- **KPI laburpen-zerrenda modernoa:** Testu-lerro trinko bat izan beharrean, 4 kopuru garbi banatzaile finekin (`10 Libre` | `1 Okupatuta` | `0 Erreserba` | `0 Garbitzen`), zenbakiak `JetBrains Mono`-n.
- **Mahai-txartelak (2 zutabe):**
  - Ezkerreko `4dp`-ko egoera-marra bertikala (edo goiko adierazle fina) + atzealde oso sotila egoeraren arabera.
  - Mahai-zenbaki handi eta argia (`Mahaia 1`), azpian `Jantoki Nagusia · 2 pax`.
  - Egoera-txapa garbia (**ikonoa + testua** — WCAG 2.2), `"Kudeatu →"` kenduta.
  - Mahai okupatuan (datua badago), azpiko lerroan pista lagungarria (`Eskaera aktiboa` edo zenbatekoa `59.10 €`).

### 6.3 `04_Mahaia_Xehetasunak` (Eskaera Kudeaketa)
- **Egoera-hautatzailea:** Segmented control garbia (`Libre | Okupatuta | Garbitzen`), eta eskaera kobratu gabe badago `Libre`/`Garbitzen` bisualki blokeatuta (`0.45` opakutasuna + giltzarrapo txikia), akatsak saihesteko (**Norman Constraints**).
- **Eskaera-lerroen zerrenda laua:** Txartel handiaren barruko txartelak kendu — zerrenda txartel garbi bakarrean (edo lerro bakoitza banatzaile finarekin):
  - Ezkerrean kantitate-kutxa (`1x` monospace) + plateraren izena + kategoria/egoera txapa txikia (`Eskatua`, `Sukaldean`, `Prest`, `Zerbitzatuta`).
  - Eskuinean prezioa lerrokatuta (`9.50 €` monospace).
  - Beheko akzio-lerroa **Fitts-en segurtasunarekin**: ezkerreko muturrean `Ezabatu` (outline gorri fina, soilik `Eskatua` egoeran) eta eskuineko muturrean `Zerbitzatu` (berde argia/solidoa).
- **Beheko ekintza-barra finkoa (*Sticky Bottom Dock*):**
  - Goiko lerroa: `GUZTIRA (5)` + **`59.10 €`** (`JetBrains Mono Bold 22sp`) eta eskuinean `+ Platerak` (tonal) + `Kontua` / `Kobratu`.
  - Beheko CTA nagusia zabalera osoan (`52dp`): **`Bidali Sukaldera (3 plater zain)`** (soilik bidaltzeko platerak daudenean nabarmenduta!).

### 6.4 `05_Eskaera_Katalogoa` (Jantokia & Barra)
- **Barra Modua (`Gorka`) vs Jantoki Modua (`Maite`):** `Gorka`-rentzat goiburuan `Barra Modua · Edariak` txapa diskretua eta 10 edari-kategoriak soilik.
- **Bilatzailea + Ahotsa:** Input borobildu garbia (`bg/surface-subtle`), ezkerrean lupa ikonoa, eskuinean garbitzeko `X` eta **Mikrofono botoi garbia (`48dp`)** ahots bidezko bilaketarako.
- **Produktu-txartel garbiak:** Izena + deskribapen/osagai laburra (1–2 lerro), prezioa `JetBrains Mono`-n, stock-egoera argia (agortuta badago ezgaituta), eta eskuinean **stepper taktila (`- 2 +`)** botoi borobilekin.
- **Saskiko barra flotagarria (*Floating Pill Cart*):** Produktuak hautatzean behean agertzen den pill dotorea (`3 produktu · 28.40 €` → **`Gehitu Eskaerara →`**).

### 6.5 `06_Ordainketa_eta_Tiketa`
- **Tiket bisuala (Paper termiko estilo modernoa):** Txartel zuri garbia goiko marka-logo txikiarekin (`T1 JATETXEA`), lerro-banatzaile puntudunekin (`- - - - -`), eta prezio/BEZ zutabe guztiak **`JetBrains Mono`**-n perfektuki lerrokatuta (`Oinarri zergagarria`, `BEZ %10`, `GUZTIRA`).
- **Ordainketa-metodoa:** 2 txartel taktil handi (`Eskudirua` vs `Txartela`) ikono handi eta hautaketa-ertz Chiantiarekin.
- **Eskudiru azkarra:** `Zehatza`, `20 €`, `50 €`, `100 €` chip azkarrak + itzuli beharreko kanbioa argi nabarmenduta (`Itzultzekoa: 12.60 €`).
- **Azken botoia:** `Baieztatu eta Itxi Mahaia` (Albahaka Berdea `#15803D`, `54dp`).

### 6.6 `07_Erreserbak_Egutegia` eta `08_Ezarpenak`
- **Erreserbak:** Hileko egutegi minimalista (egun hautatua Chianti borobilean, erreserbadun egunak puntu urre/gorri sotilarekin) + behean eguneko erreserben txartelak (ordua `JetBrains Mono`-n, pertsona kopurua, mahaia, egoera) + `+ Sortu Erreserba` fitxa garbia (`Auto-Esleitu` lerro bakarrean mantenduta).
- **Ezarpenak:** 2 txartel bisual txiki **Argia / Iluna** aukeratzeko (miniatura bidez), `EU | ES | EN` segmented control garbia, eta zerbitzari-profilak konexio-probarekin.

---

## 7. Inplementazio Fasen Kronograma (Urra-Urrats)

`Claude-rekin_garatzen_b.pdf`-eko lan-fluxuari jarraituz, **ez dugu kodea ukituko zuk Figmako diseinua onartu arte**:

| Fasea | Deskribapena | Entregagarriak | Berrikuspen Puntua |
|---|---|---|---|
| **Fase 0** | **Erabakiak hartzea** (orain) | Logo aukera, paleta, Figma taldea eta irismena finkatuta | Zure baieztapena |
| **Fase 1** | **Figma: Marka, Logo Berria + Design System** | `00_Cover & Brand` (Logo SVG), `01_Design Tokens` (Variables Light+Dark), `02_Components` | **Checkpoint #1:** Zuk Figman ikusi eta onartu |
| **Fase 2** | **Figma: 8 Pantaila Osoak (Light + Dark + Prototype)** | `03_Screens - Light`, `04_Screens - Dark`, `05_Prototype Flow` | **Checkpoint #2:** Zuk Figman berrikusi eta nahi baduzu ukituak eman |
| **Fase 3** | **Android: Oinarri Bisual Berria** | `res/font/*.ttf`, `Typography.kt`, `Color.kt`, `Theme.kt`, `ic_t1_logo.xml`, `ic_launcher_foreground.xml` | Konpilazio garbia + Git commit |
| **Fase 4** | **Android: Pantailaz Pantailako Migrazioa** | 1) Login + PIN → 2) Mahaiak → 3) Mahaia Xehetasunak → 4) Katalogoa (Jantokia + Barra) → 5) Ordainketa & Tiketa → 6) Erreserbak & Ezarpenak | Pantaila bakoitzean: `clean assembleDebug` + emuladoreko screenshot-a Figmarekin konparatuta + Git commit |
| **Fase 5** *(Aukerakoa)* | **`aldaketa_batzuk.pdf`-eko gehigarriak** | Plater bakoitzeko `oharra` (sukalderako oharrak), Beganoa/Begetarianoa iragazkia, sukaldera bidali gabe kobratzea blokeatzea, Docker C# API berreraikitzea | Funtzio bakoitza probatuta |
| **Fase 6** | **Azken QA, Mugikor Fisikoa & Dokumentazioa** | APK instalatua Emuladorean + Samsung SM-A376B-n, `img/` pantaila-argazki berriak, `README.md` (Figma linkarekin!) eta Demo gidoia eguneratuta | Demoa grabatzeko 100% prest |

---

## 8. Jarraitu Aurretik Erabakitzeko 5 Galdera Azkar

Hau erantzutean berehala ekingo diogu **Fase 1**-i (Figma fitxategia + Logo berria + Design System):

1. **Figma taldea:** Zein taldetan sortu nahi duzu fitxategia?
   - **`2MG3`** (Hezkuntza / Edu kontua — *gomendatua*) edo **`El equipo de and_uri_tel`**?
2. **Logo berriaren norabidea:** Zein nahiago duzu?
   - **A)** *Zigilu Gastronomiko Modernoa* (Chianti ezkutu/borobil findua + `T1` serif monograma + urrezko olibondo/su trazu minimalista) — *Gomendatua*
   - **B)** *Monograma Arkitektoniko Minimalista* (ertz bikoitz mehea, fondo epela, bistro/Michelin estiloa)
   - **C)** *Gaur egungo T1 zigiluaren bilakaera findua*
3. **Modu Argiaren tonua:**
   - **Warm Hospitality (`#FAF7F2` liho epela + `#FFFFFF` txartelak)** — *Gomendatua, jatetxe bero eta profesional itxura*
   - Edo **Irina Zuria estandarra (`#F8FAFC` + `#FFFFFF`)** (*JavaFX estilo-gidaren hex zehatz-zehatza*)?
4. **Tipografia nagusia (UI):**
   - **Plus Jakarta Sans** (*beroagoa, modernoagoa, ostalaritzarako oso egokia*) edo **Inter** (*JavaFX/Web gidarekin %100 berdina*)? *(Bi kasuetan `Cinzel` markarako + `JetBrains Mono` prezio/tiketetarako)*
5. **Irismena (Fase 5 — `docs/aldaketa_batzuk.pdf`-eko gehigarriak):**
   - Lehenengo **UI/UX + Figma + Logo + 8 pantailak** bukatu nahi dituzu (garbi eta azkar demo bideorako), ala diseinuarekin batera gehigarri funtzionalen bat ere sartu nahi duzu (adib. *plater bakoitzeko oharra*, *beganoa/begetarianoa iragazkia*, *sukaldera bidali gabe kobratzea blokeatzea*)?
