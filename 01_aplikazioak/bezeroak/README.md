# 1T Jatetxea · Bezeroen Aplikazioak (01_aplikazioak/bezeroak)

Karpeta honek jatetxeko bezeroek erabiltzen dituzten aplikazio guztiak biltzen ditu:

```
01_aplikazioak/bezeroak/
├── web_php_bezeroak/           # Webgune korporatiboa, karta interaktiboa, alergenoak, erreserbak eta iritziak (PHP 8.4 / Apache)
└── mobile_bezeroa_flutter/     # Bezeroen mugikorreko aplikazioa: karta, auto-eskaera mahaian eta ordainketa digitala (Flutter / Dart)
```

---

## 1. Aplikazioak

### 1.1. Web PHP Bezeroak (`web_php_bezeroak`)
- **Teknologia:** PHP 8.4, Apache 2.4, HTML5, Vanilla JavaScript, Tailwind CSS.
- **Portua:** `8085` (HTTP).
- **Helbidea:** `http://localhost:8085` edo sarean `http://<IP>:8085`.
- **Ezaugarriak:**
  - Jatetxearen gastronomia italiar-euskaldunaren karta digital osoa.
  - Alergenoen araberako iragazketa azkarra eta stockaren araberako plater erabilgarritasuna.
  - Mahai-erreserbak online egiteko modulua (dataren, txandaren eta mahaikide kopuruaren arabera).
  - Bezeroen iritziak eta 1-5 izarretako balorazioak kontsultatu eta uzteko atala.
  - REST APIarekin (`http://localhost:5050/api/v1`) komunikazio estandarra (`X-App-Id: web_php`).

### 1.2. Flutter Bezero Aplikazioa (`mobile_bezeroa_flutter`)
- **Teknologia:** Flutter 3, Dart 3 (Android, iOS eta Web laguntzarekin).
- **Plataforma Nagusia:** Android (Mugikorrak eta Tabletak).
- **Ezaugarriak:**
  - QR kode bidez mahaian bertan eskaerak auto-kudeatzeko interfaze arina.
  - Saski digitala, plateren ohar pertsonalizatuak (adib. sukaldaritza puntua).
  - FinTech eta zibersegurtasun simulazio aurreratua: Kreditu-txartelen Luhn balidazioa, tokenizazio zibersegurua (PCI-DSS simulazioa), eta 3D Secure / OTP autentifikazioa.
  - Mahai-erreserben kudeaketa eta iritzi sistema.
  - REST APIarekin (`http://localhost:5050/api/v1`) sinkronizazio osoa (`X-App-Id: mobile_bezeroa`).
