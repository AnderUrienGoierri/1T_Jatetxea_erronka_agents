import json

collection = {
    "info": {
        "_postman_id": "c8a2e783-f368-45e1-88c9-1a7f0e9d1234",
        "name": "1T Jatetxea REST API",
        "description": "1T Jatetxea erronkako ASP.NET Core Web API zentralaren Postman bilduma osoa. Taula eta eragiketa guztien kontsulta eta probak biltzen ditu: Mahaiak, Lehengaiak, Platerak, Eskaerak (KDS), Erreserbak, Langileak, Bezeroak, Iritziak, Ordainketak eta Osasun egoera.",
        "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
    },
    "item": []
}

def make_req(name, method, url_path, query=None, body_json=None, desc=""):
    headers = [
        {"key": "Content-Type", "value": "application/json", "type": "text"},
        {"key": "X-App-Id", "value": "postman", "type": "text"}
    ]
    raw_url = "{{baseUrl}}" + (("/" + url_path) if url_path else "")
    path_segments = [p for p in url_path.split("/") if p]
    
    url_obj = {
        "raw": raw_url,
        "host": ["{{baseUrl}}"],
        "path": path_segments
    }
    
    if query:
        url_obj["query"] = query
        query_str = "&".join([f"{q['key']}={q['value']}" for q in query])
        url_obj["raw"] += "?" + query_str
        
    req = {
        "name": name,
        "request": {
            "method": method,
            "header": headers,
            "url": url_obj,
            "description": desc
        },
        "response": []
    }
    
    if body_json is not None:
        req["request"]["body"] = {
            "mode": "raw",
            "raw": json.dumps(body_json, indent=2, ensure_ascii=False),
            "options": {
                "raw": {
                    "language": "json"
                }
            }
        }
    return req

# 00. Health & Sistema
f00 = {
    "name": "00. Health & Sistema",
    "item": [
        make_req("GET 01. API Informazioa eta Endpoint Zerrenda", "GET", "", desc="APIaren oinarrizko erroa. Informazio orokorra eta erabilgarri dauden bideak itzultzen ditu."),
        make_req("GET 02. Health Check (Datu-basearen konexio egoera)", "GET", "health", desc="APIaren eta MySQL datu-basearen osasun egoera (UP / DOWN) eta latentezia egiaztatzen du."),
        make_req("GET 03. Aplikazio Monitore Egoera Laburpena", "GET", "aplikazioak-linean", desc="Konektatuta dauden bezero eta aplikazioen egoera laburpena (Android, Web, KDS, etab.).")
    ]
}

# 01. Mahaiak
f01 = {
    "name": "01. Mahaiak",
    "item": [
        make_req("GET 01. Mahai Guztiak Lortu", "GET", "mahaiak", desc="Jatetxeko mahai aktibo guztien zerrenda."),
        make_req("GET 02. Mahai Guztiak (Ezabatuak barne)", "GET", "mahaiak", query=[{"key": "barneEzabatuak", "value": "true", "description": "Logikoki ezabatutako mahaiak ere itzuli"}], desc="Mahai guztiak lortu, logikoki ezabatutakoak barne."),
        make_req("GET 03. Mahai Bakarra ID bidez", "GET", "mahaiak/1", desc="ID zehatza duen mahaiaren datuak lortu."),
        make_req("GET 04. Mahai Libreak / Disponibleak", "GET", "mahaiak/disponibleak", desc="Une honetan Libre egoeran dauden mahaiak soilik itzultzen ditu."),
        make_req("POST 05. Mahai Berria Sortu", "POST", "mahaiak", body_json={
            "zenbakia": 15,
            "edukiera": 4,
            "kokapena": "Terraza"
        }, desc="Mahai berria gehitu datu-basean."),
        make_req("PUT 06. Mahaiaren Egoera Eguneratu", "PUT", "mahaiak/1/egoera", body_json={
            "egoera": "Okupatuta"
        }, desc="Mahaiaren egoera aldatu (Libre, Okupatuta, Erreserbatuta, Garbitzen)."),
        make_req("DELETE 07. Mahaia Ezabatu (Soft Delete)", "DELETE", "mahaiak/15", query=[{"key": "hard", "value": "false", "description": "false = Soft Delete (ezabatua=1)"}], desc="Mahaia logikoki ezabatu (ezabatua=1 marka jarriz)."),
        make_req("DELETE 08. Mahaia Behin Betiko Ezabatu (Hard Delete)", "DELETE", "mahaiak/15", query=[{"key": "hard", "value": "true", "description": "true = Datu-basetik guztiz ezabatu"}], desc="Mahaia datu-basetik behin betiko ezabatu."),
        make_req("PUT 09. Mahaia Berreskuratu (Restore)", "PUT", "mahaiak/15/restore", desc="Logikoki ezabatutako mahaia berreskuratu (ezabatua=0).")
    ]
}

