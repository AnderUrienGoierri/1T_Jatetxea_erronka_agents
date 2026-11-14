# T1 Jatetxea · Bezeroen Mugikorreko Aplikazioa (mobile_bezeroa)

Flutter bidez garatutako Android aplikazio profesionala bezeroek jatetxeko esperientzia osoa kudeatu ahal izateko: karta arakatzea, mahaian edo eramateko eskaerak egitea, ordainketa zibersegurua burutzea, mahaiak erreserbatzea eta iritziak uztea.

---

## 1. Funtzionalitate Nagusiak

1. **Karta Digital Interaktiboa:**
   - Kategorien araberako iragazketa azkarra (Entranteak, Pastak, Pizzak, Haragiak, Arrainak, Postreak, Edariak).
   - Bilaketa dinamikoa denbora errealean (plateraren izena edo osagaiak).
   - Prestatze denborak, alergenoen abisuak eta sukaldeko stock/ahalmen kontrola.
   - Plateraren xehetasun modala ohar pertsonalizatuekin (sukaldariarentzat).

2. **Saski Digitala eta Eskaera Mota:**
   - Bi modalitate: **Mahaitik eskatu** (mahaiaren zenbakia adieraziz) edo **Eramateko (Takeaway)**.
   - Kopuruak aldatzeko eta elementuak ezabatzeko kontrol errazak.
   - Finantza xehetasunak: Azpitotala, %10 BEZ barne eta guztizkoa.

3. **Ordainketa Zibersegurua (Fintech & Cybersecurity Simulation):**
   - **Ordainbide anitzak:** Kreditu/Zor Txartela (VISA, Mastercard, AMEX), Bizum, Google Pay, Apple Pay eta PayPal.
   - **Txartel Dinamikoa:** Txartel interaktiboa txartel-markaren detekzio automatikoarekin, titularrarekin eta iraungitze-datarekin.
   - **Luhn Algoritmoa:** Kreditu txartelen zenbakien balidazio matematikoa.
   - **PCI-DSS Maila 1 Betetzea:** Datu sentikorrak ez dira inoiz biltegiratzen; SHA-256 eta AES-256 bidezko tokenizazioa aplikatzen da (`tok_pci_aes256_...`).
   - **3D Secure 2.0 / PSD2 Autentifikazioa:** SMS bidezko 6 digituko OTP simulazio profesionala banku igorlearen segurtasuna erakusteko.
   - **Txartel Maskaratua:** Ordainagiri eta transakzio-erregistroetan azken 4 digituak bakarrik erakusten dira (`**** **** **** 1234`).
   - **Ordainagiri Digitala:** Eskaeraren lerroak, zerga-datuak, transakzioaren auditoretza tokena eta ordainketaren erreferentzia.

4. **Mahai Erreserbak:**
   - Egunaren aukeraketa egutegian (gaurkotik 90 egunera).
   - Txandak: Bazkaria 1. txanda, Bazkaria 2. txanda, Afaria 1. txanda, Afaria 2. txanda.
   - Mahaikide kopuruaren hautatzailea (1etik 16ra) eta mahai zehatzaren aukeraketa.
   - **HTTP 409 Conflict Kudeaketa:** Mahaia data eta txanda horretan jada erreserbatuta badago, gatazka abisua ematen du beste aukera bat hautatzeko eskatuz.

5. **Iritziak eta Balorazioak:**
   - Jatetxeko bezeroen batez besteko puntuazioa eta 5 izarretako barra banaketak.
   - Iritzi berria uzteko formulario interaktiboa (1-5 izarren klik bidezko hautaketa, izena, emaila eta iruzkina).
   - Bezeroen iritzi publikoen zerrenda.

6. **Informazioa eta API Zerbitzariaren Ezarpenak:**
   - Osteria & Parrillaren aurkezpena, ordutegiak, kokapena eta harremanetarako datuak.
   - API Base URL konfiguratzeko panela osasun-kontrolarekin (`checkHealth`), sare lokaleko IP desberdinetan probatzeko.

---

