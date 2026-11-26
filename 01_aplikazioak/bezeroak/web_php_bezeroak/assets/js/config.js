/**
 * JATETXEA - Konfigurazioa eta Laguntzaile Orokorrak (config.js)
 */

const clientHost = window.location.hostname || "localhost";
const rawBase = window.API_BASE_URL || `http://${clientHost}:5050/api/v1`;

// Docker barruko helbideak (jatetxea-api, host.docker.internal) eta 5000 ataka zaharra
// automatikoki nabigatzailearen ostalariarekin eta 5050 ataka zuzenarekin ordezkatzen ditugu.
let sanitizedBase = (rawBase || "")
  .replace(/https?:\/\/jatetxea-api(:\d+)?/, `http://${clientHost}:5050`)
  .replace(/https?:\/\/host\.docker\.internal(:\d+)?/, `http://${clientHost}:5050`);

if (sanitizedBase.includes(":5000")) {
  sanitizedBase = sanitizedBase.replace(":5000", ":5050");
}

const API_BASE =
  sanitizedBase && !sanitizedBase.includes("<?php")
    ? sanitizedBase
    : `http://${clientHost}:5050/api/v1`;

window.API_BASE_URL = API_BASE;

/**
 * REST APIaren eta MySQL Datu-basearen egoera egiaztatzen du /health bidean.
 */
async function egiaztatuApiEgoera() {
  const badge = document.getElementById("api-status");
  if (!badge) return;

  function ezarriEgoera(egoera, mezua) {
    badge.className = `api-status-badge ${egoera}`;
    badge.title = mezua;
    badge.setAttribute("aria-label", mezua);
  }

  let res;
  try {
    const healthUrl = API_BASE.replace("/api/v1", "/health");
    res = await fetch(healthUrl, {
      mode: "cors",
      headers: { "X-App-Id": "web_php" },
    });
  } catch (err) {
    ezarriEgoera("offline", "APIa ez dago eskuragarri; datu-basearen egoera ezezaguna da");
    return;
  }

  if (!res.ok) {
    ezarriEgoera("warning", `APIaren egoera-egiaztapenak huts egin du (HTTP ${res.status})`);
    return;
  }

  let data;
  try {
    data = await res.json();
  } catch (err) {
    ezarriEgoera("warning", "APIaren egoera-erantzuna ez da baliozkoa");
    return;
  }

  const dbOnline = data && (data.database != null
    ? data.database === "Connected"
    : data.datuBaseaKonektatuta != null
      ? data.datuBaseaKonektatuta === true
      : data.status === "Healthy");
  if (dbOnline) {
    ezarriEgoera("online", "API eta datu-basea konektatuta");
  } else {
    ezarriEgoera("warning", "APIak erantzuten du, baina datu-basearen konexioak edo egoera-egiaztapenak huts egin du");
  }
}

// 20 segundoro automatikoki API eta DB egoera freskatu
if (typeof window !== "undefined") {
  setInterval(egiaztatuApiEgoera, 20000);
}

/**
 * Alerta mezuak bistaratzen ditu UI-an.
 */
function erakutsiAlerta(elementua, mezua, mota) {
  if (!elementua) return;
  elementua.textContent = mezua;
  elementua.className = `alert-kaxa show alert-${mota}`;
}

/**
 * Alerta mezua ezkutatzen du.
 */
function ezkutatuAlerta(elementua) {
  if (!elementua) return;
  elementua.className = "alert-kaxa";
  elementua.textContent = "";
}

/**
 * HTML karaktere bereziak segurtasunez eskapatzen ditu XSS saihesteko.
 */
function eskapatuHtml(testua) {
  if (!testua) return "";
  const div = document.createElement("div");
  div.textContent = testua;
  return div.innerHTML;
}
