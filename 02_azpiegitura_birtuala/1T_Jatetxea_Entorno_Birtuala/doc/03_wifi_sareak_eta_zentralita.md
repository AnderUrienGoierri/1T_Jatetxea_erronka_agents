# 03. WiFi Sareak eta Sare Zentralita Dinamikoa

Garatzaile baten eguneroko lanean, ohikoa da kokapen fisikoz aldatzea: ikastetxeko gelan egotea, etxean egotea edo beste etxe batean (adibidez, Tolosako etxean). Sare bakoitzak IP tarte desberdina esleitzen dio ordenagailuari (DHCP bidez).

Dokumentu honek azaltzen du nola konpontzen duen arazo hau **1T Jatetxearen Sare Zentralitak** (`switch-env.ps1`).

---

## 1. Arazoaren Deskribapena

Aplikazio bezeroek (Flutter mugikorreko aplikazioak, JavaFX mahaigaineko KDSak, C# TPVak, Web PHP frontendak) API zerbitzariaren IP helbidea jakin behar dute harekin komunikatzeko:

- Helbidea kodean gogortuta (hardcoded) badago (adibidez `192.168.1.129`), Tolosako etxean ondo ibiliko da, baina Goierri Eskolara joatean (`10.x.x.x` sarean) bezero guztiek konexio errorea emango dute.
- Helbidea eskuz proiektu bakoitzean aldatzeak denbora galtzea eta akatsak eragiten ditu.

---

## 2. Irtenbidea: Zentralita Automatikoa (`switch-env.ps1`)

`02_azpiegitura_birtuala/1T_Jatetxea_Entorno_Birtuala/scripts/switch-env.ps1` scriptak zentralita baten gisa funtzionatzen du:

1. Ordenagailuko WiFi eta Ethernet egokitzaileen IP helbideak automatikoki detektatzen ditu.
2. Aurrez definitutako profilak eskaintzen ditu:
   - **`localhost`:** Garatzailearen makinako probak (`127.0.0.1` / portu lokala `5050`).
   - **`tolosako_etxea`:** Tolosako etxeko WiFi sarea (`192.168.1.129`).
   - **`eskola`:** Goierri Eskolako laborategia (`10.x.x.x`).
   - **`etxean`:** Etxeko ohiko sarea.
   - **`vbox_erronka`:** VirtualBox-eko 3 makina birtualen ingurunea (`192.168.10.10`).
   - **`custom`:** Edozein IP helbide pertsonalizatu idazteko aukera.
3. Behin profila hautatuta, **klik bakarrean** sinkronizatzen du IP helbide berria monorepoko bezero guztietan:
   - `01_aplikazioak/zerbitzari_langileak/mobile_zerbitzaria_flutter` (Flutter zerbitzarien komandera).
   - `01_aplikazioak/bezeroak/mobile_bezeroa_flutter` (Flutter bezeroen aplikazioa).
   - `01_aplikazioak/desktop_javafx` (Sukaldea, Barra eta Biltegia).
   - `01_aplikazioak/desktop_csharp` (TPV Windows Forms).
   - `01_aplikazioak/bezeroak/web_php_bezeroak` (PHP 8.4 Frontend publikoa).

---

## 3. Nola Erabili Zentralita

PowerShell terminalean exekutatu komando hau proiektuaren errotik:

```powershell
# Modu interaktiboan (menua agertuko da aukeratzeko)
powershell -ExecutionPolicy Bypass -File 02_azpiegitura_birtuala\1T_Jatetxea_Entorno_Birtuala\scripts\switch-env.ps1

# Edo zuzenean profilaren izena pasata
powershell -ExecutionPolicy Bypass -File 02_azpiegitura_birtuala\1T_Jatetxea_Entorno_Birtuala\scripts\switch-env.ps1 -Profile tolosako_etxea
powershell -ExecutionPolicy Bypass -File 02_azpiegitura_birtuala\1T_Jatetxea_Entorno_Birtuala\scripts\switch-env.ps1 -Profile eskola
powershell -ExecutionPolicy Bypass -File 02_azpiegitura_birtuala\1T_Jatetxea_Entorno_Birtuala\scripts\switch-env.ps1 -Profile localhost
powershell -ExecutionPolicy Bypass -File 02_azpiegitura_birtuala\1T_Jatetxea_Entorno_Birtuala\scripts\switch-env.ps1 -Profile vbox_erronka
```

### Irteeraren Adibidea

```text
==========================================================
       1T JATETXEA - SARE ZENTRALITA ETA PROFILAK         
==========================================================

Uneko ordenagailuan detektatutako sare helbideak:
  - Wi-Fi: 192.168.1.129

Aplikazio guztiak helbide honetara sinkronizatzen: http://192.168.1.129:5050/api/v1 ...
  [OK] Flutter Zerbitzaria eguneratua: 01_aplikazioak\zerbitzari_langileak\mobile_zerbitzaria_flutter\lib\services\api_service.dart
  [OK] Flutter Bezeroa eguneratua:     01_aplikazioak\bezeroak\mobile_bezeroa_flutter\lib\services\api_service.dart
  [OK] JavaFX Desktop eguneratua:      01_aplikazioak\desktop_javafx\src\main\resources\api.properties
  [OK] C# Desktop TPV eguneratua:      01_aplikazioak\desktop_csharp\appsettings.json
  [OK] Web PHP Frontend eguneratua:    01_aplikazioak\bezeroak\web_php_bezeroak\.env

Sinkronizazioa osatuta!
Uneko Profil Aktiboa: tolosako_etxea
REST API Helbidea:    http://192.168.1.129:5050/api/v1
Admin IT Zentrala:    http://192.168.1.129:5000
Web PHP Helbidea:     http://192.168.1.129:8085
Chat Server Helbidea: http://192.168.1.129:3001
Odoo ERP Helbidea:    http://192.168.1.129:8070
KDS Android TV:       http://192.168.1.129:8086
==========================================================
```

Guztia eguneratuta geratzen denez, mugikorreko emuladorea edo JavaFX aplikazioa abiaraztean, uneko WiFi sareko zerbitzariarekin konektatuko da berehala.
