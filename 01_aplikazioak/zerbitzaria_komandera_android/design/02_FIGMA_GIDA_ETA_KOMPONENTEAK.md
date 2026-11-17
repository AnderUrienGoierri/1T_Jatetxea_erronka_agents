# T1 Jatetxea Komandera - Figma Diseinu Fitxak eta UI/UX Gida

Gida honek azaltzen du nola antolatu eta diseinatu T1 Jatetxearen Android Komandera mugikorra Figman, "AI-slop" (prototipo hotzak eta generikoak) itxura ekidin eta nortasun propio profesionala emateko.

---

## 1. Diseinuaren Oinarriak (Design Tokens)

### 1.1 Kolore-Paleta (Figma Styles)

1. **Marka Nagusia (Ardoa & Chianti):**
   * `Brand/Chianti-Primary`: `#881337` (Aplikazioaren botoi nagusiak, aukeratutako txartelen profilak)
   * `Brand/Pomodoro-Hover`: `#9F1239` (Feedback haptiko eta sakatze-egoeretarako)
   * `Brand/Burgundy-Dark`: `#4C0519` (Atzeko itzalak eta nabarmentze sakonak)

2. **Azentu Gastronomikoak (Labea & Parmigianoa):**
   * `Accent/Gold-Amber`: `#D97706` (Prezioak, moneta sinboloak, garrantzia duten datuak)
   * `Accent/Parmigiano-Light`: `#FDE68A` (Krema xehetasunak, testu sekundario argiak)
   * `Accent/Gold-Border`: `#FCD34D` (Txapeletako urrezko ertz delikatuak)

3. **Hondoak eta Azalerak (Dark Warm Slate):**
   * `Surface/App-Background`: `#0B0F17` (Beltz sakon arbel ukituarekin, OLED pantailen kontsumoa gutxitzen du)
   * `Surface/Card-Base`: `#161F30` (Mahai eta plater txartelen oinarria)
   * `Surface/Input-Variant`: `#222E42` (PIN teklatua, testu-koadroak)
   * `Surface/Subtle-Stroke`: `#334155` (1px-ko banatze-marra dotoreak, ez gehiegi markatuak)

4. **Sukaldeko eta Zerbitzuko Egoerak (KDS / BDS koherentzia):**
   * `Status/Free-Green`: `#15803D` (Mahai libreak, zerbitzatu diren platerak, sare konexio ona)
   * `Status/Cooking-Orange`: `#C2410C` (Sukaldean martxan dauden komandak, zain dauden mahaiak)
   * `Status/Occupied-Red`: `#DC2626` (Mahai okupatuak, stock agortua, API deskonexioa)
   * `Status/Bar-Blue`: `#0284C7` (BDS edariak, garbitzen dauden mahaiak)

---

## 2. Tipografia Sistema

* **UI eta Funtzionalitaterako:** `Inter` edo `Plus Jakarta Sans`
  * Pisuak: Regular (400), Medium (500), SemiBold (600), Bold (700), Black (900).
* **Markaren Titulua:** `Cinzel` edo `Playfair Display` (Goi-titulurako aukeran) edo `Inter Black (kerning zabalarekin: +2.5sp)`.

### Tipografia Eskala:
* **Brand Title:** 28px / SemiBold edo Black (All Caps, 2.5px tracking)
* **Screen Header:** 20px / Bold
* **Card Title / Plater izena:** 16px / SemiBold
* **Body / Zenbatekoak:** 14px / Regular
* **Status Badges & Pills:** 10px-11px / Bold (All Caps, 1px tracking)
* **Caption & Time:** 12px / Medium

---

## 3. Komponente Nagusien Figma Egitura (Components & Auto-Layout)

### 3.1 Goiburuko Logotipoa (`Header / Logo & Brand`)
* **Isotipoa:** 64x64px-ko bektorial zirkularra.
  * Hondoan gradiente lineala: `#9F1239` -> `#881337`.
  * Urrezko ertza (2.5px `#FCD34D`).
  * "T1" tipografia klasiko zuria zentroan.
  * Beheko arku urreztatua eta oliba berde biak (`#15803D`).
* **Marka Testua:**
  * Lerro nagusia: `T1 JATETXEA` (Zuria) + `[OSTERIA & PARRILLA]` txapa urreztatua ondoan.
  * Azpititulua: `Zerbitzarien Komandera Mugikorra · Euskal Herria & Italia` (`#94A3B8`).

### 3.2 Mahai Txartela (`Card / Table Card`)
* **Auto-Layout:** Bertikala, Padding 14px, Corner Radius 16px.
* **Egoera-adierazleak:**
  * Goiko lerroan: Mahaiaren zenbakia (adib. "Mahaia 4") handi eta argi.
  * Eskuinean: Egoera-txapa (Libre berdez, Okupatuta gorriz, Martxan terrakotaz).
  * Beheko lerroan: Zerbitzariaren izena eta denbora kontagailua (adib. "⏱️ 24 min").
* **Subtle Elevation:** Y=4, Blur=12, Spread=0, Kolorea: `#000000` (%35 opakotasuna). Ertza: 1px `#334155`.

### 3.3 Eskaera Plater Errenkada (`List Item / Dish Row`)
* **Eskaintza garbia:** Plateraren izena, aldaerak/oharrak azpian ("Puntuan, gatzik gabe"), kantitatea biribil urrezko batean.
* **Akzio azkarrak:** Zerbitzatu botoi berde garbia (`#15803D`), ezabatu edo aldatu botoiak.

### 3.4 Egoera Barra Behean (`Footer / Connection Bar`)
* Txartel flotatzailea (floating pill), txoko borobilduak (12px), beira efektu sotila (blur).
* Elementu banatuak:
  * Ezkerrean: Wi-Fi sarearen izena eta gailuaren IPa.
  * Eskuinean: API eta DB egoera adierazle bizia (berdea/gorria).

---

## 4. Figma Fitxategia Antolatzeko Orrialdeak (Pages)

1. **`00_Cover & Brand`**: Logotipoak, isotipoak, italo-euskaldun marka kontzeptua.
2. **`01_Design Tokens`**: Kolore aldagaiak, tipografia eskala, spacing (4px, 8px, 12px, 16px, 24px).
3. **`02_Components`**: Botoiak, txartelak, egoera txapak, zenbaki-teklatua, navbar.
4. **`03_Screens - Mobile (360x800)`**:
   * `01_Login & Erabiltzaile Hautaketa`
   * `02_Mahaiak Mapa / Zerrenda`
   * `03_Eskaera Berria & Produktu Aukeraketa`
   * `04_Mahaiaren Xehetasunak & KDS Egoerak`
   * `05_Ordainketa & Fakturazioa`
5. **`04_Prototypes`**: Zerbitzariaren ohiko fluxua (Login -> Aukeratu Mahaia -> Gehitu Platerak -> Bidali Sukaldera).
