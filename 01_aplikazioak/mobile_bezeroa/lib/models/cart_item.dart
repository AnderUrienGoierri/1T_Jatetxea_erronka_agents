import 'platera.dart';

class CartItem {
  final Platera platera;
  int kopurua;
  String oharrak;

  CartItem({
    required this.platera,
    this.kopurua = 1,
    this.oharrak = '',
  });

  double get azpitotala => platera.prezioa * kopurua;
  double get guztira => azpitotala;
}
