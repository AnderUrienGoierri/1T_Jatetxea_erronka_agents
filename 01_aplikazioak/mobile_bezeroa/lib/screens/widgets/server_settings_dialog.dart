import 'package:flutter/material.dart';
import '../../services/api_service.dart';
import '../../theme/app_theme.dart';

class ServerSettingsDialog extends StatefulWidget {
  const ServerSettingsDialog({super.key});

  @override
  State<ServerSettingsDialog> createState() => _ServerSettingsDialogState();
}

class _ServerSettingsDialogState extends State<ServerSettingsDialog> {
  late TextEditingController _controller;
  bool _probakEgiten = false;
  String? _emaitzaMezua;
  bool _arrakasta = false;

  @override
  void initState() {
    super.initState();
    _controller = TextEditingController(text: ApiService.baseUrl);
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  Future<void> _testConnection() async {
    setState(() {
      _probakEgiten = true;
      _emaitzaMezua = null;
    });

    await ApiService.setBaseUrl(_controller.text);
    final (apiOk, dbOk, msg) = await ApiService.checkHealth();

    setState(() {
      _probakEgiten = false;
      _arrakasta = apiOk && dbOk;
      _emaitzaMezua = msg;
    });
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Row(
        children: [
          Icon(Icons.settings_ethernet, color: AppTheme.chiantiWine),
          SizedBox(width: 8),
          Expanded(
            child: Text(
              'API Zerbitzariaren Ezarpenak',
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
          ),
        ],
      ),
      content: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Zehaztu ordenagailuko REST API helbidea:',
              style: TextStyle(fontSize: 13, color: Colors.grey),
            ),
            const SizedBox(height: 10),
            TextField(
              controller: _controller,
              decoration: const InputDecoration(
                labelText: 'API Helbidea (URL)',
                hintText: 'http://192.168.1.129:5000/api/v1',
                prefixIcon: Icon(Icons.link),
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.url,
            ),
            const SizedBox(height: 12),
            const Text(
              'Aukera Azkarrak:',
              style: TextStyle(fontSize: 12, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 6),
            Wrap(
              spacing: 6,
              runSpacing: 6,
              children: [
                ActionChip(
                  label: const Text('Wi-Fi PC (192.168.1.129)'),
                  onPressed: () {
                    _controller.text = 'http://192.168.1.129:5000/api/v1';
                  },
                ),
                ActionChip(
                  label: const Text('Emuladorea (10.0.2.2)'),
                  onPressed: () {
                    _controller.text = 'http://10.0.2.2:5000/api/v1';
                  },
                ),
                ActionChip(
                  label: const Text('Localhost (5000)'),
                  onPressed: () {
                    _controller.text = 'http://localhost:5000/api/v1';
                  },
                ),
              ],
            ),
            if (_emaitzaMezua != null) ...[
              const SizedBox(height: 14),
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: _arrakasta ? const Color(0xFFDCFCE7) : const Color(0xFFFEE2E2),
                  borderRadius: BorderRadius.circular(8),
                  border: Border.all(
                    color: _arrakasta ? const Color(0xFF86EFAC) : const Color(0xFFFECACA),
                  ),
                ),
                child: Row(
                  children: [
                    Icon(
                      _arrakasta ? Icons.check_circle : Icons.error,
                      color: _arrakasta ? const Color(0xFF15803D) : const Color(0xFFDC2626),
                      size: 20,
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        _emaitzaMezua!,
                        style: TextStyle(
                          fontSize: 12,
                          color: _arrakasta ? const Color(0xFF15803D) : const Color(0xFFDC2626),
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context, false),
          child: const Text('Utzi'),
        ),
        OutlinedButton(
          onPressed: _probakEgiten ? null : _testConnection,
          child: _probakEgiten
              ? const SizedBox(
                  width: 14,
                  height: 14,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
              : const Text('Probatu Konexioa'),
        ),
        ElevatedButton(
          style: ElevatedButton.styleFrom(
            backgroundColor: AppTheme.chiantiWine,
            foregroundColor: Colors.white,
          ),
          onPressed: () async {
            await ApiService.setBaseUrl(_controller.text);
            if (context.mounted) {
              Navigator.pop(context, true);
            }
          },
          child: const Text('Gorde'),
        ),
      ],
    );
  }
}
