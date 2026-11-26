/**
 * JATETXEA - Karta eta Plateren Kudeaketa (karta.js)
 * PHP zerbitzariak sortutako elementuekin lan egiten du, bezeroan iragazketa azkarra eskainiz.
 */

let unekoKategoria = "Guztiak";
let unekoEdariAzpikategoria = "Guztiak";

const edariKategoriak = new Set([
  "Edaria", "Urak", "Freskagarriak", "Garagardoak", "Sagardoak",
  "Ardoak", "Kafeak", "Infusioak", "Likoreak", "Konbinatuak", "Koktailak",
]);

const alergenoIkurrak = {
  glutena: "🌾",
  krustazeoak: "🦀",
  arrautza: "🥚",
  arrautzak: "🥚",
  arraina: "🐟",
  arrainak: "🐟",
  kakahueteak: "🥜",
  kakahuetea: "🥜",
  soja: "🫘",
  laktosa: "🥛",
  esnea: "🥛",
  esnekiak: "🥛",
  "fruitu lehorrak": "🌰",
  apioa: "🥬",
  mostaza: "🫙",
  ziapea: "🫙",
  sesamoa: "🌱",
  sulfitoak: "SO₂",
  moluskuak: "🐚",
  lupinua: "🌼",
  lupinuak: "🌼",
};

function erakutsiAlergenoIkurrak(edukiontzia) {
  edukiontzia.querySelectorAll(".plater-alergenoak").forEach((elementua) => {
    const izenak = elementua.dataset.alergenoak.split(",").map((izena) => izena.trim()).filter(Boolean);
    if (!izenak.length) return;

    const zerrenda = document.createDocumentFragment();
    izenak.forEach((izena) => {
      const ikurra = alergenoIkurrak[izena.toLocaleLowerCase("eu")];
      const etiketa = document.createElement("span");
      if (ikurra) {
        etiketa.className = `alergeno-ikurra${ikurra === "SO₂" ? " kimikoa" : ""}`;
        etiketa.setAttribute("role", "img");
        etiketa.setAttribute("aria-label", `Alergenoa: ${izena}`);
        etiketa.setAttribute("title", izena);
        etiketa.dataset.izena = izena;
        etiketa.tabIndex = 0;
        etiketa.textContent = ikurra;
      } else {
        etiketa.className = "alergeno-ezezaguna";
        etiketa.textContent = izena;
      }
      zerrenda.appendChild(etiketa);
    });
    elementua.replaceChildren(zerrenda);
    elementua.classList.add("ikurrak");
    elementua.setAttribute("aria-label", "Alergenoak");
  });
}

/**
 * Kategoria-iragazkien gertaerak konfiguratzen ditu.
 */
function konfiguratuIragazkiak() {
  const botoiak = document.querySelectorAll(".filter-btn");
  if (!botoiak || botoiak.length === 0) return;

  const azpikategoriak = document.getElementById("edari-azpikategoriak");
  const azpibotoiak = ["Guztiak", ...Array.from(edariKategoriak).filter((kategoria) => kategoria !== "Edaria")]
    .map((kategoria) => {
      const botoia = document.createElement("button");
      botoia.type = "button";
      botoia.className = "edari-azpikategoria-btn";
      botoia.textContent = kategoria;
      botoia.addEventListener("click", () => {
        unekoEdariAzpikategoria = kategoria;
        eguneratuAzpibotoiak();
        iragaziPlaterak();
      });
      azpikategoriak.appendChild(botoia);
      return botoia;
    });

  function eguneratuAzpibotoiak() {
    azpibotoiak.forEach((botoia) => {
      const aktiboa = botoia.textContent === unekoEdariAzpikategoria;
      botoia.classList.toggle("active", aktiboa);
      botoia.setAttribute("aria-pressed", String(aktiboa));
    });
  }

  eguneratuAzpibotoiak();
  botoiak.forEach((btn) => {
    btn.addEventListener("click", () => {
      botoiak.forEach((b) => b.classList.remove("active"));
      btn.classList.add("active");
      unekoKategoria = btn.getAttribute("data-kategoria") || "Guztiak";
      unekoEdariAzpikategoria = "Guztiak";
      azpikategoriak.hidden = unekoKategoria !== "Edaria";
      eguneratuAzpibotoiak();
      iragaziPlaterak();
    });
  });
  iragaziPlaterak();
}

/**
 * DOM-eko plater txartelak ezkutatu edo erakusten ditu hautatutako kategoriaren arabera.
 */
function iragaziPlaterak() {
  const txartelak = document.querySelectorAll("#platerak-zerrenda .plater-card");
  if (!txartelak || txartelak.length === 0) return;

  txartelak.forEach((card) => {
    const kategoria = card.getAttribute("data-kategoria");
    const batDator = unekoKategoria === "Edaria"
      ? edariKategoriak.has(kategoria) &&
        (unekoEdariAzpikategoria === "Guztiak" || kategoria === unekoEdariAzpikategoria)
      : (unekoKategoria === "Guztiak" && !edariKategoriak.has(kategoria)) ||
        kategoria === unekoKategoria ||
        (unekoKategoria === "Sarrerakoa" && kategoria === "Lehena");
    if (batDator) {
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
    erakutsiAlergenoIkurrak(edukiontzia);
    // Jada PHPk zerbitzari-mailan renderizatu ditu txartel guztiak
    return;
  }

  // Fallback: JS bidez kargatu hutsik badago
  try {
    const res = await fetch(`${API_BASE}/platerak`, {
      headers: { "X-App-Id": "web_php" },
    });
    const json = await res.json();
    if (json.status === "success" && json.dataList && json.dataList.length > 0) {
      edukiontzia.innerHTML = json.dataList.map((p) => {
        const id = Number(p.id);
        const escapeHtml = (value) => String(value || "").replace(/[&<>"']/g, (char) => ({
          "&": "&amp;",
          "<": "&lt;",
          ">": "&gt;",
          '"': "&quot;",
          "'": "&#39;",
        })[char]);
        const irudiIzena = escapeHtml(p.izena);
        const alergenoak = escapeHtml(p.alergenoak);
        const irudia = Number.isSafeInteger(id) && id > 0
          ? `<img class="plater-irudia" src="assets/img/platerak/platera_${id}.jpg" alt="${irudiIzena}" loading="lazy" decoding="async">`
          : "";

        return `
          <div class="plater-card" data-kategoria="${p.kategoria}">
            ${irudia}
            <div class="plater-card-header">
              <span class="plater-kategoria-badge">${p.kategoria}</span>
              <span class="plater-prezioa">${p.salmentaPrezioa.toFixed(2)} &euro;</span>
            </div>
            <h3 class="plater-titulua">${p.izena}</h3>
            <p class="plater-deskribapena">${p.deskribapena || ''}</p>
            ${alergenoak ? `<p class="plater-alergenoak" data-alergenoak="${alergenoak}"><strong>Alergenoak:</strong> ${alergenoak}</p>` : ""}
          </div>
        `;
      }).join("");
      erakutsiAlergenoIkurrak(edukiontzia);
      iragaziPlaterak();
    }
  } catch (e) {
    // Isilik utzi
  }
}
