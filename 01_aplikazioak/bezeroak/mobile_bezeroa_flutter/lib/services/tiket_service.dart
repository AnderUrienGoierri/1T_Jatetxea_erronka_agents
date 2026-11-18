import 'dart:typed_data';
import 'package:pdf/pdf.dart';
import 'package:pdf/widgets.dart' as pw;
import 'package:printing/printing.dart';
import '../models/cart_item.dart';
import '../models/ordainketa.dart';

class TiketService {
  static String sortuTiketZenbakia(int eskaeraId, DateTime data) {
    final y = data.year.toString().padLeft(4, '0');
    final m = data.month.toString().padLeft(2, '0');
    final d = data.day.toString().padLeft(2, '0');
    final idStr = eskaeraId.toString().padLeft(4, '0');
    return 'TK-$y$m$d-$idStr';
  }

  static String sortuTiketTestua({
    required int eskaeraId,
    required DateTime data,
    required int mahaiZenbakia,
    required String eskaeraMota,
    required String bezeroIzena,
    required List<CartItem> saskia,
    required double guztira,
    required OrdainketaEmaitza emaitza,
  }) {
    final tiketZenbakia = sortuTiketZenbakia(eskaeraId, data);
    final subtotal = guztira / 1.10;
    final bez = guztira - subtotal;
    const int W = 46;
    final marraBikoitza = '=' * W;
    final marraBakarra = '-' * W;

    String zentroan(String t) {
      if (t.length >= W) return t;
      final ezker = (W - t.length) ~/ 2;
      return ' ' * ezker + t;
    }

    String banatuta(String ezker, String eskuin) {
      final hutsunea = W - ezker.length - eskuin.length;
      return ezker + ' ' * (hutsunea > 0 ? hutsunea : 1) + eskuin;
    }

    final sb = StringBuffer();
    sb.writeln(marraBikoitza);
    sb.writeln(zentroan('T1 JATETXEA'));
    sb.writeln(zentroan('Italo-Euskal Gastronomia'));
    sb.writeln(zentroan('IFZ / CIF: B-20123456'));
    sb.writeln(zentroan('Nafarroa Hiribidea 12, 20240 Ordizia'));
    sb.writeln(zentroan('Tel: +34 943 12 34 56 · www.t1jatetxea.eus'));
    sb.writeln(marraBikoitza);

    sb.writeln(banatuta('TIKET FISKALA:', tiketZenbakia));
    final dataStr =
        '${data.year}/${data.month.toString().padLeft(2, '0')}/${data.day.toString().padLeft(2, '0')} ${data.hour.toString().padLeft(2, '0')}:${data.minute.toString().padLeft(2, '0')}:${data.second.toString().padLeft(2, '0')}';
    sb.writeln(banatuta('DATA:', dataStr));
    if (eskaeraMota == 'Mahaitik') {
      sb.writeln(banatuta('MAHAIA: $mahaiZenbakia', 'ESKAERA: #$eskaeraId'));
    } else {
      sb.writeln(banatuta('MOTA: Eramateko', 'ESKAERA: #$eskaeraId'));
    }
    sb.writeln(banatuta('BEZEROA:', bezeroIzena));
    sb.writeln(marraBakarra);

    sb.writeln('KOP  DESKRIBAPENA        P.UNT     GUZTIRA');
    sb.writeln(marraBakarra);

    for (final item in saskia) {
      final kop = '${item.kopurua}x';
      String izena = item.platera.izena;
      if (izena.length > 20) izena = '${izena.substring(0, 19)}.';
      final pUnt = item.platera.prezioa.toStringAsFixed(2);
      final lerroGuzt = item.guztira.toStringAsFixed(2);
      final lerroa =
          '${kop.padRight(4)} ${izena.padRight(20)} ${pUnt.padLeft(6)} ${lerroGuzt.padLeft(8)} EUR';
      sb.writeln(lerroa);
    }

    sb.writeln(marraBakarra);
    sb.writeln(banatuta('AZPITOTALA (Zergarik gabe):', '${subtotal.toStringAsFixed(2)} EUR'));
    sb.writeln(banatuta('BEZ (%10 barne):', '${bez.toStringAsFixed(2)} EUR'));
    sb.writeln(marraBikoitza);
    sb.writeln(banatuta('TOTALA ORDAINTZEKO:', '${guztira.toStringAsFixed(2)} EUR'));
    sb.writeln(marraBikoitza);

    sb.writeln('ORDAINKETA DATUAK (SEGREDU FISKALA):');
    sb.writeln(banatuta('MODUA:', emaitza.mota.name.toUpperCase()));
    if (emaitza.mota == OrdainketaMota.txartela) {
      sb.writeln(banatuta('TXARTELA:', '**** **** **** 4821 (VISA)'));
      sb.writeln(banatuta('PROTOKOLOA:', 'EMV / 3D Secure v2'));
    } else if (emaitza.mota == OrdainketaMota.bizum) {
      sb.writeln(banatuta('BIZUM:', 'Berehalako Push Baimendua'));
    } else {
      sb.writeln(banatuta('WALLET:', 'NFC Biometria Baimendua'));
    }
    sb.writeln(banatuta('ERAGIKETA KODEA:', emaitza.transakzioId));
    final tokenLaburra = emaitza.tokenZifratua.length > 22
        ? '${emaitza.tokenZifratua.substring(0, 20)}..'
        : emaitza.tokenZifratua;
    sb.writeln(banatuta('TOKEN ZIFRATUA:', tokenLaburra));
    sb.writeln(banatuta('EGOERA:', 'BAIMENDUA ETA KOBRATUA'));

    sb.writeln(marraBikoitza);
    sb.writeln(zentroan('Eskerrik asko gurekin bazkaltzeagatik!'));
    sb.writeln(zentroan('Grazie mille per la vostra visita!'));
    sb.writeln(zentroan('Faktura osoa eskatzeko: info@t1jatetxea.eus'));
    sb.writeln(marraBikoitza);

    return sb.toString();
  }

