/**
 * JATETXEA - Aplikazioaren Sarrera eta Orkestrazio Nagusia (app.js)
 * Arkitektura: PHP Server-Side Rendering + Bezeroaren UI arina
 */

document.addEventListener("DOMContentLoaded", () => {
  // 1. APIaren egoera bisuala egiaztatu
  if (typeof egiaztatuApiEgoera === "function") {
    egiaztatuApiEgoera();
  }

  // 2. Erreserben dataren hautatzailea konfiguratuta (gutxienekoa gaur)
  if (typeof hasieratuDataAukeratzailea === "function") {
    hasieratuDataAukeratzailea();
  }

  // 3. Karta iragazkiak konfiguratuta (PHPk renderizatutako txartelak berehala iragazteko)
  if (typeof konfiguratuIragazkiak === "function") {
    konfiguratuIragazkiak();
  }

  // 4. Egiaztatu txartelak (fallback zerbitzari arazoetarako)
  if (typeof egiaztatuEdoKargatuKarta === "function") {
    egiaztatuEdoKargatuKarta();
  }

  // Oharra: Formularioen bidalketak (Erreserba eta Iritziak) modu naturalean PHP zerbitzarira
  // bideratzen dira (POST bidez), PHPk kudea ditzan negozio-logika eta C# REST API deiak.
});
