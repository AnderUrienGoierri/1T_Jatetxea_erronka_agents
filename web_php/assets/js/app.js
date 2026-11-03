/**
 * JATETXEA - Bezeroaren Web Interfazearen Logika (JavaScript)
 * Arkitektura: REST API bidezko komunikazioa (C# ASP.NET Core)
 * Arau profesionala: emotikonorik gabe
 */

const rawBase = window.API_BASE_URL;
const API_BASE = (rawBase && !rawBase.includes('<?php')) ? rawBase : 'http://localhost:5000/api/v1';

// Barne egoera
let platerakCache = [];
let erabilgarritasunaCache = {};
let unekoKategoria = 'Guztiak';

document.addEventListener('DOMContentLoaded', () => {
    egiaztatuApiEgoera();
    hasieratuDataAukeratzailea();
    kargatuKarta();
    kargatuMahaiak();
    kargatuIritziak();
    kargatuEstatistikak();
    konfiguratuInprimakiak();
    konfiguratuIragazkiak();
});

// =============================================================================
// 1. APIaren KONEXIO EGOERA EGIAZTATZEA
// =============================================================================
async function egiaztatuApiEgoera() {
    const badge = document.getElementById('api-status');
    if (!badge) return;

    try {
        const rootUrl = API_BASE.replace('/api/v1', '');
        const res = await fetch(rootUrl);
        if (res.ok) {
            badge.className = 'api-status-badge online';
            badge.textContent = 'REST API konektatuta';
        } else {
            throw new Error('Errorea');
        }
    } catch {
        badge.className = 'api-status-badge offline';
        badge.textContent = 'REST API deskonektatuta (localhost:5000)';
    }
}

// =============================================================================
// 2. KARTA ETA PLATEREN ERABILGARRITASUNA
// =============================================================================
async function kargatuKarta() {
    const edukiontzia = document.getElementById('platerak-zerrenda');
    if (!edukiontzia) return;

    edukiontzia.innerHTML = '<p class="kargatzen-testua">Platerak kargatzen...</p>';

    try {
        // 1. Plater guztiak berreskuratu
        const resPlaterak = await fetch(`${API_BASE}/platerak`);
        const jsonPlaterak = await resPlaterak.json();

        // 2. Erabilgarritasuna berreskuratu (zenbat anoa prestatu daitezkeen)
        const resAhalmena = await fetch(`${API_BASE}/platerak/erabilgarritasuna`);
        const jsonAhalmena = await resAhalmena.json();

        if (jsonPlaterak.status === 'success' && jsonPlaterak.dataList) {
            platerakCache = jsonPlaterak.dataList;

            erabilgarritasunaCache = {};
            if (jsonAhalmena.status === 'success' && jsonAhalmena.dataList) {
                jsonAhalmena.dataList.forEach(item => {
                    erabilgarritasunaCache[item.plateraId] = item.eginDaitezkeenPlaterak;
                });
            }

            bistaratuPlaterak();
        } else {
            edukiontzia.innerHTML = '<p>Ez da platerik aurkitu.</p>';
        }
    } catch (err) {
        edukiontzia.innerHTML = `<p class="errore-testua">Ezin izan da karta kargatu: ${err.message}. Egiaztatu REST APIa martxan dagoen.</p>`;
    }
}

function bistaratuPlaterak() {
    const edukiontzia = document.getElementById('platerak-zerrenda');
    if (!edukiontzia) return;

    const iragaziak = unekoKategoria === 'Guztiak'
        ? platerakCache
        : platerakCache.filter(p => p.kategoria === unekoKategoria);

    if (iragaziak.length === 0) {
        edukiontzia.innerHTML = '<p>Ez dago kategoria honetako platerik.</p>';
        return;
    }

    edukiontzia.innerHTML = iragaziak.map(platera => {
        const anoak = erabilgarritasunaCache[platera.id] ?? 0;
        const agortutaDago = anoak <= 0;
        const cardClass = agortutaDago ? 'plater-card agortuta' : 'plater-card';
        const stockBadge = agortutaDago
            ? '<span class="stock-tag agortuta">Agortuta</span>'
            : `<span class="stock-tag eskuragarri">${anoak} anoa eskuragarri</span>`;

        const alergenoakHtml = platera.alergenoak
            ? `<span class="alergenoak-tag">Alergenoak: ${eskapatuHtml(platera.alergenoak)}</span>`
            : '';

        return `
            <div class="${cardClass}" data-id="${platera.id}">
                <div>
                    <div class="plater-header">
                        <h4 class="plater-izena">${eskapatuHtml(platera.izena)}</h4>
                        <span class="plater-prezioa">${platera.prezioa.toFixed(2)} &euro;</span>
                    </div>
                    <p class="plater-deskribapena">${eskapatuHtml(platera.deskribapena || 'Deskribapenik ez.')}</p>
                </div>
                <div class="plater-meta">
                    <span class="kategoria-tag">${eskapatuHtml(platera.kategoria)}</span>
                    ${alergenoakHtml}
                    ${stockBadge}
                </div>
            </div>
        `;
    }).join('');
}

