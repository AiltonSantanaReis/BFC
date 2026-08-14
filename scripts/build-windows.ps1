param(
    [string]$UnityPath = "C:\Program Files\Unity\Hub\Editor\6000.3.21f1\Editor\Unity.exe"
)

$ErrorActionPreference = "Stop"
$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$LogDirectory = Join-Path $ProjectRoot "Logs"
$LogFile = Join-Path $LogDirectory "build-windows.log"
$ExecutablePath = Join-Path $ProjectRoot "Builds\Windows\BFC.exe"

if (-not (Test-Path $UnityPath)) {
    throw "Unity 6000.3.21f1 was not found at: $UnityPath"
}

New-Item -ItemType Directory -Force -Path $LogDirectory | Out-Null
Remove-Item $LogFile -Force -ErrorAction SilentlyContinue
Remove-Item $ExecutablePath -Force -ErrorAction SilentlyContinue

$arguments = @(
    "-batchmode",
    "-nographics",
    "-quit",
    "-projectPath", "`"$ProjectRoot`"",
    "-buildTarget", "StandaloneWindows64",
    "-executeMethod", "BFC.Editor.Build.BfcBuild.BuildWindows64",
    "-logFile", "`"$LogFile`""
)

$process = Start-Process `
    -FilePath $UnityPath `
    -ArgumentList $arguments `
    -Wait `
    -PassThru

if ($process.ExitCode -ne 0) {
    throw "Unity Windows build failed with exit code $($process.ExitCode). See $LogFile"
}

if (-not (Test-Path $ExecutablePath)) {
    throw "Unity returned exit code 0 but BFC.exe was not created at $ExecutablePath. The build is NOT valid. See $LogFile"
}

$executable = Get-Item $ExecutablePath
if ($executable.Length -le 0) {
    throw "BFC.exe exists but is empty. The build is NOT valid: $ExecutablePath"
}

Write-Host "BFC Windows build completed and verified: $ExecutablePath ($($executable.Length) bytes)"
