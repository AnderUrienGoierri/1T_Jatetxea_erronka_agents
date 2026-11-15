# 1T Jatetxea - Figma Diseinu Sistema eta Interfaze Arauak

Dokumentu honek **1T Jatetxea Kudeaketa Sistema Integraleko** aplikazio guztietarako (Admin IT Zentrala, ASP.NET Core API, C# Windows Forms TPV, JavaFX Sukaldea KDS, Web PHP Publikoa, Mugikorreko bezero eta zerbitzari aplikazioak, eta Android TV) diseinu-sistema bateratua, Figma hezkuntza-estandarrak eta `gemini/IA_garapena.pdf` eskuliburuak ezarritako geruzen eta aldagaien arteko baliokidetza-arauak finkatzen ditu.

---

## 1. Helburua eta Figma Lan-Eremua

- **Figma Hezkuntza Taldea:** [Goierri Eskola Figma Taldea](https://www.figma.com/files/team/1698983143399415470/recents-and-sharing/recently-viewed?fuid=1700183319308503686)
- **Oinarrizko Helburua:** IA bidez garatutako interfazeek sistema-ingeniaritzako panel administratibo profesional baten itxura edukitzea; neurriz kanpoko kolore apaingarri, distira neon edo gradiente artifizialik gabe.
- **Bat-etortze Zorrotza:** Figmako geruzen izenek, egiturak eta aldagai-kodeek %100eko korrespondentzia izan behar dute proiektuko koderako aldagai-izenekin (C#, JavaFX, PHP, TypeScript/HTML, Flutter).

---

## 2. Kolore Paleta Estandarra (Dark Zinc Monochrome)

Interfaze guztiak gai ilun profesionalean eraikitzen dira, kontraste handiko paleta monokromatikoa erabiliz. Koloreak egoera semantikoak adierazteko soilik erabiltzen dira.

### 2.1 Hondoak eta Gainazalak (Neutral Zinc)

| Tokena | Hex Balioa | Erabilera Figman eta Kodean |
| :--- | :--- | :--- |
| `surface-canvas` | `#09090b` (zinc-950) | Orriaren edo leihoaren oinarrizko hondo nagusia. |
| `surface-panel` | `#18181b` (zinc-900) | Txartelak, panel nagusiak, taulen edukiontziak. |
| `surface-header` | `#121215` (zinc-925) | Nabigazio-barrak, goiburuak, taula-goiburuak. |
| `surface-subtle` | `#27272a` (zinc-800) | Botoi sekundarioak, txip pasiboak, input eremuak. |
| `surface-hover` | `#3f3f46` (zinc-700) | Elementuen gaineko sagu-pasa (hover) egoera. |

### 2.2 Mugak eta Banatzaileak (Borders)

| Tokena | Hex Balioa | Erabilera Figman eta Kodean |
| :--- | :--- | :--- |
| `border-subtle` | `#27272a` (zinc-800) | Txartel eta panelen kanpo-mugak (1px solid). |
| `border-strong` | `#3f3f46` (zinc-700) | Banatzaile horizontal/bertikalak eta foku-mugak. |
| `border-focus` | `#71717a` (zinc-500) | Teklatu-fokua edo hautatutako elementua (1.5px). |

### 2.3 Tipografia eta Testu Kontrasteak

| Tokena | Hex Balioa | Erabilera Figman eta Kodean |
| :--- | :--- | :--- |
| `text-primary` | `#f4f4f5` (zinc-100) | Tituluak, balio nagusiak, etiketa aktiboak. |
| `text-secondary` | `#a1a1aa` (zinc-400) | Deskribapenak, zutabe-izenak, azalpen-testuak. |
| `text-muted` | `#71717a` (zinc-500) | Metadatuak, ordu-markak, leku-markak (placeholders). |

### 2.4 Egoera Semantikoen Koloreak (Bakarrik Egoeretarako)

> [!IMPORTANT]
> Debekatuta dago kolore hauek apaingarri gisa erabiltzea. Berdea, laranja edo gorria egoera jakin bat egiaztatzeko soilik erabiliko dira.

| Egoera | Testu / Ikono Hex | Hondo Hex | Muga Hex | Esanahia Proiektuan |
| :--- | :--- | :--- | :--- | :--- |
| **Online / Arrakasta** | `#10b981` (emerald-500) | `#064e3b` (emerald-950) | `#047857` (emerald-700) | Zerbitzua martxan, DB konexio ona, eskaera kobratua. |
| **Degraded / Abisua** | `#f59e0b` (amber-500) | `#451a03` (amber-950) | `#b45309` (amber-700) | API martxan baina DB itzalita, stock baxua, sukaldean zain. |
| **Offline / Errorea** | `#ef4444` (rose-500) | `#450a0a` (rose-950) | `#b91c1c` (rose-700) | Zerbitzua erorita, suebaki blokeoa, transakzio errorea. |
| **Informazioa / Sareak** | `#38bdf8` (sky-400) | `#082f49` (sky-950) | `#0369a1` (sky-700) | Sare-parametroak, VLAN datuak, IP helbideak, ataka-zenbakiak. |

---

## 3. Tipografia Arauak

Bi tipografia ofizial bakarrik onartzen dira proiektu osoan:

1. **Inter (UI eta Testu Egitura Orokorra):**
   - Letra-tipo nagusia interfazeko titulu, botoi, nabigazio eta formularioetarako.
   - Pisuak: Regular (400) gorputzerako, Medium (500) botoi eta etiketarako, SemiBold (600) goiburu eta txartel-tituluetarako.
   - Letra-tamainen eskala:
     - H1 (Pantaila goiburua): 20px / lerro-tartea 28px / SemiBold
     - H2 (Panel / Atal goiburua): 15px / lerro-tartea 22px / SemiBold
     - H3 (Azpi-atala): 13px / lerro-tartea 18px / Medium
     - Body (Testu arrunta): 13px / lerro-tartea 18px / Regular
     - Caption / Etiketa txikia: 11px / lerro-tartea 14px / Regular edo Medium

2. **JetBrains Mono (Datu Teknikoak eta Metrikak):**
   - Ezinbestekoa datu teknikoetarako: IP helbideak (`127.0.0.1`), atakak (`:5050`, `:5000`), latentziak (`0 ms`), SQL galderak, JSON erantzunak eta komando-lerroko eremuetarako.
   - Tamaina: 11px - 12px / Monospace / Regular edo Medium.

---

## 4. Geruzen Izendapen Estandarra (Layer Naming Protocol)

`gemini/IA_garapena.pdf` arau nagusia aplikatuz, Figmako geruzen izenek bat etorri behar dute zehazki kodean sortuko diren aldagai eta kontroladoreen izenekin.

### 4.1 Aurrizkien Hitzarmena

| Osagai Mota | Aurrizkia Figman | Adibidea Figman | Kodeko Baliokidea |
| :--- | :--- | :--- | :--- |
| **Botoiak** | `btn` | `btnPiztuApi`, `btnItzaliDb`, `btnKDS` | `btnPiztuApi.setOnAction(...)` / `btnPiztuApi.Click` |
| **Testu-koadroak** | `txt` | `txtPortua`, `txtIzenpea`, `txtMahaiZbk` | `txtPortua.getText()` / `txtPortua.Text` |
| **Etiketak / Mezuak** | `lbl` | `lblEgoeraApi`, `lblDbAbisua`, `lblLatentzia` | `lblEgoeraApi.setText(...)` / `lblEgoeraApi.Text` |
| **Taulak / Gridak** | `tbl` | `tblZerbitzuak`, `tblMahaiak`, `tblLehengaiak` | `TableView<Mahai> tblMahaiak` / `DataGrid` |
| **Txartelak / Panelak** | `card` | `cardBackendApi`, `cardKDS`, `cardSuebakia` | `VBox cardBackendApi` / `Panel cardBackendApi` |
| **Modal / Elkarrizketak** | `dlg` | `dlgXehetasunak`, `dlgBerretsiItzaltzea` | `Dialog dlgXehetasunak` / `Form dlgXehetasunak` |
| **Egoera-txapak (Badges)** | `badge` | `badgeOnline`, `badgeOffline`, `badgeAbisua` | `Label badgeOnline` estiloduna |
| **Hautatzaileak / Combo** | `cmb` | `cmbSareProfila`, `cmbKategoria` | `ComboBox cmbSareProfila` |

---

## 5. Pantaila Tamainak eta Erantzun-Dimentsioak (Frames)

| Ingurunea / Aplikazioa | Neurri Estandarra | Alderdi-Erlazioa | Oharrak |
| :--- | :--- | :--- | :--- |
| **Admin IT Zentrala (Web Dashboard)** | 1920x1080 eta 1280x800 | 16:9 / 16:10 | Goiburu finkoa, zerbitzuen kontrol-taula eta oineko akzioak. |
| **Desktop C# TPV (Windows Forms)** | 1280x800 eta 1024x768 | 16:10 / 4:3 | Ukimen-pantaila optimizatua, botoi handiagoak (min 44px). |
| **JavaFX KDS Sukaldea (Desktop)** | 1920x1080 | 16:9 | Sukaldeko pantaila nagusia, zutabe anitzeko komanda-txartelak. |
| **Android TV KDS** | 1920x1080 | 16:9 Paisaia | Urruneko agintearekin (D-Pad) nabigatzeko foku-diseinua. |
| **Mugikor Bezeroa & Zerbitzaria** | 390x844 (iPhone 14 / Android) | 9:19.5 Bertikala | Beheko nabigazio-barra (BottomNav), hatz-eremuko botoiak. |
| **Web Publikoa (PHP Apache)** | 1440x900 (Desktop) / 390x844 (Mobile) | Responsive | Karta publikoa eta erreserba formularioa. |

---

## 6. Auto-Layout eta Espaziatze Tokenak

Figmako frame eta osagai orok Auto-Layout erabili behar du (ez utzi elementuak absolutuki kokatuta). Horrela, kode bihurtzerakoan HTML flexbox/grid, JavaFX VBox/HBox/GridPane edo XAML Grid/StackPanel erraz sortzen dira.

- **Espaziatze-eskala (Padding / Gap):**
  - `space-1`: 4px (osagai barruko ikono eta testu arteko tartea)
  - `space-2`: 8px (botoi barruko padding bertikala, txartelen arteko tarte txikia)
  - `space-3`: 12px (inपुट padding-a, zerrendako elementuen arteko tartea)
  - `space-4`: 16px (txartelen barruko padding estandarra)
  - `space-6`: 24px (atalen arteko banaketa)
  - `space-8`: 32px (pantailako bloke nagusien arteko marjina)
- **Border Radius (Txokoen Biribiltzea):**
  - `radius-sm`: 4px (txapak, egoera-pilulak, scrollbarrak)
  - `radius-md`: 6px (botoiak, input eremuak, hautatzaileak)
  - `radius-lg`: 8px (txartelak, panelak, taulen markoak)
  - `radius-xl`: 12px (modalak, leiho mugikorrak)

---

## 7. IAri Kodea Eskatzeko Lau Osagaiko Sarrera (Input Quartet)

IAri interfaze berri baten inplementazioa eskatzean, jarraitu `gemini/IA_garapena.pdf` eskuliburuko 4. urratsa:

```text
[1. Figmako Frame-aren PNG Kaptura 2x]
[2. Txantiloia eta Proiektu Erreferentzia (pom.xml / csproj / fxml / xaml)]
[3. Klase Diagrama Testuan (Mermaid / PlantUML)]
[4. Estilo Dokumentua (Hex tokenak, neurriak, letra-tipoak)]
```

### 7.1 Eskaera-Txantiloiaren Egitura

```markdown
Mesedez, sortu ondorengo pantailaren kodea:

1. Aplikazioa: [adib. JavaFX Sukaldea KDS / C# TPV / Web PHP]
2. Pantaila: [adib. Komanden zerrenda eta egoera-aldaketa]
3. Erreferentziako Diseinua: [Figmako PNG kaptura atxikia]
4. Geruzen Izenak:
   - Taula: tblKomandak
   - Botoiak: btnPrestatu, btnZerbitzatu, btnEzeztatu
   - Etiketak: lblMahaia, lblOrdutegia, lblEgoera
5. Estilo Tokenak:
   - Hondoa: #18181b, Muga: #27272a, Testua: #f4f4f5
   - Egoera berdea: #10b981, laranja: #f59e0b, gorria: #ef4444
   - Tipografia: Inter (UI) eta JetBrains Mono (zenbakiak eta orduak)
6. Klase Diagrama (Mermaid):
   class EskaeraLerroa {
       +int id
       +int mahaiaId
       +string platerIzena
       +string egoera
   }
```

---

## 8. Diseinuaren Berrikuspen Kontrol-Zerrenda (Design Review Checklist)

Edozein interfaze onartu aurretik, puntu hauek bete direla egiaztatu behar da:

- [ ] **Kolore murrizketa:** Ez dago alferrikako kolore distiratsurik; egoera-koloreak (berdea, laranja, gorria, urdina) soilik egoerak adierazteko erabili dira.
- [ ] **Hondo monokromatikoa:** Hondo guztiak zinc ilunaren (`#09090b` eta `#18181b`) paletakoak dira.
- [ ] **Geruzen izenak:** Figmako elementu bakoitzak dagokion aurrizkia (`btn*`, `txt*`, `tbl*`, `lbl*`) dauka eta kodearekin bat dator.
- [ ] **Auto-layout:** Geruza guztiak auto-layout bidez lerrokatuta daude; tamaina aldatzean ez dira elementuak gainjartzen.
- [ ] **Datu teknikoak:** Portuak, IPak, latentziak eta metrikak JetBrains Mono letra-tipoan daude.
- [ ] **Zero Emoji:** Interfazean eta mezutan ez dago emojirik; ikono bektorial garbiak edo testu esplizituak erabili dira.
