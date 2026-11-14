class Langilea {
  final int id;
  final String erabiltzailea;
  final String izena;
  final String rola;
  final bool aktibo;

  Langilea({
    required this.id,
    required this.erabiltzailea,
    required this.izena,
    required this.rola,
    required this.aktibo,
  });

  factory Langilea.fromJson(Map<String, dynamic> json) {
    return Langilea(
      id: json['id'] as int? ?? 0,
      erabiltzailea: json['erabiltzailea'] as String? ?? '',
      izena: json['izena'] as String? ?? '',
      rola: json['rola'] as String? ?? '',
      aktibo: json['aktibo'] as bool? ?? true,
    );
  }

  Map<String, dynamic> toJson() => {
    'id': id,
    'erabiltzailea': erabiltzailea,
    'izena': izena,
    'rola': rola,
    'aktibo': aktibo,
  };
}
