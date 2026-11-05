# Abio Azkarra - Proiektua Martxan Jartzeko Gida

Dokumentu honek azaltzen du nola jarri martxan proiektu hau hutsetik edozein ordenagailutan.

---

## 1. Aurre-baldintzak (Instalatuta egon behar direnak)

- **Docker Desktop:** Datu-basea edukiontzi batean altxatzeko.
- **.NET 10 SDK:** Backend APIa eta C# Desktop aplikazioa konpilatu eta exekutatzeko.
- **Java JDK 17 edo berriagoa:** JavaFX aplikazioa exekutatzeko.
- **PHP:** (Aukerazkoa) Web ataria probatzeko.

---

## 2. Martxan Jartzeko Urratsak (Ordena honetan)

### 1. Urratsa: Datu-Basea Altxatu (MySQL)

Ireki terminal bat proiektuaren erroko karpetan eta exekutatu:

```bash
docker compose up -d
```

- Portua: `3308`
- Datu-basea: `jatetxea_db` (erabiltzailea: `root`, pasahitza: `root`)
- Docker-ek lehen aldian automatikoki sortuko ditu taula guztiak eta hasierako datuak (`espezifikazioak/datu_basea_schema.sql` fitxategitik).

---

### 2. Urratsa: Backend APIa Abiarazi (.NET 10)

Beste terminal batean (edo fitxategi-nabigatzailetik `backend_api` karpetara joanda):

```bash
cd backend_api
dotnet run
```

- APIa `http://localhost:5000` helbidean geratuko da entzuten.
- Egiaztatzeko, nabigatzailean ireki: `http://localhost:5000/swagger`

---

### 3. Urratsa: Bezeroak Abiarazi

APIa martxan dagoela, edozein bezero abiarazi daiteke:

#### A) C# Desktop TPV Zerbitzaria (Tablet POS)

Terminal berri batean:

```bash
cd desktop_csharp
dotnet run
```

_(Edo Visual Studio 2022-n `Jatetxea.sln` ireki, proiektu nagusi gisa `Jatetxea.Desktop` jarri eta F5 eman)._

#### B) JavaFX KDS (Sukaldea, Barra eta Biltegia)

Terminal berri batean:

```bash
cd desktop_javafx
mvnw.cmd javafx:run
```

_(Linux edo Mac-en bada: `./mvnw javafx:run`. Edo IntelliJ IDEA-n `desktop_javafx` proiektua ireki eta `src/main/java/eus/goierri/jatetxea/App.java` exekutatu)._

#### C) Web Ataria (Karta eta Erreserbak)

Terminal berri batean:

```bash
cd web_php
php -S localhost:8000
```

- Nabigatzailean ireki: `http://localhost:8000`

---

## Ohar Garrantzitsuak

- **Datu-basearen konexioa:** C# APIak zein JavaFX bezeroak `localhost:3308` portura konektatzen dute. Ziurtatu Docker edukiontzia martxan dagoela (`docker ps`).
- **APIaren dependentzia:** Aurre-mailako bezero guztiek (`desktop_csharp`, `desktop_javafx` eta `web_php`) APIa (`http://localhost:5000`) martxan behar dute datuak jasotzeko.
