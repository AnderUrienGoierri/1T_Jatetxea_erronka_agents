class Iritzia {
  final int id;
  final int bezeroaId;
  final String bezeroIzena;
  final String bezeroEmaila;
  final int balorazioa; // 1-5 izarrak
  final String? testua;
  final String sortzeData;

  Iritzia({
    required this.id,
    required this.bezeroaId,
    required this.bezeroIzena,
    required this.bezeroEmaila,
    required this.balorazioa,
    this.testua,
    required this.sortzeData,
  });

  factory Iritzia.fromJson(Map<String, dynamic> json) {
    return Iritzia(
      id: json['id'] as int? ?? 0,
      bezeroaId: json['bezeroaId'] as int? ?? 0,
      bezeroIzena: json['bezeroIzena'] as String? ?? 'Bezero Anonimoa',
      bezeroEmaila: json['bezeroEmaila'] as String? ?? '',
      balorazioa: json['balorazioa'] as int? ?? 5,
      testua: json['testua'] as String?,
      sortzeData: json['sortzeData'] as String? ?? '',
    );
  }
}

class IritziEstatistikak {
  final int guztiraIritziak;
  final double batazBestekoBalorazioa;
  final int bostIzarrak;
  final int lauIzarrak;
  final int hiruIzarrak;
  final int biIzarrak;
  final int izarBat;
  final int zeroIzar;

  IritziEstatistikak({
    required this.guztiraIritziak,
    required this.batazBestekoBalorazioa,
    required this.bostIzarrak,
    required this.lauIzarrak,
    required this.hiruIzarrak,
    required this.biIzarrak,
    required this.izarBat,
    required this.zeroIzar,
  });

  factory IritziEstatistikak.fromJson(Map<String, dynamic> json) {
    return IritziEstatistikak(
      guztiraIritziak: (json['guztiraIritziak'] as num?)?.toInt() ?? (json['guztira_iritziak'] as num?)?.toInt() ?? 0,
      batazBestekoBalorazioa: (json['batazBestekoBalorazioa'] as num?)?.toDouble() ?? (json['bataz_besteko_balorazioa'] as num?)?.toDouble() ?? 0.0,
      bostIzarrak: (json['bostIzarrak'] as num?)?.toInt() ?? (json['bost_izarrak'] as num?)?.toInt() ?? 0,
      lauIzarrak: (json['lauIzarrak'] as num?)?.toInt() ?? (json['lau_izarrak'] as num?)?.toInt() ?? 0,
      hiruIzarrak: (json['hiruIzarrak'] as num?)?.toInt() ?? (json['hiru_izarrak'] as num?)?.toInt() ?? 0,
      biIzarrak: (json['biIzarrak'] as num?)?.toInt() ?? (json['bi_izarrak'] as num?)?.toInt() ?? 0,
      izarBat: (json['izarBat'] as num?)?.toInt() ?? (json['izar_bat'] as num?)?.toInt() ?? 0,
      zeroIzar: (json['zeroIzar'] as num?)?.toInt() ?? (json['zero_izar'] as num?)?.toInt() ?? 0,
    );
  }
}
