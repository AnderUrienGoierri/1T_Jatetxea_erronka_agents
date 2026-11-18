import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../models/langilea.dart';
import '../models/mahaia.dart';
import '../models/platera.dart';
import '../models/eskaera.dart';
import '../services/api_service.dart';
import '../theme/app_theme.dart';
import 'widgets/oharra_modal.dart';

class EskaeraScreen extends StatefulWidget {
  final Eskaera eskaera;
  final Mahaia mahaia;
  final Langilea langilea;

  const EskaeraScreen({
    super.key,
    required this.eskaera,
    required this.mahaia,
    required this.langilea,
  });

  @override
  State<EskaeraScreen> createState() => _EskaeraScreenState();
}

class _EskaeraScreenState extends State<EskaeraScreen> {
  List<Platera> _platerak = [];
  Map<int, int> _stockMap = {};
  String _hautatutakoKategoria = 'Guztiak';
  final List<CartItem> _saskia = [];
  bool _kargatzen = false;
  bool _gordetzen = false;

  static const List<String> _kategoriak = [
    'Guztiak',
    'Edariak',
    'Hastekoak',
    'Pastak',
    'Pizzak',
    'Haragiak',
    'Arrainak',
    'Postreak',
  ];

  @override
  void initState() {
    super.initState();
    _kargatuPlaterak();
  }

  Future<void> _kargatuPlaterak() async {
    setState(() => _kargatzen = true);
    final platerak = await ApiService.getPlaterak();
    final stockak = await ApiService.getStockAhalmena();
    setState(() {
      _platerak = platerak;
      _stockMap = stockak;
      _kargatzen = false;
    });
  }

  List<Platera> get _iragazitakoPlaterak {
    if (_hautatutakoKategoria == 'Guztiak') return _platerak;
    return _platerak.where((p) {
      final kat = p.kategoria.toLowerCase();
      final sel = _hautatutakoKategoria.toLowerCase();
      // Bateragarritasuna: Edaria -> Edariak, Sarrerakoa -> Hastekoak, etab.
      if (sel == 'edariak') return kat.contains('edari');
      if (sel == 'hastekoak') return kat.contains('haste') || kat.contains('sarrera') || kat.contains('lehena') || kat.contains('entrante');
      if (sel == 'pastak') return kat.contains('pasta');
      if (sel == 'pizzak') return kat.contains('pizza');
      if (sel == 'haragiak') return kat.contains('haragi') || kat == 'bigarrena';
      if (sel == 'arrainak') return kat.contains('arrain');
      if (sel == 'postreak') return kat.contains('postre');
      return kat == sel;
    }).toList();
  }

