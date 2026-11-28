import 'package:flutter_test/flutter_test.dart';
import 'package:jatetxea_bezeroa/models/platera.dart';

void main() {
  test('APIko irudi-erreferentzia irakurtzen du', () {
    final platera = Platera.fromJson({
      'id': 25,
      'izena': 'Sagar zukua',
      'kategoria': 'Freskagarriak',
      'prezioa': 3,
      'irudiErreferentzia': 'platerak/sagar-zukua.jpg',
    });

    expect(platera.irudiErreferentzia, 'platerak/sagar-zukua.jpg');
    expect(platera.prezioa, 3.0);
  });

  test('irudirik gabeko APIko erantzuna onartzen du', () {
    final platera = Platera.fromJson({
      'id': 1,
      'izena': 'Platera',
      'kategoria': 'Lehena',
      'prezioa': 10,
    });

    expect(platera.irudiErreferentzia, isNull);
  });
}
