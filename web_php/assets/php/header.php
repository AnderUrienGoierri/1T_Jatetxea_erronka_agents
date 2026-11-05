<?php
/**
 * JATETXEA - Web Goiburua (header.php)
 *
 * @var string $api_base_url
 * @var bool $api_online
 */

// Nabigatzailearentzako (bezeroa) API helbidea: Docker barruko 'host.docker.internal'
// ordezkatzen dugu 'localhost'-ekin, nabigatzaileak errore gabe atzitu dezan.
$client_api_url = (isset($api_base_url) && strpos($api_base_url, 'host.docker.internal') !== false)
    ? str_replace('host.docker.internal', 'localhost', $api_base_url)
    : ($api_base_url ?? 'http://localhost:5000/api/v1');
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
                    <?php if (!empty($api_online)): ?>
                        <span id="api-status" class="api-status-badge online">
                            <span class="status-dot"></span> API Linean
                        </span>
                    <?php else: ?>
                        <span id="api-status" class="api-status-badge offline">
                            <span class="status-dot"></span> API Deskonektatuta
                        </span>
                    <?php endif; ?>
                </div>
            </div>
        </div>
    </header>
