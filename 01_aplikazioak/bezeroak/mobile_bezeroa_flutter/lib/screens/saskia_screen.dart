import 'package:flutter/material.dart';
import '../models/cart_item.dart';
import '../services/api_service.dart';
import '../services/cart_service.dart';
import '../theme/app_theme.dart';
import 'ordainketa/ordainketa_screen.dart';

class SaskiaScreen extends StatefulWidget {
  const SaskiaScreen({super.key});

  @override
  State<SaskiaScreen> createState() => _SaskiaScreenState();
}

class _SaskiaScreenState extends State<SaskiaScreen> {
  final CartService _cart = CartService();

  String _eskaeraMota = 'Mahaitik'; // 'Mahaitik' edo 'Eramateko'
  int _mahaiZenbakia = 1;
  List<Map<String, dynamic>> _mahaiak = [];
  bool _kargatzenMahaiak = true;

  final TextEditingController _izenaController = TextEditingController(text: 'Ander Urien');
  final TextEditingController _telefonoaController = TextEditingController(text: '+34 688 123 456');
  final TextEditingController _emailController = TextEditingController(text: 'ander.urien@ikasle.eus');
  final TextEditingController _oharrakController = TextEditingController();

  final _formKey = GlobalKey<FormState>();

  @override
  void initState() {
    super.initState();
    _kargatuMahaiak();
  }

  Future<void> _kargatuMahaiak() async {
    setState(() => _kargatzenMahaiak = true);
    final mahaiak = await ApiService.getMahaiak();
    setState(() {
      _mahaiak = mahaiak;
      _kargatzenMahaiak = false;
      if (_mahaiak.isNotEmpty) {
        if (!_mahaiak.any((m) => (m['zenbakia'] as int? ?? 0) == _mahaiZenbakia)) {
          _mahaiZenbakia = _mahaiak.first['zenbakia'] as int? ?? 1;
        }
      }
    });
  }

  @override
  void dispose() {
    _izenaController.dispose();
    _telefonoaController.dispose();
    _emailController.dispose();
    _oharrakController.dispose();
    super.dispose();
  }