# 02. Lehengaiak & Stocka
f02 = {
    "name": "02. Lehengaiak & Stocka",
    "item": [
        make_req("GET 01. Lehengaia Guztiak Lortu", "GET", "lehengaiak", desc="Biltegian dauden lehengai aktibo guztiak lortu."),
        make_req("GET 02. Lehengaia Guztiak (Ezabatuak barne)", "GET", "lehengaiak", query=[{"key": "barneEzabatuak", "value": "true"}], desc="Lehengai guztiak, ezabatuak barne."),
        make_req("GET 03. Lehengaia Bakarra ID bidez", "GET", "lehengaiak/1", desc="ID zehatza duen lehengaiaren datuak eta uneko stocka."),
        make_req("GET 04. Stock Gutxieneko Alertak", "GET", "lehengaiak/alertak", desc="Gutxieneko mailatik behera dauden lehengaiak eta proposatutako erosketa kopurua."),
        make_req("POST 05. Lehengaia Berria Sortu", "POST", "lehengaiak", body_json={
            "izena": "Oliba Olio Birjina Estra",
            "unitatea": "l",
            "unekoStocka": 25.0,
            "gutxienekoStocka": 10.0,
            "kostuUnitarioa": 8.50,
            "hornitzailea": "Goierri Olioak Koop."
        }, desc="Lehengai berri bat erregistratu biltegian."),
        make_req("POST 06. Albarana Sarrera (Stock Handitu)", "POST", "lehengaiak/sarrera", body_json={
            "lehengaiaId": 1,
            "kantitatea": 10.0,
            "hornitzailea": "Tolosa Ekoizleak",
            "oharrak": "Faktura: 2026-F042"
        }, desc="Hornitzailearen sarrera/albarana jaso eta lehengaiaren stocka gehitzen du."),
        make_req("DELETE 07. Lehengaia Ezabatu (Soft Delete)", "DELETE", "lehengaiak/1", query=[{"key": "hard", "value": "false"}], desc="Lehengaia logikoki ezabatu."),
        make_req("DELETE 08. Lehengaia Behin Betiko Ezabatu (Hard Delete)", "DELETE", "lehengaiak/1", query=[{"key": "hard", "value": "true"}], desc="Lehengaia behin betiko ezabatu."),
        make_req("PUT 09. Lehengaia Berreskuratu (Restore)", "PUT", "lehengaiak/1/restore", desc="Logikoki ezabatutako lehengaia berreskuratu.")
    ]
}

