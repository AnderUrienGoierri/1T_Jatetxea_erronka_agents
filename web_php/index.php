<?php
/**
 * Jatetxea - Bezeroaren Web Interfazea (PHP)
 * Aurre-maila: REST API bidezko komunikazioa (C# ASP.NET Core)
 * Zerbitzari-mailako kontsumoa (cURL / stream context bidez)
 */

if (session_status() === PHP_SESSION_NONE) {
    session_start();
}

// 1. API OINARRIZKO HELBIDEA ZEHAZTU
$api_base_url = getenv('API_BASE_URL');
if (!$api_base_url) {
    // Docker barruan bagaude host.docker.internal erabili, bestela localhost
    $api_base_url = (file_exists('/.dockerenv') || getenv('DOCKER_CONTAINER'))
        ? 'http://host.docker.internal:5000/api/v1'
        : 'http://localhost:5000/api/v1';
}

/**
 * C# REST APIari eskaera egiteko helper funtzio orokorra.
 */
function call_api($method, $url, $payload = null) {
    if (function_exists('curl_init')) {
        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, $url);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_TIMEOUT, 6);
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, $method);

        $headers = ['Accept: application/json'];

        if ($payload !== null) {
            $jsonData = json_encode($payload);
            curl_setopt($ch, CURLOPT_POSTFIELDS, $jsonData);
            $headers[] = 'Content-Type: application/json';
            $headers[] = 'Content-Length: ' . strlen($jsonData);
        }

        curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);

        $result = curl_exec($ch);
        $http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        $err = curl_error($ch);
        curl_close($ch);

        return [
            'status' => $http_code,
            'data' => $result ? json_decode($result, true) : null,
            'raw' => $result,
            'error' => $err
        ];
    } else {
        // Fallback: file_get_contents stream context bidez
        $opts = [
            'http' => [
                'method' => $method,
                'timeout' => 6,
                'ignore_errors' => true,
                'header' => "Accept: application/json\r\n"
            ]
        ];

        if ($payload !== null) {
            $jsonData = json_encode($payload);
            $opts['http']['header'] .= "Content-Type: application/json\r\nContent-Length: " . strlen($jsonData) . "\r\n";
            $opts['http']['content'] = $jsonData;
        }

        $context = stream_context_create($opts);
        $result = @file_get_contents($url, false, $context);
        $http_code = 500;
        if (isset($http_response_header) && count($http_response_header) > 0) {
            if (preg_match('/HTTP\/\d\.\d\s+(\d+)/', $http_response_header[0], $matches)) {
                $http_code = (int)$matches[1];
            }
        }

        return [
            'status' => $http_code,
            'data' => $result ? json_decode($result, true) : null,
            'raw' => $result,
            'error' => $result === false ? 'HTTP eskaerak huts egin du' : ''
        ];
    }
}

