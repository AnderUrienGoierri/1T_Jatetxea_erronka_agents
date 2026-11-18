class Platera {
  final int id;
  final String izena;
  final String? deskribapena;
  final String kategoria;
  final double prezioa;
  final String? alergenoak;
  final int prestatzeDenboraMin;
  final bool aktibo;
  int stockAhalmena;

  Platera({
    required this.id,
    required this.izena,
    this.deskribapena,
    required this.kategoria,
    required this.prezioa,
    this.alergenoak,
    this.prestatzeDenboraMin = 15,
    this.aktibo = true,
    this.stockAhalmena = 999,
  });

  factory Platera.fromJson(Map<String, dynamic> json) {
    return Platera(
      id: json['id'] as int? ?? 0,
      izena: json['izena'] as String? ?? '',
      deskribapena: json['deskribapena'] as String?,
      kategoria: json['kategoria'] as String? ?? 'Guztiak',
      prezioa: (json['prezioa'] as num?)?.toDouble() ?? 0.0,
      alergenoak: json['alergenoak'] as String?,
      prestatzeDenboraMin: json['prestatzeDenboraMin'] as int? ?? 15,
      aktibo: json['aktibo'] as bool? ?? true,
      stockAhalmena: json['stockAhalmena'] as int? ?? 999,
    );
  }
}
