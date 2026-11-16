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

  try {
    const healthUrl = API_BASE.replace("/api/v1", "/health");
    const res = await fetch(healthUrl, {
      mode: "cors",
      headers: { "X-App-Id": "web_php" },
    });

    if (res.ok) {
      const data = await res.json().catch(() => null);
      const isDbConnected =
        data &&
        (data.database === "Connected" ||
          data.datuBaseaKonektatuta === true ||
          data.status === "Healthy");

      if (isDbConnected) {
        badge.className = "api-status-badge online";
        badge.innerHTML = '<span class="status-dot"></span> API &amp; DB Linean';
        badge.title = "REST API eta MySQL Datu-basea konektatuta daude (Portua 5050)";
      } else {
        badge.className = "api-status-badge warning";
        badge.innerHTML =
          '<span class="status-dot" style="background:#eab308;"></span> API Linean | DB Deskonektatuta';
        badge.title = "REST API linean dago baina MySQL datu-basearekin arazoa dago";
      }
    } else {
      throw new Error("API erantzun ez-zuzena: " + res.status);
    }
  } catch (err) {
    badge.className = "api-status-badge offline";
    badge.innerHTML = '<span class="status-dot"></span> API Deskonektatuta';
    badge.title = "Ezin izan da REST APIarekin konektatu (Portua 5050)";
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
