class Mahaia {
  final int id;
  final int zenbakia;
  final int edukiera;
  final String kokapena;
  final String egoera; // Libre, Okupatuta, Erreserbatuta, Garbitzen
  final int? mahaikideak;
  final String? iraupena;
  final int? aktiboEskaeraId;

  Mahaia({
    required this.id,
    required this.zenbakia,
    required this.edukiera,
    required this.kokapena,
    required this.egoera,
    this.mahaikideak,
    this.iraupena,
    this.aktiboEskaeraId,
  });

  factory Mahaia.fromJson(Map<String, dynamic> json) {
    return Mahaia(
      id: json['id'] as int? ?? 0,
      zenbakia: json['zenbakia'] as int? ?? 0,
      edukiera: json['edukiera'] as int? ?? 4,
      kokapena: json['kokapena'] as String? ?? 'Jangela Nagusia',
      egoera: json['egoera'] as String? ?? 'Libre',
      mahaikideak: json['mahaikideak'] as int?,
      iraupena: json['iraupena'] as String?,
      aktiboEskaeraId: json['aktiboEskaeraId'] as int?,
    );
  }

  Map<String, dynamic> toJson() => {
    'id': id,
    'zenbakia': zenbakia,
    'edukiera': edukiera,
    'kokapena': kokapena,
    'egoera': egoera,
  };
}
