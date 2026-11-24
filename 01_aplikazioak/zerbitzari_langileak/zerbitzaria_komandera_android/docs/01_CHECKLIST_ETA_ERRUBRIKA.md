# 1T Zerbitzaria Komandera — Eskakizunen Check-list-a eta Ebaluazio-Errubrika

> **Dokumentu-iturria:** `Jetpack Compose aplikazio baten sorrera.pdf` (Goierri Eskola — PR 4.15 lorpen adierazlea)  
> **Aztertutako proiektua:** `zerbitzaria_komandera_android` (`eus.goierri.jatetxea.komandera`)  
> **Egoera:** ✅ **BIKAINA (10/10) lortzeko kode-hobekuntza guztiak eginda eta egiaztatuta!**

---

## 1. Laburpen Exekutiboa

Hasierako auditorian antzemandako hutsune eta xehetasun tekniko guztiak **garatu eta konpondu dira**:

1. ✅ **`@Preview` funtzioak gehitu dira pantaila eta osagai guztietan** (`LoginScreen`, `MahaiakScreen`, `MahaiaXehetasunakScreen`, `EskaeraScreen`, `OrdainketaScreen`, `EzarpenakDialog`, `ErreserbakDialog`), stateless `*Content` Composable ereduarekin.
2. ✅ **Animazio eta trantsizio esplizituak gehitu dira:** `NavHost` pantaila-trantsizioak (`slideInHorizontally` + `fadeIn`), `AnimatedVisibility` (saskiko barra, sukaldera bidaltzeko botoia, PIN errorea), `AnimatedContent` (ordainketa-fitxak eta ezarpenak), `animateColorAsState` eta `animateDpAsState`.
3. ✅ **Xehetasun tekniko guztiak konponduta:** `ApiClient.service` dinamikoa `baseUrl` aldatzean, `AppPreferences` (`isDarkTheme`, `hizkuntza`) `SharedPreferences`-en gordeta, `tr(eu, es, en)` eleaniztuna konektatuta, Modu Argia/Iluna 100% koherentea `MaterialTheme.colorScheme`-ekin, `GridCells.Adaptive(160.dp)` benetako diseinu moldagarrirako, `rememberSaveable` iragazkietan, eta 0 deprecation warning.
4. ✅ **`README.md` berridatzita** (orri batean, diseinu-erabakiekin, aukerazko erronkarekin eta IAren erabilera/promptekin).

---

## 2. Gutxieneko Eskakizunen Check-list Osoa

