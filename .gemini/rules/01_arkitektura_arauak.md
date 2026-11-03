# 3 Mailako Arkitekturaren Arau Nagusiak
1. Aurre-mailak (Web, WinForms, JavaFX) ez du inoiz datu-basearekin zuzenean hitz egiten; dena C# REST API bidez doa.
2. Datu-baseak (MySQL 8.0 InnoDB) ON DELETE RESTRICT eta errenkada-mailako blokeo pesimistak erabiltzen ditu.
3. APIak errore estandarizatuak itzultzen ditu: 200, 201, 400, 404, eta 409 Conflict gatazka konkurrentetan (adibidez mahai bikoiztua edo lehengai falta).
4. "Aldaketa txikiak, egiaztapen jarraituak": zereginak txikiak dira eta test automatizatuen bidez egiaztatzen dira.
