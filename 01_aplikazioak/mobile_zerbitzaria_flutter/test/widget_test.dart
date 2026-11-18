import 'package:flutter_test/flutter_test.dart';
import 'package:jatetxea_zerbitzaria/main.dart';

void main() {
  testWidgets('App hasiera proba', (WidgetTester tester) async {
    await tester.pumpWidget(const JatetxeaZerbitzariaApp());
    expect(find.text('T1 JATETXEA'), findsOneWidget);
  });
}
