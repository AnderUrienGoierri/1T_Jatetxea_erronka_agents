package eus.goierri.jatetxea.komandera.ui.theme

import androidx.compose.ui.graphics.Color

// ==========================================
// T1 JATETXEA v2.0 — WARM HOSPITALITY & EVENING ESPRESSO
// ==========================================

// Marka Nagusiak (Chianti & Urre Zahartua)
val ChiantiPrimary = Color(0xFF881337)     // Chianti gorri sakona (Light CTA & Brand)
val ChiantiDark = Color(0xFFBE123C)        // Chianti distiratsuagoa Modu Ilunean
val PomodoroHover = Color(0xFF9F1239)      // Hover/Active
val BurgundyDark = Color(0xFF4C0519)       // Borgoina sakona
val ChiantiTint = Color(0xFFFCE7F0)        // Chianti fondo leuna (Light)
val ChiantiTintDark = Color(0xFF3A0A1B)    // Chianti fondo leuna (Dark)

// Urrea (Antique Gold — VIP, Faktura, Barra)
val AntiqueGold = Color(0xFFB45309)        // Urre zahartua (WCAG AA+)
val AmberAccent = Color(0xFFB45309)        // Bateragarritasuna — Warm Gold
val AmberLight = Color(0xFFFDE68A)         // Urre argia
val GoldBorder = Color(0xFFD4A359)         // Ertz urreztatua
val GoldTint = Color(0xFFFEF3C7)           // Urre fondo leuna

// Warm Hospitality Light (60-30-10)
val WarmLinen = Color(0xFFFAF7F2)          // 60% Atzealde epela (Liho Epela)
val MarbleWhite = Color(0xFFFFFFFF)        // Gainazal goratuak (Txartelak)
val StoneMist = Color(0xFFF3EFE8)          // Inputak eta elementu sekundarioak
val WarmTaupe = Color(0xFFE6DFD5)          // 1dp ertz finak
val EspressoInk = Color(0xFF1C1917)        // 30% Testu nagusia (16.4:1 AAA)
val WarmSlate = Color(0xFF57534E)          // Azpitituluak (7.3:1 AAA)
val StoneMuted = Color(0xFF78716C)         // Xehetasunak (4.6:1 AA)

// Evening Espresso Dark (OLED Lagungarria)
val EveningEspresso = Color(0xFF0F0D0E)    // Atzealde ilun beroa (ez #000000)
val CharcoalWine = Color(0xFF1A1618)       // Txartelen gainazala
val WarmGraphite = Color(0xFF262023)       // Inputak eta chipak
val DarkHairline = Color(0xFF352D31)       // Ertz finak modu ilunean

// Bateragarritasun aliasak
val AmberPrimary = ChiantiPrimary
val AmberDark = PomodoroHover
val DarkBackground = EveningEspresso
val DarkSurface = CharcoalWine
val DarkSurfaceVariant = WarmGraphite
val DeepBackground = EveningEspresso
val SurfaceDark = CharcoalWine
val SurfaceVariantDark = WarmGraphite
val SurfaceBorder = DarkHairline

// Egoera Semantikoak (WCAG 2.2 POUR — Beti Ikono + Testua)
val StatusFree = Color(0xFF15803D)         // Albahaka Berdea (Libre / Zerbitzatua)
val StatusFreeLight = Color(0xFFDCFCE7)    // Berde argia txapetarako
val StatusFreeDark = Color(0xFF4ADE80)
val StatusReady = StatusFree

val StatusOccupied = Color(0xFFB91C1C)     // Gorri sakona (Okupatuta / Errorea)
val StatusOccupiedLight = Color(0xFFFEE2E2)
val StatusOccupiedDark = Color(0xFFF87171)
val StatusBusy = StatusOccupied
val StatusAgortua = StatusOccupied

val StatusPending = Color(0xFFC2410C)      // Toskana Terrakota (Sukaldean / Zain)
val StatusWarningBg = Color(0xFFFFEDD5)
val StatusPendingDark = Color(0xFFFB923C)

val StatusCleaning = Color(0xFF0369A1)     // Mediterraneo Urdina (Garbitzen)
val StatusCleaningLight = Color(0xFFE0F2FE)
val StatusCleaningDark = Color(0xFF38BDF8)
val StatusOrdering = StatusCleaning
val StatusBill = AntiqueGold

// Testuak
val TextPrimary = WarmLinen
val TextSecondary = Color(0xFFA8A29E)
val TextMuted = StoneMuted
val TextOnBrand = Color(0xFFFFFFFF)
