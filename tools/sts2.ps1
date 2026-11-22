# [Claude, 2026-11-14] Close, launch or restart Slay the Spire 2, so a mod can be rebuilt and retested
# without anyone at the keyboard. Launches through Steam (the game needs Steam running).
#
#   tools\sts2.ps1 status
#   tools\sts2.ps1 stop
#   tools\sts2.ps1 start   [-NoWait] [-Foreground] [-Mods keep|all|mp|seductress|vanilla]
#   tools\sts2.ps1 restart [-NoWait] [-Foreground] [-Mods ...]
#
# [Claude, 2026-11-21] -Mods answers Gorim's "which mods?" launcher window (tools\sts2-launcher.ps1, run by
# Steam) so nobody has to click it: it writes the word to tools\.launch_choice, which the launcher reads once.
# keep (the default) starts with the mods as they are; seductress is that mod, what it depends on, and Spire Link.
#
# start/restart wait until the main menu is usable: Spire Link's local /health answers (it is
# unauthenticated) and the game has logged that the menu finished loading. -NoWait returns as soon as
# the process exists.
#
# [Gorim, 2026-11-14] asked for the game to launch without taking focus. By default the window you
# were in is remembered; whenever the game grabs the foreground during loading it is pushed behind the
# other windows and your window is put back in front. -Foreground turns that off.
param(
    [Parameter(Position = 0)][ValidateSet('status', 'stop', 'start', 'restart')][string]$Action = 'status',
    [switch]$NoWait,
    [switch]$Foreground,
    [ValidateSet('keep', 'all', 'mp', 'seductress', 'vanilla')][string]$Mods = 'keep',
    [int]$TimeoutSec = 180
)

$AppId = 2868840
$ProcName = 'SlayTheSpire2'
$Health = 'http://127.0.0.1:15528/health'

Add-Type -Namespace Sts2Tool -Name Win -MemberDefinition @'
[DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
[DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
[DllImport("user32.dll")] public static extern bool IsWindow(IntPtr hWnd);
[DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
[DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr hWnd, IntPtr after, int x, int y, int cx, int cy, uint flags);
'@

function Get-Game { Get-Process -Name $ProcName -ErrorAction SilentlyContinue }

function Test-Health {
    try { return (Invoke-RestMethod -Uri $Health -TimeoutSec 2).status -eq 'ok' } catch { return $false }
}

# If the game currently owns the foreground window, send it to the back and give focus back.
function Restore-Focus([IntPtr]$Previous) {
    if ($Foreground) { return }
    $fg = [Sts2Tool.Win]::GetForegroundWindow()
    if ($fg -eq [IntPtr]::Zero) { return }
    $owner = 0
    [void][Sts2Tool.Win]::GetWindowThreadProcessId($fg, [ref]$owner)
    if (-not (Get-Game | Where-Object { $_.Id -eq $owner })) { return }
    # HWND_BOTTOM = 1; SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE = 0x13
    [void][Sts2Tool.Win]::SetWindowPos($fg, [IntPtr]1, 0, 0, 0, 0, 0x13)
    if ($Previous -ne [IntPtr]::Zero -and [Sts2Tool.Win]::IsWindow($Previous)) {
        [void][Sts2Tool.Win]::SetForegroundWindow($Previous)
    }
    $script:FocusReturned++
}

function Stop-Game {
    $p = Get-Game
    if (-not $p) { Write-Output 'stop: not running'; return }
    # Ask politely first so the game writes its save and settings, then force it.
    $p | ForEach-Object { [void]$_.CloseMainWindow() }
    $deadline = (Get-Date).AddSeconds(20)
    while ((Get-Game) -and (Get-Date) -lt $deadline) { Start-Sleep -Milliseconds 500 }
    if (Get-Game) {
        Get-Game | Stop-Process -Force
        Start-Sleep -Seconds 1
        Write-Output 'stop: forced'
    } else { Write-Output 'stop: closed cleanly' }
    # The DLLs stay locked for a moment after the process goes.
    Start-Sleep -Seconds 2
}

function Start-Game {
    if (Get-Game) { Write-Output 'start: already running'; return }
    $previous = [Sts2Tool.Win]::GetForegroundWindow()
    $script:FocusReturned = 0
    Set-Content -Path (Join-Path $PSScriptRoot '.launch_choice') -Value $Mods -NoNewline
    Start-Process "steam://rungameid/$AppId"
    $deadline = (Get-Date).AddSeconds($TimeoutSec)
    while (-not (Get-Game) -and (Get-Date) -lt $deadline) { Start-Sleep -Milliseconds 250 }
    if (-not (Get-Game)) { Write-Output 'start: FAILED - process never appeared (Steam dialog open?)'; exit 1 }
    if ($NoWait) {
        # Still guard focus for a few seconds, since the window appears just after the process.
        1..40 | ForEach-Object { Restore-Focus $previous; Start-Sleep -Milliseconds 250 }
        Write-Output 'start: process up'; return
    }
    $log = Join-Path $env:APPDATA 'SlayTheSpire2\logs\godot.log'
    $ready = $false
    $tick = 0
    while ((Get-Date) -lt $deadline) {
        Restore-Focus $previous
        if (-not (Get-Game)) { Write-Output 'start: FAILED - game exited during load'; exit 1 }
        # Spire Link answers before the main menu exists; the game logs this line once the menu is usable.
        if (($tick % 4) -eq 0 -and (Test-Health) -and (Test-Path $log) -and
            (Select-String -Path $log -SimpleMatch 'main menu loaded (complete)' -Quiet)) { $ready = $true; break }
        $tick++
        Start-Sleep -Milliseconds 250
    }
    if (-not $ready) { Write-Output 'start: process up, but the main menu was not ready in time'; exit 2 }
    # The game sometimes takes focus once more right after the menu appears.
    1..8 | ForEach-Object { Restore-Focus $previous; Start-Sleep -Milliseconds 250 }
    Write-Output "start: game up, main menu ready (focus handed back $($script:FocusReturned) time(s))"
}

switch ($Action) {
    'status' {
        $p = Get-Game
        if ($p) { Write-Output "running (pid $($p.Id -join ',')), spire link: $(if (Test-Health) { 'ok' } else { 'no answer' })" }
        else { Write-Output 'not running' }
    }
    'stop' { Stop-Game }
    'start' { Start-Game }
    'restart' { Stop-Game; Start-Game }
}
