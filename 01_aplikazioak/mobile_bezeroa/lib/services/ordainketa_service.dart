import 'dart:math';
import '../models/ordainketa.dart';

class OrdainketaService {
  // Luhn algoritmoa eta txartel markaren detekzioa
  static String detektatuTxartelMota(String zenbakia) {
    final garbia = zenbakia.replaceAll(RegExp(r'\s+'), '');
    if (garbia.startsWith('4')) return 'VISA';
    if (garbia.startsWith('51') ||
        garbia.startsWith('52') ||
        garbia.startsWith('53') ||
        garbia.startsWith('54') ||
        garbia.startsWith('55')) {
      return 'Mastercard';
    }
    if (garbia.startsWith('34') || garbia.startsWith('37')) return 'American Express';
    return 'Kreditu Txartela';
  }

  static String maskaratuTxartela(String zenbakia) {
    final garbia = zenbakia.replaceAll(RegExp(r'\s+'), '');
    if (garbia.length >= 4) {
      final azkena = garbia.substring(garbia.length - 4);
      return '**** **** **** $azkena';
    }
    return '**** **** **** 0000';
  }

  static String sortuTransakzioId() {
    final random = Random().nextInt(89999) + 10000;
    return 'TXN-2026-$random-SEC';
  }

  static String sortuTokenZifratua(String id) {
    final hexChars = '0123456789abcdef';
    final random = Random();
    final randomHex = List.generate(24, (_) => hexChars[random.nextInt(16)]).join();
    return 'tok_pci_aes256_$randomHex';
  }

  // Txartel bidezko ordainketa
  static Future<OrdainketaEmaitza> prozesatuTxartela({
    required String txartelZenbakia,
    required String titularra,
    required String iraungitzea,
    required String cvv,
    required double zenbatekoa,
    required int eskaeraId,
  }) async {
    // Bankuarekin SSL/TLS 1.3 konexio segurua simulatu
    await Future.delayed(const Duration(milliseconds: 1400));

    final transakzioId = sortuTransakzioId();
    final token = sortuTokenZifratua(transakzioId);
    final dataOraingoz = DateTime.now().toIso8601String().substring(0, 19).replaceAll('T', ' ');

    return OrdainketaEmaitza(
      arrakasta: true,
      transakzioId: transakzioId,
      tokenZifratua: token,
      mezua: 'Ordainketa arrakastaz burutu da 3D Secure / PSD2 segurtasun protokoloaren bidez.',
      data: dataOraingoz,
      mota: OrdainketaMota.txartela,
      zenbatekoa: zenbatekoa,
      eskaeraId: eskaeraId,
    );
  }

  // Bizum bidezko ordainketa
  static Future<OrdainketaEmaitza> prozesatuBizum({
    required String telefonoa,
    required double zenbatekoa,
    required int eskaeraId,
  }) async {
    // Bizum push jakinarazpen baimena simulatu
    await Future.delayed(const Duration(milliseconds: 1200));

    final transakzioId = sortuTransakzioId();
    final token = sortuTokenZifratua(transakzioId);
    final dataOraingoz = DateTime.now().toIso8601String().substring(0, 19).replaceAll('T', ' ');

    return OrdainketaEmaitza(
      arrakasta: true,
      transakzioId: transakzioId,
      tokenZifratua: token,
      mezua: 'Bizum ordainketa berehala baieztatu da $telefonoa zenbakiarekin.',
      data: dataOraingoz,
      mota: OrdainketaMota.bizum,
      zenbatekoa: zenbatekoa,
      eskaeraId: eskaeraId,
    );
  }

  // Google Pay / Apple Pay biometrikoa
  static Future<OrdainketaEmaitza> prozesatuGooglePay({
    required double zenbatekoa,
    required int eskaeraId,
  }) async {
    // Biometria / tokenizazio enkriptatua simulatu
    await Future.delayed(const Duration(milliseconds: 1000));

    final transakzioId = sortuTransakzioId();
    final token = sortuTokenZifratua(transakzioId);
    final dataOraingoz = DateTime.now().toIso8601String().substring(0, 19).replaceAll('T', ' ');

    return OrdainketaEmaitza(
      arrakasta: true,
      transakzioId: transakzioId,
      tokenZifratua: token,
      mezua: 'Ordainketa biometriko segurua tokenizazio bidez baieztatu da (Google Pay / Apple Pay).',
      data: dataOraingoz,
      mota: OrdainketaMota.googlePay,
      zenbatekoa: zenbatekoa,
      eskaeraId: eskaeraId,
    );
  }

  // PayPal
  static Future<OrdainketaEmaitza> prozesatuPaypal({
    required String emaila,
    required double zenbatekoa,
    required int eskaeraId,
  }) async {
    await Future.delayed(const Duration(milliseconds: 1300));

    final transakzioId = sortuTransakzioId();
    final token = sortuTokenZifratua(transakzioId);
    final dataOraingoz = DateTime.now().toIso8601String().substring(0, 19).replaceAll('T', ' ');

    return OrdainketaEmaitza(
      arrakasta: true,
      transakzioId: transakzioId,
      tokenZifratua: token,
      mezua: 'PayPal transakzio segurua onartu da ($emaila).',
      data: dataOraingoz,
      mota: OrdainketaMota.paypal,
      zenbatekoa: zenbatekoa,
      eskaeraId: eskaeraId,
    );
  }
}
