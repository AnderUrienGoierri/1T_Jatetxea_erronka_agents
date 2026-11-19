#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Sortu Garapen Faseetako Markdown Fitxategiak (78 fitxategi = 6 fase x 13 aplikazio/modulu)
Microsoft Teams Planner planifikazio integralean oinarritua.
"""

import os
import re

BASE_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
DOCS_DIR = os.path.join(BASE_DIR, '04_dokumentazioa', 'Garapen_faseak')
PLANNER_FILE = os.path.join(BASE_DIR, 'planner', 'PLANIFIKAZIOA_PLANNER.md')

PHASE_INFO = {
    1: {
        "folder": "1-Analisia",
        "izena": "1. FASEA: ANALISIA",
        "epe_orokorra": "2026/11/03 - 2026/11/08",
        "deskribapena": "Eskakizunen bilketa sakona, erabilera-kasuen definizioa, plataformen bideragarritasuna eta oinarrizko arkitekturaren zehaztapena."
    },
    2: {
        "folder": "2-Diseinua",
        "izena": "2. FASEA: DISEINUA",
        "epe_orokorra": "2026/11/09 - 2026/11/14",
        "deskribapena": "Datu-ereduen definizio zehatza, API kontratuen eta JSON eskemen egituraketa, Figma bidezko UI/UX diseinuak eta sare-topologien modelaketa."
    },
    3: {
        "folder": "4-Garapena",
        "izena": "3. FASEA: GARAPENA",
        "epe_orokorra": "2026/11/15 - 2026/11/24",
        "deskribapena": "Iturburu-kodearen garapen osoa, zerbitzarien programazioa, negozio-logika, interfaze grafikoak, scriptak eta datu-baseen inplementazioa."
    },
    4: {
        "folder": "3-Frogak",
        "izena": "4. FASEA: FROGAK",
        "epe_orokorra": "2026/11/25 - 2026/11/28",
        "deskribapena": "Proba unitarioak, integrazio-probak sistemen artean, erreserben talka eta karga-probak, eta zibersegurtasuneko ikuskapenak."
    },
    5: {
        "folder": "5-Martxan_jartzea",
        "izena": "5. FASEA: MARTXAN JARTZEA",
        "epe_orokorra": "2026/11/29 - 2026/12/01",
        "deskribapena": "Ekoizpen-inguruneko hedapena (Deployment), Docker edukiontzien abiaraztea, ekipo eta tablet fisikoetan instalazioa eta benetako kaxa-irekiera."
    },
    6: {
        "folder": "6-Mantentzea",
        "izena": "6. FASEA: MANTENTZEA",
        "epe_orokorra": "2026/12/02 - 2026/12/05",
        "deskribapena": "Sistemaren eguneroko jarraipena, segurtasun-kopiak (backupak), log-en kudeaketa, erabiltzaile-dokumentazioa eta akatsen prebentzioa."
    }
}

MODULE_INFO = {
    1: {
        "filename": "01_datu_basea.md",
        "izena": "Datu-Basea (MySQL & PostgreSQL / Odoo)",
        "azpititulua": "Datu erlazionalen eskema nagusia eta integrazio korporatiboa",
        "deskribapena": "Jatetxearen datu-eredu erlazionala (MySQL 8.0 InnoDB), ACID transakzioak, lehengaiak, stock kontrola, plateren eskandalloak eta Odoo ERP (PostgreSQL) HR integrazioa.",
        "path": "db/datu_basea_schema.sql",
        "teknologia": "MySQL 8.0 (InnoDB), PostgreSQL 16 (Odoo 17), Docker"
    },
    2: {
        "filename": "02_rest_api.md",
        "izena": "REST API Backend-a (ASP.NET Core C#)",
        "azpititulua": "Negozio-logika eta aplikazio guztien komunikazio zentralizatua",
        "deskribapena": "Aplikazio guztien komunikazio-zentroa. CRUD eragiketak, negozio-logika (erreserben talka-kontrola, stockaren deskontu pesimista, ordainketen zibersegurtasuna eta fitxaketak).",
        "path": "01_aplikazioak/admin/backend_rest_api_csharp",
        "teknologia": "C# ASP.NET Core (.NET 10), Dapper / ADO.NET, JWT, BCrypt"
    },
    3: {
        "filename": "03_app_java_admin.md",
        "izena": "APP - Java Admin - Kudeatzailea",
        "azpititulua": "Jatetxearen kudeaketa globalerako mahaigaineko panela",
        "deskribapena": "Jatetxeko arduradun eta IT administratzaileentzako mahaigaineko aplikazioa. Langileen altak/bajak, txanden esleipena, zerbitzuen monitorizazioa eta txosten globalak.",
        "path": "01_aplikazioak/admin/admin_desktop_java",
        "teknologia": "JavaFX 21, OpenJDK 21, Maven, HttpClient / REST"
    },
    4: {
        "filename": "04_app_tpv_csharp.md",
        "izena": "APP - TPV - Zerbitzariak (C# WinForms)",
        "azpititulua": "Barrako Salmenta Puntuaren Terminal Nagusia (POS)",
        "deskribapena": "Barrako Salmenta Puntuaren Terminala (TPV/POS). Mahaien mapa interaktiboa, komanden sarrera, kontuen kobraketa (eskudirua, txartela, Bizum), tiket fiskalen inprimaketa eta langileen txanda-fitxaketa.",
        "path": "01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp",
        "teknologia": "C# WinForms (.NET 10), ESC/POS Termikoa, REST API Bezeroa"
    },
    5: {
        "filename": "05_app_android_zerbitzariak.md",
        "izena": "APP - Android Zerbitzariak (Komandera Mugikorra / Tableta)",
        "azpititulua": "Areto eta terrazako komanden bilketa azkarra",
        "deskribapena": "Zerbitzariek aretoan eta terrazan erabiltzeko komandera mugikorra eta tableta bertsioa. Mahaiak kudeatzea, komandak zuzenean sukaldera bidaltzea, plateren ohar bereziak gehitzea eta fitxatzea.",
        "path": "01_aplikazioak/zerbitzari_langileak/zerbitzaria_komandera_android",
        "teknologia": "Kotlin, Jetpack Compose, Retrofit 2, Material 3"
    },
    6: {
        "filename": "06_app_android_tv_sukaldea.md",
        "izena": "APP - Android TV - Sukaldariak (KDS)",
        "azpititulua": "Sukaldeko pantaila handirako Kitchen Display System",
        "deskribapena": "Sukaldeko telebista eta pantaila handietarako Kitchen Display System (KDS). Urruneko agintearekin (D-Pad) kontrolatua, komandak denbora errealean erakustea eta prestatze-egoeren kudeaketa.",
        "path": "01_aplikazioak/sukalde_langileak/sukaldea_kds_android_tv",
        "teknologia": "Android TV, Jetpack Compose for TV / Leanback, REST Polling"
    },
    7: {
        "filename": "07_app_java_sukaldea.md",
        "izena": "APP Java - Sukaldea (KDS Mahaigainekoa)",
        "azpititulua": "Sukaldeko lan-estazioko KDS aplikazioa",
        "deskribapena": "Sukaldeko pantaila taktiletarako edo ordenagailuetarako KDS mahaigaineko bertsioa. Komanden platerak kontrolatzea (Zai -> Martxan -> Eginda) eta sukaldarien PIN fitxaketa.",
        "path": "01_aplikazioak/sukalde_langileak/desktop_sukaldea_kds",
        "teknologia": "JavaFX 21, Maven, REST API Client"
    },
    8: {
        "filename": "08_app_java_biltegia.md",
        "izena": "APP Java - Biltegia (Stock eta Inbentarioa)",
        "azpititulua": "Biltegiko stockaren jarraipena eta alertak",
        "deskribapena": "Biltegiko arduradunaren aplikazioa. Lehengaien uneko stocka, hornitzaileen albaranen sarrerak, inbentario-doikuntzak eta gutxieneko stock alertak gorriz bistaratzea.",
        "path": "01_aplikazioak/biltegi_langileak/desktop_biltegia_stock",
        "teknologia": "JavaFX 21, Maven, REST API Client"
    },
    9: {
        "filename": "09_app_chat_zerbitzaria.md",
        "izena": "APP - Zerbitzaria / Chat Internoa",
        "azpititulua": "Sailen arteko denbora errealeko komunikazio arina",
        "deskribapena": "Jatetxeko langileen arteko berehalako barne-komunikazio sistema. Areto, sukalde eta barraren arteko gelak (rooms), premiazko abisuak eta denbora errealeko mezularitza.",
        "path": "01_aplikazioak/admin/admin_IT_zentrala/server",
        "teknologia": "Node.js, Socket.IO, WebSockets"
    },
    10: {
        "filename": "10_app_web_bezeroak.md",
        "izena": "APP - Web (Apache/PHP) - Bezeroak",
        "azpititulua": "Jatetxearen atari korporatiboa eta online erreserbak",
        "deskribapena": "Jatetxearen kanpoko webgune publikoa. Karta interaktiboa, alergenoen iragazkia, online mahai-erreserben formularioa txandaka eta iritzien atala.",
        "path": "01_aplikazioak/bezeroak/web_php_bezeroak",
        "teknologia": "PHP 8.2, Apache 2.4, HTML5, Tailwind CSS, JavaScript"
    },
    11: {
        "filename": "11_app_android_bezeroak.md",
        "izena": "APP - Android Bezeroak (Flutter Mugikorra)",
        "azpititulua": "Mahaiko auto-eskaera eta auto-ordainketa mugikorretik",
        "deskribapena": "Jatetxera datozen bezeroentzako mugikorreko aplikazioa. Mahaiko QR kodea eskaneatuz auto-eskaera egiteko, eramateko eskatzeko eta ordainketa birtuala burutzeko.",
        "path": "01_aplikazioak/bezeroak/mobile_bezeroa_flutter",
        "teknologia": "Flutter 3.x, Dart, Mobile Camera (QR), REST Client"
    },
    12: {
        "filename": "12_azpiegitura_digitala.md",
        "izena": "AZPIEGITURA DIGITALA (Makina Birtualak & Docker)",
        "azpititulua": "Sare birtualak, suebakia eta zerbitzarien orkestrazioa",
        "deskribapena": "Ingurune birtualizatua: OPNsense suebakia (sareen segmentazioa, VLANak, DHCP), Windows Server 2025, Ubuntu Server eta zerbitzuak biltzen dituzten Docker edukiontziak.",
        "path": "02_azpiegitura_birtuala",
        "teknologia": "VirtualBox, OPNsense, Docker & Docker Compose, Linux/Windows Server"
    },
    13: {
        "filename": "13_azpiegitura_fisikoa.md",
        "izena": "AZPIEGITURA FISIKOA (Laborategia, Sareak eta Ekipoak)",
        "azpititulua": "Rack 19\", Cisco bideratzea, kableatua eta Wi-Fi sareak",
        "deskribapena": "Jatetxearen hardware eta sare-azpiegitura erreala: 19\" Rack-a, Cisco switch eta routerrak, kableatu egituratua (TIA-568-B), Access Point-ak eta TPV ekipo fisikoak.",
        "path": "03_laborategi_fisikoa",
        "teknologia": "Cisco IOS (Switch/Router), 19\" Rack, Patch Panel RJ45, Wi-Fi AP"
    }
}

def parse_planner():
    with open(PLANNER_FILE, encoding='utf-8') as f:
        text = f.read()

    phases_raw = re.findall(r'## (\d+)\. FASEA: ([^\n]+)\n(.*?)(?=\n## \d+\. FASEA:|\Z)', text, re.DOTALL)
    planner_data = {}

    for pnum_str, pname_str, pcontent in phases_raw:
        pnum = int(pnum_str)
        tasks = re.findall(r'### (\d+\.\d+)\. ([^\n]+)\n(.*?)(?=\n### \d+\.\d+\.|\Z)', pcontent, re.DOTALL)
        phase_tasks = {}
        for tnum_str, ttitle, tbody in tasks:
            mod_idx = int(tnum_str.split('.')[1])
            meta_match = re.search(r'- \*\*Epea:\*\* ([^|]+) \| \*\*Lehentasuna:\*\* ([^|]+) \| \*\*Rola:\*\* ([^\n]+)', tbody)
            meta = meta_match.groups() if meta_match else ('Zehaztu gabe', 'Ertaina', 'Garatzailea')
            checklist = re.findall(r'- \[ \] ([^\n]+)', tbody)
            phase_tasks[mod_idx] = {
                "tnum": tnum_str,
                "ttitle": ttitle.strip(),
                "epea": meta[0].strip(),
                "lehentasuna": meta[1].strip(),
                "rola": meta[2].strip(),
                "checklist": checklist
            }
        planner_data[pnum] = phase_tasks
    return planner_data

def generate_markdown(pnum, mod_idx, task_data):
    pinfo = PHASE_INFO[pnum]
    minfo = MODULE_INFO[mod_idx]

    md = f"""# {minfo['izena']} · {pinfo['izena']}

