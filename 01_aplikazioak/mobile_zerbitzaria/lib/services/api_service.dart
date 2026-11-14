import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';
import '../models/langilea.dart';
import '../models/mahaia.dart';
import '../models/platera.dart';
import '../models/eskaera.dart';
import '../models/ordainketa.dart';

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

  // Langileak lortu
  static Future<List<Langilea>> getLangileak() async {
    try {
      final uri = Uri.parse('$_baseUrl/langileak');
      final res = await http.get(uri, headers: _headers).timeout(const Duration(seconds: 4));
      if (res.statusCode == 200) {
        final json = jsonDecode(utf8.decode(res.bodyBytes));
        final dataList = json['dataList'] as List<dynamic>? ?? [];
        return dataList.map((item) => Langilea.fromJson(item as Map<String, dynamic>)).toList();
      }
    } catch (e) {
      // Isilik kudeatu
    }
    return [];
  }

  // Login PIN bidez
  static Future<(bool, String, Langilea?)> login(String erabiltzailea, String pasahitza) async {
    try {
      final uri = Uri.parse('$_baseUrl/langileak/login');
      final body = jsonEncode({'erabiltzailea': erabiltzailea, 'pasahitza': pasahitza});
      final res = await http.post(uri, headers: _headers, body: body).timeout(const Duration(seconds: 4));

      if (res.statusCode == 200) {
        final json = jsonDecode(utf8.decode(res.bodyBytes));
        final data = json['data'] as Map<String, dynamic>?;
        if (data != null && data['arrakasta'] == true && data['langilea'] != null) {
          final langilea = Langilea.fromJson(data['langilea'] as Map<String, dynamic>);
          return (true, data['mezua'] as String? ?? 'Ongi etorri', langilea);
        }
        return (false, data?['mezua'] as String? ?? 'PIN okerra.', null);
      } else {
        return (false, 'PIN okerra edo zerbitzariaren errorea (${res.statusCode}).', null);
      }
    } catch (e) {
      return (false, 'Konexio errorea: Ziurtatu APIa martxan dagoela ($_baseUrl).', null);
    }
  }

  // Mahaiak lortu
  static Future<List<Mahaia>> getMahaiak() async {
    try {
      final uri = Uri.parse('$_baseUrl/mahaiak');
      final res = await http.get(uri, headers: _headers).timeout(const Duration(seconds: 4));
      if (res.statusCode == 200) {
        final json = jsonDecode(utf8.decode(res.bodyBytes));
        final dataList = json['dataList'] as List<dynamic>? ?? [];
        return dataList.map((item) => Mahaia.fromJson(item as Map<String, dynamic>)).toList();
      }
    } catch (e) {
      // Isilik
    }
    return [];
  }

  // Mahaiaren egoera eguneratu
  static Future<bool> eguneratuMahaiarenEgoera(int mahaiaId, String egoera) async {
    try {
      final uri = Uri.parse('$_baseUrl/mahaiak/$mahaiaId/egoera');
      final body = jsonEncode({'egoera': egoera});
      final res = await http.put(uri, headers: _headers, body: body).timeout(const Duration(seconds: 3));
      return res.statusCode == 200;
    } catch (e) {
      return false;
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

  // Mahai baten eskaera aktibo guztiak (kontu banatuak barne)
  static Future<List<Eskaera>> getMahaiarenEskaerak(int mahaiaId) async {
    try {
      final uri = Uri.parse('$_baseUrl/eskaerak?mahaiaId=$mahaiaId');
      final res = await http.get(uri, headers: _headers).timeout(const Duration(seconds: 4));
      if (res.statusCode == 200) {
        final json = jsonDecode(utf8.decode(res.bodyBytes));
        final dataList = json['dataList'] as List<dynamic>? ?? [];
        final aktiboak = dataList
            .map((item) => Eskaera.fromJson(item as Map<String, dynamic>))
            .where((e) => e.egoera != 'Kobratua' && e.egoera != 'Ezeztatua')
            .toList();

        // Xehetasunak kargatu lerroak edukitzeko
        final List<Eskaera> emaitzak = [];
        for (var e in aktiboak) {
          final x = await getEskaeraXehetasunak(e.id);
          emaitzak.add(x ?? e);
        }
        return emaitzak;
      }
    } catch (e) {
      // Isilik
    }
    return [];
  }

  // Eskaeraren xehetasunak lerroekin
  static Future<Eskaera?> getEskaeraXehetasunak(int eskaeraId) async {
    try {
      final uri = Uri.parse('$_baseUrl/eskaerak/$eskaeraId');
      final res = await http.get(uri, headers: _headers).timeout(const Duration(seconds: 4));
      if (res.statusCode == 200) {
        final json = jsonDecode(utf8.decode(res.bodyBytes));
        if (json['data'] != null) {
          return Eskaera.fromJson(json['data'] as Map<String, dynamic>);
        }
      }
    } catch (e) {
      // Isilik
    }
    return null;
  }

  // Eskaera berria sortu
  static Future<(bool, String, Eskaera?)> sortuEskaera(int mahaiaId, int langileaId, int pertsonaKopurua) async {
    try {
      final uri = Uri.parse('$_baseUrl/eskaerak');
      final body = jsonEncode({
        'mahaiaId': mahaiaId,
        'langileaId': langileaId,
        'pertsonaKopurua': pertsonaKopurua,
      });
      final res = await http.post(uri, headers: _headers, body: body).timeout(const Duration(seconds: 4));
      final json = jsonDecode(utf8.decode(res.bodyBytes));
      if (res.statusCode == 200 || res.statusCode == 201) {
        final data = json['data'] != null ? Eskaera.fromJson(json['data'] as Map<String, dynamic>) : null;
        return (true, json['message'] as String? ?? 'Eskaera ireki da.', data);
      }
      return (false, json['message'] as String? ?? 'Ezin izan da eskaera sortu.', null);
    } catch (e) {
      return (false, 'Errorea eskaera sortzean: $e', null);
    }
  }

  // Platera gehitzea eskaerara
  static Future<(bool, String)> gehituPlateraEskaerara(int eskaeraId, int plateraId, int kantitatea, String? oharrak) async {
    try {
      final uri = Uri.parse('$_baseUrl/eskaerak/$eskaeraId/lerroak');
      final body = jsonEncode({
        'plateraId': plateraId,
        'kantitatea': kantitatea,
        'oharrak': oharrak,
      });
      final res = await http.post(uri, headers: _headers, body: body).timeout(const Duration(seconds: 4));
      if (res.statusCode == 200 || res.statusCode == 201) {
        return (true, 'Platera ondo gehitu da.');
      }
      return (false, 'Errorea platera gehitzean: ${res.body}');
    } catch (e) {
      return (false, 'Konexio errorea: $e');
    }
  }

  // Sukaldera bidali
  static Future<(bool, String)> bidaliSukaldera(int eskaeraId) async {
    try {
      final uri = Uri.parse('$_baseUrl/eskaerak/$eskaeraId/sukaldera');
      final res = await http.post(uri, headers: _headers).timeout(const Duration(seconds: 4));
      if (res.statusCode == 200) {
        return (true, 'Platerak sukaldera bidali dira eta martxan daude.');
      }
      return (false, 'Ezin izan da sukaldera bidali (${res.statusCode}).');
    } catch (e) {
      return (false, 'Konexio errorea: $e');
    }
  }

  // Eskaera lerro baten egoera eguneratu ('Eskatua', 'Sukaldean', 'Prestatua', 'Zerbitzatua')
  static Future<bool> eguneratuLerroEgoera(int lerroId, String egoera) async {
    try {
      final uri = Uri.parse('$_baseUrl/eskaerak/lerroak/$lerroId/egoera');
      final body = jsonEncode({'egoera': egoera});
      final res = await http.put(uri, headers: _headers, body: body).timeout(const Duration(seconds: 4));
      return res.statusCode >= 200 && res.statusCode < 300;
    } catch (e) {
      return false;
    }
  }

  // Eskaeraren egoera aldatu (adibidez: Kobratua)
  static Future<bool> eguneratuEskaerarenEgoera(int eskaeraId, String egoera) async {
    try {
      final uri = Uri.parse('$_baseUrl/eskaerak/$eskaeraId/egoera');
      final body = jsonEncode({'egoera': egoera});
      final res = await http.put(uri, headers: _headers, body: body).timeout(const Duration(seconds: 3));
      return res.statusCode == 200;
    } catch (e) {
      return false;
    }
  }

  // Ordainketa segurua prozesatu (Eskudirua, Txartela, Bizum) eta tiketa sortu
  static Future<(bool, String, OrdainketaErantzuna?)> prozesatuOrdainketa({
    required int eskaeraId,
    required String mota,
    required double zenbatekoa,
    double? jasotakoa,
    String? txartelZenbakia,
    String? titularra,
    String? iraungitzea,
    String? cvv,
    String? bizumTelefonoa,
    int? langileaId,
  }) async {
    try {
      final uri = Uri.parse('$_baseUrl/ordainketak');
      final body = jsonEncode({
        'eskaeraId': eskaeraId,
        'mota': mota,
        'zenbatekoa': zenbatekoa,
        'jasotakoa': jasotakoa,
        'txartelZenbakia': txartelZenbakia,
        'titularra': titularra,
        'iraungitzea': iraungitzea,
        'cvv': cvv,
        'bizumTelefonoa': bizumTelefonoa,
        'langileaId': langileaId,
      });

      final res = await http.post(uri, headers: _headers, body: body).timeout(const Duration(seconds: 6));
      final json = jsonDecode(utf8.decode(res.bodyBytes));

      if (res.statusCode == 200 || res.statusCode == 201) {
        final data = json['data'] as Map<String, dynamic>?;
        if (data != null) {
          final ord = OrdainketaErantzuna.fromJson(data);
          return (true, json['message'] as String? ?? 'Ordainketa arrakastatsua', ord);
        }
      }

      return (false, json['message'] as String? ?? 'Ezin izan da ordainketa burutu.', null);
    } catch (e) {
      return (false, 'Konexio errorea ordaintzean: $e', null);
    }
  }
}


