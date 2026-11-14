import 'package:flutter/material.dart';
import '../models/iritzia.dart';
import '../services/api_service.dart';
import '../theme/app_theme.dart';
import 'widgets/header_cart_button.dart';

class IritziakScreen extends StatefulWidget {
  const IritziakScreen({super.key});

  @override
  State<IritziakScreen> createState() => _IritziakScreenState();
}

class _IritziakScreenState extends State<IritziakScreen> {
  List<Iritzia> _iritziak = [];
  IritziEstatistikak? _estatistikak;
  bool _kargatzen = true;
  String? _errorea;

  @override
  void initState() {
    super.initState();
    _kargatuDatuak();
  }

  Future<void> _kargatuDatuak() async {
    setState(() {
      _kargatzen = true;
      _errorea = null;
    });

    try {
      final iritziak = await ApiService.getIritziak();
      final estatistikak = await ApiService.getIritziEstatistikak();
      setState(() {
        _iritziak = iritziak;
        _estatistikak = estatistikak;
        _kargatzen = false;
      });
    } catch (e) {
      setState(() {
        _errorea = e.toString();
        _kargatzen = false;
      });
    }
  }

  void _irekiIritziFormularioa() {
    int hautatutakoIzarrak = 5;
    final izenaController = TextEditingController(text: 'Ander Urien');
    final emailController = TextEditingController(text: 'ander.urien@ikasle.eus');
    final testuController = TextEditingController();
    final formKey = GlobalKey<FormState>();
    bool bidaltzen = false;

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
          child: Form(
            key: formKey,
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
                const Text(
                  'Utzi Zure Balorazioa',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: AppTheme.darkCharcoal,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  'Zure iritziak gure sukaldaritza eta zerbitzua hobetzen laguntzen digu.',
                  style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
                ),
                const SizedBox(height: 16),

                // Izar interaktiboak
                Center(
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: List.generate(5, (index) {
                      final izarZenb = index + 1;
                      return IconButton(
                        iconSize: 34,
                        padding: const EdgeInsets.symmetric(horizontal: 4),
                        icon: Icon(
                          izarZenb <= hautatutakoIzarrak ? Icons.star : Icons.star_border,
                          color: AppTheme.parmigianoGold,
                        ),
                        onPressed: () {
                          setModalState(() => hautatutakoIzarrak = izarZenb);
                        },
                      );
                    }),
                  ),
                ),
                Center(
                  child: Text(
                    '$hautatutakoIzarrak / 5 izar',
                    style: const TextStyle(fontWeight: FontWeight.bold, color: AppTheme.chiantiWine),
                  ),
                ),
                const SizedBox(height: 16),

                TextFormField(
                  controller: izenaController,
                  decoration: const InputDecoration(labelText: 'Izena *', isDense: true),
                  validator: (v) => (v == null || v.trim().isEmpty) ? 'Izena beharrezkoa da' : null,
                ),
                const SizedBox(height: 10),
                TextFormField(
                  controller: emailController,
                  keyboardType: TextInputType.emailAddress,
                  decoration: const InputDecoration(labelText: 'Emaila *', isDense: true),
                  validator: (v) => (v == null || !v.contains('@')) ? 'Email balioduna sartu' : null,
                ),
                const SizedBox(height: 10),
                TextFormField(
                  controller: testuController,
                  maxLines: 3,
                  decoration: const InputDecoration(
                    labelText: 'Iruzkina edo esperientzia (aukerakoa)',
                    hintText: 'Adib.: Txuleta eta tiramisua bikainak izan dira...',
                    isDense: true,
                  ),
                ),
                const SizedBox(height: 20),

                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    onPressed: bidaltzen
                        ? null
                        : () async {
                            if (!formKey.currentState!.validate()) return;
                            setModalState(() => bidaltzen = true);

                            final (ondo, mezua) = await ApiService.sortuIritzia(
                              izena: izenaController.text.trim(),
                              emaila: emailController.text.trim(),
                              balorazioa: hautatutakoIzarrak,
                              testua: testuController.text.trim().isNotEmpty
                                  ? testuController.text.trim()
                                  : null,
                            );

                            setModalState(() => bidaltzen = false);
                            if (mounted) {
                              Navigator.pop(ctx);
                              ScaffoldMessenger.of(this.context).showSnackBar(
                                SnackBar(
                                  backgroundColor: ondo ? AppTheme.basilGreen : Colors.red,
                                  content: Text(mezua, style: const TextStyle(color: Colors.white)),
                                ),
                              );
                              if (ondo) {
                                _kargatuDatuak();
                              }
                            }
                          },
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppTheme.chiantiWine,
                      padding: const EdgeInsets.symmetric(vertical: 14),
                      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                    ),
                    child: bidaltzen
                        ? const SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                          )
                        : const Text(
                            'Iritzia Argitaratu',
                            style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold),
                          ),
                  ),
                ),
              ],
            ),
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
              'Bezeroen Iritziak',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Colors.white),
            ),
            Text(
              'T1 Jatetxea · Komunitatea & Balorazioak',
              style: TextStyle(fontSize: 11, color: AppTheme.parmigianoGold),
            ),
          ],
        ),
        actions: const [
          HeaderCartButton(),
          SizedBox(width: 6),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: AppTheme.chiantiWine,
        icon: const Icon(Icons.rate_review, color: Colors.white),
        label: const Text('Utzi Iritzia', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
        onPressed: _irekiIritziFormularioa,
      ),
      body: _kargatzen
          ? const Center(child: CircularProgressIndicator(color: AppTheme.chiantiWine))
          : _errorea != null
              ? Center(
                  child: Padding(
                    padding: const EdgeInsets.all(24),
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        const Icon(Icons.wifi_off_rounded, size: 48, color: AppTheme.terracotta),
                        const SizedBox(height: 12),
                        const Text('Ezin izan dira iritziak kargatu', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16)),
                        const SizedBox(height: 6),
                        Text(_errorea!, textAlign: TextAlign.center, style: TextStyle(fontSize: 12, color: Colors.grey.shade600)),
                        const SizedBox(height: 16),
                        ElevatedButton(
                          onPressed: _kargatuDatuak,
                          style: ElevatedButton.styleFrom(backgroundColor: AppTheme.chiantiWine),
                          child: const Text('Saiatu berriro', style: TextStyle(color: Colors.white)),
                        ),
                      ],
                    ),
                  ),
                )
              : RefreshIndicator(
              color: AppTheme.chiantiWine,
              onRefresh: _kargatuDatuak,
              child: ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  // Estatistiken Panela
                  _buildEstatistikakCard(),
                  const SizedBox(height: 16),

                  // Zerrendako goiburua
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'Azken Balorazioak (${_iritziak.length})',
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: AppTheme.darkCharcoal,
                        ),
                      ),
                      TextButton.icon(
                        onPressed: _irekiIritziFormularioa,
                        icon: const Icon(Icons.add, size: 16, color: AppTheme.chiantiWine),
                        label: const Text('Idatzi', style: TextStyle(color: AppTheme.chiantiWine, fontSize: 13)),
                      ),
                    ],
                  ),
                  const SizedBox(height: 8),

                  if (_iritziak.isEmpty)
                    Container(
                      padding: const EdgeInsets.all(32),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(12),
                        border: Border.all(color: Colors.grey.shade200),
                      ),
                      child: Column(
                        children: [
                          Icon(Icons.chat_bubble_outline, size: 48, color: Colors.grey.shade400),
                          const SizedBox(height: 12),
                          const Text(
                            'Oraindik ez dago iritzirik.',
                            style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
                          ),
                          const SizedBox(height: 4),
                          Text(
                            'Izan lehena gure jatetxearen inguruko balorazioa uzten!',
                            style: TextStyle(color: Colors.grey.shade600, fontSize: 12),
                          ),
                        ],
                      ),
                    )
                  else
                    ..._iritziak.map((iritzia) => _buildIritziCard(iritzia)),

                  const SizedBox(height: 70), // FloatingActionButton lekua
                ],
              ),
            ),
    );
  }

  Widget _buildEstatistikakCard() {
    final kop5 = _estatistikak?.bostIzarrak ?? (_iritziak.isEmpty ? 5 : _iritziak.where((i) => i.balorazioa == 5).length);
    final kop4 = _estatistikak?.lauIzarrak ?? (_iritziak.isEmpty ? 2 : _iritziak.where((i) => i.balorazioa == 4).length);
    final kop3 = _estatistikak?.hiruIzarrak ?? (_iritziak.isEmpty ? 1 : _iritziak.where((i) => i.balorazioa == 3).length);
    final kop2 = _estatistikak?.biIzarrak ?? _iritziak.where((i) => i.balorazioa == 2).length;
    final kop1 = _estatistikak?.izarBat ?? _iritziak.where((i) => i.balorazioa == 1).length;

    final guztiraKop = (kop5 + kop4 + kop3 + kop2 + kop1);
    final guztira = (_estatistikak != null && _estatistikak!.guztiraIritziak > 0)
        ? _estatistikak!.guztiraIritziak
        : (_iritziak.isNotEmpty ? _iritziak.length : guztiraKop);
    final media = (_estatistikak != null && _estatistikak!.batazBestekoBalorazioa > 0)
        ? _estatistikak!.batazBestekoBalorazioa
        : (guztiraKop > 0 ? (kop5 * 5.0 + kop4 * 4.0 + kop3 * 3.0 + kop2 * 2.0 + kop1 * 1.0) / guztiraKop : 4.8);

    return Card(
      elevation: 1,
      color: Colors.white,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: Padding(
        padding: const EdgeInsets.all(18),
        child: Row(
          children: [
            Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                Text(
                  media.toStringAsFixed(1),
                  style: const TextStyle(
                    fontSize: 44,
                    fontWeight: FontWeight.w900,
                    color: AppTheme.chiantiWine,
                  ),
                ),
                Row(
                  mainAxisSize: MainAxisSize.min,
                  children: List.generate(5, (i) {
                    final betea = (i + 1) <= media.round();
                    return Icon(
                      betea ? Icons.star : Icons.star_border,
                      size: 16,
                      color: AppTheme.parmigianoGold,
                    );
                  }),
                ),
                const SizedBox(height: 4),
                Text(
                  '$guztira iritzi',
                  style: TextStyle(fontSize: 11, color: Colors.grey.shade600),
                ),
              ],
            ),
            const SizedBox(width: 24),
            Expanded(
              child: Column(
                children: [
                  _buildIzarBarra(5, kop5, guztiraKop),
                  _buildIzarBarra(4, kop4, guztiraKop),
                  _buildIzarBarra(3, kop3, guztiraKop),
                  _buildIzarBarra(2, kop2, guztiraKop),
                  _buildIzarBarra(1, kop1, guztiraKop),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildIzarBarra(int izarra, int kopurua, int guztira) {
    final double portzentaia = guztira > 0 ? (kopurua / guztira) : 0.0;
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2),
      child: Row(
        children: [
          Text('$izarra', style: const TextStyle(fontSize: 11, fontWeight: FontWeight.bold)),
          const SizedBox(width: 4),
          const Icon(Icons.star, size: 12, color: AppTheme.parmigianoGold),
          const SizedBox(width: 8),
          Expanded(
            child: ClipRRect(
              borderRadius: BorderRadius.circular(4),
              child: LinearProgressIndicator(
                value: portzentaia.clamp(0.0, 1.0),
                backgroundColor: Colors.grey.shade200,
                color: AppTheme.parmigianoGold,
                minHeight: 6,
              ),
            ),
          ),
          const SizedBox(width: 8),
          Text(
            '$kopurua',
            style: TextStyle(fontSize: 10, color: Colors.grey.shade600),
          ),
        ],
      ),
    );
  }

  Widget _buildIritziCard(Iritzia iritzia) {
    return Card(
      elevation: 0.5,
      color: Colors.white,
      margin: const EdgeInsets.only(bottom: 10),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(color: Colors.grey.shade200),
      ),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  radius: 18,
                  backgroundColor: AppTheme.chiantiWine.withValues(alpha: 0.1),
                  child: Text(
                    iritzia.bezeroIzena.isNotEmpty ? iritzia.bezeroIzena[0].toUpperCase() : 'B',
                    style: const TextStyle(
                      color: AppTheme.chiantiWine,
                      fontWeight: FontWeight.bold,
                      fontSize: 14,
                    ),
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        iritzia.bezeroIzena,
                        style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
                      ),
                      if (iritzia.sortzeData.isNotEmpty)
                        Text(
                          iritzia.sortzeData.split('T')[0],
                          style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
                        ),
                    ],
                  ),
                ),
                Row(
                  children: List.generate(5, (index) {
                    return Icon(
                      index < iritzia.balorazioa ? Icons.star : Icons.star_border,
                      size: 16,
                      color: AppTheme.parmigianoGold,
                    );
                  }),
                ),
              ],
            ),
            if (iritzia.testua != null && iritzia.testua!.isNotEmpty) ...[
              const SizedBox(height: 10),
              Text(
                iritzia.testua!,
                style: TextStyle(
                  fontSize: 13,
                  color: Colors.grey.shade800,
                  height: 1.35,
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
