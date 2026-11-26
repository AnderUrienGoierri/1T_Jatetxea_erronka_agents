# 2. TPV Diseinua

> [← Aurrekoa: 1. TPV Analisia](1_tpv_analisia.md) · [Aurkibidea](README.md) · [Hurrengoa: 3. TPV Garapena →](3_tpv_garapena.md)

## 2.1. Sistema-Arkitektura Orokorra

TPV mahaigaineko aplikazioak **Bezero-Zerbitzari (REST JSON)** arkitektura jarraitzen du, jatetxeko gainerako aplikazioekin (*Android Komandera* eta *Sukaldea KDS Android TV*) backend eta datu-base bera partekatuz:

```mermaid
flowchart LR
    subgraph Desktop["Desktop TPV C# (.NET 10 WinForms)"]
        UI["10 Forms + Theme.cs + UiPerf.cs"]
        API_CLIENT["Services/ApiService.cs<br/>(HttpClient + JSON DTO)"]
        UI <--> API_CLIENT
    end

    subgraph Backend["Jatetxea REST API (:5050)"]
        HEALTH["GET /health"]
        V1["/api/v1/*<br/>langileak, mahaiak, platerak,<br/>eskaerak, ordainketak, erreserbak"]
    end

    DB[("MySQL 8<br/>jatetxea_mysql")]
    KDS["Sukaldea KDS<br/>(Android TV)"]
    MOB["Komandera<br/>(Android Tablet)"]

    API_CLIENT <-->|"HTTP JSON (2.5s timeout)"| V1
    API_CLIENT -.->|"Osasun-kontrola"| HEALTH
    V1 <--> DB
    HEALTH <--> DB
    KDS <-->|"HTTP JSON"| V1
    MOB <-->|"HTTP JSON"| V1
```

---

## 2.2. Klase-Egitura eta Geruzak

```mermaid
classDiagram
    class Program {
        +Main()
    }
    class ApiService {
        -HttpClient _http
        -string _baseUrl
        +CheckHealthAsync()
        +LoginAsync(user, pin)
        +GetMahaiakAsync()
        +GetMahaiarenEskaeraGuztiakAsync(mahaiaId)
        +ProzesatuOrdainketaAsync(dto)
        +GetErreserbakAsync()
    }
    class Theme {
        +Color ChiantiPrimary
        +Color WarmLinen
        +Color GoldBorder
        +DisplayFont(size) Font
        +BodyFont(size, bold) Font
        +MonoFont(size, bold) Font
        +DrawT1Logo(g, bounds)
        +DrawRoundedCard(g, bounds, r, fill, border)
        +ApplyWarmHospitalityTheme(form)
    }
    class UiPerf {
        +EnableDoubleBuffering(root)
        +RunFlickerFree(container, action, preserveScroll)
    }
    class LoginForm
    class MahaiakForm
    class MahaiaXehetasunakForm
    class EskaeraForm
    class ErreserbakForm
    class OrdainketaForm

    Program --> LoginForm
    LoginForm --> MahaiakForm
    MahaiakForm --> MahaiaXehetasunakForm
    MahaiakForm --> EskaeraForm
    MahaiakForm --> ErreserbakForm
    MahaiaXehetasunakForm --> OrdainketaForm
    LoginForm --> ApiService
    MahaiakForm --> ApiService
    MahaiakForm --> Theme
    MahaiakForm --> UiPerf
    MahaiaXehetasunakForm --> Theme
    MahaiaXehetasunakForm --> UiPerf
```

---

## 2.3. Diseinu Sistema: T1 Jatetxea v2.0 (*Warm Hospitality*)

Diseinu osoa **`zerbitzaria_komandera_android`** estilo-gidarekin lerrokatu da eta **Figma**-n dokumentatu da profesionalki:

- **Figma fitxategia**: [T1 Jatetxea - Desktop TPV C# (UI Diseinua)](https://www.figma.com/design/DVmurlGmkzECO2NE6jTI8Y) (`2MG3 / desktop_TPV_csharp` karpetan)
- **3 orri Figma-n**: `01 · Estilo Gida & Osagaiak` (56 aldagai Light/Dark + 11 testu-estilo + 10 osagai), `02 · Pantaila Nagusiak (1440×900)` eta `03 · Modalak & Responsive (800×500)`.

![T1 Jatetxea Estilo Gida](img/00_estilo_gida.png)

### Kolore-Paleta (60–30–10 Araua & WCAG 2.2 AA)

| Token izena | Hex | Erabilera nagusia |
| :--- | :---: | :--- |
| **`WarmLinen`** | `#FAF7F2` | Aplikazioaren atzeko plano nagusia (%60) |
| **`MarbleWhite`** | `#FFFFFF` | Txartelak, goiko barrak eta azalera jasoak (%30) |
| **`StoneMist`** | `#F3EFE8` | Botoi sekundarioak, chip-ak eta hondo leunak |
| **`WarmTaupe`** | `#E6DFD5` | Txartel-ertzak eta marra banatzaileak |
| **`ChiantiPrimary`** | `#881337` | Marka-kolore nagusia, CTA botoiak eta aktibo-egoerak (%10) |
| **`PomodoroHover`** | `#9F1239` | Hover egoera botoi nagusietan |
| **`GoldBorder` / `AntiqueGold`** | `#D4A359` / `#B45309` | T1 monograma, laburpen-txartelen marko dotorea eta azentuak |
| **`EspressoInk`** | `#1C1917` | Testu nagusiak eta *Okupatuta* pilula iluna |
| **`WarmSlate` / `StoneMuted`** | `#57534E` / `#78716C` | Testu sekundarioak eta laguntza-etiketak |
| **`Success` / `SuccessBg`** | `#15803D` / `#DCFCE7` | *Libre* egoera eta *Eginda / Bidali* ekintza berdeak |
| **`Danger` / `DangerBg`** | `#B91C1C` / `#FEE2E2` | Erroreak, ezabaketa eta kontu hutsak (`✕`) |
| **`Warning` / `WarningBg`** | `#C2410C` / `#FFEDD5` | *Zai / Sukaldean* egoera |
| **`Info` / `InfoBg`** | `#0369A1` / `#E0F2FE` | *Garbitzen / Martxan* egoera |

### Tipografia Sistema (`Assets/Fonts/`)

| Maila | Letra-tipoa | Estiloa | Erabilera |
| :--- | :--- | :--- | :--- |
| **Display / Marka** | `Cinzel` | Bold | Goiburuko markak (`T1 JATETXEA · MAHAIAK`), T1 monograma |
| **UI / Gorputza** | `Plus Jakarta Sans` | Regular / Medium / SemiBold / Bold | Botoiak, mahai-izenak, platerak, nabigazio-fitxak |
| **Zenbatekoak / POS** | `JetBrains Mono` | Medium / Bold | Prezioak, `GUZTIRA`, kanbioa, 80mm-ko tiketa |

---

## 2.4. Responsive Diseinu Arkitektura (`Maximized` + `800×500` Min)

JavaFX-eko layout-eredu responsive-ak (`BorderPane`, `HBox`/`VBox`, `GridPane`, `FlowPane`, breakpoint listener) zuzenean WinForms-era itzuli dira 10 formulario guztietan:

| Kontzeptua | WinForms Inplementazioa | Portaera `800×500` → `4K` tartean |
| :--- | :--- | :--- |
| **Hasiera eta Muga** | `WindowState = Maximized`, `MinimumSize = (800, 500)`, `FormBorderStyle = Sizable` | Pantaila osoan irekitzen da (ataza-barra errespetatuz) eta `800×500`-raino txikitu daiteke. |
| **Egitura Nagusia** | `Dock = Top / Left / Right / Bottom / Fill` | Goiko barra goian, alboko panela eskuinean/ezkerrean eta edukia erdian beti. |
| **Txartel Saretak** | `FlowLayoutPanel` (`AutoScroll = true`, `WrapContents = true`) | Zutabe kopurua (2, 3, 4...) eta txartelen zabalera dinamikoki birkalkulatzen dira `Resize`-an. |
| **Breakpointak** | `< 1080px`, `< 1020px`, `< 960px`, `< 920px`, altuera `< 600px` | Goiko barra trinkotzen da (`84px` → `68px`), alboko panela estutzen da (`420px` → `290px`) eta erdiko testu osagarriak ezkutatzen dira gainjarpenik ez izateko. |

![Responsive Breakpoint 800x500](img/08_responsive_800x500.png)

---

## 2.5. Pantailen Katalogoa eta Nabigazio-Mapa

```mermaid
flowchart TD
    LOGIN["01 · LoginForm"] -->|"PIN zuzena"| MAHAIAK["02 · MahaiakForm"]
    MAHAIAK -->|"Mahai librea"| PAX["09 · MahaikideakModal"]
    PAX -->|"OK"| ESKAERA["04 · EskaeraForm"]
    ESKAERA -->|"Oharra"| OHARRA["10 · OharraModal"]
    MAHAIAK -->|"Okupatuta / Garbitzen"| XEH["03 · MahaiaXehetasunakForm"]
    XEH -->|"+ Produktuak"| ESKAERA
    XEH -->|"Kobratu"| ORD["06 · OrdainketaForm"]
    ORD -->|"Ordainduta"| TIK["07 · TiketIkusteaForm"]
    MAHAIAK -->|"Erreserbak"| ERR["05 · ErreserbakForm"]
    ERR -->|"+ Berria"| ERRB["08 · ErreserbaBerriaModal"]
```

### Pantaila Nagusien Diseinuak

#### 01 · Saioa Hasi (`LoginForm`)
![LoginForm diseinua](img/01_login_diseinua.png)

#### 02 · Mahaien Ikuspegia (`MahaiakForm`)
![MahaiakForm diseinua](img/02_mahaiak_diseinua.png)

#### 03 · Mahaiaren Xehetasunak eta Kontu Anitzak (`MahaiaXehetasunakForm`)
![MahaiaXehetasunakForm diseinua](img/03_xehetasunak_diseinua.png)

#### 04 · Komanda / Plateren Katalogoa (`EskaeraForm`)
![EskaeraForm diseinua](img/04_eskaera_diseinua.png)

#### 05 · Erreserben Egutegia (`ErreserbakForm`)
![ErreserbakForm diseinua](img/05_erreserbak_diseinua.png)

#### 06 · Ordainketa POS (`OrdainketaForm`) eta 07 · Tiket Fiskala (`TiketIkusteaForm`)

| Ordainketa POS (`OrdainketaForm`) | 80mm Tiket Fiskala (`TiketIkusteaForm`) |
| :---: | :---: |
| ![OrdainketaForm](img/06_ordainketa_diseinua.png) | ![TiketIkusteaForm](img/07_tiketa_diseinua.png) |