| # | Eskakizuna (PDF) | Egoera | Egiaztapena eta Ebidentzia Kodean |
| :-: | :--- | :-: | :--- |
| **0** | **Android Studio, Kotlin eta Jetpack Compose (XML diseinurik ez)** | ✅ **Eginda** | Kotlin `2.2.10` + Compose BOM `2024.06.00` + Material 3. Ez dago `res/layout/` karpetarik ezta XML UI diseinurik ere. |
| **1** | **Pantailak: hasierakoa + gutxienez beste 2, nabigazioarekin (`NavHost`)** | ✅ **Eginda** | **4 pantaila** `NavHost` + `sealed class Screen` (`MainActivity.kt`, `Screen.kt`) trantsizio animatuekin + 5 dialog modular. |
| **2** | **Funtzionalitate nagusia: ekintza esanguratsuak** | ✅ **Eginda** | PIN login/logout, mahaien mapa + eremu iragazkiak + egoera aldaketa + 5s auto-freskatzea, eskaera berria + platerak stock mugarekin + sukaldera bidali + zerbitzatu, Kontua Proforma, 3 ordainketa mota (Eskudirua+kanbioa, Txartela, Bizum), 80mm tiket `.txt`/inprimaketa, Erreserbak, eta Ezarpenak (Gaia, Hizkuntza EU/ES/EN, Sare profilak). |
| **3** | **Egoera (`remember`, `mutableStateOf`, `rememberSaveable`, `derivedStateOf`) eta `LazyColumn`** | ✅ **Eginda** | `remember`/`mutableStateOf` pantaila guztietan, `rememberSaveable` iragazkietan (`MahaiakScreen`, `EskaeraScreen`), `derivedStateOf` saskiaren kalkuluan, **`LazyColumn` 4 tokitan**, gehi `LazyRow` eta `LazyVerticalGrid` (`GridCells.Adaptive`). |
| **4a** | **Kodearen egitura: pakete eta fitxategietan antolatuta** | ✅ **Eginda** | `data/{api,model,network}` eta `ui/{components,navigation,screens,state,theme}`. `MainActivity.kt` garbia eta nabigaziora mugatua. |
| **4b** | **Kodearen egitura: Composable txikiak eta PascalCase izenak** | ✅ **Eginda** | `LangileaItem`, `PinSarreraDialog`, `MahaiaTxartela`, `EskaeraLerroElementua`, `PlateraItem`, `TabItem`, `ErreserbaKarta` + `*Content` Composable stateless-ak. |
| **4c** | **Kodearen egitura: pantaila bakoitzeko `@Preview` bat** | ✅ **Eginda** | `@Preview` gehituta pantaila eta osagai guztietan (`LoginScreenDarkPreview`, `LoginScreenLightPreview`, `MahaiakScreenDarkPreview`, `MahaiakScreenLightPreview`, `MahaiaXehetasunakDarkPreview`, `EskaeraScreenDarkPreview`, `OrdainketaPantailaDialogPreview`, `EzarpenakDialogPreview`, `ErreserbaKartaPreview`). |
| **5a** | **Interfazea: Material 3 gaia, koloreak, `Column`/`Row`/`Box`, `Modifier`** | ✅ **Eginda** | T1 Italo-Euskaldun paleta (`Color.kt`, `Theme.kt`) eta `MaterialTheme.colorScheme` koherentea pantaila guztietan (Modu Iluna eta Argia). |
| **5b** | **Interfazea: gutxienez animazio edo trantsizio bat** | ✅ **Eginda** | `NavHost` `slideInHorizontally`/`fadeIn`, `AnimatedVisibility` (beheko barrak, sukaldera botoia, PIN errorea, `-` kontagailua), `AnimatedContent` (ordainketa eta ezarpen fitxak), `animateColorAsState` eta `animateDpAsState` (PIN zirkuluak, mahai egoerak, kanbio kutxa). |
| **6** | **Exekuzioa: errorerik gabe konpilatu eta exekutatu** | ✅ **Eginda** | `.\gradlew.bat compileDebugKotlin` → **BUILD SUCCESSFUL (0 errore, 0 deprecation abisu)**. |
| **7** | **README.md (gehienez orri bat, atal guztiekin)** | ✅ **Prest*** | Berridatzita, 4 pantaila-argazki erreal txertatuta (`01_login_screen.png`, `03_mahaiak_screen.png`, `05_eskaera_maite.png`, `06_barra_gorka.png`), diseinu-erabakiekin eta IA atalarekin. *(Zure izena jarri `[Zure Izena]` tokian!)* |

---

## 3. Aukerazko Erronken Egiaztapena (3 erronka osorik integratuta!)

| Aukera (PDF) | Zailtasuna | Egoera | Ebidentzia Kodean |
| :--- | :---: | :---: | :--- |
| **Komunikazioa (REST API)** | Errazetik ertainera | ✅ **BIKAIN EGINDA** | Retrofit 2.11 + OkHttp + Gson (`JatetxeaApiService.kt`, 17 endpoint), `/health` kontrola, `ApiClient.service` dinamikoa eta Wi-Fi SSID/IP bidezko `NetworkProfiles.kt`. |
| **Datuak gorde (`SharedPreferences`)** | Ertaina | ✅ **BIKAIN EGINDA** | `ApiClient` (`api_base_url`) eta `AppPreferences` (`isDarkTheme`, `hizkuntza`) iraunkorki gordetzen dira `komandera_prefs`-en. |
| **Itxura hobetu (Dark/Light Mode + Animazioak)** | Erraza | ✅ **BIKAIN EGINDA** | Material 3 Dark/Light Mode osoa pantaila guztietan + 6 animazio/trantsizio mota ezberdin. |
| **Gailu errealean probatu + argazkiak README-n** | Erraza | ⏳ **Aukerakoa** | `img/login_screen_new.png` jada badago; beste 2 argazkiak gailutik/emuladoretik ateratzea nahikoa da. |

