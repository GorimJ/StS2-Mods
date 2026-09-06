# The Seductress — engine spike brief

Handoff prompt for a fresh code session. Everything needed is on disk; this file does not
assume you have seen the design conversation.

---

## What you are building

A **mechanics spike**, not the mod. The goal is to answer four engine questions that could each
force a design or architecture change, using the smallest amount of code that produces a real
answer in-game.

Read first:

- `docs/SEDUCTRESS_DESIGN.md` — the mechanical design (v2.0, 59 locked decisions). §10 is the
  risk list this spike exists to retire. §4 (Climax) and §6 (reservoirs) are the parts you will
  implement.
- `docs/SEDUCTRESS_CARDS.md` — the full card list (v10). You need about 8 of these; ignore the
  rest.
- `README_STATUS.md` — workspace state, versions, runner instructions.
- `CharacterTemplate/HOWTO_NEW_CHARACTER.md` — the scaffold you are starting from.

**Do not edit the two design docs.** They are input. If the spike proves something in them is
wrong, say so in the final report and let Gorim decide — the decision register is his.

---

## Environment

- Game **v0.107.1**, BaseLib **v3.4.5**, installed at
  `A:\SteamLibrary\steamapps\common\Slay the Spire 2\`, mods deploy to `...\mods\{Id}\`.
- Decompiles already exist: `decompile_v0107/` (game) and `decompile_baselib_345/` (BaseLib).
  **Grep these before guessing at an API.** They are the ground truth for this version pair.
- `device_bash` has **no dotnet**. Builds, git and launching the game go through the runner:
  write a PowerShell body to `_runner/queue/*.ps1` via `~/q.sh <name>`, which polls for the
  `.out` file. If `start_runner.bat` is not running, ask Gorim to double-click it — do not try
  to work around it.

### Two gotchas that have already cost a rebuild

1. **PowerShell `-replace` is case-insensitive.** Renaming the template with
   `-replace 'CHARACTERTEMPLATE','SEDUCTRESS'` will also rewrite `CharacterTemplate` to
   all-caps and break every identifier and localisation key. Use `-creplace`, uppercase
   patterns first.
2. **Never write `.tscn` or `.gd` with a UTF-8 BOM.** `Set-Content -Encoding UTF8` adds one and
   Godot fails with `Parse Error: Expected '['` at line 1. Use
   `[System.IO.File]::WriteAllText($p, $t, (New-Object System.Text.UTF8Encoding($false)))`.

---

## Where it goes

New mod folder `Seductress/` as a sibling of the other mods in `STS2Modding/`, on the
ModTemplate layout (`Seductress.csproj` + `Seductress.json`, output to `mods\Seductress\`).

The spike **grows into** the real mod — do not use a throwaway ID.

**Use the template's stock visuals.** `SpriteCharTest/` is a separate, already-validated art
proof-of-concept containing sprites extracted from a third-party game; it is gitignored and must
stay that way. Do not reference its scene, do not copy its assets, do not add art to
`Seductress/`. Keeping this spike asset-free is deliberate: it means the code can live in git.

---

## The four questions, in priority order

Answer them in this order — 1 and 2 can force a *design* change, 3 forces an *architecture*
change, 4 is the safest.

### 1. Can the health bar present inverted, as an ascending Arousal bar?

The whole design rests on `MaxHp - CurrentHp` reading as Arousal that fills as you take damage —
for the player *and* for enemies, since an enemy's HP is their arousal and they climax and pass
out at the end of it.

`HealthBarForecastRegistry` is public and can overlay `FromLeft` / `FromRight` segments without
patching, but it cannot reverse the base fill. That needs a Harmony patch on `NHealthBar`.

**Deliver:** a screenshot of the bar filling as the player takes damage, or a clear statement of
why it cannot be done and what the nearest achievable presentation is.

### 2. Can death be intercepted so 0 HP triggers Climax instead of a game over?

At 0 HP the run must not end: arousal resets to 0, +1 Sensitive for the combat, +1 Lust card
permanently. Every HP-loss path must route through it — including the character's own
self-damage cards, which is most of the deck.

**There is vanilla precedent to follow rather than invent.** A relic saves you at 0 HP and sets
you to 50%, and a potion does the same at 30% (Lizard Tail / Fairy in a Bottle equivalents).
Find those in `decompile_v0107/`, see which hook they use, and take the same one. If that hook
is reachable from a mod, this question is answered cheaply.

**Deliver:** dying to a self-damage card and to enemy damage, both producing a Climax rather
than a game over, with the Lust card actually added to the deck.

### 3. Can a mod define its own card tags?

The card list is built on tags: `Mouth`, `Ass`, `Pussy`, `Glazed`, `Deep Throat`, `Continuing`.
`CardTag.Strike` exists in vanilla as a real tag system, so the mechanism is there — the question
is whether a mod can extend the enum or register new members. Other mods appear to do similar
things; check `reference_*/` and the workshop mods in the game's `mods\` folder.

If the answer is no, a lot of card text has to change, so find out early.

**Deliver:** a card that reads "is the last attack played tagged Mouth?" and answers correctly.

### 4. Do custom resources persist and display?

Four ml reservoirs (Glazed, Mouth, Ass, Womb) plus a Sigil count. `ExtendedSaveTypes` should
handle save/load. The unvalidated part is **where they go on screen** — there is no obvious HUD
slot for five numbers.

**Deliver:** reservoirs surviving a save/quit/reload, and a screenshot of them displayed
somewhere legible. A crude debug readout is fine; this is about whether a slot exists at all.

---

## Scope

Implement: the starting deck (5 × Pump, 4 × Guard, 1 × Let Go), one attack per hole, and one
Lust card. About 8 cards. Enough to exercise Continuing, tags, intake and Climax.

**Explicitly out of scope** — do not build these, they will waste the session:

- Sigils beyond a persisting integer, Conception, pregnancy, orbs
- Relics (Plug, Wet Patch), rest-site options, events
- Voice hooks or audio
- Any balance pass — every number in the card list is provisional and will be tuned by playing
- Art, animation, portraits, card frames

---

## Output

A short report at `docs/SEDUCTRESS_SPIKE_RESULTS.md`: each of the four questions marked solved /
solved-with-caveats / blocked, the API actually used, and any design consequence Gorim needs to
rule on. Screenshots where a question is visual.

If a question turns out to be blocked, stop and report rather than designing around it — the
whole point of the spike is to surface that decision while it is still cheap.
