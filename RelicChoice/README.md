# Relic Choice (STS2 mod)

* Treasure rooms offer `AdditionalRelics` extra relics to choose between (config, default 1).
* After an elite fight the relic reward becomes a **Claim Relic** step that opens a treasure-room style pick
  (multiplayer: everyone votes, as in a normal treasure room).
* Six **Ticket** relics live in the shared pool and turn into a relic from the holder's own character pool:
  Bronze (common #1), Silver / Shiny Silver (uncommon #1/#2), Golden / Shiny Golden / Premium Golden (rare #1/#2/#3),
  where #N is the Nth relic of that rarity sorted by id, skipping relics already owned and falling back to the shared
  pool. Works for modded characters that register relics through BaseLib. The description shows the exact relic the
  current player would get.

Config: `%APPDATA%\SlayTheSpire2\modded\RelicChoiceConfig.json` (`AdditionalRelics`, `EnableAfterElites`, `EnableRainbowRelics`).

Build: `dotnet build` (dll + json → `mods\RelicChoice\`), `dotnet publish` (also exports the .pck via Godot from `Directory.Build.props`).
Template layout from Alchyr's ModTemplate-StS2; BaseLib comes from NuGet (`Alchyr.Sts2.BaseLib`).
