# The Seductress — slice 3: Breeding rework and Leg-Locked

Follows `SEDUCTRESS_SLICE2_RESULTS.md`. Built against `SEDUCTRESS_DESIGN.md` **v2.4** §7.1 / §6.3a
and `SEDUCTRESS_CARDS.md` **v13** §4 / §8. Game v0.107.1, BaseLib v3.4.5.

Neither design doc has been edited.

**Status: built, compiles clean, deployed. Not yet played** — no testing this session by Gorim's
instruction.

**Attribution:** **[Gorim]** = his decision, settled. **[Claude-Cowork]** = the design session's,
where Gorim said so explicitly. **[Claude-Code]** = this session's, needs his review.
**[verified]** = checked in the decompile or the log.

---

## What was built

### [Gorim] Sigil thresholds are 1 / 3 / 5 / 7

`Systems/Sigils.cs` now carries four constants and every gated card registers four dynamic vars
(`T1`–`T4`) reading from them, so the card text tracks the numbers rather than hard-coding them.
Changing a threshold is one edit and the text follows.

### [Gorim] All eight Breeding cards rewritten to the fixed ladder

Taken from §8 text rather than adapted from the v10 implementations. Every card now reads
**1 adds an effect · 3 grows the base · 5 grows the effect · 7 repeats** — with 0-cost cards
shedding a non-energy cost at tier 4 instead.

| Card | Rarity | Tier 4 |
|---|---|---|
| Take Him In | Common | repeats (cost) |
| Surrender | Common | 0-cost — loses the HP cost |
| Ward | Common | repeats (cost) |
| Claim | Uncommon | repeats (cost) |
| Leg Lock | Uncommon | bespoke — applies to ALL enemies |
| Ritual Mark | Uncommon | bespoke — both bonuses become +1 per Sigil |
| Consummate | Rare | 0-cost — stops exhausting a Sigil |
| Rapture | Rare | bespoke — doubles the energy |

Rarity spread is **3 Common / 3 Uncommon / 2 Rare**, matching §8.

*Claim*'s v10 "stop costing HP at 4 Sigils" clause is gone; the 2 HP persists at every tier.

### [Gorim] Tier 4 rides `ModifyCardPlayCount`

**[verified]** `AbstractModel.ModifyCardPlayCount(CardModel, Creature?, int)`, the hook
`DuplicationPower` uses. Implemented once on `SeductressCard` behind a `RepeatsAtTier4` flag, so
Powers would repeat correctly too if a repeating Power is ever added. No hand-rolled loops.

The repeat count is `EnergyCost.GetResolved()` — the card's **current** cost — so a card that cut
its own cost at a lower tier correctly repeats fewer times, per §7.1.

### [Gorim] Leg-Locked

`Powers/LegLockedPower.cs` — a Counter debuff decrementing in `AfterSideTurnEnd` when its owner is
among the participants, i.e. at the end of the enemy's own turn. Same shape as `DebilitatePower`
**[verified]**.

**Precedence in `ClimaxModel.AfterDeath`, in order:**

1. **Minions bank nothing** (slice 2 D4) — returns before anything else.
2. **Leg-Locked** → routes to Pussy.
3. The finishing attack's tag.
4. Glazed catch-all.

**[Gorim]** The minion rule wins outright: Leg-Locked overrides *where* intake goes, never
*whether* it happens. A Leg-Locked minion banks nothing.

### [Gorim] Leg Lock applies vanilla `DebilitatePower`

**[verified]** generic, not Necrobinder-gated — a plain `PowerModel` referenced only by the vanilla
`Debilitate` card. Its stacks are duration, not intensity, and it only fires on powered attacks, so
tier 3 buys two enemy turns of doubled Vulnerable rather than double the doubling. The card text
says "2 Debilitate", which is correct as duration.

### [Gorim] Starting deck is 11 cards

4 × Pump, 4 × Guard, 1 × Let Go, 1 × **Breed Me!** (moved out of the draft pool, `CardRarity.Basic`,
damage 15 → 12), 1 × **Suckle** (new: 1 cost, Attack · Mouth, deal 4, gain 5 Block).

The slice-2 `Take Him In` starting-deck scaffold is **removed**.

### [Claude-Cowork, per Gorim] Sigil scaffold relic drops 2 → 1

