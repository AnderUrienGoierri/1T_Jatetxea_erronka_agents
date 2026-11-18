import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../models/langilea.dart';
import '../models/mahaia.dart';
import '../models/eskaera.dart';
import '../services/api_service.dart';
import '../theme/app_theme.dart';
import 'eskaera_screen.dart';
import 'ordainketa/ordainketa_modal.dart';

class MahaiaXehetasunakScreen extends StatefulWidget {
  final Mahaia mahaia;
  final Langilea langilea;

  const MahaiaXehetasunakScreen({
    super.key,
    required this.mahaia,
    required this.langilea,
  });

  @override
  State<MahaiaXehetasunakScreen> createState() => _MahaiaXehetasunakScreenState();
}

class _MahaiaXehetasunakScreenState extends State<MahaiaXehetasunakScreen> {
  List<Eskaera> _eskaerak = [];
  int _hautatutakoIndex = 0;
  bool _kargatzen = false;

  @override
  void initState() {
    super.initState();
    _kargatuEskaerak();
  }

  Future<void> _kargatuEskaerak() async {
    setState(() => _kargatzen = true);
    final zerrenda = await ApiService.getMahaiarenEskaerak(widget.mahaia.id);
    setState(() {
      _eskaerak = zerrenda;
      if (_hautatutakoIndex >= _eskaerak.length) {
        _hautatutakoIndex = _eskaerak.isNotEmpty ? 0 : 0;
      }
      _kargatzen = false;
    });
  }

  Eskaera? get _unekoEskaera {
    if (_eskaerak.isEmpty || _hautatutakoIndex >= _eskaerak.length) return null;
    return _eskaerak[_hautatutakoIndex];
  }

