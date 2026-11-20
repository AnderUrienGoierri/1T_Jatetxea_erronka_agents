<#
.SYNOPSIS
    Microsoft Planner-era zereginak automatikoki inportatzeko PowerShell script-a (Microsoft Graph bidez).

.DESCRIPTION
    Script honek 'planner_zeregin_guztiak.csv' fitxategia irakurtzen du eta Microsoft Graph bidez
    zure Teams Planner-eko planean zereginak sortzen ditu.
    ADIMANTSUA DA: Dagoeneko sortuta dauden kartak detektatzen ditu bikoiztu ez daitezen!
#>

param(
    [string]$CsvPath = "$PSScriptRoot\planner_zeregin_guztiak.csv",
    [string]$GroupName = "2MG3 Ikasleak 2627",
    [string]$PlanTitle = "Planifikazioa",
    [switch]$EzabatuDaudenakLehenago = $false
)

if (-not (Test-Path $CsvPath)) {
    Write-Error "Ez da aurkitu CSV fitxategia: $CsvPath"
    exit
}

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   1T JATETXEA - TEAMS PLANNER INPORTATZAILE AUTOMATIKOA   " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Konexioa
Write-Host "`n1. Microsoft Graph-era konektatzen..." -ForegroundColor Yellow
Connect-MgGraph -Scopes "Group.Read.All", "Tasks.ReadWrite"

# 2. Taldea bilatu
Write-Host "2. Taldea bilatzen: '$GroupName'..." -ForegroundColor Yellow
$group = Get-MgGroup -Filter "displayName eq '$GroupName'"
if (-not $group) {
    Write-Warning "Ez da taldea izen zehatzez aurkitu. Zure taldeak:"
    Get-MgGroup | Select-Object Id, DisplayName | Format-Table
    $groupId = Read-Host "Kopiatu eta itsatsi hemen zure Taldearen (Group) ID-a"
} else {
    $groupId = $group.Id
    Write-Host "   -> Taldea aurkituta: $($group.DisplayName) ($groupId)" -ForegroundColor Green
}

# 3. Plana bilatu
Write-Host "3. Plana bilatzen: '$PlanTitle'..." -ForegroundColor Yellow
$plan = Get-MgPlannerPlan -GroupId $groupId | Where-Object { $_.Title -eq $PlanTitle }
if (-not $plan) {
    Write-Warning "Ez da '$PlanTitle' izeneko plana aurkitu. Taldeko planak:"
    Get-MgPlannerPlan -GroupId $groupId | Select-Object Id, Title | Format-Table
    $planId = Read-Host "Kopiatu eta itsatsi hemen Plana-ren ID-a"
} else {
    $planId = $plan.Id
    Write-Host "   -> Plana aurkituta: $($plan.Title) ($planId)" -ForegroundColor Green
}

# 4. Depositoak (Buckets) eskuratu
Write-Host "4. Zutabeak / Depositoak (Buckets) kargatzen..." -ForegroundColor Yellow
$buckets = Get-MgPlannerPlanBucket -PlannerPlanId $planId
$bucketMap = @{}
foreach ($b in $buckets) {
    $bucketMap[$b.Name.Trim().ToUpper()] = $b.Id
}

Write-Host "   -> Aurkitutako zutabeak:" -ForegroundColor Green
$bucketMap.Keys | ForEach-Object { Write-Host "      - $_" }

# Egiaztatu 6 faseak dauden
$faseak = @("ANALISIA", "DISEINUA", "GARAPENA", "FROGAK", "MARTXAN JARTZEA", "MANTENTZEA")
foreach ($f in $faseak) {
    if (-not $bucketMap.ContainsKey($f)) {
        Write-Host "   [+] Sortzen falta den zutabea: $f..." -ForegroundColor Magenta
        $newB = New-MgPlannerBucket -PlannerPlanId $planId -Name $f
        $bucketMap[$f] = $newB.Id
    }
}

