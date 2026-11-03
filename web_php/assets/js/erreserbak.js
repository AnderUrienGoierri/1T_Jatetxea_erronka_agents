/**
 * JATETXEA - Mahaiak eta Erreserben Kudeaketa (erreserbak.js)
 */

/**
 * Mahai libreen zerrenda kargatzen du hautatzailean.
 */
async function kargatuMahaiak() {
  const select = document.getElementById("erreserba-mahaia");
  if (!select) return;

  try {
    const res = await fetch(`${API_BASE}/mahaiak`);
    const json = await res.json();
    if (json.status === "success" && json.dataList) {
      select.innerHTML =
        '<option value="">-- Aukeratu mahaia --</option>' +
        json.dataList
          .map(
            (m) => `
                    <option value="${m.id}" data-edukiera="${m.edukiera}">
                        ${m.zenbakia}. Mahaia (${m.edukiera} pertsona - ${m.kokapena})
                    </option>
                `,
          )
          .join("");
    }
  } catch (err) {
    select.innerHTML = '<option value="">Errorea mahaiak kargatzean</option>';
  }
}

/**
 * Erreserben dataren hautatzailean gutxieneko data gaurkoan finkatzen du.
 */
function hasieratuDataAukeratzailea() {
  const dataInput = document.getElementById("erreserba-data");
  if (dataInput) {
    const gaur = new Date().toISOString().split("T")[0];
    dataInput.min = gaur;
    if (!dataInput.value) {
      dataInput.value = gaur;
    }
  }
}

/**
 * Erreserba berria bidaltzen du eta 409 Conflict gatazkak atsegin kudeatzen ditu.
 */
async function bidaliErreserba(e) {
  e.preventDefault();

  const alertKaxa = document.getElementById("erreserba-alerta");
  ezkutatuAlerta(alertKaxa);

  const mahaiaId = parseInt(
    document.getElementById("erreserba-mahaia").value,
    10,
  );
  const data = document.getElementById("erreserba-data").value;
  const txanda = document.getElementById("erreserba-txanda").value;
  const pertsonaKopurua = parseInt(
    document.getElementById("erreserba-pertsonak").value,
    10,
  );
  const izena = document.getElementById("bezero-izena").value.trim();
  const abizena = document.getElementById("bezero-abizena").value.trim();
  const emaila = document.getElementById("bezero-emaila").value.trim();
  const telefonoa = document.getElementById("bezero-telefonoa").value.trim();
  const oharrak = document.getElementById("erreserba-oharrak").value.trim();

  if (!mahaiaId || !data || !txanda || !emaila || !izena) {
    erakutsiAlerta(
      alertKaxa,
      "Mesedez, bete derrigorrezko eremu guztiak.",
      "errorea",
    );
    return;
  }

  const eskaeraGorputza = {
    mahaiaId,
    data,
    txanda,
    pertsonaKopurua,
    bezeroIzena: izena,
    bezeroAbizena: abizena || "-",
    bezeroEmaila: emaila,
    bezeroTelefonoa: telefonoa || "-",
    oharrak: oharrak || null,
  };

  const submitBtn = document.getElementById("btn-erreserbatu");
  if (submitBtn) {
    submitBtn.disabled = true;
    submitBtn.textContent = "Erreserba prozesatzen...";
  }

  try {
    const res = await fetch(`${API_BASE}/erreserbak`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(eskaeraGorputza),
    });

    const json = await res.json();

    if (res.status === 201) {
      // Arrakasta (201 Created)
      const d = json.data;
      erakutsiAlerta(
        alertKaxa,
        `Erreserba arrakastaz baieztatua! Erreserba ID: #${d.id} | Data: ${d.data} (${d.txanda}) | Mahaia: ${d.mahaiZenbakia}. mahaia (${d.kokapena}). E-posta bidezko baieztapena bidali da: ${d.bezeroEmaila}`,
        "arrakasta",
      );
      document.getElementById("erreserba-form").reset();
      hasieratuDataAukeratzailea();
    } else if (res.status === 409) {
      // Gatazka (409 Conflict): Mahai bikoiztua edo hutsunea okupatuta
      erakutsiAlerta(
        alertKaxa,
        `Erreserba Gatazka (409 Conflict): ${json.message || "Mahaia jada erreserbatuta dago data eta txanda horretan."} Mesedez, hautatu beste mahai bat edo beste txanda bat.`,
        "abisua",
      );
    } else {
      // Bestelako akatsa (400 Bad Request, etab.)
      erakutsiAlerta(
        alertKaxa,
        `Errorea erreserba egitean: ${json.message || "Egiaztatu datuak."}`,
        "errorea",
      );
    }
  } catch (err) {
    erakutsiAlerta(
      alertKaxa,
      `Konexio-errorea: Ezin izan da zerbitzariarekin komunikatu (${err.message}).`,
      "errorea",
    );
  } finally {
    if (submitBtn) {
      submitBtn.disabled = false;
      submitBtn.textContent = "Erreserba Baieztatu";
    }
  }
}
