$Sts2ModsPath = "A:\SteamLibrary\steamapps\common\Slay the Spire 2\mods"
$BaseLibPath = "C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\BaseLib-StS2\.godot\mono\temp\bin\Debug\publish"
$BaseLibPckPath = "C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\BaseLib-StS2\.godot\mono\temp\bin\Debug\publish"
$GodotPath = "C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\Godot_v4.5.1-stable_mono_win64\Godot_v4.5.1-stable_mono_win64.exe"

Write-Host "Ensuring Mods Directory Exists..." -ForegroundColor Cyan

if (-not (Test-Path -Path $Sts2ModsPath -PathType Container)) {
    if (Test-Path -Path $Sts2ModsPath -PathType Leaf) { Remove-Item -Path $Sts2ModsPath -Force }
    New-Item -ItemType Directory -Force -Path $Sts2ModsPath | Out-Null
}

Write-Host "Cleaning up previous versions..." -ForegroundColor Yellow
$filesToRemove = @("CsvCardAdjustments.pck", "CsvCardAdjustments.dll", "BaseLib.pck", "BaseLib.dll")
foreach ($file in $filesToRemove) {
    $filePath = Join-Path $Sts2ModsPath $file
    if (Test-Path $filePath) {
        Remove-Item -Path $filePath -Force
    }
}

Write-Host "Deploying BaseLib..." -ForegroundColor Cyan
Copy-Item -Path "$BaseLibPath\BaseLib.dll" -Destination $Sts2ModsPath -Force
Copy-Item -Path "$BaseLibPckPath\BaseLib.pck" -Destination $Sts2ModsPath -Force

Write-Host "Building CsvCardAdjustments via dotnet build..." -ForegroundColor Green
dotnet build --no-incremental

Write-Host "Manually copying .dll and .json to mods folder..." -ForegroundColor Cyan
# Godot's parallel compilation hook writes the .dll slightly after dotnet build resolves
Start-Sleep -Seconds 2
$dllPath = Join-Path $PSScriptRoot ".godot\mono\temp\bin\Debug\CsvCardAdjustments.dll"
$jsonPath = Join-Path $PSScriptRoot "mod_manifest.json"
Copy-Item -Path $dllPath -Destination $Sts2ModsPath -Force
Copy-Item -Path $jsonPath -Destination "$Sts2ModsPath\CsvCardAdjustments.json" -Force

Write-Host "Exporting Godot .pck..." -ForegroundColor Green
& $GodotPath --headless --export-pack "Windows Desktop" "$Sts2ModsPath\CsvCardAdjustments.pck"

Write-Host "All Mod Files successfully published to Steam Library!" -ForegroundColor Green
