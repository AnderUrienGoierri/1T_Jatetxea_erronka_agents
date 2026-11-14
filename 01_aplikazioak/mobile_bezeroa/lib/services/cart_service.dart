import 'package:flutter/foundation.dart';
import '../models/cart_item.dart';
import '../models/platera.dart';

class CartService extends ChangeNotifier {
  static final CartService _instance = CartService._internal();
  factory CartService() => _instance;
  CartService._internal();

  final List<CartItem> _items = [];

  List<CartItem> get items => List.unmodifiable(_items);

  int get itemCount => _items.fold(0, (sum, item) => sum + item.kopurua);

  double get azpitotala => _items.fold(0.0, (sum, item) => sum + item.azpitotala);

  double get bez => azpitotala * 0.10;

  double get guztira => azpitotala + bez;

  bool get isEmpty => _items.isEmpty;

  void add(Platera platera, {int kopurua = 1, String oharrak = ''}) {
    final index = _items.indexWhere((item) => item.platera.id == platera.id);
    if (index >= 0) {
      _items[index].kopurua += kopurua;
      if (oharrak.isNotEmpty) {
        _items[index].oharrak = oharrak;
      }
    } else {
      _items.add(CartItem(
        platera: platera,
        kopurua: kopurua,
        oharrak: oharrak,
      ));
    }
    notifyListeners();
  }

  void updateKopurua(int plateraId, int delta) {
    final index = _items.indexWhere((item) => item.platera.id == plateraId);
    if (index >= 0) {
      _items[index].kopurua += delta;
      if (_items[index].kopurua <= 0) {
        _items.removeAt(index);
      }
      notifyListeners();
    }
  }

  void updateOharrak(int plateraId, String oharrak) {
    final index = _items.indexWhere((item) => item.platera.id == plateraId);
    if (index >= 0) {
      _items[index].oharrak = oharrak;
      notifyListeners();
    }
  }

  void removeItem(int plateraId) {
    _items.removeWhere((item) => item.platera.id == plateraId);
    notifyListeners();
  }

  void clear() {
    _items.clear();
    notifyListeners();
  }
}