function konfiguratuIragazkiak() {
    const botoiak = document.querySelectorAll('.filter-btn');
    botoiak.forEach(btn => {
        btn.addEventListener('click', () => {
            botoiak.forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            unekoKategoria = btn.getAttribute('data-kategoria') || 'Guztiak';
            bistaratuPlaterak();
        });
    });
}

// =============================================================================
// 3. MAHAIEN ZERRENDA ETA ERRESERBAK
// =============================================================================
async function kargatuMahaiak() {
    const select = document.getElementById('erreserba-mahaia');
    if (!select) return;

    try {
        const res = await fetch(`${API_BASE}/mahaiak`);
        const json = await res.json();
        if (json.status === 'success' && json.dataList) {
            select.innerHTML = '<option value="">-- Aukeratu mahaia --</option>' +
                json.dataList.map(m => `
                    <option value="${m.id}" data-edukiera="${m.edukiera}">
                        ${m.zenbakia}. Mahaia (${m.edukiera} pertsona - ${m.kokapena})
                    </option>
                `).join('');
        }
    } catch (err) {
        select.innerHTML = '<option value="">Errorea mahaiak kargatzean</option>';
    }
}

function hasieratuDataAukeratzailea() {
    const dataInput = document.getElementById('erreserba-data');
    if (dataInput) {
        // Gutxienez gaurko data hautatu behar da
        const gaur = new Date().toISOString().split('T')[0];
        dataInput.min = gaur;
        if (!dataInput.value) {
            dataInput.value = gaur;
        }
    }
}

function konfiguratuInprimakiak() {
    // Erreserba inprimakia
    const erreserbaForm = document.getElementById('erreserba-form');
    if (erreserbaForm) {
        erreserbaForm.addEventListener('submit', bidaliErreserba);
    }

    // Iritzi inprimakia
    const iritziForm = document.getElementById('iritzi-form');
    if (iritziForm) {
        iritziForm.addEventListener('submit', bidaliIritzia);
    }
}

async function bidaliErreserba(e) {
    e.preventDefault();

    const alertKaxa = document.getElementById('erreserba-alerta');
    ezkutatuAlerta(alertKaxa);

    const mahaiaId = parseInt(document.getElementById('erreserba-mahaia').value, 10);
    const data = document.getElementById('erreserba-data').value;
    const txanda = document.getElementById('erreserba-txanda').value;
    const pertsonaKopurua = parseInt(document.getElementById('erreserba-pertsonak').value, 10);
    const izena = document.getElementById('bezero-izena').value.trim();
    const abizena = document.getElementById('bezero-abizena').value.trim();
    const emaila = document.getElementById('bezero-emaila').value.trim();
    const telefonoa = document.getElementById('bezero-telefonoa').value.trim();
    const oharrak = document.getElementById('erreserba-oharrak').value.trim();

    if (!mahaiaId || !data || !txanda || !emaila || !izena) {
        erakutsiAlerta(alertKaxa, 'Mesedez, bete derrigorrezko eremu guztiak.', 'errorea');
        return;
    }

    const eskaeraGorputza = {
        mahaiaId,
        data,
        txanda,
        pertsonaKopurua,
        bezeroIzena: izena,
        bezeroAbizena: abizena || '-',
        bezeroEmaila: emaila,
        bezeroTelefonoa: telefonoa || '-',
        oharrak: oharrak || null
    };

    const submitBtn = document.getElementById('btn-erreserbatu');
    if (submitBtn) {
        submitBtn.disabled = true;
        submitBtn.textContent = 'Erreserba prozesatzen...';
    }

    try {
        const res = await fetch(`${API_BASE}/erreserbak`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(eskaeraGorputza)
        });

        const json = await res.json();

        if (res.status === 201) {
            // Arrakasta (201 Created)
            const d = json.data;
            erakutsiAlerta(
                alertKaxa,
                `Erreserba arrakastaz baieztatua! Erreserba ID: #${d.id} | Data: ${d.data} (${d.txanda}) | Mahaia: ${d.mahaiZenbakia}. mahaia (${d.kokapena}). E-posta bidezko baieztapena bidali da: ${d.bezeroEmaila}`,
                'arrakasta'
            );
            document.getElementById('erreserba-form').reset();
            hasieratuDataAukeratzailea();
        } else if (res.status === 409) {
            // Gatazka (409 Conflict): Mahai bikoiztua edo hutsunea okupatuta
            erakutsiAlerta(
                alertKaxa,
                `Erreserba Gatazka (409 Conflict): ${json.message || 'Mahaia jada erreserbatuta dago data eta txanda horretan.'} Mesedez, hautatu beste mahai bat edo beste txanda bat.`,
                'abisua'
            );
        } else {
            // Bestelako akatsa (400 Bad Request, etab.)
            erakutsiAlerta(
                alertKaxa,
                `Errorea erreserba egitean: ${json.message || 'Egiaztatu datuak.'}`,
                'errorea'
            );
        }
    } catch (err) {
        erakutsiAlerta(
            alertKaxa,
            `Konexio-errorea: Ezin izan da zerbitzariarekin komunikatu (${err.message}).`,
            'errorea'
        );
    } finally {
        if (submitBtn) {
            submitBtn.disabled = false;
            submitBtn.textContent = 'Erreserba Baieztatu';
        }
    }
}

