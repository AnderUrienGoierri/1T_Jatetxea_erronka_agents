# Metodologia: IA Bidezko Software Garapena eta Diseinua

Dokumentu honek 1T Jatetxea erronkan adimen artifizialeko laguntzaileak (Gemini, Claude, GPT, Antigravity) erabiliz kalitate handiko softwarea eta interfazeak sortzeko lan-fluxua finkatzen du.

---

## 1. Oinarrizko Printzipioak

Adimen Artifizial sortzailea ez da programatzailearen ordezkoa; programatzailearen biderkatzailea da. Emaitza fidagarriak, seguruak eta garbiak lortzeko arau hauek jarraitu behar dira:

1. **Testuinguru Aberatsa (Context is King):** IAk ez daki zer nahi duzun esplizituki azaltzen ez badiozu. Beti eman fitxategi-egitura, teknologiaren bertsio zehatzak eta xede-arkitektura.
2. **Urrats Txiki eta Iteratiboak:** Ez eskatu «egin jatetxearen aplikazio osoa». Eskatu lehenik datu-eredua, gero REST APIko endpoint zehatz bat, ondoren UI txantiloia eta azkenik kontroladorearen logika.
3. **Egiaztapen Mekanikoa:** IAk sortutako kode lerro bakoitza berehala eraiki eta exekutatu behar da. Erroreak sortu ahala zuzendu behar dira, konpilazio-katean metatu gabe.
4. **Zero-Trust eta Segurtasuna:** Inoiz ez onartu pasahitzak kodean («hardcoded»), baimenik gabeko SQL kontsultak edo enkriptatu gabeko pasahitzak.

---

## 2. 5 Urratseko Garapen Metodologia

```text
[1. Zirriborroa]
        │  (Paperean / Arbelan, pantaila-egitura orokorra)
        ▼
[2. Wireframe-a]
        │  (IA-rekin sortutako SVG / HTML prototipo azkarra)
        ▼
[3. Diseinua Figman]
        │  (Diseinu-sistema, kolore-paleta, ikonoak, Auto-Layout)
        ▼
[4. Kodea IA-rekin]
        │  (Txantiloia + Klase Diagrama + Estilo Fitxategia)
        ▼
[5. Probatu eta Zuzendu]
           (Exekuzioa, stack trace osoa, alboz alboko konparaketa)
```

### 1. Urratsa: Eskuzko Zirriborroa

- Orri bakoitzean pantaila bakar bat marraztu argi onarekin eta errotulagailu ilunez.
- Pantailak zenbatu eta geziekin adierazi botoi bakoitzak nora eramaten duen.
- Marrazkian bertan oharrak idatzi: «Goitibeherako zerrenda», «Taula scroll-arekin», «Botoi honek X irekitzen du». Idatzita ez dagoena IAk bere kabuz interpretatuko du.

### 2. Urratsa: Wireframe-a IA-rekin

- Zirriborrotik abiatuta oinarrizko wireframe digitala sortu (SVG edo egitura garbia).
- Ez bilatu azken diseinu grafikoa urrats honetan; helburua osagaien antolaketa espaziala finkatzea da.

### 3. Urratsa: Diseinua Figman

- Wireframe-a Figmara eraman (SVG bidez edo eskuz).
- Diseinu-sistema aplikatu: koloreak (hex formatuan), letra-tipoak, neurriak, tarteak eta auto-layout.
- **Arau Kritikoa - Geruzei kodeko izenak eman:**
  - Figmako osagai eta geruzen izenek bat etorri behar dute kodean erabiliko diren aldagai-izenekin.
  - Adibideak:
    - Botoia: `btnBilatu`, `btnGorde`, `btnEzeztatu`
    - Testu-eremua: `txtErabiltzailea`, `txtHelmuga`, `txtPrezioa`
    - Taula: `tblMahaiak`, `tblEskaerak`, `tblLehengaiak`
    - Etiketa: `lblEgoera`, `lblGuztira`

### 4. Urratsa: Kodea IA-rekin Sortzea

Laugarren urrats honetan, IAk kodea sortu aurretik lau sarrera jaso behar ditu:

