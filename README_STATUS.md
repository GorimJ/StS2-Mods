# STS2Modding – status (3 Sep 2026)

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

Later the same day: RelicChoice 1.1.0 (tickets resolve per character, description names the exact relic;
Gorim play-tested a full run OK), GachaShopMod pulls now seeded from run seed + player slot + floor + pull
number, and **all mods migrated to the ModTemplate layout** (`mods\{Id}\` subfolders, NuGet BaseLib). Pushed to origin.

## Known rough edges / next steps
1. **RelicChoice layout**: the treasure-room holders were sitting left of centre. Reworked
   (02:09) to centre on the mean X of the vanilla multiplayer holders and reuse their
   spacing; compiled and deployed but **not yet seen in-game** – check a treasure room.
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
6. Done: stale decompiled_* folders, backup snapshots and MultiplayerPotions removed.
7. GachaShopMod multiplayer: the pull RNG is now deterministic, but the flow (LoseGold →
   RewardSynchronizer.SyncLocalGoldLost / SyncLocalObtainedRelic) is still untested with two clients.
8. Done: every mod is on the template layout. Only the Buxom port still uses the old one.

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

## 2026-09-04 (overnight, Cowork)
* **Workshop NSFW text work**: Lwed_spire_2's 199 changed keys translated into `%APPDATA%\SlayTheSpire2\localization_override\eng\`;
  xianyzm's event pack ("Sex") English-polished, audited against its decompiled code, hidden costs/odds written into
  option text, repacked as local `mods\Sex\` (overrides the delisted Workshop copy). Backup + final loc in `_runner\loc_work\`.
* **SexFixes** (new, this repo): companion mod for the Sex pack — hover tips on relic/potion options and the unused
  Milk Giver second-drink page, via one prefix on `EventModel.SetEventState`. Built clean, deployed, NOT yet seen in-game.
* NSFW-Event parked (manifests renamed `.parked` in both `mods\` and workshop 3765802910) while Lwed's text is tested.
* Ovelle White Edition is obsolete (main 0.53 has the CGs) — unsubscribe. Maoyu needs beta 0.111 — unsubscribed, waiting.
* `docs/research-2026-09/` + `docs/BUXOM_BUILD_PLAN.md`: modding-scene survey (EN/CN, animation/AI) and the phased Buxom plan.
