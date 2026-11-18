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

  // 5. Goiburuko izenean edo logoan klikatzean orria freskatu eta goran kokatu
  const hasieraEstekak = document.querySelectorAll(".brand-title-link, .header-logo-link");
  hasieraEstekak.forEach((esteka) => {
    esteka.addEventListener("click", (e) => {
      e.preventDefault();
      // URL-ko hash fragmentoa kendu eta hasierako orrira birbideratu goran kokatuz
      if (window.location.hash) {
        window.location.href = "index.php";
      } else {
        window.scrollTo({ top: 0, behavior: "smooth" });
        // Behar izanez gero freskatu
        setTimeout(() => {
          window.location.href = "index.php";
        }, 150);
      }
    });
  });
});