  Future<void> _gehituKontuBerria() async {
    int pertsonak = 1;
    final sortu = await showDialog<bool>(
      context: context,
      builder: (ctx) {
        return StatefulBuilder(
          builder: (context, setModalState) {
            return AlertDialog(
              title: const Text('Kontu Berria (Mahaikide Banatua)'),
              content: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const Text(
                    'Mahai berean beste eskaera bat irekiko da bananduta ordaintzeko.',
                    style: TextStyle(fontSize: 13, color: AppTheme.textMuted),
                  ),
                  const SizedBox(height: 14),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      IconButton.filledTonal(
                        onPressed: pertsonak > 1 ? () => setModalState(() => pertsonak--) : null,
                        icon: const Icon(Icons.remove),
                      ),
                      Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        child: Text(
                          '$pertsonak pax',
                          style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                        ),
                      ),
                      IconButton.filledTonal(
                        onPressed: () => setModalState(() => pertsonak++),
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
                  child: const Text('Sortu Kontua'),
                ),
              ],
            );
          },
        );
      },
    );

    if (sortu == true) {
      setState(() => _kargatzen = true);
      final (ok, msg, berria) = await ApiService.sortuEskaera(
        widget.mahaia.id,
        widget.langilea.id,
        pertsonak,
      );
      setState(() => _kargatzen = false);

      if (ok && berria != null) {
        await _kargatuEskaerak();
        if (_eskaerak.isNotEmpty) {
          setState(() => _hautatutakoIndex = _eskaerak.length - 1);
        }
      } else {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text(msg), backgroundColor: AppTheme.danger),
          );
        }
      }
    }
  }

  Future<void> _sukalderaBidali() async {
    final e = _unekoEskaera;
    if (e == null) return;

    setState(() => _kargatzen = true);
    final (ok, msg) = await ApiService.bidaliSukaldera(e.id);
    setState(() => _kargatzen = false);

    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(msg),
          backgroundColor: ok ? AppTheme.success : AppTheme.danger,
        ),
      );
      _kargatuEskaerak();
    }
  }

  Future<void> _gehituPlaterak() async {
    final e = _unekoEskaera;
    if (e == null) return;

    await Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => EskaeraScreen(
          eskaera: e,
          mahaia: widget.mahaia,
          langilea: widget.langilea,
        ),
      ),
    );
    _kargatuEskaerak();
  }

  Future<void> _kobratuKontua() async {
    final e = _unekoEskaera;
    if (e == null) return;

    final ordainduta = await OrdainketaModal.erakutsi(
      context,
      mahaia: widget.mahaia,
      eskaera: e,
      langilea: widget.langilea,
    );

    if (ordainduta == true) {
      await _kargatuEskaerak();
      if (_eskaerak.isEmpty && mounted) {
        Navigator.pop(context, true); // Mahaia garbi/libre geratu da
      }
    }
  }

  Future<void> _mahaiaGarbituta() async {
    final ok = await ApiService.eguneratuMahaiarenEgoera(widget.mahaia.id, 'Libre');
    if (mounted) {
      if (ok) {
        Navigator.pop(context, true);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final currency = NumberFormat.currency(locale: 'eu_ES', symbol: '€', decimalDigits: 2);
    final unekoa = _unekoEskaera;

    return Scaffold(
      appBar: AppBar(
        title: Text('Mahaia #${widget.mahaia.zenbakia} (${widget.mahaia.kokapena})'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: 'Freskatu',
            onPressed: _kargatuEskaerak,
          ),
        ],
      ),
      body: _kargatzen && _eskaerak.isEmpty
          ? const Center(child: CircularProgressIndicator())
          : Column(
              children: [
                // Kontu anitzen aukeratzailea (Sub-accounts Tabs)
                Container(
                  color: Colors.white,
                  padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                  child: Row(
                    children: [
                      Expanded(
                        child: SingleChildScrollView(
                          scrollDirection: Axis.horizontal,
                          child: Row(
                            children: List.generate(_eskaerak.length, (idx) {
                              final hautatuta = idx == _hautatutakoIndex;
                              return Padding(
                                padding: const EdgeInsets.only(right: 8),
                                child: ChoiceChip(
                                  label: Text(
                                    idx == 0 ? 'Kontua 1' : 'Kontua ${idx + 1} (Banatua)',
                                  ),
                                  selected: hautatuta,
                                  selectedColor: AppTheme.brandPrimary,
                                  labelStyle: TextStyle(
                                    color: hautatuta ? Colors.white : AppTheme.textMain,
                                    fontWeight: hautatuta ? FontWeight.bold : FontWeight.w500,
                                    fontSize: 12,
                                  ),
                                  onSelected: (_) {
                                    setState(() => _hautatutakoIndex = idx);
                                  },
                                ),
                              );
                            }),
                          ),
                        ),
                      ),
                      IconButton.filledTonal(
                        icon: const Icon(Icons.add, size: 20),
                        tooltip: 'Kontu Berria Gehitu',
                        onPressed: _gehituKontuBerria,
                      ),
                    ],
                  ),
                ),
                const Divider(height: 1, thickness: 1, color: AppTheme.border),

                // Eskaera xehetasunen txartela edo hutsa
                if (unekoa == null)
                  Expanded(
                    child: Center(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          const Icon(Icons.receipt_long, size: 54, color: AppTheme.border),
                          const SizedBox(height: 12),
                          const Text(
                            'Ez dago eskaera aktiborik mahai honetan.',
                            style: TextStyle(color: AppTheme.textMuted),
                          ),
                          const SizedBox(height: 16),
                          ElevatedButton.icon(
                            onPressed: _gehituKontuBerria,
                            icon: const Icon(Icons.add),
                            label: const Text('Eskaera Berria Sortu'),
                          ),
                          if (widget.mahaia.egoera.toLowerCase() == 'garbitzen') ...[
                            const SizedBox(height: 10),
                            OutlinedButton.icon(
                              onPressed: _mahaiaGarbituta,
                              icon: const Icon(Icons.cleaning_services),
                              label: const Text('Mahaia Libre Utzi (Garbituta)'),
                            ),
                          ],
                        ],
                      ),
                    ),
                  )
                else ...[
                  // Laburpen barra (Egoera, Zerbitzaria, Guztira)
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
                    color: AppTheme.brandLight,
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Row(
                          children: [
                            Container(
                              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                              decoration: BoxDecoration(
                                color: AppTheme.brandPrimary,
                                borderRadius: BorderRadius.circular(6),
                              ),
                              child: Text(
                                unekoa.egoera,
                                style: const TextStyle(
                                  fontSize: 11,
                                  fontWeight: FontWeight.bold,
                                  color: Colors.white,
                                ),
                              ),
                            ),
                            const SizedBox(width: 8),
                            Text(
                              '${unekoa.pertsonaKopurua} pax · #${unekoa.id}',
                              style: const TextStyle(fontSize: 12, color: AppTheme.textMuted),
                            ),
                          ],
                        ),
                        Text(
                          currency.format(unekoa.lerroak.fold(0.0, (s, l) => s + l.azpitotala)),
                          style: const TextStyle(
                            fontSize: 18,
                            fontWeight: FontWeight.w900,
                            color: AppTheme.brandPrimary,
                          ),
                        ),
                      ],
                    ),
                  ),

                  // Plateren Zerrenda
                  Expanded(
                    child: unekoa.lerroak.isEmpty
                        ? Center(
                            child: Column(
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                const Text(
                                  'Eskaera hutsik dago oraindik.',
                                  style: TextStyle(color: AppTheme.textMuted),
                                ),
                                const SizedBox(height: 12),
                                ElevatedButton.icon(
                                  onPressed: _gehituPlaterak,
                                  icon: const Icon(Icons.restaurant_menu),
                                  label: const Text('+ Komanda Berria (Platerak)'),
                                ),
                              ],
                            ),
                          )
                        : Builder(builder: (context) {
                            int lehentasuna(String egoera) {
                              final e = egoera.toLowerCase();
                              if (e.contains('egin') || e.contains('prestat')) return 0; // Eginda / Prestatua: GORAN!
                              if (e.contains('sukalde') || e.contains('martxan')) return 1;
                              if (e.contains('eskat') || e.contains('zai')) return 2;
                              if (e.contains('zerbitzat')) return 3; // Zerbitzatua: BEHERAN!
                              return 4;
                            }

                            final lerroakOrdenatuta = [...unekoa.lerroak]..sort((a, b) {
                              final pA = lehentasuna(a.egoera);
                              final pB = lehentasuna(b.egoera);
                              if (pA != pB) return pA.compareTo(pB);
                              return a.id.compareTo(b.id);
                            });

                            return ListView.separated(
                              padding: const EdgeInsets.all(12),
                              itemCount: lerroakOrdenatuta.length,
                              separatorBuilder: (_, i) => const SizedBox(height: 8),
                              itemBuilder: (ctx, index) {
                                final item = lerroakOrdenatuta[index];
                                return Container(
                                decoration: BoxDecoration(
                                  color: Colors.white,
                                  borderRadius: BorderRadius.circular(10),
                                  border: Border.all(color: AppTheme.border),
                                ),
                                padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                                child: Row(
                                  children: [
                                    Container(
                                      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                                      decoration: BoxDecoration(
                                        color: AppTheme.brandLight,
                                        borderRadius: BorderRadius.circular(6),
                                      ),
                                      child: Text(
                                        '${item.kantitatea}x',
                                        style: const TextStyle(
                                          fontWeight: FontWeight.bold,
                                          color: AppTheme.brandPrimary,
                                        ),
                                      ),
                                    ),
                                    const SizedBox(width: 10),
                                    Expanded(
                                      child: Column(
                                        crossAxisAlignment: CrossAxisAlignment.start,
                                        children: [
                                          Text(
                                            item.platerIzena,
                                            style: const TextStyle(
                                              fontSize: 14,
                                              fontWeight: FontWeight.bold,
                                              color: AppTheme.textMain,
                                            ),
                                          ),
                                          if (item.oharrak != null && item.oharrak!.isNotEmpty)
                                            Text(
                                              item.oharrak!,
                                              style: const TextStyle(
                                                fontSize: 11,
                                                fontStyle: FontStyle.italic,
                                                color: AppTheme.warningText,
                                              ),
                                            ),
                                        ],
                                      ),
                                    ),
                                    Column(
                                      crossAxisAlignment: CrossAxisAlignment.end,
                                      children: [
                                        Text(
                                          currency.format(item.azpitotala),
                                          style: const TextStyle(
                                            fontSize: 14,
                                            fontWeight: FontWeight.bold,
                                            color: AppTheme.textMain,
                                          ),
                                        ),
                                        const SizedBox(height: 4),
                                        if (item.egoera == 'Prestatua')
                                          InkWell(
                                            onTap: () async {
                                              bool ok = await ApiService.eguneratuLerroEgoera(item.id, 'Zerbitzatua');
                                              if (ok) {
                                                _kargatuEskaerak();
                                              }
                                            },
                                            child: Container(
                                              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                                              decoration: BoxDecoration(
                                                color: const Color(0xFF166534),
                                                borderRadius: BorderRadius.circular(6),
                                              ),
                                              child: const Text(
                                                'ZERBITZATU',
                                                style: TextStyle(
                                                  color: Colors.white,
                                                  fontSize: 10,
                                                  fontWeight: FontWeight.bold,
                                                ),
                                              ),
                                            ),
                                          )
                                        else
                                          Container(
                                            padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                                            decoration: BoxDecoration(
                                              color: item.egoera == 'Zerbitzatua'
                                                  ? const Color(0xFFF1F5F9)
                                                  : (item.egoera == 'Sukaldean' ? const Color(0xFFDBEAFE) : const Color(0xFFFEF3C7)),
                                              borderRadius: BorderRadius.circular(4),
                                              border: Border.all(
                                                color: item.egoera == 'Zerbitzatua'
                                                    ? const Color(0xFFCBD5E1)
                                                    : (item.egoera == 'Sukaldean' ? const Color(0xFF93C5FD) : const Color(0xFFFDE68A)),
                                              ),
                                            ),
                                            child: Text(
                                              item.egoera == 'Zerbitzatua'
                                                  ? 'ZERBITZATUA'
                                                  : (item.egoera == 'Sukaldean' ? 'MARTXAN' : 'ZAI'),
                                              style: TextStyle(
                                                color: item.egoera == 'Zerbitzatua'
                                                    ? const Color(0xFF475569)
                                                    : (item.egoera == 'Sukaldean' ? const Color(0xFF1D4ED8) : const Color(0xFFB45309)),
                                                fontSize: 10,
                                                fontWeight: FontWeight.bold,
                                              ),
                                            ),
                                          ),
                                      ],
                                    ),
                                  ],
                                ),
                              );
                            },
                          );
                        }),
                  ),

                  // Ekintza Botoi Nagusiak (Behean)
                  Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      border: Border(top: BorderSide(color: AppTheme.border.withValues(alpha: 0.8))),
                      boxShadow: const [
                        BoxShadow(
                          color: Color(0x0C000000),
                          blurRadius: 8,
                          offset: Offset(0, -3),
                        ),
                      ],
                    ),
                    child: SafeArea(
                      top: false,
                      child: Column(
                        children: [
                          Row(
                            children: [
                              Expanded(
                                child: OutlinedButton.icon(
                                  onPressed: _sukalderaBidali,
                                  icon: const Icon(Icons.soup_kitchen, size: 18),
                                  label: const Text('Sukaldera'),
                                  style: OutlinedButton.styleFrom(
                                    foregroundColor: AppTheme.warning,
                                    side: const BorderSide(color: AppTheme.warning, width: 1.5),
                                  ),
                                ),
                              ),
                              const SizedBox(width: 8),
                              Expanded(
                                child: ElevatedButton.icon(
                                  onPressed: _kobratuKontua,
                                  icon: const Icon(Icons.point_of_sale, size: 18),
                                  label: const Text('Kobratu'),
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor: AppTheme.success,
                                  ),
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 8),
                          SizedBox(
                            width: double.infinity,
                            child: ElevatedButton.icon(
                              onPressed: _gehituPlaterak,
                              icon: const Icon(Icons.add_circle_outline),
                              label: const Text('+ Komanda Berria (Gehitu Platerak)'),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                ],
              ],
            ),
    );
  }
}
