# 1T_Jatetxea_erronka_agents

Jatetxearen Kudeaketa Sistema: 3 Mailako Arkitektura eta IA Agente Kolaboratzaileekin Garatua (Goierri Eskola).

[![CI - Build & Quality Gate](https://github.com/AnderUrienGoierri/1T_Jatetxea_erronka_agents/actions/workflows/ci.yml/badge.svg)](https://github.com/AnderUrienGoierri/1T_Jatetxea_erronka_agents/actions/workflows/ci.yml)

---

## 📖 Deskribapena

Proiektu honek jatetxe baten kudeaketa sistema osoa barneratzen du:
* **Datu-Basea (MySQL 8.0 InnoDB):** Taulak, errezetak/eskandalloak, CHECK murrizketak eta errenkada-mailako blokeo pesimistak (`SELECT ... FOR UPDATE`).
* **REST APIa (ASP.NET Core .NET 10):** Transakzio pesimistak lehengaien stocka deskontatzeko eta 409 Conflict gatazken kudeaketa.
* **Aurre-Maila (3 Bezero):**
  * `web_php/`: Karta eta mahai-erreserben web aplikazioa (PHP / HTML5 / JS).
  * `desktop_csharp/`: Zerbitzarien TPV salmenta-puntua eta administrazioa (.NET 10 WinForms).
  * `desktop_javafx/`: Sukaldeko KDS eta biltegiko stock kontrola (Java 17/21 JavaFX MVC).

---

## 🛠️ Arkitektura eta Gidak

* [Arkitektura Gida Osoa](JATETXEA_AGENTEEN_ARKITEKTURA_GIDA.md)
* [Datu-Basearen Eskema (SQL)](espezifikazioak/datu_basea_schema.sql)
* [REST API Kontratua](espezifikazioak/rest_api_kontratua.md)
* [Bezeroen Interfazeak](espezifikazioak/bezero_interfazeak.md)
* [DevSecOps eta CI/CD Metodologia](espezifikazioak/cicd_devsecops_fluxua.md)
* [Agenteen Definizioak](agenteak/agente_definizioak.md)

---

## 🚀 Azkar Hasteko (Datu-basea)

```bash
docker compose up -d
```
* Portua: `3308`
* Datu-basea: `jatetxea_db` (root / root)