  static Future<Uint8List> sortuTiketPdfBytes({
    required int eskaeraId,
    required DateTime data,
    required int mahaiZenbakia,
    required String eskaeraMota,
    required String bezeroIzena,
    required List<CartItem> saskia,
    required double guztira,
    required OrdainketaEmaitza emaitza,
  }) async {
    final pdf = pw.Document();
    final tiketZenbakia = sortuTiketZenbakia(eskaeraId, data);
    final subtotal = guztira / 1.10;
    final bez = guztira - subtotal;
    final courier = pw.Font.courier();
    final courierBold = pw.Font.courierBold();

    final dataStr =
        '${data.year}/${data.month.toString().padLeft(2, '0')}/${data.day.toString().padLeft(2, '0')} ${data.hour.toString().padLeft(2, '0')}:${data.minute.toString().padLeft(2, '0')}:${data.second.toString().padLeft(2, '0')}';

    pdf.addPage(
      pw.Page(
        pageFormat: PdfPageFormat.roll80,
        margin: const pw.EdgeInsets.symmetric(horizontal: 10, vertical: 12),
        build: (pw.Context context) {
          return pw.Column(
            crossAxisAlignment: pw.CrossAxisAlignment.stretch,
            children: [
              // Burua
              pw.Center(child: pw.Text('========================================', style: pw.TextStyle(font: courier, fontSize: 8))),
              pw.Center(child: pw.Text('T1 JATETXEA', style: pw.TextStyle(font: courierBold, fontSize: 13))),
              pw.Center(child: pw.Text('Italo-Euskal Gastronomia', style: pw.TextStyle(font: courier, fontSize: 8.5))),
              pw.Center(child: pw.Text('IFZ / CIF: B-20123456', style: pw.TextStyle(font: courier, fontSize: 8))),
              pw.Center(child: pw.Text('Nafarroa Hiribidea 12, 20240 Ordizia', style: pw.TextStyle(font: courier, fontSize: 8))),
              pw.Center(child: pw.Text('Tel: +34 943 12 34 56 · www.t1jatetxea.eus', style: pw.TextStyle(font: courier, fontSize: 7.5))),
              pw.Center(child: pw.Text('========================================', style: pw.TextStyle(font: courier, fontSize: 8))),
              pw.SizedBox(height: 4),

              // Datuak
              _pdfLerroa('TIKET FISKALA:', tiketZenbakia, courier, courierBold),
              _pdfLerroa('DATA:', dataStr, courier, courierBold),
              if (eskaeraMota == 'Mahaitik')
                _pdfLerroa('MAHAIA: #$mahaiZenbakia', 'ESKAERA: #$eskaeraId', courier, courierBold)
              else
                _pdfLerroa('MOTA: Eramateko', 'ESKAERA: #$eskaeraId', courier, courierBold),
              _pdfLerroa('BEZEROA:', bezeroIzena, courier, courierBold),
              pw.SizedBox(height: 4),

              // Lerro banatzailea
              pw.Text('----------------------------------------', style: pw.TextStyle(font: courier, fontSize: 8)),
              pw.Row(
                mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
                children: [
                  pw.Text('KOP DESKRIBAPENA', style: pw.TextStyle(font: courierBold, fontSize: 7.5)),
                  pw.Text('GUZTIRA', style: pw.TextStyle(font: courierBold, fontSize: 7.5)),
                ],
              ),
              pw.Text('----------------------------------------', style: pw.TextStyle(font: courier, fontSize: 8)),

              // Produktuak
              ...saskia.map((item) {
                return pw.Padding(
                  padding: const pw.EdgeInsets.symmetric(vertical: 1.5),
                  child: pw.Row(
                    mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
                    children: [
                      pw.Expanded(
                        child: pw.Text(
                          '${item.kopurua}x ${item.platera.izena}',
                          style: pw.TextStyle(font: courier, fontSize: 8),
                          maxLines: 1,
                        ),
                      ),
                      pw.Text(
                        '${item.guztira.toStringAsFixed(2)} EUR',
                        style: pw.TextStyle(font: courierBold, fontSize: 8),
                      ),
                    ],
                  ),
                );
              }),

              pw.Text('----------------------------------------', style: pw.TextStyle(font: courier, fontSize: 8)),
              _pdfLerroa('AZPITOTALA (Zergarik gabe):', '${subtotal.toStringAsFixed(2)} EUR', courier, courier),
              _pdfLerroa('BEZ (%10 barne):', '${bez.toStringAsFixed(2)} EUR', courier, courier),
              pw.Text('========================================', style: pw.TextStyle(font: courier, fontSize: 8)),
              pw.Row(
                mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
                children: [
                  pw.Text('TOTALA ORDAINTZEKO:', style: pw.TextStyle(font: courierBold, fontSize: 9.5)),
                  pw.Text('${guztira.toStringAsFixed(2)} EUR', style: pw.TextStyle(font: courierBold, fontSize: 11)),
                ],
              ),
              pw.Text('========================================', style: pw.TextStyle(font: courier, fontSize: 8)),
              pw.SizedBox(height: 4),

              // Segurtasun datuak
              pw.Text('ORDAINKETA DATUAK (SEGREDU FISKALA):', style: pw.TextStyle(font: courierBold, fontSize: 7.5)),
              _pdfLerroa('MODUA:', emaitza.mota.name.toUpperCase(), courier, courierBold),
              _pdfLerroa('ERAGIKETA KODEA:', emaitza.transakzioId, courier, courier),
              _pdfLerroa(
                'TOKEN ZIFRATUA:',
                emaitza.tokenZifratua.length > 20
                    ? '${emaitza.tokenZifratua.substring(0, 18)}..'
                    : emaitza.tokenZifratua,
                courier,
                courier,
              ),
              _pdfLerroa('EGOERA:', 'BAIMENDUA ETA KOBRATUA', courier, courierBold),

              pw.SizedBox(height: 6),
              pw.Center(child: pw.Text('========================================', style: pw.TextStyle(font: courier, fontSize: 8))),
              pw.Center(child: pw.Text('Eskerrik asko gurekin bazkaltzeagatik!', style: pw.TextStyle(font: courier, fontSize: 8))),
              pw.Center(child: pw.Text('Grazie mille per la vostra visita!', style: pw.TextStyle(font: courier, fontSize: 8))),
              pw.Center(child: pw.Text('Faktura osoa: info@t1jatetxea.eus', style: pw.TextStyle(font: courier, fontSize: 7.5))),
              pw.Center(child: pw.Text('========================================', style: pw.TextStyle(font: courier, fontSize: 8))),
            ],
          );
        },
      ),
    );

    return pdf.save();
  }

