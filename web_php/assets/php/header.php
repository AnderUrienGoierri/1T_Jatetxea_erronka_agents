<?php
/**
 * JATETXEA - Web Goiburua (header.php)
 *
 * @var string $api_base_url
 * @var bool $api_online
 */
?>
<!DOCTYPE html>
<html lang="eu">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Jatetxea - Euskal Gastronomia eta Erreserbak</title>
    <link rel="stylesheet" href="assets/css/style.css">
    <script>
        // Bezeroaren JavaScript logikari API helbidea pasa
        window.API_BASE_URL = "<?php echo htmlspecialchars($api_base_url ?? '', ENT_QUOTES, 'UTF-8'); ?>";
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
                <?php if (!empty($api_online)): ?>
                    <span id="api-status" class="api-status-badge online">API Linean (C# REST API)</span>
                <?php else: ?>
                    <span id="api-status" class="api-status-badge offline">API Deskonektatuta</span>
                <?php endif; ?>
            </div>
        </div>
    </header>
