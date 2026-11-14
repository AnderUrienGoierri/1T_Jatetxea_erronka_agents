enum OrdainketaMota {
  txartela,
  bizum,
  googlePay,
  paypal,
}

class OrdainketaXehetasunak {
  final OrdainketaMota mota;
  final double guztira;
  final String eskaeraMota; // 'Mahaitik' edo 'Eramateko'
  final int? mahaiZenbakia;
  final String bezeroIzena;
  final String bezeroEmaila;
  final String bezeroTelefonoa;
  final String? txartelZenbakiaMaskaratua;
  final String? titularra;
  final String? bizumTelefonoa;

  OrdainketaXehetasunak({
    required this.mota,
    required this.guztira,
    required this.eskaeraMota,
    this.mahaiZenbakia,
    required this.bezeroIzena,
    required this.bezeroEmaila,
    required this.bezeroTelefonoa,
    this.txartelZenbakiaMaskaratua,
    this.titularra,
    this.bizumTelefonoa,
  });
}

class OrdainketaEmaitza {
  final bool arrakasta;
  final String transakzioId;
  final String tokenZifratua;
  final String mezua;
  final String data;
  final OrdainketaMota mota;
  final double zenbatekoa;
  final int eskaeraId;

  OrdainketaEmaitza({
    required this.arrakasta,
    required this.transakzioId,
    required this.tokenZifratua,
    required this.mezua,
    required this.data,
    required this.mota,
    required this.zenbatekoa,
    required this.eskaeraId,
  });
}
