class Erreserba {
  final int id;
  final int bezeroaId;
  final String bezeroIzena;
  final String bezeroAbizena;
  final String bezeroEmaila;
  final String bezeroTelefonoa;
  final int mahaiaId;
  final int mahaiZenbakia;
  final String kokapena;
  final String data;
  final String txanda; // Bazkaria_1, Bazkaria_2, Afaria_1, Afaria_2
  final int pertsonaKopurua;
  final String egoera;
  final String? oharrak;

  Erreserba({
    required this.id,
    required this.bezeroaId,
    required this.bezeroIzena,
    required this.bezeroAbizena,
    required this.bezeroEmaila,
    required this.bezeroTelefonoa,
    required this.mahaiaId,
    required this.mahaiZenbakia,
    required this.kokapena,
    required this.data,
    required this.txanda,
    required this.pertsonaKopurua,
    required this.egoera,
    this.oharrak,
  });

  factory Erreserba.fromJson(Map<String, dynamic> json) {
    return Erreserba(
      id: json['id'] as int? ?? 0,
      bezeroaId: json['bezeroaId'] as int? ?? 0,
      bezeroIzena: json['bezeroIzena'] as String? ?? '',
      bezeroAbizena: json['bezeroAbizena'] as String? ?? '',
      bezeroEmaila: json['bezeroEmaila'] as String? ?? '',
      bezeroTelefonoa: json['bezeroTelefonoa'] as String? ?? '',
      mahaiaId: json['mahaiaId'] as int? ?? 0,
      mahaiZenbakia: json['mahaiZenbakia'] as int? ?? 0,
      kokapena: json['kokapena'] as String? ?? '',
      data: json['data'] as String? ?? '',
      txanda: json['txanda'] as String? ?? 'Bazkaria_1',
      pertsonaKopurua: json['pertsonaKopurua'] as int? ?? 2,
      egoera: json['egoera'] as String? ?? 'Baieztatuta',
      oharrak: json['oharrak'] as String?,
    );
  }
}
