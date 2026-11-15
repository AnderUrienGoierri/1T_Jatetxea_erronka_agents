# ============================================================================
# 1T Jatetxea - ASP.NET Core REST API & Datu-Base Monitorea (Denbora Errealean)
# ============================================================================
$host.UI.RawUI.WindowTitle = "1T Jatetxea - ASP.NET Core REST API (Portua 5050)"
$host.UI.RawUI.BackgroundColor = "Black"
$host.UI.RawUI.ForegroundColor = "White"

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$apiUrl = "http://localhost:5050/health"
$apiBaseUrl = "http://localhost:5050/"
$appsOnlineUrl = "http://localhost:5050/api/v1/aplikazioak-linean"
$dbHost = "localhost:3308"
$dbName = "jatetxea_db"

while ($true) {
    $nowStr = (Get-Date).ToString("HH:mm:ss")
    $isHealthy = $false
    $apiRunning = $false
    $errDetail = ""
    $dbStatusText = ""

    # 1. Health check kontsultatu APIari (2 segundoko muga)
    try {
        $req = [System.Net.HttpWebRequest]::Create($apiUrl)
        $req.Timeout = 2000
        $req.Method = "GET"
        $resp = $req.GetResponse()
        $statusCode = [int]$resp.StatusCode

        if ($statusCode -eq 200) {
            $apiRunning = $true
            $isHealthy = $true
            $dbStatusText = "KONEKTATUTA"
        }
        $resp.Close()
    }
    catch [System.Net.WebException] {
        if ($_.Exception.Response) {
            $apiRunning = $true
            $isHealthy = $false
            $resp = $_.Exception.Response
            $statusCode = [int]$resp.StatusCode

            try {
                $stream = $resp.GetResponseStream()
                $reader = New-Object System.IO.StreamReader($stream)
                $body = $reader.ReadToEnd()
                if ($body -match '"errorea"\s*:\s*"([^"]+)"') {
                    $errDetail = $matches[1]
                } else {
                    $errDetail = "MySQL konexioa eten da edo zerbitzaria erantzunik gabe dago."
                }
            } catch {
                $errDetail = "MySQL konexioa eten da."
            }
            $resp.Close()
        } else {
            $apiRunning = $false
            $isHealthy = $false
            $errDetail = "Ezin izan da REST APIarekin konektatu (http://localhost:5050/)."
        }
    }
    catch {
        $apiRunning = $false
        $isHealthy = $false
        $errDetail = $_.Exception.Message
    }

    # 2. Pantaila garbitu errepikapenak ekiditeko eta uneko egoera soilik erakusteko
    Clear-Host

    Write-Host "================================================================================" -ForegroundColor White
    Write-Host "  1T JATETXEA - ASP.NET CORE REST API (PORTUA: 5050)                            " -ForegroundColor White
    Write-Host "  MySQL Zerbitzaria: $dbHost ($dbName) | REST API: $apiBaseUrl" -ForegroundColor Cyan
    Write-Host "  Denbora Errealeko Konexioaren eta Aplikazioen Monitorea                       " -ForegroundColor Gray
    Write-Host "================================================================================" -ForegroundColor White
    Write-Host ""

    # 3. Datu-basearen egoeraren arabera kolore bereizgarriak (Berdea / Gorria / Horia)
    if ($isHealthy) {
        Write-Host "================================================================================" -ForegroundColor Green
        Write-Host "  [$nowStr] [DATU-BASEA OK] MySQL konexioa ondo ezarri da!                      " -ForegroundColor Green
        Write-Host "  -> Egoera: KONEKTATUTA (Datu-basea erabilgarri eta aktibo)                     " -ForegroundColor Green
        Write-Host "  -> Zerbitzaria: $dbHost | Datu-basea: $dbName                      " -ForegroundColor Green
        Write-Host "  -> REST API: $apiBaseUrl (Martxan eta eskaerak hartzeko prest)         " -ForegroundColor Green
        Write-Host "================================================================================" -ForegroundColor Green
    }
    elseif ($apiRunning) {
        Write-Host "================================================================================" -ForegroundColor Red
        Write-Host "  [$nowStr] [ADI! DATU-BASEA ITZALI DA / DESKONEKTATUTA]                       " -ForegroundColor Red
        Write-Host "  -> Kausa: Ezin izan da MySQL datu-basearekin konektatu ($dbHost).             " -ForegroundColor Red
        if (![string]::IsNullOrWhiteSpace($errDetail)) {
            Write-Host "  -> Errore mezua: $errDetail" -ForegroundColor Red
        }
        Write-Host "  -> Konponbidea: 'docker compose up -d jatetxea-db' exekutatu datu-basea pizteko " -ForegroundColor Red
        Write-Host "================================================================================" -ForegroundColor Red
    }
    else {
        Write-Host "================================================================================" -ForegroundColor Yellow
        Write-Host "  [$nowStr] [KONTUZ! REST APIA EZ DAGO MARTXAN EDO ABIARAZTEN ARI DA]           " -ForegroundColor Yellow
        Write-Host "  -> Kausa: $errDetail" -ForegroundColor Yellow
        Write-Host "  -> Konponbidea: IT Kontrol Zentroan 'Piztu' sakatu edo 'docker compose up -d' " -ForegroundColor Yellow
        Write-Host "================================================================================" -ForegroundColor Yellow
    }

    # 4. KONEKTATUTAKO APLIKAZIOAK (DENBORA ERREALEAN APIA ERABILTZEN)
    Write-Host ""
    Write-Host "Konektatutako Aplikazioak (Denbora Errealean APIa Erabiltzen):" -ForegroundColor Cyan
    Write-Host "--------------------------------------------------------------------------------" -ForegroundColor DarkGray

    try {
        $appsReq = [System.Net.HttpWebRequest]::Create($appsOnlineUrl)
        $appsReq.Timeout = 1200
        $appsReq.Method = "GET"
        $appsResp = $appsReq.GetResponse()
        $appsStream = $appsResp.GetResponseStream()
        $appsReader = New-Object System.IO.StreamReader($appsStream)
        $appsJson = $appsReader.ReadToEnd()
        $appsResp.Close()

        $appsData = $appsJson | ConvertFrom-Json

        if ($appsData -and $appsData.aplikazioak) {
            foreach ($app in $appsData.aplikazioak) {
                # Formatua: [EGOERA] Izena | IP | Azkena: ...
                $izenaZabal = ($app.izena).PadRight(31)

                if ($app.egoera -eq "AKTIBO") {
                    Write-Host "  [AKTIBO]     " -ForegroundColor Green -NoNewline
                    Write-Host "$izenaZabal " -ForegroundColor White -NoNewline
                    Write-Host "| IP: $($app.ip) | Azkena: $($app.azkenEskaera) ($($app.azkenOrdua)) | $($app.eskaerakGuztira) eskaera" -ForegroundColor Green
                }
                elseif ($app.egoera -eq "ISILIK") {
                    Write-Host "  [ISILIK]     " -ForegroundColor Yellow -NoNewline
                    Write-Host "$izenaZabal " -ForegroundColor Gray -NoNewline
                    Write-Host "| IP: $($app.ip) | Azkena: $($app.azkenEskaera) ($($app.azkenOrdua)) | $($app.eskaerakGuztira) eskaera" -ForegroundColor DarkYellow
                }
                else {
                    Write-Host "  [DESKONEKT.] " -ForegroundColor DarkGray -NoNewline
                    Write-Host "$izenaZabal " -ForegroundColor DarkGray -NoNewline
                    Write-Host "| Ez da oraindik konektatu" -ForegroundColor DarkGray
                }
            }
        } else {
            Write-Host "  (Ez dago aplikaziorik erregistratuta)" -ForegroundColor DarkGray
        }
    }
    catch {
        Write-Host "  (Ezin izan da aplikazioen egoera lortu - REST APIa ez dago erabilgarri)" -ForegroundColor DarkGray
    }

    Write-Host "--------------------------------------------------------------------------------" -ForegroundColor DarkGray

    # 5. Azken API Logak eta Bezeroen Eskaerak (docker logs)
    Write-Host ""
    Write-Host "Azken API Logak eta Bezeroen Eskaerak (docker logs):" -ForegroundColor Gray
    Write-Host "--------------------------------------------------------------------------------" -ForegroundColor DarkGray

    try {
        $rawLogs = docker logs --tail 30 jatetxea_api 2>$null
        $filtered = $rawLogs | Where-Object {
            $_ -notmatch "^====" -and
            $_ -notmatch "\[DATU-BASEA" -and
            $_ -notmatch "\[ADI! DATU-BASEA" -and
            $_ -notmatch "^\s*->\s*(Zerbitzaria|Datu-basea|Egoera|Kausa|Errore|Konponbidea|APIak)" -and
            $_ -notmatch "Overriding HTTP_PORTS"
        } | Select-Object -Last 8

        if ($filtered -and $filtered.Count -gt 0) {
            foreach ($line in $filtered) {
                Write-Host "  $line" -ForegroundColor White
            }
        } else {
            Write-Host "  (Ez dago eskaera berririk une honetan)" -ForegroundColor DarkGray
        }
    } catch {
        Write-Host "  (Ezin izan dira Docker logak irakurri)" -ForegroundColor DarkGray
    }

    Write-Host "--------------------------------------------------------------------------------" -ForegroundColor DarkGray
    Write-Host " [Freskatze automatikoa 2 segundoro aktibo. Leihoa ixteko sakatu Ctrl+C] " -ForegroundColor DarkCyan

    Start-Sleep -Seconds 2
}
