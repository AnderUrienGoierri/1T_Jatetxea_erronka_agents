import 'package:flutter/material.dart';
import '../../models/cart_item.dart';
import '../../models/ordainketa.dart';
import '../../services/tiket_service.dart';
import '../../theme/app_theme.dart';

class OrdainketaArrakastaScreen extends StatefulWidget {
  final OrdainketaEmaitza emaitza;
  final String bezeroIzena;
  final String eskaeraMota;
  final int mahaiZenbakia;
  final List<CartItem> saskia;

  const OrdainketaArrakastaScreen({
    super.key,
    required this.emaitza,
    required this.bezeroIzena,
    required this.eskaeraMota,
    this.mahaiZenbakia = 1,
    this.saskia = const [],
  });

  @override
  State<OrdainketaArrakastaScreen> createState() => _OrdainketaArrakastaScreenState();
}

class _OrdainketaArrakastaScreenState extends State<OrdainketaArrakastaScreen> {
  bool _inprimatzen = false;
  bool _partekatzen = false;
  late final DateTime _data;
  late final String _tiketTestua;

  @override
  void initState() {
    super.initState();
    _data = DateTime.tryParse(widget.emaitza.data.replaceAll(' ', 'T')) ?? DateTime.now();
    _tiketTestua = TiketService.sortuTiketTestua(
      eskaeraId: widget.emaitza.eskaeraId,
      data: _data,
      mahaiZenbakia: widget.mahaiZenbakia,
      eskaeraMota: widget.eskaeraMota,
      bezeroIzena: widget.bezeroIzena,
      saskia: widget.saskia,
      guztira: widget.emaitza.zenbatekoa,
      emaitza: widget.emaitza,
    );
  }

