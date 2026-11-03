# DevSecOps eta CI/CD Metodologia Agenteekin: "Aldaketa Txikiak, Egiaztapen Jarraituak"

Dokumentu honek zehazten du **nola lan egin behar duten agenteek eta garatzaileek** proiektuan, kodearen kalitatea, segurtasuna eta fidagarritasuna bermatzeko, *"Desagertuko dira programatzaileak?"* metodologiari jarraituz.

---

## 1. CI/CD Zikloa Agenteekin (Urratsak)

```mermaid
flowchart TD
    subgraph URRATSAK["Garapen Ziklo Errepikakorra"]
        Z["1. ZEHAZTU (Garatzailea / Pertsonak)<br/>Helburua, mugak eta onarpen-probak zehaztu (Gherkin/Markdown)"]
        E["2. ERAIKI (IA Agentea)<br/>Kode aldaketa txikia eta unit-probak prestatu"]
        CI["3. CI: ETENGABEKO INTEGRAZIOA<br/>Build automatikoa + Proba automatikoak + SonarQube kode-analisia"]
        P["4. BALIOZKOTU (Garatzailea / Pertsonak)<br/>Agenteak egindako lana eta proben emaitzak berrikusi"]
        CD["5. CD: ETENGABEKO ENTREGA<br/>Bertsio egiaztatua prestatu eta ingurunera hedatu"]
    end

    Z --> E
    E --> CI
    CI -->|Huts egin du? (Fail)| BUKLEA["Agenteak errorea aztertu, kodea zuzendu eta berriz probatu"]
    BUKLEA --> E
    CI -->|Gainditu du (Pass)| P
    P -->|Onartua| CD
    P -->|Zuzendu beharra| Z
```

### Printzipioak
1. **Zehaztu:** Agendari eskatu zeregin txiki eta kontrolagarriak (adib. "Sortu lehengaien kontsumo transakzioa").
2. **Eraiki & Probatu:** Agenteak funtzionalitatea idatzi eta unitate-probak exekutatzen ditu.
3. **CI Feedback Begizta:** Testak huts eginez gero, agenteak errore-mezuak irakurri eta kodea automatikoki zuzentzen du berriz konpilatuz.
4. **DevSecOps:** Sekretuak `.env`-n gorde, SQL parametrizatuak soilik erabili eta mendekotasunak eguneratuta mantendu.
