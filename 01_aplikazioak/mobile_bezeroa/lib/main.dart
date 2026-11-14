import 'package:flutter/material.dart';
import 'services/api_service.dart';
import 'theme/app_theme.dart';
import 'screens/home_navigation_screen.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await ApiService.init();
  runApp(const JatetxeaBezeroaApp());
}

class JatetxeaBezeroaApp extends StatelessWidget {
  const JatetxeaBezeroaApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'T1 Jatetxea · Osteria & Parrilla',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.theme,
      home: const HomeNavigationScreen(),
    );
  }
}
