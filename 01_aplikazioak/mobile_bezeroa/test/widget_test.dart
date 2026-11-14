import 'package:flutter_test/flutter_test.dart';
import 'package:jatetxea_bezeroa/main.dart';
import 'package:shared_preferences/shared_preferences.dart';

void main() {
  testWidgets('Bezeroaren aplikazioaren hasieratze testa', (WidgetTester tester) async {
    SharedPreferences.setMockInitialValues({});
    await tester.pumpWidget(const JatetxeaBezeroaApp());
    await tester.pump();

    // Egiaztatu beheko nabigazio barraren etiketak ageri direla
    expect(find.text('Karta'), findsWidgets);
    expect(find.text('Erreserbak'), findsWidgets);
    expect(find.text('Iritziak'), findsWidgets);
    expect(find.text('Informazioa'), findsWidgets);
  });
}