1. **Figmako diseinua / frame-aren kaptura** (PNG 2x bereizmenarekin).
2. **Txantiloia eta erreferentziako proiektua:** pom.xml / csproj, paketeen egitura, FXML / XAML / HTML adibide oso bat.
3. **Klase-diagrama testuan:** Mermaid edo PlantUML formatuan, klase-izenak, atributuak, metodoak eta erlazioak zehaztuta.
4. **Estilo-dokumentua:** kolore hex balioak, marjinak eta letra-tipoak.

### 5. Urratsa: Probatu eta Zuzendu

- Sortutako kodea berehala konpilatu eta exekutatu.
- Zerbaitek huts egiten badu, **stack trace osoa** eman IAri, inoiz ez bakarrik «ez dabil».
- Egin konparaketa bisuala: aplikazioaren pantaila-argazkia eta Figmako frame-a alboz albo jarri desbideratzeak zuzentzeko.
- Emaitza onartzean, egin commit atomikoa.

---

## 3. Ohiko Akatsen Prebentzio-Zerrenda

| Akats Mota | Arriskua / Kausa | Konponbide Estandarra |
| :--- | :--- | :--- |
| **Izen desadostasunak** | FXML/UI izenak eta kontroladorea ez datoz bat (`fx:id="btnBilatu"` vs `@FXML Button btnBuscar`). | Figmako geruzen izen bera erabili FXML eta kontroladoreko aldagaietan. |
| **Baliabideen bide okerrak** | CSS, ikono edo irudien bide erlatiboak okerrak izatea (`getResource("estiloa.css")`). | Bide absolutuak erabili klase-ibilbidean: `getResource("/css/estiloa.css")` edo `file:///` estekak. |
| **JavaFX CSS vs Web CSS** | Web estiloko arauak JavaFX-en jartzea (`background-color` JavaFX-en). | JavaFX fitxategietan beti erabili `-fx-` aurrizkia (`-fx-background-color`, `-fx-text-fill`). |
| **Existitzen ez diren metodoak** | IAk metodo asmatuak sortzea (`btn.setOnClick()` JavaFX-en). | JavaFX-en `btn.setOnAction()`, C#-n `btn.Click +=`, Web-en `addEventListener('click')`. |
| **Moduluen baimen faltak** | `module-info.java` fitxategian FXML-ari sarbidea ez ematea. | `opens eus.goierri.jatetxea.controller to javafx.fxml;` adierazpena sartu. |
| **Scene Builder bateragarritasuna** | Klase pertsonalizatuak Scene Builder-ek ez ezagutzea. | Proiektuko JAR fitxategia sortu (`mvn package`) eta Scene Builder-eko Library panelsean inportatu. |

---

## 4. IA Eskatzean Eman Bearreko Testuinguru Txantiloia

IAri funtzionalitate edo pantaila berri bat eskatzean, egitura hau erabili behar da:

```markdown
### 1. Testuingurua eta Helburua

- Aplikazioa: [adib. JavaFX Sukaldea KDS / C# TPV / Web PHP]
- Pantaila: [adib. Sukaldeko aginduen panela]
- Erabiltzailea: [adib. Sukaldaria]

### 2. Egitura eta Klase Diagrama

```text
classDiagram
    class EskaeraKontroladorea {
        -TableView tblAginduak
        -Button btnPrestatua
        +eguneratuEgoera() void
    }
```

### 3. Figmako Elementuak eta Izendapenak

- Taula: tblAginduak
- Botoiak: btnPrestatua, btnEzeztatu
- Etiketak: lblMahaiaZenbakia, lblKronometroa

### 4. Estilo Arauak

- Letra-tipoa: Inter (Sans) eta JetBrains Mono (Monospace)
- Mihisea: #09090b (Zinc-950)
- Gainazala: #18181b (Zinc-900)
- Egoera koloreak: Berdea (#10b981), Laranja (#f59e0b), Gorria (#ef4444)

### 5. Eskakizun Espezifikoa

- Sortu soilik [Fitxategi-izena], ez aldatu beste modulurik.
- Eman kode osoa, komentatuta eta azalpen labur batekin.
```
