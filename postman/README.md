# 1T Jatetxea - Postman API Bilduma eta Ingurunea

Karpeta honek 1T Jatetxea proiektuko ASP.NET Core REST API osoaren Postman bilduma (Collection v2.1) eta ingurune aldagaiak (Environment) biltzen ditu, zuzenean Postman-en inportatu eta probak egiteko prest.

---

## 1. Fitxategiak

| Fitxategia | Deskribapena |
|---|---|
| `1T_Jatetxea_REST_API.postman_collection.json` | APIko kontroladore, taula eta metodo guztien bilduma (GET, POST, PUT, DELETE). |
| `1T_Jatetxea_Local.postman_environment.json` | Ingurune aldagaia (`baseUrl`, `serverUrl`, eta sare ezberdinetako aurre-ezarpenak). |
| `generate_collection.py` | Bilduma automatikoki berreraikitzeko Python script laguntzailea. |

---

## 2. Nola Inportatu Postman-en

1. Ireki **Postman**.
2. Egin klik goiko ezkerreko **Import** botoian.
3. Arrastatu fitxategi hauek edo hautatu eskuz:
   - `postman/1T_Jatetxea_REST_API.postman_collection.json`
   - `postman/1T_Jatetxea_Local.postman_environment.json`
4. Goiko eskuineko inguruneen zabalgarrian (Environment dropdown), hautatu **"1T Jatetxea - Lokala & Sareak"**.

---

## 3. Bildumaren Egitura (10 Karpeta eta Endpoint Guztiak)

### 00. Health & Sistema
- `GET 01. API Informazioa eta Endpoint Zerrenda`: `{{baseUrl}}/`
- `GET 02. Health Check (Datu-basearen konexio egoera)`: `{{baseUrl}}/health`
- `GET 03. Aplikazio Monitore Egoera Laburpena`: `{{baseUrl}}/aplikazioak-linean`

### 01. Mahaiak
- `GET 01. Mahai Guztiak Lortu`: `{{baseUrl}}/mahaiak`
- `GET 02. Mahai Guztiak (Ezabatuak barne)`: `{{baseUrl}}/mahaiak?barneEzabatuak=true`
- `GET 03. Mahai Bakarra ID bidez`: `{{baseUrl}}/mahaiak/1`
- `GET 04. Mahai Libreak / Disponibleak`: `{{baseUrl}}/mahaiak/disponibleak`
- `POST 05. Mahai Berria Sortu`: `{{baseUrl}}/mahaiak`
- `PUT 06. Mahaiaren Egoera Eguneratu`: `{{baseUrl}}/mahaiak/1/egoera`
- `DELETE 07. Mahaia Ezabatu (Soft Delete)`: `{{baseUrl}}/mahaiak/15?hard=false`
- `DELETE 08. Mahaia Behin Betiko Ezabatu (Hard Delete)`: `{{baseUrl}}/mahaiak/15?hard=true`
- `PUT 09. Mahaia Berreskuratu (Restore)`: `{{baseUrl}}/mahaiak/15/restore`

### 02. Lehengaiak & Stocka
- `GET 01. Lehengaia Guztiak Lortu`: `{{baseUrl}}/lehengaiak`
- `GET 02. Lehengaia Guztiak (Ezabatuak barne)`: `{{baseUrl}}/lehengaiak?barneEzabatuak=true`
- `GET 03. Lehengaia Bakarra ID bidez`: `{{baseUrl}}/lehengaiak/1`
- `GET 04. Stock Gutxieneko Alertak`: `{{baseUrl}}/lehengaiak/alertak`
- `POST 05. Lehengaia Berria Sortu`: `{{baseUrl}}/lehengaiak`
- `POST 06. Albarana Sarrera (Stock Handitu)`: `{{baseUrl}}/lehengaiak/sarrera`
- `DELETE 07. Lehengaia Ezabatu (Soft Delete)`: `{{baseUrl}}/lehengaiak/1?hard=false`
- `DELETE 08. Lehengaia Behin Betiko Ezabatu (Hard Delete)`: `{{baseUrl}}/lehengaiak/1?hard=true`
- `PUT 09. Lehengaia Berreskuratu (Restore)`: `{{baseUrl}}/lehengaiak/1/restore`

### 03. Platerak & Menua
- `GET 01. Plater Guztiak Lortu`: `{{baseUrl}}/platerak`
- `GET 02. Platerak Kategoriaren Arabera`: `{{baseUrl}}/platerak?kategoria=Bigarrena`
- `GET 03. Plater Guztiak (Ezabatuak barne)`: `{{baseUrl}}/platerak?barneEzabatuak=true`
- `GET 04. Plater Bakarra ID bidez`: `{{baseUrl}}/platerak/1`
- `GET 05. Plateren Erabilgarritasuna (Stockaren arabera)`: `{{baseUrl}}/platerak/erabilgarritasuna`
- `GET 06. Plateraren Eskandalloa (Osagaiak)`: `{{baseUrl}}/platerak/1/eskandalloa`
- `POST 07. Plater Berria Sortu (Eskandallo barne)`: `{{baseUrl}}/platerak`
- `DELETE 08. Platera Ezabatu (Soft Delete)`: `{{baseUrl}}/platerak/1?hard=false`
- `DELETE 09. Platera Behin Betiko Ezabatu (Hard Delete)`: `{{baseUrl}}/platerak/1?hard=true`
- `PUT 10. Platera Berreskuratu (Restore)`: `{{baseUrl}}/platerak/1/restore`

