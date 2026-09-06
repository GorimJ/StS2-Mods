# The Seductress — engine spike results

Answers to the four questions in `SEDUCTRESS_SPIKE_BRIEF.md`. Game **v0.107.1**, BaseLib **v3.4.5**.
Code lives in `Seductress/`, on the ModTemplate layout, asset-free by design so it can live in git.

Tested in a live run (seed `WLN1V2U9A8`) with 41 other mods loaded. Evidence is quoted from
`%APPDATA%\SlayTheSpire2\logs\godot.log`.

**Status: all four questions answered. None blocked.**

---

## Summary

| # | Question | Verdict |
|---|---|---|
| 1 | Can the health bar present inverted, as an ascending Arousal bar? | **Solved with caveats** |
| 2 | Can death be intercepted so 0 HP triggers Climax? | **Solved** |
| 3 | Can a mod define its own card tags? | **Solved** |
| 4 | Do custom resources persist and display? | **Solved** |

No question came back blocked. Nothing in the design needs to change on account of the engine.

---

## 1. Inverted health bar — solved with caveats

**API used:** two Harmony postfixes on the private methods `NHealthBar.RefreshForeground` and
`NHealthBar.RefreshText`, plus a third on `RefreshMiddleground`. Private fields are reached with
`AccessTools.FieldRefAccess`. `HealthBarForecastRegistry` was not usable — as the brief predicted,
it overlays segments but cannot reverse the base fill.

The inversion itself is small. `RefreshForeground` sets the bar's right offset from
`GetFgWidth(CurrentHp)`; `RefreshText` writes `"{CurrentHp}/{MaxHp}"`. Substituting
`MaxHp - CurrentHp` in both is the whole change. The bar is recoloured pink (`E0559C`) rather than
the vanilla damage red.

**Confirmed in play.** At 61/65 real HP the bar read `4/65` and filled pink from the left. Enemies
invert too, which the design wants — an enemy's HP is their arousal.

### Caveats

1. **The middleground had to be hidden.** Vanilla draws a trailing "recent damage" bar behind the
   foreground. Reversed, it reads backwards. Hiding it has one non-obvious requirement:
   `RefreshValues` calls `RefreshForeground()` *then* `RefreshMiddleground()`, and the latter sets
   `Visible = true` unconditionally — so it must be hidden from its own postfix. Doing it from the
   foreground postfix silently fails.

   This produced the one visible defect found in testing: an untouched enemy showed a **full orange
   bar behind an empty arousal bar**, because the middleground only collapses via a tween that
   fires when HP changes, and an enemy that has not been hit never fires it. Fixed.

2. **Poison and Doom overlays are not inverted.** They are still positioned from `CurrentHp`, so
   they will sit in the wrong place. Out of spike scope and cheap to fix later by the same method;
   worth doing before either effect can reach the player.

3. **The patch is gated on the local player being the Seductress**, so other characters — vanilla
   or from the 41 other mods loaded — are untouched.

---

## 2. Death interception / Climax — solved

**API used:** no Harmony patch. The brief's instinct was right: there is vanilla precedent, and it
is reachable from a mod.

Lizard Tail and Fairy in a Bottle both work through `AbstractModel.ShouldDie(Creature)` and
`AfterPreventingDeath(Creature)`, dispatched by `Hook.ShouldDie` over
`RunState/CombatState.IterateHookListeners`. That listener list includes
`ModHelper.IterateAllCombatStateSubscribers`, and BaseLib wraps that subscription in
`CustomSingletonModel(HookType.Combat)`.

So `ClimaxModel : CustomSingletonModel` receives the same death hook a vanilla relic does, with no
patching at all. It returns `false` from `ShouldDie` for the Seductress, and performs the Climax in
`AfterPreventingDeath`: heal to full, `+1 SensitivePower`, `AddCurseToDeck<Aching>`.

**All three death paths confirmed:**

| Path | Evidence |
|---|---|
| Enemy attack | `FUZZY_WURM_CRAWLER performing move ACID_GOOP` → `CLIMAX #1` |
| Instant kill, no HP loss (`die`) | `DevConsole: die` → `CLIMAX #2` |
| The character's own self-damage card | `playing card SEDUCTRESS-LET_GO` → `CLIMAX #3` |

