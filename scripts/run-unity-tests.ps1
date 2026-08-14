param(
    [string]$UnityPath = "C:\Program Files\Unity\Hub\Editor\6000.3.21f1\Editor\Unity.exe"
)

$ErrorActionPreference = "Stop"
$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$ResultsDirectory = Join-Path $ProjectRoot "TestResults"
$LogDirectory = Join-Path $ProjectRoot "Logs"

if (-not (Test-Path $UnityPath)) {
    throw "Unity 6000.3.21f1 was not found at: $UnityPath"
}

New-Item -ItemType Directory -Force -Path $ResultsDirectory | Out-Null
New-Item -ItemType Directory -Force -Path $LogDirectory | Out-Null

$testPlatforms = @("EditMode", "PlayMode")
foreach ($platform in $testPlatforms) {
    $resultFile = Join-Path $ResultsDirectory "$platform.xml"
    $logFile = Join-Path $LogDirectory "tests-$platform.log"

    & $UnityPath `
        -batchmode `
        -nographics `
        -projectPath $ProjectRoot `
        -runTests `
        -testPlatform $platform `
        -testResults $resultFile `
        -logFile $logFile

    if ($LASTEXITCODE -ne 0) {
        throw "Unity $platform tests failed with exit code $LASTEXITCODE. See $logFile"
    }
}

Write-Host "BFC Unity EditMode and PlayMode tests completed successfully."
