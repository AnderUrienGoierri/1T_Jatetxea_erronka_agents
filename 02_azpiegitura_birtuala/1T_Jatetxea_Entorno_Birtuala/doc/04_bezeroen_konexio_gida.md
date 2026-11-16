# 04. Bezeroen Konexio eta Funtzionamendu Gida

Jatetxearen soluzioak bezero-mota anitz ditu, bakoitza bere rol eta gailu espezifikoarekin. Dokumentu honek bezero bakoitza nola konektatzen den eta nola probatzen den azaltzen du, monorepo egitura berria jarraituz.

---

## 1. Bezeroen Banaketa Funtzionala

```text
+-----------------------------------------------------------------------------------+
|                            APLIKAZIO BEZEROEN SAREA                               |
+-----------------------------------------------------------------------------------+
  |
  +--> 1. TPV Desktop (C# .NET 10 WinForms `01_aplikazioak/desktop_csharp`)
  |      Erabiltzaileak: Barrako zerbitzariak eta kutxazainak.
  |      Gailua: Ukipen-pantaila edo kutxako PCa (Windows).
  |      Funtzioa: Mahaien egoerak ikusi, komandak sortu, kontuak kobratu.
  |
  +--> 2. Zerbitzarien Mugikorra / Tableta (Flutter `01_aplikazioak/mobile_zerbitzaria`)
  |      Erabiltzaileak: Jangelako zerbitzariak.
  |      Gailua: Android / iOS tableta edo smartphone-a.
  |      Funtzioa: Mahai ondoan komandak hartu eta sukaldera zuzenean bidali.
  |
  +--> 3. Sukaldea, Barra eta Biltegia (JavaFX 21 `01_aplikazioak/desktop_javafx`)
  |      Erabiltzaileak: Sukaldari-burua, tabernaria eta biltegiko arduraduna.
  |      Gailua: PCa edo sukaldeko terminal finkoa.
  |      Funtzioa: Sukaldeko KDS (martxan/prest), Barrako BDS eta lehengaien stocka.
  |
  +--> 4. Sukaldeko Android TV Pantaila (`01_aplikazioak/sukaldea_kds_android_tv`)
  |      Erabiltzaileak: Sukaldeko talde osoa.
  |      Gailua: Smart TV edo Android TV sukaldeko horman (Portu 8086).
  |      Funtzioa: Eskaera aktibo guztiak urrunetik argi ikustea.
  |
  +--> 5. Bezero Publikoaren Webgunea (PHP 8.4 Apache `01_aplikazioak/web_php`)
  |      Erabiltzaileak: Jatetxeko bezero orokorrak (Internetetik - Portu 8085).
  |      Gailua: Nabigatzailea (mugikorra edo PCa).
  |      Funtzioa: Karta ikusi, prezioak kontsultatu, mahaiak erreserbatu.
  |
  +--> 6. Bezeroaren Mugikorreko App-a (Flutter `01_aplikazioak/mobile_bezeroa`)
  |      Erabiltzaileak: Bezero erregistratuak.
  |      Gailua: Mugikor pertsonala.
  |      Funtzioa: Erreserbak kudeatu, platerak eskatu, iritziak utzi.
  |
  +--> 7. Admin IT Zentrala (`01_aplikazioak/admin_IT_zentrala`)
  |      Erabiltzaileak: IT Administratzailea eta Zerbitzari Burua.
  |      Gailua: Web panela (Portu 5000) eta Android Mugikorra (Jetpack Compose).
  |      Funtzioa: Docker zerbitzuak, sare portuak eta fluxu koordinatua kudeatzea.
  |
  +--> 8. Odoo 17 ERP (`01_aplikazioak/odoo_erp`)
         Erabiltzaileak: Zuzendaritza eta Giza Baliabideak (Portu 8070).
         Gailua: Web interfazea (PC/Tablet).
         Funtzioa: Erabiltzaileen baimenak, txandak, fitxaketak eta bajak.
```

---

## 2. Nola Exekutatu Bezero Bakoitza

### 1. C# TPV Mahaigainekoa (`desktop_csharp`)

- **Kokapena:** `01_aplikazioak/desktop_csharp`
- **Abioa terminaletik:**

  ```powershell
  cd 01_aplikazioak\desktop_csharp
  dotnet run
  ```

- **Abioa IT Paneletik:** `http://localhost:5000` helbidera joan eta **"Exekutatu C# TPV"** sakatu (berehala irekiko da interfaze natiboa).
- **Kredentzialak:** PIN bidezko sarbidea (adibidez: `1234` edo langilearen PIN zenbakia).

### 2. JavaFX Mahaigainekoa (`desktop_javafx`)

- **Kokapena:** `01_aplikazioak/desktop_javafx`
- **Abioa terminaletik:**

  ```powershell
  cd 01_aplikazioak\desktop_javafx
  .\hasi_kds_mahaigaina.bat
  # Edo Maven Wrapper erabiliz:
  .\mvnw.cmd javafx:run
  ```

- **Abioa IT Paneletik:** `http://localhost:5000` helbidera joan eta **"Exekutatu JavaFX (KDS / BDS / Stock)"** sakatu (leiho grafikoa berehala irekiko da).
- **Ezaugarriak:** Fitxetan banatutako 3 gune nagusi:
  - `Sukaldea (KDS)`: Zai dauden platerak sukaldean sartu eta prestatuta markatu.
  - `Barra (BDS)`: Edariak eta kafeak kudeatu.
  - `Biltegia (Stocka)`: Lehengaien stock maila eta albaranen sarrerak.

### 3. Flutter Zerbitzaria (`mobile_zerbitzaria`)

- **Kokapena:** `01_aplikazioak/mobile_zerbitzaria`
- **Abioa:**

  ```powershell
  cd 01_aplikazioak\mobile_zerbitzaria
  flutter run
  ```

### 4. Flutter Bezeroa (`mobile_bezeroa`)

- **Kokapena:** `01_aplikazioak/mobile_bezeroa`
- **Abioa:**

  ```powershell
  cd 01_aplikazioak\mobile_bezeroa
  flutter run
  ```

### 5. Sukaldeko Android TV Pantaila (`sukaldea_kds_android_tv`)

- **Kokapena:** `01_aplikazioak/sukaldea_kds_android_tv`
- **Sarbidea:** Nabigatzailetik edo Smart TVtik:
  - Helbidea: `http://localhost:8086` (edo `http://<ZERBITZARI_IP>:8086`)

### 6. Web PHP Karta eta Erreserbak (`web_php`)

- **Kokapena:** `01_aplikazioak/web_php`
- **Teknologia:** Apache + PHP 8.4
- **Sarbidea:** `http://localhost:8085`

### 7. Admin IT Zentrala (`admin_IT_zentrala`)

- **Kokapena:** `01_aplikazioak/admin_IT_zentrala`
- **Web Panela:** `http://localhost:5000`
- **Android Aplikazioa:** `01_aplikazioak/admin_IT_zentrala/mobile_android` (Android Studio edo `./gradlew installDebug`)

### 8. Odoo 17 ERP (`odoo_erp`)

- **Sarbidea:** `http://localhost:8070`
- **Erabiltzailea:** `admin` (edo sortutako kudeatzailea)
- **Pasahitza:** `Errotagain169232$$`
