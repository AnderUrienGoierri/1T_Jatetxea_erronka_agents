# 1T_Jatetxea_erronka_agents

Jatetxearen Kudeaketa Sistema: 3 Mailako Arkitektura eta IA Agente Kolaboratzaileekin Garatua (Goierri Eskola).

[![CI - Build & Quality Gate](https://github.com/AnderUrienGoierri/1T_Jatetxea_erronka_agents/actions/workflows/ci.yml/badge.svg)](https://github.com/AnderUrienGoierri/1T_Jatetxea_erronka_agents/actions/workflows/ci.yml)

---

## Deskribapena

Proiektu honek jatetxe baten kudeaketa sistema osoa barneratzen du, 3 mailako arkitektura baten bidez antolatuta:

1. **Datu-Basea (MySQL 8.0 InnoDB Dockerren):**
   * Taulak, errezetak/eskandalloak, CHECK murrizketak eta errenkada-mailako blokeo pesimistak (`SELECT ... FOR UPDATE`).
2. **REST APIa (ASP.NET Core .NET 10):**
   * Transakzio pesimistak lehengaien stocka automatikoki deskontatzeko, mahaien aforoa eta egoerak kudeatzeko eta 409 Conflict gatazkak kontrolatzeko.
3. **Aurre-Maila (3 Bezero Independente):**
   * `web_php/`: Karta, plateren erabilgarritasuna, mahai-erreserbak eta bezeroen iritziak (PHP / HTML5 / JS Fetch).
   * `desktop_csharp/`: Zerbitzarien TPV salmenta-puntua eta mahaien kudeaketa tablet diseinuan (.NET 10 WinForms).
   * `desktop_javafx/`: Sukaldeko KDS, Barrako KDS eta biltegiko stock kontrola (Java 17/21 JavaFX MVC).

---

## Arkitektura eta Moduluak

```
                                  +-------------------+
                                  |   Web Bezeroa     |
                                  |    (web_php/)     |
                                  +---------+---------+
                                            |
                                            v
+-----------------------+         +---------+---------+         +-----------------------+
|  C# Desktop (TPV)     | ------> |    REST APIa      | <------ |  JavaFX Desktop (KDS) |
|   (desktop_csharp/)   |  HTTP   |  (backend_api/)   |  HTTP   |   (desktop_javafx/)   |
+-----------------------+         +---------+---------+         +-----------------------+
                                            |
                                            v (TCP 3308)
                                  +---------+---------+
                                  |  MySQL 8.0 InnoDB |
                                  |  (Docker Compose) |
                                  +-------------------+
```

### Moduluen Deskribapena

* **`backend_api/`**: C# ASP.NET Core (.NET 10) REST APIa. Datu-basearekin komunikazio segurua bermatzen duen ataka bakarra.
* **`desktop_csharp/`**: Zerbitzarien tablet POS interfazea:
  * PIN bidezko saio-hasiera zerbitzarien zerrendarekin.
  * Mahaien mapa guneen arabera (Jangela Nagusia, Terraza, Barra, VIP, Guztiak) eta egoera-koloreekin (Librea, Okupatuta, Eskaera bidean, Garbitzen).
  * Mahaiaren xehetasun osoak: kontu banatuak (`1. Kontua`, `2. Kontua`), plateren egoerak (Zai, Martxan, Eginda), "Sukaldera Bidali" botoia, plater berriak gehitzea eta kobratzea.
  * Komanden produktuen hautatzailea eta karrito interaktiboa.
* **`desktop_javafx/`**: Sukaldeko eta biltegiko kudeaketa:
  * **Sukaldea (KDS):** Janari plateren eskaera aktiboak (Sarrerakoak, Lehenak, Bigarrenak, Postreak) eta prestaketa-egoerak.
  * **Barra (KDS):** Edariak eta kafeak bakarrik kudeatzeko pantaila bereizia.
  * **Biltegia (Stocka):** Lehengaien stocka, gutxieneko stock alertak eta albaranen bidezko sarrerak erregistratzea.
* **`web_php/`**: Jatetxeko webgune publikoa karta interaktiboarekin, stockaren araberako erabilgarritasunarekin eta erreserba-sistemarekin.

---

## Proiektua Abiarazteko Gida

### 1. Datu-Basea (Docker)

```bash
docker compose up -d
```
* Zerbitzaria: `localhost:3308`
* Datu-basea: `jatetxea_db`
* Kredentzialak: `root` / `root`

### 2. REST APIa (.NET 10)

```bash
cd backend_api
dotnet run
```
* Entzuten: `http://localhost:5000`
* Swagger / Dokumentazioa: `http://localhost:5000/swagger`

### 3. C# Desktop TPV (.NET 10)

```bash
cd desktop_csharp
dotnet run
```

### 4. JavaFX Desktop KDS eta Stocka (Maven)

```bash
cd desktop_javafx
./mvnw javafx:run
```

### 5. Web Frontend-a (PHP)

```bash
cd web_php
php -S localhost:8000
```
* Nabigatzailean ireki: `http://localhost:8000`

---

## Dokumentazio Gehigarria

* [Proiektuaren Egoera eta Testuinguru Osoa](PROIEKTUAREN_EGOERA_ETA_TESTUINGURUA.md)
* [Arkitektura Gida Osoa](JATETXEA_AGENTEEN_ARKITEKTURA_GIDA.md)
* [Datu-Basearen Eskema (SQL)](espezifikazioak/datu_basea_schema.sql)
* [REST API Kontratua](espezifikazioak/rest_api_kontratua.md)
* [Bezeroen Interfazeak](espezifikazioak/bezero_interfazeak.md)
* [DevSecOps eta CI/CD Metodologia](espezifikazioak/cicd_devsecops_fluxua.md)
* [Agenteen Definizioak](agenteak/agente_definizioak.md)

---

## Garapen Arau Nagusiak

1. **Emotikonorik Ez:** Proiektu osoan (README, kodea, iruzkinak, commit mezuak eta txat erantzunak) ez da emotikonorik onartzen.
2. **Arkitektura 3-Mailakoa:** Bezeroek ez dute inoiz datu-basearekin zuzenean hitz egiten; komunikazio guztia REST APIaren bidez doa.
3. **Prezioak Backend-etik:** Prezio guztiak datu-basetik bakarrik eskuratzen dira; bezeroek ezin dute preziorik aldatu.
4. **Datu Errealak:** Bezeroek APIarekin soilik funtzionatzen dute; konexiorik ezean egoera argi jakinarazten da.
