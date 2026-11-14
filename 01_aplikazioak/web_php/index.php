<?php
/**
 * Jatetxea - Bezeroaren Web Interfazea (PHP)
 * Arkitektura: Modularra (assets/php/ karpetako osagaiak erabiliz)
 * Aurre-maila: REST API bidezko komunikazioa (C# ASP.NET Core)
 */

if (session_status() === PHP_SESSION_NONE) {
    session_start();
}

// 1. API BEZEROA ETA POST HANDLERRAK KARGATU
require_once __DIR__ . '/assets/php/api.php';
require_once __DIR__ . '/assets/php/post_handlers.php';

// 2. ZERBITZARI-MAILAN DATUAK KARGATU (C# REST API)
$resPlaterak = call_api('GET', "$api_base_url/platerak");
$platerak = ($resPlaterak['status'] === 200 && isset($resPlaterak['data']['dataList']))
    ? $resPlaterak['data']['dataList']
    : [];

$api_online = ($resPlaterak['status'] === 200);

$ahalmenaMap = [];
$mahaiak = [];
$iritziak = [];
$estatistikak = [
    'guztiraIritziak' => 0,
    'batazBestekoBalorazioa' => 0.0,
    'bostIzarrak' => 0, 'lauIzarrak' => 0, 'hiruIzarrak' => 0,
    'biIzarrak' => 0, 'izarBat' => 0, 'zeroIzar' => 0
];

if ($api_online) {
    $resAhalmena = call_api('GET', "$api_base_url/platerak/erabilgarritasuna");
    if ($resAhalmena['status'] === 200 && isset($resAhalmena['data']['dataList'])) {
        foreach ($resAhalmena['data']['dataList'] as $item) {
            $ahalmenaMap[$item['plateraId']] = (int)$item['eginDaitezkeenPlaterak'];
        }
    }

    $resMahaiak = call_api('GET', "$api_base_url/mahaiak");
    if ($resMahaiak['status'] === 200 && isset($resMahaiak['data']['dataList'])) {
        $mahaiak = $resMahaiak['data']['dataList'];
    }

    $resIritziak = call_api('GET', "$api_base_url/iritziak");
    if ($resIritziak['status'] === 200 && isset($resIritziak['data']['dataList'])) {
        $iritziak = $resIritziak['data']['dataList'];
    }

    $resEstatistikak = call_api('GET', "$api_base_url/iritziak/estatistikak");
    if ($resEstatistikak['status'] === 200 && isset($resEstatistikak['data']['data'])) {
        $estatistikak = $resEstatistikak['data']['data'];
    } else {
        $estatistikak['guztiraIritziak'] = count($iritziak);
    }
}

// 3. FLASH MEZUAK BERRESKURATU ETA GARBITU
$flash_success = $_SESSION['flash_success'] ?? null;
$flash_conflict = $_SESSION['flash_conflict'] ?? null;
$flash_error = $_SESSION['flash_error'] ?? null;
unset($_SESSION['flash_success'], $_SESSION['flash_conflict'], $_SESSION['flash_error']);

$flash_iritzi_success = $_SESSION['flash_iritzi_success'] ?? null;
$flash_iritzi_error = $_SESSION['flash_iritzi_error'] ?? null;
unset($_SESSION['flash_iritzi_success'], $_SESSION['flash_iritzi_error']);

// 4. IKUSPEGI-OSAGAIAK (VIEW COMPONENTS) RENDERIZATU
require_once __DIR__ . '/assets/php/header.php';
require_once __DIR__ . '/assets/php/hero.php';
?>

<div class="container">
    <?php
    require_once __DIR__ . '/assets/php/karta.php';
    require_once __DIR__ . '/assets/php/erreserbak.php';
    require_once __DIR__ . '/assets/php/iritziak.php';
    require_once __DIR__ . '/assets/php/kontaktua.php';
    ?>
</div>

<?php
require_once __DIR__ . '/assets/php/footer.php';
