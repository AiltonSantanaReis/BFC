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

function Invoke-UnityAndWait {
    param(
        [string[]]$Arguments
    )

    $process = Start-Process `
        -FilePath $UnityPath `
        -ArgumentList $Arguments `
        -Wait `
        -PassThru

    return $process.ExitCode
}

$testPlatforms = @(
    @{ Name = "EditMode"; CliValue = "editmode" },
    @{ Name = "PlayMode"; CliValue = "playmode" }
)

foreach ($platform in $testPlatforms) {
    $name = $platform.Name
    $cliValue = $platform.CliValue
    $resultFile = Join-Path $ResultsDirectory "$name.xml"
    $logFile = Join-Path $LogDirectory "tests-$name.log"

    Remove-Item $resultFile -Force -ErrorAction SilentlyContinue
    Remove-Item $logFile -Force -ErrorAction SilentlyContinue

    Write-Host "Running BFC Unity $name tests..."

    $arguments = @(
        "-batchmode",
        "-nographics",
        "-projectPath", "`"$ProjectRoot`"",
        "-runTests",
        "-testPlatform", $cliValue,
        "-testResults", "`"$resultFile`"",
        "-logFile", "`"$logFile`""
    )

    $exitCode = Invoke-UnityAndWait -Arguments $arguments

    if ($exitCode -ne 0) {
        throw "Unity $name tests failed with exit code $exitCode. See $logFile"
    }

    if (-not (Test-Path $resultFile)) {
        throw "Unity $name returned exit code 0 but did not create $resultFile. The test run is NOT valid. See $logFile"
    }

    try {
        [xml]$results = Get-Content -Raw $resultFile
    }
    catch {
        throw "Unity $name produced an unreadable test result XML at $resultFile. See $logFile"
    }

    $testRun = $results.'test-run'
    if ($null -eq $testRun) {
        throw "Unity $name result XML has no <test-run> root. See $resultFile"
    }

    $total = [int]$testRun.total
    $passed = [int]$testRun.passed
    $failed = [int]$testRun.failed
    $result = [string]$testRun.result

    if ($total -lt 1) {
        throw "Unity $name completed without executing any tests. See $resultFile and $logFile"
    }

    if ($failed -ne 0 -or $result -ne "Passed") {
        throw "Unity $name tests did not pass: result=$result total=$total passed=$passed failed=$failed. See $resultFile and $logFile"
    }

    Write-Host "BFC Unity ${name}: PASS (total=$total, passed=$passed, failed=$failed)."
}

Write-Host "BFC Unity EditMode and PlayMode tests completed successfully with verified XML results."
