/**
 * JATETXEA - Karta eta Plateren Kudeaketa (karta.js)
 * PHP zerbitzariak sortutako elementuekin lan egiten du, bezeroan iragazketa azkarra eskainiz.
 */

let unekoKategoria = "Guztiak";

/**
 * Kategoria-iragazkien gertaerak konfiguratzen ditu.
 */
function konfiguratuIragazkiak() {
  const botoiak = document.querySelectorAll(".filter-btn");
  if (!botoiak || botoiak.length === 0) return;

  botoiak.forEach((btn) => {
    btn.addEventListener("click", () => {
      botoiak.forEach((b) => b.classList.remove("active"));
      btn.classList.add("active");
      unekoKategoria = btn.getAttribute("data-kategoria") || "Guztiak";
      iragaziPlaterak();
    });
  });
}

/**
 * DOM-eko plater txartelak ezkutatu edo erakusten ditu hautatutako kategoriaren arabera.
 */
function iragaziPlaterak() {
  const txartelak = document.querySelectorAll("#platerak-zerrenda .plater-card");
  if (!txartelak || txartelak.length === 0) return;

  txartelak.forEach((card) => {
    const kategoria = card.getAttribute("data-kategoria");
    if (unekoKategoria === "Guztiak" || kategoria === unekoKategoria) {
      card.style.display = "";
    } else {
      card.style.display = "none";
    }
  });
}

/**
 * PHPk platerik kargatu ez balu (adibidez zerbitzariak huts egin duelako),
 * JavaScript-ek bigarren aukera bezala APIari dei diezaioke.
 */
async function egiaztatuEdoKargatuKarta() {
  const edukiontzia = document.getElementById("platerak-zerrenda");
  if (!edukiontzia) return;

  const existitzenDira = edukiontzia.querySelectorAll(".plater-card");
  if (existitzenDira.length > 0) {
    // Jada PHPk zerbitzari-mailan renderizatu ditu txartel guztiak
    return;
  }

  // Fallback: JS bidez kargatu hutsik badago
  try {
    const res = await fetch(`${window.API_BASE_URL || "http://localhost:5000/api/v1"}/platerak`);
    const json = await res.json();
    if (json.status === "success" && json.dataList && json.dataList.length > 0) {
      edukiontzia.innerHTML = json.dataList.map((p) => `
        <div class="plater-card" data-kategoria="${p.kategoria}">
          <div class="plater-card-header">
            <span class="plater-kategoria-badge">${p.kategoria}</span>
            <span class="plater-prezioa">${p.salmentaPrezioa.toFixed(2)} &euro;</span>
          </div>
          <h3 class="plater-titulua">${p.izena}</h3>
          <p class="plater-deskribapena">${p.deskribapena || ''}</p>
        </div>
      `).join("");
      iragaziPlaterak();
    }
  } catch (e) {
    // Isilik utzi
  }
}
