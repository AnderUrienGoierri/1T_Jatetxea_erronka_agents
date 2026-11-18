import 'package:flutter/material.dart';
import '../../models/cart_item.dart';
import '../../models/ordainketa.dart';
import '../../services/api_service.dart';
import '../../services/cart_service.dart';
import '../../services/ordainketa_service.dart';
import '../../theme/app_theme.dart';
import 'ordainketa_otp_dialog.dart';
import 'ordainketa_arrakasta_screen.dart';

class OrdainketaScreen extends StatefulWidget {
  final List<CartItem> saskia;
  final double guztira;
  final String eskaeraMota; // 'Mahaitik' edo 'Eramateko'
  final int? mahaiZenbakia;
  final String bezeroIzena;
  final String bezeroEmaila;
  final String bezeroTelefonoa;
  final String oharrak;

  const OrdainketaScreen({
    super.key,
    required this.saskia,
    required this.guztira,
    required this.eskaeraMota,
    this.mahaiZenbakia,
    required this.bezeroIzena,
    required this.bezeroEmaila,
    required this.bezeroTelefonoa,
    required this.oharrak,
  });

  @override
  State<OrdainketaScreen> createState() => _OrdainketaScreenState();
}

class _OrdainketaScreenState extends State<OrdainketaScreen> {
  OrdainketaMota _hautatutakoMota = OrdainketaMota.txartela;

  // Txartelaren Kontroladoreak
  final TextEditingController _txartelZenbController = TextEditingController(text: '4548 1234 5678 9012');
  final TextEditingController _titularraController = TextEditingController(text: 'Ander Urien');
  final TextEditingController _iraungitzeController = TextEditingController(text: '08/28');
  final TextEditingController _cvvController = TextEditingController(text: '888');

  // Bizum Kontroladorea
  late TextEditingController _bizumController;

  // PayPal Kontroladorea
  late TextEditingController _paypalController;

  bool _prozesatzen = false;

  @override
  void initState() {
    super.initState();
    _bizumController = TextEditingController(text: widget.bezeroTelefonoa.isNotEmpty ? widget.bezeroTelefonoa : '600123456');
    _paypalController = TextEditingController(text: widget.bezeroEmaila.isNotEmpty ? widget.bezeroEmaila : 'bezeroa@paypal.com');
  }

  @override
  void dispose() {
    _txartelZenbController.dispose();
    _titularraController.dispose();
    _iraungitzeController.dispose();
    _cvvController.dispose();
    _bizumController.dispose();
    _paypalController.dispose();
    super.dispose();
  }