  Future<void> _inprimatuEdoDeskargatu() async {
    setState(() => _inprimatzen = true);
    try {
      await TiketService.inprimatuEdoDeskargatu(
        eskaeraId: widget.emaitza.eskaeraId,
        data: _data,
        mahaiZenbakia: widget.mahaiZenbakia,
        eskaeraMota: widget.eskaeraMota,
        bezeroIzena: widget.bezeroIzena,
        saskia: widget.saskia,
        guztira: widget.emaitza.zenbatekoa,
        emaitza: widget.emaitza,
      );
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Errorea tiketa inprimatzean / deskargatzean: $e'),
            backgroundColor: Colors.red.shade700,
          ),
        );
      }
    } finally {
      if (mounted) setState(() => _inprimatzen = false);
    }
  }

  Future<void> _partekatu() async {
    setState(() => _partekatzen = true);
    try {
      await TiketService.partekatuEdoDeskargatuPdf(
        eskaeraId: widget.emaitza.eskaeraId,
        data: _data,
        mahaiZenbakia: widget.mahaiZenbakia,
        eskaeraMota: widget.eskaeraMota,
        bezeroIzena: widget.bezeroIzena,
        saskia: widget.saskia,
        guztira: widget.emaitza.zenbatekoa,
        emaitza: widget.emaitza,
      );
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Errorea tiketa partekatzean: $e'),
            backgroundColor: Colors.red.shade700,
          ),
        );
      }
    } finally {
      if (mounted) setState(() => _partekatzen = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppTheme.bgApp,
      appBar: AppBar(
        title: const Text('Ordainketa Eginda'),
        automaticallyImplyLeading: false,
      ),
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 18),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // Ikono arrakastatsua
              Center(
                child: Container(
                  width: 72,
                  height: 72,
                  decoration: const BoxDecoration(
                    color: AppTheme.securityBg,
                    shape: BoxShape.circle,
                  ),
                  child: const Icon(
                    Icons.check_circle_rounded,
                    color: AppTheme.securityGreen,
                    size: 56,
                  ),
                ),
              ),
              const SizedBox(height: 12),
              const Center(
                child: Text(
                  'Ordainketa Arrakastatsua!',
                  style: TextStyle(
                    fontSize: 22,
                    fontWeight: FontWeight.w900,
                    color: AppTheme.brandDark,
                  ),
                ),
              ),
              const SizedBox(height: 4),
              Center(
                child: Text(
                  'Eskerrik asko, ${widget.bezeroIzena}. Zure eskaera sukaldera bidali da.',
                  textAlign: TextAlign.center,
                  style: const TextStyle(fontSize: 13, color: AppTheme.textMuted),
                ),
              ),
              const SizedBox(height: 16),

              // Botoiak: Deskargatu / Inprimatu eta Partekatu
              ElevatedButton.icon(
                onPressed: _inprimatzen ? null : _inprimatuEdoDeskargatu,
                icon: _inprimatzen
                    ? const SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                      )
                    : const Icon(Icons.print_rounded, size: 20),
                label: Text(
                  _inprimatzen ? 'Tiketa prestatzen...' : 'DESKARGATU / INPRIMATU TIKETA (PDF 80mm)',
                  style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
                ),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppTheme.brandPrimary,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 14),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),
              const SizedBox(height: 8),

              OutlinedButton.icon(
                onPressed: _partekatzen ? null : _partekatu,
                icon: _partekatzen
                    ? const SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : const Icon(Icons.share_rounded, size: 18),
                label: Text(
                  _partekatzen ? 'Prestatzen...' : 'PARTEKATU TIKETA (PDF)',
                  style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 12.5),
                ),
                style: OutlinedButton.styleFrom(
                  foregroundColor: AppTheme.brandPrimary,
                  side: const BorderSide(color: AppTheme.brandPrimary, width: 1.3),
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),

              const SizedBox(height: 16),

              // TVP Tiket Aurrebista beltza (Monospace 80mm formatu erreala)
              Container(
                decoration: BoxDecoration(
                  color: const Color(0xFF111827), // Termiko beltza
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: const Color(0xFF374151), width: 1.2),
                  boxShadow: const [
                    BoxShadow(
                      color: Color(0x33000000),
                      blurRadius: 10,
                      offset: Offset(0, 4),
                    ),
                  ],
                ),
                padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        const Row(
                          children: [
                            Icon(Icons.receipt_long, color: AppTheme.gold, size: 16),
                            SizedBox(width: 6),
                            Text(
                              'TVP TIKET TERMIKOA (80mm)',
                              style: TextStyle(
                                color: AppTheme.gold,
                                fontSize: 11,
                                fontWeight: FontWeight.bold,
                                letterSpacing: 0.5,
                              ),
                            ),
                          ],
                        ),
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                          decoration: BoxDecoration(
                            color: const Color(0x2210B981),
                            borderRadius: BorderRadius.circular(4),
                            border: Border.all(color: const Color(0xFF10B981)),
                          ),
                          child: const Text(
                            'KOBRATUA',
                            style: TextStyle(
                              color: Color(0xFF10B981),
                              fontSize: 9.5,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ),
                      ],
                    ),
                    const Divider(color: Color(0xFF374151), height: 16),
                    SingleChildScrollView(
                      scrollDirection: Axis.horizontal,
                      child: SelectableText(
                        _tiketTestua,
                        style: const TextStyle(
                          color: Color(0xFFE5E7EB),
                          fontFamily: 'monospace',
                          fontSize: 11,
                          height: 1.28,
                          letterSpacing: -0.2,
                        ),
                      ),
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 20),

              // Hasierara itzultzeko botoia
              ElevatedButton.icon(
                onPressed: () {
                  Navigator.of(context).popUntil((route) => route.isFirst);
                },
                icon: const Icon(Icons.restaurant_menu),
                label: const Text(
                  'Hasierara Itzuli (Karta)',
                  style: TextStyle(fontWeight: FontWeight.bold),
                ),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppTheme.brandDark,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 14),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),
              const SizedBox(height: 12),
            ],
          ),
        ),
      ),
    );
  }
}
