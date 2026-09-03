# STS2Modding – status (overnight rework, 3 Sep 2026)

Game: **v0.107.1** (build 2026-06-18). BaseLib: **v3.4.5** (downloaded from GitHub releases
into `mods\`). All of these were rebuilt against that pair, deployed to
`A:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\`, and exercised in-game via the dev
console (backtick) with 20-odd workshop mods also loaded:

| Mod                | State                                                                 | Verified in-game |
|--------------------|-----------------------------------------------------------------------|------------------|
| RelicChoice        | Rewritten reward-screen patch (`RewardsSet` API), extra treasure relic | Treasure room shows 2 relics; elite loot screen shows "Claim Relic" → treasure-room pick; Golden Ticket via console |
| GachaShopMod       | `Favored` enchantment gone from game → replaced by `Inky`; loc table registered; csproj now references `mods\BaseLib.dll` | Machine appears in shop, pull costs 75→100, enchant flow works; Inky ball via console |
| MonsterPredictions | Compiled unchanged; config moved to `%APPDATA%\SlayTheSpire2\modded\` | Two future intents render on monsters |
| HandSmoother       | Compiled unchanged                                                    | Opening hand proportional (2 Strike / 2 Defend / Bash from 5-4-1 deck) |
| CsvCardAdjustments | 10 compile errors fixed; 2 patches disabled (Parry rework, retain hook gone); patches applied class-by-class with logging | Loads: "Applied 35 patch classes, skipped 0". Card behaviour NOT play-tested |
| CharacterTemplate  | New – official template scaffold, see `CharacterTemplate/HOWTO_NEW_CHARACTER.md` | "The Template" in character select, starts a run, fights |
| Buxom Mod Port     | Untouched (its own git repo). Needs porting onto the template layout.  | – |

Committed as `2ea487e` on `main` (not pushed).

## Known rough edges / next steps
1. **RelicChoice layout**: extra relic holders in the treasure room are centred on the
   singleplayer holder's X, so the group sits left of screen centre. Cosmetic.
2. **RelicChoice tickets** pick class relics by `OrderBy(Id)` index; the descriptions name
   specific relics, which will drift as MegaCrit adds relics. Consider naming the relic
   explicitly per character instead of by index.
3. **Multiplayer** paths (relic vote after elites, `VoteForRelicChoiceAction`) were not
   tested – only singleplayer.
4. **BaseLib 3.4.5 vs this build**: BaseLib itself logs
   `MissingFieldException: NTreasureRoom._chestButton` when entering a treasure room
   (`CustomActTreasureChest`). Not ours, harmless so far; a newer BaseLib or the game's
   beta branch may resolve it.
5. **CsvCardAdjustments**: Parry override and Snakebite-on-retain are off. Old bugs in
   `bug_tracker.md` (Tag Team, Particle Wall/Afterlife targeting) untouched.
6. `RelicChoice/` and `MultiplayerPotions/` still carry `decompiled_sts2/` +
   `decompiled_baselib/` folders **in git** (stale, ~thousands of files). Delete and
   `git rm` them; the fresh decompile lives in `decompile_v0107/` (ignored).
7. `GachaShopMod_Backup_Stable/`, `MonsterPredictions_StableBackup/`, `MultiplayerPotions/`
   are superseded – safe to delete.
8. Migrate the other mods to the template layout (NuGet BaseLib, `mods\{Id}\` subfolder,
   ModAnalyzers) when you next touch them; `CharacterTemplate/HOWTO_NEW_CHARACTER.md` §0
   lists the differences.

## Tooling used tonight (all in-repo, git-ignored)
* `_runner/` – `start_runner.bat` launches a PowerShell loop that executes
  `_runner/queue/*.ps1` and writes `.out` next to them. This is how Cowork ran
  `dotnet build/publish`, git, and launched the game without a typed terminal.
  Double-click the .bat to bring it back; drop a file named `STOP` in `queue/` to end it.
* `decompile_v0107/` – ilspycmd output of the current `sts2.dll`;
  `decompile_baselib_345/` – same for BaseLib.
* Dev console commands that mattered: `room shop|treasure|elite|monster|event|restsite`,
  `win`, `relic <ID>`, `gold`, `card`, `travel`.

## Workshop mod that was in `mods\`
`NSFW-Event` (.json/.pck) was moved to `mods_disabled\` for testing and moved back at the end.
