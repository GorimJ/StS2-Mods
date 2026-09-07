# The Seductress — slice 3: the Breeding rework and Leg-Locked

Handoff prompt for a fresh code session. Follows `SEDUCTRESS_SLICE2_RESULTS.md`, which contains the
APIs, the traps and the build/run loop. Read that first.

---

## The goal, stated as a test

**Breeding plays as a four-tier scaling archetype, and a kill claimed by Leg Lock always banks to
the Womb.**

Slice 2 made the character draftable and playable. This slice rebuilds Breeding on the new tier
shape and closes the hole where Breeding's own progression leaked into Glazed.

Read `docs/SEDUCTRESS_DESIGN.md` (**v2.4**) §7.1 and §6.3a, and `docs/SEDUCTRESS_CARDS.md`
(**v13**) §4 and §8. **Re-read them; do not diff from memory** — a lot changed in Breeding and the
card text is the specification. **Do not edit either doc.** If the build proves something in them
wrong, report it; the decision register is Gorim's, and it now runs to 67.

---

## Attribution — read this before writing anything down

Three parties work on this character: **Gorim**, who makes the design calls; the **Cowork session**,
which does design work with him and revises the design docs; and the **code session**, which
implements. Notes that blur them waste Gorim's time — re-explaining his own decisions back to him,
or treating a relayed suggestion as settled fact.

**Use these markers in every doc, code comment and report:**

| Marker | Means |
|---|---|
| **[Gorim]** | Gorim decided this. Settled. Do not re-derive it, re-argue it, or explain it back to him as though it were new. |
| **[Claude-Cowork]** | The design session's own decision. **It signs its own work — do not infer this marker.** Use it only where Gorim has explicitly said something came from Cowork. Design content is authoritative; technical and API claims in it are unverified and should be checked. |
| **[Claude-Code]** | This session's decision, usually made in Gorim's absence. Flag for his review; he has not agreed to it yet. |
| **[verified]** | Checked against `decompile_v0107/`, `decompile_baselib_345/`, or observed in `godot.log`. Say where. |

Anything unmarked is description, not a decision.

**What to verify and what to take as given:**

* **Gorim's design statements — take as given.** When he states a rule, implement it. Do not go
  looking for confirmation in the design docs; they lag his decisions, so "it is not in §8" is not
  evidence against something he has said.
* **Cowork's engine and API claims — verify.** They come from a session that is not building
  against the decompile, and a wrong API claim costs a build cycle. Verifying these is not doubting
  Gorim; they are not his claims.
* **Your own assumptions — always verify.**

**Each session marks its own work.** The code session signs **[Claude-Code]**, the design session
signs **[Claude-Cowork]**, and both mark **[Gorim]**. Do not guess at another session's
attribution — leave it unmarked and let its author claim it, unless Gorim has said explicitly whose
it was.

---

## Where the code is

