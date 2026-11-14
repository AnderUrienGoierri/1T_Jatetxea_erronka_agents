import 'package:flutter/material.dart';
import '../models/langilea.dart';
import '../models/mahaia.dart';
import '../services/api_service.dart';
import '../theme/app_theme.dart';
import 'login_screen.dart';
import 'mahaia_xehetasunak_screen.dart';
import 'eskaera_screen.dart';

class MahaiakScreen extends StatefulWidget {
  final Langilea langilea;

  const MahaiakScreen({super.key, required this.langilea});

  @override
  State<MahaiakScreen> createState() => _MahaiakScreenState();
}

class _MahaiakScreenState extends State<MahaiakScreen> {
  List<Mahaia> _mahaiak = [];
  bool _kargatzen = false;
  String _hautatutakoGunea = 'Guztiak';

  static const List<String> _guneak = [
    'Guztiak',
    'Jangela Nagusia',
    'Terraza',
    'Barra',
    'VIP',
  ];

  @override
  void initState() {
    super.initState();
    _kargatuMahaiak();
  }

  Future<void> _kargatuMahaiak() async {
    setState(() => _kargatzen = true);
    final zerrenda = await ApiService.getMahaiak();
    setState(() {
      _mahaiak = zerrenda;
      _kargatzen = false;
    });
  }

  List<Mahaia> get _iragazitakoMahaiak {
    if (_hautatutakoGunea == 'Guztiak') return _mahaiak;
    return _mahaiak.where((m) => m.kokapena.toLowerCase() == _hautatutakoGunea.toLowerCase()).toList();
  }

  Future<void> _mahaiaKlikatu(Mahaia mahaia) async {
    if (mahaia.egoera.toLowerCase() == 'libre') {
      // Pertsona kopurua galdetu eta eskaera berria sortu
      _irekiMahaiaModal(mahaia);
    } else {
      // Okupatuta, Erreserbatuta edo Garbitzen: Xehetasunen pantailara joan
      final aldatuDa = await Navigator.push<bool>(
        context,
        MaterialPageRoute(
          builder: (_) => MahaiaXehetasunakScreen(
            mahaia: mahaia,
            langilea: widget.langilea,
          ),
        ),
      );
      if (aldatuDa == true) {
        _kargatuMahaiak();
      }
    }
  }

