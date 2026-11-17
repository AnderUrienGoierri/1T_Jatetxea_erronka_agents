# T1 Jatetxea - Android Komandera UI/UX Diseinu Sistema eta Eraldaketa Plana

Dokumentu honek T1 Jatetxearen Android komandera mugikorrerako interfaze profesional, pertsonalizatu eta modernoa definitzen du, txantiloi generikoen edo "IA itxurako" (AI-slop / Tailwind default) diseinu hotzen itxura ezabatuz.

T1 Jatetxearen marka-nortasuna (**"Kultura Bi, Sukalde Bat": Euskal Herria & Italia**) aplikazio osora hedatzen du, JavaFX mahaigaineko diseinu-sistemarekin erabateko koherentzia lortuz.

---

## 1. Gaur Egungo Diagnostikoa eta "AI Itxura" Desegiteko Arrazoiak

Gaur egungo aplikazioak hainbat akats estetiko eta funtzional ditu:
1. **Kolore generikoak eta hotzak:** Slate 900 (`#0F172A`) iluna eta anbar/laranja elektrikoa (`#F59E0B`). Hau AI bidez sortutako dashboard prototipo generikoetan (Tailwind UI lehenetsia) errepikatzen den patroia da, ez duena jatetxe baten berotasun, ardo, sukalde edo gastronomiako kalitatea islatzen.
2. **Markarik gabeko ikono orokorrak:** Material Icons estandarrak soilik erabiltzen dira (zirkulu marroi batean sardeska eta labana generikoa). T1 markaren identitatea, tipografia zaindua eta benetako logotipoa falta dira.
3. **Hierarchy tipografiko ahula:** Letra-tamaina eta pisu lehenetsiak, lerrokatze lauak eta ñabardura faltak aplikazioa "garatzaile-prototipo" itxuran uzten dute.
4. **Txartelen gainkarga eta forma zurrunak:** Txartel beltz lauak ertz arruntarekin; ez dago sakontasunik (elevation, subtle border stroke, gradient delikatuak).

---

## 2. Diseinu Sistema Berria (Italo-Euskaldun Dark & Warm)

Android aplikazioa jatetxeko zerbitzariek argi gutxiko edo argi aldakorreko jangelan erabili behar dutenez, **Dark Mode dotore eta gastronomiko bat** aplikatuko da, ez gris hotza, baizik eta **Arbel beltza, Chianti ardo gorria eta Urre epela**.

### A. Koloreen Eskala Nagusia

| Funtzioa | Izena | Hex Kodea | Deskribapena eta Erabilera |
|---|---|---|---|
| **Primary (Marka)** | Chianti Gorria | `#881337` / `#9F1239` | Goiburuak, botoi nagusiak, hautatutako elementuak, akzio primarioak |
| **Secondary (Azentua)** | Urre / Parmigiano | `#D97706` / `#F59E0B` | Xehetasunak, distirak, prezioak, garrantzi bereziko testuak |
| **Success / Zerbitzatua** | Albahaka Berdea | `#15803D` / `#22C55E` | Prest dauden platerak, mahai libreak, konexio egokia |
| **Warning / Martxan** | Toskana Terrakota | `#C2410C` / `#EA580C` | Sukaldean martxan dauden platerak, erreserbatutako mahaiak |
| **Info / Barra** | Mediterraneo Urdina | `#0284C7` / `#38BDF8` | Edariak, kafeak, barrako zerbitzua |
| **Danger / Agortua** | Pomodoro Alerta | `#DC2626` / `#EF4444` | Deskonektatuta, stock agortua, ezabatze ekintzak |

### B. Atzeko Planoak eta Azalerak (Surfaces & Depth)

* **Background (Hondo Orokorra):** `#0B0F17` (Beltz sakon arbel ukituarekin, OLED aurrezlea eta begientzat leuna)
* **Surface (Txartelak eta Xaflak):** `#161F30` (Koral/ardo azentu ñimiñoarekin edo harri iluna)
* **Surface Variant / Input:** `#222E42`
* **Subtle Stroke (Ertz Dotoreak):** `#334155` edo `%15` Chianti/Urre opakotasunarekin. Ertzek plastikotasuna kentzen diote eta kalitate sentsazioa ematen dute.

---

## 3. Logotipo eta Ikur Egitura Profesionala

Material Icons generikoaren ordez (`Icons.Default.Restaurant`), T1 Jatetxearen benetako ikus-identitatea sartuko dugu:
* **Vector Isotipoa (`ic_t1_logo.xml`):**
  * Zirkulu dotorea ardo gorriaren gradientearekin (`#9F1239` -> `#881337`).
  * Urrezko eta marra zuriko eraztun bikoitza.
  * Serif / Klasiko modernoko **T1** tipografia nagusia.
  * Oliba eta euskal haritzaren hostoen koroa sinbolikoa urre eta albahaka berdez (`#15803D`).
* **Goiburu Tipografikoa:**
  * Titulua: `T1 JATETXEA` (Kerning zabalarekin, dotorea).
  * Leloa: `OSTERIA & PARRILLA` (Albahaka berdean edo urrezko kapsulan).
  * Azpititulua: `Zerbitzarien Komandera Mugikorra`.

---

## 4. Zerbitzari-Esperientzia (UX) Hobekuntzak Komanderan

1. **Konexio eta Sare Egoeraren Barra Profesionala:**
   * Beheko barra erdi-garden (frosted glass efektua) bihurtu.
   * "Puntu gorri/berde" sinplearen ordez, badge estilizatua: Wi-Fi izena, IPa eta API egoera argi bereizita.
2. **Langile Hautaketa (Login Screen):**
   * Avatar zirkular pertsonalizatuak rolaren arabera (Zerbitzaria, Sukaldaria, Kudeatzailea).
   * Feedback haptikoa eta animazio arinak hautatzerakoan.
3. **Mahaien Mapa eta Egoerak (MahaiakScreen):**
   * Mahai txartelak jangelako sentsazioa ematen duten forma biribildu eta "chip" adierazleekin.
   * Denbora kontagailua (mahaia noiztik dagoen irekita).
4. **Eskaeren Kudeaketa (EskaeraScreen & Xehetasunak):**
   * Plater motak bereizteko ikono espezifikoak (Entsalada, Pasta/Pizza, Haragia/Arraina, Postreak, Edariak).
   * KDS/BDS egoeren etiketa koloreztatuak (Zai, Martxan, Eginda) JavaFX sistemarekin 1:1 bat etorriz.

---

## 5. Implementazio Plana (Fitxategiz Fitxategi)

1. `app/src/main/res/drawable/ic_t1_logo.xml`: Bektorial profesionala txertatu.
2. `app/src/main/java/eus/goierri/jatetxea/komandera/ui/theme/Color.kt`: Chianti, Urre, Terrakota eta Albahaka koloreak ofizialki ezarri.
3. `app/src/main/java/eus/goierri/jatetxea/komandera/ui/theme/Theme.kt`: Material 3 kolore-eskema gaurkotu gastronomia ilun aberatsera.
4. `app/src/main/java/eus/goierri/jatetxea/komandera/ui/screens/LoginScreen.kt`:
   * Goiburua eta logo berria integratu.
   * Langile-txartelen diseinu gastronomiko modernoa jarri.
   * Sareko egoera-panelaren estiloa berritu.
5. Gainerako pantailak (`MahaiakScreen`, `EskaeraScreen`): Kolore eta estilo berriekin lerrokatu.
