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
