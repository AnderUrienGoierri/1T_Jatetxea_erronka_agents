/**
 * JATETXEA - Karta eta Plateren Kudeaketa (karta.js)
 */

let platerakCache = [];
let erabilgarritasunaCache = {};
let unekoKategoria = "Guztiak";

/**
 * Platerak eta uneko stock ahalmena kargatzen ditu REST APItik.
 */
async function kargatuKarta() {
  const edukiontzia = document.getElementById("platerak-zerrenda");
  if (!edukiontzia) return;

  edukiontzia.innerHTML =
    '<p class="kargatzen-testua">Platerak kargatzen...</p>';

  try {
    // 1. Plater guztiak berreskuratu
    const resPlaterak = await fetch(`${API_BASE}/platerak`);
    const jsonPlaterak = await resPlaterak.json();

    // 2. Erabilgarritasuna berreskuratu (zenbat anoa prestatu daitezkeen kalkulatuta)
    const resAhalmena = await fetch(`${API_BASE}/platerak/erabilgarritasuna`);
    const jsonAhalmena = await resAhalmena.json();

    if (jsonPlaterak.status === "success" && jsonPlaterak.dataList) {
      platerakCache = jsonPlaterak.dataList;

      erabilgarritasunaCache = {};
      if (jsonAhalmena.status === "success" && jsonAhalmena.dataList) {
        jsonAhalmena.dataList.forEach((item) => {
          erabilgarritasunaCache[item.plateraId] = item.eginDaitezkeenPlaterak;
        });
      }

      bistaratuPlaterak();
    } else {
      edukiontzia.innerHTML = "<p>Ez da platerik aurkitu.</p>";
    }
  } catch (err) {
    edukiontzia.innerHTML = `<p class="errore-testua">Ezin izan da karta kargatu: ${err.message}. Egiaztatu REST APIa martxan dagoen.</p>`;
  }
}

/**
 * Plateren zerrenda renderizatzen du uneko kategoria-iragazkiaren arabera.
 */
function bistaratuPlaterak() {
  const edukiontzia = document.getElementById("platerak-zerrenda");
  if (!edukiontzia) return;

  const iragaziak =
    unekoKategoria === "Guztiak"
      ? platerakCache
      : platerakCache.filter((p) => p.kategoria === unekoKategoria);

  if (iragaziak.length === 0) {
    edukiontzia.innerHTML = "<p>Ez dago kategoria honetako platerik.</p>";
    return;
  }

  edukiontzia.innerHTML = iragaziak
    .map((platera) => {
      const anoak = erabilgarritasunaCache[platera.id] ?? 0;
      const agortutaDago = anoak <= 0;
      const cardClass = agortutaDago ? "plater-card agortuta" : "plater-card";
      const stockBadge = agortutaDago
        ? '<span class="stock-tag agortuta">Agortuta</span>'
        : `<span class="stock-tag eskuragarri">${anoak} anoa eskuragarri</span>`;

      const alergenoakHtml = platera.alergenoak
        ? `<span class="alergenoak-tag">Alergenoak: ${eskapatuHtml(platera.alergenoak)}</span>`
        : "";

      return `
            <div class="${cardClass}" data-id="${platera.id}">
                <div>
                    <div class="plater-header">
                        <h4 class="plater-izena">${eskapatuHtml(platera.izena)}</h4>
                        <span class="plater-prezioa">${platera.prezioa.toFixed(2)} &euro;</span>
                    </div>
                    <p class="plater-deskribapena">${eskapatuHtml(platera.deskribapena || "Deskribapenik ez.")}</p>
                </div>
                <div class="plater-meta">
                    <span class="kategoria-tag">${eskapatuHtml(platera.kategoria)}</span>
                    ${alergenoakHtml}
                    ${stockBadge}
                </div>
            </div>
        `;
    })
    .join("");
}

/**
 * Kategoria-iragazkien gertaerak konfiguratzen ditu.
 */
function konfiguratuIragazkiak() {
  const botoiak = document.querySelectorAll(".filter-btn");
  botoiak.forEach((btn) => {
    btn.addEventListener("click", () => {
      botoiak.forEach((b) => b.classList.remove("active"));
      btn.classList.add("active");
      unekoKategoria = btn.getAttribute("data-kategoria") || "Guztiak";
      bistaratuPlaterak();
    });
  });
}