# 03. Platerak & Menua
f03 = {
    "name": "03. Platerak & Menua",
    "item": [
        make_req("GET 01. Plater Guztiak Lortu", "GET", "platerak", desc="Menuko plater aktibo guztiak lortu."),
        make_req("GET 02. Platerak Kategoriaren Arabera", "GET", "platerak", query=[{"key": "kategoria", "value": "Bigarrena"}], desc="Kategoria zehatz bateko platerak iragazi (Sarrerakoa, Lehena, Bigarrena, Postrea, Edaria)."),
        make_req("GET 03. Plater Guztiak (Ezabatuak barne)", "GET", "platerak", query=[{"key": "barneEzabatuak", "value": "true"}], desc="Plater guztiak, ezabatuak barne."),
        make_req("GET 04. Plater Bakarra ID bidez", "GET", "platerak/1", desc="ID zehatza duen plateraren xehetasunak."),
        make_req("GET 05. Plateren Erabilgarritasuna (Stockaren arabera)", "GET", "platerak/erabilgarritasuna", desc="Biltegiko uneko stockarekin plater bakoitzetik zenbat anoa prestatu daitezkeen kalkulatzen du."),
        make_req("GET 06. Plateraren Eskandalloa (Osagaiak)", "GET", "platerak/1/eskandalloa", desc="Plateraren osagaien zerrenda eta proportzioak (eskandallo teknikoa)."),
        make_req("POST 07. Plater Berria Sortu (Eskandallo barne)", "POST", "platerak", body_json={
            "izena": "Txuleta Eusko Label",
            "deskribapena": "Kiloko txuleta txingarretan erreta patata frijituekin",
            "kategoria": "Bigarrena",
            "prezioa": 38.50,
            "alergenoak": None,
            "osagaiak": [
                {"lehengaiaId": 1, "kantitatea": 1.0, "unitatea": "kg"},
                {"lehengaiaId": 2, "kantitatea": 0.25, "unitatea": "kg"}
            ]
        }, desc="Plater berria sortu bere osagaiekin batera."),
        make_req("DELETE 08. Platera Ezabatu (Soft Delete)", "DELETE", "platerak/1", query=[{"key": "hard", "value": "false"}], desc="Platera logikoki ezabatu menutik."),
        make_req("DELETE 09. Platera Behin Betiko Ezabatu (Hard Delete)", "DELETE", "platerak/1", query=[{"key": "hard", "value": "true"}], desc="Platera behin betiko ezabatu."),
        make_req("PUT 10. Platera Berreskuratu (Restore)", "PUT", "platerak/1/restore", desc="Logikoki ezabatutako platera berreskuratu.")
    ]
}

# 04. Eskaerak & Komandak (KDS)
f04 = {
    "name": "04. Eskaerak & Komandak (KDS)",
    "item": [
        make_req("GET 01. Eskaera Guztiak Lortu", "GET", "eskaerak", desc="Eskaera guztien zerrenda."),
        make_req("GET 02. Eskaerak Egoeraren Arabera Iragazi", "GET", "eskaerak", query=[{"key": "egoera", "value": "Irekia"}], desc="Egoera zehatz batean dauden eskaerak (Irekia, Sukaldean, Zerbitzatua, Kobratua, Ezeztatua)."),
        make_req("GET 03. Eskaera Bakarra Lerroekin ID bidez", "GET", "eskaerak/1", desc="Eskaera baten xehetasun osoa bere plater lerroekin batera."),
        make_req("POST 04. Eskaera Berria Sortu (Mahaia Ireki)", "POST", "eskaerak", body_json={
            "mahaiaId": 1,
            "langileaId": 1,
            "pertsonaKopurua": 4
        }, desc="Mahaian eskaera/kontu berria ireki."),
        make_req("POST 05. Eskaerari Plater Lerroa Gehitu", "POST", "eskaerak/1/lerroak", body_json={
            "plateraId": 2,
            "kantitatea": 2,
            "oharrak": "Haragia gutxi eginda, mesedez"
        }, desc="Irekita dagoen eskaera bati plater bat edo gehiago gehitu."),
        make_req("POST 06. Komanda Sukaldera Bidali (KDS)", "POST", "eskaerak/1/sukaldera", desc="Eskaerako plater berriak sukaldera bidali (KDS pantailan agertzeko) eta stocka deskontatu."),
        make_req("PUT 07. Eskaera Orokorraren Egoera Eguneratu", "PUT", "eskaerak/1/egoera", body_json={
            "egoera": "Zerbitzatua"
        }, desc="Eskaera osoaren egoera aldatu (Irekia, Sukaldean, Zerbitzatua, Kobratua, Ezeztatua)."),
        make_req("PUT 08. Plater Lerro Zehatzaren Egoera Aldatu (KDS Ekintza)", "PUT", "eskaerak/lerroak/1/egoera", body_json={
            "egoera": "Prestatua"
        }, desc="Sukaldariak lerro bakoitzaren egoera aldatzen du (Eskatua -> Sukaldean -> Prestatua -> Zerbitzatua)."),
        make_req("DELETE 09. Eskaera Ezeztatu / Ezabatu", "DELETE", "eskaerak/1", desc="Eskaera ezeztatu eta mahaia libre utzi.")
    ]
}

