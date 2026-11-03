<?php
/**
 * Jatetxea - Bezeroaren Web Interfazea (PHP)
 * Aurre-maila: REST APIarekin komunikazioa (cURL / Fetch)
 */
$api_base_url = getenv('API_BASE_URL') ?: 'http://localhost:5000/api/v1';
?>
<!DOCTYPE html>
<html lang="eu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Jatetxea - Erreserbak eta Karta</title>
    <link rel="stylesheet" href="assets/css/style.css">
</head>
<body>
    <header>
        <h1>Jatetxea - Karta eta Erreserbak</h1>
        <nav>
            <a href="#karta">Karta Ikusi</a>
            <a href="#erreserba">Erreserba Egin</a>
        </nav>
    </header>

    <main>
        <section id="karta">
            <h2>Gure Karta</h2>
            <div id="platerak-zerrenda">
                <!-- Web-Frontend-Agent-ek REST API bidez kargatuko du hemendik: GET /api/v1/platerak/erabilgarritasuna -->
                <p>Platerak kargatzen...</p>
            </div>
        </section>

        <section id="erreserba">
            <h2>Mahai Erreserba</h2>
            <form id="erreserba-form">
                <!-- Erreserba inprimakia: Mahaia aukeratu, txanda, pertsona kopurua -->
                <p>Erreserba inprimakia prestatzen...</p>
            </form>
        </section>
    </main>

    <footer>
        <p>&copy; <?php echo date('Y'); ?> Jatetxea - Goierri Eskola</p>
    </footer>

    <script src="assets/js/app.js"></script>
</body>
</html>
