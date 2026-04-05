# Build installer.py into a standalone .exe using PyInstaller.
# Requires: pip install pyinstaller  (auto-installed if missing)
#
# Output: dist/StS2ModInstaller.exe
# The mods/ folder must be placed alongside the .exe before distributing.

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "Building Slay the Spire 2 Mod Installer..." -ForegroundColor Cyan

if (-not (Get-Command pyinstaller -ErrorAction SilentlyContinue)) {
    Write-Host "PyInstaller not found. Installing..." -ForegroundColor Yellow
    pip install pyinstaller
}

Push-Location $ScriptDir

pyinstaller `
    --onefile `
    --windowed `
    --name "StS2ModInstaller" `
    installer.py

Pop-Location

Write-Host ""
Write-Host "Build complete: Sts2ModDeployer\dist\StS2ModInstaller.exe" -ForegroundColor Green
