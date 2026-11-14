import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';
import '../models/platera.dart';
import '../models/erreserba.dart';
import '../models/iritzia.dart';
import '../models/cart_item.dart';

class ApiService {
  static const String _defaultUrl = 'http://192.168.1.166:5000/api/v1';
  static const String _prefKey = 'api_base_url';

  static String _baseUrl = _defaultUrl;
  static String get baseUrl => _baseUrl;

  static Future<void> init() async {
    final prefs = await SharedPreferences.getInstance();
    _baseUrl = prefs.getString(_prefKey) ?? _defaultUrl;
  }

  static Future<void> setBaseUrl(String url) async {
    _baseUrl = url.trim().replaceAll(RegExp(r'/+$'), '');
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_prefKey, _baseUrl);
  }

  static Map<String, String> get _headers => {
    'Content-Type': 'application/json; charset=utf-8',
    'Accept': 'application/json',
  };

  // Osasun-kontrola (/health)
  static Future<(bool, bool, String)> checkHealth() async {
    try {
      final rootUrl = _baseUrl.replaceAll('/api/v1', '');
      final uri = Uri.parse('$rootUrl/health');
      final res = await http.get(uri).timeout(const Duration(seconds: 3));
      if (res.statusCode == 200) {
        return (true, true, 'API eta Datu-basea konektatuta daude.');
      }
      if (res.statusCode == 503) {
        return (true, false, 'Datu-basea deskonektatuta dago.');
      }
      return (false, false, 'API egoera kodea: ${res.statusCode}');
    } catch (e) {
      return (false, false, 'Ezin da APIarekin konektatu ($_baseUrl)');
    }
  }

  // Platerak lortu
  static Future<List<Platera>> getPlaterak({String? kategoria}) async {
    try {
      String url = '$_baseUrl/platerak';
      if (kategoria != null && kategoria != 'Guztiak' && kategoria.isNotEmpty) {
        url += '?kategoria=${Uri.encodeComponent(kategoria)}';
      }
      final res = await http.get(Uri.parse(url), headers: _headers).timeout(const Duration(seconds: 4));
      if (res.statusCode == 200) {
        final json = jsonDecode(utf8.decode(res.bodyBytes));
        final dataList = json['dataList'] as List<dynamic>? ?? [];
        return dataList.map((item) => Platera.fromJson(item as Map<String, dynamic>)).toList();
      }
    } catch (e) {
      // Isilik
    }
    return [];
  }

  // Plateren stock ahalmena
  static Future<Map<int, int>> getStockAhalmena() async {
    final Map<int, int> mapa = {};
    try {
      final uri = Uri.parse('$_baseUrl/platerak/erabilgarritasuna');
      final res = await http.get(uri, headers: _headers).timeout(const Duration(seconds: 4));
      if (res.statusCode == 200) {
        final json = jsonDecode(utf8.decode(res.bodyBytes));
        final dataList = json['dataList'] as List<dynamic>? ?? [];
        for (var item in dataList) {
          final pId = item['plateraId'] as int? ?? 0;
          final kop = item['eginDaitezkeenPlaterak'] as int? ?? 0;
          if (pId > 0) mapa[pId] = kop;
        }
      }
    } catch (e) {
      // Isilik
    }
    return mapa;
  }

  // Mahaiak lortu (dataren eta txandaren arabera disponibleak iragaziz)
  static Future<List<Map<String, dynamic>>> getMahaiak({
    String? data,
    String? txanda,
    int? pertsonaKopurua,
    String? kokapena,
  }) async {
    try {
      String url = '$_baseUrl/mahaiak';
      if (data != null && txanda != null) {
        url = '$_baseUrl/mahaiak/disponibleak?data=$data&txanda=$txanda';
        if (pertsonaKopurua != null && pertsonaKopurua > 0) {
          url += '&pertsonaKopurua=$pertsonaKopurua';
        }
        if (kokapena != null && kokapena.isNotEmpty && kokapena != 'Guztiak') {
          url += '&kokapena=${Uri.encodeComponent(kokapena)}';
        }
      }
      final uri = Uri.parse(url);
      final res = await http.get(uri, headers: _headers).timeout(const Duration(seconds: 4));
      if (res.statusCode == 200) {
        final json = jsonDecode(utf8.decode(res.bodyBytes));
        final dataList = json['dataList'] as List<dynamic>? ?? [];
        if (dataList.isNotEmpty) {
          return dataList.map((item) => item as Map<String, dynamic>).toList();
        }
      }
    } catch (e) {
      // Isilik
    }
    // Fallback: datu-baseko 11 mahai errealak (offline edo API errorean)
    return _getDefaultMahaiak(kokapena: kokapena, pertsonaKopurua: pertsonaKopurua);
  }

  static List<Map<String, dynamic>> _getDefaultMahaiak({String? kokapena, int? pertsonaKopurua}) {
    final guztiak = [
      {'id': 1, 'zenbakia': 1, 'edukiera': 2, 'kokapena': 'Jantoki Nagusia', 'egoera': 'Libre'},
      {'id': 2, 'zenbakia': 2, 'edukiera': 4, 'kokapena': 'Jantoki Nagusia', 'egoera': 'Libre'},
      {'id': 3, 'zenbakia': 3, 'edukiera': 4, 'kokapena': 'Jantoki Nagusia', 'egoera': 'Libre'},
      {'id': 4, 'zenbakia': 4, 'edukiera': 6, 'kokapena': 'Jantoki Nagusia', 'egoera': 'Libre'},
      {'id': 5, 'zenbakia': 5, 'edukiera': 8, 'kokapena': 'VIP', 'egoera': 'Libre'},
      {'id': 6, 'zenbakia': 6, 'edukiera': 4, 'kokapena': 'Terraza', 'egoera': 'Libre'},
      {'id': 7, 'zenbakia': 7, 'edukiera': 2, 'kokapena': 'Barra', 'egoera': 'Libre'},
      {'id': 8, 'zenbakia': 8, 'edukiera': 2, 'kokapena': 'Barra', 'egoera': 'Libre'},
      {'id': 9, 'zenbakia': 9, 'edukiera': 4, 'kokapena': 'Terraza', 'egoera': 'Libre'},
      {'id': 10, 'zenbakia': 10, 'edukiera': 4, 'kokapena': 'Terraza', 'egoera': 'Libre'},
      {'id': 11, 'zenbakia': 11, 'edukiera': 6, 'kokapena': 'VIP', 'egoera': 'Libre'},
    ];
    return guztiak.where((m) {
      if (kokapena != null && kokapena != 'Guztiak' && m['kokapena'] != kokapena) return false;
      if (pertsonaKopurua != null && (m['edukiera'] as int) < pertsonaKopurua) return false;
      return true;
    }).toList();
  }

  // Erreserba berria sortu (409 Conflict kontrolatuz)
  static Future<(bool, String, Erreserba?)> sortuErreserba({
    required String data,
    required String txanda,
    required int pertsonaKopurua,
    required String izena,
    required String abizena,
    required String emaila,
    required String telefonoa,
    int? mahaiaId,
    int? mahaiZenbakia,
    String? oharrak,
  }) async {
    try {
      final uri = Uri.parse('$_baseUrl/erreserbak');
      final body = jsonEncode({
        'data': data,
        'txanda': txanda,
        'pertsonaKopurua': pertsonaKopurua,
        'bezeroIzena': izena.trim(),
        'bezeroAbizena': abizena.trim().isEmpty ? '-' : abizena.trim(),
        'bezeroEmaila': emaila.trim().toLowerCase(),
        'bezeroTelefonoa': telefonoa.trim(),
        'mahaiaId': mahaiaId,
        'mahaiZenbakia': mahaiZenbakia,
        'oharrak': oharrak,
      });

      final res = await http.post(uri, headers: _headers, body: body).timeout(const Duration(seconds: 5));
      final json = jsonDecode(utf8.decode(res.bodyBytes));

      if (res.statusCode == 200 || res.statusCode == 201) {
        final err = json['data'] != null ? Erreserba.fromJson(json['data'] as Map<String, dynamic>) : null;
        return (true, json['message'] as String? ?? 'Erreserba arrakastaz baieztatu da!', err);
      } else if (res.statusCode == 409) {
        return (false, json['message'] as String? ?? 'Mahaia jada okupatuta dago txanda eta data horretan (Gatazka 409). Hautatu beste mahai edo txanda bat.', null);
      } else {
        return (false, json['message'] as String? ?? 'Errorea erreserba egitean (${res.statusCode}).', null);
      }
    } catch (e) {
      return (false, 'Konexio errorea: $e', null);
    }
  }

  // Iritziak lortu
  static Future<List<Iritzia>> getIritziak() async {
    try {
      final uri = Uri.parse('$_baseUrl/iritziak');
      final res = await http.get(uri, headers: _headers).timeout(const Duration(seconds: 4));
      if (res.statusCode == 200) {
        final json = jsonDecode(utf8.decode(res.bodyBytes));
        final dataList = json['dataList'] as List<dynamic>? ?? [];
        return dataList.map((item) => Iritzia.fromJson(item as Map<String, dynamic>)).toList();
      }
    } catch (e) {
      // Isilik
    }
    return [];
  }

  // Iritzi estatistikak lortu
  static Future<IritziEstatistikak?> getIritziEstatistikak() async {
    try {
      final uri = Uri.parse('$_baseUrl/iritziak/estatistikak');
      final res = await http.get(uri, headers: _headers).timeout(const Duration(seconds: 4));
      if (res.statusCode == 200) {
        final json = jsonDecode(utf8.decode(res.bodyBytes));
        if (json['data'] != null) {
          return IritziEstatistikak.fromJson(json['data'] as Map<String, dynamic>);
        }
      }
    } catch (e) {
      // Isilik
    }
    return null;
  }

  // Iritzia sortu
  static Future<(bool, String)> sortuIritzia({
    required String izena,
    required String emaila,
    required int balorazioa,
    String? testua,
  }) async {
    try {
      final uri = Uri.parse('$_baseUrl/iritziak');
      final body = jsonEncode({
        'bezeroIzena': izena.trim(),
        'bezeroEmaila': emaila.trim().toLowerCase(),
        'balorazioa': balorazioa,
        'testua': testua?.trim(),
      });
      final res = await http.post(uri, headers: _headers, body: body).timeout(const Duration(seconds: 4));
      final json = jsonDecode(utf8.decode(res.bodyBytes));
      if (res.statusCode == 200 || res.statusCode == 201) {
        return (true, json['message'] as String? ?? 'Eskerrik asko zure iritziagatik!');
      }
      return (false, json['message'] as String? ?? 'Ezin izan da iritzia gorde.');
    } catch (e) {
      return (false, 'Konexio errorea: $e');
    }
  }

  // Bezeroaren eskaera sortu eta sukaldera bidali (Ordainketaren ostean)
  static Future<(bool, String, int)> sortuEskaeraBezeroa({
    required List<CartItem> saskia,
    required int mahaiaId,
    required String bezeroIzena,
    required String oharrak,
  }) async {
    try {
      // 1. Eskaera sortu sisteman (langileaId: 1 lehenetsia)
      final uriE = Uri.parse('$_baseUrl/eskaerak');
      final bodyE = jsonEncode({
        'mahaiaId': mahaiaId > 0 ? mahaiaId : 1,
        'langileaId': 1,
        'pertsonaKopurua': 2,
      });
      final resE = await http.post(uriE, headers: _headers, body: bodyE).timeout(const Duration(seconds: 4));
      final jsonE = jsonDecode(utf8.decode(resE.bodyBytes));

      if (resE.statusCode != 200 && resE.statusCode != 201) {
        return (false, 'Ezin izan da eskaera ireki: ${jsonE['message']}', 0);
      }

      final eskaeraId = jsonE['data']?['id'] as int? ?? 0;
      if (eskaeraId <= 0) {
        return (false, 'Eskaera ID baliogabea.', 0);
      }

      // 2. Plater guztiak banan-banan gehitu
      for (var item in saskia) {
        final uriL = Uri.parse('$_baseUrl/eskaerak/$eskaeraId/lerroak');
        final bodyL = jsonEncode({
          'plateraId': item.platera.id,
          'kantitatea': item.kopurua,
          'oharrak': item.oharrak.isNotEmpty ? item.oharrak : null,
        });
        await http.post(uriL, headers: _headers, body: bodyL).timeout(const Duration(seconds: 4));
      }

      // 3. Sukaldera bidali automatikoki
      final uriS = Uri.parse('$_baseUrl/eskaerak/$eskaeraId/sukaldera');
      await http.post(uriS, headers: _headers).timeout(const Duration(seconds: 4));

      return (true, 'Eskaera sukaldera bidali da!', eskaeraId);
    } catch (e) {
      return (false, 'Errorea eskaera bideratzean: $e', 0);
    }
  }
}