# 05. Erreserbak (Transakzionalak)
f05 = {
    "name": "05. Erreserbak (Transakzionalak)",
    "item": [
        make_req("GET 01. Erreserba Guztiak Lortu", "GET", "erreserbak", desc="Erreserba aktibo guztien zerrenda."),
        make_req("GET 02. Erreserbak Data eta Txandaren Arabera", "GET", "erreserbak", query=[
            {"key": "data", "value": "2026-11-20"},
            {"key": "txanda", "value": "Bazkaria_1"}
        ], desc="Egun eta txanda jakin bateko erreserbak iragazi."),
        make_req("GET 03. Erreserba Guztiak (Ezabatuak barne)", "GET", "erreserbak", query=[{"key": "barneEzabatuak", "value": "true"}], desc="Erreserba guztiak ezabatutakoak barne."),
        make_req("GET 04. Erreserba Bakarra ID bidez", "GET", "erreserbak/1", desc="Erreserba jakin baten xehetasunak."),
        make_req("POST 05. Erreserba Berria Sortu (Transakzio Babestua)", "POST", "erreserbak", body_json={
            "mahaiaId": 2,
            "data": "2026-11-20",
            "txanda": "Afaria_1",
            "pertsonaKopurua": 4,
            "oharrak": "Leihatila ondoan, mesedez",
            "bezeroIzena": "Miren",
            "bezeroAbizena": "Goikoetxea",
            "bezeroEmaila": "miren.goiko@example.eus",
            "bezeroTelefonoa": "600123456",
            "itxaron": 0
        }, desc="Erreserba berria sortu. Overbooking eta talken aurkako transakzio isolatua erabiltzen du."),
        make_req("DELETE 06. Erreserba Ezeztatu / Ezabatu (Soft Delete)", "DELETE", "erreserbak/1", query=[{"key": "hard", "value": "false"}], desc="Erreserba ezeztatu eta soft delete gisa markatu."),
        make_req("DELETE 07. Erreserba Behin Betiko Ezabatu (Hard Delete)", "DELETE", "erreserbak/1", query=[{"key": "hard", "value": "true"}], desc="Erreserba behin betiko ezabatu."),
        make_req("PUT 08. Erreserba Berreskuratu (Restore)", "PUT", "erreserbak/1/restore", desc="Ezeztatutako erreserba berreskuratu.")
    ]
}

# 06. Langileak & Saio-hasiera
f06 = {
    "name": "06. Langileak & Saio-hasiera",
    "item": [
        make_req("GET 01. Langile Guztiak Lortu", "GET", "langileak", desc="Langile aktibo guztien zerrenda (pasahitzik gabe)."),
        make_req("GET 02. Langile Guztiak (Ezabatuak barne)", "GET", "langileak", query=[{"key": "barneEzabatuak", "value": "true"}], desc="Langile guztiak, baja emandakoak barne."),
        make_req("GET 03. Langile Bakarra ID bidez", "GET", "langileak/1", desc="ID zehatza duen langilearen profil datuak."),
        make_req("POST 04. Langile Login / PIN Egiaztapena", "POST", "langileak/login", body_json={
            "erabiltzailea": "maite_zerb",
            "pasahitza": "1234"
        }, desc="Komandera mugikorreko edo kudeaketako saio-hasiera PIN edo pasahitz bidez."),
        make_req("POST 05. Langile Berria Sortu", "POST", "langileak", body_json={
            "erabiltzailea": "ane_zerb",
            "pasahitza": "1234",
            "izena": "Ane Mendizabal",
            "rola": "Zerbitzaria"
        }, desc="Langile berri bat erregistratu sisteman.")
    ]
}

