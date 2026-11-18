package eus.goierri.jatetxea.komandera.ui.theme

import androidx.compose.ui.graphics.Color

// ==========================================
// T1 JATETXEA - ITALO-EUSKALDUN MARKA KOLOREAK
// ==========================================

// Marka Nagusiak (Chianti & Ardoa)
val ChiantiPrimary = Color(0xFF881337)     // Chianti gorri sakona (Brand nagusia)
val PomodoroHover = Color(0xFF9F1239)      // Pomodoro gorri distiratsua (Hover/Active)
val BurgundyDark = Color(0xFF4C0519)       // Erromatar Borgoina (Testu eta itzal altuak)

// Azentuak eta Urrea (Parmigiano & Labea)
val AmberAccent = Color(0xFFD97706)        // Urre aberatsa / Amber
val AmberLight = Color(0xFFFDE68A)         // Urre argia / Parmigiano
val GoldBorder = Color(0xFFFCD34D)         // Urrezko ertz dotoreak

// BATERAGARRITASUNA (Kode zaharrak konpilatzeko):
// AmberPrimary Chianti gorri/ardo dotorearekin edo urre aberatsarekin ordezkatzen dugu
val AmberPrimary = ChiantiPrimary
val AmberDark = PomodoroHover
val DarkBackground = Color(0xFF0B0F17)     // Beltz arbel sakona (OLED lagungarria)
val DarkSurface = Color(0xFF161F30)        // Txartelen azalerak
val DarkSurfaceVariant = Color(0xFF222E42) // Input eta botoi sekundarioak

// Hondoak eta Azalerak
val DeepBackground = DarkBackground
val SurfaceDark = DarkSurface
val SurfaceVariantDark = DarkSurfaceVariant
val SurfaceBorder = Color(0xFF334155)      // Banatze-marra eta ertz sotilak

// Egoera Koloreak (Jatetxeko Funtzioak)
val StatusFree = Color(0xFF15803D)         // Albahaka Berdea (Libre / Zerbitzatua)
val StatusFreeLight = Color(0xFFDCFCE7)    // Berde argia txapetarako
val StatusReady = Color(0xFF15803D)        // Prest / Eginda

val StatusOccupied = Color(0xFFDC2626)     // Alerta Gorria / Okupatuta
val StatusBusy = Color(0xFFDC2626)         // Okupatuta
val StatusAgortua = Color(0xFFDC2626)      // Stock agortua

val StatusPending = Color(0xFFC2410C)      // Toskana Terrakota / Laranja (Martxan / Zain)
val StatusWarningBg = Color(0xFFFEF3C7)    // Parmigiano krema abisu-fondoa

val StatusCleaning = Color(0xFF0284C7)     // Mediterraneo Urdina (Garbitzen)
val StatusOrdering = Color(0xFF0284C7)     // BDS / Barra / Eskaeran
val StatusBill = Color(0xFFD97706)         // Urrea / Faktura

// Testuak eta Tipografia (WCAG AAA kontrastea)
val TextPrimary = Color(0xFFF8FAFC)        // Irina Zuria (Testu nagusia)
val TextSecondary = Color(0xFF94A3B8)      // Harri Grisa (Azpitituluak, datuak)
val TextMuted = Color(0xFF64748B)          // Laino Grisa (Xehetasunak, orduak)
val TextOnBrand = Color(0xFFFFFFFF)        // Zuri garbia botoi ilunen gainean
