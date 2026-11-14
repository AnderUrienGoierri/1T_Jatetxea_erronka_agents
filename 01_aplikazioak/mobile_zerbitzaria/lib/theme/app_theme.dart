import 'package:flutter/material.dart';

/// T1 Jatetxea (Italiar-Euskaldun Diseinu Sistema)
/// Chianti Ardoa, Parmigiano Urrea, Albahaka Berdea eta Terrakota
class AppTheme {
  // Kolore Nagusiak (Brand & Primary)
  static const Color brandPrimary = Color(0xFF881337); // Chianti & Ardo Gorria
  static const Color brandHover = Color(0xFF9F1239);   // Pomodoro Rustico
  static const Color brandDark = Color(0xFF4C0519);    // Erromatar Borgoina
  static const Color brandLight = Color(0xFFFFF1F2);

  // Natura eta Arrakasta (Success)
  static const Color success = Color(0xFF15803D);      // Baso & Albahaka Berdea
  static const Color successBg = Color(0xFFDCFCE7);
  static const Color successBorder = Color(0xFF86EFAC);

  // Azentuak & Labea (Warning & Gold)
  static const Color gold = Color(0xFFFCD34D);         // Parmigiano Urreztatua
  static const Color goldDark = Color(0xFFD97706);
  static const Color warning = Color(0xFFC2410C);      // Toskana Terrakota / Labea
  static const Color warningBg = Color(0xFFFEF3C7);
  static const Color warningBorder = Color(0xFFFCD34D);
  static const Color warningText = Color(0xFF92400E);

  // Barra BDS & Informazioa (Info)
  static const Color info = Color(0xFF0284C7);         // Mediterraneo / BDS Urdina
  static const Color infoBg = Color(0xFFE0F2FE);
  static const Color infoBorder = Color(0xFFBAE6FD);

  // Arrisku & Alerta (Danger)
  static const Color danger = Color(0xFFDC2626);       // Alerta Gorria
  static const Color dangerBg = Color(0xFFFEE2E2);
  static const Color dangerBorder = Color(0xFFFECACA);

  // Neutroak (Irina, Arbela & Papera)
  static const Color bgApp = Color(0xFFF8FAFC);        // Irina Txuria
  static const Color bgCard = Color(0xFFFFFFFF);       // Marmol Zuria
  static const Color border = Color(0xFFE2E8F0);
  static const Color textMain = Color(0xFF1E293B);     // Arbel Grisa
  static const Color textMuted = Color(0xFF64748B);    // Laino Grisa

  // Mahaien Egoera Koloreak
  static Color getTableStatusColor(String egoera) {
    switch (egoera.toLowerCase()) {
      case 'libre':
        return success;
      case 'okupatuta':
        return brandPrimary;
      case 'erreserbatuta':
        return warning;
      case 'garbitzen':
        return info;
      default:
        return textMuted;
    }
  }

  static Color getTableStatusBg(String egoera) {
    switch (egoera.toLowerCase()) {
      case 'libre':
        return successBg;
      case 'okupatuta':
        return brandLight;
      case 'erreserbatuta':
        return warningBg;
      case 'garbitzen':
        return infoBg;
      default:
        return border;
    }
  }

  // Kategoria koloreak
  static Color getKategoriaBg(String kategoria) {
    switch (kategoria.toLowerCase()) {
      case 'sarrerakoa':
        return infoBg;
      case 'lehena':
        return warningBg;
      case 'bigarrena':
        return brandLight;
      case 'postrea':
        return const Color(0xFFFDF4FF);
      case 'edaria':
        return successBg;
      default:
        return const Color(0xFFF1F5F9);
    }
  }

  static Color getKategoriaText(String kategoria) {
    switch (kategoria.toLowerCase()) {
      case 'sarrerakoa':
        return info;
      case 'lehena':
        return warningText;
      case 'bigarrena':
        return brandPrimary;
      case 'postrea':
        return const Color(0xFFC026D3);
      case 'edaria':
        return success;
      default:
        return textMain;
    }
  }

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
      cardTheme: CardThemeData(
        color: bgCard,
        elevation: 1,
        shadowColor: const Color(0x151E293B),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
          side: const BorderSide(color: border, width: 1),
        ),
      ),
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: brandPrimary,
          foregroundColor: Colors.white,
          padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 14),
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
