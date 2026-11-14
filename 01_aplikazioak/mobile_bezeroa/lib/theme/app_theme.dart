import 'package:flutter/material.dart';

/// T1 Jatetxea · Osteria & Parrilla (Italiar-Euskaldun Diseinu Sistema)
/// Chianti Ardoa, Parmigiano Urrea, Albahaka Berdea eta Terrakota
class AppTheme {
  // Brand Koloreak
  static const Color brandPrimary = Color(0xFF881337); // Chianti & Ardo Gorria
  static const Color brandHover = Color(0xFF9F1239);   // Pomodoro Rustico
  static const Color brandDark = Color(0xFF4C0519);    // Erromatar Borgoina
  static const Color brandLight = Color(0xFFFFF1F2);

  // Ezizen Koloreak (Italo-Vasco Diseinua)
  static const Color chiantiWine = Color(0xFF881337);
  static const Color parmigianoGold = Color(0xFFFCD34D);
  static const Color basilGreen = Color(0xFF15803D);
  static const Color terracotta = Color(0xFFC2410C);
  static const Color warmBackground = Color(0xFFFFFBEB);
  static const Color creamCard = Color(0xFFFEF3C7);
  static const Color darkCharcoal = Color(0xFF1C1917);

  // Natura eta Arrakasta
  static const Color success = Color(0xFF15803D);      // Baso & Albahaka Berdea
  static const Color successBg = Color(0xFFDCFCE7);
  static const Color successBorder = Color(0xFF86EFAC);

  // Urrea & Labea
  static const Color gold = Color(0xFFFCD34D);         // Parmigiano Urrea
  static const Color goldDark = Color(0xFFD97706);
  static const Color warning = Color(0xFFC2410C);      // Toskana Terrakota / Labea
  static const Color warningBg = Color(0xFFFEF3C7);
  static const Color warningBorder = Color(0xFFFCD34D);
  static const Color warningText = Color(0xFF92400E);

  // Barra BDS & Informazioa
  static const Color info = Color(0xFF0284C7);         // Mediterraneo Urdina
  static const Color infoBg = Color(0xFFE0F2FE);
  static const Color infoBorder = Color(0xFFBAE6FD);

  // Alerta & Arriskua
  static const Color danger = Color(0xFFDC2626);       // Alerta Gorria
  static const Color dangerBg = Color(0xFFFEE2E2);
  static const Color dangerBorder = Color(0xFFFECACA);

  // Neutroak
  static const Color bgApp = Color(0xFFF8FAFC);        // Irina Txuria
  static const Color bgCard = Color(0xFFFFFFFF);       // Marmol Zuria
  static const Color border = Color(0xFFE2E8F0);
  static const Color textMain = Color(0xFF1E293B);     // Arbel Grisa
  static const Color textMuted = Color(0xFF64748B);    // Laino Grisa

  // Segurtasun Kolorea
  static const Color securityGreen = Color(0xFF059669);
  static const Color securityBg = Color(0xFFECFDF5);

  // Kategoriaren araberako koloreak
  static Color getKategoriaBg(String kategoria) {
    switch (kategoria.toLowerCase()) {
      case 'sarrerakoa':
      case 'hastekoak':
        return infoBg;
      case 'lehena':
      case 'lehen platerak':
        return warningBg;
      case 'bigarrena':
      case 'haragiak':
      case 'arrainak':
        return brandLight;
      case 'postrea':
      case 'postreak':
        return const Color(0xFFFDF4FF);
      case 'edaria':
      case 'edariak':
        return successBg;
      default:
        return const Color(0xFFF1F5F9);
    }
  }

  static Color getKategoriaText(String kategoria) {
    switch (kategoria.toLowerCase()) {
      case 'sarrerakoa':
      case 'hastekoak':
        return info;
      case 'lehena':
      case 'lehen platerak':
        return warningText;
      case 'bigarrena':
      case 'haragiak':
      case 'arrainak':
        return brandPrimary;
      case 'postrea':
      case 'postreak':
        return const Color(0xFFC026D3);
      case 'edaria':
      case 'edariak':
        return success;
      default:
        return textMain;
    }
  }

  static ThemeData get theme => lightTheme;

  static ThemeData get lightTheme {
    return ThemeData(
      useMaterial3: true,
      scaffoldBackgroundColor: bgApp,
      colorScheme: const ColorScheme.light(
        primary: brandPrimary,
        secondary: goldDark,
        surface: bgCard,
        error: danger,
        onPrimary: Colors.white,
        onSecondary: Colors.white,
        onSurface: textMain,
        onError: Colors.white,
      ),
      fontFamily: 'Roboto',
      appBarTheme: const AppBarTheme(
        backgroundColor: brandDark,
        foregroundColor: Colors.white,
        elevation: 0,
        centerTitle: false,
        titleTextStyle: TextStyle(
          color: Colors.white,
          fontSize: 18,
          fontWeight: FontWeight.w800,
          letterSpacing: 0.8,
        ),
      ),
      bottomNavigationBarTheme: const BottomNavigationBarThemeData(
        backgroundColor: Colors.white,
        selectedItemColor: brandPrimary,
        unselectedItemColor: textMuted,
        selectedLabelStyle: TextStyle(fontWeight: FontWeight.bold, fontSize: 12),
        unselectedLabelStyle: TextStyle(fontWeight: FontWeight.w500, fontSize: 11),
        type: BottomNavigationBarType.fixed,
        elevation: 8,
      ),
      cardTheme: CardThemeData(
        color: bgCard,
        elevation: 1,
        shadowColor: const Color(0x101E293B),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(14),
          side: const BorderSide(color: border, width: 1),
        ),
      ),
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: brandPrimary,
          foregroundColor: Colors.white,
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 14),
          elevation: 2,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(10),
          ),
          textStyle: const TextStyle(
            fontSize: 15,
            fontWeight: FontWeight.w700,
            letterSpacing: 0.3,
          ),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: brandPrimary,
          side: const BorderSide(color: brandPrimary, width: 1.5),
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 13),
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(10),
          ),
          textStyle: const TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w700,
          ),
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: Colors.white,
        contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: const BorderSide(color: border, width: 1.5),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: const BorderSide(color: border, width: 1.5),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: const BorderSide(color: brandPrimary, width: 2),
        ),
      ),
    );
  }
}
