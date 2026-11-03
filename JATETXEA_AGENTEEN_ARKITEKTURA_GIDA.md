# Jatetxearen Kudeaketa Sistema: 3 Mailako Arkitektura eta IA Agenteekin Garatzeko Gida Integrala (v2.0)

Dokumentu honek zehazten du nola diseinatu, egituratu eta garatu **Jatetxe baten Kudeaketa Sistema Osoa** 3 mailako arkitekturan oinarrituta, **Lehengaien Stock Kontrol automatikoa (Errezetak/Eskandalloak)** eta **IA Agente Kolaboratzaileak** erabiliz.

Metodologia honek bat egiten du **"Desagertuko dira programatzaileak?"** printzipioekin (*"Aldaketa txikiak, egiaztapen jarraituak"*) eta Antigravity plataformako agente-egiturarekin.

---

## 1. 3 Mailako Sistemaren Arkitektura

```mermaid
graph TD
    subgraph AURRE_MAILA["1. AURRE-MAILA: ERABILTZAILEAK (3 Interfaze)"]
        WebClient["Web-App (Bezeroak)<br/>• HTML5, CSS3, Modern JS (Port: 5500)<br/>• Karta, Mahai-erreserbak, Agortutako platerak desgaitu"]
        CSharpClient["C# Mahaigainekoa (Zerbitzariak/Kudeaketa)<br/>• .NET 10 WinForms / WPF<br/>• TPV / Salmenta-puntua, Soft/Hard Delete, --cli / --gui"]
        JavaFXClient["JavaFX Mahaigainekoa (Sukaldea / KDS & Stock)<br/>• JavaFX 21+ MVC Arkitektura<br/>• Sukaldeko aginduak denbora errealean<br/>• Erosketa Zerrenda eta Stock alertak"]
    end

    subgraph TARTEKO_MAILA["2. TARTEKO MAILA: REST API (C# .NET)"]
        API["API_jatetxea_core (ASP.NET Core Web API)<br/>• Port: 5000 / 5001 (HTTPS)"]
        Controllers["Kontroladore Nagusiak:<br/>• ErreserbakKontroladorea<br/>• MahaiakKontroladorea<br/>• PlaterakKontroladorea (Karta)<br/>• LehengaiakKontroladorea (Stock eta Alertak)<br/>• EskaerakKontroladorea<br/>• FakturakKontroladorea"]
        SecurityTrans["Transakzio & Segurtasun Mekanismoak:<br/>• MySqlTransaction (IsolationLevel.ReadCommitted)<br/>• SELECT ... FOR UPDATE (Blokeo pesimista lehengaietan)<br/>• Errezeta bidezko proportzioen deskontu automatikoa<br/>• Blokeo ordena: Mahaia -> Eskaera -> Lehengaia -> Platera<br/>• Salbuespenak: 1062 (UNIQUE), 1213 (Deadlock)<br/>• HTTP: 200, 201, 400, 404, 409 Conflict (Stock falta)"]
        API --> Controllers
        Controllers --> SecurityTrans
    end

    subgraph DATU_BASEA["3. DATU-BASE MAILA: MYSQL INNODB"]
        MySQL["MySQL 8.0 InnoDB (Port: 3306)<br/>utf8mb4_0900_ai_ci"]
        Tables["Taula Nagusiak:<br/>• lehengaiak (id, izena, unitatea, uneko_stocka, gutxieneko_stocka)<br/>• platerak (id, izena, kategoria, prezioa, alergenoak)<br/>• plater_osagaiak (errezeta / proportzioak - N:M)<br/>• stock_mugimenduak (auditoria / historikoa)<br/>• mahaiak, erreserbak, eskaerak, eskaera_lerroak, langileak, bezeroak"]
        Views["Ikuspegi Automatikoak (Views):<br/>• v_erosi_beharreko_lehengaiak (Erosketa-zerrenda)<br/>• v_plateren_stock_ahalmena (Erabilgarritasuna)"]
        MySQL --> Tables
        Tables --> Views
    end

    WebClient -->|"JSON / REST (HTTPS)"| API
    CSharpClient -->|"JSON / REST (HTTPS)"| API
    JavaFXClient -->|"JSON / REST (HTTPS)"| API
    SecurityTrans -->|"TCP / SQL (Pooled Connections)"| MySQL
```

---

## 2. Datu-Basearen Egitura Osoa (`espezifikazioak/datu_basea_schema.sql`)

Datu-basearen eskema berrituak funtzionalitate hauek bermatzen ditu:
1. **`lehengaiak`**: Biltegiko osagai guztiak beren neurri-unitatearekin (`kg`, `g`, `l`, `ml`, `unitatea`).
2. **`plater_osagaiak`**: Errezeta bakoitzaren proportzioak (`Txuleta Errea` = `1.0 kg Okela` + `0.05 l Olio`).
3. **`stock_mugimenduak`**: Deskontu guztien eta albaranen sarreren auditoria.
4. **`v_erosi_beharreko_lehengaiak`**: Stocka gutxienekoaren azpitik duten lehengaiak automatikoki zerrendatzeko.
5. **`v_plateren_stock_ahalmena`**: Biltegiko osagaien arabera plater bakoitzetik zenbat anoa egin daitezkeen unean-unean kalkulatzeko.

---

## 3. Martxan Jartzea Docker Compose bidez

Proiektuaren erroan `docker-compose.yml` fitxategia sortu da. Agindu bakar batekin MySQL 8.0 datu-basea eta eskema osoa automatikoki martxan jartzeko:

```bash
docker compose up -d
```
