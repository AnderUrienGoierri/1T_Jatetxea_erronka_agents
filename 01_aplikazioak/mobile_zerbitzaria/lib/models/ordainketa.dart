class OrdainketaErantzuna {
  final int id;
  final int eskaeraId;
  final String tiketZenbakia;
  final String mota;
  final double zenbatekoa;
  final double? jasotakoa;
  final double? itzulitakoa;
  final String? txartelMaskaratua;
  final String? txartelMarka;
  final String operazioKodea;
  final String? tokenZifratua;
  final String egoera;
  final String data;
  final String tiketTestua;
  final double azpitotala;
  final double bez;

  OrdainketaErantzuna({
    required this.id,
    required this.eskaeraId,
    required this.tiketZenbakia,
    required this.mota,
    required this.zenbatekoa,
    this.jasotakoa,
    this.itzulitakoa,
    this.txartelMaskaratua,
    this.txartelMarka,
    required this.operazioKodea,
    this.tokenZifratua,
    required this.egoera,
    required this.data,
    required this.tiketTestua,
    required this.azpitotala,
    required this.bez,
  });

  factory OrdainketaErantzuna.fromJson(Map<String, dynamic> json) {
    return OrdainketaErantzuna(
      id: json['id'] as int? ?? 0,
      eskaeraId: json['eskaeraId'] as int? ?? 0,
      tiketZenbakia: json['tiketZenbakia'] as String? ?? '',
      mota: json['mota'] as String? ?? 'Eskudirua',
      zenbatekoa: (json['zenbatekoa'] as num?)?.toDouble() ?? 0.0,
      jasotakoa: (json['jasotakoa'] as num?)?.toDouble(),
      itzulitakoa: (json['itzulitakoa'] as num?)?.toDouble(),
      txartelMaskaratua: json['txartelMaskaratua'] as String?,
      txartelMarka: json['txartelMarka'] as String?,
      operazioKodea: json['operazioKodea'] as String? ?? '',
      tokenZifratua: json['tokenZifratua'] as String?,
      egoera: json['egoera'] as String? ?? 'Arrakastatsua',
      data: json['data'] as String? ?? '',
      tiketTestua: json['tiketTestua'] as String? ?? '',
      azpitotala: (json['azpitotala'] as num?)?.toDouble() ?? 0.0,
      bez: (json['bez'] as num?)?.toDouble() ?? 0.0,
    );
  }
}