`SigilScaffold` grants 1 Sigil, so it demonstrates exactly tier 1. Still marked test scaffolding,
still `IsAllowed => false`, still removed with Conception alongside `SigilsConsoleCmd`.

---

## Not done, deliberately

* **[Claude-Cowork, per Gorim] Tier 4 is untuned.** Implemented exactly as §8 specifies, with no
  numbers invented or adjusted. The 4–6 Sigils per run target is unverified since the multi-elite
  rate dropped to 15 ml (slice 2 D6), so whether 7 is a stretch goal or unreachable is still
  unknown.
* Everything in slice 2's out-of-scope list stands **except the Sigil passive**, which Gorim asked
  for during this session and is now built (below): Conception, pregnancy, orbs as a channel/evoke
  loop, Stomach / Gulp / precum / the spend order, the Plug and Wet Patch relics, art and audio.

---

## Things worth Gorim's eye

### [Gorim] The Sigil passive is now built, so Ritual Mark is whole

Added after the rest of the slice, at Gorim's request, because two of Ritual Mark's four rungs were
otherwise inert.

`Orbs/SigilOrb.cs` — a `CustomOrbModel` fixed at **1 damage to all hittable enemies at the start of
your turn** (§7.0), rather than the Glass Orb's decaying 4. *Ritual Mark* raises it through
`RitualMarkPower.SigilPassiveBonus(...)`, so its tier 1 (+1), tier 3 (+4) and tier 4 (+1 per Sigil)
are all live.

**Display.** [Gorim] Vanilla's Lightning Orb scene, instantiated and tinted pink via `Modulate`, so
the mod stays asset-free. **Orb capacity is pinned to the effective Sigil count**, so there are
never empty slots implying a cap — §7 is explicit that Sigils are not Defect orbs, and the queue is
borrowed purely as a display. Nothing in the pool channels or evokes one.

Capacity is re-synced in `BeforeSideTurnStart`, ahead of the queue's own turn-start trigger, so the
passive fires on turn one and mid-combat changes are picked up. *Consummate* exhausting a Sigil
removes its orb immediately, which is what §7 means by losing the passive until the fight ends.

**This makes Leg-Locked matter.** The passive deals damage with no card source, so a kill it lands
has no finishing tag and falls through to Glazed — precisely the leak §6.3a describes, and now
reproducible rather than hypothetical.

**Confirmed in play** (`docs/spike-screenshots/slice3-sigil-orb-tooltip.png`): the orb renders pink
with its count, the passive applies 1 at turn start, the 11-card starting deck is dealt with both
class cards, and the scaffold grants exactly 1 Sigil.

**Bug found and fixed in the same screenshot:** the orb tooltip printed `{EvokeAmount}` raw and
described the evoke rather than the passive. Orb loc variables are `{Passive}` and `{Evoke}`, and
**only the smartDescription receives them** — `OrbModel.Description` is documented as taking no
variables, so a token there prints raw. Both lines now describe the passive, since nothing evokes a
Sigil. Fixed and compiled; **not yet seen in game**, as the deploy needs a restart.

**Caveats:** `Modulate` multiplies, so the tint darkens as well as colours — a placeholder, not
final art. The tooltip *icon* is a plain texture and cannot be tinted the same way, so it stays
lightning-coloured. And the orb sync's shrink path (capacity dropping when a Sigil is exhausted) is
**built but never run** — no testing this session.

### [Claude-Code] Relief had no icon and drew blank

`RestSiteOption.Icon` derives its path from `OptionId`, so Relief asked for
`res://images/ui/rest_site/option_seductress_relief.png`, which does not exist. The resulting
`AssetLoadException` fires inside `NRestSiteButton._Ready`.

**It aborts `NRestSiteButton.Reload()` partway.** That method runs, in order:

```csharp
_icon.Texture = Option.Icon;               // throws here
_label.SetTextAutoSize(Option.Title...);   // never runs
if (!_option.IsEnabled) { grey shader }    // never runs
```

So the button kept the scene's **default label and art** ("Dig"), never greyed out, and yet behaved
correctly — unclickable, with the right hover text — because `_isUnclickable` and the description
are set outside `Reload`. One missing asset produced a button that looked like an unrelated vanilla
option, looked enabled, and did nothing.

`CustomRestSiteOption.CustomIconPath` exists for exactly this and was never overridden. Now points
at the template's placeholder relic art, deliberately not a vanilla rest-site icon, which would
read as one of the existing options.

