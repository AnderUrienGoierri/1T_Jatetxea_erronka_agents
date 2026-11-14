import 'package:flutter/material.dart';
import '../models/platera.dart';
import '../services/api_service.dart';
import '../services/cart_service.dart';
import '../theme/app_theme.dart';
import 'saskia_screen.dart';
import 'widgets/header_cart_button.dart';
import 'widgets/server_settings_dialog.dart';

class KartaScreen extends StatefulWidget {
  const KartaScreen({super.key});

  @override
  State<KartaScreen> createState() => _KartaScreenState();
}

class _KartaScreenState extends State<KartaScreen> {
  final CartService _cart = CartService();

  List<Platera> _platerGuztiak = [];
  List<Platera> _iragazitakoPlaterak = [];
  bool _kargatzen = true;
  String? _errorea;

  String _hautatutakoKategoria = 'Guztiak';
  final TextEditingController _bilaketaController = TextEditingController();

  final List<String> _kategoriak = [
    'Guztiak',
    'Entranteak',
    'Pastak',
    'Pizzak',
    'Haragiak',
    'Arrainak',
    'Postreak',
    'Edariak',
  ];

  @override
  void initState() {
    super.initState();
    _kargatuKarta();
    _bilaketaController.addListener(_iragazi);
  }

  @override
  void dispose() {
    _bilaketaController.dispose();
    super.dispose();
  }

  Future<void> _kargatuKarta() async {
    setState(() {
      _kargatzen = true;
      _errorea = null;
    });

    try {
      final platerak = await ApiService.getPlaterak();
      final stockMapa = await ApiService.getStockAhalmena();
      for (var p in platerak) {
        if (stockMapa.containsKey(p.id)) {
          p.stockAhalmena = stockMapa[p.id]!;
        }
      }
      setState(() {
        _platerGuztiak = platerak.where((p) => p.aktibo).toList();
        _iragazi();
        _kargatzen = false;
      });
    } catch (e) {
      setState(() {
        _errorea = e.toString();
        _kargatzen = false;
      });
    }
  }

  void _iragazi() {
    final query = _bilaketaController.text.trim().toLowerCase();
    setState(() {
      _iragazitakoPlaterak = _platerGuztiak.where((platera) {
        final kategoriaBat = _kategoriaKointziditzenDu(platera.kategoria, _hautatutakoKategoria);
        final testuBat = query.isEmpty ||
            platera.izena.toLowerCase().contains(query) ||
            (platera.deskribapena?.toLowerCase().contains(query) ?? false);
        return kategoriaBat && testuBat;
      }).toList();
    });
  }

  bool _kategoriaKointziditzenDu(String platerKategoria, String hautatua) {
    if (hautatua == 'Guztiak') return true;
    final p = platerKategoria.trim().toLowerCase();
    final h = hautatua.trim().toLowerCase();
    if (p == h) return true;
    if (h == 'entranteak') {
      return p == 'entranteak' || p == 'sarrerakoa' || p == 'sarrerakoak' || p == 'hastekoak' || p == 'lehena';
    }
    if (h == 'pastak') {
      return p == 'pastak' || p == 'pasta';
    }
    if (h == 'pizzak') {
      return p == 'pizzak' || p == 'pizza';
    }
    if (h == 'haragiak') {
      return p == 'haragiak' || p == 'haragia' || p == 'bigarrena';
    }
    if (h == 'arrainak') {
      return p == 'arrainak' || p == 'arraina';
    }
    if (h == 'postreak') {
      return p == 'postreak' || p == 'postrea';
    }
    if (h == 'edariak') {
      return p == 'edariak' || p == 'edaria';
    }
    return false;
  }

