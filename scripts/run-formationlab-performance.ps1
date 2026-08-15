param(
    [string]$UnityPath = "C:\Program Files\Unity\Hub\Editor\6000.3.21f1\Editor\Unity.exe",
    [int]$Width = 1280,
    [int]$Height = 720,
    [double]$WarmupSeconds = 2,
    [double]$SampleSeconds = 10,
    [int]$TimeoutSeconds = 60
)

$ErrorActionPreference = "Stop"
$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$LogDirectory = Join-Path $ProjectRoot "Logs"
$BuildLog = Join-Path $LogDirectory "build-formationlab-windows.log"
$PlayerLog = Join-Path $LogDirectory "formationlab-performance.log"
$ExecutablePath = Join-Path $ProjectRoot "Builds\FormationLab\BFC-FormationLab.exe"

if (-not (Test-Path $UnityPath)) {
    throw "Unity 6000.3.21f1 was not found at: $UnityPath"
}

if ($Width -le 0 -or $Height -le 0) {
    throw "Width and Height must be positive integers."
}

if ($WarmupSeconds -le 0 -or $SampleSeconds -le 0) {
    throw "WarmupSeconds and SampleSeconds must be positive."
}

if ($TimeoutSeconds -le 0) {
    throw "TimeoutSeconds must be positive."
}

New-Item -ItemType Directory -Force -Path $LogDirectory | Out-Null
Remove-Item $BuildLog -Force -ErrorAction SilentlyContinue
Remove-Item $PlayerLog -Force -ErrorAction SilentlyContinue
Remove-Item $ExecutablePath -Force -ErrorAction SilentlyContinue

Write-Host "Building FormationLab Windows performance player..."

$buildArguments = @(
    "-batchmode",
    "-nographics",
    "-quit",
    "-projectPath", "`"$ProjectRoot`"",
    "-buildTarget", "StandaloneWindows64",
    "-executeMethod", "BFC.Editor.Build.BfcBuild.BuildFormationLabWindows64",
    "-logFile", "`"$BuildLog`""
)

$buildProcess = Start-Process `
    -FilePath $UnityPath `
    -ArgumentList $buildArguments `
    -Wait `
    -PassThru

if ($buildProcess.ExitCode -ne 0) {
    throw "FormationLab Windows build failed with exit code $($buildProcess.ExitCode). See $BuildLog"
}

if (-not (Test-Path $ExecutablePath)) {
    throw "Unity returned exit code 0 but the FormationLab executable was not created at $ExecutablePath. See $BuildLog"
}

$executable = Get-Item $ExecutablePath
if ($executable.Length -le 0) {
    throw "FormationLab executable exists but is empty: $ExecutablePath"
}

$warmupInvariant = $WarmupSeconds.ToString([System.Globalization.CultureInfo]::InvariantCulture)
$sampleInvariant = $SampleSeconds.ToString([System.Globalization.CultureInfo]::InvariantCulture)

$playerArguments = @(
    "-screen-fullscreen", "0",
    "-screen-width", "$Width",
    "-screen-height", "$Height",
    "-logFile", "`"$PlayerLog`"",
    "-bfcFormationPerf",
    "-bfcPerfWarmupSeconds=$warmupInvariant",
    "-bfcPerfSampleSeconds=$sampleInvariant"
)

Write-Host "Running FormationLab performance capture at ${Width}x${Height}..."

$startInfo = New-Object System.Diagnostics.ProcessStartInfo
$startInfo.FileName = $ExecutablePath
$startInfo.Arguments = ($playerArguments -join " ")
$startInfo.UseShellExecute = $false

$playerProcess = [System.Diagnostics.Process]::Start($startInfo)
if ($null -eq $playerProcess) {
    throw "Could not start FormationLab performance player."
}

$completed = $playerProcess.WaitForExit($TimeoutSeconds * 1000)
if (-not $completed) {
    try {
        $playerProcess.Kill()
    }
    catch {
    }

    throw "FormationLab performance player timed out after $TimeoutSeconds seconds. See $PlayerLog"
}

if ($playerProcess.ExitCode -ne 0) {
    throw "FormationLab performance player exited with code $($playerProcess.ExitCode). See $PlayerLog"
}

if (-not (Test-Path $PlayerLog)) {
    throw "FormationLab player completed but did not create $PlayerLog"
}

$result = Select-String -Path $PlayerLog -Pattern '\[BFC FormationPerf\] RESULT' | Select-Object -Last 1
if ($null -eq $result) {
    throw "FormationLab player completed but no performance RESULT line was found. See $PlayerLog"
}

Write-Host ""
Write-Host "FormationLab Windows performance capture completed:"
Write-Host $result.Line
Write-Host "Player log: $PlayerLog"
Write-Host "Build log:  $BuildLog"