**A note on how this was diagnosed**, since it went into the record wrong twice. From the stack
trace alone the conclusion was "the button never rendered" — wrong. Corrected to "only the icon
failed, the button drew blank" — also wrong; it was wearing the prefab's default face. Only reading
`Reload()` line by line, against three observations from play (my hover text, a "Dig" label with no
Shovel relic in the save, and a click that did nothing), gave the actual behaviour.

The lesson is narrow and worth keeping: **an exception mid-`_Ready` leaves a node in a partially
initialised state, not an obviously broken one.** Reason about which assignments ran before the
throw rather than assuming the node is absent or blank.

### [Claude-Code] Three card upgrades are still invisible

STS2 has **one description key per card and no upgraded variant**, so an upgrade can only be shown
by moving a *number*, and that number has to be a dynamic var. Wording never changes.

Four cards were written with a hidden `_upgraded` bool instead, making their upgrades invisible on
the card face. *Spit* is fixed — its half-ml became a `{Portion}%` var, 50 upgrading to 100, so the
change now shows and diffs.

**Still invisible, because their upgrades add a clause rather than move a number:**

| Card | Upgrade |
|---|---|
| Bat Eyelashes | also applies 1 Vulnerable |
| Overflow | also applies 1 Weak |
| No Gag Reflex | Deep Throat cards also draw 1 |

Each works mechanically; none shows any difference on the upgraded card. Two ways out, both
Gorim's call: make the upgrades numeric instead (Bat Eyelashes 1 → 2 Weak, Overflow 1 → 2
Vulnerable), which is on-convention and self-documenting but changes the design; or find how
vanilla presents a clause-adding upgrade, which I have not located — every reference card carries
exactly one description key.

### [Gorim] Ritual Mark's upgrade lifts the Sigil bonuses too

§8's upgrade note is "+3 base", which moved only the Pussy numbers. That broke the parity the card
text depends on: at 5 Sigils it claimed *"Sigils deal 4 too"* while Pussy cards were on 7.

**[Gorim 2026-09-07]** the upgrade lifts the Sigil side as well. All four numbers now move by +3,
so the top rung matches again:

| | Base | Upgraded |
|---|---|---|
| Pussy cards | 2 | 5 |
| Pussy at 3 Sigils | 4 | 7 |
| Sigils at 1 Sigil | 1 | 4 |
| Sigils at 5 Sigils | 4 | 7 |

The two Sigil values were hard-coded ints and are now dynamic vars, so they both upgrade and
display.

### [Claude-Code] Tiered damage numbers get no modifier preview

`DamageVar.UpdateCardPreview` runs `Hook.ModifyDamage` over the registered `Damage` var only, so
the number on a card folds in Strength, Weak and enchantments. The tier-swapped values —
`{TierDamage}` on *Claim*, *Take Him In* and *Breed Me!* — are plain `DynamicVar`s, so they display
raw.

At 3+ Sigils with any damage modifier up, those cards therefore **display an unmodified tier number
while dealing a modified one**. Purely cosmetic, but it reads exactly like a bug.

Fixable by registering the tier values as `DamageVar`s rather than plain vars, which would give
them their own preview. Not done — worth doing alongside whatever the design pass decides about
tier text.

### [Gorim] The Ass rule converts 1 ml to 1 Strength, not 1 per 10 ml

**[Gorim 2026-09-07]** §6.1 states "+1 Strength per 10 ml lost" and works it as 200 ml → +2.
Changed to **one-to-one**: 10 ml is +1, 200 ml is +20.

**Why.** At 1:10 the bank never paid off. Ass leaks per floor *and* per Ass-tagged attack, so it
rarely reaches the 100 ml where the old ratio produced its very first point of Strength — the
archetype's signature mechanic was inert for most of a run.

**Consequence, accepted.** The 10× lands multiplied on the cards that change the drain percentage.
At 200 ml Ass:

| Card | Drain | Bonus | Result |
|---|---|---|---|
| Clench | 10% | +20 | 2 hits of 25 = 50 |
| Rut | 10% | +20 | 3 hits of 26 = 78 |
| Overflow | 20% | +40 | 41 to ALL |
| Blowout | 50% | +100 | 4 hits of 106 = **424** |

§7's worked examples put Blowout at 64 and Overflow at 5. **[Gorim] Left as is** — the bank is a
long-term resource, spending half of it buys one turn, and the drain means it is not something you
can do every fight.

