# Building a new STS2 character mod (framework)

This folder is Alchyr's official **CharacterModTemplate** (from `Alchyr.Sts2.Templates`,
Aug 2026) instantiated by hand with the mod id `CharacterTemplate`, paths set for this
machine, and the mandatory localization keys filled in. It builds, publishes and loads on
STS2 **v0.107.1** with BaseLib **3.4.5** ("The Template" shows up in character select and
plays as an Ironclad clone). Treat it as the reference skeleton: copy it, rename, replace.

## 0. How this differs from the March/April-era mods in this workspace

| Then (Buxom port, RelicChoice etc.)                         | Now (this template)                                            |
|-------------------------------------------------------------|----------------------------------------------------------------|
| `BaseLib.dll` referenced by HintPath into `mods\`           | `<PackageReference Include="Alchyr.Sts2.BaseLib" Version="*"/>` (NuGet) |
| `mod_manifest.json` with `"dependencies": ["BaseLib"]`      | `{Id}.json` with `[{"id":"BaseLib","min_version":"3.4.5"}]` + `min_game_version` (build target auto-updates min_version) |
| `.dll/.pck/.json` dropped loose into `mods\`                | Everything in `mods\{Id}\` subfolder (loader scans recursively) |
| No analyzers                                                | `Alchyr.Sts2.ModAnalyzers` fails the build if a model's loc keys are missing (error STS001) – this is how I found the required character keys below |
| Godot 4.5.1 mono for `--export-pack`                        | Same, but MegaCrit now ships **MegaDot 4.5.1** (their fork); the .pck must not come from a *newer* Godot. `Directory.Build.props` holds `GodotPath`. |
| Harmony `PatchAll()`                                        | `PatchAll(assembly)`; for many patches prefer patching class-by-class in a try/catch (see CsvCardAdjustments/MainFile.cs) so one stale patch doesn't kill the mod |

## 1. Make a new character from this scaffold

1. Copy `CharacterTemplate/` to `MyChar/` (no spaces in the name).
2. Rename: every file/folder and identifier `CharacterTemplate` → `MyChar`
   (csproj, json, project.godot `config/name` + `assembly_name`, the `CharacterTemplate/`
   asset folder, the `CharacterTemplateCode/` namespace folder, `MainFile.ModId`,
   `CharacterId`, the loc keys `CHARACTERTEMPLATE-CHARACTER_TEMPLATE.*`).
   The loc key prefix is `{MODID}-{CLASS_NAME_SNAKE}` in upper case – e.g. class
   `TheBuxom` in mod `BuxomModPort` → `BUXOMMODPORT-THE_BUXOM`.
3. `dotnet build MyChar.csproj` → compiles + copies `.dll/.json/.pdb` to `mods\MyChar\`.
   `dotnet publish MyChar.csproj` → also runs Godot `--export-pack` and copies the `.pck`.
   (Godot prints exit code -1 on this machine but the .pck is produced and loads.)
4. Launch the game; open the dev console with ` (backtick) – it is enabled whenever mods
   are loaded – and use `room shop`, `room treasure`, `room elite`, `win`,
   `relic MYCHAR-SOME_RELIC`, `card ...` to test without playing through.

## 2. What a character actually consists of

```
MyCharCode/
  MainFile.cs                 [ModInitializer] – Harmony PatchAll, nothing else needed
  Character/MyChar.cs         : PlaceholderCharacterModel (or CustomCharacterModel)
  Character/MyCharCardPool.cs : CustomCardPoolModel  – card back colour (H/S/V), energy icons
  Character/MyCharRelicPool.cs: CustomRelicPoolModel
  Character/MyCharPotionPool.cs: CustomPotionPoolModel
  Cards/MyCharCard.cs         abstract base, [Pool(typeof(MyCharCardPool))]; real cards inherit it
  Powers/MyCharPower.cs       abstract base : CustomPowerModel
  Relics/MyCharRelic.cs       abstract base, [Pool(typeof(MyCharRelicPool))]
  Potions/MyCharPotion.cs     abstract base, [Pool(typeof(MyCharPotionPool))]
  Extensions/StringExtensions.cs  res:// path helpers with fallbacks to placeholder art
MyChar/                        (asset root, becomes res://MyChar/ inside the .pck)
  images/card_portraits/{card}.png  (250x190; big/ 1000x760)   – full-art 250x350 / 606x852
  images/powers/{power}.png (+big/)  images/relics/{relic}.png, {relic}_outline.png (+big/)
  images/potions/{potion}.png (+outline/)
  images/charui/  character_icon_*.png, char_select_*.png, char_select_*_locked.png,
                  map_marker_*.png, big_energy.png, text_energy.png
  localization/eng/{characters,cards,powers,relics,ancients,card_keywords,static_hover_tips}.json
  mod_image.png
```

`PlaceholderCharacterModel` uses the base-game character's assets (`PlaceholderID`,
default `ironclad`) for everything you have not overridden – combat visuals, rest-site,
merchant, energy counter, select-screen background. That is why "The Template" looks
like the Ironclad. Override one thing at a time.

