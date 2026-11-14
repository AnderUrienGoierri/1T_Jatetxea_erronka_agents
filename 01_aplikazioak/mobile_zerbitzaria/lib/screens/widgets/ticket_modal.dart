import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../models/eskaera.dart';
import '../../services/api_service.dart';
import '../../theme/app_theme.dart';

class TicketModal extends StatefulWidget {
  final Eskaera eskaera;
  final VoidCallback onKobratua;

  const TicketModal({
    super.key,
    required this.eskaera,
    required this.onKobratua,
  });

  static Future<void> show(BuildContext context, Eskaera eskaera, VoidCallback onKobratua) {
    return showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) => TicketModal(
        eskaera: eskaera,
        onKobratua: onKobratua,
      ),
    );
  }

  @override
  State<TicketModal> createState() => _TicketModalState();
}

class _TicketModalState extends State<TicketModal> {
  bool _kargatzen = false;

  double get _guztira {
    if (widget.eskaera.lerroak.isNotEmpty) {
      return widget.eskaera.lerroak.fold(0.0, (sum, item) => sum + item.azpitotala);
    }
    return widget.eskaera.guztira;
  }

  double get _azpitotala => _guztira / 1.10;
  double get _bez => _guztira - _azpitotala;

  Future<void> _kobratu() async {
    setState(() => _kargatzen = true);

    // 1. Eskaera Kobratua jarri
    final ok = await ApiService.eguneratuEskaerarenEgoera(widget.eskaera.id, 'Kobratua');
    
    // 2. Mahaiaren beste eskaerarik ba ote den egiaztatu
    final besteak = await ApiService.getMahaiarenEskaerak(widget.eskaera.mahaiaId);
    if (besteak.isEmpty) {
      // Mahaia libre utzi edo garbitzen
      await ApiService.eguneratuMahaiarenEgoera(widget.eskaera.mahaiaId, 'Garbitzen');
    }

    setState(() => _kargatzen = false);

    if (mounted) {
      Navigator.pop(context);
      widget.onKobratua();
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(ok ? 'Kontua arrakastaz kobratu da.' : 'Errorea kontua kobratzean.'),
          backgroundColor: ok ? AppTheme.success : AppTheme.danger,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final currency = NumberFormat.currency(locale: 'eu_ES', symbol: '€', decimalDigits: 2);
    final height = MediaQuery.of(context).size.height * 0.82;

    return Container(
      height: height,
      decoration: const BoxDecoration(
        color: AppTheme.bgCard,
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      padding: const EdgeInsets.fromLTRB(20, 16, 20, 20),
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
          // Ticket Goiburua
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Mahaia #${widget.eskaera.mahaiZenbakia}',
                    style: const TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.w900,
                      color: AppTheme.brandPrimary,
                    ),
                  ),
                  Text(
                    'Eskaera #${widget.eskaera.id} · ${widget.eskaera.langileIzena}',
                    style: const TextStyle(fontSize: 12, color: AppTheme.textMuted),
                  ),
                ],
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                decoration: BoxDecoration(
                  color: AppTheme.warningBg,
                  borderRadius: BorderRadius.circular(999),
                  border: Border.all(color: AppTheme.warning),
                ),
                child: const Text(
                  'TICKET / KONTUA',
                  style: TextStyle(
                    fontSize: 11,
                    fontWeight: FontWeight.bold,
                    color: AppTheme.warningText,
                  ),
                ),
              ),
            ],
          ),
          const Divider(height: 24, thickness: 1),
          // Plateren Zerrenda
          Expanded(
            child: widget.eskaera.lerroak.isEmpty
                ? const Center(
                    child: Text(
                      'Ez dago platerik eskaera honetan.',
                      style: TextStyle(color: AppTheme.textMuted),
                    ),
                  )
                : ListView.separated(
                    itemCount: widget.eskaera.lerroak.length,
                    separatorBuilder: (_, i) => const Divider(height: 1, color: AppTheme.border),
                    itemBuilder: (ctx, index) {
                      final item = widget.eskaera.lerroak[index];
                      return Padding(
                        padding: const EdgeInsets.symmetric(vertical: 8),
                        child: Row(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Container(
                              padding: const EdgeInsets.symmetric(horizontal: 7, vertical: 3),
                              decoration: BoxDecoration(
                                color: AppTheme.brandLight,
                                borderRadius: BorderRadius.circular(6),
                              ),
                              child: Text(
                                '${item.kantitatea}x',
                                style: const TextStyle(
                                  fontWeight: FontWeight.bold,
                                  color: AppTheme.brandPrimary,
                                  fontSize: 13,
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
                                      fontWeight: FontWeight.w600,
                                      fontSize: 14,
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
                            Text(
                              currency.format(item.azpitotala),
                              style: const TextStyle(
                                fontWeight: FontWeight.w700,
                                fontSize: 14,
                                color: AppTheme.textMain,
                              ),
                            ),
                          ],
                        ),
                      );
                    },
                  ),
          ),
          const Divider(height: 20, thickness: 1.5),
          // Datu Fiskalak / Totalak
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('Azpitotala:', style: TextStyle(color: AppTheme.textMuted, fontSize: 13)),
              Text(currency.format(_azpitotala), style: const TextStyle(fontSize: 13)),
            ],
          ),
          const SizedBox(height: 4),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text('BEZ (%10):', style: TextStyle(color: AppTheme.textMuted, fontSize: 13)),
              Text(currency.format(_bez), style: const TextStyle(fontSize: 13)),
            ],
          ),
          const SizedBox(height: 8),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
            decoration: BoxDecoration(
              color: AppTheme.brandLight,
              borderRadius: BorderRadius.circular(10),
              border: Border.all(color: AppTheme.brandPrimary.withValues(alpha: 0.3)),
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  'GUZTIRA:',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.w900,
                    color: AppTheme.brandDark,
                  ),
                ),
                Text(
                  currency.format(_guztira),
                  style: const TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.w900,
                    color: AppTheme.brandPrimary,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 16),
          // Botoiak
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  onPressed: () => Navigator.pop(context),
                  child: const Text('Itxi'),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                flex: 2,
                child: ElevatedButton.icon(
                  onPressed: _kargatzen ? null : _kobratu,
                  icon: _kargatzen
                      ? const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                        )
                      : const Icon(Icons.point_of_sale),
                  label: const Text('KOBRATU (Kontua Itxi)'),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: AppTheme.success,
                  ),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
