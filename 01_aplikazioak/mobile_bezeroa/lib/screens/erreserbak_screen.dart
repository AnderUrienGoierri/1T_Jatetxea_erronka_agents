import 'package:flutter/material.dart';
import '../models/erreserba.dart';
import '../services/api_service.dart';
import '../theme/app_theme.dart';
import 'widgets/header_cart_button.dart';

class ErreserbakScreen extends StatefulWidget {
  const ErreserbakScreen({super.key});

  @override
  State<ErreserbakScreen> createState() => _ErreserbakScreenState();
}

class _ErreserbakScreenState extends State<ErreserbakScreen> {
  final _formKey = GlobalKey<FormState>();

  static DateTime _hasierakoData() {
    var d = DateTime.now().add(const Duration(days: 1));
    if (d.weekday == DateTime.monday) {
      d = d.add(const Duration(days: 1));
    }
    return d;
  }

  late DateTime _hautatutakoData;
  String _hautatutakoTxanda = 'Bazkaria_1';
  int _pertsonaKopurua = 2;
  String _hautatutakoKokapena = 'Guztiak';

  final List<String> _kokapenak = const [
    'Guztiak',
    'Jantoki Nagusia',
    'Terraza',
    'Barra',
    'VIP',
  ];

  List<Map<String, dynamic>> _mahaiak = [];
  int? _hautatutakoMahaiaId;
  int? _hautatutakoMahaiZenbakia;
  bool _kargatzenMahaiak = false;

  final TextEditingController _izenaController = TextEditingController(text: 'Ander');
  final TextEditingController _abizenaController = TextEditingController(text: 'Urien');
  final TextEditingController _emailController = TextEditingController(text: 'ander.urien@ikasle.eus');
  final TextEditingController _telefonoaController = TextEditingController(text: '+34 688 123 456');
  final TextEditingController _oharrakController = TextEditingController();

  bool _bidaltzen = false;
  String? _arrakastaMezua;
  String? _erroreMezua;

  final Map<String, String> _txandak = {
    'Bazkaria_1': 'Bazkaria 1. txanda (13:30 - 15:00)',
    'Bazkaria_2': 'Bazkaria 2. txanda (15:00 - 16:30)',
    'Afaria_1': 'Afaria 1. txanda (20:30 - 22:00)',
    'Afaria_2': 'Afaria 2. txanda (22:00 - 23:30)',
  };

  @override
  void initState() {
    super.initState();
    _hautatutakoData = _hasierakoData();
    _kargatuMahaiak();
  }

  @override
  void dispose() {
    _izenaController.dispose();
    _abizenaController.dispose();
    _emailController.dispose();
    _telefonoaController.dispose();
    _oharrakController.dispose();
    super.dispose();
  }

  Future<void> _kargatuMahaiak() async {
    setState(() => _kargatzenMahaiak = true);
    final mahaiak = await ApiService.getMahaiak(
      data: _formatData(_hautatutakoData),
      txanda: _hautatutakoTxanda,
      pertsonaKopurua: _pertsonaKopurua,
      kokapena: _hautatutakoKokapena,
    );
    setState(() {
      _mahaiak = mahaiak;
      _kargatzenMahaiak = false;
      if (_hautatutakoMahaiaId != null && !_mahaiak.any((m) => m['id'] == _hautatutakoMahaiaId)) {
        _hautatutakoMahaiaId = null;
        _hautatutakoMahaiZenbakia = null;
      }
    });
  }