  static pw.Widget _pdfLerroa(String ezker, String eskuin, pw.Font fRegular, pw.Font fBold) {
    return pw.Padding(
      padding: const pw.EdgeInsets.symmetric(vertical: 0.8),
      child: pw.Row(
        mainAxisAlignment: pw.MainAxisAlignment.spaceBetween,
        children: [
          pw.Text(ezker, style: pw.TextStyle(font: fRegular, fontSize: 7.5)),
          pw.Text(eskuin, style: pw.TextStyle(font: fBold, fontSize: 7.5)),
        ],
      ),
    );
  }

  static Future<void> inprimatuEdoDeskargatu({
    required int eskaeraId,
    required DateTime data,
    required int mahaiZenbakia,
    required String eskaeraMota,
    required String bezeroIzena,
    required List<CartItem> saskia,
    required double guztira,
    required OrdainketaEmaitza emaitza,
  }) async {
    final pdfBytes = await sortuTiketPdfBytes(
      eskaeraId: eskaeraId,
      data: data,
      mahaiZenbakia: mahaiZenbakia,
      eskaeraMota: eskaeraMota,
      bezeroIzena: bezeroIzena,
      saskia: saskia,
      guztira: guztira,
      emaitza: emaitza,
    );

    final tiketZenb = sortuTiketZenbakia(eskaeraId, data);
    await Printing.layoutPdf(
      onLayout: (PdfPageFormat format) async => pdfBytes,
      name: 'Tiket-$tiketZenb.pdf',
      format: PdfPageFormat.roll80,
    );
  }

  static Future<void> partekatuEdoDeskargatuPdf({
    required int eskaeraId,
    required DateTime data,
    required int mahaiZenbakia,
    required String eskaeraMota,
    required String bezeroIzena,
    required List<CartItem> saskia,
    required double guztira,
    required OrdainketaEmaitza emaitza,
  }) async {
    final pdfBytes = await sortuTiketPdfBytes(
      eskaeraId: eskaeraId,
      data: data,
      mahaiZenbakia: mahaiZenbakia,
      eskaeraMota: eskaeraMota,
      bezeroIzena: bezeroIzena,
      saskia: saskia,
      guztira: guztira,
      emaitza: emaitza,
    );

    final tiketZenb = sortuTiketZenbakia(eskaeraId, data);
    await Printing.sharePdf(
      bytes: pdfBytes,
      filename: 'Tiket-$tiketZenb.pdf',
    );
  }
}
