# 1T Jatetxea - Sukaldeko KDS Android TV Aplikazioa

Sukaldeko langileek (sukaldari eta sukalde-laguntzaileek) eskaerak telebista pantailan ikusteko eta kudeatzeko garatutako Android TV aplikazioa, **Jetpack Compose for TV** erabiliz.

---

## 1. Ezaugarri Nagusiak

- **Jetpack Compose for TV (`androidx.tv.material3`, `androidx.tv.foundation`)**:
  - Telebistako agintearen bidezko (D-pad) foku nabigazio osoa (`Card`, `Button`, `LazyRow`).
  - Iluntasun handiko diseinua (High-contrast Dark UI) sukaldeko argitasun eta distantzia handietan garbi ikusteko.
- **REST API Konexioa (`backend_api`)**:
  - `GET /api/v1/eskaerak`: Eskaera aktiboak eta prestatu beharreko platerak eskuratzen ditu.
  - `GET /api/v1/eskaerak/{id}`: Eskaera bakoitzaren lerro xehatuak eta oharrak (alergenoak, puntua).
  - `PUT /api/v1/eskaerak/lerroak/{lerroId}/egoera`: Plater bakoitza banaka "Prestatua" edo "Eginda" moduan markatzeko.
  - `PUT /api/v1/eskaerak/{eskaeraId}/egoera`: Eskaera osoa "Prestatua" jartzeko.
- **Birkarga Automatikoko Polling-a**:
  - 4 segundoz behin atzeko planoan kontsultatzen du APIa (Coroutines eta StateFlow), zerbitzariek sartutako eskaera berriak berehala ager daitezen.
- **IP Konfigurazio Dinamikoa**:
  - Goiburuan klik eginez edo agintearekin hautatuta, APIaren IP helbidea aldatu daiteke (Emuladorean `http://10.0.2.2:5000/api/v1/` eta LANeko telebistan `http://192.168.1.166:5000/api/v1/`).

---

## 2. Arkitektura eta Paketeen Egitura

```
app/src/main/java/eus/goierri/jatetxea/sukaldea/
├── data/
│   ├── api/
│   │   ├── JatetxeaApiService.kt   # Retrofit HTTP interfazea
│   │   └── RetrofitClient.kt       # Singleton bezeroa eta IP aldaketa
│   └── model/
│       ├── Eskaera.kt              # Eskaera orokorra
│       ├── EskaeraLerroa.kt        # Plater bakoitza eta egoera
│       └── ApiErantzuna.kt         # APIaren erantzun estandarra
└── ui/
    ├── MainActivity.kt             # Android TV Activity nagusia
    ├── components/
    │   ├── GoiburuaBarra.kt        # Goiburuko barra (online, ordua, filtroak)
    │   └── EskaeraTxartela.kt      # Eskaeren TV txartela eta ekintza-botoiak
    ├── theme/
    │   ├── Color.kt                # Kolore paleta iluna
    │   └── Theme.kt                # Material3 TV gaia
    └── viewmodel/
        └── SukaldeaViewModel.kt    # Polling, egoera erreaktiboa eta ekintzak
```

---

## 3. Nola Ireki eta Exekutatu Android Studio-n

1. Ireki **Android Studio**.
2. **Open** aukeratu eta hautatu karpeta hau:
   `C:\Dev\GoierriEskola\2_maila\1T_Jatxetxea_Erronka\1T_Jatetxea_erronka_agents\sukaldea_kds_android_tv`
3. Gradle sinkronizazioa amaitutakoan, sortu edo hautatu **Android TV** gailu birtual bat (AVD):
   - Adibidez: `Android TV (1080p) - API 34`.
4. Sakatu **Run** (Play botoia berdea).
5. Aplikazioa telebistan irekiko da eta zuzenean konektatuko da jatetxearen backend APIarekin (`http://10.0.2.2:5000/api/v1/`).