**And the run still ends.** With `LustLimit` reached, the next 0 HP produced a real game over —
`GameOverScreen` in the log, no Climax, no exceptions. Intercepting every death is only correct if
the run can still end, so this is the other half of the answer rather than an afterthought.

The `die` case is the significant one. It calls `CreatureCmd.Kill` directly with no damage at all,
so interception happens at the death hook rather than depending on damage bookkeeping. That
retires most of design risk §10.1 — **anything routing through `CreatureCmd.Kill` is covered**,
which includes events, curses, Poison and Doom.

### Implementation note that will bite anyone who touches this

`KillWithoutCheckingWinCondition` zeroes HP *before* asking `ShouldDie`, then re-enters itself if
the creature is still dead afterwards, throwing after ten attempts. A preventer that does not
restore HP therefore hangs the combat. Healing to full is load-bearing, not decoration — which is
also exactly what the design wants (arousal resets to 0).

### Known gap

`CreatureCmd.Kill(creature, force: true)` skips the hook by design. Nothing in the base game calls
it on the player outside a real game-over, so this is correct behaviour rather than a hole — but it
is the one way to die that Climax will not catch, and it is worth knowing about.

---

## 3. Mod-defined card tags — solved

**Answer: a mod cannot extend the `CardTag` enum, and does not need to.**

`CardTag` is a plain C# enum, so no member can be added at runtime. But the type's own
documentation calls tags "behaviour-less metadata that other models can use to look things up",
nothing in the game switches over it or enumerates its members, and `CardModel.Tags` is a
`HashSet<CardTag>`. Mod-defined values are therefore just unused integers cast into the enum. They
store, compare and hash exactly like vanilla members, and vanilla lookups never match them because
they ask for their own values.

`SeductressTags` defines them from 1000 up, leaving MegaCrit room to add vanilla tags without
colliding:

```csharp
public const CardTag Glazed = (CardTag)1001;
public const CardTag Mouth  = (CardTag)1002;
public const CardTag Ass    = (CardTag)1003;
public const CardTag Pussy  = (CardTag)1004;
```

**Confirmed in both directions** by the `TasteTest` probe card, which asks the brief's exact
question — "is the last attack played tagged Mouth?" — against the vanilla `Tags` collection:

```
TasteTest: last attack = SEDUCTRESS-LIP_SERVICE, tags = [1002], Mouth? True
TasteTest: last attack = SEDUCTRESS-HANDFUL,     tags = [1001], Mouth? False
```

**No design consequence. The card list's tags stand as written** — `Mouth`, `Ass`, `Pussy`,
`Glazed`, `Deep Throat` and `Continuing` all work as real tags, and adding a fourth Deep Throat
card later costs nothing.

*Related observation:* at least one other loaded mod (`LustTravel2`) ships a card-tag registry of
its own that hashes names into `CardKeyword` values. Worth knowing that approach exists, but the
cast approach above is self-contained and needs no dependency.

---

## 4. Reservoirs — persist and display — solved

### Persistence

**API used:** `ExtendedSaveTypes.RegisterSavedValue<IRunState, int>(id, getter, setter, writer, reader)`,
called once at mod init, one registration per value. BaseLib already patches `RunManager.ToSave` and
`RunState.FromSerializable`, so the values ride along with the ordinary run save — no file of our
own, no config directory.

Run-scoped state itself is held in a `ConditionalWeakTable<object, ReservoirState>` keyed on the
`IRunState`, so a new run starts empty and the table does not leak.

**Confirmed twice, independently.** The values land in `current_run.save` as plain JSON, in a
`save_dict_Int32` block beside the vanilla run data:

```json
"save_dict_Int32": {
  "Seductress.Ass": 0,
  "Seductress.Glazed": 0,
  "Seductress.Mouth": 0,
  "Seductress.Sigils": 0,
  "Seductress.Stomach": 0,
  "Seductress.Womb": 25
}
```

