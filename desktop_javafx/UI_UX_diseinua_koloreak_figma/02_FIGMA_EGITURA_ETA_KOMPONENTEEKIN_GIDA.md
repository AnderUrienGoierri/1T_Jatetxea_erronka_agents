# Figma Proiektuaren Gida eta Komponente Egitura
## T1 Jatetxea (Sukaldea, Barra eta Biltegia)

Dokumentu honek urratsez urrats azaltzen du nola antolatu eta eraiki Figma proiektua gure JavaFX aplikazioarekin (KDS, BDS eta Stock) 1:1 baliokidetasuna lortzeko.

---

## 1. Figma Orrialdeen Egitura (Pages Structure)

Figma fitxategian gomendatzen den orrialde-banaketa:

1. **`Cover & Info`**: Proiektuaren izena, taldekideak, data eta kontzeptua ("T1 Jatetxea: Italiar-Euskaldun KDS / BDS").
2. **`Design Tokens & Styles`**: Kolore-paleta, tipografia-estiloak, ertz-erradioak (border-radius) eta itzalak (drop shadows).
3. **`UI Components (Library)`**: Botoiak, txapak (badges), taulak, txartelak, sarrera-eremuak (inputs) eta nabigazio-barra.
4. **`Pantailak - Desktop (1180x750)`**:
   * `01_Sukaldea_KDS`: Sukaldeko eskaera aktiboak eta platerak.
   * `02_Barra_BDS`: Barrako edariak eta kafeak.
   * `03_Biltegia_Stock`: Lehengaien stocka, alertak eta albaranen sarrera.
5. **`Modalak & Dialogs`**:
   * `Oharrak_Modala`: Eskaera-lerroen oharrak (txipak + testua).
   * `Albarana_Sortu_Modala`: Lehengaien sarrera berria erregistratzeko.

---

## 2. Figma Styles (Koloreak eta Tipografia)

Sortu Figma-n ondorengo **Color Styles** eta **Text Styles** izen hauekin (Figma Team Library-n partekatu daitezkeenak):

### Color Styles
* `Brand / Primary` -> `#881337`
* `Brand / Hover` -> `#9F1239`
* `Brand / Dark` -> `#4C0519`
* `Success / Main` -> `#15803D`
* `Success / Light` -> `#DCFCE7`
* `Success / Border` -> `#86EFAC`
* `Warning / Main` -> `#C2410C`
* `Warning / Light` -> `#FEF3C7`
* `Warning / Border` -> `#FCD34D`
* `Info / Main` -> `#0284C7`
* `Info / Light` -> `#E0F2FE`
* `Danger / Main` -> `#DC2626`
* `Danger / Light` -> `#FEE2E2`
* `Neutral / App Bg` -> `#F8FAFC`
* `Neutral / Surface Card` -> `#FFFFFF`
* `Neutral / Border` -> `#E2E8F0`
* `Neutral / Text Main` -> `#1E293B`
* `Neutral / Text Muted` -> `#64748B`

### Text Styles
* `Display / Logo` -> Font: Inter/Cinzel, 22px, Bold (700)
* `Heading / H1 Title` -> Font: Inter/Segoe UI, 18px, Bold (700)
* `Heading / H2 Card` -> Font: Inter/Segoe UI, 15px, Bold (700)
* `Body / Medium Bold` -> Font: Inter/Segoe UI, 13px, SemiBold (600)
* `Body / Regular` -> Font: Inter/Segoe UI, 13px, Regular (400)
* `Caption / Small` -> Font: Inter/Segoe UI, 11px, Regular (400)
* `Badge / Label` -> Font: Inter/Segoe UI, 11px, Bold (700), All Caps (aukerakoa)
* `KPI / Metric` -> Font: Inter/Segoe UI, 26px, Bold (700)

---

## 3. Komponente Nagusiak (Figma Components)

### 3.1 Goiko Nabigazio Barra (`TopBar`)
* **Neurriak:** Width: 1180px, Height: 60px.
* **Auto-Layout:** Horizontal, Spacing: 16px, Padding: (Vertical: 12px, Horizontal: 24px).
* **Elementuak:**
  1. `Brand Label`: "T1 Jatetxea" (Kolorea: `Brand/Primary`, 22px Bold).
  2. `Banatzailea`: "|" (`#94A3B8`).
  3. `Title Label`: "Sukaldeko KDS Pantaila" / "Barrako BDS Pantaila" / "Biltegiko Stock Kudeaketa".
  4. `Spacer`: Fill container (Space-between).
  5. `NavTabs (Segmented Control)`:
     * `Sukaldea (KDS)`
     * `Barra (BDS)`
     * `Biltegia (Stocka)`
  6. `Auto-Refresh Checkbox`: "Auto-Freskatu (10s)".
  7. `Freskatu Botoia`: Secondary button.

### 3.2 Botoiak (`Button Component`)
Sortu komponente bakarra aldaerekin (**Variants**):
* **State:** `Default`, `Hover`, `Active`, `Disabled`
* **Variant / Type:**
  * `Primary`: Background: `Brand/Primary`, Text: White, Corner Radius: 6px.
  * `Secondary`: Background: `#F1F5F9`, Text: `#334155`, Border: `#CBD5E1`.
  * `Success` ("Eginda" / "Zerbitzatua"): Background: `Success/Main` (`#15803D`), Text: White.
  * `Warning` ("Martxan"): Background: `Warning/Main` (`#C2410C`), Text: White.
  * `Danger` ("Ezabatu" / "Alerta"): Background: `Danger/Light`, Text: `Danger/Main`.