  void _erakutsiPlateraXehetasunak(Platera platera) {
    int kopurua = 1;
    final oharController = TextEditingController();

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) => StatefulBuilder(
        builder: (context, setModalState) => Container(
          padding: EdgeInsets.only(
            left: 20,
            right: 20,
            top: 20,
            bottom: MediaQuery.of(context).viewInsets.bottom + 24,
          ),
          decoration: const BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.vertical(top: Radius.circular(24)),
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Center(
                child: Container(
                  width: 48,
                  height: 4,
                  decoration: BoxDecoration(
                    color: Colors.grey.shade300,
                    borderRadius: BorderRadius.circular(2),
                  ),
                ),
              ),
              const SizedBox(height: 16),
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                          decoration: BoxDecoration(
                            color: AppTheme.chiantiWine.withValues(alpha: 0.1),
                            borderRadius: BorderRadius.circular(6),
                          ),
                          child: Text(
                            platera.kategoria.toUpperCase(),
                            style: const TextStyle(
                              fontSize: 11,
                              fontWeight: FontWeight.bold,
                              color: AppTheme.chiantiWine,
                            ),
                          ),
                        ),
                        const SizedBox(height: 6),
                        Text(
                          platera.izena,
                          style: const TextStyle(
                            fontSize: 20,
                            fontWeight: FontWeight.bold,
                            color: AppTheme.darkCharcoal,
                          ),
                        ),
                      ],
                    ),
                  ),
                  Text(
                    '${platera.prezioa.toStringAsFixed(2)} EUR',
                    style: const TextStyle(
                      fontSize: 22,
                      fontWeight: FontWeight.w800,
                      color: AppTheme.chiantiWine,
                    ),
                  ),
                ],
              ),
              if (platera.deskribapena != null && platera.deskribapena!.isNotEmpty) ...[
                const SizedBox(height: 12),
                Text(
                  platera.deskribapena!,
                  style: TextStyle(
                    fontSize: 14,
                    color: Colors.grey.shade700,
                    height: 1.4,
                  ),
                ),
              ],
              const SizedBox(height: 14),
              Wrap(
                spacing: 12,
                runSpacing: 8,
                children: [
                  Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(Icons.schedule, size: 16, color: Colors.grey.shade600),
                      const SizedBox(width: 4),
                      Text(
                        '~${platera.prestatzeDenboraMin} min',
                        style: TextStyle(fontSize: 12, color: Colors.grey.shade700),
                      ),
                    ],
                  ),
                  if (platera.alergenoak != null && platera.alergenoak!.isNotEmpty)
                    Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        const Icon(Icons.warning_amber_rounded, size: 16, color: AppTheme.terracotta),
                        const SizedBox(width: 4),
                        Text(
                          'Alergenoak: ${platera.alergenoak}',
                          style: const TextStyle(fontSize: 12, color: AppTheme.terracotta),
                        ),
                      ],
                    ),
                ],
              ),
              const Divider(height: 28),
              const Text(
                'Oharrak sukalderako (hautazkoa)',
                style: TextStyle(
                  fontSize: 13,
                  fontWeight: FontWeight.w600,
                  color: AppTheme.darkCharcoal,
                ),
              ),
              const SizedBox(height: 6),
              TextField(
                controller: oharController,
                decoration: InputDecoration(
                  hintText: 'Adib.: Gazta gabe, puntuan, gluten intolerantea...',
                  hintStyle: TextStyle(fontSize: 13, color: Colors.grey.shade400),
                  contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
                  filled: true,
                  fillColor: AppTheme.warmBackground,
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(10),
                    borderSide: BorderSide(color: Colors.grey.shade300),
                  ),
                ),
              ),
              const SizedBox(height: 18),
              Row(
                children: [
                  Container(
                    decoration: BoxDecoration(
                      border: Border.all(color: Colors.grey.shade300),
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: Row(
                      children: [
                        IconButton(
                          icon: const Icon(Icons.remove, size: 18),
                          onPressed: kopurua > 1
                              ? () => setModalState(() => kopurua--)
                              : null,
                        ),
                        Text(
                          '$kopurua',
                          style: const TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        IconButton(
                          icon: const Icon(Icons.add, size: 18),
                          onPressed: () => setModalState(() => kopurua++),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(width: 14),
                  Expanded(
                    child: ElevatedButton(
                      onPressed: () {
                        _cart.add(
                          platera,
                          kopurua: kopurua,
                          oharrak: oharController.text.trim(),
                        );
                        Navigator.pop(ctx);
                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(
                            backgroundColor: AppTheme.darkCharcoal,
                            content: Text(
                              '${platera.izena} saskira gehitu da ($kopurua)',
                              style: const TextStyle(color: Colors.white),
                            ),
                            action: SnackBarAction(
                              label: 'SASKIA',
                              textColor: AppTheme.parmigianoGold,
                              onPressed: () {
                                Navigator.push(
                                  context,
                                  MaterialPageRoute(builder: (_) => const SaskiaScreen()),
                                );
                              },
                            ),
                          ),
                        );
                      },
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppTheme.chiantiWine,
                        padding: const EdgeInsets.symmetric(vertical: 14),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(10),
                        ),
                      ),
                      child: Text(
                        'Gehitu (${(platera.prezioa * kopurua).toStringAsFixed(2)} EUR)',
                        style: const TextStyle(
                          fontSize: 15,
                          fontWeight: FontWeight.bold,
                          color: Colors.white,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.warmBackground,
      appBar: AppBar(
        backgroundColor: AppTheme.chiantiWine,
        elevation: 0,
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: const [
            Text(
              'T1 JATETXEA',
              style: TextStyle(
                fontSize: 17,
                fontWeight: FontWeight.w800,
                letterSpacing: 1.2,
                color: Colors.white,
              ),
            ),
            Text(
              'Osteria & Parrilla · Karta Digitala',
              style: TextStyle(
                fontSize: 11,
                color: AppTheme.parmigianoGold,
                letterSpacing: 0.5,
              ),
            ),
          ],
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.settings_outlined, color: Colors.white, size: 20),
            tooltip: 'API Zerbitzaria',
            padding: const EdgeInsets.all(8),
            constraints: const BoxConstraints(),
            onPressed: () async {
              final aldatuDa = await showDialog<bool>(
                context: context,
                builder: (_) => const ServerSettingsDialog(),
              );
              if (aldatuDa == true) {
                _kargatuKarta();
              }
            },
          ),
          const HeaderCartButton(),
          const SizedBox(width: 6),
        ],
      ),
      body: Column(
        children: [
          // Goiburu Bilaketa barra
          Container(
            color: AppTheme.chiantiWine,
            padding: const EdgeInsets.fromLTRB(16, 0, 16, 14),
            child: TextField(
              controller: _bilaketaController,
              style: const TextStyle(color: Colors.white, fontSize: 14),
              decoration: InputDecoration(
                hintText: 'Bilatu platera, pasta, haragia edo osagaiak...',
                hintStyle: TextStyle(color: Colors.white.withValues(alpha: 0.6), fontSize: 13),
                prefixIcon: const Icon(Icons.search, color: AppTheme.parmigianoGold, size: 20),
                suffixIcon: _bilaketaController.text.isNotEmpty
                    ? IconButton(
                        icon: const Icon(Icons.clear, color: Colors.white70, size: 18),
                        onPressed: () => _bilaketaController.clear(),
                      )
                    : null,
                filled: true,
                fillColor: Colors.white.withValues(alpha: 0.15),
                contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: BorderSide.none,
                ),
              ),
            ),
          ),

          // Kategoria fitxak
          Container(
            height: 48,
            color: Colors.white,
            child: ListView.separated(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              itemCount: _kategoriak.length,
              separatorBuilder: (_, _) => const SizedBox(width: 8),
              itemBuilder: (context, index) {
                final kategoria = _kategoriak[index];
                final hautatua = _hautatutakoKategoria == kategoria;
                return ChoiceChip(
                  label: Text(kategoria),
                  selected: hautatua,
                  onSelected: (val) {
                    if (val) {
                      setState(() {
                        _hautatutakoKategoria = kategoria;
                        _iragazi();
                      });
                    }
                  },
                  selectedColor: AppTheme.chiantiWine,
                  labelStyle: TextStyle(
                    fontSize: 12,
                    fontWeight: hautatua ? FontWeight.bold : FontWeight.w500,
                    color: hautatua ? Colors.white : AppTheme.darkCharcoal,
                  ),
                  backgroundColor: AppTheme.warmBackground,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(18),
                    side: BorderSide(
                      color: hautatua ? AppTheme.chiantiWine : Colors.grey.shade300,
                    ),
                  ),
                  showCheckmark: false,
                );
              },
            ),
          ),

          // Plateren zerrenda
          Expanded(
            child: _kargatzen
                ? const Center(
                    child: CircularProgressIndicator(color: AppTheme.chiantiWine),
                  )
                : _errorea != null
                    ? Center(
                        child: Padding(
                          padding: const EdgeInsets.all(24),
                          child: Column(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              const Icon(Icons.wifi_off_rounded, size: 48, color: AppTheme.terracotta),
                              const SizedBox(height: 12),
                              const Text(
                                'Ezin izan dira platerak kargatu',
                                style: TextStyle(
                                  fontSize: 16,
                                  fontWeight: FontWeight.bold,
                                  color: AppTheme.darkCharcoal,
                                ),
                              ),
                              const SizedBox(height: 6),
                              Text(
                                _errorea!,
                                textAlign: TextAlign.center,
                                style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
                              ),
                              const SizedBox(height: 16),
                              ElevatedButton(
                                onPressed: _kargatuKarta,
                                style: ElevatedButton.styleFrom(
                                  backgroundColor: AppTheme.chiantiWine,
                                ),
                                child: const Text('Saiatu berriro', style: TextStyle(color: Colors.white)),
                              ),
                            ],
                          ),
                        ),
                      )
                    : _iragazitakoPlaterak.isEmpty
                        ? Center(
                            child: Column(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Icon(Icons.restaurant_menu, size: 48, color: Colors.grey.shade400),
                                const SizedBox(height: 12),
                                Text(
                                  'Ez da platerik aurkitu iragazki honekin',
                                  style: TextStyle(fontSize: 14, color: Colors.grey.shade600),
                                ),
                              ],
                            ),
                          )
                        : RefreshIndicator(
                            color: AppTheme.chiantiWine,
                            onRefresh: _kargatuKarta,
                            child: ListView.separated(
                              padding: const EdgeInsets.all(16),
                              itemCount: _iragazitakoPlaterak.length,
                              separatorBuilder: (_, _) => const SizedBox(height: 12),
                              itemBuilder: (context, index) {
                                final platera = _iragazitakoPlaterak[index];
                                return _buildPlaterCard(platera);
                              },
                            ),
                          ),
          ),
        ],
      ),
    );
  }

  Widget _buildPlaterCard(Platera platera) {
    final agortua = platera.stockAhalmena <= 0;

    return Card(
      elevation: 1,
      color: Colors.white,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(14),
        side: BorderSide(color: Colors.grey.shade200),
      ),
      child: InkWell(
        borderRadius: BorderRadius.circular(14),
        onTap: agortua ? null : () => _erakutsiPlateraXehetasunak(platera),
        child: Padding(
          padding: const EdgeInsets.all(14),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                          decoration: BoxDecoration(
                            color: AppTheme.chiantiWine.withValues(alpha: 0.08),
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Text(
                            platera.kategoria,
                            style: const TextStyle(
                              fontSize: 10,
                              fontWeight: FontWeight.w600,
                              color: AppTheme.chiantiWine,
                            ),
                          ),
                        ),
                        if (agortua) ...[
                          const SizedBox(width: 6),
                          Container(
                            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                            decoration: BoxDecoration(
                              color: Colors.red.shade100,
                              borderRadius: BorderRadius.circular(4),
                            ),
                            child: const Text(
                              'AGORTUA',
                              style: TextStyle(
                                fontSize: 10,
                                fontWeight: FontWeight.bold,
                                color: Colors.red,
                              ),
                            ),
                          ),
                        ],
                      ],
                    ),
                    const SizedBox(height: 6),
                    Text(
                      platera.izena,
                      style: TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.bold,
                        color: agortua ? Colors.grey : AppTheme.darkCharcoal,
                      ),
                    ),
                    if (platera.deskribapena != null && platera.deskribapena!.isNotEmpty) ...[
                      const SizedBox(height: 4),
                      Text(
                        platera.deskribapena!,
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: TextStyle(
                          fontSize: 12,
                          color: Colors.grey.shade600,
                          height: 1.3,
                        ),
                      ),
                    ],
                    const SizedBox(height: 8),
                    Row(
                      children: [
                        Icon(Icons.schedule, size: 14, color: Colors.grey.shade500),
                        const SizedBox(width: 4),
                        Text(
                          '${platera.prestatzeDenboraMin} min',
                          style: TextStyle(fontSize: 11, color: Colors.grey.shade600),
                        ),
                        if (platera.alergenoak != null && platera.alergenoak!.isNotEmpty) ...[
                          const SizedBox(width: 12),
                          const Icon(Icons.info_outline, size: 14, color: AppTheme.terracotta),
                          const SizedBox(width: 4),
                          Expanded(
                            child: Text(
                              platera.alergenoak!,
                              maxLines: 1,
                              overflow: TextOverflow.ellipsis,
                              style: const TextStyle(fontSize: 11, color: AppTheme.terracotta),
                            ),
                          ),
                        ],
                      ],
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 12),
              Column(
                crossAxisAlignment: CrossAxisAlignment.end,
                children: [
                  Text(
                    '${platera.prezioa.toStringAsFixed(2)} EUR',
                    style: TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.bold,
                      color: agortua ? Colors.grey : AppTheme.chiantiWine,
                    ),
                  ),
                  const SizedBox(height: 10),
                  FilledButton(
                    onPressed: agortua ? null : () => _erakutsiPlateraXehetasunak(platera),
                    style: FilledButton.styleFrom(
                      backgroundColor: AppTheme.chiantiWine,
                      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                      minimumSize: const Size(0, 32),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(8),
                      ),
                    ),
                    child: const Text(
                      'Aukeratu',
                      style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
