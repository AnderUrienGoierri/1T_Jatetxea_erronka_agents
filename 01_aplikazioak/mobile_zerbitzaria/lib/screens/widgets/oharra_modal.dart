import 'package:flutter/material.dart';
import '../../theme/app_theme.dart';

class OharraModal extends StatefulWidget {
  final String platerIzena;
  final String hasierakoOharra;

  const OharraModal({
    super.key,
    required this.platerIzena,
    this.hasierakoOharra = '',
  });

  static Future<String?> show(BuildContext context, String platerIzena, String hasierakoOharra) {
    return showModalBottomSheet<String>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (ctx) => OharraModal(
        platerIzena: platerIzena,
        hasierakoOharra: hasierakoOharra,
      ),
    );
  }

  @override
  State<OharraModal> createState() => _OharraModalState();
}

class _OharraModalState extends State<OharraModal> {
  late TextEditingController _textController;

  static const List<String> _aurrezDefinituak = [
    'Gazta gutxi',
    'Gaztarik gabe',
    'Tipularik gabe',
    'Puntuan',
    'Ondo egina',
    'Gutxi egina',
    'Izotzik gabe',
    'Limoiarekin',
    'Zukua aparte',
    'Glutenik gabe',
    'Saltsa aparte',
    'Ogi gehigarria',
  ];

  @override
  void initState() {
    super.initState();
    _textController = TextEditingController(text: widget.hasierakoOharra);
  }

  @override
  void dispose() {
    _textController.dispose();
    super.dispose();
  }

  void _gehituChip(String testua) {
    String unekoa = _textController.text.trim();
    if (unekoa.isEmpty) {
      _textController.text = testua;
    } else {
      List<String> zatiak = unekoa.split(',').map((e) => e.trim()).where((e) => e.isNotEmpty).toList();
      if (zatiak.contains(testua)) {
        zatiak.remove(testua);
      } else {
        zatiak.add(testua);
      }
      _textController.text = zatiak.join(', ');
    }
    setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final bottomInset = MediaQuery.of(context).viewInsets.bottom;

    return Container(
      decoration: const BoxDecoration(
        color: AppTheme.bgCard,
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      padding: EdgeInsets.fromLTRB(20, 16, 20, 20 + bottomInset),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.start,
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
          const SizedBox(height: 14),
          Row(
            children: [
              const Icon(Icons.edit_note, color: AppTheme.brandPrimary, size: 24),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  'Oharra: ${widget.platerIzena}',
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w800,
                    color: AppTheme.textMain,
                  ),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ],
          ),
          const SizedBox(height: 6),
          const Text(
            'Hautatu aukera azkarrak edo idatzi sukalderako adierazpena:',
            style: TextStyle(fontSize: 12, color: AppTheme.textMuted),
          ),
          const SizedBox(height: 12),
          Wrap(
            spacing: 6,
            runSpacing: 6,
            children: _aurrezDefinituak.map((chipText) {
              final unekoa = _textController.text;
              final hautatuta = unekoa.contains(chipText);
              return ChoiceChip(
                label: Text(chipText),
                selected: hautatuta,
                onSelected: (_) => _gehituChip(chipText),
                selectedColor: AppTheme.warningBg,
                labelStyle: TextStyle(
                  fontSize: 12,
                  fontWeight: hautatuta ? FontWeight.w700 : FontWeight.w500,
                  color: hautatuta ? AppTheme.warningText : AppTheme.textMain,
                ),
                side: BorderSide(
                  color: hautatuta ? AppTheme.warning : AppTheme.border,
                ),
              );
            }).toList(),
          ),
          const SizedBox(height: 14),
          TextField(
            controller: _textController,
            maxLines: 2,
            maxLength: 150,
            decoration: InputDecoration(
              hintText: 'Adib.: saltsarik gabe, haragia bero-bero...',
              suffixIcon: _textController.text.isNotEmpty
                  ? IconButton(
                      icon: const Icon(Icons.clear, size: 18),
                      onPressed: () {
                        _textController.clear();
                        setState(() {});
                      },
                    )
                  : null,
            ),
          ),
          const SizedBox(height: 14),
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  onPressed: () => Navigator.pop(context, null),
                  child: const Text('Utzi'),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: ElevatedButton(
                  onPressed: () {
                    Navigator.pop(context, _textController.text.trim());
                  },
                  child: const Text('Gorde Oharra'),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