That file was read from disk **after the game process had exited**, which proves the write half on
its own. The read half was then confirmed by relaunching the game and resuming the run: the HUD
came back reading `Womb 25 / 100 ml`. Because the process restarted, the `RunState` object was
destroyed and rebuilt from the file — a quit-to-menu would not have proven this, since the live
object can survive that and be read back without ever touching the save.

### Display

**There is no vanilla affordance, and BaseLib's is a dead end.** BaseLib has a custom-resource
system (`CustomResource` / `CustomResources<T>`), but `BasicResourceVisualsHandler.AddDisplay` is
an empty method, and the whole system is scoped to `PlayerCombatState` while the reservoirs are
run-scoped. Neither half fits.

**What works** is the pattern the Buxom port already established: a plain Godot `Control` parented
onto `NCombatUi` via a Harmony postfix on `Activate`, removed again on `Deactivate`. The empty
gutter to the lower left of the character comfortably holds seven lines at 12px, and does not fight
the vanilla HUD for space.

Confirmed legible in play, showing the four containers, Stomach, Sigils, the live *Continuing*
routing mode, and the Climax / Sensitive / Lust counters.

**So the answer to "is there a slot at all" is yes, with room to spare.** The current readout is
deliberately crude — HUD legibility is deferred to playtest by decision (§10.6) and nothing here
constrains that decision.

### Intake, confirmed in passing

Routing and the Sensitive multiplier both work. With three Sensitive stacks, a normal kill after a
Pussy-tagged attack paid `Intake 25 ml -> Pussy` — `10 × (1 + 0.5 × 3)`, filling the Womb.

**Spike simplification worth knowing:** intake currently reads the *Continuing* mode rather than
the specific card that landed the killing blow, and pays a flat normal-enemy 10 ml with no
elite/boss tiers. Enough to exercise routing, the multiplier and the display; the real version
needs the finishing card and the §6.2 tier table.

---

## Decisions this spike has surfaced

Things the design docs did not settle, which the implementation has now settled by default,
recorded so they are not changed later by accident. The decision register in
`SEDUCTRESS_DESIGN.md` §12 is Gorim's; nothing here has been written into it.

### D1. A Lust card gained from Climax enters the run deck, not the current combat

`CardPileCmd.AddCurseToDeck<Aching>(player)` calls `RunState.CreateCard(...)` then
`Add(card, PileType.Deck)` — the master deck. The card cannot be drawn until the next combat
shuffles it in.

**Why keep it.** §4 already refused to end your turn on Climax, on the grounds that the event was
being taxed three times over. Putting the Lust card into the live draw pile would be a fourth tax
on the same event, and it would fall hardest on Breeding, whose entire plan is to Climax
repeatedly.

**The cost, stated plainly.** Multiple Climaxes within one fight are close to free *in that fight*.
The death clock is real but it only ticks between combats.

**The lever, if that ever reads as too soft.** Swap to
`CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, player)`, which puts it in the
current combat's discard pile and so into the next shuffle.

### D2. Sensitive multiplies self-damage, ignoring `ValueProp.Unpowered`

`SensitivePower.ModifyDamageMultiplicative` applies `1 + 0.5 × stacks` to **all** damage the owner
takes, including damage flagged `Unpowered` — which is what the character's own self-damage cards
use (`Let Go`, `Clench`, the whole blood-magic axis).

This is what §4 asks for: "two climaxes halve your effective bar" only holds if the cards that
generate most of your arousal are included. But it is a deliberate departure from what `Unpowered`
means everywhere else in the game, where it exists precisely to keep powers out of a damage
calculation.

**Gorim's call.** The alternative — respecting `Unpowered` — makes Sensitive substantially weaker
than designed, because most of the deck would be exempt from it.

### D3. Three Lust cards maximum — climaxing at 3 ends the run

`ClimaxModel.LustLimit = 3`, read as "holding this many means the next 0 HP ends the run", so a
fourth Lust card is never gained.

**[decided 2026-09-06, Gorim]** after a design pass in the parallel session. The spike was first
built to §5's headline ("5 Lust in deck ends the run"), then briefly set to 4 mid-testing, and
landed on 3. The shorter clock is paired with a **Lust-removal option at rest sites**, which is
what makes 3 survivable rather than brutal.

