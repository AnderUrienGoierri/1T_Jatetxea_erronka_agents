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
    $host_parts = explode(':', $_SERVER['HTTP_HOST'] ?? 'localhost');
    $client_host = $host_parts[0];
    $client_port = $host_parts[1] ?? '';
    if (empty($client_host) || $client_host === '_' || $client_host === '0.0.0.0') {
        $client_host = 'localhost';
    }
    // VirtualBox NAT bidez 8088 portutik sartzen bada, APIa 5051 portuan dago; bestela 5050.
    $api_port = ($client_port === '8088') ? '5051' : '5050';
    $client_api_url = "http://{$client_host}:{$api_port}/api/v1";
}
?>
<!DOCTYPE html>
<html lang="eu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>T1 Jatetxea · Osteria &amp; Parrilla | Italiar-Euskaldun Gastronomia</title>
    <link rel="icon" type="image/svg+xml" href="assets/img/t1_isotipoa.svg">
    
    <!-- Google Fonts: Cinzel, Plus Jakarta Sans eta JetBrains Mono (T1 v2.0) -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Cinzel:wght@600;700;800;900&family=JetBrains+Mono:wght@500;700&family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap" rel="stylesheet">
    
    <link rel="stylesheet" href="assets/css/style.css?v=<?php echo filemtime(__DIR__ . '/../css/style.css'); ?>">
    <link rel="stylesheet" href="assets/css/nav.css?v=<?php echo filemtime(__DIR__ . '/../css/nav.css'); ?>">
    <script>
        // Bezeroaren JavaScript logikari API helbidea pasa (nabigatzailetik erabilgarria)
        window.API_BASE_URL = "<?php echo htmlspecialchars($client_api_url, ENT_QUOTES, 'UTF-8'); ?>";
        (function() {
            var savedTheme = localStorage.getItem('t1_web_theme') || 'light';
            document.documentElement.setAttribute('data-theme', savedTheme);
        })();
        function toggleWebTheme() {
            var cur = document.documentElement.getAttribute('data-theme') || 'light';
            var next = cur === 'dark' ? 'light' : 'dark';
            document.documentElement.setAttribute('data-theme', next);
            localStorage.setItem('t1_web_theme', next);
            var btn = document.getElementById('webThemeBtn');
            if (btn) btn.textContent = next === 'dark' ? 'Modu Argia' : 'Modu Iluna';
        }
        document.addEventListener('DOMContentLoaded', function() {
            var cur = document.documentElement.getAttribute('data-theme') || 'light';
            var btn = document.getElementById('webThemeBtn');
            if (btn) btn.textContent = cur === 'dark' ? 'Modu Argia' : 'Modu Iluna';
        });
    </script>
</head>
<body>

    <!-- GOIBURUA -->
    <header>
        <div class="header-container">
            <!-- Ezkerrean: Jatetxearen Izena eta Nortasuna -->
            <div class="logo-area">
                <a href="index.php" class="header-brand-wrap" title="T1 Jatetxea - Hasierara Itzuli">
                    <img src="assets/img/t1_logo_dark.svg" alt="T1 Logo B" class="header-logo-icon">
                    <div class="brand-text-block">
                        <h1>T1 JATETXEA</h1>
                        <span class="brand-subtitle">OSTERIA &amp; PARRILLA · ITALO-EUSKALDUNA</span>
                    </div>
                </a>
            </div>

            <!-- Erdian: Nabigazio-menua -->
            <nav>
                <ul>
                    <li><a href="#karta">Karta</a></li>
                    <li><a href="#erreserba">Erreserba</a></li>
                    <li><a href="#iritziak">Iritziak</a></li>
                    <li><a href="#kontaktua">Kontaktua</a></li>
                </ul>
            </nav>

            <!-- Eskuinaldean: Gaia + API egoera -->
            <div class="header-right">
                <details class="settings-menu">
                    <summary class="settings-toggle" aria-label="Ezarpenak" title="Ezarpenak">
                        <svg viewBox="0 1 24 24" width="24" height="24" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
                            <path d="M10 2.5h4l.5 2.2c.6.2 1.2.4 1.8.8l1.9-1.2L21 7l-1.2 2c.3.5.6 1.1.7 1.7l2.2.5v3.6l-2.2.5c-.1.6-.4 1.2-.7 1.7l1.2 2-2.8 2.7-1.9-1.2c-.6.4-1.2.6-1.8.8l-.5 2.2h-4l-.5-2.2c-.6-.2-1.2-.4-1.8-.8l-1.9 1.2L3 19l1.2-2c-.3-.5-.6-1.1-.7-1.7l-2.2-.5v-3.6l2.2-.5c.1-.6.4-1.2.7-1.7L3 7l2.8-2.7 1.9 1.2c.6-.4 1.2-.6 1.8-.8Z"></path>
                            <circle cx="12" cy="13" r="3.1"></circle>
                        </svg>
                    </summary>
                    <div class="settings-panel">
                        <button type="button" id="webThemeBtn" class="theme-toggle-btn" onclick="toggleWebTheme()">
                            Modu Iluna
                        </button>
                    </div>
                </details>
                <?php
                    if (!empty($api_online) && !empty($db_online)) {
                        $statusClass = 'online';
                        $statusText = 'API eta datu-basea konektatuta';
                    } elseif (!empty($api_online)) {
                        $statusClass = 'warning';
                        $statusText = 'APIak erantzuten du, baina datu-basearen konexioak edo egoera-egiaztapenak huts egin du';
                    } else {
                        $statusClass = 'offline';
                        $statusText = 'APIa ez dago eskuragarri; datu-basearen egoera ezezaguna da';
                    }
                ?>
                <span id="api-status" class="api-status-badge <?php echo $statusClass; ?>" role="status" tabindex="0" aria-label="<?php echo $statusText; ?>" title="<?php echo $statusText; ?>"></span>
            </div>
        </div>
    </header>
