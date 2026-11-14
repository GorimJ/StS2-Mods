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

## Status

[verified, 2026-11-14, in game] health, observe, card catalogue, playing an untargeted card, the `mods` section
with Seductress state, and console commands (`help`, `reservoir`, `sigils`).

Not tested: end turn, targeted attacks, shop purchase, timeline, rewards, map, events, rest sites.
