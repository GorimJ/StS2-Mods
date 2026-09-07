# The Seductress — slice 2: a playable run

Follows `SEDUCTRESS_SPIKE_RESULTS.md`. Built against `SEDUCTRESS_DESIGN.md` v2.1 and
`SEDUCTRESS_CARDS.md` v10. Game **v0.107.1**, BaseLib **v3.4.5**.

Neither design doc has been edited. Everything below that disagrees with them is reported, not
applied.

**Status: built and in playtest. Seven bugs found and fixed during play; three design points
clarified or newly decided.**

---

## What was built

### 1. Rest-site relief — the unblocker

`ReliefRestSiteOption : CustomRestSiteOption`, added through the vanilla
`TryModifyRestSiteOptions` hook (the same one Shovel uses for Dig). Removes 1 Lust card and does
not heal. No per-act cap, per §5 — the opportunity cost is the limiter.

It removes the *first* Lust card found rather than offering a choice. With only two Lust cards and
no way to tell them apart in value, a selection screen would be a click with no decision in it.
Worth revisiting if a third Lust card ever differentiates them.

Greyed out but still hoverable when there is nothing to remove, matching how Smith behaves with no
upgradable cards. **Confirmed in play 2026-09-07** — hovering it shows "You have no Lust cards to
remove."

**Caveat found the same day:** the option had **no icon**, because `RestSiteOption.Icon` derives its
path from `OptionId` and `option_seductress_relief.png` does not exist. The `AssetLoadException`
fails the texture without removing the node, so the button was present and functional but drawn
blank — easy to miss beside three illustrated buttons. Fixed in slice 3 by overriding
`CustomRestSiteOption.CustomIconPath`.

### 2. Per-floor leak

Applied on entering a new floor, keyed on `RunState.TotalFloor` so re-entering a room does not
double-charge. Rates from §6.2 and §6.4: Womb 10%, Ass 5% with 25% on the excess above 200,
Glazed 25%, Mouth 0%.

**Leak rounds down**, deliberately mirroring intake's round-up so both favour the player. One
consequence worth knowing: anything below 10 ml in the Womb never decays at all, so a trickle is
sticky. If that reads wrong, the fix is a minimum-1 floor on the drain.

### 3. Max HP and starting state

Max HP **65**, starting deck 5 × Pump / 4 × Guard / 1 × Let Go, both from §12. **Provisional** —
§12 lists Max HP as proposed-not-confirmed, and nothing here validates it.

### 4. The card pool

**36 draftable cards plus the 2 Lust cards.** Every group is now deep enough to draft from.

| Group | Implemented | Deferred |
|---|---|---|
| Basics | Pump, Guard, Let Go | — |
| Core | Second Wind, Bat Eyelashes, Abandon, Afterglow | — |
| Freehand | Handful, Smothered, Thighplay, Show Off | Wet Work |
| Draining | Throat Fuck, Deep Throat, Balls Deep, No Gag Reflex, Lip Service, Scoop It Up, Spit, Restraint | Lick Pre-cum, Gulp, Milked Dry |
| Squeezing | Clench, Stuffed, Grind, Rut, Stretch My Hole Daddy, Milking Grip, Overflow, Blowout | — |
| Breeding | Take Him In, Breed Me!, Surrender, Claim, Ward, Ritual Mark, Consummate, Rapture | — |
| Ancient | Edging Form | — |
| Lust | Aching, Craving | — |

Ten supporting Powers were written alongside them.

### 5. Instrumentation

One line per floor, emitted when the floor is left so entry and exit arousal are both known:

```
[Seductress] FLOOR 7 Monster | arousal 12->31/65 | climaxes 2 (+1) | lust 2/3 |
             mouth 10 ass 40 womb 55 glazed 0 | sigils 2 | intake +30 | leak -8 | relief no
```

Plus per-event lines for Climax, intake, leak, relief, routing changes and every Ass spend.

### 6. Housekeeping

* `Taste Test` deleted.
* **Poison and Doom overlays inverted** (risk 2a). Their meaning inverts cleanly — vanilla draws
  the HP you are about to lose at the right-hand end of the fill; here the same effects are arousal
  you are about to *gain*, so the segment extends rightward from the current arousal edge. Both
  offsets are now set explicitly, because vanilla only writes `OffsetLeft` in its lethal branch and
  would otherwise leave a stale value behind.

