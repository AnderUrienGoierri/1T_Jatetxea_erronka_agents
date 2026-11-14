# 1T Jatetxea - IT Kontrol Zentrala (Admin Hub)
## Goierri Eskola - Zibersegurtasuna eta Sare Sistemak

**1T Jatetxea IT Kontrol Zentrala** jatetxearen sistema osoa monitorizatu, kontrolatu eta kudeatzeko garatutako kontrol-panela da. Portatilean, mahaigainean zein WiFi bidez konektatutako mugikor edo tabletetan erabil daiteke.

---

## 1. Taldea eta Baimenak (IT Administradoreak)

Aplikazioak ekintza bakoitza nork egin duen erregistratzen du (auditoretza log-a):

| Erabiltzailea | Administratzailea | Rola eta Erantzukizuna |
| :--- | :--- | :--- |
| **AnderU** | Ander Urien | IT Lead / FullStack Arkitektoa eta Koordinatzailea |
| **Julen** | Julen | Sare eta Suebaki Ingeniaria (OPNsense / Cisco Switch) |
| **Iker** | Iker | Sistemak eta Birtualizazioa (VirtualBox 3 Makinak) |
| **Aratz** | Aratz | Zibersegurtasuna eta Proiektuaren Ebaluazioa |

---

## 2. Gaitasun Nagusiak

1. **Docker Edukiontzien Kontrol Osoa:**
   - Edukiontzi guztien egoera denbora errealean (MySQL, API, PHP Web, Chat, KDS TV, Odoo ERP, Postgres).
   - Ekintzak: Altxatu Guztia (`up -d`), Gelditu Guztia (`down`), Zerbitzu jakin bat berrabiarazi (`restart`), eta azken 50 lerroetako log-ak ikusi.
2. **VirtualBox 3 Makina Birtualen Kudeaketa:**
   - Erronka-OPNsense, Erronka-WinServer2025 eta Erronka-Ubuntu egoera detektatu.
   - Klik bakarreko abiarazte ixila (`headless`) eta itzaltze segurua (`ACPI powerbutton`).
3. **Sareko Portuen Osasuna (TCP Health Check):**
   - Ataka kritikoen egiaztapen automatikoa 5 segundoro:
     * Port 5000: ASP.NET Core API
     * Port 3308: MySQL Datu-basea
     * Port 8085: Web PHP ataria
     * Port 3001: Barne Txat zerbitzaria
     * Port 8086: Sukaldeko KDS TV
     * Port 8069: Odoo 17 ERP
     * Port 8443: OPNsense WebGUI
     * Port 3391: Windows Server RDP
     * Port 2230/2231/2232: Linux eta Windows SSH
4. **Sare Zentralita Dinamikoa (WiFi switch-env):**
   - Kokapenaren araberako profilak aldatu: `localhost`, `eskola`, `tolosako_etxea`, `etxean`, `vbox`.
5. **Laborategi Fisikoaren Integrazioa:**
   - Cisco Catalyst 2960 portuen esleipenak eta VLAN taula (10, 20, 30, 99).
   - Ping tresna interaktiboa laborategiko IPak probatzeko.
6. **Auditoretza Erregistroa:**
   - Orduz orduko ekintzen segimendua (nork abiarazi duen Docker, nork aldatu duen sarea, etab.).

---

## 3. Nola Abiarazi eta Erabili

### 3.1. Ordenagailuan (Desktop)
1. Egin klik bikoitza [hasi_admin_zentrala.bat](file:///C:/Dev/GoierriEskola/2_maila/1T_Jatxetxea_Erronka/admin_IT_zentrala/hasi_admin_zentrala.bat) fitxategian (edo PowerShell-etik: `.\hasi_admin_zentrala.ps1`).
2. Zerbitzaria martxan jarriko da eta automatikoki irekiko du panela zure nabigatzailean:
   ```
   http://localhost:5050
   ```

### 3.2. Mugikorrean edo Tabletan (WiFi)
1. Ziurtatu mugikorra/tableta eta eramangarria WiFi sare berean daudela (adib. Goierri Eskolako laborategiko WiFian).
2. Eramangarriaren IP lokala aurkitu (`ipconfig` bidez, adib. `192.168.10.100` edo `192.168.1.50`).
3. Ireki mugikorreko edo tabletako nabigatzailea eta sartu:
   ```
   http://[ZURE-PORTATILAREN-IP-A]:5050
   ```
4. Panela guztiz sentikorra (responsive) da eta mugikorreko zein tabletako pantailara egokitzen da.

---

## 4. API Endpoints (Garatzaileentzat)

Flutter edo beste edozein aplikaziotatik integratzeko eskuragarri dauden REST zerbitzuak:

- `GET /api/docker/status` - Docker edukiontzien egoera JSON formatuan.
- `POST /api/docker/up` - Docker edukiontziak martxan jarri.
- `POST /api/docker/down` - Docker edukiontziak gelditu.
- `POST /api/docker/restart/{service}` - Zerbitzu zehatz bat berrabiarazi.
- `GET /api/docker/logs/{service}` - Zerbitzu baten azken log-ak.
- `GET /api/vbox/status` - VirtualBox makinen egoera.
- `POST /api/vbox/start-all` - 3 makinak headless abiarazi.
- `POST /api/vbox/stop-all` - 3 makinak ACPI bidez gelditu.
- `GET /api/health/ports` - TCP ataken osasun matrizea.
- `GET /api/env/list` - Eskuragarri dauden sare profilak.
- `POST /api/env/switch/{envId}` - Sare profila aldatu.
- `POST /api/lab/ping` - Sareko IP bati ping egin (`{"host":"192.168.10.10"}`).
- `GET /api/audit` - Auditoretza erregistro osoa.
