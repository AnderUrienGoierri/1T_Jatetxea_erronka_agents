# Segurtasun eta DevSecOps Arauak

1. **Sekretuak eta Pasahitzak:**
   - Git-era igotzen den fitxategi batean **EZ DA INOIZ IPINIKO** pasahitzik, API gakorik edo konexio-katerik.
   - Tokiko garapenean `appsettings.Development.json` edo `.env` fitxategiak erabiliko dira, eta beti `.gitignore`-n sartuta egongo dira.
2. **SQL Injekzioaren Aurkako Neurriak:**
   - Ez erabili inoiz kate-kateatzerik (`string concatenation`) SQL kontsultetan.
   - Parametrizatutako kontsultak soilik erabili (`MySqlParameter`, Dapper `@param`).
3. **Sarrera Balidazioak:**
   - Datu guztiak baliozkotu zerbitzari mailan (DTOak, luzerak, motak, zenbaki positiboak).
4. **Baimenak (Rolen Kudeaketa):**
   - Endpoint kritikoak (adibidez plateren prezioak aldatzea edo salmenta txostenak ikustea) `[Authorize(Roles = "Admin,Kudeatzailea")]` babesarekin egongo dira.
