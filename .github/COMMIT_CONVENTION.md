# 1T Jatetxea - Komit Hitzarmena eta Git Arauak

Agiri honek **1T Jatetxea Kudeaketa Sistema Integralerako** git komit-mezuak idazteko arau zorrotzak finkatzen ditu. Komit-historia argia, profesionalean oinarritua eta automatikoki analizagarria izatea da helburua.

---

## 1. Oinarrizko Arau Gidariak

1. **Conventional Commits 1.0.0 Estandarra:**

   Komit-mezu bakoitzak formatu estandarra jarraitu behar du:

   ```text
   <mota>(<esparrua>): <deskribapen laburra aginte-moduan>

   [aukerazko gorputza xehetasunekin]

   [aukerazko oina: apurtze-aldaketak edo gai-zenbakiak]
   ```

2. **Zero Emoji Arau Zorrotza:**
   - Erabat debekatuta dago emojirik erabiltzea komit-mezuetan, adar-izenetan edo PR tituluetan.
   - Tonuak teknikoa, zehatza eta profesionala izan behar du beti.

3. **Urrats Bakar Bakoitzeko Komit Atomiko Txikia (Derrigorrezkoa):**
   - `gemini/IA_garapena.pdf` metodologiari jarraituz, aldaketak **beti komit txikietan banatu behar dira**, pauso txiki eta zehatz bakoitza bereiziz (adibidez: aktiboak/irudiak gehitzea, logika sortzea, UI integrazioa, dokumentazioa, konfigurazioa).
   - Ez metatu hainbat aplikazio, modulu edo pauso ezberdinetako aldaketak komit bakar batean.
   - Pantaila, osagai edo konponketa txiki bakoitza egiaztatu bezain laster bere komit atomikoa sortuko da.

---

## 2. Onartutako Motak (Types)

| Mota | Esanahia | Adibidea |
| :--- | :--- | :--- |
| `feat` | Funtzionalitate berri bat erabiltzailearentzat edo sistemarentzat | `feat(api): gehitu lehengaien stock alerta endpoint-a` |
| `fix` | Aurreko errore edo akats baten zuzenketa | `fix(admin): zuzendu OPNsense birbideratze esteka 8443 atakara` |
| `refactor` | Egitura hobekuntza, kanpoko portaera aldatu gabe | `refactor(db): optimizatu transakzioetako SELECT FOR UPDATE blokeoak` |
| `style` | Formatu-aldaketak, indentazioa, espazioak, semantika aldatu gabe | `style(figma): lerrokatu botoien auto-layout padding-ak 12px-ra` |
| `docs` | Dokumentazioa soilik aldatzea | `docs(readme): eguneratu sare-profilen gida teknikoa` |
| `test` | Proba automatizatuak gehitu edo zuzentzea | `test(api): gehitu erreserba bikoitzen aurkako konkurrentzia-probak` |
| `ci` | CI/CD lan-fluxuen aldaketak (GitHub Actions, gidoiak) | `ci(actions): gehitu FXML sintaxi egiaztatzailea lan-fluxura` |
| `chore` | Mantentze-lan orokorrak, bertsio-igoerak, mendekotasunak | `chore(deps): eguneratu MySqlConnector bertsio egonkorrera` |
| `perf` | Errendimendua nabarmen hobetzen duen kode-aldaketa | `perf(kds): murriztu sukaldeko polling latentzia 500ms-ra` |

---

## 3. Onartutako Esparruak (Scopes)

Esparruak komitak eragiten dion azpisistema edo osagaia adierazten du:

- `api`: ASP.NET Core REST API (5050 ataka, kontroladoreak, DTOak).
- `admin`: Admin IT Zentrala panela eta kudeaketa zerbitzaria (5000 ataka).
- `desktop-csharp`: C# Windows Forms TPV mahaigaineko bezeroa.
- `kds-javafx`: JavaFX Sukaldea KDS mahaigaineko panela (FXML, CSS, Controller).
- `web-php`: Web PHP Frontend publikoa (8085 ataka).
- `mobile-client`: Flutter bezeroen mugikor-aplikazioa.
- `mobile-waiter`: Flutter zerbitzarien mugikor-aplikazioa.
- `android-komandera`: Android Jetpack Compose zerbitzarien komandera aplikazioa.
- `tv`: Android TV Sukaldea KDS aplikazioa.
- `github`: `.github` arauak, txantiloiak eta automatizazioak.
- `config`: Sare-profilak eta ingurune-konfigurazioak.
- `db`: MySQL 8.0 datu-basea, eskema, migrazioak eta prozedurak.
- `docker`: Docker Compose zerbitzuak eta edukiontziak.
- `opnsense`: OPNsense suebakia, NAT arauak eta routing-a.
- `cisco`: Cisco 2960 laborategi fisikoko kommutazioa eta VLANak.
- `figma`: Figma diseinu-sistema, tokenak eta interfaze-txantiloiak.
- `security`: DevSecOps, sarbide-kontrola eta baimenak.

---

## 4. Komit Onen eta Txarren Adibideak

### 4.1 Adibide Egokiak (Zuzenak)

- `feat(api): inplementatu 5050 atakan datu-basearen egoera monitorea`
- `fix(admin): ziurtatu 5000 portuan admin panela eta 5050 portuan apia mantentzea`
- `style(kds-javafx): egokitu komanden txartelak zinc paletara eta egoera berde/gorrietara`
- `docs(figma): zehaztu geruza-izenen eta kontroladore-aldagaien arteko hitzarmena`
- `refactor(web-php): garbitu POST parametroen saneatzea sql injection aurka`

### 4.2 Adibide Desegokiak (Debekatuak)

- `aldaketak eginda` (motarik ez, esparrurik ez, azalpenik ez)
- `feat: ikono berriak gehitu` (emojirik gabekoa)
- `fix bug` (zehaztasunik ez, esparrurik ez)
- `WIP` (ez da onartzen behin-behineko komit ilunik egitea)
