<?php
/**
 * JATETXEA - Web Goiburua (header.php)
 *
 * @var string $api_base_url
 * @var bool $api_online
 */

// Nabigatzailearentzako (bezeroaren JavaScript-a) REST API helbidea kalkulatu:
// REST APIaren ostalari-ataka 5050 da (5000 ataka IT Zentralarentzat da).
// Horrez gain, bezeroak webgunera sartzeko erabilitako ostalari bera (localhost edo tokiko IP) ezartzen dugu.
$client_api_env = getenv('CLIENT_API_BASE_URL');
if (!empty($client_api_env)) {
    $client_api_url = $client_api_env;
} else {
    $client_host = explode(':', $_SERVER['HTTP_HOST'] ?? 'localhost')[0];
    if (empty($client_host) || $client_host === '_' || $client_host === '0.0.0.0') {
        $client_host = 'localhost';
    }
    $client_api_url = "http://{$client_host}:5050/api/v1";
}
?>
<!DOCTYPE html>
<html lang="eu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>T1 Jatetxea · Osteria &amp; Parrilla | Italiar-Euskaldun Gastronomia</title>
    <link rel="icon" type="image/svg+xml" href="assets/img/t1_isotipoa.svg">
    
    <!-- Google Fonts: Cinzel (Titulu artistikoak) eta Inter (UI garbia) -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Cinzel:wght@600;700;800;900&family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    
    <link rel="stylesheet" href="assets/css/style.css">
    <script>
        // Bezeroaren JavaScript logikari API helbidea pasa (nabigatzailetik erabilgarria)
        window.API_BASE_URL = "<?php echo htmlspecialchars($client_api_url, ENT_QUOTES, 'UTF-8'); ?>";
    </script>
</head>
<body>

    <!-- GOIBURUA -->
    <header>
        <div class="header-container">
            <!-- Ezkerrean: Jatetxearen Izena eta Nortasuna -->
            <div class="logo-area">
                <a href="index.php" class="header-brand-wrap" title="T1 Jatetxea - Hasierara Itzuli">
                    <img src="assets/img/t1_isotipoa.svg" alt="T1 Isotipoa" class="header-logo-icon">
                    <div class="brand-text-block">
                        <h1>T1 JATETXEA</h1>
                        <span class="brand-subtitle">OSTERIA &amp; PARRILLA · ITALO-EUSKALDUNA</span>
                    </div>
                </a>
            </div>

            <!-- Erdian: Nabigazio-menua -->
            <nav>
                <ul>
                    <li><a href="#karta" class="active">Karta</a></li>
                    <li><a href="#erreserba">Erreserba</a></li>
                    <li><a href="#iritziak">Iritziak</a></li>
                    <li><a href="#kontaktua">Kontaktua</a></li>
                </ul>
            </nav>

            <!-- Eskuinaldean: API egoera eta Laguntza -->
            <div class="header-right">
                <div class="api-status-wrapper">
                    <?php if (!empty($api_online) && !empty($db_online)): ?>
                        <span id="api-status" class="api-status-badge online" title="REST API eta MySQL Datu-basea konektatuta daude">
                            <span class="status-dot"></span> API &amp; DB Linean
                        </span>
                    <?php elseif (!empty($api_online) && empty($db_online)): ?>
                        <span id="api-status" class="api-status-badge warning" style="background:#451a03; color:#fde047; border:1px solid #ca8a04; display:inline-flex; align-items:center; gap:6px; padding:4px 10px; border-radius:20px; font-size:12px; font-weight:600;" title="API-ra konektatuta dago, baina MySQL datu-basearekin ezin izan da konexioa lortu">
                            <span class="status-dot" style="background:#eab308;"></span> API Konektatuta | DB Deskonektatuta
                        </span>
                    <?php else: ?>
                        <span id="api-status" class="api-status-badge offline" title="Ezin izan da REST APIarekin konektatu">
                            <span class="status-dot"></span> API Deskonektatuta
                        </span>
                    <?php endif; ?>
                </div>
            </div>
        </div>
    </header>

    <?php if (!empty($api_online) && empty($db_online)): ?>
    <div id="db-alert-banner" style="background:#991b1b; color:#ffffff; font-weight:700; text-align:center; padding:10px 16px; font-size:13px; border-bottom:2px solid #7f1d1d; letter-spacing:0.3px;">
        ADI: REST API zerbitzariarekin konexioa ondo lortu da (Portua 5050), baina MySQL datu-basearekin ezin izan da konexioa ezarri! (Datu-basea itzalita dago)
    </div>
    <?php elseif (empty($api_online)): ?>
    <div id="api-alert-banner" style="background:#7f1d1d; color:#fee2e2; font-weight:600; text-align:center; padding:8px 16px; font-size:13px; border-bottom:1px solid #991b1b;">
        OHARRA: REST API nagusia deskonektatuta dago (http://localhost:5050). Datu estatikoak erakusten ari dira.
    </div>
    <?php endif; ?>