// =============================================================================
// 4. IRITZIAK ETA BALORAZIOAK
// =============================================================================
async function kargatuEstatistikak() {
    const kaxa = document.getElementById('balorazio-estatistikak');
    if (!kaxa) return;

    try {
        const res = await fetch(`${API_BASE}/iritziak/estatistikak`);
        const json = await res.json();
        if (json.status === 'success' && json.data) {
            const d = json.data;
            const guztira = d.guztiraIritziak || 0;
            const batez = d.batazBestekoBalorazioa ? d.batazBestekoBalorazioa.toFixed(1) : '0.0';

            const calcPct = (kant) => guztira > 0 ? ((kant / guztira) * 100).toFixed(0) : 0;

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
        kaxa.innerHTML = '<p>Ezin izan dira estatistikak kargatu.</p>';
    }
}

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

async function kargatuIritziak() {
    const edukiontzia = document.getElementById('iritziak-zerrenda');
    if (!edukiontzia) return;

    try {
        const res = await fetch(`${API_BASE}/iritziak`);
        const json = await res.json();
        if (json.status === 'success' && json.dataList) {
            if (json.dataList.length === 0) {
                edukiontzia.innerHTML = '<p>Oraindik ez dago iritzirik. Izan zaitez lehena zure balorazioa uzten!</p>';
                return;
            }

            edukiontzia.innerHTML = json.dataList.map(ir => `
                <div class="iritzi-txartela">
                    <div class="iritzi-goiburua">
                        <span class="iritzi-egilea">${eskapatuHtml(ir.bezeroIzena)}</span>
                        <span class="iritzi-data">${eskapatuHtml(ir.sortzeData)}</span>
                    </div>
                    <div style="font-weight: 600; font-size: 0.85rem; color: #b45309; margin-bottom: 0.3rem;">
                        Balorazioa: ${ir.balorazioa} / 5 izar
                    </div>
                    <p class="iritzi-testua">${eskapatuHtml(ir.testua || 'Iruzkinik gabe.')}</p>
                </div>
            `).join('');
        }
    } catch (err) {
        edukiontzia.innerHTML = '<p>Ezin izan dira iritziak kargatu.</p>';
    }
}

async function bidaliIritzia(e) {
    e.preventDefault();

    const alertKaxa = document.getElementById('iritzi-alerta');
    ezkutatuAlerta(alertKaxa);

    const emaila = document.getElementById('iritzi-emaila').value.trim();
    const izena = document.getElementById('iritzi-izena').value.trim();
    const balorazioa = parseInt(document.getElementById('iritzi-balorazioa').value, 10);
    const testua = document.getElementById('iritzi-testua').value.trim();

    if (!emaila || isNaN(balorazioa)) {
        erakutsiAlerta(alertKaxa, 'Emaila eta balorazioa derrigorrezkoak dira.', 'errorea');
        return;
    }

    try {
        const res = await fetch(`${API_BASE}/iritziak`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                bezeroEmaila: emaila,
                bezeroIzena: izena || 'Bezeroa',
                balorazioa: balorazioa,
                testua: testua || null
            })
        });

        const json = await res.json();
        if (res.status === 201) {
            erakutsiAlerta(alertKaxa, 'Mila esker zure iritzia partekatzeagatik! Zure balorazioa erregistratu da.', 'arrakasta');
            document.getElementById('iritzi-form').reset();
            kargatuIritziak();
            kargatuEstatistikak();
        } else {
            erakutsiAlerta(alertKaxa, `Errorea iritzia gordetzean: ${json.message || 'Saiatu berriro.'}`, 'errorea');
        }
    } catch (err) {
        erakutsiAlerta(alertKaxa, `Konexio-errorea: ${err.message}`, 'errorea');
    }
}

// =============================================================================
// LAGUNTZAILEAK (ALERTAK ETA SEGURTASUNA)
// =============================================================================
function erakutsiAlerta(elementua, mezua, mota) {
    if (!elementua) return;
    elementua.textContent = mezua;
    elementua.className = `alert-kaxa show alert-${mota}`;
}

function ezkutatuAlerta(elementua) {
    if (!elementua) return;
    elementua.className = 'alert-kaxa';
    elementua.textContent = '';
}

function eskapatuHtml(str) {
    if (!str) return '';
    return str
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