// 2. FORMULARIOEN BIDALKETAK KUDEATU (POST bidez)
if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $action = $_POST['action'] ?? '';

    if ($action === 'erreserbatu') {
        $mahaiaId = !empty($_POST['mahaia_id']) ? (int)$_POST['mahaia_id'] : null;
        $data = trim($_POST['data'] ?? '');
        $txanda = trim($_POST['txanda'] ?? '');
        $pertsonak = (int)($_POST['pertsona_kopurua'] ?? 2);
        $izena = trim($_POST['bezero_izena'] ?? '');
        $abizena = trim($_POST['bezero_abizena'] ?? '');
        $emaila = trim($_POST['bezero_emaila'] ?? '');
        $telefonoa = trim($_POST['bezero_telefonoa'] ?? '');
        $oharrak = trim($_POST['erreserba_oharrak'] ?? '');

        if (!$mahaiaId || empty($data) || empty($txanda) || empty($izena) || empty($emaila)) {
            $_SESSION['flash_error'] = "Mesedez, bete derrigorrezko eremu guztiak erreserba egiteko.";
        } else {
            $postData = [
                'mahaiaId' => $mahaiaId,
                'data' => $data,
                'txanda' => $txanda,
                'pertsonaKopurua' => $pertsonak,
                'bezeroIzena' => $izena,
                'bezeroAbizena' => !empty($abizena) ? $abizena : '-',
                'bezeroEmaila' => $emaila,
                'bezeroTelefonoa' => !empty($telefonoa) ? $telefonoa : '-',
                'oharrak' => !empty($oharrak) ? $oharrak : null
            ];

            $res = call_api('POST', "$api_base_url/erreserbak", $postData);

            if ($res['status'] === 201) {
                $errId = $res['data']['data']['id'] ?? '';
                $mZenb = $res['data']['data']['mahaiZenbakia'] ?? $mahaiaId;
                $_SESSION['flash_success'] = "Erreserba arrakastaz baieztatu da (Erreserba ID: #$errId, Mahaia: #$mZenb, Data: $data, Txanda: $txanda). Ongi etorri!";
            } elseif ($res['status'] === 409) {
                $_SESSION['flash_conflict'] = "Gatazka (409 Conflict): Aukeratutako mahaia hartuta dago jada data eta txanda horretan. Mesedez, hautatu beste mahai edo txanda bat.";
            } else {
                $mezua = $res['data']['message'] ?? 'Ezin izan da erreserba burutu zerbitzariaren erroreagatik.';
                $_SESSION['flash_error'] = "Errorea (" . $res['status'] . "): " . htmlspecialchars($mezua, ENT_QUOTES, 'UTF-8');
            }
        }

        header("Location: " . $_SERVER['PHP_SELF'] . "#erreserba");
        exit;
    }

    if ($action === 'iritzia') {
        $emaila = trim($_POST['iritzi_emaila'] ?? '');
        $izena = trim($_POST['iritzi_izena'] ?? '');
        $balorazioa = isset($_POST['iritzi_balorazioa']) ? (int)$_POST['iritzi_balorazioa'] : -1;
        $testua = trim($_POST['iritzi_testua'] ?? '');

        if (empty($emaila) || $balorazioa < 0 || $balorazioa > 5) {
            $_SESSION['flash_iritzi_error'] = "Mesedez, adierazi e-posta eta balorazio balioduna (0 eta 5 izar artean).";
        } else {
            $postData = [
                'bezeroEmaila' => $emaila,
                'bezeroIzena' => !empty($izena) ? $izena : 'Bezeroa',
                'balorazioa' => $balorazioa,
                'testua' => !empty($testua) ? $testua : null
            ];

            $res = call_api('POST', "$api_base_url/iritziak", $postData);

            if ($res['status'] === 201) {
                $_SESSION['flash_iritzi_success'] = "Mila esker zure balorazioagatik! Arrakastaz argitaratu da gure atarian.";
            } else {
                $mezua = $res['data']['message'] ?? 'Ezin izan da iritzia gorde.';
                $_SESSION['flash_iritzi_error'] = "Errorea (" . $res['status'] . "): " . htmlspecialchars($mezua, ENT_QUOTES, 'UTF-8');
            }
        }

        header("Location: " . $_SERVER['PHP_SELF'] . "#iritziak");
        exit;
    }
}

// 3. ZERBITZARI-MAILAN DATUAK KARGATU (C# REST API)
$resPlaterak = call_api('GET', "$api_base_url/platerak");
$platerak = ($resPlaterak['status'] === 200 && isset($resPlaterak['data']['dataList']))
    ? $resPlaterak['data']['dataList']
    : [];

$api_online = ($resPlaterak['status'] === 200);

$resAhalmena = call_api('GET', "$api_base_url/platerak/erabilgarritasuna");
$ahalmenaMap = [];
if ($resAhalmena['status'] === 200 && isset($resAhalmena['data']['dataList'])) {
    foreach ($resAhalmena['data']['dataList'] as $item) {
        $ahalmenaMap[$item['plateraId']] = (int)$item['eginDaitezkeenPlaterak'];
    }
}

$resMahaiak = call_api('GET', "$api_base_url/mahaiak");
$mahaiak = ($resMahaiak['status'] === 200 && isset($resMahaiak['data']['dataList']))
    ? $resMahaiak['data']['dataList']
    : [];

$resIritziak = call_api('GET', "$api_base_url/iritziak");
$iritziak = ($resIritziak['status'] === 200 && isset($resIritziak['data']['dataList']))
    ? $resIritziak['data']['dataList']
    : [];

