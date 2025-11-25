# Post-build obfuscation script using ConfuserEx
# This encrypts all strings in the compiled assembly

$ErrorActionPreference = "SilentlyContinue"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$buildDir = Join-Path $scriptDir "bin\Release\net451"
$outputDir = Join-Path $scriptDir "bin\obfuscated"
$configFile = Join-Path $scriptDir "ConfuserEx.crproj"
$dllName = "Jelly.exe"

# Check if assembly exists
$dllPath = Join-Path $buildDir $dllName
if (-not (Test-Path $dllPath)) {
    Write-Host "Assembly not found at: $dllPath"
    exit 0
}

# Check for ConfuserEx
$confuserPath = "C:\Program Files\ConfuserEx\ConfuserEx.CLI.exe"
$confuserPath64 = "C:\Program Files (x86)\ConfuserEx\ConfuserEx.CLI.exe"

if (-not (Test-Path $confuserPath) -and -not (Test-Path $confuserPath64)) {
    Write-Host "ConfuserEx not found. Install it from: https://github.com/yck1509/ConfuserEx/releases"
    Write-Host "Or run: choco install confuserex"
    exit 0
}

$confuserExe = if (Test-Path $confuserPath) { $confuserPath } else { $confuserPath64 }

# Create output directory
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir | Out-Null
}

Write-Host "Starting obfuscation with ConfuserEx..."
Write-Host "Input: $dllPath"
Write-Host "Config: $configFile"

# Run ConfuserEx
& $confuserExe $configFile

# Check if successful
$obfuscatedDll = Join-Path $outputDir $dllName
if (Test-Path $obfuscatedDll) {
    Write-Host "Obfuscation successful!"
    Write-Host "Replacing original DLL..."
    Copy-Item $obfuscatedDll $dllPath -Force
    Write-Host "Done! DLL has been obfuscated and encrypted."
} else {
    Write-Host "Obfuscation failed or output not found"
}