  void _gehituSaskira(Platera platera) {
    setState(() {
      final index = _saskia.indexWhere((item) => item.platera.id == platera.id && item.oharrak.isEmpty);
      if (index >= 0) {
        _saskia[index].kopurua++;
      } else {
        _saskia.add(CartItem(platera: platera));
      }
    });

    ScaffoldMessenger.of(context).removeCurrentSnackBar();
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('${platera.izena} komandara gehitu da.'),
        duration: const Duration(milliseconds: 900),
        behavior: SnackBarBehavior.floating,
      ),
    );
  }

  double get _saskiaGuztira {
    return _saskia.fold(0.0, (sum, item) => sum + item.azpitotala);
  }

  int get _saskiaKopurua {
    return _saskia.fold(0, (sum, item) => sum + item.kopurua);
  }

  Future<void> _gordeKomanda({bool sukalderaBidali = true}) async {
    if (_saskia.isEmpty) return;

    setState(() => _gordetzen = true);

    bool denaOndo = true;
    for (var item in _saskia) {
      final (ok, _) = await ApiService.gehituPlateraEskaerara(
        widget.eskaera.id,
        item.platera.id,
        item.kopurua,
        item.oharrak.isNotEmpty ? item.oharrak : null,
      );
      if (!ok) denaOndo = false;
    }

    if (denaOndo && sukalderaBidali) {
      await ApiService.bidaliSukaldera(widget.eskaera.id);
    }

    setState(() => _gordetzen = false);

    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            denaOndo
                ? (sukalderaBidali ? 'Komanda gorde eta sukaldera bidali da!' : 'Komanda gorde da.')
                : 'Errorea plater batzuk gehitzean.',
          ),
          backgroundColor: denaOndo ? AppTheme.success : AppTheme.danger,
        ),
      );
      Navigator.pop(context, true);
    }
  }

  void _irekiSaskiaModal() {
    final currency = NumberFormat.currency(locale: 'eu_ES', symbol: '€', decimalDigits: 2);

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) {
        return StatefulBuilder(
          builder: (context, setModalState) {
            return Container(
              height: MediaQuery.of(context).size.height * 0.78,
              decoration: const BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
              ),
              padding: const EdgeInsets.fromLTRB(16, 12, 16, 16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Center(
                    child: Container(
                      width: 40,
                      height: 4,
                      decoration: BoxDecoration(
                        color: AppTheme.border,
                        borderRadius: BorderRadius.circular(2),
                      ),
                    ),
                  ),
                  const SizedBox(height: 12),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'Komanda: Mahaia #${widget.mahaia.zenbakia}',
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.w900,
                          color: AppTheme.brandPrimary,
                        ),
                      ),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                        decoration: BoxDecoration(
                          color: AppTheme.brandLight,
                          borderRadius: BorderRadius.circular(999),
                        ),
                        child: Text(
                          '$_saskiaKopurua plater',
                          style: const TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.bold,
                            color: AppTheme.brandPrimary,
                          ),
                        ),
                      ),
                    ],
                  ),
                  const Divider(height: 20),
                  Expanded(
                    child: _saskia.isEmpty
                        ? const Center(
                            child: Text(
                              'Ez duzu platerik hautatu oraindik.',
                              style: TextStyle(color: AppTheme.textMuted),
                            ),
                          )
                        : ListView.separated(
                            itemCount: _saskia.length,
                            separatorBuilder: (_, i) => const Divider(height: 1),
                            itemBuilder: (context, index) {
                              final item = _saskia[index];
                              return Padding(
                                padding: const EdgeInsets.symmetric(vertical: 8),
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Row(
                                      children: [
                                        Expanded(
                                          child: Text(
                                            item.platera.izena,
                                            style: const TextStyle(
                                              fontWeight: FontWeight.bold,
                                              fontSize: 14,
                                            ),
                                          ),
                                        ),
                                        Text(
                                          currency.format(item.azpitotala),
                                          style: const TextStyle(
                                            fontWeight: FontWeight.w800,
                                            fontSize: 14,
                                            color: AppTheme.brandPrimary,
                                          ),
                                        ),
                                      ],
                                    ),
                                    const SizedBox(height: 6),
                                    Row(
                                      children: [
                                        // Kopuru aldatzailea
                                        Container(
                                          decoration: BoxDecoration(
                                            border: Border.all(color: AppTheme.border),
                                            borderRadius: BorderRadius.circular(8),
                                          ),
                                          child: Row(
                                            mainAxisSize: MainAxisSize.min,
                                            children: [
                                              InkWell(
                                                onTap: () {
                                                  setModalState(() {
                                                    if (item.kopurua > 1) {
                                                      item.kopurua--;
                                                    } else {
                                                      _saskia.removeAt(index);
                                                    }
                                                  });
                                                  setState(() {});
                                                },
                                                child: const Padding(
                                                  padding: EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                                                  child: Icon(Icons.remove, size: 16),
                                                ),
                                              ),
                                              Padding(
                                                padding: const EdgeInsets.symmetric(horizontal: 8),
                                                child: Text(
                                                  '${item.kopurua}',
                                                  style: const TextStyle(fontWeight: FontWeight.bold),
                                                ),
                                              ),
                                              InkWell(
                                                onTap: () {
                                                  setModalState(() => item.kopurua++);
                                                  setState(() {});
                                                },
                                                child: const Padding(
                                                  padding: EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                                                  child: Icon(Icons.add, size: 16),
                                                ),
                                              ),
                                            ],
                                          ),
                                        ),
                                        const SizedBox(width: 8),
                                        // Oharra Botoia
                                        Expanded(
                                          child: InkWell(
                                            onTap: () async {
                                              final oharra = await OharraModal.show(
                                                context,
                                                item.platera.izena,
                                                item.oharrak,
                                              );
                                              if (oharra != null) {
                                                setModalState(() => item.oharrak = oharra);
                                                setState(() {});
                                              }
                                            },
                                            child: Container(
                                              padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 6),
                                              decoration: BoxDecoration(
                                                color: item.oharrak.isNotEmpty
                                                    ? AppTheme.warningBg
                                                    : const Color(0xFFF1F5F9),
                                                borderRadius: BorderRadius.circular(8),
                                                border: Border.all(
                                                  color: item.oharrak.isNotEmpty
                                                      ? AppTheme.warning
                                                      : AppTheme.border,
                                                ),
                                              ),
                                              child: Row(
                                                children: [
                                                  Icon(
                                                    Icons.note_alt_outlined,
                                                    size: 14,
                                                    color: item.oharrak.isNotEmpty
                                                        ? AppTheme.warningText
                                                        : AppTheme.textMuted,
                                                  ),
                                                  const SizedBox(width: 4),
                                                  Expanded(
                                                    child: Text(
                                                      item.oharrak.isNotEmpty ? item.oharrak : '+ Oharra',
                                                      style: TextStyle(
                                                        fontSize: 11,
                                                        color: item.oharrak.isNotEmpty
                                                            ? AppTheme.warningText
                                                            : AppTheme.textMuted,
                                                        fontWeight: item.oharrak.isNotEmpty
                                                            ? FontWeight.bold
                                                            : FontWeight.normal,
                                                      ),
                                                      maxLines: 1,
                                                      overflow: TextOverflow.ellipsis,
                                                    ),
                                                  ),
                                                ],
                                              ),
                                            ),
                                          ),
                                        ),
                                      ],
                                    ),
                                  ],
                                ),
                              );
                            },
                          ),
                  ),
                  const Divider(height: 20),
                  // Totala eta Bidali botoiak
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text(
                        'GUZTIRA:',
                        style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                      ),
                      Text(
                        currency.format(_saskiaGuztira),
                        style: const TextStyle(
                          fontSize: 20,
                          fontWeight: FontWeight.w900,
                          color: AppTheme.brandPrimary,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 12),
                  ElevatedButton.icon(
                    onPressed: _saskia.isEmpty || _gordetzen
                        ? null
                        : () {
                            Navigator.pop(ctx);
                            _gordeKomanda(sukalderaBidali: true);
                          },
                    icon: _gordetzen
                        ? const SizedBox(
                            width: 16,
                            height: 16,
                            child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                          )
                        : const Icon(Icons.send),
                    label: const Text('GORDE ETA SUKALDERA BIDALI'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppTheme.brandPrimary,
                      padding: const EdgeInsets.symmetric(vertical: 14),
                    ),
                  ),
                ],
              ),
            );
          },
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final currency = NumberFormat.currency(locale: 'eu_ES', symbol: '€', decimalDigits: 2);

    return Scaffold(
      appBar: AppBar(
        title: Text('Komanda · Mahaia #${widget.mahaia.zenbakia}'),
        actions: [
          IconButton(
            icon: Badge(
              isLabelVisible: _saskia.isNotEmpty,
              label: Text('$_saskiaKopurua'),
              child: const Icon(Icons.shopping_cart),
            ),
            tooltip: 'Ikusi Komanda',
            onPressed: _irekiSaskiaModal,
          ),
        ],
      ),
      body: Column(
        children: [
          // Kategoria Filtroak
          Container(
            color: Colors.white,
            padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 12),
            child: SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: Row(
                children: _kategoriak.map((kat) {
                  final hautatuta = _hautatutakoKategoria == kat;
                  return Padding(
                    padding: const EdgeInsets.only(right: 6),
                    child: ChoiceChip(
                      label: Text(kat),
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
                        setState(() => _hautatutakoKategoria = kat);
                      },
                    ),
                  );
                }).toList(),
              ),
            ),
          ),
          const Divider(height: 1, thickness: 1, color: AppTheme.border),

          // Plateren Zerrenda
          Expanded(
            child: _kargatzen && _platerak.isEmpty
                ? const Center(child: CircularProgressIndicator())
                : _iragazitakoPlaterak.isEmpty
                    ? Center(
                        child: Text(
                          'Ez dago platerik kategoria honetan ($_hautatutakoKategoria)',
                          style: const TextStyle(color: AppTheme.textMuted),
                        ),
                      )
                    : ListView.builder(
                        padding: const EdgeInsets.fromLTRB(12, 12, 12, 80),
                        itemCount: _iragazitakoPlaterak.length,
                        itemBuilder: (ctx, index) {
                          final platera = _iragazitakoPlaterak[index];
                          final stocka = _stockMap[platera.id] ?? platera.stockAhalmena;
                          final agortuta = stocka == 0;

                          return Card(
                            margin: const EdgeInsets.only(bottom: 10),
                            child: Padding(
                              padding: const EdgeInsets.all(12),
                              child: Row(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  // Kategoria Ikonoa / Tag
                                  Container(
                                    width: 38,
                                    height: 38,
                                    decoration: BoxDecoration(
                                      color: AppTheme.getKategoriaBg(platera.kategoria),
                                      borderRadius: BorderRadius.circular(8),
                                    ),
                                    alignment: Alignment.center,
                                    child: Text(
                                      platera.kategoria.isNotEmpty ? platera.kategoria.substring(0, 2).toUpperCase() : 'PL',
                                      style: TextStyle(
                                        fontSize: 12,
                                        fontWeight: FontWeight.bold,
                                        color: AppTheme.getKategoriaText(platera.kategoria),
                                      ),
                                    ),
                                  ),
                                  const SizedBox(width: 12),
                                  // Informazioa
                                  Expanded(
                                    child: Column(
                                      crossAxisAlignment: CrossAxisAlignment.start,
                                      children: [
                                        Text(
                                          platera.izena,
                                          style: const TextStyle(
                                            fontSize: 15,
                                            fontWeight: FontWeight.bold,
                                            color: AppTheme.textMain,
                                          ),
                                        ),
                                        if (platera.deskribapena != null && platera.deskribapena!.isNotEmpty)
                                          Text(
                                            platera.deskribapena!,
                                            style: const TextStyle(fontSize: 12, color: AppTheme.textMuted),
                                            maxLines: 2,
                                            overflow: TextOverflow.ellipsis,
                                          ),
                                        const SizedBox(height: 4),
                                        Row(
                                          children: [
                                            Text(
                                              currency.format(platera.prezioa),
                                              style: const TextStyle(
                                                fontSize: 15,
                                                fontWeight: FontWeight.w900,
                                                color: AppTheme.brandPrimary,
                                              ),
                                            ),
                                            const SizedBox(width: 8),
                                            if (agortuta)
                                              Container(
                                                padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                                                decoration: BoxDecoration(
                                                  color: AppTheme.dangerBg,
                                                  borderRadius: BorderRadius.circular(4),
                                                ),
                                                child: const Text(
                                                  'Agortuta',
                                                  style: TextStyle(
                                                    fontSize: 10,
                                                    fontWeight: FontWeight.bold,
                                                    color: AppTheme.danger,
                                                  ),
                                                ),
                                              )
                                            else if (stocka < 10)
                                              Text(
                                                '($stocka geratzen dira)',
                                                style: const TextStyle(fontSize: 11, color: AppTheme.warningText),
                                              ),
                                          ],
                                        ),
                                      ],
                                    ),
                                  ),
                                  // Gehitu botoia (+)
                                  IconButton.filled(
                                    onPressed: agortuta ? null : () => _gehituSaskira(platera),
                                    icon: const Icon(Icons.add),
                                    style: IconButton.styleFrom(
                                      backgroundColor: AppTheme.brandPrimary,
                                      foregroundColor: Colors.white,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          );
                        },
                      ),
          ),
        ],
      ),
      bottomSheet: _saskia.isEmpty
          ? null
          : InkWell(
              onTap: _irekiSaskiaModal,
              child: Container(
                padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                decoration: const BoxDecoration(
                  color: AppTheme.brandPrimary,
                  borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
                  boxShadow: [
                    BoxShadow(
                      color: Color(0x2A881337),
                      blurRadius: 10,
                      offset: Offset(0, -4),
                    ),
                  ],
                ),
                child: SafeArea(
                  top: false,
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Row(
                        children: [
                          const Icon(Icons.shopping_bag_outlined, color: Colors.white, size: 22),
                          const SizedBox(width: 8),
                          Text(
                            '$_saskiaKopurua plater hautatuta',
                            style: const TextStyle(
                              color: Colors.white,
                              fontWeight: FontWeight.bold,
                              fontSize: 14,
                            ),
                          ),
                        ],
                      ),
                      Row(
                        children: [
                          Text(
                            currency.format(_saskiaGuztira),
                            style: const TextStyle(
                              color: AppTheme.gold,
                              fontWeight: FontWeight.w900,
                              fontSize: 17,
                            ),
                          ),
                          const SizedBox(width: 6),
                          const Icon(Icons.keyboard_arrow_up, color: Colors.white),
                        ],
                      ),
                    ],
                  ),
                ),
              ),
            ),
    );
  }
}
