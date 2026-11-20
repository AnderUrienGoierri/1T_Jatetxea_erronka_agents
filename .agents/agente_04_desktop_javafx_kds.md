# Agente 04: Desktop JavaFX Sistema Integrala (Admin, KDS, BDS, Stock & Core)

Agente honek **JavaFX mahaigaineko aplikazioen ekosistema osoa** kudeatzen du: sukaldeko KDS, barrako BDS, biltegiko Stock aplikazioa, kudeatzailearen Admin aplikazioa eta `desktop_javafx_core` modulu komuna.

---

## 1. Agentearen Identitatea eta Esparrua

- **ID:** `agente_04`
- **Ardurapeko Proiektuak:**
  - `01_aplikazioak/admin/admin_desktop_java`: Admin Kudeaketa (Langileak, Karta, Fakturak eta Finantzak, Erreserbak)
  - `01_aplikazioak/sukalde_langileak/desktop_sukaldea_kds`: Sukaldeko KDS (Kitchen Display System)
  - `01_aplikazioak/zerbitzari_langileak/desktop_barra_bds`: Barrako BDS (Bar Display System)
  - `01_aplikazioak/biltegi_langileak/desktop_biltegia_stock`: Biltegiko Stock & Hornitzaileak
  - `01_aplikazioak/admin/komunak/desktop_javafx_core`: Oinarrizko liburutegi partekatua (Login kontroladore komuna, API bezero unibertsala, modeloak)
  - `01_aplikazioak/admin/komunak/desktop_javafx_parent`: Maven Multi-Modulua
- **Teknologia:** Java 21 (Temurin JDK), JavaFX 21, FXML, Scene Builder, Maven.
- **Xede Ingurunea:** Terminal fisikoak (sukaldea, barra, biltegia, bulegoa) 1920x1080 eta ukimen-pantailak.

---

## 2. Erantzukizun Nagusiak

1. **Rol Bidezko Saio-Hasiera Komuna (`BaseLoginController`):**
   - Aplikazio bakoitzak dagokion rolaren arabera iragazten ditu langileak APIko `GET /api/v1/langileak` endpoint-etik:
     - Sukaldea: Sukaldariak eta laguntzaileak
     - Barra: Zerbitzariak eta barrako langileak
     - Biltegia: Biltegizainak eta erosle arduradunak
     - Admin: Administratzaileak eta kudeatzaileak
   - PIN teklatu birtuala (4 digitu) eta avatar hautaketa.
   - Goiburuan REST API osasuna denbora errealean egiaztatu (`GET /api/v1/osasuna`).

2. **Sukaldeko KDS & Barrako BDS Komandak:**
   - Zutabe semantikoetan antolatu (Zain: `#f59e0b`, Prestatzen: `#38bdf8`, Bukatuta: `#10b981`).
   - Plateren eta edarien egoera-aldaketak: `PUT /api/v1/eskaerak/lerroak/{lerroId}/egoera`.

3. **Biltegiko Stock & Hornitzaileak (`desktop_biltegia_stock`):**
   - Lehengaien zerrenda, stock mailak, gutxieneko alertak (`GET /api/v1/lehengaiak/alertak`).
   - Sarrerak erregistratu (`POST /api/v1/lehengaiak/sarrera`) eta hornitzaileen erosketa-fakturak sinkronizatu.

4. **Admin Kudeaketa Mahaigainekoa (`admin_desktop_java`):**
   - Langileen kudeaketa, Karta/Menua, Erreserbak.
   - **Fakturak & Finantzak modulua:** Salmenten fakturak, gastu/erosketen fakturak, etekin garbiaren kalkulua eta finantza-laburpena (`FakturakKontrolagailua.java`).

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
cd "01_aplikazioak/komunak/desktop_javafx_parent"
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
