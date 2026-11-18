# Figma transferentzia - T1 Jatetxea Kudeaketa

Dokumentu honek Figma proiektua sortzeko eta JavaFX pantailekin bat etortzeko oinarria ematen du. Ez da Figma kontu edo `.fig` fitxategi pribaturik sortu; pantaila-eskemak eta tokenak hemen jasota daude.

## Figma Pages

1. `Cover & Info`: T1 Jatetxea Kudeaketa, desktop back-office.
2. `Design Tokens`: koloreak, tipografia, tarteak eta osagaiak.
3. `Pantailak - Desktop`: `login`, `dashboard`, `langileak`, `menua`, `osagaiak` eta `mahaiak`.
4. `States`: karga, hutsik, errorea/API deskonektatuta, zerbitzua irekita eta baieztapen-modala.

## Frame bakoitzaren neurriak eta konposizioa

- Lan-eremua: 1440 x 900 px; ezkerreko nabigazioa 248 px; goiko barra 78 px.
- Login: 1180 x 760 px; marka-panel ardo-gorria eta formulario argia.
- Laburpena: lau KPI txartel eta kudeaketa-oharren panela.
- Langileak: zerrenda zabala eta eskuinaldean langile berria sortzeko formularioa.
- Menua: plateren zerrenda; azpian plater-datuak eta errezetako osagaien hautaketa.
- Osagaiak: inbentario-taula; azpian osagai berria eta stock-sarrera.
- Mahaiak: mahaien zerrenda eta eskuinaldean altako/egoera-formularioa; idazketa blokeatuta zerbitzua aktibo edo ezezaguna denean.

## Design tokens

| Token | Balioa |
|---|---|
| Brand / Primary | `#881337` |
| Brand / Hover | `#9F1239` |
| Brand / Dark | `#4C0519` |
| Success / Main | `#15803D` |
| Warning / Main | `#C2410C` |
| Warning / Light | `#FEF3C7` |
| Danger / Main | `#DC2626` |
| App / Background | `#F8FAFC` |
| Surface / Card | `#FFFFFF` |
| Border / Subtle | `#E2E8F0` |
| Text / Primary | `#1E293B` |
| Text / Muted | `#64748B` |

Tipografia: Segoe UI; H1 23 px/700, H2 16 px/700, testua 13 px/400, etiketak 11-12 px/700. Txartelak 10 px-ko izkinarekin; botoiak/eremuak 6 px-koarekin. Fitxategi honetako pantailen izenak `src/main/resources/eus/goierri/jatetxea/admin/fxml/` karpetako FXMLekin bat datoz.
