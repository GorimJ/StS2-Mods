$TargetDir = "C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\TheWatcherMod"

# 1. Replace text in files
$Extensions = @('.cs', '.json', '.csproj', '.ps1', '.tres', '.md')
Get-ChildItem -Path $TargetDir -Recurse | Where-Object { $Extensions -contains $_.Extension -and -not $_.PSIsContainer } | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match 'TheWatcherMod' -or $content -match 'thewatchermod' -or $content -match 'THEWATCHERMOD') {
        $content = $content.Replace('TheWatcherMod', 'MultiplayerPotions')
        $content = $content.Replace('THEWATCHERMOD', 'MULTIPLAYERPOTIONS')
        $content = $content.Replace('thewatchermod', 'multiplayerpotions')
        Set-Content -Path $_.FullName -Value $content -NoNewline
    }
}

# 2. Rename files
Get-ChildItem -Path $TargetDir -Recurse -File | Where-Object { $_.Name -match 'TheWatcherMod|thewatchermod' } | ForEach-Object {
    $newName = $_.Name.Replace('TheWatcherMod', 'MultiplayerPotions').Replace('thewatchermod', 'multiplayerpotions')
    Rename-Item -Path $_.FullName -NewName $newName
}

# 3. Rename inner directories
Get-ChildItem -Path $TargetDir -Recurse -Directory | Where-Object { $_.Name -match 'TheWatcherMod|thewatchermod' } | ForEach-Object {
    $newName = $_.Name.Replace('TheWatcherMod', 'MultiplayerPotions').Replace('thewatchermod', 'multiplayerpotions')
    Rename-Item -Path $_.FullName -NewName $newName
}

Write-Host "Inner renames complete."
