# T1 Jatetxea (Italiar-Euskaldun Diseinu Sistema)
## Kolore Paleta, Tipografia eta UI/UX Gida Osoa

Dokumentu honek T1 Jatetxearen nortasun grafikoa definitzen du, **Euskal Herriko** eta **Italiako** gastronomia-tradizio aberatsen arteko fusiotik sortutako jatetxe kontzeptu modernorako.

---

## 1. Nortasuna eta Kontzeptua: "Kultura Bi, Sukalde Bat"

Euskal eta Italiar sukaldaritzek oinarrizko balio bera dute: **lehengaiaren kalitatearekiko errespetu gorena**, artisautza (euskal ikatza/parrilla eta egurrezko italiar labea), eta mahaiaren inguruko elkarretaratze soziala.

* **Euskal ukituak:** Haritza, granitoa/arbela, sagardoa, txakolina, Idiazabal gazta, arrain freskoa eta ikurrinaren koloreak (gorria, berdea, zuria).
* **Italiar ukituak:** Terracotta toskanarra, oliba-olioa, albahaka freskoa, Parmigianoa, Chianti ardoa eta espressoaren krema.

Interfaze grafikoak (**Sukaldeko KDS**, **Barrako BDS** eta **Biltegiko Stock Kudeaketa**) lan-ingurune azkar, garbi eta oso intuitibo bat eskaini behar du, sukaldeko eta barrako argitasun eta erritmo bizian akatsik gabe funtzionatzeko.

---

## 2. Kolore Paleta Ofiziala (Italo-Euskalduna)

### A. Kolore Nagusiak (Brand & Primary)

| Izena | Hex Kodea | RGB | JavaFX Aldagaia | Erabilera |
|---|---|---|---|---|
| **Chianti & Ardo Gorria** | `#881337` | `136, 19, 55` | `-fx-color-brand-primary` | Goiburuak, logoa, botoi nagusiak, nabigazio fitxa aktiboa |
| **Pomodoro Rustico** | `#9F1239` | `159, 18, 57` | `-fx-color-brand-hover` | Botoien *hover* efektua, abisu garrantzitsuak |
| **Erromatar Borgoina** | `#4C0519` | `76, 5, 25` | `-fx-color-brand-dark` | Testu nabarmenduak, kontraste altuko elementuak |

### B. Bigarren Mailako Koloreak (Natura & Freskotasuna)

| Izena | Hex Kodea | RGB | JavaFX Aldagaia | Erabilera |
|---|---|---|---|---|
| **Baso & Albahaka Berdea** | `#15803D` | `21, 128, 61` | `-fx-color-success` | "Eginda" / "Zerbitzatua" botoiak, stock osasuntsua |
| **Oliba / Salvia Argia** | `#DCFCE7` | `220, 252, 231` | `-fx-color-success-bg` | "Eginda" txapen atzeko planoa, baieztatze koadroak |
| **Salvia Borda** | `#86EFAC` | `134, 239, 172` | `-fx-color-success-border`| Berde txapen ertzak eta marra banatzaileak |

### C. Azentu eta Alerta Koloreak (Labea, Gazta & Barra)

| Izena | Hex Kodea | RGB | JavaFX Aldagaia | Erabilera |
|---|---|---|---|---|
| **Toskana Terrakota / Laranja** | `#C2410C` | `194, 65, 12` | `-fx-color-warning` | "Martxan" botoiak, sukaldeko prestaketa lanak |
| **Parmigiano Urre Argia** | `#FEF3C7` | `254, 243, 199` | `-fx-color-warning-bg` | Oharrak, eskaeren abisu-panelak, "Zai" egoera |
| **Urre Borda** | `#FCD34D` | `252, 211, 77` | `-fx-color-warning-border`| Oharren koadroen ertzak |
| **Mediterraneo / BDS Urdina** | `#0284C7` | `2, 132, 199` | `-fx-color-info` | Barrako BDS elementuak, edariak, aldaketa-botoiak |
| **Urdin Argia** | `#E0F2FE` | `224, 242, 254` | `-fx-color-info-bg` | BDS edarien txapen atzeko planoa |
| **Alerta Gorria** | `#DC2626` | `220, 38, 38` | `-fx-color-danger` | "Stock agortua", "Ezabatu", konexio eza |
| **Alerta Gorri Argia** | `#FEE2E2` | `254, 226, 226` | `-fx-color-danger-bg` | Alerta gorrien atzeko planoa |

### D. Oinarrizko Neutroak (Irina, Arbela & Papera)

