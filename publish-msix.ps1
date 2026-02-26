# VivaVoz MSIX Build Script (run on Windows)
# Prerequisites: .NET 10 SDK, Windows 10 SDK

$ErrorActionPreference = "Stop"

$ProjectDir = "source\VivaVoz"
$OutputDir = "publish\msix"

Write-Host "Building VivaVoz MSIX package..." -ForegroundColor Cyan

# 1. Publish self-contained
dotnet publish $ProjectDir\VivaVoz.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    -p:PublishTrimmed=false `
    -o "$OutputDir\app"

# 2. Copy Whisper Base model
$modelsDir = "$OutputDir\app\models"
if (-not (Test-Path $modelsDir)) { New-Item -ItemType Directory -Path $modelsDir }
# NOTE: Download ggml-base.bin from https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-base.bin
# and place it in the models/ directory before running this script
if (Test-Path "models\ggml-base.bin") {
    Copy-Item "models\ggml-base.bin" "$modelsDir\ggml-base.bin"
    Write-Host "Whisper Base model bundled." -ForegroundColor Green
} else {
    Write-Warning "ggml-base.bin not found in models/. Download from HuggingFace first."
}

# 3. Create MSIX (requires Windows SDK makeappx.exe)
# Update the mapping file and run:
# & "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\makeappx.exe" pack /d "$OutputDir\app" /p "$OutputDir\VivaVoz.msix" /l

Write-Host ""
Write-Host "Published to: $OutputDir\app" -ForegroundColor Green
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Run makeappx.exe to create .msix from the app folder"
Write-Host "  2. Sign with signtool.exe or submit unsigned to Partner Center"
Write-Host "  3. Upload to Microsoft Partner Center for Store distribution"
