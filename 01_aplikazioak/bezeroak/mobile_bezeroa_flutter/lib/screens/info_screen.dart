import 'package:flutter/material.dart';
import '../services/api_service.dart';
import '../theme/app_theme.dart';
import 'widgets/header_cart_button.dart';

class InfoScreen extends StatefulWidget {
  const InfoScreen({super.key});

  @override
  State<InfoScreen> createState() => _InfoScreenState();
}

class _InfoScreenState extends State<InfoScreen> {
  final TextEditingController _urlController = TextEditingController();
  bool _egiaztatzen = false;
  String? _osasunEgoera;
  bool? _konektatuta;

  @override
  void initState() {
    super.initState();
    _urlController.text = ApiService.baseUrl;
  }

  @override
  void dispose() {
    _urlController.dispose();
    super.dispose();
  }

  Future<void> _egiaztatuKonexioa() async {
    setState(() {
      _egiaztatzen = true;
      _osasunEgoera = null;
    });

    await ApiService.setBaseUrl(_urlController.text.trim());
    final (apiOk, dbOk, mezua) = await ApiService.checkHealth();

    setState(() {
      _egiaztatzen = false;
      _konektatuta = apiOk && dbOk;
      _osasunEgoera = mezua;
    });
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
              'Jatetxearen Informazioa',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Colors.white),
            ),
            Text(
              'T1 Jatetxea · Osteria & Parrilla',
              style: TextStyle(fontSize: 11, color: AppTheme.parmigianoGold),
            ),
          ],
        ),
        actions: const [
          HeaderCartButton(),
          SizedBox(width: 6),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Aurkezpen txartela
            Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                gradient: const LinearGradient(
                  colors: [AppTheme.chiantiWine, Color(0xFF5A0C23)],
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                ),
                borderRadius: BorderRadius.circular(16),
                boxShadow: [
                  BoxShadow(
                    color: AppTheme.chiantiWine.withValues(alpha: 0.25),
                    blurRadius: 10,
                    offset: const Offset(0, 4),
                  ),
                ],
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                    decoration: BoxDecoration(
                      color: AppTheme.parmigianoGold,
                      borderRadius: BorderRadius.circular(6),
                    ),
                    child: const Text(
                      'EST. 2026 · GOIERRI',
                      style: TextStyle(
                        fontSize: 10,
                        fontWeight: FontWeight.w900,
                        color: AppTheme.darkCharcoal,
                        letterSpacing: 1.1,
                      ),
                    ),
                  ),
                  const SizedBox(height: 12),
                  const Text(
                    'T1 JATETXEA',
                    style: TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.w900,
                      color: Colors.white,
                      letterSpacing: 1.5,
                    ),
                  ),
                  const Text(
                    'Osteria & Parrilla Italo-Vasca',
                    style: TextStyle(
                      fontSize: 14,
                      color: AppTheme.parmigianoGold,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(height: 12),
                  Text(
                    'Tradizio toskanarra eta euskal parrillaren arteko fusio gastronomiko paregabea. '
                    'Egunero eskuz egindako pasta freskoak, egur-labean erretako pizzak, '
                    'eta Goierriko txuleta eta arrain freskoak parrillan.',
                    style: TextStyle(
                      fontSize: 12,
                      color: Colors.white.withValues(alpha: 0.9),
                      height: 1.5,
                    ),
                  ),
                ],
              ),
            ),

            const SizedBox(height: 20),

            // Ordutegia
            _buildInfoAtala(
              titulua: 'Ordutegiak',
              ikonoa: Icons.access_time_rounded,
              edukia: Column(
                children: [
                  _buildOrdutegiLerroa('Asteartetik Ostiralera', '13:30 - 16:30 | 20:30 - 23:30'),
                  const Divider(height: 12),
                  _buildOrdutegiLerroa('Larunbat eta Igandeetan', '13:00 - 17:00 | 20:30 - 00:00'),
                  const Divider(height: 12),
                  _buildOrdutegiLerroa('Astelehenetan', 'ITXITA (Langileen atsedena)', kolorea: AppTheme.terracotta),
                ],
              ),
            ),

            const SizedBox(height: 16),

            // Kokapena eta Kontaktua
            _buildInfoAtala(
              titulua: 'Kokapena eta Kontaktua',
              ikonoa: Icons.location_on_outlined,
              edukia: Column(
                children: [
                  _buildKontaktuLerroa(Icons.place_outlined, 'Helbidea', 'Kale Nagusia 14, Ordizia (Gipuzkoa)'),
                  const Divider(height: 12),
                  _buildKontaktuLerroa(Icons.phone_outlined, 'Telefonoa', '+34 943 12 34 56'),
                  const Divider(height: 12),
                  _buildKontaktuLerroa(Icons.email_outlined, 'Emaila', 'kontaktua@t1jatetxea.eus'),
                  const Divider(height: 12),
                  _buildKontaktuLerroa(Icons.language_outlined, 'Webgunea', 'https://www.t1jatetxea.eus'),
                ],
              ),
            ),

            const SizedBox(height: 16),

            // Segurtasuna eta Ziurtagiriak
            _buildInfoAtala(
              titulua: 'Segurtasuna eta Ordainketak',
              ikonoa: Icons.shield_outlined,
              edukia: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _buildSegurtasunElementua(
                    'PCI-DSS Maila 1 & Tokenizazioa',
                    'Kreditu txartelen zenbakiak ez dira inoiz zerbitzarian gordetzen; SHA-256 / AES-256 tokenizazio bidez babestuta daude.',
                  ),
                  const SizedBox(height: 8),
                  _buildSegurtasunElementua(
                    'PSD2 / 3D Secure 2.0',
                    'Banku igorlearen SMS OTP kode egiaztapena bermatzen du iruzurrezko karguak ekiditeko.',
                  ),
                  const SizedBox(height: 8),
                  _buildSegurtasunElementua(
                    'Ordainketa Anitzak',
                    'VISA, Mastercard, American Express, Bizum, Apple Pay, Google Pay eta PayPal.',
                  ),
                ],
              ),
            ),

            const SizedBox(height: 16),

            // Zerbitzariaren Ezarpenak (API Konexioa)
            _buildInfoAtala(
              titulua: 'Zerbitzariaren Ezarpenak (API)',
              ikonoa: Icons.settings_ethernet,
              edukia: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Aplikazioak komunikatzeko erabiltzen duen REST APIaren helbidea:',
                    style: TextStyle(fontSize: 12, color: Colors.grey.shade700),
                  ),
                  const SizedBox(height: 10),
                  TextField(
                    controller: _urlController,
                    decoration: InputDecoration(
                      isDense: true,
                      prefixIcon: const Icon(Icons.link, size: 20),
                      border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
                      hintText: 'http://10.0.2.2:5000/api/v1',
                    ),
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      ElevatedButton.icon(
                        onPressed: _egiaztatzen ? null : _egiaztatuKonexioa,
                        style: ElevatedButton.styleFrom(
                          backgroundColor: AppTheme.chiantiWine,
                          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                        ),
                        icon: _egiaztatzen
                            ? const SizedBox(
                                width: 14,
                                height: 14,
                                child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                              )
                            : const Icon(Icons.refresh, size: 16, color: Colors.white),
                        label: const Text('Gorde eta Egiaztatu', style: TextStyle(color: Colors.white, fontSize: 12)),
                      ),
                      const SizedBox(width: 12),
                      if (_konektatuta != null)
                        Expanded(
                          child: Text(
                            _konektatuta! ? 'Konektatuta' : 'Deskonektatuta',
                            style: TextStyle(
                              fontSize: 12,
                              fontWeight: FontWeight.bold,
                              color: _konektatuta! ? AppTheme.basilGreen : Colors.red,
                            ),
                          ),
                        ),
                    ],
                  ),
                  if (_osasunEgoera != null) ...[
                    const SizedBox(height: 8),
                    Text(
                      _osasunEgoera!,
                      style: TextStyle(
                        fontSize: 11,
                        color: _konektatuta == true ? Colors.green.shade800 : Colors.red.shade800,
                      ),
                    ),
                  ],
                ],
              ),
            ),

            const SizedBox(height: 24),
            Center(
              child: Text(
                'T1 Jatetxea v1.0.0 · Bezeroen Aplikazio Digitala',
                style: TextStyle(fontSize: 11, color: Colors.grey.shade500),
              ),
            ),
            const SizedBox(height: 16),
          ],
        ),
      ),
    );
  }

  Widget _buildInfoAtala({
    required String titulua,
    required IconData ikonoa,
    required Widget edukia,
  }) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: Colors.grey.shade200),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(ikonoa, color: AppTheme.chiantiWine, size: 20),
              const SizedBox(width: 8),
              Text(
                titulua,
                style: const TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.bold,
                  color: AppTheme.darkCharcoal,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          edukia,
        ],
      ),
    );
  }

  Widget _buildOrdutegiLerroa(String eguna, String orduak, {Color? kolorea}) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(eguna, style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600)),
        Text(
          orduak,
          style: TextStyle(
            fontSize: 12,
            fontWeight: FontWeight.w500,
            color: kolorea ?? Colors.grey.shade700,
          ),
        ),
      ],
    );
  }

  Widget _buildKontaktuLerroa(IconData ikonoa, String etiketa, String balioa) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Icon(ikonoa, size: 16, color: Colors.grey.shade600),
        const SizedBox(width: 10),
        SizedBox(
          width: 75,
          child: Text(
            etiketa,
            style: const TextStyle(fontSize: 12, color: Colors.grey),
          ),
        ),
        Expanded(
          child: Text(
            balioa,
            style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600),
          ),
        ),
      ],
    );
  }

  Widget _buildSegurtasunElementua(String titulua, String deskribapena) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Icon(Icons.check_circle, size: 16, color: AppTheme.basilGreen),
        const SizedBox(width: 8),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                titulua,
                style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold),
              ),
              Text(
                deskribapena,
                style: TextStyle(fontSize: 11, color: Colors.grey.shade600, height: 1.3),
              ),
            ],
          ),
        ),
      ],
    );
  }
}
