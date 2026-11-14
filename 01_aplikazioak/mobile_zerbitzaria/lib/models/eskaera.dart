import 'platera.dart';

class Eskaera {
  final int id;
  final int mahaiaId;
  final int mahaiZenbakia;
  final int langileaId;
  final String langileIzena;
  final int pertsonaKopurua;
  final String egoera; // Irekia, Sukaldean, Zerbitzatua, Kobratua
  final double guztira;
  final String sortzeData;
  final List<EskaeraLerroa> lerroak;

  Eskaera({
    required this.id,
    required this.mahaiaId,
    required this.mahaiZenbakia,
    required this.langileaId,
    required this.langileIzena,
    required this.pertsonaKopurua,
    required this.egoera,
    required this.guztira,
    required this.sortzeData,
    this.lerroak = const [],
  });

  factory Eskaera.fromJson(Map<String, dynamic> json) {
    var rawLerroak = json['lerroak'] as List<dynamic>? ?? [];
    List<EskaeraLerroa> parsedLerroak = rawLerroak
        .map((item) => EskaeraLerroa.fromJson(item as Map<String, dynamic>))
        .toList();

    return Eskaera(
      id: json['id'] as int? ?? 0,
      mahaiaId: json['mahaiaId'] as int? ?? 0,
      mahaiZenbakia: json['mahaiZenbakia'] as int? ?? 0,
      langileaId: json['langileaId'] as int? ?? 0,
      langileIzena: json['langileIzena'] as String? ?? '',
      pertsonaKopurua: json['pertsonaKopurua'] as int? ?? 2,
      egoera: json['egoera'] as String? ?? 'Irekia',
      guztira: (json['guztira'] as num?)?.toDouble() ?? 0.0,
      sortzeData: json['sortzeData'] as String? ?? '',
      lerroak: parsedLerroak,
    );
  }
}

class EskaeraLerroa {
  final int id;
  final int plateraId;
  final String platerIzena;
  final String kategoria;
  final int kantitatea;
  final double unitatePrezioa;
  final String egoera;
  final String? oharrak;

  EskaeraLerroa({
    required this.id,
    required this.plateraId,
    required this.platerIzena,
    required this.kategoria,
    required this.kantitatea,
    required this.unitatePrezioa,
    required this.egoera,
    this.oharrak,
  });

  double get azpitotala => unitatePrezioa * kantitatea;

  factory EskaeraLerroa.fromJson(Map<String, dynamic> json) {
    return EskaeraLerroa(
      id: json['id'] as int? ?? 0,
      plateraId: json['plateraId'] as int? ?? 0,
      platerIzena: json['platerIzena'] as String? ?? '',
      kategoria: json['kategoria'] as String? ?? '',
      kantitatea: (json['kantitatea'] ?? json['kopurua'] ?? 1) as int,
      unitatePrezioa: (json['unitatePrezioa'] as num?)?.toDouble() ?? 0.0,
      egoera: json['egoera'] as String? ?? 'Irekia',
      oharrak: json['oharrak'] as String?,
    );
  }
}

class CartItem {
  final Platera platera;
  int kopurua;
  String oharrak;

  CartItem({
    required this.platera,
    this.kopurua = 1,
    this.oharrak = '',
  });

  double get azpitotala => platera.prezioa * kopurua;
}
