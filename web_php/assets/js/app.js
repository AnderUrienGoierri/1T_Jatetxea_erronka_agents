/**
 * JATETXEA - Aplikazioaren Sarrera eta Orkestrazio Nagusia (app.js)
 * Moduluak: config.js, karta.js, erreserbak.js, iritziak.js
 */

document.addEventListener("DOMContentLoaded", () => {
  // 1. Oinarrizko egoera eta API egiaztapena
  egiaztatuApiEgoera();
  hasieratuDataAukeratzailea();

  // 2. Datuen hasierako karga asinkronoa
  kargatuKarta();
  kargatuMahaiak();
  kargatuIritziak();
  kargatuEstatistikak();

  // 3. Formularioen eta iragazkien gertaerak
  konfiguratuIragazkiak();

  const erreserbaForm = document.getElementById("erreserba-form");
  if (erreserbaForm) {
    erreserbaForm.addEventListener("submit", bidaliErreserba);
  }

  const iritziForm = document.getElementById("iritzi-form");
  if (iritziForm) {
    iritziForm.addEventListener("submit", bidaliIritzia);
  }
});
