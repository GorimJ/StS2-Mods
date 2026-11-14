# Spire Link local patch

[Claude, 2026-11-14] Our local changes to Spire Link, kept as a patch so they survive outside the temp folder
they were made in. This is a diff only, not a copy of Spire Link's source.

- Upstream: https://github.com/PingChunChung/spire-link
- Applies to commit: `903c70819c2795ef20840ee62484960d16526cdc`
- Game version it was built and run against: v0.113.0

## What it changes

- Compiles on game v0.113.0 (merchant inventory and purchase moved to `MerchantSynchronizer`; `NUnlockEpochScreen` is gone).
- Card catalogue no longer fails when one mod's card throws while being built or upgraded.
- `POST /v1/console` with body `{ "command": "..." }` runs one dev-console command through the game's own console.
- Every state gets a `mods` section. A mod opts in with a public static class named `SpireLinkExport` that has a
  public static `Snapshot()` returning a dictionary. Seductress does this in
  `Seductress/SeductressCode/Integration/SpireLinkExport.cs`.

## Rebuild

```
git clone https://github.com/PingChunChung/spire-link
cd spire-link
git checkout 903c708
git apply ../SpireLinkPatch/spire-link-local.patch
dotnet build SpireLink.csproj -c Release -p:STS2GameDir="A:/SteamLibrary/steamapps/common/Slay the Spire 2" -o out
```

Copy `out/SpireLink.dll` to the game's `mods/SpireLink/` folder, next to `SpireLink.json` (a copy of the
repo's `mod_manifest.json`).

## Later additions

- [Claude, 2026-11-14] The main menu now offers `timeline` as an action (upstream only shows it as an option).
- [Claude, 2026-11-14] `POST /v1/recovery` accepts `mode: "abandon_pending"`. Spire Link refuses new actions after
  one whose effect it never saw (a click that did nothing, say); this drops that latch without touching the game.
  Found when a timeline "advance" with nothing to advance left it blocked until the game was restarted.

## Tools

- `tools/sts2.ps1` closes, launches or restarts the game through Steam and waits for the main menu. By default it
  hands focus back to whatever window was in front.
- `tools/spirelink_drive.py` is a small client: `st` (state and numbered actions), `act <n|label>`, `con <command>`,
  `conraw <command>`, `raw <key...>`, `unstick`. It reads the key from Spire Link's own config and never prints it.

## Status

[verified, 2026-11-14, in game] health, observe, card catalogue, the `mods` section with Seductress state, console
commands, targeted and untargeted cards, end turn, potions, rewards, card rewards, map travel, shop purchases and
card removal, rest site options (Smith, Indulge, Relief), treasure, events, an Ancient event, elite and boss fights,
game over, main menu, continue, singleplayer, character select, embark, timeline open and back.

"Decision mechanics" in the state is an upstream experiment that only understands two vanilla cards (Strangle and
Silent's Defend) and three powers, on one exact game build. Every other card of every character shows as unknown
there. It is not something the rest of the API depends on.

Not tested: daily and custom modes, multiplayer, abandon run, the `abandon_pending` recovery with a real latch.
