# 1T Zerbitzaria Komandera — Demo Bideoaren Gidoia eta Grabazio-Gida

> **Helburua:** Ebaluazio-errubrikako **Demoa eta galderak (%10)** atalean **Bikaina (10/10)** lortzea.
> **Egilea:** Ander Urien Telleria _(banakako entrega)_
> **Iraupen ofiziala (PDF):** **3 – 4 minutu** (helburu ideala: **3 minutu eta 30 segundo**).
> **Zer erakutsi behar da:** Aplikazioa **zuzenean exekutatzen** + **kodearen egitura** azalduta.

---

## 1. Grabatu Aurreko Prestaketa (10 minutu lehenago)

Grabazioari `REC` eman aurretik, egiaztatu zerrenda hau lehen hartualdian garbi atera dadin:

- [ ] **Backend-a eta DB martxan:** Abiarazi `backend_rest_api_csharp` (`5050` atakan) eta egiaztatu `/api/v1/health` berdea dagoela (`API eta DB konektatuta`).
- [ ] **Datu garbiak probetarako:** Ziurtatu gutxienez **mahai libre bat** (adib. Mahaia 1) dagoela eta plater batzuek stocka dutela, ziklo osoa etenik gabe egiteko.
- [ ] **Konexioa prest (Gailu erreala edo Emuladorea):**
  - _Emuladorea (AVD):_ Ezarpenetan hautatu `Android Emuladorea` profila (`http://10.0.2.2:5050/api/v1/`).
  - _Mugikor erreala USB bidez:_ Exekutatu `adb reverse tcp:5050 tcp:5050` eta hautatu `USB Kablez / Localhost` (`http://127.0.0.1:5050/api/v1/`), **edo** Wi-Fi sare berean zaudetela erabili zure Wi-Fi profila.
- [ ] **Android Studio garbi eta irakurgarri:**
  - Ireki aldez aurretik pestainetan erakutsiko dituzun 5 fitxategi hauek soilik (ordenan):
    1. `MainActivity.kt`
    2. `ui/navigation/Screen.kt`
    3. `ui/screens/MahaiakScreen.kt` (eta `@Preview` panela prest Split moduan!)
    4. `ui/screens/EskaeraScreen.kt`
    5. `data/api/JatetxeaApiService.kt` + `data/network/NetworkProfiles.kt`
  - Handitu Android Studio-ko letra-tamaina (`Settings > Editor > Font > Size: 16` edo `18`) bideoan kodea argi irakurtzeko.
- [ ] **Pantaila garbia:** Itxi jakinarazpenak (Windows-en **Ez molestatu / Focus Assist** aktibatu) eta jarri ezkerrean Android Studio eta eskuinean emuladorea / mugikorraren ispilua (**Device Mirroring**).
- [ ] **Audio proba (10 seg):** Grabatu proba labur bat eta entzun ahotsa argi, bolumen egokian eta atzeko zaratarik gabe entzuten dela.

---

## 2. Minutuz Minutuko Gidoia (3 min 35 seg)

> **Oharra:** Banakako entrega denez, jarraitu gidoi osoa erritmo lasai, argi eta naturalean (~3 min 30 seg).

### 🎬 1. BLOKEA — Sarrera eta Ideia (`0:00 – 0:25`)

- **Pantailan:** `LoginScreen` irekita emuladorean/mugikorrean (**Logo B** berria, _Warm Hospitality_ modu argia, 3 blokeak `Jantokia / Barra / Admin` eta `API Konektatuta` puntu berdearekin).
- **Ekintza:** Ezer klikatu gabe aurkeztu.
- **Esateko gidoia:**
  > _"Kaixo! Ander Urien naiz, eta **1T Jatetxea — Zerbitzari Komandera** Android aplikazioa aurkeztuko dut, osorik **Kotlin eta Jetpack Compose (Material 3)** erabiliz garatua, XML diseinurik gabe._
  > _Aplikazio hau jatetxeko zerbitzariek eta barrako langileek mahaiak kudeatu, sukaldera komandak bidali, erreserbak ikusi eta bezeroei mahaian bertan kobratzeko komandera mugikorra da, Figman diseinatutako UI/UX sistema propioarekin."_

---

### 🎬 2. BLOKEA — Live Demo 1: Login (Maite `1002`), Sare-Konexioa eta Mahaien Mapa (`0:25 – 1:05`)

