# T1 Jatetxea: Logoa eta Ikus-Identitatea
## Italiar-Euskaldun Kontzeptua (Logo Proposamenak eta Bektoreak)

Dokumentu honek T1 Jatetxearen ikus-identitatea, logoaren garapen kontzeptuala eta JavaFX zein Figma-n erabiltzeko SVG bektoreak zehazten ditu.

---

## 1. Izen Proposamenak eta Eslogana

Gure jatetxeak Italiako tradizio sukaldaria (pasta freskoa, labe tradizionala, risottoak, oliba-olioa) eta Euskal Herriko nortasuna (euskal txingarra, Idiazabal, arrain eta haragi gorena) uztartzen ditu.

* **Izen Ofiziala:** `T1 Jatetxea`
* **Azpi-marka / Kontzeptua:**
  * Aukera A (Gomendatua): **"T1 Jatetxea · Osteria & Parrilla"**
  * Aukera B: **"T1 Jatetxea · Trattoria Euskalduna"**
  * Aukera C: **"T1 Jatetxea · Napoli & Donostia"**
* **Eslogana:** *"Bi tradizio, artisautza bakarra"* (Due tradizioni, una sola passione)

---

## 2. Logoaren Sinbologia eta Elementu Grafikoak

Logoak bi mundu gastronomikoen ikur nagusiak uztartzen ditu:
1. **Italiako elementuak:** Oliba adarra (bakea eta kalitatea), labeko garra/terrakota eta ardo kopa.
2. **Euskal elementuak:** Lauburuaren lerro biribildu dotoreak, haritz hostoa eta sukaldeko sua/txingarra.
3. **Konposizio orokorra:**
   * Isotipoa (Ikurra): Koroa zirkularra, alde batean oliba hostoak (Italia) eta bestean haritz hostoak (Euskadi), erdialdean "T1" ikur tipografiko sendoa duela.
   * Tipografia: Letra serif sendoa eta modernoa ("Cinzel" / "Playfair" estilokoa), azpian sans-serif garbi batekin ("JATETXEA · OSTERIA & PARRILLA").

---

## 3. SVG Bektore Ofiziala (Figma eta JavaFX-rako)

Kopiatu eta erabili SVG bektore hau zuzenean Figma-n (`Paste as SVG`) edo gorde `logo_t1.svg` gisa `desktop_javafx/src/main/resources/eus/goierri/jatetxea/images/` karpetan:

```xml
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 400 120" width="400" height="120">
  <defs>
    <!-- Gradiente aberatsa Chianti eta Euskal Ardo Gorriarekin -->
    <linearGradient id="ardoGradientea" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" stop-color="#9F1239" />
      <stop offset="100%" stop-color="#881337" />
    </linearGradient>
    <linearGradient id="olibaGradientea" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" stop-color="#16A34A" />
      <stop offset="100%" stop-color="#15803D" />
    </linearGradient>
  </defs>

  <!-- ISOTIPOA (EZKERREAN): EMBLema ZIRKULARRA -->
  <g transform="translate(10, 10)">
    <!-- Kanpoko eraztuna -->
    <circle cx="50" cy="50" r="46" fill="url(#ardoGradientea)" stroke="#FCD34D" stroke-width="2" />
    <circle cx="50" cy="50" r="41" fill="none" stroke="#FFFFFF" stroke-opacity="0.3" stroke-width="1.5" stroke-dasharray="3,3" />

    <!-- T1 Ikurra zentroan -->
    <text x="50" y="60" font-family="'Segoe UI', 'Cinzel', serif" font-size="34" font-weight="900" fill="#FFFFFF" text-anchor="middle" letter-spacing="-1">T1</text>
    
    <!-- Azpiko adar txikia (Oliba / Haritza) -->
    <path d="M 32 75 Q 50 82 68 75" fill="none" stroke="#FCD34D" stroke-width="2.5" stroke-linecap="round" />
    <circle cx="43" cy="78" r="2.5" fill="#16A34A" />
    <circle cx="57" cy="78" r="2.5" fill="#16A34A" />
  </g>

  <!-- LOGOTIPOA (ESKUINEAN): TESTUA -->
  <g transform="translate(125, 20)">
    <!-- Izenburu Nagusia -->
    <text x="0" y="44" font-family="'Segoe UI', 'Cinzel', serif" font-size="36" font-weight="800" fill="#881337" letter-spacing="1">T1 JATETXEA</text>
    
    <!-- Azpititulua (Italo-Euskaldun kontzeptua) -->
    <text x="2" y="68" font-family="'Segoe UI', 'Inter', sans-serif" font-size="12" font-weight="700" fill="#15803D" letter-spacing="3.5">OSTERIA &amp; PARRILLA</text>
    
    <!-- Marra bereizlea urreztatua -->
    <rect x="2" y="76" width="220" height="2" fill="#FCD34D" rx="1" />
    
    <!-- Xehetasun txikia -->
    <text x="2" y="92" font-family="'Segoe UI', 'Inter', sans-serif" font-size="10" font-style="italic" fill="#64748B">Euskal Herria &amp; Italia · Sukalde Tradizionala</text>
  </g>
</svg>
```

---

## 4. Logoaren Erabilera-Aldaerak (Variants)

1. **Logo Nagusia (Koloretan):**
   * Hondo txurietan edo argietan (`#FFFFFF`, `#F8FAFC`).
   * "T1 JATETXEA" `#881337` (ardo gorria), azpititulua `#15803D` (albahaka berdea).
2. **Logo Monokromatiko Zuria (Negative):**
   * Hondo ilunetan (`#881337` goiburuan edo `#1E293B` pantailan).
   * Elementu guztiak zuriak (`#FFFFFF`), xehetasun urreztatuekin (`#FCD34D`).
3. **Isotipo Bakarra (App Icon / Favicon):**
   * 64x64, 128x128 edo 256x256 px karratua.
   * "T1" zirkulu gorriaren barruan oliba/haritz adarrarekin.
   * JavaFX-ko leihoaren goiko izkinan (`stage.getIcons().add(...)`) eta mahaigaineko ikonoan erabiltzeko.

---

## 5. Nola Integratu JavaFX Aplikazioan

### 5.1 Leihoaren Ikonoa (Stage Icon)
`desktop_javafx/src/main/java/eus/goierri/jatetxea/App.java` fitxategian:
```java
Image ikonoa = new Image(App.class.getResourceAsStream("images/t1_icon.png"));
stage.getIcons().add(ikonoa);
```

### 5.2 Goiko Nabigazio Barran (`barra-view.fxml`, `sukaldea-view.fxml`, `biltegia-view.fxml`)
FXML-an irudia zuzenean integratzeko:
```xml
<HBox alignment="CENTER_LEFT" spacing="12.0" styleClass="top-bar">
    <ImageView fitHeight="38.0" fitWidth="38.0" pickOnBounds="true" preserveRatio="true">
        <image>
            <Image url="@images/t1_icon.png" />
        </image>
    </ImageView>
    <Label styleClass="brand-label" text="T1 Jatetxea" />
    <Label style="-fx-text-fill: #94a3b8; -fx-font-size: 18px;" text="|" />
    <Label styleClass="title-label" text="Sukaldeko KDS Pantaila" />
    ...
</HBox>
```

