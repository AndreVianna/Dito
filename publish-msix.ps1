# VivaVoz MSIX Build Script (run on Windows)
# Prerequisites: .NET 10 SDK, Windows 10 SDK
# Usage: .\publish-msix.ps1 [-Arch x64|arm64]

param(
    [ValidateSet("x64", "arm64")]
    [string]$Arch = "x64"
)

$ErrorActionPreference = "Stop"

$ProjectDir = "source\VivaVoz"
$OutputDir = "publish\msix"
$AppDir = "$OutputDir\app"
$RuntimeId = "win-$Arch"

Write-Host "Building VivaVoz MSIX package ($RuntimeId)..." -ForegroundColor Cyan

# 1. Clean previous output
if (Test-Path $AppDir) {
    Remove-Item $AppDir -Recurse -Force
}

# 2. Publish self-contained for target architecture
Write-Host "Publishing for $RuntimeId..." -ForegroundColor Yellow
dotnet publish "$ProjectDir\VivaVoz.csproj" `
    -c Release `
    -r $RuntimeId `
    --self-contained true `
    -p:PublishSingleFile=false `
    -p:PublishTrimmed=false `
    -o $AppDir

# 3. Fix native library layout for MSIX
# MSIX needs native DLLs in the app root, not in runtimes/ subfolder
Write-Host "Fixing native library layout..." -ForegroundColor Yellow

$nativeDir = "$AppDir\runtimes\$RuntimeId\native"
if (Test-Path $nativeDir) {
    Copy-Item "$nativeDir\*" $AppDir -Force
    Write-Host "  Copied native DLLs from $nativeDir to app root" -ForegroundColor Green
} else {
    Write-Warning "Native directory not found: $nativeDir"
    Write-Warning "Checking if DLLs are already in app root..."
    $whisperDll = Get-ChildItem $AppDir -Filter "whisper.dll" -ErrorAction SilentlyContinue
    if (-not $whisperDll) {
        Write-Error "whisper.dll not found anywhere in output. Build will fail at runtime."
        exit 1
    }
}

# Remove all runtimes/ folder (linux, macos, other archs — not needed)
if (Test-Path "$AppDir\runtimes") {
    Remove-Item "$AppDir\runtimes" -Recurse -Force
    Write-Host "  Removed runtimes/ folder (unused platforms)" -ForegroundColor Green
}

# Remove macOS Metal shader if present
if (Test-Path "$AppDir\ggml-metal.metal") {
    Remove-Item "$AppDir\ggml-metal.metal" -Force
    Write-Host "  Removed ggml-metal.metal (macOS only)" -ForegroundColor Green
}

# 4. Copy Whisper Base model
$modelsDir = "$AppDir\models"
if (-not (Test-Path $modelsDir)) { New-Item -ItemType Directory -Path $modelsDir | Out-Null }
if (Test-Path "models\ggml-base.bin") {
    Copy-Item "models\ggml-base.bin" "$modelsDir\ggml-base.bin"
    Write-Host "  Whisper Base model bundled." -ForegroundColor Green
} else {
    Write-Warning "ggml-base.bin not found in models/. Download from HuggingFace first."
    Write-Warning "  https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-base.bin"
}

# 5. Copy AppxManifest
Copy-Item "$ProjectDir\Package.appxmanifest" "$AppDir\AppxManifest.xml" -Force
Write-Host "  AppxManifest.xml copied." -ForegroundColor Green

# 6. Verify native DLLs are in place
Write-Host ""
Write-Host "Verifying native libraries..." -ForegroundColor Yellow
$requiredDlls = @("whisper.dll", "ggml-base-whisper.dll", "ggml-cpu-whisper.dll", "ggml-whisper.dll")
$allFound = $true
foreach ($dll in $requiredDlls) {
    if (Test-Path "$AppDir\$dll") {
        Write-Host "  [OK] $dll" -ForegroundColor Green
    } else {
        Write-Host "  [MISSING] $dll" -ForegroundColor Red
        $allFound = $false
    }
}

if (-not $allFound) {
    Write-Error "Missing native DLLs. MSIX will fail at runtime."
    exit 1
}

# 7. Create MSIX
Write-Host ""
Write-Host "Creating MSIX package..." -ForegroundColor Yellow

# Find makeappx.exe
$sdkPaths = @(
    "C:\Program Files (x86)\Windows Kits\10\bin\10.0.26100.0\x64\makeappx.exe",
    "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\makeappx.exe",
    "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22000.0\x64\makeappx.exe"
)

$makeappx = $null
foreach ($path in $sdkPaths) {
    if (Test-Path $path) {
        $makeappx = $path
        break
    }
}

if (-not $makeappx) {
    Write-Error "makeappx.exe not found. Install Windows 10 SDK."
    exit 1
}

$msixPath = "$OutputDir\VivaVoz-$Arch.msix"
& $makeappx pack /d $AppDir /p $msixPath /l /o

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "SUCCESS: $msixPath" -ForegroundColor Green
    $size = [math]::Round((Get-Item $msixPath).Length / 1MB, 1)
    Write-Host "  Size: ${size} MB" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Test locally: Add-AppPackage -Path $msixPath"
    Write-Host "  2. Upload to Microsoft Partner Center for Store distribution"
} else {
    Write-Error "makeappx.exe failed with exit code $LASTEXITCODE"
    exit 1
}