  Future<void> _aukeratuData() async {
    DateTime initial = _hautatutakoData;
    if (initial.weekday == DateTime.monday) {
      initial = initial.add(const Duration(days: 1));
    }

    final aukera = await showDatePicker(
      context: context,
      initialDate: initial,
      firstDate: DateTime.now(),
      lastDate: DateTime.now().add(const Duration(days: 90)),
      selectableDayPredicate: (DateTime day) {
        // Astelehenetan ITXITA (atseden eguna)
        return day.weekday != DateTime.monday;
      },
      helpText: 'HAUTATU ERRESERBA EGUNA (Astelehenetan itxita)',
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: const ColorScheme.light(
              primary: AppTheme.chiantiWine,
              onPrimary: Colors.white,
              surface: Colors.white,
              onSurface: AppTheme.darkCharcoal,
            ),
          ),
          child: child!,
        );
      },
    );

    if (aukera != null) {
      setState(() {
        _hautatutakoData = aukera;
        _hautatutakoMahaiaId = null;
        _hautatutakoMahaiZenbakia = null;
      });
      _kargatuMahaiak();
    }
  }

  String _formatData(DateTime data) {
    final y = data.year.toString().padLeft(4, '0');
    final m = data.month.toString().padLeft(2, '0');
    final d = data.day.toString().padLeft(2, '0');
    return '$y-$m-$d';
  }

  Future<void> _bidaliErreserba() async {
    if (!_formKey.currentState!.validate()) return;

    if (_hautatutakoData.weekday == DateTime.monday) {
      setState(() {
        _erroreMezua = 'Astelehenetan jatetxea itxita dago (atseden eguna). Mesedez, aukeratu beste egun bat.';
      });
      return;
    }

    setState(() {
      _bidaltzen = true;
      _arrakastaMezua = null;
      _erroreMezua = null;
    });

    String? oharrak = _oharrakController.text.trim().isNotEmpty ? _oharrakController.text.trim() : null;
    if (_hautatutakoKokapena != 'Guztiak') {
      final tag = '[Kokapena: $_hautatutakoKokapena]';
      oharrak = oharrak != null ? '$tag $oharrak' : tag;
    }

    final (ondo, mezua, erreserba) = await ApiService.sortuErreserba(
      data: _formatData(_hautatutakoData),
      txanda: _hautatutakoTxanda,
      pertsonaKopurua: _pertsonaKopurua,
      izena: _izenaController.text.trim(),
      abizena: _abizenaController.text.trim(),
      emaila: _emailController.text.trim(),
      telefonoa: _telefonoaController.text.trim(),
      mahaiaId: _hautatutakoMahaiaId,
      mahaiZenbakia: _hautatutakoMahaiZenbakia,
      oharrak: oharrak,
    );

    setState(() {
      _bidaltzen = false;
      if (ondo) {
        _arrakastaMezua = mezua;
        _erakutsiBaieztapenModala(erreserba);
      } else {
        _erroreMezua = mezua
            .replaceAll('(MahaiaId edo MahaiZenbakia)', '')
            .replaceAll('(mahaiaId edo mahaiZenbakia)', '')
            .trim();
      }
    });
  }

  void _erakutsiBaieztapenModala(Erreserba? erreserba) {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (ctx) => AlertDialog(
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: Row(
          children: const [
            Icon(Icons.check_circle_outline, color: AppTheme.basilGreen, size: 28),
            SizedBox(width: 10),
            Text(
              'Erreserba Baieztatuta!',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: AppTheme.darkCharcoal,
              ),
            ),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Container(
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: AppTheme.warmBackground,
                borderRadius: BorderRadius.circular(10),
                border: Border.all(color: AppTheme.parmigianoGold.withValues(alpha: 0.5)),
              ),
              child: Column(
                children: [
                  _infoErreserbaLerroa('Bezeroa', '${_izenaController.text} ${_abizenaController.text}'),
                  _infoErreserbaLerroa('Data', _formatData(_hautatutakoData)),
                  _infoErreserbaLerroa('Txanda', _txandak[_hautatutakoTxanda] ?? _hautatutakoTxanda),
                  _infoErreserbaLerroa('Mahaikideak', '$_pertsonaKopurua pertsona'),
                  if (_hautatutakoMahaiZenbakia != null)
                    _infoErreserbaLerroa('Mahaia', 'Mahaia #$_hautatutakoMahaiZenbakia'),
                ],
              ),
            ),
            const SizedBox(height: 12),
            Text(
              'Baieztapen mezua ${_emailController.text} helbidera eta SMS bidez bidali da.',
              style: TextStyle(fontSize: 12, color: Colors.grey.shade700),
            ),
          ],
        ),
        actions: [
          ElevatedButton(
            onPressed: () {
              Navigator.pop(ctx);
              _oharrakController.clear();
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: AppTheme.chiantiWine,
              shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
            ),
            child: const Text('Itxi', style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
    );
  }

  Widget _infoErreserbaLerroa(String etiketa, String balioa) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 3),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(etiketa, style: const TextStyle(fontSize: 12, color: Colors.grey)),
          Text(balioa, style: const TextStyle(fontSize: 12, fontWeight: FontWeight.bold)),
        ],
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
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: const [
            Text(
              'Erreserbatu Mahaia',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: Colors.white,
              ),
            ),
            Text(
              'T1 Jatetxea · Osteria & Parrilla',
              style: TextStyle(
                fontSize: 11,
                color: AppTheme.parmigianoGold,
              ),
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
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Alertekin kudeaketa (409 Conflict edo erroreak)
              if (_erroreMezua != null)
                Container(
                  margin: const EdgeInsets.only(bottom: 16),
                  padding: const EdgeInsets.all(14),
                  decoration: BoxDecoration(
                    color: Colors.red.shade50,
                    borderRadius: BorderRadius.circular(10),
                    border: Border.all(color: Colors.red.shade300),
                  ),
                  child: Row(
                    children: [
                      const Icon(Icons.error_outline, color: Colors.red, size: 24),
                      const SizedBox(width: 10),
                      Expanded(
                        child: Text(
                          _erroreMezua!,
                          style: TextStyle(
                            fontSize: 13,
                            color: Colors.red.shade900,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),

              if (_arrakastaMezua != null)
                Container(
                  margin: const EdgeInsets.only(bottom: 16),
                  padding: const EdgeInsets.all(14),
                  decoration: BoxDecoration(
                    color: Colors.green.shade50,
                    borderRadius: BorderRadius.circular(10),
                    border: Border.all(color: Colors.green.shade300),
                  ),
                  child: Row(
                    children: [
                      const Icon(Icons.check_circle_outline, color: AppTheme.basilGreen, size: 24),
                      const SizedBox(width: 10),
                      Expanded(
                        child: Text(
                          _arrakastaMezua!,
                          style: TextStyle(
                            fontSize: 13,
                            color: Colors.green.shade900,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),

              // 1. Data eta Txanda
              const Text(
                '1. Data eta Txanda',
                style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.bold,
                  color: AppTheme.darkCharcoal,
                ),
              ),
              const SizedBox(height: 8),
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: Colors.grey.shade200),
                ),
                child: Column(
                  children: [
                    // Data aukeraketa
                    InkWell(
                      onTap: _aukeratuData,
                      borderRadius: BorderRadius.circular(8),
                      child: Container(
                        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 12),
                        decoration: BoxDecoration(
                          border: Border.all(color: Colors.grey.shade300),
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: Row(
                          children: [
                            const Icon(Icons.calendar_today, color: AppTheme.chiantiWine, size: 20),
                            const SizedBox(width: 12),
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  const Text('Data', style: TextStyle(fontSize: 11, color: Colors.grey)),
                                  Text(
                                    _formatData(_hautatutakoData),
                                    style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                                  ),
                                ],
                              ),
                            ),
                            const Text('Aldatu', style: TextStyle(color: AppTheme.chiantiWine, fontSize: 13, fontWeight: FontWeight.w600)),
                          ],
                        ),
                      ),
                    ),
                    const SizedBox(height: 12),

                    // Txanda dropdown
                    DropdownButtonFormField<String>(
                      initialValue: _hautatutakoTxanda,
                      decoration: const InputDecoration(
                        labelText: 'Txanda',
                        prefixIcon: Icon(Icons.access_time, size: 20, color: AppTheme.chiantiWine),
                        isDense: true,
                      ),
                      items: _txandak.entries.map((entry) {
                        return DropdownMenuItem<String>(
                          value: entry.key,
                          child: Text(entry.value, style: const TextStyle(fontSize: 13)),
                        );
                      }).toList(),
                      onChanged: (val) {
                        if (val != null) {
                          setState(() {
                            _hautatutakoTxanda = val;
                            _hautatutakoMahaiaId = null;
                            _hautatutakoMahaiZenbakia = null;
                          });
                          _kargatuMahaiak();
                        }
                      },
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 16),

              // 2. Pertsona kopurua eta Mahaia
              const Text(
                '2. Mahaikideak eta Mahaia',
                style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.bold,
                  color: AppTheme.darkCharcoal,
                ),
              ),
              const SizedBox(height: 8),
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: Colors.grey.shade200),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        const Text('Pertsona Kopurua:', style: TextStyle(fontWeight: FontWeight.w600, fontSize: 13)),
                        Row(
                          children: [
                            IconButton(
                              icon: const Icon(Icons.remove_circle_outline),
                              color: AppTheme.chiantiWine,
                              onPressed: _pertsonaKopurua > 1
                                  ? () {
                                      setState(() => _pertsonaKopurua--);
                                      _kargatuMahaiak();
                                    }
                                  : null,
                            ),
                            Text(
                              '$_pertsonaKopurua',
                              style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                            ),
                            IconButton(
                              icon: const Icon(Icons.add_circle_outline),
                              color: AppTheme.chiantiWine,
                              onPressed: _pertsonaKopurua < 16
                                  ? () {
                                      setState(() => _pertsonaKopurua++);
                                      _kargatuMahaiak();
                                    }
                                  : null,
                            ),
                          ],
                        ),
                      ],
                    ),
                    const Divider(height: 16),

                    // Kokapena / Eremua aukeraketa
                    const Text('Jatetxeko Eremua / Mota:', style: TextStyle(fontWeight: FontWeight.w600, fontSize: 13)),
                    const SizedBox(height: 6),
                    SingleChildScrollView(
                      scrollDirection: Axis.horizontal,
                      child: Row(
                        children: _kokapenak.map((kok) {
                          final hautatuta = _hautatutakoKokapena == kok;
                          return Padding(
                            padding: const EdgeInsets.only(right: 6),
                            child: ChoiceChip(
                              label: Text(kok),
                              selected: hautatuta,
                              selectedColor: AppTheme.chiantiWine,
                              backgroundColor: Colors.white,
                              labelStyle: TextStyle(
                                color: hautatuta ? Colors.white : AppTheme.darkCharcoal,
                                fontWeight: hautatuta ? FontWeight.bold : FontWeight.normal,
                                fontSize: 12,
                              ),
                              side: BorderSide(
                                color: hautatuta ? AppTheme.chiantiWine : Colors.grey.shade300,
                              ),
                              onSelected: (val) {
                                if (val) {
                                  setState(() {
                                    _hautatutakoKokapena = kok;
                                    _hautatutakoMahaiaId = null;
                                    _hautatutakoMahaiZenbakia = null;
                                  });
                                  _kargatuMahaiak();
                                }
                              },
                            ),
                          );
                        }).toList(),
                      ),
                    ),
                    const SizedBox(height: 12),

                    _kargatzenMahaiak
                        ? const Center(
                            child: Padding(
                              padding: EdgeInsets.symmetric(vertical: 8),
                              child: LinearProgressIndicator(color: AppTheme.chiantiWine),
                            ),
                          )
                        : DropdownButtonFormField<int?>(
                            isExpanded: true,
                            initialValue: _hautatutakoMahaiaId,
                            decoration: InputDecoration(
                              labelText: 'Mahaia hautatu (${_mahaiak.length} libre data eta txanda honetan)',
                              prefixIcon: const Icon(Icons.table_bar, size: 20, color: AppTheme.chiantiWine),
                              isDense: true,
                            ),
                            items: [
                              DropdownMenuItem<int?>(
                                value: null,
                                child: Text(
                                  _hautatutakoKokapena == 'Guztiak'
                                      ? 'Edozein mahai libre (Jatetxeak esleitua)'
                                      : 'Edozein mahai libre ($_hautatutakoKokapena)',
                                  style: const TextStyle(fontSize: 13, fontStyle: FontStyle.italic),
                                  overflow: TextOverflow.ellipsis,
                                ),
                              ),
                              ..._mahaiak.map((m) {
                                final id = m['id'] as int? ?? 0;
                                final zenb = m['zenbakia'] as int? ?? id;
                                final kokapena = m['kokapena'] as String? ?? 'Jantokia';
                                final edukiera = m['edukiera'] as int? ?? 4;
                                return DropdownMenuItem<int?>(
                                  value: id,
                                  child: Text(
                                    'Mahaia #$zenb · $kokapena ($edukiera pax)',
                                    style: const TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
                                    overflow: TextOverflow.ellipsis,
                                  ),
                                );
                              }),
                            ],
                            onChanged: (val) {
                              setState(() {
                                _hautatutakoMahaiaId = val;
                                if (val != null) {
                                  final match = _mahaiak.firstWhere(
                                    (m) => m['id'] == val,
                                    orElse: () => {},
                                  );
                                  _hautatutakoMahaiZenbakia = match['zenbakia'] as int?;
                                } else {
                                  _hautatutakoMahaiZenbakia = null;
                                }
                              });
                            },
                          ),
                  ],
                ),
              ),

              const SizedBox(height: 16),

              // 3. Harremanetarako Datuak
              const Text(
                '3. Harremanetarako Datuak',
                style: TextStyle(
                  fontSize: 15,
                  fontWeight: FontWeight.bold,
                  color: AppTheme.darkCharcoal,
                ),
              ),
              const SizedBox(height: 8),
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: Colors.grey.shade200),
                ),
                child: Column(
                  children: [
                    Row(
                      children: [
                        Expanded(
                          child: TextFormField(
                            controller: _izenaController,
                            decoration: const InputDecoration(labelText: 'Izena *', isDense: true),
                            validator: (v) => (v == null || v.trim().isEmpty) ? 'Izena beharrezkoa da' : null,
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: TextFormField(
                            controller: _abizenaController,
                            decoration: const InputDecoration(labelText: 'Abizena *', isDense: true),
                            validator: (v) => (v == null || v.trim().isEmpty) ? 'Abizena beharrezkoa da' : null,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _emailController,
                      keyboardType: TextInputType.emailAddress,
                      decoration: const InputDecoration(labelText: 'Emaila (baieztapenerako) *', isDense: true),
                      validator: (v) => (v == null || !v.contains('@')) ? 'Email baliodun bat sartu' : null,
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _telefonoaController,
                      keyboardType: TextInputType.phone,
                      decoration: const InputDecoration(labelText: 'Telefono zenbakia *', isDense: true),
                      validator: (v) => (v == null || v.trim().isEmpty) ? 'Telefonoa beharrezkoa da' : null,
                    ),
                    const SizedBox(height: 12),
                    TextFormField(
                      controller: _oharrakController,
                      maxLines: 2,
                      decoration: const InputDecoration(
                        labelText: 'Oharrak (alergiak, haur aulkia, ospakizuna...)',
                        isDense: true,
                      ),
                    ),
                  ],
                ),
              ),

              const SizedBox(height: 24),

              // Erreserba Baieztatzeko botoia
              SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  onPressed: _bidaltzen ? null : _bidaliErreserba,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: AppTheme.chiantiWine,
                    padding: const EdgeInsets.symmetric(vertical: 16),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                    elevation: 2,
                  ),
                  child: _bidaltzen
                      ? const SizedBox(
                          height: 20,
                          width: 20,
                          child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2),
                        )
                      : const Text(
                          'Erreserba Baieztatu',
                          style: TextStyle(
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
      ),
    );
  }
}
