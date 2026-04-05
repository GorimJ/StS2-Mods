# Build installer.py into a standalone .exe using PyInstaller.
# Requires: pip install pyinstaller
#
# Output: dist/installer.exe
# The mods/ folder must be placed alongside installer.exe before distributing.

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "Building Slay the Spire 2 Mod Installer..." -ForegroundColor Cyan

# Check PyInstaller is available
if (-not (Get-Command pyinstaller -ErrorAction SilentlyContinue)) {
    Write-Host "PyInstaller not found. Installing..." -ForegroundColor Yellow
    pip install pyinstaller
}

Push-Location $ScriptDir
try {
    pyinstaller `
        --onefile `
        --windowed `
        --name "StS2ModInstaller" `
        --icon "icon.ico" `
        installer.py
} catch {
    # --icon is optional; retry without it if the file doesn't exist
    pyinstaller `
        --onefile `
        --windowed `
        --name "StS2ModInstaller" `
        installer.py
}
Pop-Location

Write-Host ""
Write-Host "Build complete: Sts2ModDeployer\dist\StS2ModInstaller.exe" -ForegroundColor Green
Write-Host "Run package.ps1 to collect mod files and create a distributable zip." -ForegroundColor DarkGray