$resEstatistikak = call_api('GET', "$api_base_url/iritziak/estatistikak");
$estatistikak = ($resEstatistikak['status'] === 200 && isset($resEstatistikak['data']['data']))
    ? $resEstatistikak['data']['data']
    : [
        'guztiraIritziak' => count($iritziak),
        'batazBestekoBalorazioa' => 0.0,
        'bostIzarrak' => 0, 'lauIzarrak' => 0, 'hiruIzarrak' => 0,
        'biIzarrak' => 0, 'izarBat' => 0, 'zeroIzar' => 0
    ];

// Flash mezuak lortu eta garbitu
$flash_success = $_SESSION['flash_success'] ?? null;
$flash_conflict = $_SESSION['flash_conflict'] ?? null;
$flash_error = $_SESSION['flash_error'] ?? null;
unset($_SESSION['flash_success'], $_SESSION['flash_conflict'], $_SESSION['flash_error']);

$flash_iritzi_success = $_SESSION['flash_iritzi_success'] ?? null;
$flash_iritzi_error = $_SESSION['flash_iritzi_error'] ?? null;
unset($_SESSION['flash_iritzi_success'], $_SESSION['flash_iritzi_error']);
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
                <?php if ($api_online): ?>
                    <span id="api-status" class="api-status-badge online">API Linean (C# REST API)</span>
                <?php else: ?>
                    <span id="api-status" class="api-status-badge offline">API Deskonektatuta</span>
                <?php endif; ?>
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

        <!-- 1. KARTA ATALA (PHP ZERBITZARI-MAILAKO ERRENDERIZAZIOA) -->
        <section id="karta">
            <div class="section-header">
                <h2>Gure Karta</h2>
                <p>Egunero bertako baserritar eta arraindegietatik jasotako lehengaiekin prestatua (Guztira: <?php echo count($platerak); ?> plater eta edari)</p>
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

            <!-- Plateren Grid-a (PHP bidez zerbitzarian sortua) -->
            <div id="platerak-zerrenda" class="platerak-grid">
                <?php if (empty($platerak)): ?>
                    <p class="oharra-testua">Ez dago platerik erabilgarri edo REST APIa ez dago martxan.</p>
                <?php else: ?>
                    <?php foreach ($platerak as $p): ?>
                        <?php
                            $plateraId = (int)$p['id'];
                            $kategoria = htmlspecialchars($p['kategoria'], ENT_QUOTES, 'UTF-8');
                            $izena = htmlspecialchars($p['izena'], ENT_QUOTES, 'UTF-8');
                            $deskribapena = htmlspecialchars($p['deskribapena'] ?? '', ENT_QUOTES, 'UTF-8');
                            $alergenoak = htmlspecialchars($p['alergenoak'] ?? '', ENT_QUOTES, 'UTF-8');
                            $prezioZenb = (float)($p['prezioa'] ?? $p['salmentaPrezioa'] ?? 0);
                            $prezioa = number_format($prezioZenb, 2, ',', '.') . ' €';
                            $denbora = (int)($p['prestatzeDenboraMin'] ?? 15);
                            
                            $stockAhalmena = isset($ahalmenaMap[$plateraId]) ? $ahalmenaMap[$plateraId] : null;
                            $agortuta = ($stockAhalmena !== null && $stockAhalmena === 0);
                        ?>
                        <div class="plater-card <?php echo $agortuta ? 'agortuta' : ''; ?>" data-kategoria="<?php echo $kategoria; ?>">
                            <div class="plater-header">
                                <h3 class="plater-izena"><?php echo $izena; ?></h3>
                                <span class="plater-prezioa"><?php echo $prezioa; ?></span>
                            </div>
                            <p class="plater-deskribapena"><?php echo $deskribapena; ?></p>
                            
                            <?php if (!empty($alergenoak)): ?>
                                <p style="font-size: 0.8rem; color: #d97706; margin-bottom: 0.5rem;">
                                    <strong>Alergenoak:</strong> <?php echo $alergenoak; ?>
                                </p>
                            <?php endif; ?>

                            <?php if (!empty($p['osagaiak']) && is_array($p['osagaiak'])): ?>
                                <div class="plater-osagaiak">
                                    <strong>Osagaiak:</strong>
                                    <ul>
                                        <?php foreach ($p['osagaiak'] as $osagaia): ?>
                                            <li><?php echo htmlspecialchars($osagaia['lehengaiaIzena'], ENT_QUOTES, 'UTF-8'); ?></li>
                                        <?php endforeach; ?>
                                    </ul>
                                </div>
                            <?php endif; ?>

                            <div class="plater-meta">
                                <span class="kategoria-tag"><?php echo $kategoria; ?></span>
                                <span class="prestatze-denbora" style="font-size: 0.8rem; color: #64748b;"><?php echo $denbora; ?> min</span>
                                <?php if ($agortuta): ?>
                                    <span class="stock-tag agortuta">Agortuta</span>
                                <?php elseif ($stockAhalmena !== null): ?>
                                    <span class="stock-tag eskuragarri"><?php echo $stockAhalmena; ?> anoa</span>
                                <?php endif; ?>
                            </div>
                        </div>
                    <?php endforeach; ?>
                <?php endif; ?>
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
                    
                    <!-- PHP bidezko Alertak -->
                    <?php if ($flash_success): ?>
                        <div class="alert-kaxa alert-success" style="display: block;">
                            <strong>Baieztatuta:</strong> <?php echo htmlspecialchars($flash_success, ENT_QUOTES, 'UTF-8'); ?>
                        </div>
                    <?php endif; ?>

                    <?php if ($flash_conflict): ?>
                        <div class="alert-kaxa alert-error" style="display: block; border-left: 4px solid #dc2626;">
                            <strong>Gatazka (409):</strong> <?php echo htmlspecialchars($flash_conflict, ENT_QUOTES, 'UTF-8'); ?>
                        </div>
                    <?php endif; ?>

                    <?php if ($flash_error): ?>
                        <div class="alert-kaxa alert-error" style="display: block;">
                            <?php echo htmlspecialchars($flash_error, ENT_QUOTES, 'UTF-8'); ?>
                        </div>
                    <?php endif; ?>

                    <!-- JS alertentzako lekua -->
                    <div id="erreserba-alerta" class="alert-kaxa"></div>

                    <form id="erreserba-form" method="POST" action="index.php#erreserba">
                        <input type="hidden" name="action" value="erreserbatu">

                        <div class="form-row">
                            <div class="form-group">
                                <label for="erreserba-data">Data (*):</label>
                                <input type="date" name="data" id="erreserba-data" class="form-control" value="<?php echo date('Y-m-d'); ?>" required>
                            </div>
                            <div class="form-group">
                                <label for="erreserba-txanda">Txanda (*):</label>
                                <select name="txanda" id="erreserba-txanda" class="form-control" required>
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
                                <input type="number" name="pertsona_kopurua" id="erreserba-pertsonak" class="form-control" min="1" max="20" value="2" required>
                            </div>
                            <div class="form-group">
                                <label for="erreserba-mahaia">Mahaia (*):</label>
                                <select name="mahaia_id" id="erreserba-mahaia" class="form-control" required>
                                    <?php if (empty($mahaiak)): ?>
                                        <option value="">Mahaiak kargatzen edo zerbitzaria itzalita...</option>
                                    <?php else: ?>
                                        <option value="">-- Hautatu mahaia --</option>
                                        <?php foreach ($mahaiak as $m): ?>
                                            <option value="<?php echo (int)$m['id']; ?>">
                                                Mahaia #<?php echo (int)$m['zenbakia']; ?> (<?php echo (int)$m['edukiera']; ?> pertsona - <?php echo htmlspecialchars($m['kokapena'] ?? 'Jantokia', ENT_QUOTES, 'UTF-8'); ?>)
                                            </option>
                                        <?php endforeach; ?>
                                    <?php endif; ?>
                                </select>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="bezero-izena">Izena (*):</label>
                                <input type="text" name="bezero_izena" id="bezero-izena" class="form-control" placeholder="Adib. Ane" required>
                            </div>
                            <div class="form-group">
                                <label for="bezero-abizena">Abizena:</label>
                                <input type="text" name="bezero_abizena" id="bezero-abizena" class="form-control" placeholder="Adib. Iparragirre">
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="bezero-emaila">E-posta helbidea (*):</label>
                                <input type="email" name="bezero_emaila" id="bezero-emaila" class="form-control" placeholder="ane@adibidea.eus" required>
                            </div>
                            <div class="form-group">
                                <label for="bezero-telefonoa">Telefonoa:</label>
                                <input type="tel" name="bezero_telefonoa" id="bezero-telefonoa" class="form-control" placeholder="600112233">
                            </div>
                        </div>

                        <div class="form-group">
                            <label for="erreserba-oharrak">Oharrak edo Alergiak:</label>
                            <textarea name="erreserba_oharrak" id="erreserba-oharrak" class="form-control" rows="2" placeholder="Aulki gehigarria, ospakizuna, gluten intolerantea..."></textarea>
                        </div>

                        <button type="submit" id="btn-erreserbatu" class="btn btn-primary" style="width: 100%;">Erreserba Baieztatu (PHP bidez)</button>
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
                        <strong>Garrantzitsua:</strong> Mahai bererako eta txanda bererako eskaera bat baino gehiago jasotzen bada aldi berean, C# APIak lehentasuna lehenengoari emango dio (409 Conflict errorearen kudeaketa transakzionala). Gatazka gertatuz gero, sistemak abisu garbia erakutsiko du.
                    </div>
                </div>
            </div>
        </section>

        <!-- 3. BEZEROEN IRITZIAK ATALA (PHP ZERBITZARI-MAILAKO ERRENDERIZAZIOA) -->
        <section id="iritziak">
            <div class="section-header">
                <h2>Bezeroen Balorazioak</h2>
                <p>Gure bezeroek partekatutako esperientziak eta iritziak</p>
            </div>

            <div class="iritziak-grid">
                <!-- Ezkerra: Estatistika orokorrak -->
                <div id="balorazio-estatistikak" class="estatistikak-kaxa">
                    <?php
                        $media = number_format((float)($estatistikak['batazBestekoBalorazioa'] ?? 0), 1);
                        $guztira = (int)($estatistikak['guztiraIritziak'] ?? 0);
                    ?>
                    <div class="estatistika-puntuazioa"><?php echo $media; ?></div>
                    <div class="estatistika-izarrak">
                        <?php
                            $borobildua = (int)round((float)$media);
                            for ($i = 1; $i <= 5; $i++) {
                                echo ($i <= $borobildua) ? '★' : '☆';
                            }
                        ?>
                    </div>
                    <div class="estatistika-guztira"><?php echo $guztira; ?> bezeroren balorazioak</div>

                    <div class="izar-distribuzioa">
                        <?php
                            $mailak = [
                                5 => $estatistikak['bostIzarrak'] ?? 0,
                                4 => $estatistikak['lauIzarrak'] ?? 0,
                                3 => $estatistikak['hiruIzarrak'] ?? 0,
                                2 => $estatistikak['biIzarrak'] ?? 0,
                                1 => $estatistikak['izarBat'] ?? 0,
                                0 => $estatistikak['zeroIzar'] ?? 0
                            ];
                            foreach ($mailak as $izar => $kopurua):
                                $portzentaia = $guztira > 0 ? round(($kopurua / $guztira) * 100) : 0;
                        ?>
                            <div class="distribuzio-lerroa">
                                <span><?php echo $izar; ?> ★</span>
                                <div class="barra-hutsik">
                                    <div class="barra-beteta" style="width: <?php echo $portzentaia; ?>%;"></div>
                                </div>
                                <span class="lerro-kopurua"><?php echo $kopurua; ?></span>
                            </div>
                        <?php endforeach; ?>
                    </div>
                </div>

                <!-- Eskuina: Iritzi zerrenda eta iritzi berria idazteko inprimakia -->
                <div>
                    <div class="iritziak-zerrenda-kaxa">
                        <h3 style="margin-bottom: 1rem; color: #1e293b; font-size: 1.15rem;">Azken Iritziak (<?php echo count($iritziak); ?>)</h3>
                        <div id="iritziak-zerrenda">
                            <?php if (empty($iritziak)): ?>
                                <p class="oharra-testua">Oraindik ez dago iritzirik. Izan zaitez lehena zure esperientzia partekatzen!</p>
                            <?php else: ?>
                                <?php foreach ($iritziak as $i): ?>
                                    <?php
                                        $bezeroa = htmlspecialchars($i['bezeroIzena'] ?? 'Bezeroa', ENT_QUOTES, 'UTF-8');
                                        $dataIritzi = htmlspecialchars(substr($i['sortzeData'] ?? '', 0, 10), ENT_QUOTES, 'UTF-8');
                                        $puntuak = (int)($i['balorazioa'] ?? 5);
                                        $testua = htmlspecialchars($i['testua'] ?? '', ENT_QUOTES, 'UTF-8');
                                    ?>
                                    <div class="iritzi-txartela">
                                        <div class="iritzi-goiburua">
                                            <strong><?php echo $bezeroa; ?></strong>
                                            <span class="iritzi-data"><?php echo $dataIritzi; ?></span>
                                        </div>
                                        <div class="iritzi-izarrak">
                                            <?php
                                                for ($x = 1; $x <= 5; $x++) {
                                                    echo ($x <= $puntuak) ? '★' : '☆';
                                                }
                                            ?>
                                        </div>
                                        <?php if (!empty($testua)): ?>
                                            <p class="iritzi-testua"><?php echo $testua; ?></p>
                                        <?php endif; ?>
                                    </div>
                                <?php endforeach; ?>
                            <?php endif; ?>
                        </div>
                    </div>

                    <!-- Iritzia uzteko inprimakia -->
                    <div class="erreserba-form-area" style="margin-top: 1.5rem; padding: 1.5rem; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 8px;">
                        <h4 style="margin-bottom: 1rem; color: #1e293b;">Utzi Zure Balorazioa</h4>
                        
                        <?php if ($flash_iritzi_success): ?>
                            <div class="alert-kaxa alert-success" style="display: block;">
                                <?php echo htmlspecialchars($flash_iritzi_success, ENT_QUOTES, 'UTF-8'); ?>
                            </div>
                        <?php endif; ?>

                        <?php if ($flash_iritzi_error): ?>
                            <div class="alert-kaxa alert-error" style="display: block;">
                                <?php echo htmlspecialchars($flash_iritzi_error, ENT_QUOTES, 'UTF-8'); ?>
                            </div>
                        <?php endif; ?>

                        <div id="iritzi-alerta" class="alert-kaxa"></div>

                        <form id="iritzi-form" method="POST" action="index.php#iritziak">
                            <input type="hidden" name="action" value="iritzia">

                            <div class="form-row">
                                <div class="form-group">
                                    <label for="iritzi-emaila">E-posta helbidea (*):</label>
                                    <input type="email" name="iritzi_emaila" id="iritzi-emaila" class="form-control" placeholder="zure.emaila@adibidea.eus" required>
                                </div>
                                <div class="form-group">
                                    <label for="iritzi-izena">Izena:</label>
                                    <input type="text" name="iritzi_izena" id="iritzi-izena" class="form-control" placeholder="Zure izena">
                                </div>
                            </div>
                            <div class="form-group">
                                <label for="iritzi-balorazioa">Puntuazioa (0 eta 5 artean) (*):</label>
                                <select name="iritzi_balorazioa" id="iritzi-balorazioa" class="form-control" required>
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
                                <textarea name="iritzi_testua" id="iritzi-testua" class="form-control" rows="2" placeholder="Nola joan da zure esperientzia?"></textarea>
                            </div>
                            <button type="submit" class="btn btn-secondary" style="width: 100%;">Iritzia Bidali (PHP bidez)</button>
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

    <!-- MODULARIZATUTAKO JAVASCRIPT FITXATEGIAK (Interaktibitate arina eta iragazkiak) -->
    <script src="assets/js/config.js"></script>
    <script src="assets/js/karta.js"></script>
    <script src="assets/js/erreserbak.js"></script>
    <script src="assets/js/iritziak.js"></script>
    <script src="assets/js/app.js"></script>
</body>
</html>