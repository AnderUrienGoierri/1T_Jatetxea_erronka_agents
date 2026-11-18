import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:intl/intl.dart';
import '../../models/eskaera.dart';
import '../../models/langilea.dart';
import '../../models/mahaia.dart';
import '../../models/ordainketa.dart';
import '../../services/api_service.dart';
import '../../theme/app_theme.dart';

class OrdainketaModal extends StatefulWidget {
  final Mahaia mahaia;
  final Eskaera eskaera;
  final Langilea langilea;

  const OrdainketaModal({
    super.key,
    required this.mahaia,
    required this.eskaera,
    required this.langilea,
  });

  static Future<bool?> erakutsi(BuildContext context, {
    required Mahaia mahaia,
    required Eskaera eskaera,
    required Langilea langilea,
  }) {
    return showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (_) => OrdainketaModal(
        mahaia: mahaia,
        eskaera: eskaera,
        langilea: langilea,
      ),
    );
  }

  @override
  State<OrdainketaModal> createState() => _OrdainketaModalState();
}

class _OrdainketaModalState extends State<OrdainketaModal> {
  final currency = NumberFormat.currency(locale: 'eu_ES', symbol: '€');

  String _mota = 'Eskudirua'; // 'Eskudirua', 'Txartela', 'Bizum'
  bool _kargatzen = false;

  // Eskudirua
  late final TextEditingController _jasotakoaCtrl;

  // Txartela
  final _txartelCtrl = TextEditingController(text: '4532 8921 7842 4821');
  final _iraungCtrl = TextEditingController(text: '12/28');
  final _cvvCtrl = TextEditingController(text: '842');
  final _titularCtrl = TextEditingController(text: 'Bezero Arrunta');

  // Bizum
  final _bizumCtrl = TextEditingController(text: '644 12 34 56');

  double get _totala => widget.eskaera.lerroak.fold(0.0, (s, l) => s + l.azpitotala);

  @override
  void initState() {
    super.initState();
    _jasotakoaCtrl = TextEditingController(text: _totala.toStringAsFixed(2));
  }

  @override
  void dispose() {
    _jasotakoaCtrl.dispose();
    _txartelCtrl.dispose();
    _iraungCtrl.dispose();
    _cvvCtrl.dispose();
    _titularCtrl.dispose();
    _bizumCtrl.dispose();
    super.dispose();
  }

  double get _jasotakoaBalioa => double.tryParse(_jasotakoaCtrl.text.replaceAll(',', '.')) ?? _totala;
  double get _kanbioa => _jasotakoaBalioa - _totala;

  Future<void> _prozesatuOrdainketa() async {
    setState(() => _kargatzen = true);

    double? jasotakoa;
    if (_mota == 'Eskudirua') {
      jasotakoa = _jasotakoaBalioa;
      if (jasotakoa < _totala) {
        setState(() => _kargatzen = false);
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Jasotako zenbatekoa ez da nahikoa kontua ordaintzeko.')),
        );
        return;
      }
    }

    final (ok, mezua, ordainketa) = await ApiService.prozesatuOrdainketa(
      eskaeraId: widget.eskaera.id,
      mota: _mota,
      zenbatekoa: _totala,
      jasotakoa: jasotakoa,
      txartelZenbakia: _mota == 'Txartela' ? _txartelCtrl.text : null,
      titularra: _mota == 'Txartela' ? _titularCtrl.text : null,
      iraungitzea: _mota == 'Txartela' ? _iraungCtrl.text : null,
      cvv: _mota == 'Txartela' ? _cvvCtrl.text : null,
      bizumTelefonoa: _mota == 'Bizum' ? _bizumCtrl.text : null,
      langileaId: widget.langilea.id,
    );

    setState(() => _kargatzen = false);

    if (!mounted) return;