# 5. Dagoeneko planifikazioan dauden zereginak eskuratu (Bikoizketak ekiditeko!)
Write-Host "5. Dagoeneko existitzen diren zereginak aztertzen..." -ForegroundColor Yellow
$existingTasks = Get-MgPlannerPlanTask -PlannerPlanId $planId
Write-Host "   -> Gaur egun existitzen diren kartak: $($existingTasks.Count)" -ForegroundColor Cyan

# 6. CSV irakurri eta zereginak sortu
$tasksToImport = Import-Csv -Path $CsvPath -Encoding UTF8
Write-Host "`n6. Zereginak inportatzen (Guztira: $($tasksToImport.Count))..." -ForegroundColor Yellow

$sortuak = 0
$saltsatuak = 0
$eguneratuak = 0

foreach ($t in $tasksToImport) {
    $bucketName = $t.'Fasea / Zutabea (Bucket)'.Trim().ToUpper()
    $bucketId = $bucketMap[$bucketName]
    $title = $t.'Karta Titulua (Task Name)'.Trim()

    if (-not $bucketId) {
        Write-Warning "Ez da aurkitu zutabea: $bucketName. Saltatzen: $title"
        continue
    }

    $startDate = $null
    $dueDate = $null
    if ($t.'Hasiera Data (Start Date)') {
        $startDate = [DateTime]::ParseExact($t.'Hasiera Data (Start Date)', 'yyyy/MM/dd', $null)
    }
    if ($t.'Amaiera Data (Due Date)') {
        $dueDate = [DateTime]::ParseExact($t.'Amaiera Data (Due Date)', 'yyyy/MM/dd', $null)
    }

    # Konprobatu ea jada existitzen den zeregin hau zutabe honetan
    $dagoena = $existingTasks | Where-Object { 
        $_.BucketId -eq $bucketId -and ($_.Title.Trim().ToLower() -eq $title.ToLower() -or $_.Title.Trim().ToLower() -like "*$($title.ToLower())*") 
    } | Select-Object -First 1

    if ($dagoena) {
        # Jada existitzen da: Ez dugu berriro bikoiztuko! Datak eguneratuko dizkiogu
        try {
            $updateParams = @{}
            if ($dueDate) { $updateParams.dueDateTime = $dueDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") }
            if ($startDate) { $updateParams.startDateTime = $startDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") }
            
            if ($updateParams.Count -gt 0) {
                Update-MgPlannerTask -PlannerTaskId $dagoena.Id -IfMatch $dagoena.AdditionalProperties['@odata.etag'] -BodyParameter $updateParams | Out-Null
                $eguneratuak++
                Write-Host " [EGUNERATUA (Datak)] $title ($bucketName)" -ForegroundColor Yellow
            } else {
                $saltsatuak++
                Write-Host " [EXISTITZEN DA] $title ($bucketName) -> Bikoizketa ekidinda" -ForegroundColor Gray
            }
        } catch {
            $saltsatuak++
            Write-Host " [EXISTITZEN DA] $title ($bucketName)" -ForegroundColor Gray
        }
    } else {
        # Ez da existitzen: Sortu karta berria
        $params = @{
            planId = $planId
            bucketId = $bucketId
            title = $title
        }
        if ($dueDate) { $params.dueDateTime = $dueDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") }
        if ($startDate) { $params.startDateTime = $startDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") }

        try {
            New-MgPlannerTask -BodyParameter $params | Out-Null
            $sortuak++
            Write-Host " [BERRIA SORTUA] $title -> $bucketName" -ForegroundColor Green
        } catch {
            Write-Error " Errorea '$title' sortzerakoan: $_"
        }
    }
}

Write-Host "`n==========================================================" -ForegroundColor Cyan
Write-Host "   INPORTAZIOA AMAITUTA!" -ForegroundColor Green
Write-Host "   - Karta berriak sortuak: $sortuak" -ForegroundColor Green
Write-Host "   - Jada zeudenak eguneratuak: $eguneratuak" -ForegroundColor Yellow
Write-Host "   - Saltatutakoak (bikoizketarik ez): $saltsatuak" -ForegroundColor Gray
Write-Host "==========================================================" -ForegroundColor Cyan
