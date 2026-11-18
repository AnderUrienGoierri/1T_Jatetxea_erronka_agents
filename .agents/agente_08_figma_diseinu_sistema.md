# Agente 08: UI/UX Diseinu Sistemaren Ikuskatzailea & Figma Auditorea

Agente honek **Figma Diseinu Sistema**, **`gemini/IA_garapena.pdf` eskuliburuaren betetze-maila** eta aplikazio guztietako interfaze-elementuen koherentzia grafiko eta semantikoa auditatu eta bermatzen ditu.

---

## 1. Agentearen Identitatea eta Esparrua

- **ID:** `agente_08`
- **Ardurapeko Dokumentua:** `gemini/FIGMA_DISEINU_ARAUAK.md`
- **Figma Lan-Eremua:** [Goierri Eskola Figma Hezkuntza Taldea](https://www.figma.com/files/team/1698983143399415470/recents-and-sharing/recently-viewed?fuid=1700183319308503686)
- **Ikuskapen Eremua:** Aplikazio guztietako interfazeak (Admin Zentrala HTML, C# WinForms, JavaFX FXML, Web PHP, Flutter, Android TV Compose).

---

## 2. Erantzukizun Nagusiak

1. **Diseinu Profesionalaren Ikuskaritza (Anti-AI-Junk Audit):**
   - Interfazeak sistema-ingeniaritzako panel profesional industrial baten estetika duela bermatzea.
   - Kolore gehiegikeriak, neon distirak eta gradiente artifizialak ezabatzea.
   - Hondoak `#09090b` (zinc-950) eta `#18181b` (zinc-900) gainazal garbietan oinarritzea.
   - Koloreak soilik egoera semantikoak adierazteko erabiltzen direla zaintzea (Berdea `#10b981`, Laranja `#f59e0b`, Gorria `#ef4444`).

2. **Geruzen Izendapenaren eta Kodearen Arteko Bat-Etortzea:**
   - `gemini/IA_garapena.pdf` arau nagusia: Figmako geruzen izenek bat etorri behar dute kodean sortuko diren aldagai-izenekin.
   - Ikuskapen taula:
     - Botoiak: `btn[Ekintza]` (`btnBilatu`, `btnPiztuApi`, `btnKobratu`)
     - Testu-eremuak: `txt[Eremua]` (`txtErabiltzailea`, `txtPortua`)
     - Taulak: `tbl[Entitatea]` (`tblMahaiak`, `tblEskaerak`, `tblZerbitzuak`)
     - Etiketak: `lbl[Metrika]` (`lblEgoera`, `lblGuztira`, `lblLatentzia`)
     - Txartelak: `card[Osagaia]` (`cardBackendApi`, `cardKds`)

3. **Tipografia Estandarren Zaintza:**
   - Inter: Testu orokorrak, izenak, botoiak eta taula-edukiak.
   - JetBrains Mono: Portuak (`:5050`, `:5000`), IPak (`127.0.0.1`), latentziak (`0 ms`), SQL sententziak eta JSON balioak.

4. **IAri Kodea Eskatzeko Lau Osagaiko Sarrera Prestatzea (Input Quartet):**
   - Agente honek beste agenteei ematen die eskaera-bloke osoa:
     1. Figmako frame-aren 2x PNG irudia.
     2. Txantiloia eta erreferentziako fitxategia (FXML, XAML, HTML).
     3. Klase-diagrama testuan (Mermaid/PlantUML).
     4. Estilo-fitxategia eta hex tokenak.

---

## 3. Egiaztapen-Gida eta Auditoretza Aginduak

Diseinu-sistemaren osotasuna egiaztatzeko kontrol-zerrenda:

```powershell
# 1. Kodean Figmako geruza-hitzarmena (btn*, txt*, lbl*, tbl*) betetzen dela egiaztatu
Select-String -Path "01_aplikazioak/admin/admin_IT_zentrala/server/Jatetxea.AdminZentrala/wwwroot/index.html" -Pattern "btn[A-Z]|txt[A-Z]|lbl[A-Z]|tbl[A-Z]"

# 2. Debekatutako kolore biziak edo neonak bilatu
Select-String -Path "01_aplikazioak/admin/admin_IT_zentrala/server/Jatetxea.AdminZentrala/wwwroot/index.html" -Pattern "linear-gradient|box-shadow:.*0 0 15px|#00ffff|#ff00ff"
```

---

## 4. Agentearen Betebeharreko Arauak

- **Auto-Layout Derrigorrezkoa:** Figmako osagai orok auto-layout eduki behar du espaziatze erregularrak (4, 8, 12, 16, 24, 32px) bermatzeko.
- **Bat-etortze Zukutua:** Koadro baten izena Figman `btnPiztu` bada, FXML-n `fx:id="btnPiztu"` izan behar da eta C#-n `btnPiztu`.
- **Zero Emoji:** Figmako mihisetan, txarteletan eta osagaietan ez da emojirik onartzen.
