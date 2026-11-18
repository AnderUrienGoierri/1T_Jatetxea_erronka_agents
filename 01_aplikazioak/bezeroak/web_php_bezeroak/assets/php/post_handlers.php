<?php
/**
 * JATETXEA - Formularioen Bidalketak Kudeatzeko Handlerra (post_handlers.php)
 * POST eskaerak prozesatu, C# REST APIari bidali eta flash mezuekin berbideratu.
 *
 * @var string $api_base_url
 */
require_once __DIR__ . '/api.php';

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $action = $_POST['action'] ?? '';

    // 1. ERRESERBA BERRIA SORTU
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

    // 2. BEZEROAREN IRITZI BERRIA GORDE
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