    if (ok && ordainketa != null) {
      // Tiket fiskala erakutsi
      await _erakutsiTiketaDialog(ordainketa);
      if (mounted) Navigator.pop(context, true);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(mezua),
          backgroundColor: AppTheme.danger,
        ),
      );
    }
  }

  Future<void> _erakutsiTiketaDialog(OrdainketaErantzuna ord) async {
    await showDialog(
      context: context,
      barrierDismissible: false,
      builder: (ctx) {
        return AlertDialog(
          title: Row(
            children: [
              const Icon(Icons.check_circle, color: AppTheme.success, size: 26),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  'Tiket Fiskala (${ord.tiketZenbakia})',
                  style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                ),
              ),
            ],
          ),
          content: SizedBox(
            width: double.maxFinite,
            height: 380,
            child: SingleChildScrollView(
              child: Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: const Color(0xFFF8FAFC),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(color: AppTheme.border),
                ),
                child: SelectableText(
                  ord.tiketTestua,
                  style: const TextStyle(
                    fontFamily: 'monospace',
                    fontSize: 10.5,
                    height: 1.25,
                    color: Color(0xFF0F172A),
                  ),
                ),
              ),
            ),
          ),
          actions: [
            TextButton.icon(
              onPressed: () {
                Clipboard.setData(ClipboardData(text: ord.tiketTestua));
                ScaffoldMessenger.of(ctx).showSnackBar(
                  const SnackBar(content: Text('Tiketa arbelean kopiatu da!')),
                );
              },
              icon: const Icon(Icons.copy, size: 18),
              label: const Text('Kopiatu'),
            ),
            ElevatedButton.icon(
              style: ElevatedButton.styleFrom(
                backgroundColor: AppTheme.brandPrimary,
                foregroundColor: Colors.white,
              ),
              onPressed: () {
                ScaffoldMessenger.of(ctx).showSnackBar(
                  const SnackBar(content: Text('Tiketa 80mm-ko inprimagailura bidali da.')),
                );
                Navigator.pop(ctx);
              },
              icon: const Icon(Icons.print, size: 18),
              label: const Text('Inprimatu (ESC/POS)'),
            ),
          ],
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final subtotal = _totala / 1.10;
    final bez = _totala - subtotal;

    return Container(
      decoration: const BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      padding: EdgeInsets.only(
        top: 16,
        left: 20,
        right: 20,
        bottom: MediaQuery.of(context).viewInsets.bottom + 20,
      ),
      child: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // Arraste heldulekua
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
            const SizedBox(height: 14),

            // Goiburua
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      'Mahaia ${widget.mahaia.zenbakia} - Kontua Kobratu',
                      style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: AppTheme.brandDark),
                    ),
                    Text(
                      'Eskaera #${widget.eskaera.id} · ${widget.langilea.izena}',
                      style: const TextStyle(fontSize: 12, color: AppTheme.textMuted),
                    ),
                  ],
                ),
                Text(
                  currency.format(_totala),
                  style: const TextStyle(fontSize: 22, fontWeight: FontWeight.w900, color: AppTheme.brandPrimary),
                ),
              ],
            ),
            const SizedBox(height: 6),
            Text(
              'Azpitotala: ${currency.format(subtotal)} · BEZ (%10 barne): ${currency.format(bez)}',
              style: const TextStyle(fontSize: 11, color: AppTheme.textMuted),
            ),
            const Divider(height: 24),

            // Ordainketa Mota Hautatzailea
            SegmentedButton<String>(
              segments: const [
                ButtonSegment(value: 'Eskudirua', label: Text('Eskudirua'), icon: Icon(Icons.payments_outlined)),
                ButtonSegment(value: 'Txartela', label: Text('Txartela'), icon: Icon(Icons.credit_card)),
                ButtonSegment(value: 'Bizum', label: Text('Bizum'), icon: Icon(Icons.phone_android)),
              ],
              selected: {_mota},
              onSelectionChanged: (set) => setState(() => _mota = set.first),
            ),
            const SizedBox(height: 16),

            // Edukia hautatutako motaren arabera
            if (_mota == 'Eskudirua') ...[
              const Text('Bezeroak emandako dirua (Jasotakoa):', style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
              const SizedBox(height: 6),
              TextField(
                controller: _jasotakoaCtrl,
                keyboardType: const TextInputType.numberWithOptions(decimal: true),
                style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                decoration: InputDecoration(
                  prefixText: '€ ',
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                  filled: true,
                  fillColor: const Color(0xFFF8FAFC),
                ),
                onChanged: (_) => setState(() {}),
              ),
              const SizedBox(height: 10),
              Wrap(
                spacing: 8,
                children: [
                  ActionChip(
                    label: const Text('Zehatza'),
                    onPressed: () => setState(() => _jasotakoaCtrl.text = _totala.toStringAsFixed(2)),
                  ),
                  ActionChip(
                    label: const Text('+ 5 €'),
                    onPressed: () {
                      final n = ((_totala / 5).ceil() * 5).toDouble();
                      setState(() => _jasotakoaCtrl.text = n.toStringAsFixed(2));
                    },
                  ),
                  ActionChip(
                    label: const Text('20 €'),
                    onPressed: () => setState(() => _jasotakoaCtrl.text = '20.00'),
                  ),
                  ActionChip(
                    label: const Text('50 €'),
                    onPressed: () => setState(() => _jasotakoaCtrl.text = '50.00'),
                  ),
                  ActionChip(
                    label: const Text('100 €'),
                    onPressed: () => setState(() => _jasotakoaCtrl.text = '100.00'),
                  ),
                ],
              ),
              const SizedBox(height: 12),
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: _kanbioa >= 0 ? const Color(0xFFDCFCE7) : const Color(0xFFFEE2E2),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      _kanbioa >= 0 ? 'Itzulitakoa (Kanbioa):' : 'Falta dena:',
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        color: _kanbioa >= 0 ? const Color(0xFF15803D) : const Color(0xFFB91C1C),
                      ),
                    ),
                    Text(
                      currency.format(_kanbioa.abs()),
                      style: TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.w900,
                        color: _kanbioa >= 0 ? const Color(0xFF15803D) : const Color(0xFFB91C1C),
                      ),
                    ),
                  ],
                ),
              ),
            ] else if (_mota == 'Txartela') ...[
              const Text('Kreditu / Zordunketa Txartela (EMV / Contactless):', style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
              const SizedBox(height: 6),
              TextField(
                controller: _txartelCtrl,
                decoration: InputDecoration(
                  prefixIcon: const Icon(Icons.credit_card),
                  labelText: 'Txartel Zenbakia',
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),
              const SizedBox(height: 10),
              Row(
                children: [
                  Expanded(
                    child: TextField(
                      controller: _iraungCtrl,
                      decoration: InputDecoration(
                        labelText: 'Iraungitzea',
                        border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: TextField(
                      controller: _cvvCtrl,
                      obscureText: true,
                      decoration: InputDecoration(
                        labelText: 'CVV',
                        border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 10),
              TextField(
                controller: _titularCtrl,
                decoration: InputDecoration(
                  labelText: 'Titularra',
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),
            ] else ...[
              const Text('Bizum Telefono Zenbakia:', style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
              const SizedBox(height: 6),
              TextField(
                controller: _bizumCtrl,
                keyboardType: TextInputType.phone,
                decoration: InputDecoration(
                  prefixIcon: const Icon(Icons.phone_android),
                  labelText: 'Telefonoa (9 digitu)',
                  border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),
              const SizedBox(height: 8),
              const Text(
                'Bezeroak bere mugikorrean berehalako jakinarazpena jasoko du.',
                style: TextStyle(fontSize: 11, color: AppTheme.textMuted),
              ),
            ],

            const SizedBox(height: 20),

            // Botoiak
            Row(
              children: [
                Expanded(
                  child: OutlinedButton(
                    onPressed: _kargatzen ? null : () => Navigator.pop(context),
                    child: const Text('Utzi'),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  flex: 2,
                  child: ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppTheme.brandPrimary,
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(vertical: 14),
                    ),
                    onPressed: (_kargatzen || (_mota == 'Eskudirua' && _kanbioa < 0))
                        ? null
                        : _prozesatuOrdainketa,
                    child: _kargatzen
                        ? const SizedBox(
                            height: 18,
                            width: 18,
                            child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                          )
                        : Text(
                            'KOBRATU (${currency.format(_totala)})',
                            style: const TextStyle(fontWeight: FontWeight.bold),
                          ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
