<?php
/**
 * JATETXEA - REST API Bezeroaren Funtzioak (api.php)
 * C# ASP.NET Core APIarekin komunikatzeko ardura du.
 */

// 1. APIaren Oinarrizko Helbidea
$api_base_url = getenv('API_BASE_URL');
if (!$api_base_url) {
    // Docker barruan bagaude host.docker.internal erabili, bestela localhost
    $api_base_url = (file_exists('/.dockerenv') || getenv('DOCKER_CONTAINER'))
        ? 'http://host.docker.internal:5000/api/v1'
        : 'http://localhost:5000/api/v1';
}

/**
 * C# REST APIari HTTP eskaera egiteko helper funtzio orokorra.
 *
 * @param string $method GET, POST, PUT, DELETE
 * @param string $url Eskaeraren helbide osoa
 * @param array|null $payload Bidali beharreko datuak (POST/PUT denean)
 * @return array Erantzunaren kodea, datuak eta erroreak
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

