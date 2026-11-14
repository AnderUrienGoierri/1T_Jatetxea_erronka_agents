/**
 * JATETXEA - Iritziak eta Balorazioak (iritziak.js)
 */

/**
 * Balorazio orokorren estatistikak kargatzen ditu (v_balorazio_orokorrak).
 */
async function kargatuEstatistikak() {
  const kaxa = document.getElementById("balorazio-estatistikak");
  if (!kaxa) return;

  try {
    const res = await fetch(`${API_BASE}/iritziak/estatistikak`);
    const json = await res.json();
    if (json.status === "success" && json.data) {
      const d = json.data;
      const guztira = d.guztiraIritziak || 0;
      const batez = d.batazBestekoBalorazioa
        ? d.batazBestekoBalorazioa.toFixed(1)
        : "0.0";

      const calcPct = (kant) =>
        guztira > 0 ? ((kant / guztira) * 100).toFixed(0) : 0;

      kaxa.innerHTML = `
                <div class="batez-bestekoa">${batez}</div>
                <div class="izar-bistaratzailea">Puntuazioa: ${batez} / 5.0</div>
                <div class="guztira-label">${guztira} bezeroren balorazioak</div>
                <div class="izarren-banaketa">
                    ${marraztuIzarBanaketaLerroa(5, d.bostIzarrak, calcPct(d.bostIzarrak))}
                    ${marraztuIzarBanaketaLerroa(4, d.lauIzarrak, calcPct(d.lauIzarrak))}
                    ${marraztuIzarBanaketaLerroa(3, d.hiruIzarrak, calcPct(d.hiruIzarrak))}
                    ${marraztuIzarBanaketaLerroa(2, d.biIzarrak, calcPct(d.biIzarrak))}
                    ${marraztuIzarBanaketaLerroa(1, d.izarBat, calcPct(d.izarBat))}
                </div>
            `;
    }
  } catch (err) {
    kaxa.innerHTML = "<p>Ezin izan dira estatistikak kargatu.</p>";
  }
}

/**
 * Izarren banaketa portzentualaren barra marrazten du.
 */
function marraztuIzarBanaketaLerroa(izarrak, kopurua, portzentajea) {
  return `
        <div class="izar-lerroa">
            <span>${izarrak} izar</span>
            <div class="barra-atzeko">
                <div class="barra-aurreko" style="width: ${portzentajea}%;"></div>
            </div>
            <span>${kopurua || 0}</span>
        </div>
    `;
}

/**
 * Azken iritzien zerrenda kargatzen du.
 */
async function kargatuIritziak() {
  const edukiontzia = document.getElementById("iritziak-zerrenda");
  if (!edukiontzia) return;

  try {
    const res = await fetch(`${API_BASE}/iritziak`);
    const json = await res.json();
    if (json.status === "success" && json.dataList) {
      if (json.dataList.length === 0) {
        edukiontzia.innerHTML =
          "<p>Oraindik ez dago iritzirik. Izan zaitez lehena zure balorazioa uzten!</p>";
        return;
      }

      edukiontzia.innerHTML = json.dataList
        .map(
          (ir) => `
                <div class="iritzi-txartela">
                    <div class="iritzi-goiburua">
                        <span class="iritzi-egilea">${eskapatuHtml(ir.bezeroIzena)}</span>
                        <span class="iritzi-data">${eskapatuHtml(ir.sortzeData)}</span>
                    </div>
                    <div style="font-weight: 600; font-size: 0.85rem; color: #b45309; margin-bottom: 0.3rem;">
                        Balorazioa: ${ir.balorazioa} / 5 izar
                    </div>
                    <p class="iritzi-testua">${eskapatuHtml(ir.testua || "Iruzkinik gabe.")}</p>
                </div>
            `,
        )
        .join("");
    }
  } catch (err) {
    edukiontzia.innerHTML = "<p>Ezin izan dira iritziak kargatu.</p>";
  }
}

/**
 * Bezeroaren balorazio berria bidaltzen du.
 */
async function bidaliIritzia(e) {
  e.preventDefault();

  const alertKaxa = document.getElementById("iritzi-alerta");
  ezkutatuAlerta(alertKaxa);

  const emaila = document.getElementById("iritzi-emaila").value.trim();
  const izena = document.getElementById("iritzi-izena").value.trim();
  const balorazioa = parseInt(
    document.getElementById("iritzi-balorazioa").value,
    10,
  );
  const testua = document.getElementById("iritzi-testua").value.trim();

  if (!emaila || isNaN(balorazioa)) {
    erakutsiAlerta(
      alertKaxa,
      "Emaila eta balorazioa derrigorrezkoak dira.",
      "errorea",
    );
    return;
  }

  try {
    const res = await fetch(`${API_BASE}/iritziak`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        bezeroEmaila: emaila,
        bezeroIzena: izena || "Bezeroa",
        balorazioa: balorazioa,
        testua: testua || null,
      }),
    });

    const json = await res.json();
    if (res.status === 201) {
      erakutsiAlerta(
        alertKaxa,
        "Mila esker zure iritzia partekatzeagatik! Zure balorazioa erregistratu da.",
        "arrakasta",
      );
      document.getElementById("iritzi-form").reset();
      kargatuIritziak();
      kargatuEstatistikak();
    } else {
      erakutsiAlerta(
        alertKaxa,
        `Errorea iritzia gordetzean: ${json.message || "Saiatu berriro."}`,
        "errorea",
      );
    }
  } catch (err) {
    erakutsiAlerta(alertKaxa, `Konexio-errorea: ${err.message}`, "errorea");
  }
}