| Izena | Hex Kodea | RGB | JavaFX Aldagaia | Erabilera |
|---|---|---|---|---|
| **Irina Txuria (Background)** | `#F8FAFC` | `248, 250, 252` | `-fx-color-bg-app` | Aplikazioaren atzeko plano orokorra |
| **Marmol Zuria (Surface)** | `#FFFFFF` | `255, 255, 255` | `-fx-color-bg-card` | Txartelak, taulak, modala, goiko barra |
| **Egur/Harri Borda (Border)** | `#E2E8F0` | `226, 232, 240` | `-fx-color-border-subtle` | Marra banatzaileak, taulako errenkadak |
| **Arbel Grisa (Text Primary)** | `#1E293B` | `30, 41, 59` | `-fx-color-text-main` | Izenburuak, testu nagusia (WCAG AAA kontrastea) |
| **Laino Grisa (Text Secondary)**| `#64748B` | `100, 116, 139` | `-fx-color-text-muted` | Azpitituluak, unitateak, orduak, laguntza testuak |

---

## 3. Tipografia Eskala (Typography Scale)

JavaFX eta Figmarako gomendatutako letra-tipoak:
* **UI eta Funtzionalitaterako:** `Segoe UI` (Windows bertakoa), `Inter` (Figma modernoetan) edo `system-ui`.
* **Goiburu artistiko edo Logoarentzat (aukerakoa):** `Cinzel` edo `Playfair Display` (Italiar dotorezia klasikoa euskal indarrarekin).

| Maila | Tamaina | Pisua | Erabilera JavaFX-en |
|---|---|---|---|
| **H1 - Goiburu Nagusia** | 20px - 22px | Bold (700) | Pantaila bakoitzaren izenburua (`.title-label`) |
| **H2 - Atalen Izenburua** | 16px - 18px | SemiBold (600) | Txartelen goiburuak (`.card-title`) |
| **H3 - Plateren Izenak** | 14px - 15px | Bold (700) | Tauletako eta txarteletako produktuak |
| **Gorputza (Body)** | 13px | Regular (400) | Testu orokorra, taulako gelaxkak |
| **Txiki / Xehetasuna** | 11px - 12px | Regular / Italic | Oharrak, orduak, laguntzaile testuak |
| **Txapa / Badge** | 11px - 12px | Bold (700) | Egoera txapak (uppercase edo nabarmendua) |
| **KPI Balioak** | 24px - 28px | Bold (700) | Biltegiko stock zenbaki handiak |

---

## 4. Irisgarritasuna eta Kontraste Araua (WCAG 2.1)

Pantaila profesionaletan (KDS/BDS) maiz gertatzen den arazo bat letren eta atzeko planoaren arteko kontraste eskasa da (adibidez: letra txuria hondo argian).

**Arau zorrotzak:**
1. **Atzeko plano argietan (`#FFFFFF`, `#F8FAFC`, `#FEF3C7`):**
   * Testuaren koloreak **beti** iluna izan behar du: `#1E293B` (testu nagusia), `#92400E` (oharretarako marroi/amber iluna) edo `#15803D` (berde iluna).
   * **Inoiz ez** erabili testu txuria (`#FFFFFF`) edo gris argia hondo hori/laranja/berde argietan.
2. **Atzeko plano ilunetan (`#881337`, `#15803D`, `#C2410C`):**
   * Testuaren koloreak **beti** zuria izan behar du: `#FFFFFF` (Bold).
3. **Errenkada hautatuen estiloa (Table selection):**
   * Taulan errenkada bat hautatzean, atzeko planoak `#881337` (ardo gorria) hartzen badu, testu guztiak zuri bihurtu behar dira (`#FFFFFF`), eta txapek beren kontrastea mantendu behar dute.

---

## 5. UI/UX Osagaien Fitxak eta Egoerak

### 5.1 Sukaldeko KDS Txartelak
* **Zai (Pending):** Hondoa zuri/arbel argia, egoera txapa horia (`#FEF3C7` hondoa, `#92400E` testua).
* **Martxan (Cooking):** Terrakota/laranja kolorea (`#C2410C`), sukaldeko langileak begi-bistaz erraz ikusteko labean edo sutan dagoela.
* **Eginda (Done):** Berde freskoa (`#15803D`), zerbitzariari platera pasatzeko prest dagoela adieraziz.

### 5.2 Barrako BDS Txartelak
* **Edaria (Drink):** Mediterraneo urdin tonua (`#0284C7`), edariak eta kafeak plateretatik bereizteko.
* **Barrako Oharrak:** Txapa amber berezia (adibidez: "Izotzik gabe", "Kafesnea epela", "Limoiarekin").

### 5.3 Biltegiko Stock Txartelak
* **Stock Normala:** Zenbaki berdea edo arbel grisa.
* **Gutxieneko Stocka Gainditua:** Alerta koadro gorria (`#FEE2E2` hondoa, `#DC2626` testua eta ertza).