**Not built here.** Rest-site options are out of spike scope, so the mod currently has a 3-card
clock and no way to clear it. That is a deliberate gap, not an oversight — but it means the spike
build is unwinnable over a long run, and the removal option is the first thing the real mod needs.

`SEDUCTRESS_DESIGN.md` §5 has **not** been edited — the register is Gorim's, and §5 is being
reworked in the design session. The code reads 3; §5 currently reads 5. Deliberate divergence.

Changing the number is a one-line edit to `LustLimit`.

---

## Cosmetic bugs found and fixed during testing

Neither affected mechanics; both were logged exceptions in card text layout.

* Card descriptions use paired colour tags only (`[gold]`, `[blue]`) — `[block]` and `[energy]` are
  not self-closing tags and throw `Found end tag center, expected …`. Keywords are written as plain
  words wrapped in `[gold]…[/gold]`.
* The energy icon is a SmartFormat formatter, not a tag: `{Energy:energyIcons()}`.

## One trap worth recording for future character mods

Do **not** construct a `CustomSingletonModel` from your `ModInitializer`. `ModelDb.Init`
instantiates every registered model type itself, and `CustomSingletonModel` subscribes for combat
hooks in its constructor. Constructing one yourself produces a second instance whose subscription
is rejected (`Tried to subscribe for CombatState hooks with id …, but it's already been used`)
while still overwriting your static `Instance` — leaving the live `Instance` unsubscribed and every
hook silently dead. Let `ModelDb` own it.

---

## What this spike does not cover

Deliberately out of scope per the brief, listed so nothing here is mistaken for done:

* **Intake fidelity.** Routing reads the *Continuing* mode rather than the finishing card, and pays
  a flat 10 ml with no elite/boss tiers (§6.2).
* **Leak per floor.** Nothing decays yet — Womb 10%, Ass 5%/25%, Glazed 25%.
* **The Ass tag rule** is implemented on `Clench` only, as flat bonus damage rather than real
  Strength, so it cannot leak into the rest of the turn. Multi-hit scaling is untested.
* **Sigils, Conception, pregnancy** — the counter persists and displays; nothing produces one.
* **Stomach** — persists and displays; `Gulp` and the swallow path do not exist.
* **Precum**, the Mouth generation exception, and the spend order (§2).
* **Relics, rest sites, events, voice hooks, art, balance.**
* **Poison and Doom bar overlays** are not inverted (see §1 caveat 2).

## Cards implemented

Nine, enough to exercise every system under test:

| Card | Type | Tag | Why it is here |
|---|---|---|---|
| Pump ×5 | Attack | *Continuing* | Starting deck; the tag-inheritance case |
| Guard ×4 | Skill | — | Starting deck; a Skill, so untagged |
| Let Go ×1 | Skill | — | Starting deck; the self-damage death path |
| Handful | Attack | Glazed | Freehand; drags Continuing off a hole |
| Lip Service | Attack | Mouth | Draining |
| Clench | Attack | Ass | Squeezing, plus the Ass tag rule |
| Take Him In | Attack | Pussy | Breeding |
| Aching | Curse | — | The Lust card |
| **Taste Test** | Attack | — | **Spike only, not a design card.** The Q3 probe |

`Taste Test` should be deleted when the tag question stops needing a witness.

## Running it again

```
dotnet publish Seductress/Seductress.csproj
```

Deploys `.dll`, `.pdb`, `.json` and `.pck` to `mods\Seductress\`. Godot's export prints exit
code -1 on this machine and the `.pck` is still produced correctly — expected, see
`CharacterTemplate/HOWTO_NEW_CHARACTER.md`.

The game **must be launched from Steam** (`steam://rungameid/2868840`). Launching the exe directly
fails Steam init and no mods load at all.

Every system logs to `%APPDATA%\SlayTheSpire2\logs\godot.log` with a `[Seductress]` prefix —
Climaxes, intake, routing changes and the tag probe. That log is how this spike was verified and is
the fastest way to check a change without playing to the situation.
