$assemblies = @(
    "A:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\0Harmony.dll",
    "A:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\GodotSharp.dll",
    "A:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll",
    "A:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\BaseLib.dll"
)

foreach ($asmPath in $assemblies) {
    if (Test-Path $asmPath) {
        $null = [Reflection.Assembly]::LoadFrom($asmPath)
    }
}

$baseLib = [Reflection.Assembly]::LoadFrom("A:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\BaseLib.dll")
try {
    $types = $baseLib.GetTypes()
    $potionType = $types | Where-Object { $_.Name -eq "CustomPotionModel" }
    
    if ($potionType) {
        Write-Host "--- CustomPotionModel Properties ---"
        $potionType.GetProperties() | ForEach-Object { Write-Host "$($_.PropertyType.Name) $($_.Name)" }
        
        Write-Host "--- CustomPotionModel Methods ---"
        $potionType.GetMethods() | Where-Object { $_.DeclaringType.Name -match "Potion" } | ForEach-Object { 
            $params = $_.GetParameters() | ForEach-Object { "$($_.ParameterType.Name) $($_.Name)" }
            $paramStr = [string]::Join(", ", $params)
            Write-Host "$($_.ReturnType.Name) $($_.Name)($paramStr)" 
        }
    } else {
        Write-Host "CustomPotionModel not found!"
    }
} catch {
    Write-Host "Error: $($_.Exception.Message)"
    if ($_.Exception.LoaderExceptions) {
        foreach ($e in $_.Exception.LoaderExceptions) {
            Write-Host "LoaderException: $($e.Message)"
        }
    }
}