### 04. Eskaerak & Komandak (KDS)
- `GET 01. Eskaera Guztiak Lortu`: `{{baseUrl}}/eskaerak`
- `GET 02. Eskaerak Egoeraren Arabera Iragazi`: `{{baseUrl}}/eskaerak?egoera=Irekia`
- `GET 03. Eskaera Bakarra Lerroekin ID bidez`: `{{baseUrl}}/eskaerak/1`
- `POST 04. Eskaera Berria Sortu (Mahaia Ireki)`: `{{baseUrl}}/eskaerak`
- `POST 05. Eskaerari Plater Lerroa Gehitu`: `{{baseUrl}}/eskaerak/1/lerroak`
- `POST 06. Komanda Sukaldera Bidali (KDS)`: `{{baseUrl}}/eskaerak/1/sukaldera`
- `PUT 07. Eskaera Orokorraren Egoera Eguneratu`: `{{baseUrl}}/eskaerak/1/egoera`
- `PUT 08. Plater Lerro Zehatzaren Egoera Aldatu (KDS Ekintza)`: `{{baseUrl}}/eskaerak/lerroak/1/egoera`
- `DELETE 09. Eskaera Ezeztatu / Ezabatu`: `{{baseUrl}}/eskaerak/1`

### 05. Erreserbak (Transakzionalak)
- `GET 01. Erreserba Guztiak Lortu`: `{{baseUrl}}/erreserbak`
- `GET 02. Erreserbak Data eta Txandaren Arabera`: `{{baseUrl}}/erreserbak?data=2026-11-20&txanda=Bazkaria_1`
- `GET 03. Erreserba Guztiak (Ezabatuak barne)`: `{{baseUrl}}/erreserbak?barneEzabatuak=true`
- `GET 04. Erreserba Bakarra ID bidez`: `{{baseUrl}}/erreserbak/1`
- `POST 05. Erreserba Berria Sortu (Transakzio Babestua)`: `{{baseUrl}}/erreserbak`
- `DELETE 06. Erreserba Ezeztatu / Ezabatu (Soft Delete)`: `{{baseUrl}}/erreserbak/1?hard=false`
- `DELETE 07. Erreserba Behin Betiko Ezabatu (Hard Delete)`: `{{baseUrl}}/erreserbak/1?hard=true`
- `PUT 08. Erreserba Berreskuratu (Restore)`: `{{baseUrl}}/erreserbak/1/restore`

### 06. Langileak & Saio-hasiera
- `GET 01. Langile Guztiak Lortu`: `{{baseUrl}}/langileak`
- `GET 02. Langile Guztiak (Ezabatuak barne)`: `{{baseUrl}}/langileak?barneEzabatuak=true`
- `GET 03. Langile Bakarra ID bidez`: `{{baseUrl}}/langileak/1`
- `POST 04. Langile Login / PIN Egiaztapena`: `{{baseUrl}}/langileak/login`
- `POST 05. Langile Berria Sortu`: `{{baseUrl}}/langileak`

### 07. Bezeroak
- `GET 01. Bezero Guztiak Lortu`: `{{baseUrl}}/bezeroak`
- `GET 02. Bezero Guztiak (Ezabatuak barne)`: `{{baseUrl}}/bezeroak?barneEzabatuak=true`
- `GET 03. Bezero Bakarra ID bidez`: `{{baseUrl}}/bezeroak/1`
- `POST 04. Bezero Berria Sortu`: `{{baseUrl}}/bezeroak`
- `PUT 05. Bezeroaren Datuak Eguneratu`: `{{baseUrl}}/bezeroak/1`
- `DELETE 06. Bezeroa Ezabatu (Soft Delete)`: `{{baseUrl}}/bezeroak/1`

### 08. Iritziak
- `GET 01. Iritzi Guztiak Lortu`: `{{baseUrl}}/iritziak`
- `GET 02. Iritzi Estatistikak (Bataz bestekoa eta izarrak)`: `{{baseUrl}}/iritziak/estatistikak`
- `POST 03. Iritzi Berria Utzi`: `{{baseUrl}}/iritziak`
- `DELETE 04. Iritzia Ezabatu`: `{{baseUrl}}/iritziak/1`

### 09. Ordainketak & Tiketak
- `POST 01. Ordainketa Burutu (Txartela / Eskudirua / Bizum)`: `{{baseUrl}}/ordainketak`
- `GET 02. Eskaeraren Ordainketa Xehetasunak Lortu`: `{{baseUrl}}/ordainketak/eskaera/1`
- `GET 03. Tiket Testua Inprimatzeko Lortu (ESC/POS)`: `{{baseUrl}}/ordainketak/1/inprimatu`

---

## 4. Oinarrizko Datuak eta PINak

- Langileen PIN lehenetsiak: `1234`
- Erabiltzaile laginak:
  - `maite_zerb` (Zerbitzaria)
  - `mikel_zerb` (Zerbitzaria)
  - `unai_zerb` (Zerbitzaria)
  - `gorka_barra` (Barrakoa)
  - `anderu` (AdminIT)
