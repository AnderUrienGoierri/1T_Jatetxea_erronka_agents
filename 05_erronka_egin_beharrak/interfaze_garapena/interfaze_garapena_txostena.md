# Interfazeen Garapena — Erronkako Egin Beharren Azterketa eta Dokumentazio Osoa

**Modulua:** 0488 · Interfazeen Garapena (Goierri Eskola — DAM)  
**Proiektua:** 1T Jatetxea — Erronka Integrala  
**Aztertutako aplikazio nagusiak:**
- **TPV (Areto & Kutxazaina):** `01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp` (C# · `.NET 10` WinForms)
- **Gerentea (Administrazioa):** `01_aplikazioak/admin/admin_desktop_java` (Java 21 · JavaFX `21.0.6` + AtlantaFX)
- **Backend REST API:** `01_aplikazioak/admin/backend_rest_api_csharp` (ASP.NET Core `.NET 10` + NHibernate)

**Oinarri-dokumentuak:** [interfaze_garapena_egin_beharrak.png](interfaze_garapena_egin_beharrak.png), [INTERFAZEEN_GARAPENA_TEORIA_LABURPENA.pdf](INTERFAZEEN_GARAPENA_TEORIA_LABURPENA.pdf) eta [MVC_ETA_JAVAFX_PANE_LABURPENA.pdf](MVC_ETA_JAVAFX_PANE_LABURPENA.pdf).

---

## 0. Laburpen Taula: Egin Beharren Egoera Erreala (11 Irizpideak)

Kodea, dokumentazioa eta diagramak zuzenean egiaztatu ondoren, hau da `interfaze_garapena_egin_beharrak.png` taulako **11 puntuen** benetako egoera:

| # | Noiz konprobatu | Zer konprobatu | Egoera erreala | Laburpena / Hutsuneak |
|---|---|---|---|---|
| **1** | Dokumentuan | Diseinu sistema PDF-an eta dokumentuan dago, eta aplikazio guztiekin bat dator | **Beteta (% 100)** | Badago PDF ofiziala (`T1_Jatetxea_Diseinu_Sistema_eta_Koloreak.pdf`), MD dokumentazioa eta TPV Figma estilo-gida (`2_tpv_diseinua.md`). Marka `#881337` eta egoera-koloreak bateratuta daude; v1 (Slate, Gerentea) eta v2 (*Warm Hospitality*, TPV) bilakaera dokumentatuta dago. |
| **2** | Dokumentuan | Klase diagrama (DrawIO, TPV eta Gerente) PDF-an eta dokumentuan dago, eta aplikazioarekin bat dator | **Beteta (% 100)** | `klase_diagrama.drawio`-k 3 orri oso ditu (`Sistema Orokorra`, `Gerente Aplikazioa (JavaFX MVC)` eta `TPV Aplikazioa (C# WinForms)`), `klase_diagrama.md` eguneratuta dago eta aplikazioen benetako klase, interfaze eta herentziekin % 100 bat dator. |
| **3** | Aplikazioa erakustean | Mahaigaineko aplikazioa JavaFX-ekin garatua | **Beteta (% 100)** | `admin_desktop_java`: Java 21, JavaFX `21.0.6` (`javafx-controls`, `javafx-fxml`), AtlantaFX `2.0.1`, 9 FXML bista (`txata.fxml` barne) + `app.css`. |
| **4** | Aplikazioa erakustean | Gerente aplikazioan login-a egin daiteke zerbitzariko APIaren kontra | **Beteta (% 100)** | `SaioHasieraKontrolagailua` → `ApiBezeroa.login()` → `POST /api/v1/langileak/login` (BCrypt + `isManager()` rol-egiaztapena + `/health` semaforoa 10s-ro). |
| **5** | Aplikazioa erakustean | Gerente aplikazioak erabiltzaileak kudeatzen ditu, txata aktibatuz eta desaktibatuz, eta txata funtzionatzen du | **Beteta (% 100)** | DBan `langileak.txata_aktibo` + `txat_mezuak` taula; APIan `PUT /langileak/{id}/txata`, `PUT /langileak/{id}/aktibo` eta `TxataController` (`/api/v1/txata/*`, `403` blokeoarekin + Socket.IO zubia); Gerentean `Langileak` (Txata zutabea + aktibatu/desaktibatu botoia) + `Barne Txata` pantaila (`txata.fxml`); TPV-an `TxataForm` funtzionala! |
| **6** | Aplikazioa erakustean | Aktibatutako erabiltzaileek bakarrik egin dezakete login-a TPV-an | **Beteta (% 100)** | Backend-ean `LangileakController.SaioaHasi()`-k `!l.Ezabatua` eta `if (!langilea.Aktibo) return Unauthorized(...)` egiaztatzen ditu; TPV-an `LoginForm.cs`-ek `l.Aktibo` iragazten du eta Gerentean `Aktibatu / Desaktibatu (TPV)` botoiarekin zuzenean frogatu daiteke. |
| **7** | Aplikazioa erakustean | Stocka kudeatzen du (stocka handitu, salmenta-prezioa eta erositako kopurua aldatu) | **Beteta (% 100)** | `OsagaiakKontrolagailua`-n stocka handitu (`POST /lehengaiak/sarrera`), gutxieneko stocka eta erosketa-kostua aldatu (`PUT /lehengaiak/{id}`); `MenuaKontrolagailua`-n salmenta-prezioa (`PUT /platerak/{id}`) eta errezeta aldatu. |
| **8** | Kodean | MVC patroia: bistak kontrollerrekin lotuta, API-aren dei egokiak | **Beteta (% 100)** | Gerentean MVC zorrotza (`fxml` ↔ `BaseAdminKontrolagailua` + `KontrolagailuFabrika` ↔ `model` + `ApiKontratua`/`ApiBezeroa`). TPV-an geruzatutako WinForms banaketa (`*.Designer.cs` ↔ `BaseTpvForm` azpiklaseak ↔ `Models` + `IApiService`/`ApiService`). |
| **9** | Kodean | Java eta C# aplikazioak objektuei bideratuta daude (herentzia, interfazeak...) | **Beteta (% 100)** | **Java:** `ApiKontratua` (`interface`), `PantailaFreskagarria` (`interface`), `BaseAdminKontrolagailua` (`abstract`, 7 kontroladorek hedatua), `ApiSalbuespena extends IOException`, `JatetxeaKudeaketaAplikazioa extends Application`, `KontrolagailuFabrika implements Callback`, `CheckedSupplier<T>`. **C#:** `IApiService` (`interface`), `IDpiEgokigarria` (`interface`), `BaseTpvForm : Form` (8 pantailek heredatua), `ApiResponse<T>`, `sealed class/record`. |
| **10** | Dokumentuan | Nielsen: 10 heuristikatik 5 aplikatu eta justifikatuta daude | **Beteta (% 100)** | H1, H2, H3, H4 eta H5 heuristikoak aplikatuta daude bi aplikazioetan eta dokumentu honetan kode-ebidentziekin justifikatu dira. |
| **11** | Dokumentuan | Gestalt: 4 arau aplikatu eta justifikatuta daude | **Beteta (% 100)** | G1 (Hurbiltasuna), G2 (Antzekotasuna), G3 (Eskualde komuna) eta Irudia/Funtsa (+ G6 Jarraitutasuna) justifikatu dira kode-ebidentziekin. |

---

## 1. Diseinu Sistema (PDF, Dokumentua eta Aplikazioen Koherentzia)

> **Irizpidea (Dokumentuan):** *Diseinu sistema PDF-an eta dokumentuan dago, eta aplikazio guztiekin bat dator.*

### 1.1. Non dago dokumentatuta?

Proiektuko diseinu-sistema ofiziala honako fitxategietan jasota dago:
- **PDF ofiziala:** [T1_Jatetxea_Diseinu_Sistema_eta_Koloreak.pdf](../../01_aplikazioak/admin/komunak/desktop_javafx_core/UI_UX_diseinua_koloreak_figma/T1_Jatetxea_Diseinu_Sistema_eta_Koloreak.pdf) (eta `.docx` iturburua).
- **Markdown dokumentazioa:** [01_DISEINU_SISTEMA_ETA_KOLOREAK.md](../../01_aplikazioak/admin/komunak/desktop_javafx_core/UI_UX_diseinua_koloreak_figma/01_DISEINU_SISTEMA_ETA_KOLOREAK.md), [02_FIGMA_EGITURA_ETA_KOMPONENTEEKIN_GIDA.md](../../01_aplikazioak/admin/komunak/desktop_javafx_core/UI_UX_diseinua_koloreak_figma/02_FIGMA_EGITURA_ETA_KOMPONENTEEKIN_GIDA.md) eta [04_LOGOA_ETA_IKUS_IDENTITATEA.md](../../01_aplikazioak/admin/komunak/desktop_javafx_core/UI_UX_diseinua_koloreak_figma/04_LOGOA_ETA_IKUS_IDENTITATEA.md).
- **TPV Figma Estilo Gida (v2.0 *Warm Hospitality*):** [2_tpv_diseinua.md](../../01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp/docs/2_tpv_diseinua.md) (`img/00_estilo_gida.png` barne).

### 1.2. Estilo Gidaren 7 atalen betetzea (`INTERFAZEEN_GARAPENA_TEORIA_LABURPENA.pdf` — 4. Gaia)

1. **Kolore-paleta (60–30–10 araua eta WCAG 2.2 AA/AAA):**
   - **% 10 Marka / CTA (Chianti & Ardo Gorria):** `#881337` (hover `#9F1239`, iluna `#4C0519`) — aplikazio guztietan berdin-berdina.
   - **% 60 Atzeko plano neutroa:** `#F8FAFC` (*Irina Txuria*, Gerentea JavaFX) / `#FAF7F2` (*Warm Linen*, TPV C# v2).
   - **% 30 Azalerak / Txartelak:** `#FFFFFF` (*Marmol Zuria*) eta ertz leunak (`#E2E8F0` / `#E6DFD5`).
   - **Egoera semantikoak:** Arrakasta/Libre `#15803D` (`#DCFCE7` hondoa), Abisua/Sukaldean `#C2410C` (`#FEF3C7`/`#FFEDD5`), Alerta/Ezabatu `#DC2626`/`#B91C1C` (`#FEE2E2`), Info/Garbitzen `#0284C7`/`#0369A1` (`#E0F2FE`).
2. **Tipografia:** Gerentean `Segoe UI` (11px–30px eskala `app.css`-n); TPV-an `Cinzel` (marka/goiburuak), `Segoe UI` / `Plus Jakarta Sans` (gorputza) eta `JetBrains Mono` (prezioak, guztizkoak, 80mm tiketak) `Assets/Fonts/` karpetatik kargatuta.
3. **Diseinu-egiturak gailuka (Responsive):** Gerentean `BorderPane` + `VBox`/`HBox` + `StackPane` (`setMinWidth(1000)`, `setMaximized(true)`); TPV-an `Maximized` + `MinimumSize(800, 500)` + `FlowLayoutPanel` + PerMonitorV2 DPI eskalatzea + breakpoint-ak (`< 1040px`, `< 960px`, `< 860px`).
4. **Elementu interaktiboak:** Botoi-hierarkia koherentea (`primary-button`, `secondary-button`, `danger-button`, `nav-button`) egoera guztiekin (`normal`, `hover`, `pressed`, `disabled`).
5. **Ikonografia eta Monograma:** `T1` monograma bateratua (Gerentean `.brand-mark` / `.brand-mark-small`; TPV-an `Theme.DrawT1Logo()` GDI+ vektore bidez, Android `ic_t1_logo.xml`-rekin lerrokatuta).
6. **Irudien estiloa:** TPV-an `Assets/Images/Platerak/` eta `Kategoriak/` argazkiak `ProductImageCache.DrawImageCover()` (`object-fit: cover`) bidez deformaziorik gabe.
7. **Idazketa-estiloa:** Euskara bateratua, argia eta jatetxeko terminologia naturalarekin (`Komanda`, `Mahaikideak`, `Osagaiak eta inbentarioa`, `Stock-sarrera`, `Zerbitzua itxita`).

### 1.3. Aplikazioen arteko bat-etortze egiaztapena eta oharra

| Elementua | Diseinu Sistema (PDF v1) | Gerentea (`app.css`) | TPV C# (`Theme.cs`) | Bat dator? |
|---|---|---|---|---|
| Marka nagusia | `#881337` / `#9F1239` | `#881337` / `#9F1239` | `#881337` / `#9F1239` | **Bai (% 100)** |
| Arrakasta / Libre | `#15803D` / `#DCFCE7` | `#166534` / `#DCFCE7` | `#15803D` / `#DCFCE7` | **Bai** |
| Abisua / Sukaldean | `#C2410C` / `#FEF3C7` | `#C2410C` / `#FEF3C7` | `#C2410C` / `#FFEDD5` | **Bai** |
| Arriskua / Errorea | `#DC2626` / `#FEE2E2` | `#B91C1C` / `#FEE2E2` | `#B91C1C` / `#FEE2E2` | **Bai** |
| Txartelak (Surface) | `#FFFFFF` | `#FFFFFF` | `#FFFFFF` | **Bai (% 100)** |
| Hondoa eta Testua | `#F8FAFC` / `#1E293B` | `#F8FAFC` / `#1E293B` | `#FAF7F2` / `#1C1917` *(v2 beroa)* | **Ñabardura (azaldu edo bateratu)** |

> **Gomendioa entregatu aurretik:** PDF ofizialean (`T1_Jatetxea_Diseinu_Sistema_eta_Koloreak.pdf`) gehitu taula txiki bat **v1.0 (Back-office Slate: Gerentea/KDS)** eta **v2.0 (*Warm Hospitality*: TPV/Android)** tonu neutroen bateragarritasuna justifikatuz, marka-nukleo (`#881337`) eta kolore semantiko berak partekatzen dituztelako.

---

## 2. Klase Diagrama (DrawIO, TPV eta Gerente)

> **Irizpidea (Dokumentuan):** *Klase diagrama (DrawIO, TPV eta Gerente) PDF-an eta dokumentuan dago, eta aplikazioarekin bat dator.*

### 2.1. Egoera Eguneratua (DrawIO 3 Orri + Markdown)

- [klase_diagrama.drawio](../../04_dokumentazioa/garapen_diagramak/klase_diagrama.drawio) fitxategia **3 orri** osorekin eguneratu da:
  1. **`1T Jatetxea - Klase Diagrama Osoa`:** Domeinu-entitateak eta REST API zerbitzaria.
  2. **`Gerente Aplikazioa (JavaFX MVC)`:** `ApiKontratua` (`interface`), `ApiBezeroa`, `ApiSalbuespena`, `KontrolagailuFabrika`, `EkintzaAsinkronoak`, `PantailaFreskagarria` (`interface`), `BaseAdminKontrolagailua` (`abstract`), 8 kontroladoreak (`TxataKontrolagailua` barne) eta 11 modelo klaseak.
  3. **`TPV Aplikazioa (C# WinForms)`:** `IApiService` (`interface`), `ApiService`, `IDpiEgokigarria` (`interface`), `BaseTpvForm : Form`, 8 pantailak (`TxataForm` barne), 3 modalak, UI motorra (`Theme`, `UiPerf`, `ProductImageCache`) eta DTO guztiak.
- [klase_diagrama.md](../../04_dokumentazioa/garapen_diagramak/klase_diagrama.md) eta [klase_diagrama.drawio.pdf](../../04_dokumentazioa/garapen_diagramak/klase_diagrama.drawio.pdf) fitxategiak aplikazioekin lerrokatuta daude.

### 2.2. Gerente Aplikazioaren (`admin_desktop_java`) Klase Diagrama Erreala

```mermaid
classDiagram
    direction TB

    class Application {
        <<JavaFX>>
        +start(primaryStage: Stage) void
    }
    class Callback~P,R~ {
        <<interface>>
        +call(param: P) R
    }
    class IOException {
        <<Java>>
    }

    class Abiarazlea {
        +main(args: String[]) void
    }
    class JatetxeaKudeaketaAplikazioa {
        -apiClient: ApiBezeroa
        -stage: Stage
        +start(primaryStage: Stage) void
        +showLogin() void
        +showWorkspace(session: KudeatzaileSaioa) void
        -loadScene(resource, session, w, h) void
    }
    class KontrolagailuFabrika {
        -apiClient: ApiBezeroa
        -stage: Stage
        -session: KudeatzaileSaioa
        +call(controllerType: Class~?~) Object
    }
    class KudeatzaileSaioa {
        -user: KudeatzaileErabiltzailea
        -configurationWritable: BooleanProperty
        -serviceStatusText: StringProperty
        +getUser() KudeatzaileErabiltzailea
        +isConfigurationWritable() boolean
        +setServiceState(writable, message) void
    }
    class ApiBezeroa {
        -baseUrl: String
        -httpClient: HttpClient
        -gson: Gson
        +fromEnvironment() ApiBezeroa
        +checkHealth() ApiHealth
        +login(username, password) KudeatzaileErabiltzailea
        +getList~T~(path, itemType) List~T~
        +getData~T~(path, dataType) T
        +post(path, body) void
        +put(path, body) void
        +delete(path) void
        +getServiceStatus() ServiceStatus
        +getFakturak(mota) List~Faktura~
        +getFinantzaLaburpena() FinantzaLaburpena
    }
    class ApiSalbuespena {
        -statusCode: int
        +getStatusCode() int
    }
    class EkintzaAsinkronoak {
        -EXECUTOR: ExecutorService
        +run~T~(action: CheckedSupplier~T~, success, failure) void
    }
    class CheckedSupplier~T~ {
        <<interface>>
        +get() T
    }
    class PantailaLaguntza {
        +parseNonNegative(value, fieldName) double
        +parsePositive(value, fieldName) double
        +confirm(title, message) boolean
    }

    class SaioHasieraKontrolagailua {
        -apiClient: ApiBezeroa
        -application: JatetxeaKudeaketaAplikazioa
        -healthCheckTimeline: Timeline
        +refreshApiStatus() void
        +signIn() void
    }
    class DiseinuNagusiaKontrolagailua {
        -contentHost: StackPane
        +showDashboard() void
        +showStaff() void
        +showMenu() void
        +showIngredients() void
        +showTables() void
        +showInvoices() void
        +refreshServiceStatus() void
        +signOut() void
    }
    class LaburpenKontrolagailua {
        +refresh() void
    }
    class LangileakKontrolagailua {
        -employees: ObservableList~Langilea~
        +refresh() void
        +saveStaff() void
        +editStaff() void
        +cancelEdit() void
        +deactivateStaff() void
    }
    class MenuaKontrolagailua {
        -menuItems: ObservableList~Platera~
        -recipeComponents: ObservableList~RecipeComponent~
        +refresh() void
        +addRecipeIngredient() void
        +removeRecipeIngredient() void
        +saveMenuItem() void
        +editMenuItem() void
        +deactivateMenuItem() void
    }
    class OsagaiakKontrolagailua {
        -ingredients: ObservableList~Lehengaia~
        -stockAlerts: ObservableList~String~
        +refresh() void
        +saveIngredient() void
        +editIngredient() void
        +registerStockEntry() void
        +deactivateIngredient() void
    }
    class MahaiakKontrolagailua {
        -tables: ObservableList~Mahaia~
        +refresh() void
        +saveTable() void
        +editTable() void
        +deactivateTable() void
    }
    class FakturakKontrolagailua {
        -masterList: ObservableList~Faktura~
        -filteredList: FilteredList~Faktura~
        +refresh() void
        +saveInvoice() void
        +markAsPaid() void
        +markAsCancelled() void
    }

    class KudeatzaileErabiltzailea {
        +id: int
        +erabiltzailea: String
        +izena: String
        +rola: String
        +aktibo: boolean
        +isManager() boolean
        +displayName() String
    }
    class Langilea
    class Platera
    class Lehengaia
    class LehengaiaAlerta
    class PlaterEskandalloa
    class Mahaia
    class Faktura
    class FinantzaLaburpena

    Application <|-- JatetxeaKudeaketaAplikazioa
    IOException <|-- ApiSalbuespena
    Callback <|.. KontrolagailuFabrika
    Abiarazlea ..> JatetxeaKudeaketaAplikazioa
    JatetxeaKudeaketaAplikazioa --> ApiBezeroa
    JatetxeaKudeaketaAplikazioa ..> KontrolagailuFabrika
    JatetxeaKudeaketaAplikazioa ..> KudeatzaileSaioa
    KudeatzaileSaioa --> KudeatzaileErabiltzailea
    ApiBezeroa ..> ApiSalbuespena
    EkintzaAsinkronoak ..> CheckedSupplier
    KontrolagailuFabrika ..> SaioHasieraKontrolagailua
    KontrolagailuFabrika ..> DiseinuNagusiaKontrolagailua
    KontrolagailuFabrika ..> LaburpenKontrolagailua
    KontrolagailuFabrika ..> LangileakKontrolagailua
    KontrolagailuFabrika ..> MenuaKontrolagailua
    KontrolagailuFabrika ..> OsagaiakKontrolagailua
    KontrolagailuFabrika ..> MahaiakKontrolagailua
    KontrolagailuFabrika ..> FakturakKontrolagailua
    LangileakKontrolagailua --> Langilea
    MenuaKontrolagailua --> Platera
    MenuaKontrolagailua --> PlaterEskandalloa
    OsagaiakKontrolagailua --> Lehengaia
    OsagaiakKontrolagailua --> LehengaiaAlerta
    MahaiakKontrolagailua --> Mahaia
    FakturakKontrolagailua --> Faktura
    FakturakKontrolagailua --> FinantzaLaburpena
    OsagaiakKontrolagailua ..> PantailaLaguntza
    MenuaKontrolagailua ..> PantailaLaguntza
    LangileakKontrolagailua ..> PantailaLaguntza
```

### 2.3. TPV Aplikazioaren (`desktop_tpv_csharp`) Klase Diagrama Erreala

```mermaid
classDiagram
    direction TB

    class Form {
        <<WinForms>>
    }

    class Program {
        +Main() void
    }
    class ApiService {
        -_http: HttpClient
        -_baseUrl: string
        +CheckHealthAsync() Task
        +GetLangileakAsync() Task~List~LangileaDto~~
        +LoginAsync(erab, pin) Task
        +GetMahaiakAsync() Task~List~MahaiaDto~~
        +AldatuMahaiarenEgoeraAsync(id, egoera) Task
        +GetPlaterakAsync(kategoria) Task~List~PlateraDto~~
        +GetPlaterenStockAhalmenaAsync() Task~Dictionary~
        +SortuEskaeraAsync(mahaiaId, langileaId, pax) Task
        +GehituPlateraEskaeraraAsync(eskaeraId, plateraId, kop, oharra) Task
        +EzabatuEskaeraLerroaAsync(lerroId) Task
        +BidaliSukalderaAsync(eskaeraId) Task
        +ProzesatuOrdainketaAsync(dto) Task
        +GetErreserbakAsync(data, txanda) Task
        +SortuErreserbaAsync(dto) Task
        +EzeztatuErreserbaAsync(id) Task
    }
    class Theme {
        <<static>>
        +ChiantiPrimary: Color
        +WarmLinen: Color
        +GoldBorder: Color
        +DisplayFont(c, pt) Font
        +BodyFont(c, pt, bold) Font
        +MonoFont(c, pt, bold) Font
        +AttachMainFormDpiAdaptation(form, onRefresh) void
        +DrawT1Logo(g, bounds) void
        +DrawRoundedCard(g, bounds, r, fill, border) void
        +StyleButton(btn, bg, fg, border, r) void
        +ApplyWarmHospitalityTheme(form) void
        +GetEgoeraKoloreak(egoera) Tuple
    }
    class UiPerf {
        <<static>>
        +EnableDoubleBuffering(root: Control) void
        +RunFlickerFree(container, action, preserveScroll) void
    }
    class ProductImageCache {
        <<static>>
        -Cache: ConcurrentDictionary
        +GetImage(platera: PlateraDto) Bitmap
        +DrawImageCover(g, img, target) void
        +ToSlug(text) string
    }

    class LoginForm {
        -_api: ApiService
        -_baimendutakoLangileak: List~LangileaDto~
        -_pin: string
        +SaiatuKonektatzenAsync(eskuz) Task
        +LoginExekutatuAsync() Task
    }
    class MahaiakForm {
        -_txartelakById: Dictionary~int, MahaiTxartelUi~
        -_tenporizadorea: Timer
        +KargatuMahaiakAsync(isSilent, behartu) Task
        +BistaratuMahaiak(behartuBerreraiki) void
    }
    class MahaiaXehetasunakForm {
        -_eskaerak: List~EskaeraDto~
        -_autoRefreshTimer: Timer
        -_pulseTimer: Timer
        +SukalderaBidaliKlikatuaAsync() Task
        +KobratuEtaItxiAsync() Task
    }
    class EskaeraForm {
        -_platerak: List~PlateraDto~
        -_stockMap: Dictionary~int, int~
        -_karritoa: List~CartItem~
        +EskaeraBidaliAsync() Task
    }
    class ErreserbakForm {
        +KargatuErreserbakAsync() Task
    }
    class OrdainketaForm {
        +OrdainduAsync() Task
    }
    class TiketIkusteaForm
    class MahaikideakModal
    class OharraModal
    class ErreserbaBerriaModal

    class ApiResponse~T~
    class LangileaDto
    class MahaiaDto
    class PlateraDto
    class EskaeraDto
    class EskaeraLerroaDto
    class CartItem
    class OrdainketaEskaeraDto
    class OrdainketaErantzunaDto
    class ErreserbaDto

    Form <|-- LoginForm
    Form <|-- MahaiakForm
    Form <|-- MahaiaXehetasunakForm
    Form <|-- EskaeraForm
    Form <|-- ErreserbakForm
    Form <|-- OrdainketaForm
    Form <|-- TiketIkusteaForm
    Form <|-- MahaikideakModal
    Form <|-- OharraModal
    Form <|-- ErreserbaBerriaModal

    Program ..> LoginForm
    LoginForm --> ApiService
    LoginForm --> MahaiakForm
    MahaiakForm --> MahaiaXehetasunakForm
    MahaiakForm --> EskaeraForm
    MahaiakForm --> ErreserbakForm
    MahaiakForm ..> MahaikideakModal
    EskaeraForm ..> OharraModal
    EskaeraForm ..> ProductImageCache
    MahaiaXehetasunakForm --> OrdainketaForm
    OrdainketaForm --> TiketIkusteaForm
    ErreserbakForm ..> ErreserbaBerriaModal
    ApiService ..> ApiResponse
    ApiService ..> LangileaDto
    ApiService ..> MahaiaDto
    ApiService ..> PlateraDto
    ApiService ..> EskaeraDto
    ApiService ..> OrdainketaEskaeraDto
    ApiService ..> ErreserbaDto
    EskaeraDto "1" *-- "0..*" EskaeraLerroaDto
    EskaeraForm "1" *-- "0..*" CartItem
    LoginForm ..> Theme
    MahaiakForm ..> Theme
    MahaiakForm ..> UiPerf
```

> **Ekintza beharrezkoa:** Sartu goiko bi diagrama-bloke hauek (edo bere orriak) `04_dokumentazioa/garapen_diagramak/klase_diagrama.drawio` fitxategian eta esportatu berriro `klase_diagrama.drawio.pdf` gisa, ebaluazioko 2. puntua % 100 betetzeko.

---

## 3. Mahaigaineko Aplikazioa JavaFX-ekin Garatua

> **Irizpidea (Aplikazioa erakustean):** *Mahaigaineko aplikazioa JavaFX-ekin garatua.*

Gerente aplikazioa (`01_aplikazioak/admin/admin_desktop_java/admin_desktop_java`) **Java 21 + JavaFX 21.0.6** teknologiarekin dago garatua:

- **Menpekotasunak (`pom.xml`):**
  - `org.openjfx:javafx-controls:21.0.6` eta `org.openjfx:javafx-fxml:21.0.6`
  - `io.github.mkpaz:atlantafx-base:2.0.1` (`PrimerLight` oinarri-tema modernoa + `app.css` pertsonalizatua)
  - `com.google.code.gson:2.11.0` (JSON serializazioa/deserializazioa)
  - `org.junit.jupiter:junit-jupiter:5.12.1` (3 test klase: `ManagerUserTest`, `ApiClientHealthTest`, `ManagerScreensFxmlTest`)
- **Modulu-sistema (`module-info.java`):** `javafx.controls`, `javafx.fxml`, `java.net.http`, `com.google.gson`, `atlantafx.base` eskatzen ditu eta `controller`/`model` paketeak irekitzen ditu (`opens ... to javafx.fxml`).
- **Stage → Scene → Pane arkitektura (`JatetxeaKudeaketaAplikazioa.java`):**
  - `Abiarazlea.java`-k abiarazten du `Application.launch(JatetxeaKudeaketaAplikazioa.class, args)`.
  - Leihoaren konfigurazio egokigarria: `stage.setMinWidth(1000)`, `stage.setMinHeight(680)` eta `Platform.runLater(() -> stage.setMaximized(true))`.
  - **8 FXML bista (`src/main/resources/.../fxml/`):** `saio-hasiera.fxml` (`StackPane` + `HBox`), `diseinu-nagusia.fxml` (`BorderPane`: ezkerrean alboko menua `VBox`, erdian `VBox` + goiko barra `HBox` + `StackPane contentHost`), `laburpena.fxml`, `langileak.fxml`, `menua.fxml`, `osagaiak.fxml`, `mahaiak.fxml`, `fakturak.fxml`.
- **Abiarazteko:** `hasi_kudeaketa_mahaigaina.bat` edo `mvnw javafx:run`.

---

## 4. Gerente Aplikazioan Login-a Zerbitzariko APIaren Kontra

> **Irizpidea (Aplikazioa erakustean):** *Gerente aplikazioan login-a egin daiteke zerbitzariko APIaren kontra.*

Autentifikazio-fluxu osoa **REST APIaren kontra** egiten da (inoiz ez zuzenean MySQL datu-basera):

1. **Konexio-osasunaren egiaztapena (`SaioHasieraKontrolagailua.java:47-86`):**
   - Pantaila irekitzean eta **10 segundoro** (`Timeline`), `ApiBezeroa.checkHealth()` deitzen du asinkronoki (`GET http://localhost:5050/health`).
   - Semaforo bisuala eguneratzen du: berdea (`API martxan · DB konektatuta`), laranja (API bai baina DB deskonektatuta) edo gorria (API lineaz kanpo).
2. **Saio-hasiera (`SaioHasieraKontrolagailua.java:89-117` → `ApiBezeroa.java:94-109`):**
   - `erabiltzailea` eta `PIN/pasahitza` hutsik ez daudela egiaztatzen du.
   - `EkintzaAsinkronoak.run(...)` bidez `POST /api/v1/langileak/login` eskaera bidaltzen du JSON gorputzarekin (`{"erabiltzailea": "...", "pasahitza": "..."}`).
   - Goiburu identifikagarriak bidaltzen ditu (`X-App-Id: kudeaketa_desktop_java`, `X-App-Name: T1 Jatetxea Kudeaketa Desktop`).
3. **Backend balidazioa (`LangileakController.cs:86-161`):**
   - `!l.Ezabatua` eta `l.Aktibo` egiaztatzen ditu.
   - Pasahitza/PINa **BCrypt** hash-aren kontra egiaztatzen du (`SegurtasunHelper.EgiaztatuPin`), eta JWT Bearer tokena + `LangileaDTO` itzultzen ditu.
4. **Kudeatzaile-baimenaren egiaztapena (`KudeatzaileErabiltzailea.java:13-19`):**
   - `user.isManager()` metodoak egiaztatzen du rola `Admin`, `AdminIT`, `Kudeatzailea` edo `Jabea` dela. Zerbitzari batek Gerente aplikazioan sartzen saiatuz gero, blokeatu egiten du (*"Kudeatzaile edo administratzaile baimena behar da"*).
5. **Lan-espaziora jauzia:** `application.showWorkspace(new KudeatzaileSaioa(user))` deitzen du eta `diseinu-nagusia.fxml` kargatzen du.

---

## 5. Erabiltzaileen Kudeaketa eta Txata (Aktibatzea, Desaktibatzea eta Funtzionamendua)

> **Irizpidea (Aplikazioa erakustean):** *Gerente aplikazioak erabiltzaileak kudeatzen ditu, txata aktibatuz eta desaktibatuz, eta txata funtzionatzen du.*

### 5.1. Inplementazio Osoa (DB + REST API + Gerente JavaFX + TPV C# + Socket.IO)

1. **Datu-basea (`db/01_egitura_schema.sql` eta `jatetxea_mysql`):**
   - `langileak.txata_aktibo TINYINT(1) NOT NULL DEFAULT 1` zutabea gehituta.
   - `txat_mezuak` taula (`id`, `langilea_id` FK, `gela`, `testua`, `sortze_data`) gehituta.
2. **REST APIa (`LangileakController.cs` + `TxataController.cs` + `TxatMezuaMap.cs`):**
   - `PUT /api/v1/langileak/{id}/txata` → langile baten barne-txata aktibatzen edo desaktibatzen du (`LockMode.Upgrade` transakzioan).
   - `PUT /api/v1/langileak/{id}/aktibo` → langilea aktibatzen edo desaktibatzen du TPV/sistemarako sarbidea kontrolatzeko.
   - `GET /api/v1/txata/egoera/{langileaId}` → langileak txata baimenduta daukan itzultzen du (`aktibo && txataAktibo`).
   - `GET /api/v1/txata/mezuak?gela=...&langileaId=...` eta `POST /api/v1/txata/mezuak` → zerbitzari mailan bermatzen dute `txata_aktibo == false` bada **HTTP 403 Forbidden** itzultzea (*"Txata desaktibatuta daukazu"*), eta mezu berriak `jatetxea-chat` (`:3001`, Socket.IO) zerbitzarira zubitzen dituzte.
3. **Gerente JavaFX (`LangileakKontrolagailua.java` + `langileak.fxml` + `TxataKontrolagailua.java` + `txata.fxml`):**
   - **Langileak pantailan:** `Txata` zutabea (`Aktibatuta` / `Desaktibatuta`), **`Txata aktibatu / desaktibatu`** botoia (`toggleStaffChat`), **`Aktibatu / Desaktibatu (TPV)`** botoia (`toggleStaffActive`), eta CRUD osoa (sortu, editatu, desgaitu).
   - **Barne Txata pantailan (`diseinu-nagusia.fxml` alboko menuan):** Gela aukeraketa (`Orokorra`, `Jangela`, `Sukaldea`, `Barra`), 2.5 segundoko auto-sinkronizazioa (`Timeline`), eta mezuen bidalketa/irakurketa.
4. **TPV C# (`MahaiakForm.cs` + `TxataForm.cs` + `ApiService.cs`):**
   - Goiko barran **`Txata`** botoi berria (`_btnTxata`) `TxataForm` leihoa irekitzeko.
   - 2 segundoro egiaztatzen du `GetTxatEgoeraAsync()`; Gerenteak txata desaktibatzen badio, berehala **blokeatzen dira idazketa-eremua eta `Bidali` botoia** eta alerta gorria erakusten du!

### 5.2. Erakustaldiko Demo Fluxua (Irakaslearen aurrean)

1. Gerentean (`Langileak`), hautatu `mikel_zerb` → ziurtatu **Txata = Aktibatuta**.
2. Gerentean (`Barne Txata` → `Orokorra`), idatzi mezua → TPV-an (`mikel_zerb` → `Txata` botoia) **2 segundotan agertzen da**, eta Mikelek erantzuten du → Gerentean agertzen da!
3. Gerentean (`Langileak`), sakatu **`Txata desaktibatu`** `mikel_zerb`-entzat → TPV-ko `TxataForm`-en berehala **Txata DESAKTIBATUTA** agertzen da eta ezin du mezurik bidali (`403`).
4. Gerentean sakatu **`Desaktibatu (TPV)`** → Mikelek TPV-an saioa ixtean, desagertu egiten da avatarretatik eta APIak `401` ematen dio login saiakeran! Ondoren berriro **`Aktibatu (TPV)`** sakatuz gero, berriz sar daiteke.

---

## 6. Aktibatutako Erabiltzaileek Bakarrik Egin Dezakete Login-a TPV-an

> **Irizpidea (Aplikazioa erakustean):** *Aktibatutako erabiltzaileek bakarrik egin dezakete login-a TPV-an.*

### 6.1. Nola funtzionatzen duen elkarrekintzak (Gerentea ↔ REST API ↔ TPV C#)

1. **Gerentean desgaitzea (`LangileakKontrolagailua.java:187-216`):**
   - Kudeatzaileak langile bat hautatu eta *"Hautatutako langilea desgaitu"* sakatzean, `DELETE /api/v1/langileak/{id}` deitzen da.
   - Backend-eko `LangileakController.Desaktibatu()`-k (`L299-319`) transakzio barruan `langilea.Aktibo = false`, `langilea.Ezabatua = true` eta `EzabatzeData = DateTime.Now` gordetzen ditu.
2. **Backend-eko blokeo zorrotza (`LangileakController.cs:101-108`):**
   ```csharp
   var langilea = await session.Query<Langilea>()
       .FirstOrDefaultAsync(l => !l.Ezabatua && (l.Erabiltzailea == erab || l.Id == posibleId));
   if (langilea == null)
       return Unauthorized(new { status = "error", message = "Erabiltzailea edo PINa ez da zuzena." });
   if (!langilea.Aktibo)
       return Unauthorized(new { status = "error", message = "Langile hau desgaituta dago." });
   ```
   Horrela, langile desgaitu batek APIari zuzenean `POST /api/v1/langileak/login` bidalita ere, **HTTP 401 Unauthorized** jasotzen du beti.
3. **TPV C# Login pantailan (`LoginForm.cs:508-540, 830-883, 944-1009`):**
   - `GET /api/v1/langileak` deitzean, backend-ak `!l.Ezabatua` dutenak soilik itzultzen ditu; beraz, Gerentean desgaitutako langilea **automatikoki desagertzen da TPV-ko avatar-zerrendatik** freskatzean (`F5` edo *Freskatu*).
   - Gainera, `DaBaimendua()` metodoak TPV-rako baimendutako rolak bakarrik erakusten ditu (`Zerbitzaria`, `Barrakoa`, `AdminIT`, `Admin`, `Kudeatzailea`).
   - Saioa hastean `ApiService.LoginAsync()`-ek APIaren erantzuna egiaztatzen du; `401` bada, TPV-ak sarbidea ukatu eta errore-mezua erakusten du.

> **Hobekuntza txiki gomendatua (`LoginForm.cs:533`):** `aktibo = 0` eta `ezabatua = 0` egoera bereizi bat erabiliz gero (adib. berraktibatzeko toggle-a gehitzean), `BistaratuLangileak()` metodoan `.Where(l => l.Aktibo && DaBaimendua(l))` jartzea gomendatzen da, aktibatuta ez daudenak avatarretan ere ager ez daitezen.

---

## 7. Stockaren Kudeaketa (Stocka Handitu, Salmenta-Prezioa eta Erositako Kopurua Aldatu)

> **Irizpidea (Aplikazioa erakustean):** *Stocka kudeatzen du (stocka handitu, salmenta-prezioa eta erositako kopurua aldatu).*

Gerente aplikazioan (`admin_desktop_java`) stockaren eta prezioen kudeaketa bi pantaila koordinatutan egiten da (eta TPV-ak kontsumo automatikoa egiten du errezeten bidez):

### 7.1. Lehengaien Stocka eta Erosketa-Sarrerak (`OsagaiakKontrolagailua.java` + `osagaiak.fxml`)

| Ekintza | UI Kontrola (`osagaiak.fxml`) | Metodoa & API Deia | Zer gertatzen da DBan |
|---|---|---|---|
| **Inbentarioa eta Alertak ikusi** | `ingredientsTable` + `alertList` | `refresh()` → `GET /lehengaiak` + `GET /lehengaiak/alertak` | Uneko stocka, gutxienekoa, unitatea, hornitzailea eta *"Stock baxua"* alerta (erosi beharreko kopuru proposatuarekin: `v_erosi_beharreko_lehengaiak`). |
| **Stocka handitu (erosketa / albarana)** | `entryQuantityField` + `entryNoteField` + `stockEntryButton` | `registerStockEntry()` → `POST /api/v1/lehengaiak/sarrera` | Transakzio pesimistan (`LockMode.Upgrade`), `lehengaiak.uneko_stocka += kantitatea` eguneratzen da eta `stock_mugimenduak` taulan `Sarrera_Albarana` erregistratzen da. |
| **Osagaia editatu (gutxieneko stocka eta erosketa-kostua)** | `editButton` → `minimumStockField`, `costField`, `supplierField` | `editIngredient()` + `saveIngredient()` → `PUT /api/v1/lehengaiak/{id}` | Gutxieneko stocka, **unitateko erosketa-kostua (€)** eta hornitzailea aldatzen dira, stock-historia hautsi gabe. |
| **Osagai berria sortu** | `nameField`, `unitCombo`, `stockField`, `minimumStockField`, `costField` | `saveIngredient()` → `POST /api/v1/lehengaiak` | Osagai berria eta hasierako stock-mugimendua batera sortzen ditu. |
| **Osagaia desgaitu** | `deactivateButton` | `deactivateIngredient()` → `DELETE /api/v1/lehengaiak/{id}` | Errezeta aktiboetan erabiltzen ez bada, soft delete egiten du berrespenarekin. |

### 7.2. Salmenta-Prezioa eta Errezetako Kopuruak (`MenuaKontrolagailua.java` + `menua.fxml`)

- **Salmenta-prezioa aldatu:** Taulatik platera hautatzean (`loadMenuItem()`), `GET /api/v1/platerak/{id}/eskandalloa` kargatzen da; `priceField` eremuan **salmenta-prezioa (€)** aldatu eta `PUT /api/v1/platerak/{id}` bidez gordetzen da (`PantailaLaguntza.parseNonNegative()` balidazioarekin).
- **Errezetako osagai-kantitatea aldatu:** `ingredientCombo` + `recipeQuantityField` bidez plater bakoitzak kontsumitzen duen lehengai-kopurua gehitu, aldatu edo kendu daiteke (`RecipeComponent`).
- **Segurtasun-araua:** Zerbitzua irekita badago (`openTables > 0`), `KudeatzaileSaioa.configurationWritableProperty()`-k eta `verifyServiceClosed()`-ek menuko aldaketak blokeatzen dituzte kontuak ez nahasteko.

> **Erakustaldirako oharra (*"erositako kopurua aldatu"*):** Erakustaldian argi azaldu `Osagaiak` pantailan **Stock-sarrera** formularioarekin erositako kopurua (`entryQuantityField`) eta oharra/albarana sartuta stocka handitzen dela (eta behar bada `Fakturak` atalean hornitzaile-faktura erregistratzen dela). Era berean, `Menua` pantailan plateraren **salmenta-prezioa** aldatzen dela erakutsi (aurrez ziurtatu mahai guztiak itxita/libre daudela `Zerbitzua itxita` ager dadin!).

---

## 8. MVC Patroia: Bistak Kontroladoreekin Lotuta eta API Dei Egokiak

> **Irizpidea (Kodean):** *MVC patroia: bistak kontrollerrekin lotuta, API-aren dei egokiak.*

### 8.1. Gerente Aplikazioa (`admin_desktop_java`) — JavaFX MVC Garbia

`MVC_ETA_JAVAFX_PANE_LABURPENA.pdf` dokumentuko ereduarekin **% 100 bat dator**:

```mermaid
flowchart LR
    USER["Kudeatzailea"] --> VIEW["VIEW (.fxml + app.css)<br/>8 pantaila deklaratibo<br/>fx:controller · fx:id · onAction"]
    VIEW -->|"Gertaerak"| CTRL["CONTROLLER (*Kontrolagailua.java)<br/>KontrolagailuFabrika (DI)<br/>@FXML · ObservableList · FilteredList"]
    CTRL -->|"UI eguneratu"| VIEW
    CTRL -->|"EkintzaAsinkronoak.run()"| API["MODEL & API GERUZA<br/>ApiBezeroa (HttpClient + Gson)<br/>KudeatzaileSaioa · 9 Model klase"]
    API <-->|"HTTP JSON (REST)"| BACKEND["ASP.NET Core REST API (:5050)<br/>NHibernate → MySQL 8"]
```

- **Bistak (`View` — `src/main/resources/.../fxml/`):**
  - 8 FXML fitxategi (`saio-hasiera.fxml`, `diseinu-nagusia.fxml`, `laburpena.fxml`, `langileak.fxml`, `menua.fxml`, `osagaiak.fxml`, `mahaiak.fxml`, `fakturak.fxml`) + `css/app.css`.
  - **Ez dute SQL kontsultarik ezta HTTP deirik.** Bakarrik `fx:controller`, `fx:id` eta `onAction="#..."` loturak definitzen dituzte.
- **Kontroladoreak (`Controller` — `src/main/java/.../controller/`):**
  - 8 kontroladore (`SaioHasieraKontrolagailua`, `DiseinuNagusiaKontrolagailua`, `LaburpenKontrolagailua`, `LangileakKontrolagailua`, `MenuaKontrolagailua`, `OsagaiakKontrolagailua`, `MahaiakKontrolagailua`, `FakturakKontrolagailua`).
  - **`KontrolagailuFabrika`** (`FXMLLoader.setControllerFactory`) bidez injektatzen zaizkie `ApiBezeroa`, `Stage` eta `KudeatzaileSaioa` menpekotasunak (Singleton/global zikinik gabe).
  - **UI haria ez da inoiz blokeatzen:** API dei guztiak `EkintzaAsinkronoak.run()` bidez exekutatzen dira 4 daemon hariko `ExecutorService` batean, eta erantzunak JavaFX Application Thread-ean (`Task.setOnSucceeded` / `setOnFailed`) prozesatzen dira.
- **Modeloa eta API bezeroa (`Model` + `ApiBezeroa`):**
  - 9 modelo klase (`KudeatzaileErabiltzailea`, `Langilea`, `Platera`, `Lehengaia`, `LehengaiaAlerta`, `PlaterEskandalloa`, `Mahaia`, `Faktura`, `FinantzaLaburpena`) + `KudeatzaileSaioa`.
  - `ApiBezeroa`-k HTTP metodo egokiak erabiltzen ditu: `GET`, `POST`, `PUT`, `DELETE`, timeout-ak (4s–10s), JSON parsing segurua eta HTTP kodeen erroreen itzulpena (`ApiSalbuespena`).

### 8.2. TPV Aplikazioa (`desktop_tpv_csharp`) — Geruzatutako WinForms Arkitektura

WinForms-en MVC-ren baliokide estandarra (View/Designer + Code-Behind Controller + Models + Service) garbi banatuta dago:
- **Bistak (`Forms/*/*.Designer.cs` + `UI/Theme.cs`, `UI/UiPerf.cs`, `UI/ProductImageCache.cs`):** Diseinu bisuala, kontrolen egitura, koloreak, DPI eskalatzea eta marrazketa.
- **Kontroladoreak (`Forms/*/*.cs` — 10 Form/Modal):** Erabiltzailearen gertaerak (`Click`, `KeyDown`, `Resize`, `Timer.Tick`) kudeatzen dituzte, baina **ez dute `HttpClient` edo JSON parsimik zuzenean egiten**; dena `_api` zerbitzuari delegatzen diote (`async`/`await` bidez).
- **Modeloak (`Models/Models.cs`):** 13 DTO/eredu klase (`ApiResponse<T>`, `LangileaDto`, `MahaiaDto`, `PlateraDto`, `EskaeraDto`, `EskaeraLerroaDto`, `CartItem`, `OrdainketaEskaeraDto`, `OrdainketaErantzunaDto`, `ErreserbaDto`...).
- **API Zerbitzua (`Services/ApiService.cs`):** REST APIko dei guztiak zentralizatuta (`GET`, `POST`, `PUT`, `DELETE`), goiburu pertsonalizatuekin (`X-App-Id: csharp_tpv`, `X-Langilea`, `X-Rola`) eta `localhost`/`127.0.0.1` fallback automatikoarekin.

### 8.3. API Dei Egokien Laburpena (HTTP Metodoak)

| Aplikazioa | `GET` (Irakurri) | `POST` (Sortu / Ekintza) | `PUT` (Eguneratu) | `DELETE` (Desgaitu / Ezabatu) |
|---|---|---|---|---|
| **Gerentea (`ApiBezeroa.java`)** | `/health`, `/system/service-status`, `/langileak`, `/platerak`, `/platerak/{id}/eskandalloa`, `/lehengaiak`, `/lehengaiak/alertak`, `/mahaiak`, `/fakturak`, `/fakturak/laburpena` | `/langileak/login`, `/langileak`, `/platerak`, `/lehengaiak`, `/lehengaiak/sarrera`, `/mahaiak`, `/fakturak` | `/langileak/{id}`, `/platerak/{id}`, `/lehengaiak/{id}`, `/mahaiak/{id}`, `/fakturak/{id}/egoera` | `/langileak/{id}`, `/platerak/{id}`, `/lehengaiak/{id}`, `/mahaiak/{id}` |
| **TPV (`ApiService.cs`)** | `/health`, `/langileak`, `/mahaiak`, `/platerak`, `/platerak/erabilgarritasuna`, `/eskaerak`, `/eskaerak/{id}`, `/erreserbak` | `/langileak/login`, `/eskaerak`, `/eskaerak/{id}/lerroak`, `/eskaerak/{id}/sukaldera`, `/ordainketak`, `/erreserbak` | `/mahaiak/{id}/egoera`, `/eskaerak/{id}/egoera`, `/eskaerak/lerroak/{id}/egoera` | `/eskaerak/lerroak/{id}`, `/eskaerak/{id}`, `/erreserbak/{id}` |

---

## 9. Objektuei Bideratutako Programazioa (Java eta C#)

> **Irizpidea (Kodean):** *Java eta C# aplikazioak objektuei bideratuta daude (herentzia, interfazeak...).*

### 9.1. Java (`admin_desktop_java` eta `desktop_javafx_core`)

| OOP Kontzeptua | Non dago kodean | Azalpena |
|---|---|---|
| **Herentzia (`extends`)** | `JatetxeaKudeaketaAplikazioa extends Application` (`L15`)<br>`ApiSalbuespena extends IOException` (`L5`)<br>`new Task<T>() { ... }` (`EkintzaAsinkronoak.java:20`) | JavaFX bizi-zikloaren hedapena, salbuespen pertsonalizatua HTTP `statusCode`-arekin, eta ataza asinkronoen espezializazioa. |
| **Klase abstraktuak eta herentzia propioa (`desktop_javafx_core`)** | `BaseController` (abstraktua) → `BaseEskaeraController` (abstraktua) / `BaseLoginController` | Proiektuko JavaFX nukleo komunak (*Template Method* patroia): `getAppResourceClass()` eta `iragaziEskaerak()` metodo abstraktuak azpiklaseetan (KDS/BDS) gainidazteko. |
| **Interfazeak (`implements` / `@FunctionalInterface`)** | `KontrolagailuFabrika implements Callback<Class<?>, Object>` (`L10`)<br>`@FunctionalInterface public interface CheckedSupplier<T>` (`EkintzaAsinkronoak.java:32`) | Kontroladoreen DI fabrika polimorfikoa eta salbuespen egiaztatuak jaurti ditzakeen lambda-interfaze funtzional propioa. |
| **Enkapsulazioa eta Konposizioa** | `KudeatzaileSaioa`, `ApiBezeroa`, `Faktura` (`private` eremuak + getter/formatu metodoak) | Barne-egoera babestua (`BooleanProperty`, `StringProperty`, `HttpClient` pribatua) eta konposizioa (kontroladore bakoitzak `ApiBezeroa` eta `KudeatzaileSaioa` jasotzen ditu). |
| **Generikoak eta `record`-ak (Java 21)** | `ApiBezeroa.getList<T>`, `getData<T>`, `EkintzaAsinkronoak.run<T>`<br>`ServiceStatus`, `ApiHealth`, `DashboardData`, `MenuData`, `RecipeComponent`, `InventoryData` | Mota-segurtasuna kodea errepikatu gabe eta datu-transferentzia objektu aldaezinak (immutable). |

### 9.2. C# (`desktop_tpv_csharp` eta `backend_rest_api_csharp`)

| OOP Kontzeptua | Non dago kodean | Azalpena |
|---|---|---|
| **Herentzia (`: Form`)** | 10 formulario guztiak: `LoginForm : Form`, `MahaiakForm : Form`, `MahaiaXehetasunakForm : Form`, `EskaeraForm : Form`, `ErreserbakForm : Form`, `OrdainketaForm : Form`, `TiketIkusteaForm : Form`, `MahaikideakModal : Form`, `OharraModal : Form`, `ErreserbaBerriaModal : Form` | WinForms leihoen bizi-zikloa, gertaerak (`Load`, `Resize`, `DpiChanged`, `KeyDown`, `Paint`) eta kontrolekin integrazioa heredatzen dituzte. *(Gainera, backend C#-n 11 kontroladoreek `BaseApiKontroladorea`-tik heredatzen dute).* |
| **Generikoak (`<T>`)** | `ApiResponse<T>` (`Models.cs:5`)<br>`ConditionalWeakTable<Control, BaseFontSpec>` (`Theme.cs:182`)<br>`ConcurrentDictionary<string, Bitmap?>` (`ProductImageCache.cs:17`) | API erantzun guztientzako bilgarri generiko bakarra eta bilduma tipatu seguruak. |
| **Enkapsulazioa, Propietate Kalkulatuak eta Klase Zigilatuak (`sealed`)** | `ApiService` (`private HttpClient _http`), `EskaeraLerroaDto.Azpitotala => UnitatePrezioa * Kantitatea`, `CartItem.Azpitotala`, `private sealed class AvatarCardInfo` (`LoginForm.cs:10`), `private sealed class MahaiTxartelUi` (`MahaiakForm.cs:10`), `private sealed record BaseFontSpec` (`Theme.cs:181`) | Barne-egoeraren babesa, konexioaren kudeaketa zentralizatua eta UI txartelen egoera-enkapsulazioa. |
| **Gainkarga (Overloading)** | `Theme.DisplayFont(...)`, `Theme.BodyFont(...)`, `Theme.MonoFont(...)`, `MahaiakForm()` / `MahaiakForm(langilea, api)` | Eraikitzaile gainkargatuak (Visual Studio Designer vs exekuzio erreala) eta DPI-ra egokitutako metodo-sinadurak. |

### 9.3. Inplementatutako OOP Herentzia eta Interfaze Propioak (Laburpena)

Bi aplikazioetan domeinu propioan sortutako interfaze eta oinarrizko klaseak aktibo eta probatuta daude:
1. **Gerente JavaFX (`admin_desktop_java`):**
   - `ApiKontratua` (`interface`) ← `ApiBezeroa implements ApiKontratua`
   - `PantailaFreskagarria` (`interface`) ← `BaseAdminKontrolagailua` (`abstract class`, `apiClient`, `session`, `ezarriMezua`, `garbitu`) ← 7 pantaila-kontroladore (`Laburpen`, `Langileak`, `Menua`, `Osagaiak`, `Mahaiak`, `Fakturak`, `Txata`).
   - `DiseinuNagusiaKontrolagailua`-k polimorfikoki `PantailaFreskagarria activeController` kudeatzen du pantailaz aldatzean `garbitu()` deitzeko.
2. **TPV C# (`desktop_tpv_csharp`):**
   - `IApiService` (`interface`) ← `ApiService : IApiService`
   - `IDpiEgokigarria` (`interface`) ← `BaseTpvForm : Form, IDpiEgokigarria` (`virtual AplikatuGaiKomuna()`, `virtual EgokituDpiAldaketan()`) ← 8 formulario nagusiak (`LoginForm`, `MahaiakForm`, `MahaiaXehetasunakForm`, `EskaeraForm`, `ErreserbakForm`, `OrdainketaForm`, `TiketIkusteaForm`, `TxataForm`).

---

## 10. Nielsen-en 10 Heuristikoetatik 5 Aplikatuta eta Justifikatuta

> **Irizpidea (Dokumentuan):** *Nielsen: 10 heuristikatik 5 aplikatu eta justifikatuta daude.*  
> **Oinarria:** [INTERFAZEEN_GARAPENA_TEORIA_LABURPENA.pdf](INTERFAZEEN_GARAPENA_TEORIA_LABURPENA.pdf) (2. Gaia — H1–H10).

Jarraian, bi aplikazioetan (Gerente JavaFX eta TPV C#) zehatz-mehatz aplikatu diren **5 heuristiko nagusiak** justifikatzen dira benetako kode-erreferentziekin:

### H1. Sistemaren egoeraren ikusgarritasuna (*Visibility of system status*)
- **Printzipioa:** Erabiltzaileak une oro jakin behar du sistema zer egiten ari den eta zer egoeratan dagoen, denbora egokian emandako feedbackaren bidez (Nielsen-en 0,1s / 1s / 10s denborak).
- **Aplikazioa Gerentean:**
  - `SaioHasieraKontrolagailua.java:58-86`: Goialdean **API/DB osasun-semaforoa** dago (`apiStatusDot` + `apiStatusLabel`), 10 segundoro automatikoki eguneratzen dena (`API egiaztatzen...` → berdea `API martxan · DB konektatuta` / laranja / gorria).
  - `DiseinuNagusiaKontrolagailua.java:59-81`: Goiko barran **zerbitzu-egoeraren bandera** (`serviceStatusLabel`) beti ikusgai dago (`status-ok` berdea itxita dagoenean; `status-warning` horia mahaiak irekita daudenean).
  - Ekintza asinkrono guztietan botoiak desgaitzen dira (`signInButton.setDisable(true)`, `createButton.setDisable(true)`) eta testu-feedbacka ematen da (*"Langilea sortzen..."*, *"Osagaiak kargatzen..."*).
- **Aplikazioa TPV-an:**
  - `LoginForm.cs:112-155, 762-881`: Goiko barran API egoera-puntua (`_lblApiStatusDot`) + 3 segundoro auto-berkonexioa; PIN teklatuan 4 puntu borobilak (`SetupPinDotsPaint()`) berehala (< 0,1s) betetzen dira.
  - `MahaiakForm.cs:445-461`: Goiburuak mahai kopuruen laburpen zuzena erakusten du (*Libre · Okupatuta · Sukaldean · Garbitzen*) eta `MahaiaXehetasunakForm.cs`-ek sukaldean prest dauden plateretan dir-dir abisu bisuala (`_pulseTimer`) aktibatzen du.
- **Justifikazioa:** Jatetxeko erritmo bizian, zerbitzariak zein gerenteak segundo batean jakin behar dute APIa martxan dagoen, zenbat mahai dauden libre edo aldaketa bat gordetzen ari den, klik bikoitzak eta ziurgabetasuna saihestuz.

### H2. Sistemaren eta mundu errealaren bat-etortzea (*Match between system and the real world*)
- **Printzipioa:** Erabiltzailearen hizkuntza, terminologia eta eredu mentala erabili behar dira, kode teknikoak (`409`, `503`, `FK constraint`) ezkutatuz.
- **Aplikazioa Gerentean:**
  - Interfaze osoa euskarazko ostalaritza-hiztegian idatzita dago (*"Menua eta errezetak"*, *"Osagaiak eta inbentarioa"*, *"Stock-sarrera"*, *"Oharra / albarana"*, *"Unitateko kostua (€)"*, *"Salmenta / Erosketa"*).
  - API errore teknikoak mezu ulergarri bihurtzen dira (`SaioHasieraKontrolagailua`, `ApiBezeroa`, `MenuaKontrolagailua`: *"Zerbitzua irekita dago (2 mahai). Menuaren aldaketak blokeatuta daude."*).
- **Aplikazioa TPV-an:**
  - Mahaiak jatetxeko benetako gune fisikoen arabera daude antolatuta (`Jangela Nagusia`, `Terraza`, `Barra`, `VIP`), eta kartako produktuak zerbitzariaren ohiko kategorietan (`Edariak` → `Urak`, `Ardoak`, `Kafeak`..., `Hastekoak`, `Haragiak`, `Postreak`) argazkiekin.
  - Egoerak jatetxeko prozesu erreala islatzen dute: `Libre` → `Okupatuta` → `Sukaldean` → `Prestatua` → `Zerbitzatua` → `Garbitzen`.
- **Justifikazioa:** Langileek ez dute ezagutza informatikorik behar aplikazioa ulertzeko; beren eguneroko lan-fluxu fisikoaren isla zuzena da.

### H3. Erabiltzailearen kontrola eta askatasuna (*User control and freedom*)
- **Printzipioa:** Akatsen edo iritziz aldatzearen aurrean *"larrialdi-irteera"* argiak eskaini behar dira, prozesua galdu edo blokeatu gabe.
- **Aplikazioa Gerentean:**
  - Edizio-modu guztietan (`LangileakKontrolagailua`, `OsagaiakKontrolagailua`, `MenuaKontrolagailua`) **`Edizioa utzi`** (`cancelEditButton`) botoia dinamikoki agertzen da, formularioa hasierako egoerara garbi itzultzeko.
  - Errezeta batean osagaiak gehitu eta kendu daitezke (`removeRecipeIngredient`) gorde aurretik.
  - Ezabaketak **Soft Delete** bidez egiten dira, datu historikoak galdu gabe, eta `Saioa itxi` botoia beti eskura dago.
- **Aplikazioa TPV-an:**
  - PIN teklatuan **`Clear`** (`Esc`) eta **`←`** (`Backspace`) daude uneoro.
  - Pantaila guztietan **`Atzera`** (`btnAtzera`) botoia dago, eta `EskaeraForm`-eko saskian plateren kopurua `+`/`-` alda edo ezaba daiteke sukaldera bidali aurretik (`EzabatuEskaeraLerroaAsync` bidez stocka itzuliz).
- **Justifikazioa:** Ostalaritzako presarekin akatsak ohikoak dira (mahai okerra ukitu, plater okerra sartu); atzera egiteko eta zuzentzeko askatasunak estresa eta errore ekonomikoak ezabatzen ditu.

### H4. Koherentzia eta estandarrak (*Consistency and standards*)
- **Printzipioa:** Barne-koherentzia (ikono, kolore, izen eta kokapen berak pantaila guztietan) eta kanpo-koherentzia (Jakob-en legea: merkatuko estandarrak).
- **Aplikazioa Gerentean:**
  - `diseinu-nagusia.fxml`-k nabigazio-barra finkoa (ezkerrean) eta goiko barra finkoa mantentzen ditu pantaila guztietan; soilik erdiko `StackPane contentHost` aldatzen da.
  - `app.css`-k botoi-klase estandarrak definitzen ditu aplikazio osoan: `.primary-button` (Chianti `#881337`, ekintza nagusia), `.secondary-button` (grisa, laguntzailea) eta `.danger-button` (gorria, desgaitzea).
- **Aplikazioa TPV-an:**
  - `Theme.ApplyWarmHospitalityTheme(this)` metodoak 10 formulario guztietan paleta, tipografia (`Cinzel`/`Segoe UI`/`JetBrains Mono`), ertz biribilduak eta egoera-kolore berak aplikatzen ditu.
  - Goiko barrak beti egitura bera du (ezkerrean logoa/`Atzera` + titulua; eskuinean langile-profila + ekintzak).
- **Justifikazioa:** Behin pantaila bat ulertuta, erabiltzaileak automatikoki daki beste guztiak nola erabili, ikaskuntza-kurba minimizatuz.

### H5. Erroreen prebentzioa (*Error prevention*)
- **Printzipioa:** Errore-mezu ona ematea baino hobea da errorea bera gertatzea saihestea (*constraints* logikoak, balidazioak eta berrespenak).
- **Aplikazioa Gerentean:**
  - **Zerbitzu-blokeo automatikoa (`KudeatzaileSaioa` + `MenuaKontrolagailua:333-361`):** Mahai bat irekita badago, menua/errezetak editatzeko botoiak automatikoki desgaitzen dira (`createButton.setDisable(disabled)`), eta gorde aurretik berriro egiaztatzen da (`verifyServiceClosed()`).
  - **Ekintza kritikoetan berrespen-modala (`PantailaLaguntza.confirm()`):** Langilea, platera edo osagaia desgaitu aurretik `Alert.AlertType.CONFIRMATION` eskatzen da.
  - **Norberaren blokeoa prebenitzea (`LangileakKontrolagailua:193`):** Kudeatzaileak ezin du bere burua desgaitu saioa irekita duen bitartean.
  - **Sarrera-balidazioak (`PantailaLaguntza:12-30`):** Zenbaki negatiboak (`parseNonNegative`), zero/negatiboak stock-sarreran (`parsePositive`), email formatua (regex) eta PIN luzera (`>= 4`) APIra bidali aurretik gelditzen dira.
  - Hautapenik ez badago, `editButton`, `deactivateButton` eta `stockEntryButton` desgaituta daude (`updateSelectionButtons()`).
- **Aplikazioa TPV-an:**
  - **Rol-murrizketa bisuala (`EskaeraForm.cs:72-82`):** `Barrakoa` rolarekin sartzean, sukaldeko kategoriak (`Hastekoak`, `Haragiak`) ezkutatu egiten dira okerreko eskaerarik ez egiteko.
  - **Stock-ahalmenaren kontrola (`ApiService.GetPlaterenStockAhalmenaAsync()`):** Agortutako platerak argi markatzen dira eta ez da uzten stocka baino gehiago eskatzen.
  - **PIN luzera-muga (`LoginForm.cs:929`):** Ezin dira 4 digitu baino gehiago sartu, eta APIa deskonektatuta badago teklatua blokeatzen da.
- **Justifikazioa:** Kontabilitatean, stockean eta mahai-zerbitzuan gertatutako *slip* eta *mistake* akats garestienak gertatu aurretik ekiditen dira.

*(Gehigarri gisa, H6 «Gogoratu beharrean ezagutu» — taulatik klik eginez editatzea eta TPV avatar-zerrenda — eta H7 «Malgutasuna eta eraginkortasuna» — TPV-an teklatu fisikoa `F5`/`Enter`/`0-9`/`Esc` + ukipen-teklatua — ere aplikatuta daude).*

---

## 11. Gestalt-en 4 Arau Aplikatuta eta Justifikatuta

> **Irizpidea (Dokumentuan):** *Gestalt: 4 arau aplikatu eta justifikatuta daude.*  
> **Oinarria:** [INTERFAZEEN_GARAPENA_TEORIA_LABURPENA.pdf](INTERFAZEEN_GARAPENA_TEORIA_LABURPENA.pdf) (5. Gaia — G1–G7 + Irudia eta funtsa).

### 1. G1 — Hurbiltasunaren Legea (*Proximity*)
- **Printzipioa:** Elkarren ondoan dauden elementuak garunak talde funtzional bakar gisa hautematen ditu, aparteko marra edo koadrorik behar gabe.
- **Non aplikatu da:**
  - **Gerentea (`langileak.fxml`, `osagaiak.fxml`, `menua.fxml`, `saio-hasiera.fxml`):** Formulario guztietan eremu bakoitzaren etiketa (`Label.field-label`) eta sarrera-kutxa (`TextField`/`ComboBox`) `VBox spacing="5"` edo `spacing="6"` txikiarekin elkartuta daude, eta eremu-talde desberdinen artean `spacing="10"`–`18` handiagoa dago. Horrela, begirada bakar batean ikusten da zein etiketa dagokion zein sarrerari.
  - **TPV (`LoginForm.cs` & `MahaiakForm.cs`):** Avatar bakoitzean borobila, izena eta rola elkarren gainean daude (`gap = 8px` txartelen artean), eta mahai-txartel bakoitzean zenbakia, edukiera eta egoera-txapa bateratuta agertzen dira.
- **Justifikazioa:** Karga kognitiboa murrizten du eta formulario luzeetan (langileak, fakturak, errezetak) datuak sartzeko abiadura handitzen du.

### 2. G2 — Antzekotasunaren Legea (*Similarity*)
- **Printzipioa:** Kolore, forma edo tamaina bera duten elementuak funtzio edo kategoria berekoak direla interpretatzen du erabiltzaileak.
- **Non aplikatu da:**
  - **Gerentea (`app.css`):** Ekintza nagusi guztiek itxura bera dute (`.primary-button`: `#881337` + testu zuria + 6px erradioa); ekintza laguntzaileek `.secondary-button` (`#F1F5F9` + ertza); ekintza arriskutsuek `.danger-button` (`#FEE2E2` + `#B91C1C`); alboko nabigazio-botoi guztiek `.nav-button` estilo berdina partekatzen dute.
  - **TPV (`Theme.cs:744-755` — `GetEgoeraKoloreak`):** Areto osoan egoera bereko mahaiek kolore-sistema identikoa dute (`Libre` = berdea `#DCFCE7`/`#15803D`, `Okupatuta` = beltz-espresso `#1C1917`, `Sukaldean` = laranja `#FFEDD5`/`#C2410C`, `Garbitzen` = urdina `#E0F2FE`/`#0369A1`). Kategoria-fitxek (`LotuFitxak()`) eta plater-txartelek ere diseinu uniforme bera dute.
- **Justifikazioa:** Zerbitzariak testu bakar bat irakurri gabe ere, segundo erdian bereizten ditu aretoko mahai libreak, okupatuak eta garbitzekoak; gerenteak, berriz, berehala bereizten ditu gorde eta ezabatu botoiak.

### 3. G3 — Eskualde Komunaren Legea (*Common Region*)
- **Printzipioa:** Ertz, koadro edo atzeko plano berean bilduta dauden elementuak unitate logiko bakartzat hartzen dira.
- **Non aplikatu da:**
  - **Gerentea (`app.css:123-129`, `osagaiak.fxml`, `menua.fxml`, `fakturak.fxml`):** Pantailaren hondoa gris leuna da (`#F8FAFC`), eta bloke funtzional bakoitza txartel zuri batean (`.card` eta `.metric-card`: `#FFFFFF`, `#E2E8F0` ertza, `10px` erradioa, `18px` padding) biltzen da. Adibidez, `osagaiak.fxml`-n 3 eskualde komun argi bereizten dira: (1) *Inbentarioa taula*, (2) *Osagai berria / editatu formularioa* eta (3) *Stock-sarrera + Alertak*.
  - **TPV (`Theme.DrawRoundedCard()`):** Hondoa `WarmLinen` (`#FAF7F2`) da, eta `LoginForm`-eko saio-txartelak (`GoldBorder` ertzarekin), `MahaiakForm`-eko mahai-txartelek eta `MahaiaXehetasunakForm`-eko laburpen/tiket panelek `MarbleWhite` (`#FFFFFF`) azalera biribilduetan biltzen dituzte beren datu eta botoiak.
- **Justifikazioa:** Informazio dentsitate handiko kudeaketa-pantailetan hierarkia bisual garbia ezartzen du, atal desberdinetako eremuak elkarrekin nahastea eragotziz.

### 4. Irudia eta Funtsa (*Figure–Ground*) — *(+ G6 Jarraitutasuna)*
- **Printzipioa:** Begiak berehala bereizten du elementu nagusia (**irudia**, aurreko planoa) atzeko planotik (**funtsa**), kontrastearen, itzalen eta tarteen bidez.
- **Non aplikatu da:**
  - **Gerentea (`saio-hasiera.fxml` + `app.css:8-29`, `PantailaLaguntza.confirm()`):** Login pantailan `.login-card` txartel zuriak itzal sakona (`dropshadow(gaussian, rgba(15,23,42,0.12), 28, ...)`) eta ezkerreko `#881337` marka-panela ditu hondo gradiente leunaren (`#F8FAFC → #F1F5F9`) gainean, begirada berehala saio-hasierara finkatuz. Berrespen-modalek (`Alert`) atzeko lan-espazioa blokeatu eta arreta osoa erabaki kritikoan jartzen dute.
  - **TPV (`MahaikideakModal`, `OharraModal`, `ErreserbaBerriaModal`, `LoginForm.cs:204`):** Modal guztiak (`ShowDialog(this)`) guraso-leihoaren erdian irekitzen dira (`ApplyModalDpiSize`) atzeko planoa geldituta utziz, eta `LoginForm`-en hautatutako langilearen avatarra `#881337` Chianti + `GoldBorder` bihurtzen da gainerako `StoneMist` avatarren gainetik nabarmenduz.
  - *(Gainera, **G6 Jarraitutasuna** argi ikusten da Gerenteko `TableView` zutabe lerrokatuetan, alboko nabigazio bertikalean eta TPV-ko 4 urratseko PIN puntuetan eta `FlowLayoutPanel` sareta erregularretan).*
- **Justifikazioa:** Erabiltzailearen arreta une bakoitzeko zeregin nagusira bideratzen du (login, berrespena, mahaikide-kopurua, oharra), distrakzio bisualak kenduz.

---

## 12. Defentsa eta Entrega Aurreko Egiaztapen-Zerrenda (10/10 Prest!)

Hutsune guztiak inplementatu, konpilatu eta zuzenean probatu ondoren, **11 irizpideak % 100 beteta** daude:

- [x] **1. puntua (Diseinu sistema):** PDF ofiziala + MD dokumentazioa + Gerente (`app.css`) eta TPV (`Theme.cs`) lerrokatuta.
- [x] **2. puntua (DrawIO Klase Diagrama):** `klase_diagrama.drawio` (3 orri: Sistema Orokorra + Gerente JavaFX + TPV C#) eta `klase_diagrama.md` eguneratuta *(DrawIO-tik `File -> Export as -> PDF` eginda 3 orriak PDF bateratuan esportatu daitezke)*.
- [x] **3–4. puntuak (JavaFX + Gerente Login):** Java 21 + JavaFX 21.0.6, 9 FXML bista (JUnit 5 test guztiak `PASS`), API login + BCrypt + rol-kontrola.
- [x] **5. puntua (Erabiltzaileak eta Txata):** `langileak.txata_aktibo` + `txat_mezuak` DBan, `PUT /langileak/{id}/txata`, `TxataController` (`403` blokeoarekin + Socket.IO zubia), Gerente `langileak.fxml` + `txata.fxml`, eta TPV `TxataForm` martxan eta E2E probatuta.
- [x] **6. puntua (TPV Login aktiboak):** Backend `401` blokeoa + TPV `LoginForm.cs` `l.Aktibo` iragazkia + Gerenteko `Aktibatu / Desaktibatu (TPV)` botoia (`PUT /langileak/{id}/aktibo`).
- [x] **7. puntua (Stocka):** `OsagaiakKontrolagailua` (stock-sarrera, gutxienekoa, erosketa-kostua, alertak) + `MenuaKontrolagailua` (salmenta-prezioa, errezetak).
- [x] **8–9. puntuak (MVC + OOP Java & C#):** `ApiKontratua`, `PantailaFreskagarria`, `BaseAdminKontrolagailua`, `IApiService`, `IDpiEgokigarria`, `BaseTpvForm`.
- [x] **10–11. puntuak (Nielsen & Gestalt):** Nielsen-en 5 Heuristikoak (H1–H5) eta Gestalt-en 4 Arauak (G1, G2, G3, Irudia/Funtsa) dokumentu honetan kode-ebidentziekin justifikatuta.
