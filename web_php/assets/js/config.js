/**
 * JATETXEA - Konfigurazioa eta Laguntzaile Orokorrak (config.js)
 */

const rawBase = window.API_BASE_URL;
const API_BASE =
  rawBase && !rawBase.includes("<?php")
    ? rawBase
    : "http://localhost:5000/api/v1";

/**
 * REST APIaren konexio-egoera egiaztatzen du erro-bidean.
 */
async function egiaztatuApiEgoera() {
  const badge = document.getElementById("api-status");
  if (!badge) return;

  try {
    const rootUrl = API_BASE.replace("/api/v1", "");
    const res = await fetch(rootUrl);
    if (res.ok) {
      badge.className = "api-status-badge online";
      badge.textContent = "REST API konektatuta";
    } else {
      throw new Error("Errorea");
    }
  } catch {
    badge.className = "api-status-badge offline";
    badge.textContent = "REST API deskonektatuta (localhost:5000)";
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
 * XSS ahultasunak saihesteko HTML karaktere bereziak ihes egiten ditu.
 */
function eskapatuHtml(str) {
  if (!str) return "";
  return str
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#039;");
}
