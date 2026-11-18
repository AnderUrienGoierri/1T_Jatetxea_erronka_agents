import 'package:flutter/material.dart';
import '../theme/app_theme.dart';
import 'karta_screen.dart';
import 'erreserbak_screen.dart';
import 'iritziak_screen.dart';
import 'info_screen.dart';

class HomeNavigationScreen extends StatefulWidget {
  const HomeNavigationScreen({super.key});

  @override
  State<HomeNavigationScreen> createState() => _HomeNavigationScreenState();
}

class _HomeNavigationScreenState extends State<HomeNavigationScreen> {
  int _oraingoIndizea = 0;

  final List<Widget> _pantailak = const [
    KartaScreen(),
    ErreserbakScreen(),
    IritziakScreen(),
    InfoScreen(),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: IndexedStack(
        index: _oraingoIndizea,
        children: _pantailak,
      ),
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _oraingoIndizea,
        onTap: (indizea) => setState(() => _oraingoIndizea = indizea),
        type: BottomNavigationBarType.fixed,
        backgroundColor: Colors.white,
        selectedItemColor: AppTheme.chiantiWine,
        unselectedItemColor: Colors.grey.shade500,
        selectedLabelStyle: const TextStyle(fontWeight: FontWeight.bold, fontSize: 12),
        unselectedLabelStyle: const TextStyle(fontSize: 11),
        items: const [
          BottomNavigationBarItem(
            icon: Icon(Icons.restaurant_menu),
            label: 'Karta',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.calendar_month_outlined),
            activeIcon: Icon(Icons.calendar_month),
            label: 'Erreserbak',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.star_outline),
            activeIcon: Icon(Icons.star),
            label: 'Iritziak',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.info_outline),
            activeIcon: Icon(Icons.info),
            label: 'Informazioa',
          ),
        ],
      ),
    );
  }
}