  Future<void> _ordainketaBideratu() async {
    setState(() => _prozesatzen = true);

    String metodoIzena = 'Kreditu Txartela';
    String telefonoaEdoMask = OrdainketaService.maskaratuTxartela(_txartelZenbController.text);

    if (_hautatutakoMota == OrdainketaMota.bizum) {
      metodoIzena = 'Bizum';
      telefonoaEdoMask = _bizumController.text;
    } else if (_hautatutakoMota == OrdainketaMota.googlePay) {
      metodoIzena = 'Google Pay / Apple Pay';
      telefonoaEdoMask = 'GPay Biometria';
    } else if (_hautatutakoMota == OrdainketaMota.paypal) {
      metodoIzena = 'PayPal';
      telefonoaEdoMask = _paypalController.text;
    }

    // 1. 3D Secure / OTP SMS baieztapen dialogoa
    final baieztatuta = await OrdainketaOtpDialog.show(
      context,
      metodoa: metodoIzena,
      telefonoaEdoTxartela: telefonoaEdoMask,
      zenbatekoa: widget.guztira,
    );

    if (baieztatuta != true) {
      setState(() => _prozesatzen = false);
      return;
    }

    // 2. Eskaera sortu APIan eta sukaldera bidali
    final (eskaeraOk, mezua, eskaeraId) = await ApiService.sortuEskaeraBezeroa(
      saskia: widget.saskia,
      mahaiaId: widget.mahaiZenbakia ?? 1,
      bezeroIzena: widget.bezeroIzena,
      oharrak: widget.oharrak,
    );

    if (!eskaeraOk) {
      setState(() => _prozesatzen = false);
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(mezua), backgroundColor: AppTheme.danger),
        );
      }
      return;
    }

    // 3. Pasarela simulazioaren token eta transakzio IDa sortu
    OrdainketaEmaitza emaitza;
    switch (_hautatutakoMota) {
      case OrdainketaMota.txartela:
        emaitza = await OrdainketaService.prozesatuTxartela(
          txartelZenbakia: _txartelZenbController.text,
          titularra: _titularraController.text,
          iraungitzea: _iraungitzeController.text,
          cvv: _cvvController.text,
          zenbatekoa: widget.guztira,
          eskaeraId: eskaeraId,
        );
        break;
      case OrdainketaMota.bizum:
        emaitza = await OrdainketaService.prozesatuBizum(
          telefonoa: _bizumController.text,
          zenbatekoa: widget.guztira,
          eskaeraId: eskaeraId,
        );
        break;
      case OrdainketaMota.googlePay:
        emaitza = await OrdainketaService.prozesatuGooglePay(
          zenbatekoa: widget.guztira,
          eskaeraId: eskaeraId,
        );
        break;
      case OrdainketaMota.paypal:
        emaitza = await OrdainketaService.prozesatuPaypal(
          emaila: _paypalController.text,
          zenbatekoa: widget.guztira,
          eskaeraId: eskaeraId,
        );
        break;
    }

    setState(() => _prozesatzen = false);

    final saskiaKopia = List<CartItem>.from(widget.saskia);
    final mahaiZenb = widget.mahaiZenbakia ?? 1;

    // Ordainketa amaitutakoan saskia zerotik hasi (hustu)
    CartService().clear();

    if (mounted) {
      Navigator.pushReplacement(
        context,
        MaterialPageRoute(
          builder: (_) => OrdainketaArrakastaScreen(
            emaitza: emaitza,
            bezeroIzena: widget.bezeroIzena,
            eskaeraMota: widget.eskaeraMota,
            mahaiZenbakia: mahaiZenb,
            saskia: saskiaKopia,
          ),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Ordainketa Segurua'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(18),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Zenbatekoaren eta Eskaeraren laburpena
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                gradient: const LinearGradient(
                  colors: [AppTheme.brandDark, AppTheme.brandPrimary],
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                ),
                borderRadius: BorderRadius.circular(14),
                boxShadow: const [
                  BoxShadow(
                    color: Color(0x22881337),
                    blurRadius: 10,
                    offset: Offset(0, 4),
                  ),
                ],
              ),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        widget.eskaeraMota == 'Mahaitik'
                            ? 'Mahaia #${widget.mahaiZenbakia}'
                            : 'Eramateko (Takeaway)',
                        style: const TextStyle(
                          color: AppTheme.gold,
                          fontSize: 13,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 2),
                      Text(
                        '${widget.saskia.length} plater hautatuta',
                        style: const TextStyle(color: Colors.white, fontSize: 12),
                      ),
                    ],
                  ),
                  Text(
                    '${widget.guztira.toStringAsFixed(2)} €',
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 24,
                      fontWeight: FontWeight.w900,
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 16),

            // Zibersegurtasun Txartela
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
              decoration: BoxDecoration(
                color: AppTheme.securityBg,
                borderRadius: BorderRadius.circular(10),
                border: Border.all(color: AppTheme.securityGreen.withValues(alpha: 0.3)),
              ),
              child: Row(
                children: [
                  const Icon(Icons.verified_user, color: AppTheme.securityGreen, size: 22),
                  const SizedBox(width: 10),
                  const Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          'Zibersegurtasun Bermatua (PCI-DSS & 3D Secure)',
                          style: TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.bold,
                            color: AppTheme.securityGreen,
                          ),
                        ),
                        Text(
                          '256-bit TLS enkriptazioa · Datu sentikorrak ez dira inoiz gordetzen.',
                          style: TextStyle(fontSize: 10, color: AppTheme.textMuted),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),

            const Text(
              'Hautatu Ordainketa Bidea:',
              style: TextStyle(fontSize: 15, fontWeight: FontWeight.bold, color: AppTheme.textMain),
            ),
            const SizedBox(height: 10),

            // Metodoen Aukeratzailea
            _buildMetodoAukera(
              mota: OrdainketaMota.txartela,
              titulua: 'Kreditu / Zordunketa Txartela',
              azpititulua: 'VISA, Mastercard, AMEX',
              ikonoa: Icons.credit_card,
            ),
            _buildMetodoAukera(
              mota: OrdainketaMota.bizum,
              titulua: 'Bizum',
              azpititulua: 'Berehalako ordainketa mugikorrarekin',
              ikonoa: Icons.phone_android,
            ),
            _buildMetodoAukera(
              mota: OrdainketaMota.googlePay,
              titulua: 'Google Pay / Apple Pay',
              azpititulua: 'Ordainketa biometriko azkarra (1-klik)',
              ikonoa: Icons.fingerprint,
            ),
            _buildMetodoAukera(
              mota: OrdainketaMota.paypal,
              titulua: 'PayPal',
              azpititulua: 'Ordainketa segurua zure kontuarekin',
              ikonoa: Icons.account_balance_wallet,
            ),

            const SizedBox(height: 16),

            // Hautatutako metodoaren inprimakia
            if (_hautatutakoMota == OrdainketaMota.txartela) _buildTxartelaForm(),
            if (_hautatutakoMota == OrdainketaMota.bizum) _buildBizumForm(),
            if (_hautatutakoMota == OrdainketaMota.googlePay) _buildGooglePayCard(),
            if (_hautatutakoMota == OrdainketaMota.paypal) _buildPaypalForm(),

            const SizedBox(height: 24),

            // Ordainketa Baieztatu Botoia
            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                onPressed: _prozesatzen ? null : _ordainketaBideratu,
                icon: _prozesatzen
                    ? const SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                      )
                    : const Icon(Icons.lock_outline),
                label: Text(
                  _prozesatzen
                      ? 'Zifratzen eta Prozesatzen...'
                      : 'Ordaindu Seguruki (${widget.guztira.toStringAsFixed(2)} €)',
                ),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppTheme.brandPrimary,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildMetodoAukera({
    required OrdainketaMota mota,
    required String titulua,
    required String azpititulua,
    required IconData ikonoa,
  }) {
    final hautatuta = _hautatutakoMota == mota;

    return InkWell(
      onTap: () => setState(() => _hautatutakoMota = mota),
      borderRadius: BorderRadius.circular(12),
      child: Container(
        margin: const EdgeInsets.only(bottom: 8),
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
        decoration: BoxDecoration(
          color: hautatuta ? AppTheme.brandLight : Colors.white,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(
            color: hautatuta ? AppTheme.brandPrimary : AppTheme.border,
            width: hautatuta ? 2 : 1,
          ),
        ),
        child: Row(
          children: [
            Icon(ikonoa, color: hautatuta ? AppTheme.brandPrimary : AppTheme.textMuted, size: 26),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    titulua,
                    style: TextStyle(
                      fontSize: 14,
                      fontWeight: hautatuta ? FontWeight.bold : FontWeight.w600,
                      color: hautatuta ? AppTheme.brandDark : AppTheme.textMain,
                    ),
                  ),
                  Text(azpititulua, style: const TextStyle(fontSize: 11, color: AppTheme.textMuted)),
                ],
              ),
            ),
            Icon(
              hautatuta ? Icons.radio_button_checked : Icons.radio_button_off,
              color: hautatuta ? AppTheme.brandPrimary : Colors.grey.shade400,
              size: 20,
            ),
          ],
        ),
      ),
    );
  }

  // Txartelaren Inprimakia
  Widget _buildTxartelaForm() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppTheme.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text(
                'Txartelaren Datuak',
                style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
              ),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                decoration: BoxDecoration(
                  color: AppTheme.brandLight,
                  borderRadius: BorderRadius.circular(4),
                ),
                child: Text(
                  OrdainketaService.detektatuTxartelMota(_txartelZenbController.text),
                  style: const TextStyle(
                    fontSize: 11,
                    fontWeight: FontWeight.bold,
                    color: AppTheme.brandPrimary,
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _txartelZenbController,
            keyboardType: TextInputType.number,
            maxLength: 19,
            decoration: const InputDecoration(
              labelText: 'Txartel Zenbakia',
              prefixIcon: Icon(Icons.credit_card),
              counterText: '',
            ),
            onChanged: (_) => setState(() {}),
          ),
          const SizedBox(height: 10),
          TextField(
            controller: _titularraController,
            textCapitalization: TextCapitalization.words,
            decoration: const InputDecoration(
              labelText: 'Titularraren Izena eta Abizena',
              prefixIcon: Icon(Icons.person_outline),
            ),
          ),
          const SizedBox(height: 10),
          Row(
            children: [
              Expanded(
                child: TextField(
                  controller: _iraungitzeController,
                  keyboardType: TextInputType.datetime,
                  maxLength: 5,
                  decoration: const InputDecoration(
                    labelText: 'Iraungitzea (MM/YY)',
                    prefixIcon: Icon(Icons.calendar_today_outlined),
                    counterText: '',
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: TextField(
                  controller: _cvvController,
                  keyboardType: TextInputType.number,
                  obscureText: true,
                  maxLength: 4,
                  decoration: const InputDecoration(
                    labelText: 'CVV / CVC',
                    prefixIcon: Icon(Icons.lock_outline),
                    counterText: '',
                  ),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  // Bizum Inprimakia
  Widget _buildBizumForm() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppTheme.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Row(
            children: [
              Icon(Icons.touch_app, color: AppTheme.brandPrimary, size: 20),
              SizedBox(width: 8),
              Text(
                'Bizum Zenbakia',
                style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
              ),
            ],
          ),
          const SizedBox(height: 8),
          const Text(
            'Zure bankuko Bizum aplikazioan jakinarazpen bat jasoko duzu ordainketa onartzeko.',
            style: TextStyle(fontSize: 12, color: AppTheme.textMuted),
          ),
          const SizedBox(height: 12),
          TextField(
            controller: _bizumController,
            keyboardType: TextInputType.phone,
            maxLength: 9,
            decoration: const InputDecoration(
              labelText: 'Mugikor Zenbakia (9 digitu)',
              prefixIcon: Icon(Icons.phone),
              counterText: '',
              hintText: '600123456',
            ),
          ),
        ],
      ),
    );
  }

  // Google Pay Txartela
  Widget _buildGooglePayCard() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppTheme.border),
      ),
      child: Column(
        children: [
          const Icon(Icons.fingerprint, size: 48, color: AppTheme.securityGreen),
          const SizedBox(height: 8),
          const Text(
            'Google Pay / Apple Pay Prest',
            style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 4),
          const Text(
            'Zure gailuko hatz-marka edo aurpegi-ezagutza erabiliko da token zifratu baten bidez.',
            textAlign: TextAlign.center,
            style: TextStyle(fontSize: 12, color: AppTheme.textMuted),
          ),
        ],
      ),
    );
  }

  // PayPal Inprimakia
  Widget _buildPaypalForm() {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppTheme.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'PayPal Kontua',
            style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
          ),
          const SizedBox(height: 8),
          TextField(
            controller: _paypalController,
            keyboardType: TextInputType.emailAddress,
            decoration: const InputDecoration(
              labelText: 'PayPal E-posta Helbidea',
              prefixIcon: Icon(Icons.email_outlined),
            ),
          ),
        ],
      ),
    );
  }
}