- **Pantailan:** Aplikazioa zuzenean.
- **Ekintzak pausoz pauso:**
  1. Seinalatu beheko barrean **Wi-Fi detekzioa** eta **API/DB osasun-adierazle berdea** (`http://10.0.2.2:5050/api/v1/` automatikoki hautatua emuladorean).
  2. Ireki **Ezarpenak** (gurpil ikonoa) → erakutsi sare-profilak, **Modu Iluna / Argia** switch-a eta **Hizkuntza (`EU`/`ES`/`EN`)** (`SharedPreferences`-en gordeta). Itxi dialogoa.
  3. Aukeratu **Maite (`Zerbitzaria`)** langileen `LazyColumn`-etik eta sartu **PIN `1002`** teklatu animatuan.
  4. `MahaiakScreen`-en: erakutsi estatistika-barra, klikatu eremu-iragazki bat (`Jantoki Nagusia`, `Barra`, `Guztiak`) eta ireki **Erreserbak** (egutegi ikonoa). Itxi dialogoa.
- **Esateko gidoia:**
  > _"Lehen pantailan (`LoginScreen`), aplikazioak automatikoki detektatzen du emuladorea edo Wi-Fia eta gure C# REST APIaren `/health` egoera egiaztatzen du. Ezarpenetan sare-profilak, gai iluna/argia eta hizkuntza gordetzen dira `SharedPreferences`-en._
  > _Lehenik **Maite zerbitzariarekin** sartuko gara (`1002` PINarekin). Bigarren pantailan (`MahaiakScreen`), mahai guztiak `LazyVerticalGrid` moldagarri batean agertzen dira, 5 segundoro auto-freskatzen dira, eremuka iragaz ditzakegu eta eguneko erreserbak kontsultatu."_

---

### 🎬 3. BLOKEA — Live Demo 2: Maiteren Eskaera + Gorka Barrakoa (`2001`) + Ordainketa (`1:05 – 2:15`)

- **Pantailan:** Aplikazioa zuzenean.
- **Ekintzak pausoz pauso:**
  1. **Maiterekin:** Klikatu **Mahaia 1** → **"Sortu Eskaera Berria"** (`2` pax) → **"Gehitu Platerak Katalogoatik"**.
  2. Erakutsi Maitek **karta osoa** ikusten duela (`Sarrerakoa`, `Lehena`, `Bigarrena`, `Postrea` + edariak) eta probatu segundo 3z **bilatzailea** (testuz edo mikrofono 🎤 ikonoarekin ahots bidez!). Gehitu 2 plater (`Gildak` + `Idiazabal Gazta`, ikusi stocka eta beheko saski barra animatua) → **"Gehitu Eskaerara"** → sakatu **"Bidali Sukaldera"**.
  3. Itzuli atzera eta **Saioa Itxi** (goiko ezkerreko ikonoa) → orain sartu **Gorka (`Barrakoa`)** erabiltzailearekin (**PIN `2001`**)!
  4. Klikatu **Mahaia 1** → seinalatu beheko botoiak **"Edariak"** dioela → sakatu **"Edariak"**.
  5. **Momentu izarra (`Barra Katalogoa`):** Erakutsi **`BARRA MODUA · Edariak, kafeak, infusioak eta koktailak soilik`** bannerra eta 10 kategoria zehatzak (`Urak`, `Freskagarriak`, `Garagardoak`, `Sagardoak`, `Ardoak`, `Kafeak`, `Infusioak`, `Likoreak`, `Konbinatuak`, `Koktailak`) — **platerik ez da agertzen!** Gehitu `Aperol Spritz` edo `Cappuccino` bat → **"Gehitu Eskaerara"**.
  6. Sakatu **"Kontua"** (80mm proforma) → **"Kobratu Orain"** → `Eskudirua` fitxan sakatu `50€` (kanbio berdea animatua) → **"KOBRATU ETA TIKETA SORTU"** → erakutsi **Tiket Fiskala** (`.txt` / Inprimatu) eta itxi (`Garbitzen` egoera).
