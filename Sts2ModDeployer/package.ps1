# Packages the installer + latest built mods into a distributable zip.
#
# Steps:
#   1. Copies mod files from game's mods/ folder into deployer/mods/
#   2. Builds installer.py -> StS2ModInstaller.exe  (via build.ps1)
#   3. Zips StS2ModInstaller.exe + mods/ -> dist/StS2Mods.zip
#
# Prerequisites:
#   - Mods must already be built and deployed to the game's mods/ folder
#     (run dotnet publish / PublishMod.ps1 in each mod project first)
#   - pip install pyinstaller

$ErrorActionPreference = "Stop"
$ScriptDir    = Split-Path -Parent $MyInvocation.MyCommand.Path
$WorkspaceDir = Split-Path -Parent $ScriptDir
$Sts2ModsPath = "A:\SteamLibrary\steamapps\common\Slay the Spire 2\mods"

# Mod list. Edit to add/remove mods. MultiplayerPotions excluded (deprecated).
$Mods = @(
    @{ Id = "BaseLib";            HasPck = $true  },
    @{ Id = "CsvCardAdjustments"; HasPck = $true  },
    @{ Id = "GachaShopMod";       HasPck = $true  },
    @{ Id = "HandSmoother";       HasPck = $false },
    @{ Id = "MonsterPredictions"; HasPck = $false },
    @{ Id = "RelicChoice";        HasPck = $true  }
)

# Step 1 - collect mod files
Write-Host "Collecting mod files from $Sts2ModsPath ..." -ForegroundColor Cyan

$ModsDestDir = Join-Path $ScriptDir "mods"
if (Test-Path $ModsDestDir) { Remove-Item $ModsDestDir -Recurse -Force }

foreach ($mod in $Mods) {
    $id      = $mod.Id
    $destDir = Join-Path $ModsDestDir $id
    New-Item -ItemType Directory -Force -Path $destDir | Out-Null

    $dllSrc = Join-Path $Sts2ModsPath "$id.dll"
    if (Test-Path $dllSrc) {
        Copy-Item $dllSrc $destDir
    } else {
        Write-Warning "$id.dll not found in mods folder - build it first."
    }

    if ($mod.HasPck) {
        $pckSrc = Join-Path $Sts2ModsPath "$id.pck"
        if (Test-Path $pckSrc) {
            Copy-Item $pckSrc $destDir
        } else {
            Write-Warning "$id.pck not found in mods folder."
        }
    }

    # Copy manifest from workspace source (not from game folder)
    $manifestSrc = $null
    $candidates = @(
        (Join-Path $WorkspaceDir "$id\mod_manifest.json"),
        (Join-Path $WorkspaceDir "$id-StS2\mod_manifest.json"),
        (Join-Path $WorkspaceDir "$id\$id.json")
    )
    foreach ($c in $candidates) {
        if (Test-Path $c) { $manifestSrc = $c; break }
    }
    if ($manifestSrc) {
        $destName = if ($id -eq "HandSmoother" -or $id -eq "MonsterPredictions") { "$id.json" } else { "mod_manifest.json" }
        Copy-Item $manifestSrc (Join-Path $destDir $destName)
    } else {
        Write-Warning "No manifest found for $id."
    }

    Write-Host "  OK $id" -ForegroundColor Green
}

# Step 2 - build the installer exe
Write-Host ""
Write-Host "Building installer executable..." -ForegroundColor Cyan
& (Join-Path $ScriptDir "build.ps1")

# Step 3 - zip everything together
$DistDir = Join-Path $ScriptDir "dist"
$ZipPath = Join-Path $DistDir "StS2Mods.zip"

Write-Host ""
Write-Host "Creating distributable zip..." -ForegroundColor Cyan

if (Test-Path $ZipPath) { Remove-Item $ZipPath -Force }

$ExePath = Join-Path $DistDir "StS2ModInstaller.exe"
if (-not (Test-Path $ExePath)) {
    Write-Error "Installer exe not found at $ExePath - build step may have failed."
}

$TempPackDir = Join-Path $DistDir "StS2Mods_package"
if (Test-Path $TempPackDir) { Remove-Item $TempPackDir -Recurse -Force }
New-Item -ItemType Directory -Path $TempPackDir | Out-Null
Copy-Item $ExePath $TempPackDir
Copy-Item $ModsDestDir (Join-Path $TempPackDir "mods") -Recurse

Compress-Archive -Path "$TempPackDir\*" -DestinationPath $ZipPath
Remove-Item $TempPackDir -Recurse -Force

Write-Host ""
Write-Host "Done! Distributable package: $ZipPath" -ForegroundColor Green
Write-Host "Send the zip to your friend - they unzip and run StS2ModInstaller.exe" -ForegroundColor DarkGray
