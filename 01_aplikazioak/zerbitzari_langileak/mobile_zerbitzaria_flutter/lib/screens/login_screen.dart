import 'package:flutter/material.dart';
import '../models/langilea.dart';
import '../services/api_service.dart';
import '../theme/app_theme.dart';
import 'mahaiak_screen.dart';
import 'widgets/server_settings_dialog.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  List<Langilea> _langileak = [];
  Langilea? _hautatutakoLangilea;
  String _pin = '';
  bool _kargatzen = false;
  String? _erroreMezua;
  bool _apiLinean = false;

  @override
  void initState() {
    super.initState();
    _kargatuDatuak();
  }

  Future<void> _kargatuDatuak() async {
    setState(() {
      _kargatzen = true;
      _erroreMezua = null;
    });

    final (apiOk, _, _) = await ApiService.checkHealth();
    final zerrenda = await ApiService.getLangileak();

    setState(() {
      _apiLinean = apiOk;
      _langileak = zerrenda;
      if (_langileak.isNotEmpty && _hautatutakoLangilea == null) {
        _hautatutakoLangilea = _langileak.first;
      }
      _kargatzen = false;
    });
  }

  void _sartuZenbakia(String digitua) {
    if (_pin.length < 4) {
      setState(() {
        _pin += digitua;
        _erroreMezua = null;
      });
      if (_pin.length == 4) {
        _loginEgin();
      }
    }
  }

  void _ezabatuZenbakia() {
    if (_pin.isNotEmpty) {
      setState(() {
        _pin = _pin.substring(0, _pin.length - 1);
        _erroreMezua = null;
      });
    }
  }

  void _garbituPIN() {
    setState(() {
      _pin = '';
      _erroreMezua = null;
    });
  }

  Future<void> _loginEgin() async {
    if (_hautatutakoLangilea == null) {
      setState(() => _erroreMezua = 'Mesedez, aukeratu zerbitzari bat.');
      return;
    }

    setState(() => _kargatzen = true);

    final (arrakasta, mezua, langilea) = await ApiService.login(
      _hautatutakoLangilea!.erabiltzailea,
      _pin,
    );

    setState(() => _kargatzen = false);

    if (arrakasta && langilea != null) {
      if (mounted) {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(
            builder: (_) => MahaiakScreen(langilea: langilea),
          ),
        );
      }
    } else {
      setState(() {
        _erroreMezua = mezua;
        _pin = '';
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Row(
          children: [
            Text('T1 JATETXEA'),
            SizedBox(width: 8),
            Text(
              '· Zerbitzariak',
              style: TextStyle(fontSize: 13, color: AppTheme.gold, fontWeight: FontWeight.normal),
            ),
          ],
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.settings),
            tooltip: 'API Ezarpenak',
            onPressed: () async {
              final aldatuDa = await showDialog<bool>(
                context: context,
                builder: (_) => const ServerSettingsDialog(),
              );
              if (aldatuDa == true) {
                _kargatuDatuak();
              }
            },
          ),
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: 'Freskatu',
            onPressed: _kargatuDatuak,
          ),
        ],
      ),
      body: SafeArea(
        child: _kargatzen && _langileak.isEmpty
            ? const Center(child: CircularProgressIndicator())
            : SingleChildScrollView(
                padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 20),
                child: Column(
                  children: [
                    // API Egoera
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                      decoration: BoxDecoration(
                        color: _apiLinean ? AppTheme.successBg : AppTheme.dangerBg,
                        borderRadius: BorderRadius.circular(999),
                        border: Border.all(
                          color: _apiLinean ? AppTheme.successBorder : AppTheme.dangerBorder,
                        ),
                      ),
                      child: Row(
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Container(
                            width: 8,
                            height: 8,
                            decoration: BoxDecoration(
                              shape: BoxShape.circle,
                              color: _apiLinean ? AppTheme.success : AppTheme.danger,
                            ),
                          ),
                          const SizedBox(width: 6),
                          Text(
                            _apiLinean ? 'API Linean (${ApiService.baseUrl})' : 'API Deskonektatuta',
                            style: TextStyle(
                              fontSize: 11,
                              fontWeight: FontWeight.bold,
                              color: _apiLinean ? AppTheme.success : AppTheme.danger,
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 16),
                    const Text(
                      'Hautatu Zerbitzaria',
                      style: TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.w800,
                        color: AppTheme.textMain,
                      ),
                    ),
                    const SizedBox(height: 12),
                    // Zerbitzarien txartel-aukeraketa
                    if (_langileak.isEmpty)
                      Container(
                        padding: const EdgeInsets.all(14),
                        decoration: BoxDecoration(
                          color: AppTheme.warningBg,
                          borderRadius: BorderRadius.circular(10),
                          border: Border.all(color: AppTheme.warning),
                        ),
                        child: const Text(
                          'Ezin izan dira langileak kargatu. Egiaztatu goiko gurpiltxoan (ezarpenak) API helbidea zuzena dela.',
                          style: TextStyle(fontSize: 12, color: AppTheme.warningText),
                          textAlign: TextAlign.center,
                        ),
                      )
                    else
                      SizedBox(
                        height: 70,
                        child: ListView.separated(
                          scrollDirection: Axis.horizontal,
                          itemCount: _langileak.length,
                          separatorBuilder: (_, i) => const SizedBox(width: 10),
                          itemBuilder: (ctx, idx) {
                            final l = _langileak[idx];
                            final hautatuta = _hautatutakoLangilea?.id == l.id;
                            return InkWell(
                              onTap: () {
                                setState(() {
                                  _hautatutakoLangilea = l;
                                  _pin = '';
                                  _erroreMezua = null;
                                });
                              },
                              borderRadius: BorderRadius.circular(10),
                              child: Container(
                                padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
                                decoration: BoxDecoration(
                                  color: hautatuta ? AppTheme.brandPrimary : Colors.white,
                                  borderRadius: BorderRadius.circular(10),
                                  border: Border.all(
                                    color: hautatuta ? AppTheme.brandDark : AppTheme.border,
                                    width: hautatuta ? 2 : 1,
                                  ),
                                  boxShadow: [
                                    if (hautatuta)
                                      BoxShadow(
                                        color: AppTheme.brandPrimary.withValues(alpha: 0.3),
                                        blurRadius: 8,
                                        offset: const Offset(0, 3),
                                      ),
                                  ],
                                ),
                                child: Row(
                                  children: [
                                    CircleAvatar(
                                      radius: 16,
                                      backgroundColor: hautatuta ? Colors.white : AppTheme.brandLight,
                                      child: Text(
                                        l.izena.isNotEmpty ? l.izena[0] : '?',
                                        style: TextStyle(
                                          fontWeight: FontWeight.bold,
                                          color: hautatuta ? AppTheme.brandPrimary : AppTheme.brandDark,
                                          fontSize: 14,
                                        ),
                                      ),
                                    ),
                                    const SizedBox(width: 10),
                                    Column(
                                      mainAxisAlignment: MainAxisAlignment.center,
                                      crossAxisAlignment: CrossAxisAlignment.start,
                                      children: [
                                        Text(
                                          l.izena,
                                          style: TextStyle(
                                            fontWeight: FontWeight.bold,
                                            fontSize: 13,
                                            color: hautatuta ? Colors.white : AppTheme.textMain,
                                          ),
                                        ),
                                        Text(
                                          l.rola,
                                          style: TextStyle(
                                            fontSize: 11,
                                            color: hautatuta ? AppTheme.gold : AppTheme.textMuted,
                                          ),
                                        ),
                                      ],
                                    ),
                                  ],
                                ),
                              ),
                            );
                          },
                        ),
                      ),
                    const SizedBox(height: 20),
                    // PIN puntuak
                    Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: List.generate(4, (index) {
                        final beteta = index < _pin.length;
                        return Container(
                          margin: const EdgeInsets.symmetric(horizontal: 8),
                          width: 16,
                          height: 16,
                          decoration: BoxDecoration(
                            shape: BoxShape.circle,
                            color: beteta ? AppTheme.brandPrimary : Colors.transparent,
                            border: Border.all(
                              color: beteta ? AppTheme.brandPrimary : AppTheme.border,
                              width: 2,
                            ),
                          ),
                        );
                      }),
                    ),
                    if (_erroreMezua != null) ...[
                      const SizedBox(height: 10),
                      Text(
                        _erroreMezua!,
                        style: const TextStyle(
                          color: AppTheme.danger,
                          fontSize: 12,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                    const SizedBox(height: 14),
                    const Text(
                      'PIN lehenetsia: 1234',
                      style: TextStyle(fontSize: 11, color: AppTheme.textMuted),
                    ),
                    const SizedBox(height: 16),
                    // Teklatu numerikoa
                    _buildTeklatua(),
                  ],
                ),
              ),
      ),
    );
  }

  Widget _buildTeklatua() {
    return Container(
      constraints: const BoxConstraints(maxWidth: 320),
      child: Column(
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: ['1', '2', '3'].map((d) => _buildTekla(d)).toList(),
          ),
          const SizedBox(height: 10),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: ['4', '5', '6'].map((d) => _buildTekla(d)).toList(),
          ),
          const SizedBox(height: 10),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: ['7', '8', '9'].map((d) => _buildTekla(d)).toList(),
          ),
          const SizedBox(height: 10),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: [
              _buildTeklaSpecial(
                label: 'C',
                onPressed: _garbituPIN,
                color: AppTheme.warning,
              ),
              _buildTekla('0'),
              _buildTeklaSpecial(
                icon: Icons.backspace_outlined,
                onPressed: _ezabatuZenbakia,
                color: AppTheme.textMuted,
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildTekla(String digitua) {
    return InkWell(
      onTap: () => _sartuZenbakia(digitua),
      borderRadius: BorderRadius.circular(35),
      child: Container(
        width: 68,
        height: 68,
        decoration: BoxDecoration(
          color: Colors.white,
          shape: BoxShape.circle,
          border: Border.all(color: AppTheme.border),
          boxShadow: const [
            BoxShadow(
              color: Color(0x0A000000),
              blurRadius: 4,
              offset: Offset(0, 2),
            ),
          ],
        ),
        alignment: Alignment.center,
        child: Text(
          digitua,
          style: const TextStyle(
            fontSize: 24,
            fontWeight: FontWeight.bold,
            color: AppTheme.textMain,
          ),
        ),
      ),
    );
  }

  Widget _buildTeklaSpecial({String? label, IconData? icon, required VoidCallback onPressed, required Color color}) {
    return InkWell(
      onTap: onPressed,
      borderRadius: BorderRadius.circular(35),
      child: Container(
        width: 68,
        height: 68,
        decoration: BoxDecoration(
          color: color.withValues(alpha: 0.08),
          shape: BoxShape.circle,
        ),
        alignment: Alignment.center,
        child: label != null
            ? Text(
                label,
                style: TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                  color: color,
                ),
              )
            : Icon(icon, color: color, size: 22),
      ),
    );
  }
}