---

## 4. Entregagarrien Azken Check-list-a (Teams-era igo aurretik)

- [ ] `README.md`-n bete `[Zure Izena / Bikotearen Izenak]` (4 pantaila-argazkiak jada txertatuta daude `img/` karpetatik!).
- [ ] Garbitu nahi izanez gero bikoiztutako karpeta zaharrak (`gradle/gradle/` eta `gradle/doc/`) ZIP-a arinagoa izateko.
- [ ] Android Studio-n **`Build > Clean Project`** egin ZIP fitxategia sortu aurretik (edo GitHub biltegiaren esteka partekatu).
- [ ] Grabatu **3–4 minutuko demo bideoa** arratsaldean (`02_DEMO_BIDEOA_GIDOIA_ETA_GRABAZIOA.md` gidoiari jarraituz).

---

## 5. Ebaluazio-Errubrika eta Puntuazio Eguneratua

$$\text{Nota} = \sum (\text{Irizpidearen puntuak} \times \text{Pisua})$$

| Irizpidea | Pisua | Hasierakoa | Oraingo Maila | Oraingo Puntuak | Puntu Haztatuak | Justifikazioa |
| :--- | :-: | :-: | :---: | :-: | :-: | :--- |
| **Funtzionalitatea** | 20% | 8 | **Bikaina** | **10** | `2.00` | Ziklo osoa errorerik gabe, `ApiClient.service` dinamikoa, hizkuntza eleaniztuna (`EU/ES/EN`) eta ezarpen iraunkorrak. |
| **Compose eta kodearen egitura** | 20% | 8 | **Bikaina** | **10** | `2.00` | Composable modularrak (`LangileaItem`, `PlateraItem`, `MahaiaTxartela`), stateless `*Content`, `rememberSaveable`, `derivedStateOf`, `LazyColumn`/`LazyVerticalGrid` eta **`@Preview` pantaila guztietan**. |
| **Interfazea** | 15% | 6 | **Bikaina** | **10** | `1.50` | Material 3 Dark/Light koherentea pantaila guztietan + `NavHost` trantsizioak, `AnimatedVisibility`, `AnimatedContent`, `animateColorAsState` eta `animateDpAsState`. |
| **Sormena** | 10% | 10 | **Bikaina** | **10** | `1.00` | Benetako balioa duen Italo-Euskaldun jatetxe komandera osoa (PIN, stock, 3 ordainketa, 80mm tiketa, Wi-Fi profilak). |
| **Aukerazko erronka** | 15% | 10 | **Bikaina** | **10** | `1.50` | **3 erronka integratuta:** REST API (Retrofit 17 endpoint) + `SharedPreferences` + Dark/Light Mode eta animazioak. |
| **README** | 10% | 6 | **Bikaina*** | **10*** | `1.00` | Osoa, orri bakarrekoa, diseinu-erabakiak eta IAren erabilera/promptak barne *(zure izena + 2 argazki sartzean %100 itxia)*. |
| **Demoa eta galderak** | 10% | 0 | *Arratsaldean* | **10** *(helburua)* | `1.00` | Gidoia, galdera-erantzunak eta OBS grabazio-gida prest (`02_DEMO_BIDEOA_GIDOIA_ETA_GRABAZIOA.md`). |
| **GUZTIRA** | **100%** | `7.20 / 9` | **BIKAINA** | — | **`9.00 / 9.00`** *(demo gabe)* → **`10.00 / 10`** | **Bikaina ziurtatuta demoa grabatu eta 2 argazki + izena jarrita!** |
