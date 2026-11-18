# Scene Builder - pantailak editatzeko gida

Pantaila bakoitza bere FXML fitxategian dago, `fx:controller` eta `fx:id` loturak gordeta. Ireki banan-banan Scene Builder-en:

| Pantaila | FXML |
|---|---|
| Saio-hasiera | `src/main/resources/eus/goierri/jatetxea/admin/fxml/saio-hasiera.fxml` |
| Aplikazioaren shell-a / nabigazioa | `src/main/resources/eus/goierri/jatetxea/admin/fxml/diseinu-nagusia.fxml` |
| Laburpena | `src/main/resources/eus/goierri/jatetxea/admin/fxml/laburpena.fxml` |
| Langileak | `src/main/resources/eus/goierri/jatetxea/admin/fxml/langileak.fxml` |
| Menua eta errezetak | `src/main/resources/eus/goierri/jatetxea/admin/fxml/menua.fxml` |
| Osagaiak eta inbentarioa | `src/main/resources/eus/goierri/jatetxea/admin/fxml/osagaiak.fxml` |
| Mahaiak | `src/main/resources/eus/goierri/jatetxea/admin/fxml/mahaiak.fxml` |

## Lan-fluxua

1. Ireki FXML fitxategia Scene Builder-en, ez `target/classes` barruko kopia.
2. Erabili `VBox` eta `HBox` edukiontziak taldekatze eta tarte erantzunkorrerako; `BorderPane` aplikazioaren shell-ean dago.
3. Mantendu `fx:id`-ak kontroleko eremuarekin bat. `onAction` metodoek kontroladoreko metodo publiko/`@FXML` bidezko ekintzak erabiltzen dituzte.
4. Mantendu estilo-klaseak eta gehitu diseinu-arauak `src/main/resources/eus/goierri/jatetxea/admin/css/app.css` fitxategian; erabili JavaFX CSS propietateak (`-fx-` aurrizkia).
5. Gorde FXMLa iturburu-bidean eta egiaztatu `mvnw.cmd clean test` bidez.

Pantaila-edukiak FXML independenteak dira; aplikazioan `diseinu-nagusia.fxml`-eko edukiontzian txertatzen dira. Pantaila bakoitzak `app.css` estilo-orria erlatiboki kargatzen du, Scene Builder-en aurrebistak aplikazioaren itxura bera izan dezan. Kontroladoreek parameterik gabeko konstruktorea dute editorearen aurrebistarako; aplikazioan, kontroladoreen menpekotasunak `KontrolagailuFabrika` bidez jasotzen dituzte. Scene Builder-ek ez du APIa exekutatzen, eta datu-kargak aplikazioa APIarekin abiarazten denean bakarrik ikusiko dira.
