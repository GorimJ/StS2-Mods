$Sts2ModsPath = "A:\SteamLibrary\steamapps\common\Slay the Spire 2\mods"
$BaseLibPath = "C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\BaseLib-StS2\.godot\mono\temp\bin\Debug\publish"
$BaseLibPckPath = "C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\BaseLib-StS2\.godot\mono\temp\bin\Debug\publish"

Write-Host "Deploying BaseLib to Slay the Spire 2 Mods Folder..." -ForegroundColor Cyan

if (-not (Test-Path -Path $Sts2ModsPath -PathType Container)) {
    if (Test-Path -Path $Sts2ModsPath -PathType Leaf) { Remove-Item -Path $Sts2ModsPath -Force }
    New-Item -ItemType Directory -Force -Path $Sts2ModsPath | Out-Null
}

Copy-Item -Path "$BaseLibPath\BaseLib.dll" -Destination $Sts2ModsPath -Force
Copy-Item -Path "$BaseLibPckPath\BaseLib.pck" -Destination $Sts2ModsPath -Force

Write-Host "BaseLib Deployed! Building and Publishing RelicChoice..." -ForegroundColor Green

dotnet publish

Write-Host "All Mod Files successfully published to Steam Library!" -ForegroundColor Green