* `Seductress/` builds clean and is on the ModTemplate layout, asset-free.
* **[Claude-Code] [verified]** Eight changes are built and deployed but have never been played. `dotnet build` auto-copies
  the `.dll` to `mods\Seductress\` via `CopyToModsFolderOnBuild`, so the deployed DLL is current;
  the `.pck` is one publish behind and carries only images and the rest-site loc file. Run a
  `dotnet publish` before the first playtest.
* **[Claude-Code] [verified]** Slice 2 is entirely uncommitted — about 54 changed or new paths. Worth committing before
  starting, so this slice's changes are separable.
* **[Gorim]** Decisions **D4 (minions give no intake)**, **D5 (Breed Me!'s multiplier replaces its
  +5)** and **D6 (multi-elites pay 15 ml)** were all his calls during the slice 2 playtest, and are
  now design decisions 65–67. Nothing built on them needs
  undoing.

---

## What this slice breaks

### 1. [Gorim] Sigil thresholds become 1 / 3 / 5 / 7 — four tiers, not three

`Systems/Sigils.cs` holds the constants and `Sigils.At(player, tier)` reads them. There are
currently three (`Tier1 = 2`, `Tier2 = 4`, `Tier3 = 6`) and three matching `DynamicVar`s registered
on `BreedMe`, `Claim` and `Ward` so the card text prints the numbers. **A fourth is needed
throughout.** Once the constants and vars are right the card text tracks them automatically — that
is what the vars are for.

Beware: `Sigils.Effective(player)` subtracts Sigils exhausted this combat by *Consummate*, and
several cards re-evaluate their tier live rather than snapshotting. Keep that — a snapshot bug cost
a playtest already.

### 2. [Gorim] All eight Breeding cards are rewritten

They now follow a fixed shape: **tier 1 adds an effect, 3 grows the base, 5 grows the effect, 7
repeats a number of times equal to the card's current cost.** 0-cost cards shed their non-energy
cost at tier 7 instead — *Consummate* stops exhausting a Sigil, *Surrender* stops costing HP.

**Take the text from §8 verbatim. Do not adapt the existing implementations** — the shape changed,
not just the numbers.

### 3. [Gorim] Tier 4 rides `ModifyCardPlayCount`, not hand-rolled loops

**[Claude-Cowork] claim, [verified] by the code session:** `AbstractModel.ModifyCardPlayCount(CardModel card, Creature? target, int playCount)`,
whose only vanilla implementer is `DuplicationPower`. The current Breeding cards hand-roll their
repeats with `for` loops around `DamageCmd`; tier 4 must not.

Using the hook means **Powers repeat too**, which loops do not give you. *Ritual Mark* and *Rapture*
have bespoke tier 4s by design choice, not because repeating them was impossible — implement them
as written rather than assuming the generic path was unavailable.

### 4. [Gorim] The starting deck becomes 11 cards

4 × Pump, 4 × Guard, 1 × Let Go, plus two class cards:

* **Breed Me!** — moved out of the draft pool, damage **15 → 12**.
* **Suckle** — new basic: 1 cost, Attack · *Mouth*, deal 4, gain 5 Block.

**Remove the `Take Him In` starting-deck scaffold** added in slice 2 (marked in
`Character/Seductress.cs`). Breed Me! replaces it properly, so the deck now has tagged cards by
design rather than by test hack.

### 5. [Gorim] Rarity moves

*Ward* drops to **Common**. A new Uncommon **Leg Lock** takes the draft slot Breed Me! vacated.

---

## New work

### [Gorim] Leg-Locked — a rules word, not card text

See §6.3a. **While an enemy is Leg-Locked, its death routes to the Womb whatever killed it**,
overriding *both* the finishing attack's tag and the Glazed catch-all.

Get the precedence right — this is the whole point. It is the fix for Poison, reflect damage and
the Sigil passive quietly leaking Breeding's progression into Glazed, and it only works if the
Leg-Locked check runs **before** the finishing-tag lookup and before the Glazed fallback. In the
current code that means testing it at the top of `ClimaxModel.AfterDeath`, ahead of the
`ContinuingHole` read.

Shape: a **Counter** power that counts down at the **end of the enemy's turn** — the same shape as
`DebilitatePower`, which decrements in `AfterSideTurnEnd` when its owner is among the participants.

**[Gorim] The minion rule wins over Leg-Locked.** A Leg-Locked minion banks nothing — minions
providing resources is design-breaking, and no kill-claim effect may override that. Leg-Locked
overrides the *routing* of intake, never whether intake happens at all. Check the minion test
first, then Leg-Locked, then the finishing tag, then Glazed.

### [Gorim] Leg Lock applies vanilla `DebilitatePower`

**[Claude-Cowork] claim, [verified] by the code session:** a plain `PowerModel`, not Necrobinder-gated; the only vanilla card referencing it is
`Debilitate` (`decompile_v0107/MegaCrit.Sts2.Core.Models.Powers/DebilitatePower.cs`). So `PowerCmd.Apply<DebilitatePower>(...)` works directly.

**Its stacks are duration, not intensity.** The multiplier is `amount + (amount - 1)` applied
**once**, however many stacks are held, and both its Vulnerable and Weak modifiers bail unless
`props.IsPoweredAttack()`. Do not expect stacking to scale the effect, and do not expect it to
touch unblockable or unpowered damage.

---

## Two cautions — [Claude-Cowork], per Gorim

* **[Claude-Cowork] Do not tune tier 4.** The "4–6 Sigils per run" target is unverified since the multi-elite rate
  dropped to 15 ml (D6), so nobody yet knows whether 7 Sigils is a stretch goal or unreachable.
  1 / 3 / 5 is safe either way; 7 is not yet a number anyone can reason about.
* **[Claude-Cowork] Consider dropping the Sigil scaffold relic from 2 to 1**, so it demonstrates exactly tier 1
  rather than overshooting it. `Relics/SigilScaffold.cs`, marked as test scaffolding.

---

## Out of scope

Unchanged from slice 2, and still deliberate:

* Conception, pregnancy, orbs, and the Sigil passive beyond a counter and its thresholds
* Stomach, `Gulp`, the swallow path, precum, the Mouth generation exception, the spend order
* The Plug and Wet Patch relics
* Voice hooks, audio, art, portraits
* Any balance pass — that is what a played run is for

---

## Traps, carried forward

Every one of these cost time already:

* **`dotnet publish Seductress/Seductress.csproj`.** Godot prints exit code -1 and the `.pck` is
  still correct. The game must be **closed** or the DLL copy fails — and that failure is `MSB3021`,
  not `error CS`, so grep for both.
* **Launch from Steam** (`steam://rungameid/2868840`). The exe direct fails Steam init and loads no
  mods at all.
* **Do not construct a `CustomSingletonModel` yourself.** `ModelDb.Init` instantiates it; a second
  instance silently unsubscribes the live one.
* **Routing must be set in `BeforeCardPlayed`, not `AfterCardPlayed`** — a tagged attack that lands
  the killing blow fires `AfterDeath` from inside its own resolution.
* **Card text:** paired colour tags only (`[gold]`, `[blue]`); `{Energy:energyIcons()}` for the
  energy icon; never write a vanilla keyword in the description, since keyword lines render from
  `CanonicalKeywords` and you will get it twice.
* **One unregistered `{token}` makes SmartFormat print the entire description raw**, including the
  tokens that *were* valid. If a card shows `{Damage}`, look for an unregistered var elsewhere in
  the same string.
* **Screenshots must go to a file path** — `docs/spike-screenshots/`. Win+PrtScn saves
  automatically; Win+Shift+S only reaches the clipboard and is lost.
* **Instrument first, then have Gorim play, then read the log.** Claude cannot drive the game.
  `tail -F` the log filtered to `[Seductress]` floor summaries and errors — per-kill lines are too
  noisy. Two of slice 2's seven bugs were only findable this way, and both came from Gorim
  questioning a number.

---

## Output

Start `docs/SEDUCTRESS_SLICE3_RESULTS.md`: what was built, what was deferred and why, any card text
that had to change to be implementable, and anything the design docs get wrong.

Mark every decision in it **[Gorim]**, **[Claude-Cowork]** or **[Claude-Code]** per the convention above, so the next session
can tell at a glance what is settled and what still needs his ruling.

Then tell Gorim it is ready to play, and what to watch for.