## 2. Diseinu-Sistema: Italiar-Euskaldun Estetika

Aplikazioak `desktop_javafx` eta `web_php`-n ezarritako kolore eta tipografia identitatea jarraitzen du:

- **Chianti Ardo Gorria (`#881337`):** Kolore nagusia goiburuetan, botoi garrantzitsuetan eta akzidenteetan.
- **Parmigiano Urrea (`#FCD34D`):** Prezio-etiketak, izarrak eta txartel bereizgarriak.
- **Albahaka Berdea (`#15803D`):** Baieztapenak, segurtasun adierazleak eta arrakasta mezua.
- **Toskana Terrakota (`#C2410C`):** Alergenoen abisuak eta sukaldaritza oharrak.
- **Atzealde Epelea (`#FFFBEB`):** Toskanako landa giroa gogorarazten duen hondo atsegina.

---

## 3. Arkitektura eta Fitxategien Egitura

```
mobile_bezeroa/
├── lib/
│   ├── main.dart                                    # Sarrera nagusia eta gaia
│   ├── theme/
│   │   └── app_theme.dart                           # Kolore tokenak eta Material 3 gaia
│   ├── models/
│   │   ├── platera.dart                             # Plateraren datu-eredua
│   │   ├── cart_item.dart                           # Saskiko lerroa (kopurua eta oharrak)
│   │   ├── ordainketa.dart                          # Ordainketa motak eta emaitzak
│   │   ├── erreserba.dart                           # Mahai erreserbaren eredua
│   │   └── iritzia.dart                             # Iritziak eta estatistikak
│   ├── services/
│   │   ├── api_service.dart                         # Backend REST API komunikazioa
│   │   ├── cart_service.dart                        # Saskiaren egoera globala (ChangeNotifier)
│   │   └── ordainketa_service.dart                  # Ordainketa pasabide eta zibersegurtasun logika
│   └── screens/
│       ├── home_navigation_screen.dart              # Beheko 4 fitxen nabigazio barra
│       ├── karta_screen.dart                        # Karta arakatzailea eta bilaketa
│       ├── saskia_screen.dart                       # Saskia, eskaera mota eta bezero datuak
│       ├── erreserbak_screen.dart                   # Mahai erreserba formularioa
│       ├── iritziak_screen.dart                     # Iritziak eta balorazio estatistikak
│       ├── info_screen.dart                         # Jatetxearen info eta API konfigurazioa
│       └── ordainketa/
│           ├── ordainketa_screen.dart               # Ordainbideen panela eta txartel dinamikoa
│           ├── ordainketa_otp_dialog.dart           # 3D Secure 2.0 PSD2 SMS OTP simulazioa
│           └── ordainketa_arrakasta_screen.dart     # Ordainagiri digitala eta audit tokena
└── test/
    └── widget_test.dart                             # Widget proba automatikoa
```

---

## 4. Nola Ireki eta Abiarazi Android Studion

1. **Ireki Android Studio:**
   - Aukeratu **Open** eta hautatu `mobile_bezeroa` karpeta.
2. **Dependentziak eskuratu:**
   - Terminalean: `flutter pub get`
3. **Android Gailua / Emuladorea:**
   - Piztu Android Emuladorea (Device Manager-etik) edo konektatu mugikor fisiko bat USB bidez (USB arazketa aktibatuta).
4. **Abiarazi:**
   - Egin klik **Run 'main.dart'** botoian (Triangelu berdea) edo terminalean:
     ```bash
     flutter run
     ```

---

## 5. Backend-arekiko Konexioa

- **Android Emulator bidez:** `http://10.0.2.2:5000/api/v1` helbidea lehenetsita dago (emuladoreak ordenagailu anfitrioira jotzeko erabiltzen duena).
- **Mugikor fisiko bidez:** Joan aplikazioaren **Informazioa** fitxara eta sartu zure ordenagailuaren tokiko IP helbidea (adibidez: `http://192.168.1.50:5000/api/v1`) eta sakatu **Gorde eta Egiaztatu**.
