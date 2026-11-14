import 'package:flutter/material.dart';
import '../../theme/app_theme.dart';

class OrdainketaOtpDialog extends StatefulWidget {
  final String metodoa;
  final String telefonoaEdoTxartela;
  final double zenbatekoa;

  const OrdainketaOtpDialog({
    super.key,
    required this.metodoa,
    required this.telefonoaEdoTxartela,
    required this.zenbatekoa,
  });

  static Future<bool?> show(
    BuildContext context, {
    required String metodoa,
    required String telefonoaEdoTxartela,
    required double zenbatekoa,
  }) {
    return showDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (ctx) => OrdainketaOtpDialog(
        metodoa: metodoa,
        telefonoaEdoTxartela: telefonoaEdoTxartela,
        zenbatekoa: zenbatekoa,
      ),
    );
  }

  @override
  State<OrdainketaOtpDialog> createState() => _OrdainketaOtpDialogState();
}

class _OrdainketaOtpDialogState extends State<OrdainketaOtpDialog> {
  final TextEditingController _otpController = TextEditingController(text: '123456');
  bool _egiaztatzen = false;

  @override
  void dispose() {
    _otpController.dispose();
    super.dispose();
  }

  Future<void> _baieztatu() async {
    setState(() => _egiaztatzen = true);
    await Future.delayed(const Duration(milliseconds: 900));
    setState(() => _egiaztatzen = false);

    if (mounted) {
      Navigator.pop(context, true);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      title: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: AppTheme.securityBg,
              shape: BoxShape.circle,
            ),
            child: const Icon(Icons.security, color: AppTheme.securityGreen, size: 24),
          ),
          const SizedBox(width: 10),
          const Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  '3D Secure 2.0',
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                ),
                Text(
                  'PSD2 Segurtasun Baieztapena',
                  style: TextStyle(fontSize: 11, color: AppTheme.textMuted),
                ),
              ],
            ),
          ),
        ],
      ),
      content: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: const Color(0xFFF1F5F9),
                borderRadius: BorderRadius.circular(10),
                border: Border.all(color: AppTheme.border),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text('Jatetxea:', style: TextStyle(fontSize: 12, color: AppTheme.textMuted)),
                      const Text('T1 Jatetxea Osteria', style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
                    ],
                  ),
                  const SizedBox(height: 4),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text('Zenbatekoa:', style: TextStyle(fontSize: 12, color: AppTheme.textMuted)),
                      Text(
                        '${widget.zenbatekoa.toStringAsFixed(2)} €',
                        style: const TextStyle(fontSize: 14, fontWeight: FontWeight.bold, color: AppTheme.brandPrimary),
                      ),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(height: 14),
            Text(
              'Zure bankuak (${widget.metodoa}) 6 digituko segurtasun kode bat bidali du SMS bidez (${widget.telefonoaEdoTxartela}):',
              style: const TextStyle(fontSize: 12, color: AppTheme.textMain, height: 1.4),
            ),
            const SizedBox(height: 12),
            TextField(
              controller: _otpController,
              keyboardType: TextInputType.number,
              textAlign: TextAlign.center,
              maxLength: 6,
              style: const TextStyle(fontSize: 22, fontWeight: FontWeight.bold, letterSpacing: 6),
              decoration: InputDecoration(
                hintText: '123456',
                counterText: '',
                fillColor: AppTheme.bgApp,
                helperText: 'Probarako kodea automatikoki sartuta: 123456',
                helperStyle: const TextStyle(color: AppTheme.securityGreen, fontSize: 11),
              ),
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: _egiaztatzen ? null : () => Navigator.pop(context, false),
          child: const Text('Utzi'),
        ),
        ElevatedButton(
          onPressed: _egiaztatzen ? null : _baieztatu,
          style: ElevatedButton.styleFrom(backgroundColor: AppTheme.securityGreen),
          child: _egiaztatzen
              ? const SizedBox(
                  width: 16,
                  height: 16,
                  child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                )
              : const Text('Baimendu eta Ordaindu'),
        ),
      ],
    );
  }
}