- **Esateko gidoia:**
  > _"Maiterekin 1. mahaian eskaera ireki eta bi sarrerako gehitzen ditugu karta osotik, stocka denbora errealean egiaztatuta, eta **Sukaldera bidaltzen** ditugu._
  > _Orain saioa itxi eta **Gorka barrako zerbitzariarekin** sartuko gara (`2001` PINa). Mahaia berean **Edariak** sakatzean, aplikazioak automatikoki **Barra Modua** aktibatzen du: Gorkak ez ditu sukaldeko platerak ikusten, baizik eta datu-basean zabaldutako **10 kategoria zehatzak** (Urak, Freskagarriak, Garagardoak, Sagardoak, Ardoak, Kafeak, Infusioak, Likoreak, Konbinatuak eta Koktailak). Edari bat gehitu, proforma eta eskudiruko ordainketa kanbioarekin burutu, eta **80mm-ko tiket fiskala** sortzen da."_

---

### 🎬 4. BLOKEA — Kodearen Egitura eta Jetpack Compose (`2:10 – 3:05`)

- **Pantailan:** Android Studio (Project zuhaitza + irekita dauden 5 fitxategiak).
- **Ekintzak pausoz pauso:**
  1. Erakutsi ezkerreko **Project zuhaitza** (`data/api`, `data/model`, `data/network`, `ui/components`, `ui/navigation`, `ui/screens`, `ui/state`, `ui/theme`).
  2. Klikatu `MainActivity.kt` eta `Screen.kt`: erakutsi `NavHost`, 4 rutak (`mahaiId` / `eskaeraId` parametroekin) eta trantsizioak.
  3. Klikatu `MahaiakScreen.kt` (edo `EskaeraScreen.kt`): erakutsi `remember { mutableStateOf(...) }`, `LazyVerticalGrid`/`LazyColumn`, Composable txiki bat (`MahaiaTxartela` / `PlateraItem`), animazio bat eta **`@Preview` panela** eskuinean errendatuta!
  4. Klikatu `JatetxeaApiService.kt` eta `NetworkProfiles.kt`: erakutsi Retrofit interfazea (`@GET`, `@POST`, `@PUT` `suspend` funtzioekin), `SharedPreferences` bidezko URL gordetzea eta Wi-Fi profilak.
- **Esateko gidoia:**
  > _"Orain kodearen egitura erakutsiko dugu. Proiektua geruza argitan banatu dugu: `data` paketean APIa, DTO modeloak eta sare-profilak daude; `ui` paketean, berriz, `theme`, `navigation`, `state`, `components` eta `screens`. Horrela `MainActivity.kt` oso garbi geratzen da eta `NavHost` nabigazioa soilik kudeatzen du, `Screen` sealed class-arekin eta pantaila arteko trantsizioekin._
  > _Pantaila bakoitzean egoera `remember` eta `mutableStateOf` bidez kudeatzen dugu, zerrendak `LazyColumn` eta `LazyVerticalGrid`-ekin optimizatuta daude, osagai berrerabilgarriak Composable txikietan banatu ditugu, eta pantaila guztiek beren **`@Preview`** funtzioa dute Android Studio-n zuzenean ikusteko._
  > _Aukerazko erronka gisa **REST API komunikazioa** inplementatu dugu **Retrofit eta Coroutine**-ekin (`JatetxeaApiService.kt`, 17 endpoint), eta gainera `SharedPreferences` erabiltzen dugu sare-konfigurazioa eta lehentasunak gordetzeko."_

---

### 🎬 5. BLOKEA — Diseinu-Erabakiak, IAren Erabilera eta Itxiera (`3:05 – 3:35`)

- **Pantailan:** `README.md` aurrebista (v2.0 pantaila-argazkiekin, Figma estekekin eta IA atalarekin).
- **Esateko gidoia:**
  > _"Diseinu-erabaki nagusiak izan dira: Figman diseinatutako **Warm Hospitality** paleta (Modu Argia lehenetsita + Iluna aukeran) eta 3 letra-tipo (`Cinzel`, `Plus Jakarta Sans`, `JetBrains Mono`), `NavHost` parametro tipatuekin, rolaren araberako Barra Modua eta `mDNS` bidezko konexio automatikoa._
  > _Garapenean IA laguntzaile gisa erabili dut pausoz pauso, lerro bakoitza berrikusiz eta gailu errealean zein emuladorean probatuz. Informazio osoa `README.md`-n dago._
  > _Eskerrik asko ikusteagatik, eta edozein galderari erantzuteko prest nago!"_

---

