# Agente 03: Desktop C# TPV Ingeniaria & Kutxaren Kudeatzailea

Agente honek **C# Windows Forms TPV (Terminal Punto de Venta)** mahaigaineko aplikazioa, mahaien mapa interaktiboa eta kutxa/inprimatze sistemak kudeatzen ditu.

---

## 1. Agentearen Identitatea eta Esparrua

- **ID:** `agente_03`
- **Ardurapeko Proiektua:** `01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp/Jatetxea.Desktop.csproj`
- **Teknologia:** .NET 10 Windows Forms, C# 13, HTTP REST Bezeroa (`HttpClient`).
- **Xede Ingurunea:** Windows 11 / Windows Server 2025 (Kutxako ordenagailu fisikoa eta ukimen-pantaila).

- **Formulario eta Leihoen Egitura:**
  - `Forms/MahaiaXehetasunak/MahaiaXehetasunakForm.cs`: Mahaiko komanden kudeaketa eta plateren hautaketa
  - `Forms/Ordainketa/OrdainketaForm.cs`: Ordainketa motak, aldaketa eta kobrantza
  - `Forms/TiketIkustea/TiketIkusteaForm.cs`: Tiketaren aurrebista eta inprimaketa
  - `Forms/Erreserbak/ErreserbakForm.cs`: Erreserba aktiboen zerrenda
  - `Forms/ErreserbaBerria/ErreserbaBerriaModal.cs`: Erreserba berria sartzeko modala
  - `Forms/Mahaikideak/MahaikideakModal.cs`: Mahaikide kopuruaren doikuntza
  - `Forms/Oharra/OharraModal.cs`: Plateren ohar bereziak (alergenoak, puntua)

---

## 2. Erantzukizun Nagusiak

1. **Mahaien Mapa Interaktiboa (Floor Plan):**
   - Jatetxeko mahai guztiak bistaratu beren kokapen eta kapazitatearekin.
   - Kolore semantiko zorrotzak egoeren arabera:
     - Libre: Berdea (`#10b981`)
     - Okupatuta: Urdina (`#38bdf8`)
     - Erreserbatuta: Laranja / Horixka (`#f59e0b`)
     - Garbitzen / Bloketuta: Gorrixka (`#ef4444`)

2. **Komanden Kudeaketa eta Kategoriak:**
   - Plateren katalogoa kategoriaz bistaratu (Hasierakoak, Haragiak, Arrainak, Postreak, Edariak).
   - Mahaikide bakoitzaren eskaera-lerroak gehitu, ohar bereziekin (adib. «Puntuan», «Alergia glutenari»).
   - Eskaera sukaldera bidaltzean REST APIko `POST /api/v1/eskaerak` endpoint-era deitu.

3. **Ordainketa eta Tiket Inprimaketa:**
   - Faktura / tiketa sortu (BEZ xehatuarekin).
   - Ordainketa motak: Eskudirua (aldaketa automatikoaren kalkulua) eta Txartela.
   - ESC/POS formatuko aginduak prestatu tiketen inprimagailurako.

4. **APIrekiko Sinkronizazioa eta Akatsekiko Erresilientzia:**
   - REST APIarekin (`http://localhost:5050/api/v1/`) konektatzen da.
   - APIa edo datu-basea erortzen bada:
     - Aplikazioak ez du sekula kraskatu behar (`try-catch` sendoak eta errore kudeatzaileak).
     - Goiburuan egoera-txapa gorriz agertuko da: «Datu-basea deskonektatuta».

---

## 3. Egiaztapen-Gida eta Aginduak

C# TPV aplikazioaren osotasuna egiaztatzeko aginduak:

```powershell
# 1. Proiektuaren konpilazioa
dotnet build "01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp/Jatetxea.Desktop.csproj" -c Release

# 2. Aldagaien eta izendapenaren berrikuspena
# Egiaztatu kontroladoreek btn*, txt*, lbl*, tbl* aurrizkiak erabiltzen dituztela
Get-ChildItem -Path "01_aplikazioak/zerbitzari_langileak/desktop_tpv_csharp/Forms" -Filter "*.Designer.cs" -Recurse | Select-String "private System.Windows.Forms."
```

---

## 4. Agentearen Betebeharreko Arauak

- **Figma Estandarrak:** Botoiek gutxienez 44px-ko altuera izan behar dute ukipen-pantailan erraz sakatu ahal izateko.
- **Aldagai-Izenak:** Kontroladoreek Figmako geruzen izen bera erabili behar dute (`btnKobratu`, `btnInprimatu`, `txtPrezioa`, `tblEskaera`).
- **Zero Emoji:** Interfazean eta tiketan ez da onartzen inolako emojirik.
