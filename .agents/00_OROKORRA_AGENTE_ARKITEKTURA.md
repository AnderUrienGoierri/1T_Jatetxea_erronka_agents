# 1T Jatetxea - Agente Anitzeko Arkitektura Orokorra (Multi-Agent System)

Dokumentu honek **1T Jatetxea Kudeaketa Sistema Integralaren** multi-agente egitura definitzen du. Proiektuaren tamaina, teknologia-aniztasuna (.NET, JavaFX, PHP, Flutter, Android, Docker, VirtualBox, Cisco, OPNsense) eta segurtasun-eskakizunak bermatze aldera, agente espezializatuen rola, erantzukizun-mugak eta integrazio-protokoloak finkatzen dira.

---

## 1. Agenteen Mapa eta Espezializazioak

| Agente ID | Agentearen Rola eta Izena | Azpisistema / Esparrua | Tekno-Pilaketa |
| :--- | :--- | :--- | :--- |
| `agente_01` | **Backend REST API Arkitektoa** | `01_aplikazioak/admin/backend_rest_api_csharp` | ASP.NET Core (.NET 10), MySQL 8.0, Docker (5050), Fakturak & Finantzak, Real-time Monitorea |
| `agente_02` | **Admin IT Zentralaren Ingeniaria** | `01_aplikazioak/admin/admin_IT_zentrala`, `Admin_IT_Zentrala.exe`, `odoo_erp` | ASP.NET Core (.NET 11/10), C# WinForms/WPF Launcher, Docker CLI, Odoo 17 ERP (5000) |
| `agente_03` | **Desktop C# TPV Ingeniaria** | `01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp` | C# .NET 10 Windows Forms, ESC/POS Inprimagailua, Mahai & Eskaera Kudeaketa |
| `agente_04` | **Desktop JavaFX Sistema Integrala** | `admin_desktop_java`, `desktop_sukaldea_kds`, `desktop_barra_bds`, `desktop_biltegia_stock`, `komunak/desktop_javafx_*` | Java 21 Temurin, JavaFX 21, FXML, Scene Builder, Maven Multi-Modulua (Admin, KDS, BDS, Stock) |
| `agente_05` | **Web PHP Garatzailea & DMZ Zaindaria** | `01_aplikazioak/bezeroak/web_php_bezeroak` | Apache, PHP 8.2/8.4, VLAN 20 DMZ (8085), cURL REST Client |
| `agente_06` | **Mugikor eta Android TV Garatzailea** | `mobile_zerbitzaria_flutter`, `zerbitzaria_komandera_android`, `mobile_bezeroa_flutter`, `sukaldea_kds_android_tv` | Flutter (Dart), Android Jetpack Compose for TV, Kotlin, ADB Scripts |
| `agente_07` | **Sare, Segurtasun eta DevSecOps Ingeniaria** | `02_azpiegitura_birtuala`, `03_laborategia` | OPNsense (8443), Cisco 2960 VLANak, WireGuard VPN, CI/CD |
| `agente_08` | **Figma UI/UX Ikuskatzaile eta Auditorea** | `gemini/`, Figma Educational Workspace | Figma Auto-Layout, Token Sistema, Inter / JetBrains, Zinc Dark Mode |

---

## 1.1 Proiektuko Aplikazio Guztien Matrizea Rolen Arabera

Proiektuak honako aplikazio eta modulu guztiak biltzen ditu:

1. **Admin / Kudeaketa:**
   - `Admin_IT_Zentrala.exe`: Erroko instalatzailea, konfigurazio morroia eta zerbitzuen abiarazle grafikoa.
   - `admin_IT_zentrala` (Portua: 5000): Zerbitzuen egoera eta orkestrazio web-panela.
   - `admin_desktop_java`: Kudeatzailearen JavaFX aplikazioa (Langileen kudeaketa, Karta/Menua, Fakturak eta Finantza Laburpena, Erreserbak).
   - `backend_rest_api_csharp` (Portua: 5050): Sistemaren ASP.NET Core REST API nagusia.
   - `odoo_erp` (Portua: 8069/8070): Enpresa Kudeaketa Integratua (ERP).
   - `mobile_android`: Admin IT Zentralaren Android mugikorreko kontrol-panela.

2. **Jangela / Zerbitzari Langileak:**
   - `desktop_tpv_csharp`: Areto eta kutxako TPV mahaigainekoa (.NET Windows Forms).
   - `mobile_zerbitzaria_flutter`: Zerbitzarien mugikor komandera azkarra (Flutter).
   - `zerbitzaria_komandera_android`: Zerbitzarien komandera natiboa (Android Kotlin).

3. **Barra Langileak:**
   - `desktop_barra_bds`: Bar Display System (BDS) bezeroa edariak eta barrako eskaerak prestatu eta markatzeko (JavaFX).

4. **Sukalde Langileak:**
   - `desktop_sukaldea_kds`: Kitchen Display System (KDS) bezeroa sukaldeko komandak kontrolatzeko (JavaFX).
   - `sukaldea_kds_android_tv`: Telebista handietarako KDS natiboa (Android TV Jetpack Compose, D-Pad kontrolarekin).
   - `sukaldea_tv` (Portua: 8086): Docker web bidezko KDS telebista simulagailua.