# 07. Bezeroak
f07 = {
    "name": "07. Bezeroak",
    "item": [
        make_req("GET 01. Bezero Guztiak Lortu", "GET", "bezeroak", desc="Erregistratutako bezero guztien zerrenda."),
        make_req("GET 02. Bezero Guztiak (Ezabatuak barne)", "GET", "bezeroak", query=[{"key": "barneEzabatuak", "value": "true"}], desc="Bezero guztiak, ezabatuak barne."),
        make_req("GET 03. Bezero Bakarra ID bidez", "GET", "bezeroak/1", desc="ID zehatza duen bezeroaren fitxa osoa."),
        make_req("POST 04. Bezero Berria Sortu", "POST", "bezeroak", body_json={
            "izena": "Jon",
            "abizena": "Agirre",
            "telefonoa": "654987321",
            "emaila": "jon.agirre@example.eus"
        }, desc="Bezero berria gehitu datu-basean."),
        make_req("PUT 05. Bezeroaren Datuak Eguneratu", "PUT", "bezeroak/1", body_json={
            "izena": "Jon",
            "abizena": "Agirre Zabala",
            "telefonoa": "654987321",
            "emaila": "jon.agirre.zabala@example.eus"
        }, desc="Bezero baten kontaktu-datuak eguneratu."),
        make_req("DELETE 06. Bezeroa Ezabatu (Soft Delete)", "DELETE", "bezeroak/1", desc="Bezeroa logikoki ezabatu (ezabatua=1).")
    ]
}

# 08. Iritziak
f08 = {
    "name": "08. Iritziak",
    "item": [
        make_req("GET 01. Iritzi Guztiak Lortu", "GET", "iritziak", desc="Jatetxeko bezeroen iritzi eta balorazio guztiak."),
        make_req("GET 02. Iritzi Estatistikak (Bataz bestekoa eta izarrak)", "GET", "iritziak/estatistikak", desc="Balorazio orokorraren estatistika kalkulatuak (bataz besteko puntuazioa, izar banaketa)."),
        make_req("POST 03. Iritzi Berria Utzi", "POST", "iritziak", body_json={
            "bezeroIzena": "Miren",
            "bezeroAbizena": "Goikoetxea",
            "bezeroEmaila": "miren.goiko@example.eus",
            "balorazioa": 5,
            "testua": "Janari bikaina eta zerbitzu ezin hobea! Seguru itzuliko garela."
        }, desc="Bezeroak iritzi eta puntuazio berria utzi (0tik 5era)."),
        make_req("DELETE 04. Iritzia Ezabatu", "DELETE", "iritziak/1", desc="Iritzi bat ezabatu ID bidez.")
    ]
}

# 09. Ordainketak & Tiketak
f09 = {
    "name": "09. Ordainketak & Tiketak",
    "item": [
        make_req("POST 01. Ordainketa Burutu (Txartela / Eskudirua / Bizum)", "POST", "ordainketak", body_json={
            "eskaeraId": 1,
            "mota": "Txartela",
            "zenbatekoa": 45.50,
            "txartelZenbakia": "4532XXXXXXXX1234",
            "titularra": "Ander Urien",
            "iraungitzea": "12/28",
            "cvv": "123",
            "langileaId": 1
        }, desc="Eskaera kobratu eta ordainketa erregistratu (Txartela, Eskudirua, Bizum, etab.). Tiketa sortzen du."),
        make_req("GET 02. Eskaeraren Ordainketa Xehetasunak Lortu", "GET", "ordainketak/eskaera/1", desc="Eskaera jakin bati lotutako ordainketa eta tiketa lortu."),
        make_req("GET 03. Tiket Testua Inprimatzeko Lortu (ESC/POS)", "GET", "ordainketak/1/inprimatu", desc="Tiket fisikoa inprimagailura bidaltzeko testu-formatuan lortu.")
    ]
}

collection["item"] = [f00, f01, f02, f03, f04, f05, f06, f07, f08, f09]

with open("postman/1T_Jatetxea_REST_API.postman_collection.json", "w", encoding="utf-8") as f:
    json.dump(collection, f, indent=2, ensure_ascii=False)

print("Postman bilduma ongi sortu da!")
