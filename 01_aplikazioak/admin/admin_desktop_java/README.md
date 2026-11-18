# T1 Jatetxea - Kudeaketa Desktop

Jabe edo kudeatzaileentzako JavaFX back-office aplikazioa. Langileen mahaigaineko KDS aplikaziotik aparte garatzen da: hemen langileak, platerak/errezetak, osagaiak/inbentarioa eta mahaiak kudeatzen dira.

## Teknologia

- Java 21+, JavaFX 21 eta FXML.
- Scene Builder bidez ireki daitezkeen pantaila-fitxategi bereiziak.
- AtlantaFX `Primer Light` gaia eta T1 Jatetxearen JavaFX CSS pertsonalizatua.
- Java `HttpClient` eta Gson; ez dago datu-basera zuzeneko konexiorik.
- Maven Wrapper.

## Exekutatu

JDK 21 instalatuta egon behar da. PowerShell-en:

```powershell
# Hautatu JDK 21 zure ordenagailuan instalatutako bidera egokituta.
$env:JAVA_HOME = "C:\bidea\jdk-21"
$env:Path = "$env:JAVA_HOME\bin;$env:Path"
cd "01_aplikazioak\admin\admin_desktop_java\admin_desktop_java"
.\mvnw.cmd clean test
.\mvnw.cmd javafx:run
```

APIa abiarazi beste terminal batean; honek MySQL zerbitzua ere martxan behar du:

```powershell
cd "01_aplikazioak\backend_rest_api_csharp"
.\hasi_api_kontsola.bat
```

**Portuen arau finkoa:** Admin IT Zentrala `http://localhost:5000/` helbidean dago; REST APIa, berriz, `http://localhost:5050/` helbidean. Ez erabili 5000 APIrako. Docker barruan bakarrik entzuten du APIak 5000 portuan; ostalaritik `5050:5000` mapaketa erabiltzen da. JavaFX aplikazioak APIra 5050 bidez konektatu behar du.

Lehenetsitako REST API helbidea `http://localhost:5050/api/v1` da. Saio-hasierako goiburuko adierazleak APIa eta datu-basea bereizita erakusten ditu, eta 10 segundoz behin eguneratzen da. Beste helbide bat emateko:

```powershell
$env:API_BASE_URL = "http://zerbitzaria:5050/api/v1"
.\mvnw.cmd javafx:run
```

APIko erabiltzaile eta PINarekin hasi saioa; `AdminIT`, `Admin`, `Kudeatzailea` edo `Jabea` rola behar da. Erabiltzaile arruntek ez dute kudeaketa-eremua irekitzen.

## Pantailak eta diseinua

FXML iturburuak:

- `admin_desktop_java\src\main\resources\eus\goierri\jatetxea\admin\fxml\saio-hasiera.fxml`
- `admin_desktop_java\src\main\resources\eus\goierri\jatetxea\admin\fxml\diseinu-nagusia.fxml`
- `admin_desktop_java\src\main\resources\eus\goierri\jatetxea\admin\fxml\laburpena.fxml`
- `admin_desktop_java\src\main\resources\eus\goierri\jatetxea\admin\fxml\langileak.fxml`
- `admin_desktop_java\src\main\resources\eus\goierri\jatetxea\admin\fxml\menua.fxml`
- `admin_desktop_java\src\main\resources\eus\goierri\jatetxea\admin\fxml\osagaiak.fxml`
- `admin_desktop_java\src\main\resources\eus\goierri\jatetxea\admin\fxml\mahaiak.fxml`

Figma transferentzia eta Scene Builder pausoak `admin_desktop_java\design\figma\README.md` eta `admin_desktop_java\design\scene-builder\README.md` dokumentuetan daude. FXMLak `VBox`, `HBox` eta `BorderPane` erabiliz antolatuta daude, pantaila-tamainara egokitzeko.

## API integrazioa eta mugak

REST bezeroa `admin_desktop_java\src\main\java\eus\goierri\jatetxea\admin\api\ApiBezeroa.java` da. Langileak editatu, sortu eta desgaitu daitezke; edizioan PIN berria aukerakoa da. Menuan plateraren datuak eta errezetako osagai/kantitateak editatu daitezke. Inbentarioak stock baxuko erosketen alertak erakusten ditu, osagaien metadatuak editatzen ditu eta stock-aldaketak albaran-sarrera auditatu gisa erregistratzen ditu; metadatuen edizioak ez du stocka aldatzen. Sareko deiak atzeko hari batean exekutatzen dira.

`GET /api/v1/system/service-status` endpointak mahai okupatuak eta eskaera irekiak aztertzen ditu. Menuaren eta mahaien konfigurazio orokorra blokeatzen dira zerbitzua aktibo dagoenean. Mahai baten xehetasunak bakarrik editatu daitezke mahaia libre dagoenean eta eskaera edo gaurko/etorkizuneko erreserba aktiborik ez duenean; beste mahai bateko zerbitzuak ez du aldaketa hori blokeatzen. Backend-ak baldintza hauek transakzio eta lerro-blokeo bidez egiaztatzen ditu. Osagai baten unitatea ezin da aldatu stock-historia edo errezeta bati lotuta badago. Egoera ezin bada egiaztatu, UIak idazketa desgaitzen du.

Loginak `AdminIT`, `Admin`, `Kudeatzailea` edo `Jabea` rola egiaztatzen du; langile berri bat sortzeko izena, abizena, erabiltzaile-izena, NANa, emaila, PINa eta rola behar dira. PINa APIak BCrypt bidez gordetzen du; langilea editatzean PIN eremua hutsik utz daiteke eta lehengoa bere horretan mantenduko da. Uneko REST APIak JWT tokenik itzultzen ez duenez, rolaren egiaztapena desktop bezeroaren mailan dago; APIko beste bide guztiak JWTz babestea backend-eko hurrengo segurtasun-lana da.

Arkitektura-iturria: `..\doc\Jatetxearen Kudeaketa Sistemaren Arkitektura eta Testuinguru Teknikoa.pdf`.