### 3.3 Egoera Txapak (`Status Badge`)
KDS/BDS tauletan plateren egoera bereizteko:
* **`Zai (Pending)`**:
  * Background: `#FEF3C7` (Urre argia)
  * Text: `#92400E` (Amber iluna)
  * Border: 1px `#FCD34D`
  * Radius: 4px, Padding: 4px 10px
* **`Martxan (Cooking)`**:
  * Background: `#FFEDD5` (Laranja argia)
  * Text: `#C2410C` (Terrakota iluna)
  * Border: 1px `#FDBA74`
  * Radius: 4px, Padding: 4px 10px
* **`Prestatua / Eginda (Ready)`**:
  * Background: `#DCFCE7` (Berde argia)
  * Text: `#15803D` (Berde sakona)
  * Border: 1px `#86EFAC`
  * Radius: 4px, Padding: 4px 10px
* **`Zerbitzatua (Served)`**:
  * Background: `#E0F2FE` (Urdin argia)
  * Text: `#0369A1` (Urdin iluna)
  * Border: 1px `#BAE6FD`
  * Radius: 4px, Padding: 4px 10px

### 3.4 Taularen Egitura (`DataTable`)
* **Header Row**: Height 40px, Background: `#F1F5F9`, Text: `#475569`, 12px Bold.
* **Row Normal**: Height 38px, Background: `#FFFFFF`, Border-bottom: 1px `#F1F5F9`.
* **Row Selected**: Height 38px, Background: `#881337` (Ardo gorria), Text: `#FFFFFF` (Zuria).
* **Row Alert**: Height 38px, Background: `#FEF2F2` (Gorrixka stock baxuetan), Text: `#991B1B`.

---

## 4. Frame Diseinu Nagusiak (Pantailetako Layout-a)

### Frame 1: Sukaldeko KDS Pantaila (`1180 x 750 px`)
* **Ezkerraldea (45% zabalera - 500px):**
  * Txartel panela (`Card Panel`):
    * Goiburua: "Eskaera Aktiboak (Sukaldean)" + "X eskaera" etiketa ardoa.
    * Taula: `# ID`, `Mahaia`, `Zerbitzaria`, `Egoera`, `Platerak`, `Ordua`.
    * Beheko botoia: "Eskaera Osoa Eginda Markatu" (Berdea).
* **Eskuinaldea (55% zabalera - 610px):**
  * Txartel panela (`Card Panel`):
    * Goiburua: "Eskaera #X - Mahaia Y" + "Z plater".
    * Taula: `Platera`, `Kop`, `Sukaldeko Oharrak`, `Egoera`.
    * Beheko botoiak: `[Zai jarri]` (Grisa) | `[Martxan jarri]` (Laranja) | `[Eginda markatu]` (Berdea).

### Frame 2: Barrako BDS Pantaila (`1180 x 750 px`)
* Sukaldeko egitura berbera, baina edarientzat espezifikoa:
  * Ezkerraldean: "Eskaera Aktiboak (Barran)" (edariak dituzten eskaerak soilik).
  * Eskuinaldean: "Hautatutako Eskaeraren Edariak".
  * Zutabeak: `Edaria`, `Kop`, `Barrako Oharrak`, `Egoera`.
  * Ekintzak: `[Zai jarri]` | `[Martxan jarri]` | `[Zerbitzatuta markatu]`.

### Frame 3: Biltegiko Stock Pantaila (`1180 x 750 px`)
* **Goiko KPI Txartelak (HBox horizontal 3 zutabez):**
  * Txartel 1: "Guztira Lehengaiak" (Zenbaki handia: 18).
  * Txartel 2: "Stock Normala" (Zenbaki handia berdea: 14).
  * Txartel 3: "Stock Kritikoa / Gutxienekoa" (Zenbaki handia gorria: 4).
* **Erdialdean:**
  * Bilaketa kutxa (Search input) + Kategoria iragazkiak (Haragia, Arraina, Barazkiak, Likidoak, Orea/Irina, Esnekiak).
  * Lehengaien taula stock-barra grafikoarekin.
* **Behealdean:**
  * Albaranen bidezko sarrerak erregistratzeko inprimaki panela.

---

## 5. Figmatik JavaFX-rako Garapen Fluxua

1. **Auto-Layout erabili beti:**
   * Figma-ko `Auto-Layout (Horizontal)` -> JavaFX `HBox`.
   * Figma-ko `Auto-Layout (Vertical)` -> JavaFX `VBox`.
   * Figma-ko `Auto-Layout (Space-between)` -> JavaFX `Region HBox.hgrow="ALWAYS"`.
2. **Padding eta Spacing zenbaki biribilak erabili:**
   * Padding-ak: 8px, 12px, 16px, 24px.
   * Spacing-ak: 8px, 12px, 16px.
   * Border radius: 6px (botoiak, eremuak), 8px (txartelak).
3. **Esportatu beharreko irudiak:**
   * Logo bektoriala SVG formatuan esportatu.
   * Ikonoak (freskatu, bilatu, abisua) SVG edo PNG garbian esportatu `desktop_javafx/src/main/resources/eus/goierri/jatetxea/icons/` karpetara.

