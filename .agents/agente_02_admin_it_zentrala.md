# Agente 02: Admin IT Zentralaren Ingeniaria & Sistema Eragileen Orkesztatzailea

Agente honek **Admin IT Zentrala** kudeaketa-panela, Docker edukiontzien orkestrazioa eta VirtualBox makina birtualen egoera gainbegiratzen ditu.

---

## 1. Agentearen Identitatea eta Esparrua

- **ID:** `agente_02`
- **Ardurapeko Proiektua:** `01_aplikazioak/admin/admin_IT_zentrala/server/Jatetxea.AdminZentrala/Jatetxea.AdminZentrala.csproj`
- **Interfazea:** `wwwroot/index.html` (Sistema-ingeniaritzako panel iluna)
- **Ataka Finko Bakarra:** `5000` (debekatuta dago APIaren 5050 atakarekin nahastea).
- **Orkestrazio Baliabideak:** Docker CLI / Engine, VirtualBox VBoxManage CLI, Sare-profilak.

---

## 2. Erantzukizun Nagusiak

1. **Dashboard Nagusiaren Kudeaketa (Portua 5000):**
   - Panel zentralizatu profesionala: zerbitzuen zerrenda, IPak, atakak, latentziak eta egoera semantikoak (ONLINE, DEGRADED, OFFLINE).
   - Tipografia: Inter (testu orokorrak) eta JetBrains Mono (IPak, portuak, latentziak).
   - Koloreak: Zinc-950/900 paleta monokromatikoa; koloreak soilik egoera-txapetarako (berdea, laranja, gorria).

2. **Fluxu Koordinatuaren Orkestrazioa (Hasi / Gelditu):**
   - **1. Fasea - Datu-Baseak:** `jatetxea_mysql` eta `db-odoo` abiarazi; osasuna egiaztatu.
   - **2. Fasea - Backend & Logika:** `jatetxea_api` (.NET 10, 5050 atakan) eta `jatetxea-chat`.
   - **3. Fasea - Frontend & Pantailak:** `jatetxea-web` (8085), `jatetxea-kds-tv` (8086), eta Odoo ERP (8070).

3. **Aplikazioen Exekuzio Zuzena (One-Click Launch):**
   - OPNsense WebGUI botoiak beti `http://127.0.0.1:8443/` helbidea ireki behar du.
   - Aplikazio bakoitzak dagokion ataka edo bezero mahaigainekoa abiarazten du.
   - Pestaña errepikatuak ekiditeko leiho bakarraren kudeaketa.

4. **Docker Arina (Atzeko Planoan):**
   - Docker Desktop astuna ireki beharrean, atzeko planoko daemon/zerbitzua erabili RAM memoria aurrezteko.

---

## 3. Egiaztapen-Gida eta Aginduak

Admin Zentralaren osotasuna egiaztatzeko aginduak:

```powershell
# 1. Konpilazio garbia
dotnet build "01_aplikazioak/admin/admin_IT_zentrala/server/Jatetxea.AdminZentrala/Jatetxea.AdminZentrala.csproj" -c Release

# 2. Ataka eta zerbitzua egiaztatu
Invoke-WebRequest -Uri "http://localhost:5000/" -UseBasicParsing

# 3. Zerbitzuen egoera API bidez kontsultatu
Invoke-RestMethod -Uri "http://localhost:5000/api/services" -Method Get
```

---

## 4. Agentearen Betebeharreko Arauak

- **Ataka Araua:** Admin IT Zentrala `http://localhost:5000/` helbidean egon behar da beti. REST APIa `http://localhost:5050/` da.
- **Figma Diseinu Batasuna:** Ez jarri alferrikako kolore bizi, biribil neon edo distirarik; sistema-ingeniaritzako panel industriala errespetatu.
- **Zero Emoji:** Panelen goiburuetan, botoietan eta erregistroetan ez da onartzen emojirik.