**On the economy** — an earlier draft of this note claimed *Milking Grip* might make refill fast
enough to undercut the "not every fight" reasoning. **[Gorim] That was modelled wrong.** Drain
scales with **attacks**; refill scales with **kills**. At 200 ml against a normal enemy:

| | Spend / attack | Intake / kill | Net at 1 attack per kill | at 2 attacks per kill |
|---|---|---|---|---|
| No Milking Grip | 20 | 10 | −10 | −28 |
| Milking Grip | 10 | 20 | +10 | ≈0 |

So Milking Grip takes the archetype from bleeding to roughly break-even, and only gains when
enemies die to a single attack. It is not a runaway.

**The rule is self-limiting**, which supports leaving Blowout alone. Because the drain is a
*percentage*, the absolute spend shrinks with the bank, so the Strength tapers within a fight
without ever emptying it. From 200 ml with no kills: **+20, +18, +17, +15, +13**.

**Still worth watching:** whether a Squeezing deck can reach 200 ml at all inside act 1, given the
per-floor leak and the per-attack drain. [Gorim] expects not to engage with the mechanic in act 1
under the old ratio; the 1:1 change is meant to fix exactly that, and is unverified.

### Open design notes from playtest — not acted on

Both raised by Gorim in play; neither settled, so nothing was changed.

* **Let Go may not belong in the starting deck.** Noted that it is a third non-basic card alongside
  Breed Me! and Suckle. Consequence worth carrying into the design pass: cutting it leaves the
  starting deck with **no self-damage at all**, which is the character's blood-magic axis — Climax
  would become something that only happens *to* you until the first draft, the same gap §4's
  two-class-card argument was written to close. The deck would drop to a clean 10, matching Ironclad.
* **Suckle is too plain for a starter** and should engage a mechanic. It came out plain for a
  structural reason: it is the Mouth half, Skills are untagged so it had to be an Attack, and every
  mechanic Mouth engages with — Stomach, precum, the spend order, *Gulp* — is unbuilt. Gorim is
  weighing precum, an Ass card despite earlier reservations, or a third starting card.

### [Claude-Code] Namespaces are flat

Generated card files all declare `namespace Seductress.SeductressCode.Cards` regardless of which
folder they sit in (`Basics/`, `Breeding/`, …). Consistent with the slice-2 cards and harmless, but
it means folder and namespace disagree. Worth tidying if it ever grates.

### [Claude-Code] `Rapture`'s tier 2 cost reduction is live, not snapshotted

It re-evaluates on card-entered and on draw, the same pattern the slice-2 `Take Him In` cost bug
forced. Rapture does not repeat at tier 4, so the reduced cost has no interaction with the repeat
count — but if it ever gains one, the interaction in §7.1 ("a card that reduced its own cost has
already banked that value") applies automatically, since the repeat reads the resolved cost.

---

## How to run it

```
dotnet publish Seductress/Seductress.csproj
```

Already deployed. Launch from Steam (`steam://rungameid/2868840`).

Console commands: `sigils <n>` (test scaffold), `card SEDUCTRESS-<CARD_ID>`, `room RestSite`,
`damage <n> 0`, `die`.

**What to exercise first when testing resumes:**

1. **Leg-Locked precedence.** Leg Lock an enemy, then kill it with something untagged — Pump before
   any tagged attack, or a Sigil-passive-style effect. It must bank to Womb, not Glazed. Then
   Leg Lock a *minion* and confirm it banks nothing.
2. **Tier 4 repeats.** `sigils 7`, then play Take Him In (1 cost → repeats once, so twice total) and
   Breed Me! (2 cost → three times total). Watch for the repeat firing through
   `ModifyCardPlayCount` rather than the card looping.
3. **Tier 1 on the first Sigil.** With the scaffold at 1, every Breeding card's tier 1 should
   already be live on floor 1 — that is the change §7.1 exists for.
4. **The Sigil passive and its orbs.** One pink orb should appear with the scaffold's single Sigil,
   and deal 1 to all enemies at the start of each turn. `sigils 5` should show five orbs and five
   slots, no empties. Playing *Consummate* should remove one orb. Landing a kill with the passive
   while the target is **not** Leg-Locked should bank to Glazed; Leg-Locking it first should bank
   to Womb.
