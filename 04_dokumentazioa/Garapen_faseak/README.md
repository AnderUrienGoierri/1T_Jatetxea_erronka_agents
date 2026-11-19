# Jatetxea Erronka - Garapen Faseak eta Betebeharrak

Dokumentazio-karpeta honek jatetxearen kudeaketa sistemaren **6 Garapen Faseak** biltzen ditu, **Microsoft Teams Planner** planifikazio integralean zehaztutako **12 aplikazio eta modulu** bakoitzarentzat fitxategi espezifikoekin (guztira 72 dokumentu tekniko).

*(Oharra: Flutter bezeroen aplikazio mugikorra erronkatik kanpo utzi da; Native Android eta Android TV aplikazioak erabiltzen dira).*

---

## Garapen Faseen Egitura

| Fasea | Karpeta | Epea | Modulu Kopurua | Deskribapen Laburra |
| :--- | :--- | :--- | :---: | :--- |
| **1. Analisia** | [`1-Analisia`](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/04_dokumentazioa/Garapen_faseak/1-Analisia) | 2026/11/03 - 2026/11/08 | 12 | Eskakizunak, erabilera-kasuak eta plataformen bideragarritasuna |
| **2. Diseinua** | [`2-Diseinua`](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/04_dokumentazioa/Garapen_faseak/2-Diseinua) | 2026/11/09 - 2026/11/14 | 12 | Datu-eredua, REST API kontratua, UI/UX (Figma) eta topologiak |
| **3. Garapena** | [`4-Garapena`](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/04_dokumentazioa/Garapen_faseak/4-Garapena) | 2026/11/15 - 2026/11/24 | 12 | Kodeketa, API kontroladoreak, interfaze grafikoak eta zerbitzuak |
| **4. Frogak** | [`3-Frogak`](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/04_dokumentazioa/Garapen_faseak/3-Frogak) | 2026/11/25 - 2026/11/28 | 12 | Proba unitarioak, integrazio-probak, karga eta segurtasuna |
| **5. Martxan Jartzea** | [`5-Martxan_jartzea`](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/04_dokumentazioa/Garapen_faseak/5-Martxan_jartzea) | 2026/11/29 - 2026/12/01 | 12 | Ekoizpen-ingurunea, Docker hedatzea, APK instalazioak eta kaxa-irekiera |
| **6. Mantentzea** | [`6-Mantentzea`](file:///C:/Dev/GoierriEskola/1T_Jatetxea_erronka_agents/04_dokumentazioa/Garapen_faseak/6-Mantentzea) | 2026/12/02 - 2026/12/05 | 12 | Backup automatikoak, erroreen log-ak, eguneraketak eta jarraipena |

---

## 12 Aplikazio eta Moduluak

Fase bakoitzean fitxategi hauek aurkituko dituzu:

1. `01_datu_basea.md` - Datu-Basea (MySQL 8.0 & PostgreSQL / Odoo)
2. `02_rest_api.md` - REST API Backend-a (ASP.NET Core C#)
3. `03_app_java_admin.md` - APP - Java Admin - Kudeatzailea (JavaFX)
4. `04_app_tpv_csharp.md` - APP - TPV - Zerbitzariak (C# WinForms)
5. `05_app_android_zerbitzariak.md` - APP - Android Zerbitzariak (Komandera Mugikorra / Tableta - Jetpack Compose)
6. `06_app_android_tv_sukaldea.md` - APP - Android TV - Sukaldariak (KDS - Jetpack Compose for TV)
7. `07_app_java_sukaldea.md` - APP Java - Sukaldea (KDS Mahaigainekoa - JavaFX)
8. `08_app_java_biltegia.md` - APP Java - Biltegia (Stock eta Inbentarioa - JavaFX)
9. `09_app_chat_zerbitzaria.md` - APP - Zerbitzaria / Chat Internoa (Socket.IO)
10. `10_app_web_bezeroak.md` - APP - Web (Apache/PHP) - Bezeroak
11. `11_azpiegitura_digitala.md` - AZPIEGITURA DIGITALA (Makina Birtualak & Docker)
12. `12_azpiegitura_fisikoa.md` - AZPIEGITURA FISIKOA (Laborategia, Sareak eta Ekipoak)

---

> [!NOTE]
> Fitxategi bakoitzak Planner-eko kartaren datuak (`Epea`, `Rola`, `Lehentasuna`), betebeharren checklist-a (`[ ]`), eginkizunen zehaztapen teknikoa, proiektuko iturburu-koderako estekak eta *Definition of Done (DoD)* balidazioak barne hartzen ditu.
