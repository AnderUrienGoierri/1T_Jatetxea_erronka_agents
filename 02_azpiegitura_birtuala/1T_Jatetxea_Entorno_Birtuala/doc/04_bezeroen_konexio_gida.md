# 04. Bezeroen Konexio eta Funtzionamendu Gida

Jatetxearen soluzioak bezero-mota anitz ditu, bakoitza bere rol eta gailu espezifikoarekin. Dokumentu honek bezero bakoitza nola konektatzen den eta nola probatzen den azaltzen du.

---

## 1. Bezeroen Banaketa Funtzionala

```
+-----------------------------------------------------------------------------------+
|                            APLIKAZIO BEZEROEN SAREA                               |
+-----------------------------------------------------------------------------------+
  |
  +--> 1. TPV Desktop (C# .NET 10 WinForms)
  |      Erabiltzaileak: Barrako zerbitzariak eta kutxazainak.
  |      Gailua: Ukipen-pantaila edo kutxako PCa (Windows).
  |      Funtzioa: Mahaien egoerak ikusi, komandak sortu, kontuak kobratu.
  |
  +--> 2. Zerbitzarien Mugikorra / Tableta (Flutter `mobile_zerbitzaria`)
  |      Erabiltzaileak: Jangelako zerbitzariak.
  |      Gailua: Android / iOS tableta edo smartphone-a.
  |      Funtzioa: Mahai ondoan komandak hartu eta sukaldera zuzenean bidali.
  |
  +--> 3. Sukaldea, Barra eta Biltegia (JavaFX 21 `desktop_javafx`)
  |      Erabiltzaileak: Sukaldari-burua, biltegiko arduraduna.
  |      Gailua: PCa edo sukaldeko terminal finkoa.
  |      Funtzioa: Sukaldeko KDS (martxan/prest), Barrako BDS eta lehengaien stocka.
  |
  +--> 4. Sukaldeko Android TV Pantaila (KDS Web Display)
  |      Erabiltzaileak: Sukaldeko talde osoa.
  |      Gailua: Smart TV edo Android TV (1080p/4K) sukaldeko horman.
  |      Funtzioa: Eskaera aktibo guztiak urrunetik argi ikustea.
  |
  +--> 5. Bezero Publikoaren Webgunea (PHP / HTML5 / JS `web_php`)
  |      Erabiltzaileak: Jatetxeko bezero orokorrak (Internetetik).
  |      Gailua: Nabigatzailea (mugikorra edo PCa).
  |      Funtzioa: Karta ikusi, prezioak kontsultatu, mahaiak erreserbatu.
  |
  +--> 6. Bezeroaren Mugikorreko App-a (Flutter `mobile_bezeroa`)
  |      Erabiltzaileak: Bezero erregistratuak.
  |      Gailua: Mugikor pertsonala.
  |      Funtzioa: Erreserbak kudeatu, platerak eskatu, iritziak utzi.
  |
  +--> 7. Odoo 17 ERP (Kudeaketa Zentralizatua)
         Erabiltzaileak: Zuzendaritza eta Giza Baliabideak.
         Gailua: Web interfazea (PC/Tablet).
         Funtzioa: Erabiltzaileen baimenak, txandak, fitxaketak eta bajak.
```

---

## 2. Nola Exekutatu Bezero Bakoitza

### 1. C# TPV Mahaigainekoa (`desktop_csharp`)
* **Kokapena:** `C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\desktop_csharp`
* **Abioa:**
  ```powershell
  cd C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\desktop_csharp
  dotnet run
  ```
* **Kredentzialak:** PIN bidezko sarbidea (adibidez: `1234` edo langilearen PIN zenbakia).

### 2. JavaFX Mahaigainekoa (`desktop_javafx`)
* **Kokapena:** `C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\desktop_javafx`
* **Abioa:**
  ```powershell
  cd C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\desktop_javafx
  mvn clean javafx:run
  ```
* **Ezaugarriak:** Fitxetan banatutako 3 gune nagusi:
  * `Sukaldea (KDS)`: Zai dauden platerak sukaldean sartu eta prestatuta markatu.
  * `Barra (BDS)`: Edariak eta kafeak bakarrik kudeatu.
  * `Biltegia (Stocka)`: Lehengaien stock maila eta albaranen sarrerak.

### 3. Flutter Zerbitzaria (`mobile_zerbitzaria`)
* **Kokapena:** `C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\mobile_zerbitzaria`
* **Abioa:**
  ```powershell
  cd C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\mobile_zerbitzaria
  flutter run
  ```

### 4. Flutter Bezeroa (`mobile_bezeroa`)
* **Kokapena:** `C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\mobile_bezeroa`
* **Abioa:**
  ```powershell
  cd C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\mobile_bezeroa
  flutter run
  ```

### 5. Sukaldeko Android TV Pantaila
* **Sarbidea:** Edozein telebista adimendun edo nabigatzailetatik:
  * Helbidea: `http://<ZERBITZARI_IP>:8086`

### 6. Odoo ERP (Giza Baliabideak eta Baimenak)
* **Sarbidea:**
  * Helbidea: `http://<ZERBITZARI_IP>:8069`
  * Erabiltzailea: `admin` (edo sortutako kudeatzailea)
  * Pasahitza: `Errotagain169232$$`