## 3. Irakaslearen Galdera Posibleak (Prestaketa Azkarra)

PDFak ohartarazten du: _"nik edozein lerro galdetu dezaket"_. Hona hemen galdera ohikoenak eta erantzun zehatzak:

| Galdera Posiblea                                                                                               | Erantzun Argia eta Laburra                                                                                                                                                                                                                                                                             |
| :------------------------------------------------------------------------------------------------------------- | :----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **1. Zergatik erabili duzue `remember { mutableStateOf(...) }` eta ez `var` arrunt bat?**                      | Compose-k berkonposaketa (_recomposition_) egiten duenean aldagai arruntak berrezarri egiten dira eta UIak ez du aldaketa antzematen. `mutableStateOf`-ek Compose-ri abisatzen dio balioa aldatzean pantaila eguneratzeko, eta `remember`-ek balioa mantentzen du berkonposaketen artean.              |
| **2. Zertarako da `LaunchedEffect` (`MahaiakScreen` / `LoginScreen`)?**                                        | Composable-a pantailan sartzean _coroutine_ bat modu seguruan abiarazteko (adibidez APIari deitzeko edo 5 segundoro mahaiak freskatzeko). Pantailatik irtetean, Compose-k automatikoki bertan behera uzten du (`while(true)` ez da atzeko planoan geratzen).                                           |
| **3. Zer alde dago `LazyColumn` eta `Column` + `verticalScroll` artean?**                                      | `LazyColumn`-ek pantailan ikusten diren elementuak soilik sortu eta errendatzen ditu (errendimendu hobea zerrenda luzeetan, `RecyclerView` bezala). `Column`-ek guztiak batera kargatzen ditu memorian.                                                                                                |
| **4. Nola pasatzen dituzue datuak pantaila batetik bestera `NavHost`-en?**                                     | `Screen.kt`-n ibilbide parametrizatuak definituta (`"mahaia_xehetasunak/{mahaiId}"`), `navArgument("mahaiId") { type = NavType.IntType }` bidez jasotzen ditugu `backStackEntry.arguments`-etik, eta saio-egoera (`unekoZerbitzaria`) `KomanderaApp`-en gora igota (_state hoisting_) mantentzen dugu. |
| **5. Nola funtzionatzen du REST API deiak UIa blokeatu gabe?**                                                 | `JatetxeaApiService`-ko funtzioak `suspend` dira eta `rememberCoroutineScope().launch` barruan deitzen dira. Retrofit-ek sareko eskaera atzeko hariekin (_background thread_) egiten du eta erantzuna iristean Compose-ren egoera eguneratzen du.                                                      |
| **6. Nola funtzionatzen du `SharedPreferences`-ek `ApiClient`-en?**                                            | `ApiClient.init(context)` deitzean `komandera_prefs` fitxategitik gordetako `api_base_url` irakurtzen du; lehen aldia bada, `NetworkProfiles`-ek Wi-Fiaren arabera gomendatutako URL-a ezartzen du eta gordetzen du app-a itxita ere mantentzeko.                                                      |
| **7. Zergatik erabili duzue `saskia = saskia.map { it.copy(...) }` zuzenean `item.kopurua++` egin beharrean?** | Compose-ren `mutableStateOf<List<...>>`-ek zerrendaren erreferentzia berria ikusi behar du aldaketa antzemateko. Elementu baten barneko `var` bat aldatze hutsak ez du beti berkonposaketa abiarazten; lista/kopia berria esleituz (`copy`), UIa berehala eta fidagarritasunez eguneratzen da.         |
| **8. Nola inprimatzen dira tiketak Android-etik?**                                                             | `OrdainketaScreen.kt`-ko `inprimatuHTML()` funtzioak 80mm-ko HTML tiket bat sortzen du, `WebView` batean kargatu eta Android-en `PrintManager` zerbitzu ofiziala deitzen du (PDF gorde edo inprimagailura bidaltzeko).                                                                                 |

---

## 4. Nola Grabatu Bideoa Gaur Arratsaldean (Windows)

Zure ordenagailua egiaztatu dugu eta **OBS Studio jada instalatuta daukazu!** (`C:\Program Files\obs-studio\bin\64bit\obs64.exe`). Hona hemen 3 aukerarik onenak:

### 🏆 1. Aukera (GOMENDATUA): OBS Studio + Android Studio

