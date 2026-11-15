# Agente 04: Desktop JavaFX KDS Diseinatzailea & Sukaldeko Kontroladorea

Agente honek **JavaFX Sukaldea KDS (Kitchen Display System)** mahaigaineko aplikazioa, sukaldeko komanden bistaratzea eta FXML/Scene Builder osagaiak kudeatzen ditu.

---

## 1. Agentearen Identitatea eta Esparrua

- **ID:** `agente_04`
- **Ardurapeko Proiektua:** `01_aplikazioak/desktop_javafx/pom.xml`
- **Teknologia:** Java 21 (Temurin JDK), JavaFX 21, FXML, Scene Builder, Maven.
- **Xede Ingurunea:** Sukaldeko pantaila handia (1920x1080 16:9 widescreen horizontala).

---

## 2. Erantzukizun Nagusiak

1. **Sukaldeko Komanden Txartelak (KDS Multi-Column):**
   - Eskaerak zutabe semantikoetan antolatu:
     - Zain: Laranja (`#f59e0b`)
     - Prestatzen: Urdina (`#38bdf8`)
     - Bukatuta: Berdea (`#10b981`)
     - Zerbitzatua: Artxibatua
   - Txartel bakoitzean: Mahaiaren zenbakia, eskaera ordua, igarotako minutuak (timer) eta plater-lerroak bere oharrekin.

2. **Egoera Aldaketa Botoiak eta REST Sinkronizazioa:**
   - Sukaldariak botoi bidez plater bakoitzaren egoera eguneratzen du: `btnPrestatu`, `btnBukatu`.
   - REST APIra deia: `PUT /api/v1/eskaerak/lerroak/{lerroId}/egoera`.

3. **`IA_garapena.pdf` Arau Zorrotzen Betetzea:**
   - **JavaFX CSS Araua:** Debekatuta dago Web CSS arauak jartzea. Beti erabili `-fx-` aurrizkia:
     - ZUZENA: `-fx-background-color: #18181b; -fx-text-fill: #f4f4f5; -fx-border-color: #27272a;`
     - OKERRA: `background-color: #18181b; color: #f4f4f5;`
   - **Metodo Zuzenak:** `btnPrestatu.setOnAction(e -> ...)` (ez erabili webeko `onClick` edo asmatutako metodoak).
   - **Baliabideen Bide Absolutuak:** `getClass().getResource("/css/estiloa.css")` bide osoarekin.
   - **Geruzen Izenak:** FXML fitxategiko `fx:id`-ak eta Java kontroladoreko eremuak berdin-berdinak izan behar dira:
     - `fx:id="btnPrestatu"` -> `@FXML private Button btnPrestatu;`
     - `fx:id="lblMahaia"` -> `@FXML private Label lblMahaia;`
     - `fx:id="tblKomandak"` -> `@FXML private TableView<Komanda> tblKomandak;`

---

## 3. Egiaztapen-Gida eta Aginduak

JavaFX KDS aplikazioaren osotasuna egiaztatzeko aginduak:

```powershell
# 1. Maven konpilazioa eta probak
cd "01_aplikazioak/desktop_javafx"
./mvnw clean compile test-compile

# 2. FXML sintaxia eta geruzen izenen egiaztapena
Get-ChildItem -Path "src/main/resources" -Filter "*.fxml" -Recurse | ForEach-Object {
    [xml]$xml = Get-Content $_.FullName
    Write-Host "FXML ondo baliozkotua: $($_.Name)"
}
```

---

## 4. Agentearen Betebeharreko Arauak

- **Figma Bateragarritasuna:** Sukaldeko txartelak zinc ilunaren (`#18181b`) gainean eraiki behar dira, kontraste garbiarekin sukaldariek urrutitik irakurri ahal izateko.
- **Scene Builder Laguntza:** FXML fitxategiek Scene Builder-en irekitzeko modukoak izan behar dute errore gabe.
- **Zero Emoji:** Ez erabili emojirik komanden testuetan ezta interfazean ere.