### Required localization (analyzer-enforced) – `characters.json`
```
{ID}.title, .titleObject, .description, .pronounObject, .pronounSubject,
{ID}.pronounPossessive, .possessiveAdjective, .goldMonologue (Sunken Treasury event),
{ID}.eventDeathPrevention, .aromaPrinciple (Aroma of Chaos event),
{ID}.cardsModifierTitle, .cardsModifierDescription, .banter.alive.endTurnPing,
{ID}.banter.dead.endTurnPing         (+ optional .title_plural, .blurb_1..3)
```
and in `ancients.json` the Architect dialogue: `THE_ARCHITECT.talk.{ID}.0-0r.char`,
`.0-0r.next`, `.0-1r.ancient`, `.0-attack` (`r` = repeating line; `.char`/`.ancient`
pick the speaker – see `MegaCrit.Sts2.Core.Entities.Ancients.AncientDialogueSet` in the
decompile for the full grammar, other ancients use the same pattern).

### Cards / powers / relics
Card: `class Foo : MyCharCard { public Foo() : base(cost, CardType.Attack, CardRarity.Common,
TargetType.AnyEnemy) {} ... override OnPlay(PlayerChoiceContext ctx, CardPlay play) }` with
`DynamicVars` for numbers (`DamageVar`, `BlockVar`, `PowerVar<T>`, ...).
Note the **v0.107 signatures** (these broke our old mods):
* `PowerCmd.Apply<T>(choiceContext, target(s), amount, applier, cardSource)`
* `CardPileCmd.AddGeneratedCardToCombat(card, PileType, Player? creator, CardPilePosition)`
* `RewardsSet` replaces `NRewardsScreen.SetRewards`; `RewardsCmd.Offer` builds a set per player.
Copy the pattern from `Buxom Mod Port/Code/Cards/*.cs` and `Code/Powers/*.cs` – the mechanics
code there is still valid, only the visuals plumbing changed.

### Visuals (the part that hurt on the Buxom port)
BaseLib wants an `NCreatureVisuals`-shaped scene: root Control with `Visuals` (Node2D),
`Bounds` (Control), `IntentPosition` and `CenterPos` (Marker2D); optional `OrbPos`,
`TalkPos`, `PhobiaModeVisuals`. Three escalating options:
1. **Static PNG** – `NodeFactory<NCreatureVisuals>.CreateFromResource("res://MyChar/images/character/idle.png")`
   or just `CustomVisualPath` to a .tscn that only has `Visuals` + `Bounds`.
   This is what `Buxom Mod Port/BuxomModPort/scenes/buxom_character.tscn` does
   (`compose_character.py` flattened the Spine skeleton into one PNG).
2. **Godot animations** – add an `AnimationPlayer` (or `AnimationTree` with a state
   machine) to the scene with animations named `idle`, `attack`, `cast`, `hurt`, `die`;
   BaseLib wires them up and returns to `idle` automatically. `buxom_skeleton.tscn`
   (generated by `generate_godot_scene.py` from the Spine parts) is the starting point for
   this – cut-out animation in the Godot editor, no Spine licence needed.
3. **Spine** – the game uses Spine 4.2 via `libspine_godot`; override
   `SetupCustomAnimationStates` on the character model. Only worth it if you own Spine.

Same shape for rest site (`NRestSiteCharacter`) and merchant (`NMerchantCharacter`) –
BaseLib has node factories for all three plus `NEnergyCounter`
(see `[BaseLib] Created node factory for ...` in godot.log).
Wiki: https://alchyr.github.io/BaseLib-Wiki/docs/scenes/creature-visuals.html

## 3. What to salvage from the Buxom port
* Mechanics: `BuxomPower`/`CapacityPower` (two-resource meter), `BuxomBar`+`BuxomBarPatch`
  (custom UI bar injected into the combat HUD), `BuxomVisualManager`/`BuxomVisualPatch`
  (swap sprite by power level) – all reusable patterns for any character with a gauge.
* Asset pipeline scripts: `extract_parts.py` / `compose_*.py` / `generate_godot_scene.py`
  turn a Spine atlas+json into layered PNGs and a Godot skeleton scene.
* The Buxom csproj still uses the old HintPath/manifest layout; when you come back to it,
  port it onto this template rather than patching it in place.

## 4. Gotchas collected tonight
* The game scans **every** `.json` under `mods\` as a manifest – keep configs in
  `user://modded/` (`%APPDATA%\SlayTheSpire2\modded\`).
* Manifest `version` must be semver (`1.0.0`, not `1.0`).
* Godot imports a `.csv` in the project as a *translation* and litters `*.translation`
  files (see GachaShopMod) – rename data files to `.txt`/`.json` or exclude them.
* Loc table names are global: register a mod-prefixed one
  (`BaseLib.Utils.CustomLocTableManager.Register("mychar_ui")`) instead of `"ui"`.
* `ModAnalyzers` error STS001 = missing loc key; the message lists exactly which keys.
* Fresh decompiles: `ilspycmd -p -o decompile_v0107 "...\data_sts2_windows_x86_64\sts2.dll"`
  (ilspycmd is installed as a dotnet tool; there is a newer 11.x available).
