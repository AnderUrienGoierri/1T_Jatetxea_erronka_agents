)

# 1T Jatetxea - Zerbitzaria Komandera (Android Jetpack Compose)

Android aplikazio natibo txiki, moderno eta osoa, **Kotlin** eta **Jetpack Compose (Material 3)** bidez garatua, 1T Jatetxeko zerbitzariek aretoko mahaiak kudeatu eta sukalderako komandak azkar sortzeko.

Mugikor eta tablet formatuetara guztiz egokitua (**Responsive** diseinua `LazyVerticalGrid`-ekin).

---

## Pantailak eta Funtzionalitateak

## 1. Pantailak eta Funtzionalitateak

Irakaslearen eskakizunak betez, aplikazioak **3 pantaila** nagusi ditu `NavHost` nabigazio modernoarekin:

1. **Hasierako Pantaila (`LoginScreen`):**
   - Zerbitzari aktiboa hautatzeko interfaze dotorea.
   - Txanda hasi eta aretoko mahaietara sartzeko balidazioa.
   - `@Preview` barne.

2. **Mahaien Mapa eta Egoerak (`MahaiakScreen`):**
   - Mahaien zerrenda/sarea egoera-kode bisualekin (Berdea = Libre, Gorria = Okupatuta, Urdina = Eskaeran, Horia = Fakturatua).
   - Eremuen araberako iragazkia (_Areto Nagusia_, _Terraza_, _VIP Gunea_).
   - Diseinu moldagarria (Mugikorrean 2 zutabe, Tabletan 4 zutabe).
   - `@Preview` barne.

3. **Komanda Sortzea eta Plateren Katalogoa (`EskaeraScreen`):**
   - Kategoria bidezko platerak (Hasierakoak, Plater Nagusiak, Postreak, Edariak).
   - Platerak gehitu, kendu, unitateak aldatu eta denbora errealean prezio osoa kalkulatu.
   - Komanda sukaldera bidaltzeko ekintza.
   - `@Preview` barne.

---

## Aukerazko Erronka: Komunikazioa (REST API)

## 2. Aukerazko Erronka: Komunikazioa (REST API)

- **Aukeratutako Erronka:** Web zerbitzu bati dei egin (**REST API Komunikazioa** Retrofit bidez).
- Aplikazioak komanda sortzean `POST /api/v1/eskaerak` deia egiten dio jatetxeko backend zentralari (`backend_rest_api_csharp` 5050 atakan). Sare konexiorik ez badago, tokiko modu seguruan erregistratzen da.

---

## Teknologia eta Egitura

## 3. Teknologia eta Egitura

- **Hizkuntza:** Kotlin 1.9.24
- **UI Framework:** Android Jetpack Compose + Material 3 (XML gabea)
- **Egoeraren Kudeaketa:** `remember`, `mutableStateOf`, `derivedStateOf`
- **Nabigazioa:** `androidx.navigation.compose.NavHost`
- **Zerrenda Optimizatuak:** `LazyColumn`, `LazyRow`, `LazyVerticalGrid`
- **Sarea:** Retrofit 2.11.0 + OkHttp + Gson

---

## Nola exekutatu Android Studion

## 4. Nola exekutatu Android Studion

1. Ireki Android Studio.
2. Hautatu `Open` eta joan proiektuaren karpetara:
   `01_aplikazioak/zerbitzaria_komandera_android`
3. Utzi Gradleri sinkronizatzen (`Sync Project with Gradle Files`).
4. Aukeratu emuladorea edo konektatutako mugikorra eta sakatu **Run (Shift + F10)**.
