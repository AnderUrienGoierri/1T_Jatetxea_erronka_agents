$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = [System.IO.Path]::GetFullPath((Join-Path $scriptDir "..\..\.."))
$launcher = Join-Path $projectRoot "Admin_IT_Zentrala.exe"

if (-not (Test-Path -LiteralPath $launcher)) {
    throw "Admin IT Zentrala abiarazlea ez da aurkitu: $launcher"
}

Start-Process -FilePath $launcher -WorkingDirectory $projectRoot