  Future<void> _irekiMahaiaModal(Mahaia mahaia) async {
    int pertsonak = 2;

    final sortu = await showDialog<bool>(
      context: context,
      builder: (ctx) {
        return StatefulBuilder(
          builder: (context, setModalState) {
            return AlertDialog(
              title: Row(
                children: [
                  const Icon(Icons.table_restaurant, color: AppTheme.brandPrimary),
                  const SizedBox(width: 8),
                  Text('Mahaia #${mahaia.zenbakia} Ireki'),
                ],
              ),
              content: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Kokapena: ${mahaia.kokapena} (Geh. ${mahaia.edukiera} lagun)',
                    style: const TextStyle(fontSize: 13, color: AppTheme.textMuted),
                  ),
                  const SizedBox(height: 16),
                  const Text(
                    'Mahaikide Kopurua:',
                    style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 10),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      IconButton.filledTonal(
                        onPressed: pertsonak > 1
                            ? () => setModalState(() => pertsonak--)
                            : null,
                        icon: const Icon(Icons.remove),
                      ),
                      Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 20),
                        child: Text(
                          '$pertsonak',
                          style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
                        ),
                      ),
                      IconButton.filledTonal(
                        onPressed: pertsonak < 20
                            ? () => setModalState(() => pertsonak++)
                            : null,
                        icon: const Icon(Icons.add),
                      ),
                    ],
                  ),
                ],
              ),
              actions: [
                TextButton(
                  onPressed: () => Navigator.pop(ctx, false),
                  child: const Text('Utzi'),
                ),
                ElevatedButton(
                  onPressed: () => Navigator.pop(ctx, true),
                  child: const Text('Komandara Joan'),
                ),
              ],
            );
          },
        );
      },
    );

    if (sortu == true) {
      // Eskaera sortu APIan
      setState(() => _kargatzen = true);
      final (ok, msg, eskaeraBerria) = await ApiService.sortuEskaera(
        mahaia.id,
        widget.langilea.id,
        pertsonak,
      );
      setState(() => _kargatzen = false);

      if (ok && eskaeraBerria != null) {
        if (mounted) {
          // Komanda pantailara nabigatu
          await Navigator.push(
            context,
            MaterialPageRoute(
              builder: (_) => EskaeraScreen(
                eskaera: eskaeraBerria,
                mahaia: mahaia,
                langilea: widget.langilea,
              ),
            ),
          );
          _kargatuMahaiak();
        }
      } else {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(msg),
              backgroundColor: AppTheme.danger,
            ),
          );
        }
      }
    }
  }

  void _saioaItxi() {
    Navigator.pushReplacement(
      context,
      MaterialPageRoute(builder: (_) => const LoginScreen()),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text('T1 JATETXEA'),
            Text(
              'Zerbitzaria: ${widget.langilea.izena}',
              style: const TextStyle(fontSize: 12, color: AppTheme.gold, fontWeight: FontWeight.w400),
            ),
          ],
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: 'Freskatu',
            onPressed: _kargatuMahaiak,
          ),
          IconButton(
            icon: const Icon(Icons.logout),
            tooltip: 'Saioa Itxi',
            onPressed: _saioaItxi,
          ),
        ],
      ),
      body: Column(
        children: [
          // Guneen Fitxak (Tabs)
          Container(
            color: Colors.white,
            padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 12),
            child: SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: Row(
                children: _guneak.map((gune) {
                  final hautatuta = _hautatutakoGunea == gune;
                  return Padding(
                    padding: const EdgeInsets.only(right: 6),
                    child: ChoiceChip(
                      label: Text(gune),
                      selected: hautatuta,
                      selectedColor: AppTheme.brandPrimary,
                      labelStyle: TextStyle(
                        color: hautatuta ? Colors.white : AppTheme.textMain,
                        fontWeight: hautatuta ? FontWeight.bold : FontWeight.w500,
                        fontSize: 13,
                      ),
                      side: BorderSide(
                        color: hautatuta ? AppTheme.brandDark : AppTheme.border,
                      ),
                      onSelected: (_) {
                        setState(() => _hautatutakoGunea = gune);
                      },
                    ),
                  );
                }).toList(),
              ),
            ),
          ),
          const Divider(height: 1, thickness: 1, color: AppTheme.border),
          // Mahaien Sareta
          Expanded(
            child: _kargatzen && _mahaiak.isEmpty
                ? const Center(child: CircularProgressIndicator())
                : _iragazitakoMahaiak.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            const Icon(Icons.table_restaurant, size: 48, color: AppTheme.border),
                            const SizedBox(height: 12),
                            Text(
                              'Ez dago mahairik gune honetan ($_hautatutakoGunea)',
                              style: const TextStyle(color: AppTheme.textMuted),
                            ),
                          ],
                        ),
                      )
                    : RefreshIndicator(
                        onRefresh: _kargatuMahaiak,
                        child: GridView.builder(
                          padding: const EdgeInsets.all(14),
                          gridDelegate: const SliverGridDelegateWithMaxCrossAxisExtent(
                            maxCrossAxisExtent: 220,
                            mainAxisExtent: 140,
                            crossAxisSpacing: 12,
                            mainAxisSpacing: 12,
                          ),
                          itemCount: _iragazitakoMahaiak.length,
                          itemBuilder: (ctx, index) {
                            final m = _iragazitakoMahaiak[index];
                            return _buildMahaiaCard(m);
                          },
                        ),
                      ),
          ),
        ],
      ),
    );
  }

  Widget _buildMahaiaCard(Mahaia mahaia) {
    final statusColor = AppTheme.getTableStatusColor(mahaia.egoera);
    final statusBg = AppTheme.getTableStatusBg(mahaia.egoera);

    return InkWell(
      onTap: () => _mahaiaKlikatu(mahaia),
      borderRadius: BorderRadius.circular(12),
      child: Container(
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: statusColor.withValues(alpha: 0.4),
            width: 1.5,
          ),
          boxShadow: const [
            BoxShadow(
              color: Color(0x0C000000),
              blurRadius: 6,
              offset: Offset(0, 2),
            ),
          ],
        ),
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Mahaia ${mahaia.zenbakia}',
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w900,
                    color: AppTheme.textMain,
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                  decoration: BoxDecoration(
                    color: statusBg,
                    borderRadius: BorderRadius.circular(999),
                    border: Border.all(color: statusColor.withValues(alpha: 0.6)),
                  ),
                  child: Text(
                    mahaia.egoera,
                    style: TextStyle(
                      fontSize: 10,
                      fontWeight: FontWeight.bold,
                      color: statusColor,
                    ),
                  ),
                ),
              ],
            ),
            Row(
              children: [
                const Icon(Icons.people_outline, size: 14, color: AppTheme.textMuted),
                const SizedBox(width: 4),
                Text(
                  '${mahaia.edukiera} pax',
                  style: const TextStyle(fontSize: 12, color: AppTheme.textMuted),
                ),
                const Spacer(),
                Text(
                  mahaia.kokapena,
                  style: const TextStyle(fontSize: 11, color: AppTheme.textMuted),
                ),
              ],
            ),
            Container(
              width: double.infinity,
              padding: const EdgeInsets.symmetric(vertical: 4),
              decoration: BoxDecoration(
                color: statusBg.withValues(alpha: 0.5),
                borderRadius: BorderRadius.circular(6),
              ),
              child: Text(
                mahaia.egoera.toLowerCase() == 'libre'
                    ? '+ Komanda Berria'
                    : 'Ikusi Kontuak / Gehitu',
                style: TextStyle(
                  fontSize: 11,
                  fontWeight: FontWeight.bold,
                  color: statusColor,
                ),
                textAlign: TextAlign.center,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
