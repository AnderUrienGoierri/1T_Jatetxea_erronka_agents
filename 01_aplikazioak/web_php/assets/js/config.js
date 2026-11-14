/**
 * JATETXEA - Konfigurazioa eta Laguntzaile Orokorrak (config.js)
 */

const rawBase = window.API_BASE_URL;
// host.docker.internal baletor, automatikoki localhost-era bihurtu nabigatzailean
const sanitizedBase = (rawBase || "").replace("host.docker.internal", "localhost");
const API_BASE =
  sanitizedBase && !sanitizedBase.includes("<?php")
    ? sanitizedBase
    : "http://localhost:5000/api/v1";

/**
 * REST APIaren konexio-egoera egiaztatzen du erro-bidean.
 */
async function egiaztatuApiEgoera() {
  const badge = document.getElementById("api-status");
  if (!badge) return;

  try {
    const rootUrl = API_BASE.replace("/api/v1", "");
    const res = await fetch(rootUrl, { mode: "cors" });
    if (res.ok) {
      badge.className = "api-status-badge online";
      badge.textContent = "API Linean";
    } else {
      throw new Error("API erantzun ez-zuzena");
    }
  } catch (err) {
    badge.className = "api-status-badge offline";
    badge.textContent = "API Deskonektatuta";
  }
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
