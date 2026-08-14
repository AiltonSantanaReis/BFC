param(
    [string]$UnityPath = "C:\Program Files\Unity\Hub\Editor\6000.3.21f1\Editor\Unity.exe"
)

$ErrorActionPreference = "Stop"
$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$LogDirectory = Join-Path $ProjectRoot "Logs"
$LogFile = Join-Path $LogDirectory "build-windows.log"

if (-not (Test-Path $UnityPath)) {
    throw "Unity 6000.3.21f1 was not found at: $UnityPath"
}

New-Item -ItemType Directory -Force -Path $LogDirectory | Out-Null

& $UnityPath `
    -batchmode `
    -nographics `
    -quit `
    -projectPath $ProjectRoot `
    -buildTarget StandaloneWindows64 `
    -executeMethod BFC.Editor.Build.BfcBuild.BuildWindows64 `
    -logFile $LogFile

if ($LASTEXITCODE -ne 0) {
    throw "Unity Windows build failed with exit code $LASTEXITCODE. See $LogFile"
}

Write-Host "BFC Windows build completed: $ProjectRoot\Builds\Windows\BFC.exe"
