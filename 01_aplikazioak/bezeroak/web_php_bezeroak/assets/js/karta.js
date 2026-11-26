/**
 * JATETXEA - Karta eta Plateren Kudeaketa (karta.js)
 * PHP zerbitzariak sortutako elementuekin lan egiten du, bezeroan iragazketa azkarra eskainiz.
 */

let unekoKategoria = "Guztiak";
let unekoEdariAzpikategoria = "Guztiak";

const EDARI_KATEGORIAK = new Set([
  "Edaria",
  "Edariak",
  "Urak",
  "Freskagarriak",
  "Garagardoak",
  "Sagardoak",
  "Ardoak",
  "Kafeak",
  "Infusioak",
  "Likoreak",
  "Konbinatuak",
  "Koktailak",
]);

/**
 * Kategoria-iragazkien eta edarien azpikategorien gertaerak konfiguratzen ditu.
 */
function konfiguratuIragazkiak() {
  const botoiak = document.querySelectorAll(".filter-btn");
  const subBar = document.getElementById("edari-azpikategoriak");
  const subBotoiak = document.querySelectorAll(".subfilter-btn");

  if (!botoiak || botoiak.length === 0) return;

  botoiak.forEach((btn) => {
    btn.addEventListener("click", () => {
      botoiak.forEach((b) => b.classList.remove("active"));
      btn.classList.add("active");
      unekoKategoria = btn.getAttribute("data-kategoria") || "Guztiak";

      if (subBar) {
        if (unekoKategoria === "Edaria") {
          subBar.style.display = "flex";
        } else {
          subBar.style.display = "none";
          unekoEdariAzpikategoria = "Guztiak";
          subBotoiak.forEach((sb) =>
            sb.classList.toggle("active", sb.getAttribute("data-edari-kat") === "Guztiak")
          );
        }
      }
      iragaziPlaterak();
    });
  });

  subBotoiak.forEach((sbtn) => {
    sbtn.addEventListener("click", () => {
      subBotoiak.forEach((sb) => sb.classList.remove("active"));
      sbtn.classList.add("active");
      unekoEdariAzpikategoria = sbtn.getAttribute("data-edari-kat") || "Guztiak";
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
    const kategoria = card.getAttribute("data-kategoria") || "";
    let erakutsi = false;

    if (unekoKategoria === "Guztiak") {
      erakutsi = true;
    } else if (unekoKategoria === "Edaria") {
      if (unekoEdariAzpikategoria === "Guztiak") {
        erakutsi = EDARI_KATEGORIAK.has(kategoria);
      } else {
        erakutsi = kategoria === unekoEdariAzpikategoria;
      }
    } else {
      erakutsi = kategoria === unekoKategoria;
    }

    card.style.display = erakutsi ? "" : "none";
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
    return;
  }

  try {
    const res = await fetch(`${API_BASE}/platerak`, {
      headers: { "X-App-Id": "web_php" },
    });
    const json = await res.json();
    if (json.status === "success" && json.dataList && json.dataList.length > 0) {
      const katFallback = {
        Sarrerakoa: "sarrerakoa.jpg",
        Lehena: "lehena.jpg",
        Bigarrena: "bigarrena.jpg",
        Postrea: "postrea.jpg",
        Edaria: "ardoak.jpg",
        Urak: "urak.jpg",
        Freskagarriak: "freskagarriak.jpg",
        Garagardoak: "garagardoak.jpg",
        Sagardoak: "sagardoak.jpg",
        Ardoak: "ardoak.jpg",
        Kafeak: "kafeak.jpg",
        Infusioak: "infusioak.jpg",
        Likoreak: "likoreak.jpg",
        Konbinatuak: "konbinatuak.jpg",
        Koktailak: "koktailak.jpg",
      };
      edukiontzia.innerHTML = json.dataList.map((p) => {
        const prezio = Number(p.prezioa ?? p.salmentaPrezioa ?? 0).toFixed(2).replace(".", ",");
        const fb = `assets/img/kategoriak/${katFallback[p.kategoria] || "bigarrena.jpg"}`;
        return `
        <div class="plater-card" data-kategoria="${p.kategoria}">
          <div class="plater-img-wrap">
            <img src="assets/img/platerak/platera_${p.id}.jpg"
                 alt="${p.izena}"
                 class="plater-img"
                 loading="lazy"
                 decoding="async"
                 onerror="this.onerror=null;this.src='${fb}';">
          </div>
          <div class="plater-body">
            <div class="plater-header">
              <h3 class="plater-izena">${p.izena}</h3>
              <span class="plater-prezioa">${prezio} &euro;</span>
            </div>
            <p class="plater-deskribapena">${p.deskribapena || ''}</p>
            <div class="plater-meta">
              <span class="kategoria-tag">${p.kategoria}</span>
              <span class="prestatze-denbora">${p.prestatzeDenboraMin ?? 15} min</span>
            </div>
          </div>
        </div>
      `;
      }).join("");
      iragaziPlaterak();
    }
  } catch (e) {
    // Isilik utzi
  }
}