> **Atala:** {minfo['azpititulua']}  
> **Fasearen Epea Orokorra:** {pinfo['epe_orokorra']}  
> **Moduluaren Epe Zehatza:** {task_data['epea']}  
> **Arduraduna / Rola:** {task_data['rola']}  
> **Lehentasuna (Planner):** {task_data['lehentasuna']}  

---

## 1. Testuingurua eta Helburua

**{pinfo['izena']}** fasean, **{minfo['izena']}** moduluaren helburu nagusia honakoa da:
{pinfo['deskribapena']}

Modulu honek sistemaren arkitekturan betetzen duen eginkizuna:
> {minfo['deskribapena']}

- **Kodearen kokapena (Path):** [{minfo['path']}](file:///{os.path.join(BASE_DIR, minfo['path']).replace(os.sep, '/')})
- **Erabilitako Teknologia Nagusiak:** `{minfo['teknologia']}`

---

## 2. Betebeharrak eta Eginkizunak (Planner Karta: `{task_data['tnum']}`)

Fase honetan aplikazio honek burutu beharreko zeregin nagusien zerrenda:

"""
    for item in task_data['checklist']:
        md += f"- [ ] **{item}**\n"

    md += f"""
---

## 3. Zereginen Zehaztapen Operatiboa

Jarraian, zeregin bakoitzaren garapen-gida eta jarraibide teknikoak xehatzen dira:

"""
    for i, item in enumerate(task_data['checklist'], 1):
        md += f"### 3.{i}. {item}\n"
        md += f"- **Helburua:** Zeregin honen bidez, modulua fase honetan behar den egoera funtzionalera eramatea.\n"
        md += f"- **Jarduera teknikoak:**\n"
        md += f"  - Ezarritako arkitektura-arauak eta kodetze-estandarrak zorrotz jarraitzea.\n"
        md += f"  - Proiektuko beste moduluekin (REST API, Datu-basea, Sareak) integrazio garbia bermatzea.\n"
        md += f"  - Frogak eta balidazio automatikoak gainditzea.\n\n"

    md += f"""---

## 4. Loturak eta Menpekotasunak

- **Sarrerako Menpekotasunak:** Fase honek aurreko faseko emaitzak eta azpiegituraren egoera behar ditu martxan egoteko.
- **Irteerako Ekarpenak:** Modulu honek sortutako emaitzak hurrengo faseetarako eta beste bezero-aplikazioentzat oinarri dira.
- **Lotutako Modulu Nagusiak:**
  - `01_datu_basea.md` (Datuak atzitzeko eta gordetzeko).
  - `02_rest_api.md` (Negozio-logika eta endpoint-ak kontsumitzeko).

---

## 5. Fase Amaierako Entregagaiak (Deliverables)

Fase hau bukatutzat emateko entregatu behar diren elementuak:
1. Moduluari dagokion iturburu-kodea edo konfigurazio-fitxategi eguneratuak.
2. Frogak eta balidazio-txostenak (errore gabe exekutatzen dela egiaztatuta).
3. Teams Planner-eko dagokion kartaren (`{task_data['tnum']}`) atazak osatuta markatzea.

---

## 6. Onarpen Irizpideak (Definition of Done - DoD)

- [ ] Checklist-eko zeregin guztiak burututa eta berrikusita daude.
- [ ] Kodeak edo konfigurazioak ez du akats larririk ezta segurtasun-zuluskarik.
- [ ] Dokumentazio teknikoa eta iruzkinak behar bezala osatu dira.
- [ ] Proba-ingurunean funtzionamendu zuzena baieztatu da.
"""
    return md

def main():
    print("Planner datuak kargatzen...")
    planner_data = parse_planner()
    
    total_files = 0
    for pnum, pinfo in PHASE_INFO.items():
        target_dir = os.path.join(DOCS_DIR, pinfo['folder'])
        os.makedirs(target_dir, exist_ok=True)
        print(f"Sortzen fasea: {pinfo['folder']} ({pinfo['izena']})...")
        
        tasks = planner_data.get(pnum, {})
        for mod_idx, minfo in MODULE_INFO.items():
            task_data = tasks.get(mod_idx)
            if not task_data:
                print(f"  [KONTUZ] Ez da aurkitu daturik modulu honentzat: P{pnum} M{mod_idx}")
                continue
            
            filepath = os.path.join(target_dir, minfo['filename'])
            content = generate_markdown(pnum, mod_idx, task_data)
            with open(filepath, 'w', encoding='utf-8') as f:
                f.write(content)
            total_files += 1

    print(f"\nEginda! Guztira {total_files} fitxategi sortu dira arrakastaz '{DOCS_DIR}' barruan.")

if __name__ == '__main__':
    main()