---

## Scaffolding — remove when Conception lands

Both are marked in-code with a banner comment naming what removes them.

* **`SigilScaffold`** ("Fertile Sign") — a starting relic granting 2 Sigils. `IsAllowed` returns
  false so it can never appear in a reward pool.
* **`SigilsConsoleCmd`** — `sigils <n>` sets the count directly. The only practical way to reach
  tiers 2 and 3 without a full run of pregnancies.

**What this costs, stated plainly.** With 2 Sigils from floor 1, Breeding sits at roughly its
act-2 power throughout act 1. **A run played with this relic cannot tell us whether Breeding's
intended early-game weakness feels right** — by design Sigils arrive from elite and boss
pregnancies and the archetype should feel thin early. That remains untested and is not evidence
either way.

---

## Things the design docs get wrong or leave open

### A. [Claude-Code] §7.1 and the card list disagree on Sigil thresholds — **needs a ruling**

`SEDUCTRESS_DESIGN.md` §7.1 specifies **two tiers at 3 / 6**, and §12 lists those values under
*Proposed, not confirmed*. `SEDUCTRESS_CARDS.md` v10 writes **every** Breeding card with **three
tiers at 2 / 4 / 6**, and the slice-2 brief inherits that — its scaffold reasoning ("Tier 1 is then
live from floor 1") only works if tier 1 is 2.

**Built to 2 / 4 / 6**, because that is what the concrete card text says and what the brief
assumes. The values are constants in `Systems/Sigils.cs`, so switching is one edit — but the card
*text* would also need rewriting, so this is worth settling before the numbers get playtested.

### B. [Claude-Code] The Sigil passive does not exist, so *Ritual Mark* is half inert

§7.0 gives Sigils a passive (1 damage to ALL enemies at the start of your turn). The brief puts it
out of scope, so it is not built. *Ritual Mark* reads "your Sigils **and** your Pussy cards deal 1
additional damage" — only the second half does anything right now. The card is still worth drafting
in a Pussy deck, but it is weaker than its text implies and should not be judged on this run.

### C. Card text that had to change to be implementable

Only one, and it is cosmetic. **Bat Eyelashes** is written as a Skill targeting ALL enemies; the
upgrade adds Vulnerable. No text change was needed — but note that the base card and its upgrade
now differ by a hidden flag rather than a dynamic var, so the card's description does not update
when upgraded. Same pattern on *Spit*, *Overflow* and *No Gag Reflex*. Cosmetic, and easy to fix
with a proper var if it grates in play.

### D. Scope notes on two mechanics

* **The Ass rule is implemented as flat per-hit bonus damage, not real Strength.** For the card's
  own hits this is arithmetically identical — Strength adds to each hit too — and it cannot leak
  into the rest of the turn, which "for that attack" requires. It will *not* interact with
  Strength-doubling effects, and it does not show in the UI.
* **`Stretch My Hole Daddy` only affects this character's own cards.** A vanilla or cross-mod
  attack that costs HP will not gain the extra hit. That covers the whole intended pool.

### E. Deferred cards and why

| Card | Blocked on |
|---|---|
| Wet Work | The spend order (Glazed → Mouth → Ass → Womb), §2 |
| Lick Pre-cum | Precum |
| Milked Dry | Precum |
| Gulp | Stomach and the swallow path |

Also inert: the **Deep Throat** tag's auto-swallow. All three Deep Throat cards exist and carry the
tag, and *No Gag Reflex* reads the family correctly — but "climaxes caused by this card are
swallowed immediately" needs Stomach, so the tag currently does nothing beyond marking the family.

---

## Bugs found in playtest and fixed

### 1. Tagged kills routed to the wrong hole — *the significant one*

The routing mode was set in `AfterCardPlayed`, which fires **after** the card fully resolves,
including any deaths it causes. So a tagged attack that landed the killing blow triggered
`AfterDeath` from inside its own resolution and routed by the **previous** mode. The card set its
own tag a moment too late to benefit from it.

Found in play: a Deep Throat kill banked to **Glazed** instead of Mouth. Gorim noticed the Stomach
was empty and asked; the Stomach part was expected, the routing was not.

This is the common case, not an edge — a tagged attack killing something is how a reservoir fills
at all, and §6.0 is explicit that "the attack that finished them routes their output via its
tag". The spike results had flagged the read-the-mode approach as a simplification; that
understated it.

**Fixed** by moving the mode to `BeforeCardPlayed`. Verified in play: a Take Him In kill now banks
`womb 10, glazed 0` where it previously banked to Glazed.

### 2. Per-floor leak was charged to the wrong floor

Leak was applied on room entry *before* the previous floor's line was emitted, so that line showed
reservoir levels the player never actually left the floor with, and the loss was attributed one
floor early. Self-consistent but shifted — which would have quietly corrupted the exact data this
slice exists to produce.

**Fixed** by reordering to end-floor, begin-floor, then leak.

### 3. `Handful` printed its keywords twice

Keyword lines render automatically from `CanonicalKeywords`, above and below the body text. Naming
Innate and Exhaust in the description as well printed each twice. **Fixed.** Worth knowing for any
future card: never write a vanilla keyword into the description. (Consummate's "Exhaust a Sigil" is
the verb, not the keyword, and is fine — it declares no keywords.)

### 4. No way to see that Consummate had worked

The HUD showed the stored Sigil count, not the effective one, so exhausting a Sigil was invisible.
**Fixed:** the line now reads `Sigils 1/2 (-1 exhausted)` when any are spent.

### 5. Intake ignored the §6.2 encounter tiers — *the other significant one*

Every enemy paid a flat **10 ml** regardless of encounter. Elites should pay 20 each (multi) or 40
(solo), and bosses 60.

This was carried forward from the spike, where it was documented as a simplification, and I fixed
only the routing half of it this slice — the flat rate went unexamined even though the brief said
to read the rates from §6 rather than from memory. I did that for leak and not for intake.

It is worse than a wrong number. §6.3 makes **elites and bosses the pregnancy moments**, and the
whole 1–2-pregnancies-per-act target rests on those tiers. At a flat 10 the Womb filled at roughly
a third of the intended rate, so nothing about pregnancy pacing was measurable.

**Fixed**, and the intake log line now shows its working —
`Intake 60 ml -> Pussy (base 20, sensitive x1.0, Elite)` — so a wrong number is diagnosable
without arithmetic.

Solo-vs-multi is counted as **living non-minion enemies plus non-minion monsters already killed
this combat**, rather than sampled at turn boundaries. The first attempt sampled on
`AfterSideTurnStart`, which would have read zero before the first turn tick and priced a
multi-enemy elite as *solo* — the expensive direction.

### 6. `Take Him In`'s cost tier was a snapshot, not a live check

The 6-Sigil "costs 0" clause was applied once in `AfterCardEnteredCombat`, so a card already in
hand never noticed the threshold being crossed — whether by the console command or by Consummate
exhausting a Sigil mid-fight. **Fixed** to re-evaluate on draw and at the start of each player turn.

### 7. Sigil-tier card text printed raw

`Breed Me!`, `Claim` and `Ward` used `{Tier1}`/`{Tier2}`/`{Tier3}` placeholders that were never
registered as dynamic vars. One unknown token makes SmartFormat abandon the whole string, so the
cards displayed their entire description unformatted, `{Damage}` included. **Fixed** by registering
the three thresholds as vars reading from the `Sigils` constants, so the text tracks the numbers.


## Changes made during playtest at Gorim's request

* **`Take Him In` added to the starting deck**, marked as test scaffolding. The designed starting
  deck contains no tagged card at all, so nothing routes anywhere until the first draft, which made
  the reservoirs untestable from floor 1. The starting deck is not final; remove when it is.

* **`Consummate` reduced from 2 energy to 0.** At 2 its net energy was zero, so the card was "draw
  3, and lose a Sigil for the combat" — a **Rare strictly worse than StS1's Common Skim**, and it
  played that way. Note it was weak even with its downside *understated*, since the Sigil passive
  is not built and exhausting one currently costs almost nothing. Worth re-checking once the
  passive exists; it may want to be 1. `SEDUCTRESS_CARDS.md` still says 2 — deliberate divergence.

## New decisions taken during playtest

Neither design doc covers these. All three are **Gorim's calls**, made while playing; recorded here
for the register rather than proposed.

**Attribution key used throughout:** **[Gorim]** = his decision, settled. **[Claude-Cowork]** = from
the design session, relayed. **[Claude-Code]** = this session's decision, needs his review.
**[verified]** = checked in the decompile or observed in the log.

### D4. [Gorim] Minions produce no intake

**[decided 2026-09-06, Gorim]** A minion kill banks nothing, and minions do not count toward
encounter size — so an elite that summons adds stays a **solo** elite at 40 ml rather than being
demoted to the 20-each multi rate.

Implemented against `Creature.IsSecondaryEnemy`, which is the game's own definition: an enemy
carrying `MinionPower` or `IllusionPower`, which cannot survive without a living primary. That is
firmer than a hand-maintained list of monster ids.

**Why it matters beyond tidiness.** Without it any summoner is an intake fountain — minions are
individually cheap to kill and often respawn, so they would pay better per unit of effort than the
enemy summoning them, and Breeding would want to farm adds rather than kill elites.

### D5. [Gorim] Breed Me!'s Sigil multiplier replaces its +5, rather than compounding

**[clarified 2026-09-06, Gorim]** At tier the intake is `base × 2` or `base × 3`; the flat +5 only
applies below tier 1.

This contradicts `SEDUCTRESS_CARDS.md` §8, which says the clauses "double or triple the card's
**total** kill intake **including that 5**". Built to the clarification. Resulting table, before the
Sensitive multiplier:

| | Normal | Elite (multi) | Elite (solo) | Boss |
|---|---|---|---|---|
| Base | 10 | 15 each | 40 | 60 |
| Breed Me!, 0–1 Sigils | 15 | 20 | 45 | 65 |
| Breed Me!, 2–3 Sigils | 20 | 30 | 80 | 120 |
| Breed Me!, 4+ Sigils | 30 | 45 | 120 | 180 |

A 3-enemy elite finished with Breed Me! at 4+ Sigils is 180 ml against a 100 ml Womb, so §6.3's
"elites and bosses are the pregnancy moments" is now true in the build for the first time.
**Overflow above 100 is discarded**, since pregnancy is not implemented — fine for testing fill
rate, but it means the size of an overshoot cannot be measured.

### D6. [Gorim] Multi-enemy elites pay 15 ml each, not §6.2's 20

**[decided 2026-09-06, Gorim].** `SEDUCTRESS_DESIGN.md` §6.2 says 20 each; built to 15.

**This makes §6.3 stale and the doc should be updated.** Per kill, ×`(1 + 0.5 × Sensitive)`,
rounded up:

| Encounter | 0 stacks | 1 stack | 2 stacks |
|---|---|---|---|
| Elite, 3 enemies | 45 | **69** | 90 |
| Elite, solo | 40 | 60 | 80 |
| Boss | 60 | 90 | 120 |

§6.3 currently states **60 / 90 / 120** for a three-enemy elite, and uses that to conclude "elites
and bosses are the pregnancy moments". At 15 ml a three-enemy elite **no longer completes a
pregnancy with one Sensitive stack** — 69 against a 100 ml Womb, where the doc says 90. The
"1–2 pregnancies per act, 4–6 Sigils per run" target in §6.3 is derived from the old rate and no
longer follows.

The ordering still works: a three-enemy elite (45) beats a solo one (40), so clearing more enemies
remains better — but the margin is now 5 ml rather than 20, which makes the choice between elite
types nearly neutral rather than clearly favouring crowds.

## Observations from play, not acted on

* **Breeding's Sigil tiers are hard to read as a group.** Every card's tiers do something different
  — draw on *Take Him In*, damage-then-HP-relief on *Claim*, extra hits on *Breed Me!*, Block and
  Thorns on *Ward*. There is no pattern to anchor on, so "what does another Sigil get me" is not
  answerable at a glance, and a tier can be misread as raising damage when it does not. Not a bug,
  and nothing was changed — noted for the design pass.

---

## How to run it

```
dotnet publish Seductress/Seductress.csproj
```

Launch from Steam (`steam://rungameid/2868840`). Console commands that matter:
`sigils <n>`, `card SEDUCTRESS-<CARD_ID>`, `room RestSite`, `damage <n> 0`, `die`.