5. **Biltegi Langileak:**
   - `desktop_biltegia_stock`: Lehengaien stock sarrera, irteera, gutxieneko alertak eta hornitzaileen erosketak kudeatzeko bezeroa (JavaFX).

6. **Bezeroak:**
   - `web_php_bezeroak` (Portua: 8085): Bezeroen webgune publikoa (karta, mahai-erreserbak, iritziak).
   - `mobile_bezeroa_flutter`: Bezeroen mugikorretarako eskaera eta fidelizazio aplikazioa (Flutter).

7. **Liburutegi Partekatua (Komunak):**
   - `desktop_javafx_core`: JavaFX bezero guztientzako oinarri partekatua (PIN bidezko saio-hasiera rolaren arabera, REST API bezero unibertsala, modeloak, leiho-kudeatzailea).
   - `desktop_javafx_parent`: Maven guraso proiektua mendekotasun guztiak bateratzeko.

---

## 2. Ataken eta Zerbitzuen Banaketa Zorrotza

Edozein agente-aldaketak errespetatu behar du ondorengo ataka-mapa finkoa:

```text
====================================================================================
Ataka      Zerbitzua                     Protokoloa   VLAN       Agente Erantzulea
====================================================================================
5000       Admin IT Zentrala (Web UI)   HTTP         VLAN 99    agente_02
5050       ASP.NET Core REST API         HTTP         VLAN 10    agente_01
3308       MySQL 8.0 Datu-Base Nagusia   TCP/MySQL    VLAN 10    agente_01
8085       Web PHP Frontend Publikoa     HTTP         VLAN 20    agente_05
8086       Sukaldea KDS Web / TV Nginx   HTTP         VLAN 10    agente_04 / agente_06
8443       OPNsense WebGUI Kudeaketa     HTTPS        VLAN 99    agente_07
3001       Barne Txat Zerbitzaria        HTTP/WS      VLAN 10    agente_01 / agente_02
8070       Odoo 17 ERP                   HTTP         VLAN 10    agente_02 / agente_07
5432       PostgreSQL Odoo DB            TCP/PG       VLAN 10    agente_07
3391       WinServer 2025 Urruneko RDP   TCP/RDP      VLAN 10    agente_07
2230-2232  SSH Administrazio Atakak      TCP/SSH      VLAN 99    agente_07
====================================================================================
```

> [!CAUTION]
> Inoiz ez trukatu 5000 eta 5050 atakak. 5000 Admin IT Zentralerako da beti; 5050 REST APIrako da.

---

## 3. Agenteen Arteko Kontratuak eta Datu-Fluxua

```text
[Web PHP Publikoa (8085)]       [Mugikor Bezeroa (Flutter)]
          │                                   │
          │ (HTTP REST JSON)                  │ (HTTP REST JSON)
          ▼                                   ▼
┌─────────────────────────────────────────────────────────────┐
│          ASP.NET Core REST API Nagusia (5050)               │
│  - 32 Endpoint transakzional (SELECT FOR UPDATE)            │
│  - DatuBaseaMonitorZerbitzua (Resilient Health Check)       │
└──────────────────────────────┬──────────────────────────────┘
                               │
            ┌──────────────────┴──────────────────┐
            ▼                                     ▼
 [MySQL 8.0 DB (3308)]                 [Desktop C# TPV (Kutxa)]
 (InnoDB errenkada blokeoak)           [Desktop JavaFX KDS (Sukaldea)]
                                       [Mugikor Zerbitzaria (Flutter)]
                                       [Android TV KDS (1080p)]
```

### 3.1 Datu-Basea Itzalita Dagoeneko Salbuespen-Protokoloa
1. **APIren Portaera:** MySQL erorita badago, REST APIak martxan jarraitzen du (5050 atakan) eta errorea ematen du erantzun JSON garbi batean:
   ```json
   {
     "status": 503,
     "error": "Datu-basea deskonektatuta dago",
     "datuBaseaKonektatuta": false,
     "abisua": "[ADI! DATU-BASEA ITZALI DA / DESKONEKTATUTA]..."
   }
   ```
2. **Bezeroen Portaera (TPV, KDS, Web, Admin):** Agente guztiek ziurtatu behar dute bezeroek ez dutela kraskatzen (crash) konexio faltagatik; goiburuan mezu laranjaz/gorriz adieraziko dute «Datu-basea deskonektatuta».

---

## 4. Agenteen Egiaztapen-Ateak (Quality Gates)

Kode-lerro berri bat ekoizpenera igaro aurretik, hiru galbahe gainditu behar ditu:
1. **Diseinu Ate (agente_08):** `gemini/FIGMA_DISEINU_ARAUAK.md` arauak betetzen ditu (zinc iluna, semantika bakarrik, geruza-izenak = kode-izenak).
2. **Segurtasun Ate (agente_07):** `Zero Secret Leakage` eta isolamendu sare-arauak betetzen ditu; ez dago emojirik.
3. **Integrazio Ate (agente_01/02):** API kontratuak eta ataka finkoak errespetatzen ditu.
