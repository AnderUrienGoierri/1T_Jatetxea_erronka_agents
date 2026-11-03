<?php
/**
 * Jatetxea - Bezeroaren Web Interfazea (PHP)
 * Aurre-maila: REST API bidezko komunikazioa (C# ASP.NET Core)
 * Arau profesionala: emotikonorik gabe
 */
$api_base_url = getenv('API_BASE_URL') ?: 'http://localhost:5000/api/v1';
?>
<!DOCTYPE html>
<html lang="eu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Jatetxea - Euskal Gastronomia eta Erreserbak</title>
    <link rel="stylesheet" href="assets/css/style.css">
    <script>
        // Backend APIaren oinarrizko helbidea pasatu JavaScript logikari
        window.API_BASE_URL = "<?php echo htmlspecialchars($api_base_url, ENT_QUOTES, 'UTF-8'); ?>";
    </script>
</head>
<body>

    <!-- GOIBURUA -->
    <header>
        <div class="header-container">
            <div class="logo-area">
                <h1>JATETXEA</h1>
                <span>Euskal Sukaldaritza Tradizionala eta Garaikidea</span>
            </div>
            <nav>
                <ul>
                    <li><a href="#karta" class="active">Karta</a></li>
                    <li><a href="#erreserba">Erreserba</a></li>
                    <li><a href="#iritziak">Iritziak</a></li>
                    <li><a href="#kontaktua">Kontaktua</a></li>
                </ul>
            </nav>
            <div>
                <span id="api-status" class="api-status-badge offline">Egiaztatzen...</span>
            </div>
        </div>
    </header>

    <!-- HERO ATALA -->
    <section class="hero">
        <div class="hero-content">
            <h2>Gure Lurraldeko Zapore Autentikoak</h2>
            <p>Euskal Okela Txuleta, Tolosako Babarrunak sakramentu guztiekin eta eguneko arrain freskoak. Gozatu esperientzia gastronomiko paregabeaz.</p>
            <div class="hero-buttons">
                <a href="#erreserba" class="btn btn-primary">Mahaia Erreserbatu</a>
                <a href="#karta" class="btn btn-secondary">Karta Arakatu</a>
            </div>
        </div>
    </section>

    <div class="container">

        <!-- 1. KARTA ATALA -->
        <section id="karta">
            <div class="section-header">
                <h2>Gure Karta</h2>
                <p>Egunero bertako baserritar eta arraindegietatik jasotako lehengaiekin prestatua</p>
            </div>

            <!-- Kategoria-iragazkiak -->
            <div class="kategoria-filters">
                <button type="button" class="filter-btn active" data-kategoria="Guztiak">Guztiak</button>
                <button type="button" class="filter-btn" data-kategoria="Sarrerakoa">Sarrerakoak</button>
                <button type="button" class="filter-btn" data-kategoria="Lehena">Lehen Platerak</button>
                <button type="button" class="filter-btn" data-kategoria="Bigarrena">Bigarren Platerak</button>
                <button type="button" class="filter-btn" data-kategoria="Postrea">Postreak</button>
                <button type="button" class="filter-btn" data-kategoria="Edaria">Edariak</button>
            </div>

            <!-- Plateren Grid-a (JavaScript bidez REST APItik kargatua) -->
            <div id="platerak-zerrenda" class="platerak-grid">
                <p>Platerak kargatzen...</p>
            </div>
        </section>

        <!-- 2. MAHAI ERRESERBA ATALA -->
        <section id="erreserba">
            <div class="section-header">
                <h2>Mahai Erreserba</h2>
                <p>Aukeratu zure data, txanda eta mahaia unean bertako baieztapenarekin</p>
            </div>

            <div class="erreserba-wrapper">
                <!-- Erreserba inprimakia -->
                <div class="erreserba-form-area">
                    <h3>Erreserbaren Datuak</h3>
                    
                    <!-- Feedback Alerta (Arrakasta edo 409 Conflict) -->
                    <div id="erreserba-alerta" class="alert-kaxa"></div>

                    <form id="erreserba-form">
                        <div class="form-row">
                            <div class="form-group">
                                <label for="erreserba-data">Data (*):</label>
                                <input type="date" id="erreserba-data" class="form-control" required>
                            </div>
                            <div class="form-group">
                                <label for="erreserba-txanda">Txanda (*):</label>
                                <select id="erreserba-txanda" class="form-control" required>
                                    <option value="Bazkaria_1">Bazkaria 1 (13:00 - 14:30)</option>
                                    <option value="Bazkaria_2">Bazkaria 2 (14:45 - 16:30)</option>
                                    <option value="Afaria_1">Afaria 1 (20:30 - 22:00)</option>
                                    <option value="Afaria_2">Afaria 2 (22:15 - 00:00)</option>
                                </select>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="erreserba-pertsonak">Mahaikide Kopurua (*):</label>
                                <input type="number" id="erreserba-pertsonak" class="form-control" min="1" max="20" value="2" required>
                            </div>
                            <div class="form-group">
                                <label for="erreserba-mahaia">Mahaia (*):</label>
                                <select id="erreserba-mahaia" class="form-control" required>
                                    <option value="">Mahaiak kargatzen...</option>
                                </select>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="bezero-izena">Izena (*):</label>
                                <input type="text" id="bezero-izena" class="form-control" placeholder="Adib. Ane" required>
                            </div>
                            <div class="form-group">
                                <label for="bezero-abizena">Abizena:</label>
                                <input type="text" id="bezero-abizena" class="form-control" placeholder="Adib. Iparragirre">
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="bezero-emaila">E-posta helbidea (*):</label>
                                <input type="email" id="bezero-emaila" class="form-control" placeholder="ane@adibidea.eus" required>
                            </div>
                            <div class="form-group">
                                <label for="bezero-telefonoa">Telefonoa:</label>
                                <input type="tel" id="bezero-telefonoa" class="form-control" placeholder="600112233">
                            </div>
                        </div>

                        <div class="form-group">
                            <label for="erreserba-oharrak">Oharrak edo Alergiak:</label>
                            <textarea id="erreserba-oharrak" class="form-control" rows="2" placeholder="Aulki gehigarria, ospakizuna, gluten intolerantea..."></textarea>
                        </div>

                        <button type="submit" id="btn-erreserbatu" class="btn btn-primary" style="width: 100%;">Erreserba Baieztatu</button>
                    </form>
                </div>

                <!-- Informazio osagarria eta arauak -->
                <div class="erreserba-info-area">
                    <div>
                        <h3>Jantokiko Arauak eta Ordutegiak</h3>
                        <p style="font-size: 0.9rem; color: #64748b; margin-bottom: 1rem;">
                            Gure zerbitzua ahalik eta ondoen antolatzeko eta elikagaien alferrik galtzea saihesteko, mahai bakoitza txandaka kudeatzen da:
                        </p>
                        <ul class="txanden-ordutegiak">
                            <li><span>Bazkaria 1:</span> <strong>13:00 - 14:30</strong></li>
                            <li><span>Bazkaria 2:</span> <strong>14:45 - 16:30</strong></li>
                            <li><span>Afaria 1:</span> <strong>20:30 - 22:00</strong></li>
                            <li><span>Afaria 2:</span> <strong>22:15 - 00:00</strong></li>
                        </ul>
                    </div>

                    <div class="oharra-kaxa">
                        <strong>Garrantzitsua:</strong> Mahai bererako eta txanda bererako eskaera bat baino gehiago jasotzen bada aldi berean, sistemak lehentasuna lehenengoari emango dio (409 Conflict errorearen kudeaketa automatikoa). Gatazka gertatuz gero, mesedez hautatu beste mahai edo txanda bat.
                    </div>
                </div>
            </div>
        </section>

        <!-- 3. BEZEROEN IRITZIAK ATALA -->
        <section id="iritziak">
            <div class="section-header">
                <h2>Bezeroen Balorazioak</h2>
                <p>Gure bezeroek partekatutako esperientziak eta iritziak</p>
            </div>

            <div class="iritziak-grid">
                <!-- Ezkerra: Estatistika orokorrak -->
                <div id="balorazio-estatistikak" class="estatistikak-kaxa">
                    <p>Estatistikak kargatzen...</p>
                </div>

                <!-- Eskuina: Iritzi zerrenda eta iritzi berria idazteko inprimakia -->
                <div>
                    <div class="iritziak-zerrenda-kaxa">
                        <h3 style="margin-bottom: 1rem; color: #1e293b; font-size: 1.15rem;">Azken Iritziak</h3>
                        <div id="iritziak-zerrenda">
                            <p>Iritziak kargatzen...</p>
                        </div>
                    </div>

                    <!-- Iritzia uzteko inprimakia -->
                    <div class="erreserba-form-area" style="margin-top: 1.5rem; padding: 1.5rem; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 8px;">
                        <h4 style="margin-bottom: 1rem; color: #1e293b;">Utzi Zure Balorazioa</h4>
                        <div id="iritzi-alerta" class="alert-kaxa"></div>
                        <form id="iritzi-form">
                            <div class="form-row">
                                <div class="form-group">
                                    <label for="iritzi-emaila">E-posta helbidea (*):</label>
                                    <input type="email" id="iritzi-emaila" class="form-control" placeholder="zure.emaila@adibidea.eus" required>
                                </div>
                                <div class="form-group">
                                    <label for="iritzi-izena">Izena:</label>
                                    <input type="text" id="iritzi-izena" class="form-control" placeholder="Zure izena">
                                </div>
                            </div>
                            <div class="form-group">
                                <label for="iritzi-balorazioa">Puntuazioa (0 eta 5 artean) (*):</label>
                                <select id="iritzi-balorazioa" class="form-control" required>
                                    <option value="5">5 izar - Bikaina</option>
                                    <option value="4">4 izar - Oso ona</option>
                                    <option value="3">3 izar - Ona</option>
                                    <option value="2">2 izar - Hobetzekoa</option>
                                    <option value="1">1 izar - Eskasa</option>
                                    <option value="0">0 izar - Oso txarra</option>
                                </select>
                            </div>
                            <div class="form-group">
                                <label for="iritzi-testua">Iruzkina:</label>
                                <textarea id="iritzi-testua" class="form-control" rows="2" placeholder="Nola joan da zure esperientzia?"></textarea>
                            </div>
                            <button type="submit" class="btn btn-secondary" style="width: 100%;">Iritzia Bidali</button>
                        </form>
                    </div>
                </div>
            </div>
        </section>

        <!-- 4. KONTAKTUA ETA ORDUTEGIA -->
        <section id="kontaktua">
            <div class="section-header">
                <h2>Kokapena eta Harremana</h2>
                <p>Zure zain gaude Goierriko bihotzean</p>
            </div>
            <div style="background-color: #ffffff; border: 1px solid #e2e8f0; border-radius: 8px; padding: 2rem; display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 1.5rem; text-align: center;">
                <div>
                    <h4 style="color: #1e293b; margin-bottom: 0.5rem;">Helbidea</h4>
                    <p style="color: #64748b;">Goierri Kalea 10, 20240 Ordizia, Gipuzkoa</p>
                </div>
                <div>
                    <h4 style="color: #1e293b; margin-bottom: 0.5rem;">Telefonoa</h4>
                    <p style="color: #64748b;">+34 943 12 34 56</p>
                </div>
                <div>
                    <h4 style="color: #1e293b; margin-bottom: 0.5rem;">E-posta</h4>
                    <p style="color: #64748b;">erreserbak@jatetxea.eus</p>
                </div>
                <div>
                    <h4 style="color: #1e293b; margin-bottom: 0.5rem;">Asteazkenetik Igandera</h4>
                    <p style="color: #64748b;">13:00 - 16:30 | 20:30 - 00:00 (Astelehen eta astearteetan itxita)</p>
                </div>
            </div>
        </section>

    </div>

    <!-- OINA -->
    <footer>
        <p>&copy; <?php echo date('Y'); ?> Jatetxea - Goierri Eskola. 3-Mailako Arkitektura (Web PHP, Desktop C#, Desktop JavaFX, C# REST API, MySQL 8.0 InnoDB).</p>
    </footer>

    <!-- MODULARIZATUTAKO JAVASCRIPT FITXATEGIAK -->
    <script src="assets/js/config.js"></script>
    <script src="assets/js/karta.js"></script>
    <script src="assets/js/erreserbak.js"></script>
    <script src="assets/js/iritziak.js"></script>
    <script src="assets/js/app.js"></script>
</body>
</html>