Kalitate onena, irudia + mikrofonoa oso garbi, eta nahi duzun moduan pantaila antolatuta.

1. **Prestatu pantaila:**
   - Ireki **Android Studio** pantaila osoan.
   - Eskuineko panelean ireki **Running Devices** (emuladorea edo USB bidez konektatutako mugikor erreala — _Android Studio-k automatikoki ispilatzen du mugikor erreala USB Debugging aktibatuta baduzu!_). Horrela ezkerrean kodea eta eskuinean aplikazioa zuzenean ikusiko dira.
2. **Konfiguratu OBS Studio (2 minutu):**
   - Ireki **OBS Studio**.
   - **Sources (Iturriak)** koadroan sakatu **`+`** → gehitu:
     - **Display Capture (Pantaila-harrapaketa)** → hautatu zure monitorea.
     - **Audio Input Capture (Audio sarrera)** → hautatu zure mikrofonoa (egiaztatu barra berdea mugitzen dela hitz egitean, gorrian sartu gabe!).
   - **Settings (Ezarpenak) > Output (Irteera) > Recording:**
     - _Recording Format:_ **`mp4`** (edo `mkv` eta gero `File > Remux Recordings` eginez MP4ra pasa, seguruagoa da ordenagailua trabatuz gero).
     - _Recording Path:_ Aukeratu `Bideoak` edo proiektuko karpeta bat.
   - **Settings > Video:** `1920x1080`, `30 FPS` (nahikoa eta arina Teams-erako).
   - **Settings > Hotkeys:** Jarri lasterbide bat (adib. `F9` = Start Recording, `F10` = Stop Recording) OBS leihoa bideoan ez agertzeko!
3. **Grabatu:**
   - Minimiza OBS, joan Android Studiora, sakatu **`F9`**, arnasa hartu 1 segundo eta hasi gidoiarekin.
   - Amaitzean itxaron 1 segundo eta sakatu **`F10`**.
4. **Berrikusi:** Ireki sortutako `.mp4` fitxategia, egiaztatu **3:00 – 4:00 minutu** artean dagoela eta audioa argi entzuten dela.

---

### ⚡ 2. Aukera (Oso azkarra, instalaziorik gabe): Windows Snipping Tool edo Clipchamp

OBS konfiguratzeko gogorik ez baduzu:

- **Snipping Tool (Windows 11):** Sakatu `Win + Shift + R` → hautatu pantaila osoa → aktibatu **mikrofono ikonoa** goiko barran → sakatu **Start**. Zuzenean `.mp4` gordetzen du.
- **Microsoft Clipchamp** (Windows-en barne dago): `Grabatu eta sortu > Pantaila` → aukeratu mikrofonoa → grabatu eta behar baduzu hasiera/amaiera moztu bertan, gero esportatu `1080p MP4`-n.

---

### 📱 Mugikor Erreala Pantailan Erakusteko (Aukerakoa)

Mugikor errealean grabatu nahi baduzu (eta bide batez "Gailu errealean probatu" gehitu):

1. Aktibatu mugikorrean **Garapenerako aukerak (Developer Options) > USB arazketa (USB Debugging)**.
2. Konektatu mugikorra USB kablez ordenagailura.
3. Android Studio-n ireki **View > Tool Windows > Running Devices** → zure mugikorraren pantaila zuzenean agertuko da Android Studio barruan, eta bertatik saguarekin kontrolatu ahal izango duzu!
4. Sare lokalarekin arazorik baduzu, exekutatu Android Studio-ko terminalean:

   ```powershell
   adb reverse tcp:5050 tcp:5050
   ```

   Eta aplikazioaren Ezarpenetan hautatu **`USB Kablez / Localhost`** profila.

---

### 💡 Grabazio Aholku Azkarrak

- **Egin entsegu azkar bat (grabatu gabe):** Behin gidoia ozen irakurrita eta klik guztiak probatuta, grabazio erreala askoz naturalagoa eta fluidoagoa ateratzen da.
- **Ez gelditu akats txiki batean:** Hitz bat nahasten baduzu, lasai jarraitu edo zuzendu naturalki — 3:30eko bideo jarraitu eta naturala hobea da mozketa artifizial asko dituena baino.
- **Fitxategiaren izena Teams-erako:** `1T_Komandera_Demo_[ZureIzena].mp4`.
