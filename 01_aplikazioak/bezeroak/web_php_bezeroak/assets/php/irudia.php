<?php
require_once __DIR__ . '/api.php';

$key = $_GET['key'] ?? '';
if (!is_string($key) || !preg_match('~\A(?:platerak/[a-z0-9-]+\.jpg|kredituak\.html)\z~', $key)) {
    http_response_code(400);
    exit('Irudi-erreferentzia baliogabea.');
}

$apiRoot = preg_replace('~/api/v1/?$~', '', $api_base_url);
$url = $apiRoot . '/media/public/' . $key;
$context = stream_context_create(['http' => [
    'timeout' => 5,
    'ignore_errors' => true,
    'follow_location' => 0,
]]);
$content = @file_get_contents($url, false, $context);
$statusLine = $http_response_header[0] ?? '';
if ($content === false || !preg_match('~^HTTP/\S+ 200\b~', $statusLine)) {
    error_log('Ezin izan da irudia APItik berreskuratu: ' . $key);
    http_response_code(502);
    exit('Irudia ez dago erabilgarri.');
}

header($key === 'kredituak.html' ? 'Content-Type: text/html; charset=utf-8' : 'Content-Type: image/jpeg');
header('X-Content-Type-Options: nosniff');
header('Cache-Control: public, max-age=300');
echo $content;
