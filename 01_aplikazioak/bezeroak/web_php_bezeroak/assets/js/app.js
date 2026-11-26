/**
 * JATETXEA - Aplikazioaren Sarrera eta Orkestrazio Nagusia (app.js)
 * Arkitektura: PHP Server-Side Rendering + Bezeroaren UI arina
 */

function konfiguratuNabigazioa() {
  const header = document.querySelector("header");
  const estekak = Array.from(document.querySelectorAll("header nav a[href^='#']"));
  const atalak = estekak.map((esteka) => ({
    esteka,
    atala: document.getElementById(esteka.getAttribute("href").slice(1)),
  })).filter((item) => item.atala);
  if (!header || !atalak.length) return;

  let aukeratutakoAtala = null;
  let denboraMuga = 0;
  let programatuta = false;

  function aktibatu(aktiboa) {
    atalak.forEach(({ esteka, atala }) => {
      const hautatuta = atala === aktiboa;
      esteka.classList.toggle("active", hautatuta);
      if (hautatuta) esteka.setAttribute("aria-current", "location");
      else esteka.removeAttribute("aria-current");
    });
  }

  function eguneratu() {
    programatuta = false;
    const goiburua = header.getBoundingClientRect().height;
    document.documentElement.style.scrollPaddingTop = `${goiburua + 12}px`;
    const behean = window.scrollY + window.innerHeight >= document.documentElement.scrollHeight - 2;
    if (aukeratutakoAtala && performance.now() < denboraMuga &&
        !behean && Math.abs(aukeratutakoAtala.getBoundingClientRect().top - goiburua - 12) > 3) return;
    aukeratutakoAtala = null;
    let aktiboa = null;
    atalak.forEach(({ atala }) => {
      if (atala.getBoundingClientRect().top <= goiburua + 20) aktiboa = atala;
    });
    if (behean) aktiboa = atalak[atalak.length - 1].atala;
    aktibatu(aktiboa);
  }

  function eskatuEguneratzea() {
    if (!programatuta) {
      programatuta = true;
      requestAnimationFrame(eguneratu);
    }
  }

  atalak.forEach(({ esteka, atala }) => esteka.addEventListener("click", () => {
    aukeratutakoAtala = atala;
    denboraMuga = performance.now() + 2000;
    aktibatu(atala);
  }));
  ["wheel", "touchstart", "keydown"].forEach((gertaera) => {
    window.addEventListener(gertaera, () => { aukeratutakoAtala = null; }, { passive: true });
  });
  window.addEventListener("scroll", eskatuEguneratzea, { passive: true });
  window.addEventListener("resize", eskatuEguneratzea);
  window.addEventListener("load", eskatuEguneratzea);
  eguneratu();
}

document.addEventListener("DOMContentLoaded", () => {
  konfiguratuNabigazioa();
  const ezarpenak = document.querySelector(".settings-menu");
  if (ezarpenak) {
    document.addEventListener("click", (event) => {
      if (!ezarpenak.contains(event.target)) ezarpenak.open = false;
    });
    document.addEventListener("keydown", (event) => {
      if (event.key === "Escape" && ezarpenak.open) {
        ezarpenak.open = false;
        ezarpenak.querySelector("summary").focus();
      }
    });
  }
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