  void _joanOrdainketara() {
    if (_cart.isEmpty) return;
    if (!_formKey.currentState!.validate()) return;

    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (_) => OrdainketaScreen(
          saskia: _cart.items,
          guztira: _cart.guztira,
          eskaeraMota: _eskaeraMota,
          mahaiZenbakia: _eskaeraMota == 'Mahaitik' ? _mahaiZenbakia : null,
          bezeroIzena: _izenaController.text.trim(),
          bezeroEmaila: _emailController.text.trim(),
          bezeroTelefonoa: _telefonoaController.text.trim(),
          oharrak: _oharrakController.text.trim(),
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
        leading: IconButton(
          icon: const Icon(Icons.arrow_back, color: Colors.white),
          onPressed: () => Navigator.pop(context),
        ),
        title: const Text(
          'Zure Saskia',
          style: TextStyle(
            color: Colors.white,
            fontWeight: FontWeight.bold,
            fontSize: 18,
          ),
        ),
        actions: [
          AnimatedBuilder(
            animation: _cart,
            builder: (context, _) {
              if (_cart.isEmpty) return const SizedBox.shrink();
              return TextButton.icon(
                onPressed: () {
                  showDialog(
                    context: context,
                    builder: (ctx) => AlertDialog(
                      title: const Text('Saskia hustu?'),
                      content: const Text('Ziur zaude eskaera guztia ezabatu nahi duzula?'),
                      actions: [
                        TextButton(
                          onPressed: () => Navigator.pop(ctx),
                          child: const Text('Utzi'),
                        ),
                        ElevatedButton(
                          onPressed: () {
                            _cart.clear();
                            Navigator.pop(ctx);
                          },
                          style: ElevatedButton.styleFrom(backgroundColor: Colors.red),
                          child: const Text('Hustu', style: TextStyle(color: Colors.white)),
                        ),
                      ],
                    ),
                  );
                },
                icon: const Icon(Icons.delete_outline, color: Colors.white70, size: 18),
                label: const Text('Hustu', style: TextStyle(color: Colors.white70, fontSize: 13)),
              );
            },
          ),
        ],
      ),
      body: AnimatedBuilder(
        animation: _cart,
        builder: (context, _) {
          if (_cart.isEmpty) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(32),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(Icons.remove_shopping_cart_outlined, size: 64, color: Colors.grey.shade400),
                    const SizedBox(height: 16),
                    const Text(
                      'Zure saskia hutsik dago',
                      style: TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                        color: AppTheme.darkCharcoal,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Arakatu gure karta eta gehitu zure gustuko plater italiar eta euskaldunak!',
                      textAlign: TextAlign.center,
                      style: TextStyle(color: Colors.grey.shade600, fontSize: 13),
                    ),
                    const SizedBox(height: 24),
                    ElevatedButton(
                      onPressed: () => Navigator.pop(context),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppTheme.chiantiWine,
                        padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                      ),
                      child: const Text('Karta Ikusi', style: TextStyle(color: Colors.white)),
                    ),
                  ],
                ),
              ),
            );
          }

          return SingleChildScrollView(
            padding: const EdgeInsets.all(16),
            child: Form(
              key: _formKey,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Eskaera mota hautatzailea
                  Container(
                    padding: const EdgeInsets.all(6),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(14),
                      border: Border.all(color: Colors.grey.shade300),
                    ),
                    child: Row(
                      children: [
                        Expanded(
                          child: _buildMotaButton('Mahaitik', 'Jatetxean (Mahaitik)', Icons.table_restaurant),
                        ),
                        const SizedBox(width: 6),
                        Expanded(
                          child: _buildMotaButton('Eramateko', 'Eramateko (Takeaway)', Icons.takeout_dining),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 16),

                  // Mahai hautaketa (Mahaitik bada)
                  if (_eskaeraMota == 'Mahaitik')
                    Container(
                      padding: const EdgeInsets.all(14),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(12),
                        border: Border.all(color: Colors.grey.shade200),
                      ),
                      child: Row(
                        children: [
                          const Icon(Icons.pin_drop, color: AppTheme.chiantiWine),
                          const SizedBox(width: 12),
                          const Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  'Mahaiaren Zenbakia',
                                  style: TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
                                ),
                                Text(
                                  'Zein mahaitan zaude eserita?',
                                  style: TextStyle(fontSize: 11, color: Colors.grey),
                                ),
                              ],
                            ),
                          ),
                          _kargatzenMahaiak
                              ? const SizedBox(
                                  width: 20,
                                  height: 20,
                                  child: CircularProgressIndicator(strokeWidth: 2, color: AppTheme.chiantiWine),
                                )
                              : DropdownButton<int>(
                                  value: _mahaiak.any((m) => (m['zenbakia'] as int?) == _mahaiZenbakia)
                                      ? _mahaiZenbakia
                                      : (_mahaiak.isNotEmpty ? (_mahaiak.first['zenbakia'] as int? ?? 1) : 1),
                                  underline: const SizedBox.shrink(),
                                  items: _mahaiak.map((m) {
                                    final zenb = m['zenbakia'] as int? ?? 1;
                                    final kokapena = m['kokapena'] as String? ?? 'Jantokia';
                                    return DropdownMenuItem<int>(
                                      value: zenb,
                                      child: Text(
                                        'Mahaia #$zenb ($kokapena)',
                                        style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13),
                                      ),
                                    );
                                  }).toList(),
                                  onChanged: (val) {
                                    if (val != null) setState(() => _mahaiZenbakia = val);
                                  },
                                ),
                        ],
                      ),
                    ),

                  const SizedBox(height: 16),
                  const Text(
                    'Aukeratutako Platerak',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: AppTheme.darkCharcoal,
                    ),
                  ),
                  const SizedBox(height: 8),

                  // Elementuen zerrenda
                  ..._cart.items.map((item) => _buildCartItemCard(item)),

                  const SizedBox(height: 16),
                  const Text(
                    'Bezeroaren Datuak',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: AppTheme.darkCharcoal,
                    ),
                  ),
                  const SizedBox(height: 8),

                  // Datuen formularioko kutxa
                  Container(
                    padding: const EdgeInsets.all(14),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(12),
                      border: Border.all(color: Colors.grey.shade200),
                    ),
                    child: Column(
                      children: [
                        TextFormField(
                          controller: _izenaController,
                          decoration: const InputDecoration(
                            labelText: 'Izena eta Abizenak *',
                            prefixIcon: Icon(Icons.person_outline, size: 20),
                            isDense: true,
                          ),
                          validator: (v) => (v == null || v.trim().isEmpty) ? 'Izena ezinbestekoa da' : null,
                        ),
                        const SizedBox(height: 12),
                        Row(
                          children: [
                            Expanded(
                              child: TextFormField(
                                controller: _telefonoaController,
                                keyboardType: TextInputType.phone,
                                decoration: const InputDecoration(
                                  labelText: 'Telefonoa *',
                                  prefixIcon: Icon(Icons.phone_outlined, size: 20),
                                  isDense: true,
                                ),
                                validator: (v) => (v == null || v.trim().isEmpty) ? 'Telefonoa beharrezkoa da' : null,
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: TextFormField(
                                controller: _emailController,
                                keyboardType: TextInputType.emailAddress,
                                decoration: const InputDecoration(
                                  labelText: 'Emaila *',
                                  prefixIcon: Icon(Icons.email_outlined, size: 20),
                                  isDense: true,
                                ),
                                validator: (v) => (v == null || !v.contains('@')) ? 'Email balioduna sartu' : null,
                              ),
                            ),
                          ],
                        ),
                        const SizedBox(height: 12),
                        TextFormField(
                          controller: _oharrakController,
                          decoration: const InputDecoration(
                            labelText: 'Eskaera Oharrak (alergia bereziak, ordua...)',
                            prefixIcon: Icon(Icons.notes, size: 20),
                            isDense: true,
                          ),
                        ),
                      ],
                    ),
                  ),

                  const SizedBox(height: 20),

                  // Prezioen laburpena
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(14),
                      border: Border.all(color: Colors.grey.shade200),
                    ),
                    child: Column(
                      children: [
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Text('Azpitotala (Janaria & Edariak)', style: TextStyle(color: Colors.grey.shade700, fontSize: 13)),
                            Text('${_cart.azpitotala.toStringAsFixed(2)} EUR', style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13)),
                          ],
                        ),
                        const SizedBox(height: 6),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Text('BEZ (%10 barne)', style: TextStyle(color: Colors.grey.shade700, fontSize: 13)),
                            Text('${_cart.bez.toStringAsFixed(2)} EUR', style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 13)),
                          ],
                        ),
                        const Divider(height: 20),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            const Text(
                              'GUZTIRA',
                              style: TextStyle(
                                fontSize: 17,
                                fontWeight: FontWeight.bold,
                                color: AppTheme.darkCharcoal,
                              ),
                            ),
                            Text(
                              '${_cart.guztira.toStringAsFixed(2)} EUR',
                              style: const TextStyle(
                                fontSize: 20,
                                fontWeight: FontWeight.w800,
                                color: AppTheme.chiantiWine,
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),

                  const SizedBox(height: 16),

                  // Zibersegurtasun oharra
                  Container(
                    padding: const EdgeInsets.all(12),
                    decoration: BoxDecoration(
                      color: AppTheme.basilGreen.withValues(alpha: 0.08),
                      borderRadius: BorderRadius.circular(10),
                      border: Border.all(color: AppTheme.basilGreen.withValues(alpha: 0.3)),
                    ),
                    child: Row(
                      children: [
                        const Icon(Icons.lock_outline, color: AppTheme.basilGreen, size: 20),
                        const SizedBox(width: 10),
                        Expanded(
                          child: Text(
                            'Ordainketa 256-bit SSL/TLS enkriptazio bidez babestuta. 3D Secure 2.0 / PSD2 segurtasuna.',
                            style: TextStyle(
                              fontSize: 11,
                              color: Colors.green.shade900,
                              fontWeight: FontWeight.w500,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),

                  const SizedBox(height: 20),

                  // Ordaintzeko botoia
                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton(
                      onPressed: _joanOrdainketara,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppTheme.chiantiWine,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                        elevation: 2,
                      ),
                      child: Text(
                        'Ordainketara Joan (${_cart.guztira.toStringAsFixed(2)} EUR)',
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                          color: Colors.white,
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(height: 24),
                ],
              ),
            ),
          );
        },
      ),
    );
  }

  Widget _buildMotaButton(String mota, String label, IconData icon) {
    final hautatua = _eskaeraMota == mota;
    return InkWell(
      onTap: () => setState(() => _eskaeraMota = mota),
      borderRadius: BorderRadius.circular(10),
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 10),
        decoration: BoxDecoration(
          color: hautatua ? AppTheme.chiantiWine : Colors.transparent,
          borderRadius: BorderRadius.circular(10),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              icon,
              size: 16,
              color: hautatua ? Colors.white : Colors.grey.shade700,
            ),
            const SizedBox(width: 6),
            Text(
              label,
              style: TextStyle(
                fontSize: 11,
                fontWeight: hautatua ? FontWeight.bold : FontWeight.normal,
                color: hautatua ? Colors.white : Colors.grey.shade800,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildCartItemCard(CartItem item) {
    return Card(
      elevation: 0.5,
      color: Colors.white,
      margin: const EdgeInsets.only(bottom: 8),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12),
        side: BorderSide(color: Colors.grey.shade200),
      ),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    item.platera.izena,
                    style: const TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.bold,
                      color: AppTheme.darkCharcoal,
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    '${item.platera.prezioa.toStringAsFixed(2)} EUR / unitatea',
                    style: TextStyle(fontSize: 11, color: Colors.grey.shade600),
                  ),
                  if (item.oharrak.isNotEmpty) ...[
                    const SizedBox(height: 4),
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
                      decoration: BoxDecoration(
                        color: Colors.amber.shade50,
                        borderRadius: BorderRadius.circular(4),
                        border: Border.all(color: Colors.amber.shade200),
                      ),
                      child: Text(
                        'Oharra: ${item.oharrak}',
                        style: TextStyle(fontSize: 11, color: Colors.amber.shade900),
                      ),
                    ),
                  ],
                ],
              ),
            ),
            const SizedBox(width: 8),
            Row(
              children: [
                IconButton(
                  icon: const Icon(Icons.remove_circle_outline, size: 20),
                  color: Colors.grey.shade600,
                  onPressed: () => _cart.updateKopurua(item.platera.id, -1),
                  constraints: const BoxConstraints(),
                  padding: const EdgeInsets.all(4),
                ),
                Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 8),
                  child: Text(
                    '${item.kopurua}',
                    style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.add_circle_outline, size: 20),
                  color: AppTheme.chiantiWine,
                  onPressed: () => _cart.updateKopurua(item.platera.id, 1),
                  constraints: const BoxConstraints(),
                  padding: const EdgeInsets.all(4),
                ),
              ],
            ),
            const SizedBox(width: 12),
            Text(
              '${item.azpitotala.toStringAsFixed(2)} EUR',
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.bold,
                color: AppTheme.chiantiWine,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
