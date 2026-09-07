# The Seductress — card list v13 draft

38 draftable cards plus 5 basics. Companion to `SEDUCTRESS_DESIGN.md` v2.4.

Changes from v12: **Leg-Locked** added as a rules word; *Leg Lock* is Breeding's new Uncommon; *Ritual Mark* and *Rapture* rebuilt; *Ward* to Common. Changes from v11: Sigil thresholds **1 / 3 / 5 / 7** on a **fixed tier shape**; Breeding rewritten to it; the starting deck gains *Breed Me!* and *Suckle*. Changes from v10, all from the slice-2 playtest: *Consummate* costs **0**; *Breed Me!*'s Sigil multiplier **replaces** its +5 rather than compounding. Earlier changes from v9: a single **universal spend order** replaces per-card spend clauses (§2);
**Freehand rebuilt** around structural roles rather than bigger numbers; *Scoop It Up* moved out
of Core and into Draining; **Deep Throat** becomes a tag with reminder text carrying auto-swallow
for the whole family; *Balls Deep* reworked into recursion. Earlier: the Mouth exception generates
**precum** (combat-only, never banks); intake rule redrawn by **card type** — Powers may generate,
Attacks and Skills may not; **holes and containers separated** (see §2); **skills are no longer
tagged**; Squeezing rebuilt around **ass attacks spending the bank for Strength**.

---

## 1. Naming rule

All card text uses vanilla Slay the Spire terms. If a mechanic exists in the base game it keeps
the base game's word; only new systems get new names. Lewd translation happens once, at the end.

| Written as | Means |
|---|---|
| **Deal X damage** | damage to an enemy |
| **Lose X HP** | unblockable self-damage |
| **Heal X** | HP restored |
| **Gain X Block** | block |

**Calibration** (v0.107 decompile): Strike 1 / 6 damage / +3 · Defend 1 / 5 Block / +3 · Anger
0 / 6 damage Common · Demon Form 3 Rare Power. Self-damage precedent: Bloodletting and Offering,
`ValueProp.Unblockable | ValueProp.Unpowered`.

---

## 2. Holes and containers

**[decided]** These are two different things and the design was conflating them.

| Hole (the tag) | Container (the reservoir) | Capacity | Behaviour |
|---|---|---|---|
| **Mouth** | **Mouth** in combat, then **Stomach** | 50 ml → unbounded | Holds cum *and* precum. Empties every combat: cum swallows into the Stomach, precum evaporates |
| **Pussy** | **Womb** | 100 ml | Leaks 10% of current per floor. 100 ml ⇒ pregnancy ⇒ Sigil |
| **Ass** | **Ass** | 200 ml | Both hole and container. Leaks 5% (25% above 200) |
| *(none)* | **Glazed** | — | External. Leaks 25% |

So the tag on a card names **the hole it uses**, and the hole determines **where the semen ends
up**. *Womb* is no longer a tag — the tag is **Pussy**, and Womb is what it fills. *Stomach* is
new, and it is a better name than "Total Swallowed" for the permanent Draining track.

**[decided] Tags go on attacks only.** A tag means *finishing with this attack fills this hole*.
Skills never finish anything, so they are never tagged, however hole-specific their text is. This
also means **only attacks set the *Continuing* mode** — lead with a tagged attack and your Pumps
follow it.

### Fill rates

Per enemy climax, routed by the finishing attack's tag, × `1 + 0.5 × Sensitive`, **rounded up**:

| Source | Fill |
|---|---|
| Normal enemy | 10 ml |
| Elite, multi-enemy | 20 ml each |
| Elite, single | 40 ml |
| Boss | 60 ml |

### Spending: one order, everywhere

**[decided]** A card that says **"Spend X ml"** draws from your containers in a fixed order:

> **Glazed → Mouth → Ass → Womb**
> *(skin, mouth, ass, pussy)*

Reminder text on every spender; no card needs to restate it. Ass may still have dedicated
spenders of its own — the Ass tag rule in §7 is a separate mechanism — but every generic spend
follows this order.

**The order is least-valuable-first, and that is what makes it interesting.** Glazed leaks 25% a
floor and does nothing on its own. Mouth cycles out every combat anyway. Ass is a real bank. Womb
is the run's progression track. So a generic spender is cheap for a Glazed-heavy deck and
genuinely dangerous for a focused one: play *Wet Work* with nothing on your skin and no mouthful
banked, and it eats your Ass — or worse, your Conception.

That self-limits without a single restriction clause. Spenders read as free utility and quietly
cost the most to exactly the decks that can least afford them.

---

## 3. Attacks and Skills do not generate intake — Powers may

**[decided]** The rule is now drawn by card type rather than by exception, because the problem was
never "cards generate cum", it was **repeatability**. An attack or skill can be replayed every
turn and every shuffle; a Power resolves once and then sits there. So:

- **Attacks and Skills** may **spend** or **read** a reservoir. They may not fill it.
- **Powers may fill.** One-shot per combat, costs a card slot and the energy up front, and cannot
  be spammed.
- **On-kill bonuses are always fine** on any card type — they are gated behind the thing that is
  supposed to gate them.

**[decided] The Mouth exception generates *precum*, not cum.** Precum sits in the Mouth and
behaves identically to cum **inside** combat — it can be spent by *Gulp*, *Spit* and anything else
that empties the Mouth, and it counts against the 50 ml cap. At end of combat it is simply gone:
it does not heal and it does not enter the **Stomach**.

That fixes the economy problem. Mouth cards can generate the fuel they need to function without
inflating Draining's permanent track, so the exception costs nothing structurally.

> **Spending order: precum first.** Anything that spends Mouth uses precum before cum, so a card
> that generates precum is never wasted and your bankable cum is preserved for the swallow.

*Scoop It Up* also survives, since it **transfers** Glazed into Mouth rather than creating
anything — and what it moves is real cum, so it banks.

This also retroactively legitimises *Edging Form*, which was flagged as a rule-break and is now
simply a Power doing what Powers may do.

| Archetype | Position on the HP bar | In-combat game |
|---|---|---|
| **Draining** (Mouth) | Stay **high** | Payoffs above half HP. Weak, Block, healing, Sensitive removal. |
| **Squeezing** (Ass) | Stay **low, don't tip over** | Payoffs below half HP. Spends the bank for Strength. |
| **Breeding** (Pussy) | **Climax repeatedly** | Payoffs for having Climaxed. Wants Sensitive for the intake multiplier. |

---

## 4. Basics — starting deck (not draftable)

4 × Pump, 4 × Guard, 1 × Let Go, 1 × Breed Me!, 1 × Suckle. **Eleven cards.**

| Card | Cost | Type | Text | Upgrade |
|---|---|---|---|---|
| **Pump** | 1 | Attack · *Continuing* | Deal **6** damage. | 9 damage |
| **Guard** | 1 | Skill | Gain **5** Block. | 8 Block |
| **Let Go** | 0 | Skill | Lose **20** HP. Draw **2**. | Draw 3 |
| **Breed Me!** | 2 | Attack · *Pussy* | Deal **12** damage. Kills from this card give **5 ml** extra. *1 Sigil:* intake from this card's kills is **doubled**. *3 Sigils:* deal **16** instead. *5 Sigils:* **tripled** instead. *7 Sigils:* repeats **2** times. | 16 damage |
| **Suckle** | 1 | Attack · *Mouth* | Deal **4** damage. Gain **5** Block. | 6 / 7 |

**[decided] Two class cards, not one.** Ironclad's single Bash is the outlier — Silent, Defect and
Watcher all start with two, and Watcher's pair is the model: each card demonstrates a different
half of the mechanic. As designed this deck had **no tagged card at all**, so nothing routed
anywhere until the first draft and the character's central mechanic was invisible for the opening
floors.

*Breed Me!* is the Pussy half. It teaches intake explicitly — the flat **5 ml** makes the reservoir
move visibly on a kill rather than hoping you notice the HUD — and its tiers begin firing on your
**first** Sigil. **Damage dropped 15 → 12** for the starter slot: 15 at 2 energy is well above
Bash's 8 and Eruption's 9, and the card already carries more text than either.

*Suckle* is the Mouth half, and the reason it is an Attack rather than a Skill is that **Skills are
untagged by decision**. That constraint produced the better card: Draining's identity is staying
high on the bar, so a Mouth attack that grants Block demonstrates the archetype's whole thesis in
one line, with no rules exception.

**Ass is the wrong starter.** Its rule spends a bank that starts empty and takes most of an act to
fill, so an Ass starter is a plain attack with a dead clause for the entire opening.

**[note] *Breed Me!* leaves the draftable pool**, since starters are not draftable. Breeding is a
card short and wants a new Common — see §12.

> **Continuing** — this card uses whichever hole your last Attack used this combat. If you have
> not used one yet, it routes to **Glazed**. Resets each combat.

Because skills are untagged, only attacks set the mode. Turn-one Pumps go to Glazed until you
lead with a tagged attack, which keeps Glazed live and makes your opening sequence a small
puzzle.

---

## 5. Core — 4 cards

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Second Wind** | 1 | Skill | Common | Heal **8**. | 12 |
| **Bat Eyelashes** | 0 | Skill | Common | Apply **1 Weak** to ALL enemies. | Also 1 Vulnerable |
| **Abandon** | 0 | Skill | Rare | **Climax immediately.** Draw **5**. | Draw 6 |
| **Afterglow** | 1 | Power | Rare | Whenever you Climax, gain **15** Block and draw **2**. | 20 Block |

---

## 5a. Freehand (Glazed) — 5 cards

Hands, breasts, thighs, skin. No hole used, so nothing is collected. Rather than paying for that
with raw damage, these now do things the tagged groups cannot.

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Handful** | 0 | Attack · *Glazed* | Common | **Innate.** Deal **11** damage. **Exhaust.** | 14 damage |
| **Smothered** | 2 | Attack · *Glazed* | Common | Deal **5** damage. Gain **10** Block. Gain **5** Block next turn. | 7 / 13 / 7 |
| **Thighplay** | 1 | Attack · *Glazed* | Uncommon | Deal **9** damage. Your next **Ass** or **Pussy** card costs **1 less**. | 12 damage |
| **Show Off** | 0 | Attack · *Glazed* | Uncommon | Gain **1 Vulnerable**. Deal **10** damage to ALL enemies and apply **1 Vulnerable**. | 14 damage |
| **Wet Work** | 0 | Skill | Uncommon | **Spend 10 ml.** Gain **10** Block. | Spend 8 ml |

**The group has an identity now.** Previously it was "the same cards but bigger", which is a
weak reason to draft anything. Each of these does something structural instead:

- *Handful* is a free opener, and being **Innate** means it is always in your first hand — which
  drags your *Continuing* mode to Glazed on turn one. Playing it is tempo; holding it is routing.
  That tension only exists because it is Innate, and it is the best thing about the card.
- *Smothered* makes Glazed the defensive colour, which nothing else covers.
- *Thighplay* is the bridge: a Glazed attack that discounts your next Ass or Pussy card, so a
  focused deck has a real reason to run one.
- *Show Off* buys AoE with self-Vulnerable, which fits the character's whole "escalation is a
  cost" premise rather than being generically strong.
- *Wet Work* is deliberately cheap and deliberately dangerous — see the spend order in §2.

---

## 6. Draining (Mouth) — 10 cards

Keep HP high, bank mouthfuls, swallow at the bell into the **Stomach**.

### The Deep Throat family

**[decided]** *Deep Throat* is a **card family**, not one card — the same pattern as vanilla's
`CardTag.Strike`, which the decompile confirms exists as a real tag system. That fixes the
problem with *No Gag Reflex*: a Power naming a single common is dead until you draft that exact
card, but a Power naming a family of three has something to work with from the first draft.

> **Deep Throat** — Climaxes caused by this card are swallowed immediately.

**[decided]** Auto-swallow is carried by the **tag**, not written on each card. That is what makes
the tag worth having: it is a rules word with its own reminder text, so a fourth Deep Throat card
costs nothing to add and *No Gag Reflex* has a real body of cards to modify. Mechanically it also
means the family is the only way to bank mid-combat — everything else waits for *Gulp* or the
bell — which is the actual reason to draft into it rather than just playing big Mouth attacks.

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Throat Fuck** | 1 | Attack · *Mouth* · **Deep Throat** | Common | Deal **9** damage. Lose **3** HP. | 12 damage |
| **Deep Throat** | 2 | Attack · *Mouth* · **Deep Throat** | Common | Deal **16** damage. Lose **4** HP. Draw **1**. | 20 damage |
| **Balls Deep** | 1 | Attack · *Mouth* · **Deep Throat** | Uncommon | Deal **9** damage. Lose **3** HP. Put a card from your discard pile on top of your draw pile. If it is a different **Mouth** card, put it into your hand instead. | Deal 12 |
| **No Gag Reflex** | 1 | Power | Uncommon | **Deep Throat** cards cost **1 less** and no longer lose HP. | Also draw 1 on each |

*Balls Deep* is now recursion rather than a damage spike, and the Mouth clause is a routing
reward rather than a threshold check: it always returns something, and it returns it *faster* if
you are actually playing Mouth cards. The **3** HP keeps the family's whole cost pattern intact,
so *No Gag Reflex* removes something real on all three members.

**"Different"** does the real work. Retrieving *Balls Deep* itself puts it on the draw pile, not
in hand, so the loop always costs a draw — which is what stops it becoming a soft infinite once
*No Gag Reflex* has zeroed the cost and the HP. Self-recursion stays available, it is just never
the fastest line.

### The rest

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Lip Service** | 1 | Attack · *Mouth* | Common | Deal **7** damage. | 10 |
| **Lick Pre-cum** | 1 | Skill | Common | Gain **5 ml precum**. Apply **1 Weak**. | 8 ml |
| **Scoop It Up** | 1 | Skill | Uncommon | Empty your Glazed. Gain that much **Mouth**. | Costs 0 |
| **Gulp** | 0 | Skill | Uncommon | Swallow now: Heal **half** your Mouth in ml and bank the cum into your Stomach, then empty it. If you swallowed more than **25 ml**, remove **1 Sensitive**. | Full ml |
| **Spit** | 1 | Attack | Uncommon | Empty your Mouth. Deal damage equal to **half the ml** to ALL enemies, and apply **1 Weak and 1 Vulnerable per 10 ml** emptied. | Full ml |
| **Restraint** | 2 | Power | Uncommon | While your HP is above half, gain **4** Block at the start of your turn. | 6 Block |
| **Milked Dry** | 3 | Attack · *Mouth* | Rare | Deal **10** damage to ALL enemies. Gain **15 ml precum**. | 14 damage |

*Spit* is untagged despite being an attack — you are expelling, not receiving, so it fills
nothing and never sets the Continuing mode.

*Scoop It Up* sits here rather than in Core because, although it carries no **Mouth** tag, it only
does anything for a Mouth deck. It is the seam between Freehand and Draining: Glazed collects
nothing on its own, and this is the one card that makes it collect after the fact. What it moves
is real cum, so it banks — unlike *Lick Pre-cum* and *Milked Dry*, which do not.

---

## 7. Squeezing (Ass) — 8 cards

**[decided] The Ass tag has its own rule**, and it is the archetype:

> **Ass-tagged attacks:** lose **10% of your current Ass** (rounded up). This attack gains
> **+1 Strength per 10 ml lost**.

A hole in active use leaks, and the leaking is what powers the hit. Because the Strength applies
per hit, this pushes the whole archetype toward **multi-hit attacks** — which is where its
identity now lives, rather than in reading a number.

Worked: at 200 ml a card loses 20 ml and gains **+2** for that attack; at an inflated 400 ml it
loses 40 and gains **+4**. On a three-hit card that is +6 or +12 damage. The bank is ammunition
that depletes across a fight and refills from kills routed back into it.

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Clench** | 1 | Attack · *Ass* | Common | Deal **5** damage twice. Lose **4** HP. | 7 twice |
| **Stuffed** | 2 | Attack · *Ass* | Common | Deal **12** damage. Lose **8** HP. If your HP is below half, hit **one additional time**. | 15 damage |
| **Grind** | 1 | Attack · *Ass* | Uncommon | Deal **6** damage. Gain Block equal to the damage dealt. Lose **2** HP. | 8 damage |
| **Rut** | 2 | Attack · *Ass* | Uncommon | Deal **6** damage three times. Lose **6** HP. | 8 three times |
| **Stretch My Hole Daddy** | 2 | Power | Uncommon | Attacks that cost you HP hit **one additional time**. | Costs 1 |
| **Milking Grip** | 2 | Power | Uncommon | Ass-tagged attacks lose **5%** instead of 10%, but gain Strength **as though they had lost 10%**. Ass-tagged kills gain **double** intake. | Lose 3% |
| **Overflow** | 1 | Attack · *Ass* | Uncommon | Deal **1** damage to ALL enemies and apply **Vulnerable**. This card loses **double** the usual Ass. | Also 1 Weak |
| **Blowout** | 3 | Attack · *Ass* | Rare | This card loses **50%** of your Ass instead of 10%. Deal **6** damage four times. | 8 four times |

**[decided] The old *Grind* was strictly better than *Clench*** and is gone. Three hits of 3 for
no HP beat two hits of 5 for 4 HP the moment the Ass rule applies, because Strength lands per
hit: at 200 ml that was 5×3 = 15 free against 7×2 = 14 for 4 HP. A common that dominates another
common at the same cost is a dead draft slot, not a choice.

*Stuffed* takes the vacated Common slot. The new *Grind* is the archetype's only defensive card,
and deliberately **single-hit** — which inverts everything else here. The Ass rule still applies,
so at 200 ml it is 8 damage and **8** Block for 1 energy and 2 HP, and the Block scales with the
bank exactly like the damage does. *Clench* is not dominated in return: multi-hit still wins on
raw output the fuller you are, so the two commons split cleanly into "spend the bank for damage"
and "spend the bank for a turn of safety".

*Grind* drains the bank to produce Block, which is ammunition spent on defence — but Squeezing has
to survive at low HP, so it needs a defensive card and this is the only one that scales with the
thing the archetype is already hoarding. Paying ammunition for it is the correct cost.

*Blowout* at 200 ml loses 100 ml for +10 Strength, so four hits of 16 — 64 damage, at the cost of
half the bank. That is the archetype's ceiling and it should hurt to use.

*Milking Grip* is the archetype's engine rather than a compromise: half the drain, **full**
Strength, and double intake on Ass kills. It solves Squeezing's structural problem — a bank that
depletes faster than kills refill it — and it is the card a Squeezing deck is looking for.

*Stretch My Hole Daddy* multiplies everything, because Strength applies per hit. *Clench* becomes
5×3, *Rut* becomes 6×4, *Stuffed* becomes 12×2 (×3 below half), and *Grind* becomes 6×2 — which,
if Block is per instance of damage, doubles the Block too. **[decided]** *Grind* grants Block
**per hit**, so under the Power it is 6×2 damage and 6×2 Block. That is intentional: the Power is
a 2-cost Uncommon that only pays off in a deck built around HP-cost attacks, and this is one of
the payoffs it exists for.

*Overflow* is the cheap bank-dump: base damage of 1 is almost irrelevant, since the Ass rule at
double drain does the work. At 200 ml it loses 40 ml for +4 Strength, so 5 damage and Vulnerable to
everything for 1 energy; at an inflated 400 ml it is 9 to everything. The drain is the real cost,
not the energy — which is why it is Uncommon at 1 rather than Rare at 2.

---

## 8. Breeding (Pussy) — 8 cards

Finishers, Conception, and the Sigil payoff. **Sigil gating at 1 / 3 / 5 / 7**, on the fixed tier
shape from design §7.1:

> **1** adds an effect · **3** grows the base · **5** grows the effect · **7** repeats, a number of
> times equal to its cost. **0-cost cards lose their non-energy cost at 7** instead of repeating.

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Take Him In** | 1 | Attack · *Pussy* | Common | Deal **8** damage. *1:* draw **1**. *3:* deal **12** instead. *5:* draw **2** instead. *7:* repeats **1** time. | 11 damage |
| **Surrender** | 0 | Skill | Common | Lose **10** HP. Draw **1**, plus **1** per Climax this combat. *1:* gain **5** Block. *3:* draw **2** plus per Climax. *5:* gain **10** Block instead. *7:* lose no HP. | Lose 8 HP |
| **Ward** | 1 | Skill | Common | Gain **6** Block. *1:* reflect **5** damage to attackers this turn. *3:* gain **12** Block instead. *5:* reflect **10** instead. *7:* repeats **1** time. | 9 base |
| **Claim** | 1 | Attack · *Pussy* | Uncommon | Deal **7** damage. Lose **2** HP. *1:* apply **1 Vulnerable**. *3:* deal **12** instead. *5:* apply **2 Vulnerable** instead. *7:* repeats **1** time. | 10 / 16 |
| **Leg Lock** | 1 | Skill | Uncommon | Gain **6** Block. Apply **1 Vulnerable** and **1 Leg-Locked**. *1:* also apply **1 Debilitate**. *3:* apply **2 Leg-Locked** and **2 Vulnerable**. *5:* apply **2 Debilitate** instead. *7:* apply all of it to ALL enemies. | 9 Block |
| **Ritual Mark** | 1 | Power | Uncommon | Your **Pussy** cards deal **2** additional damage. *1:* your Sigils deal **1** additional damage. *3:* Pussy cards deal **4** instead. *5:* Sigils deal **4** too. *7:* both bonuses become **+1 per Sigil**. | +3 base |
| **Consummate** | 0 | Skill | Rare | **Exhaust a Sigil** for this combat: gain **2** energy and draw **3**. *1:* gain **10** Block. *3:* gain **3** energy instead. *5:* gain **20** Block instead. *7:* does not exhaust a Sigil. | Draw 4 |
| **Rapture** | 3 | Power | Rare | Gain **10** Block. Whenever you Climax, gain **3** energy and **2 Strength**. *1:* also gain **10** Block whenever you Climax. *3:* this costs **1 less**. *5:* both Block values become **20**. *7:* **double** the energy gained. | 3 Strength |

> **Leg-Locked** — while an enemy is Leg-Locked, its death routes its output to your **Pussy**,
> whatever killed it. Counts down at the end of its turn.

**Leg-Locked is a kill claim** — a claim staked on a kill that has not happened yet, which is why
it counts down. It is a rules word rather than just this card's text, and it solves a problem the
design has carried since §6.0. Routing keys off the *finishing card's* tag, so a kill delivered by Poison, a
Sigil passive, reflect damage, or any untagged card falls through to Glazed (§6.4) — and Breeding,
which needs Womb fill more than any other archetype, is exactly the deck most likely to kill things
with something other than an attack. Leg-Locked lets you **commit the route in advance**, which is
a decision rather than a correction.

It **overrides both** the finishing-card tag and the Glazed catch-all. Because it counts down like
Debilitate, marking an enemy you then fail to kill is wasted — so the card asks you to read the
fight, and stacking it is buying time rather than buying power.

**Debilitate is vanilla and generic**, not Necrobinder-locked — a `PowerModel` with
`StackType.Counter` that Vulnerable and Weak both query. **Its stacks are duration, not intensity**:
the multiplier is `amount + (amount - 1)` applied once however many stacks are on, decrementing at
the end of the owner's turn. So *Leg Lock*'s tier 3 buys two enemy turns of doubled Vulnerable, not
double the doubling. It also only fires on powered attacks, so it never touches your own
self-damage.

**Every gated card now reads the same way****Every gated card now reads the same way**, so a Breeding deck holds one ladder rather than a
dozen unrelated clauses. The scale lives in the shape; only the specifics differ.

**Not every card is gated.** *Ritual Mark* and *Rapture* carry no tiers — *Ritual Mark* already
scales with Sigils through its effect rather than through a threshold, and *Rapture* keys off
Climax, which is the other axis. Forcing tiers onto them would add text without adding a decision.

*Claim* loses the "at 4 Sigils, stop costing HP" clause it carried in v10. Removing a downside does
not fit any rung of the ladder, and the shape is worth more than that one clause. The 2 HP now
persists at every tier.

**Powers repeat too.** `DuplicationPower` is vanilla and generic, so a repeated Power applies its
buff twice — the only real exception is a non-stacking Barricade-style effect, and this pool has
none. *Ritual Mark*'s `+1 per Sigil` and *Rapture*'s doubled energy are **choices**, taken because
each is more interesting than doubling, not because doubling was unavailable. The rule stands as:
**everything repeats at tier 4; 0-cost cards shed their non-energy cost instead.**

*Ritual Mark* at 7 Sigils is **+7**, against the **+8** that plain doubling would give — so the
scaling tier is a marginal downgrade at exactly the threshold that unlocks it, and only pulls ahead
from 8 Sigils. Kept deliberately: converting a static buff into a scaling one is the fantasy, and
7 Sigils is already past the run target, so the tier is aspirational either way.

*Rapture*'s base Block moved off the Climax and onto **casting**, so the card does something the
turn you play it rather than sitting inert until you happen to Climax — which matters on a 3-cost
Rare. Tier 1 then puts Block back on the Climax as well, which is the "adds an effect" rung doing
real work rather than bolting on a rider.

*Consummate*'s tier 1 lands at exactly the moment the card becomes playable at all — you need a
Sigil to exhaust — so its first Sigil both unlocks and improves it.

*Rapture* puts the energy **on the Climax**, not on play, so a Climax you cause on your own turn
refunds 3 energy and one the enemy causes gives you only the Strength and Block. That asymmetry is
the point: Climaxing is otherwise something that happens *to* you, and this is the card that makes
choosing the moment worth real tempo. It pairs with *Abandon* — 0 cost, Climax immediately, Draw 5
— which is not free: the Climax is a third of the death clock and Lust is expensive to remove, so
the pair is bought with permanent run resource. That is the right price, and it sits on the meta
layer where the character's tension already lives.

*Breed Me!* has moved to the **starting deck** (§4); *Ward* drops to Common to fill the gap, and
*Leg Lock* is the new Uncommon. Rarity spread is 3 Common / 3 Uncommon / 2 Rare.

---

## 9. Ancient — 1 card

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Edging Form** | 1 | Power | Ancient | Whenever you play an Attack, gain **1 ml** in the hole that Attack used. | 2 ml |

No longer a rule-break at all — it is a Power, and Powers may fill (§3).

**Correction: it is best in Squeezing, not worst.** I had this backwards. Draining and Breeding do
not bleed their reservoir by attacking, so a per-attack trickle is a nice-to-have. Ass bleeds 10%
on every tagged attack, so the trickle is a *floor*, and it matters most exactly when the bank is
low and the archetype is struggling: at 200 ml an attack loses 20 and regains 1, barely noticeable,
but at 20 ml it loses 2 and regains 1, halving the drain. It props the archetype up at its weakest
point, which is the most valuable thing an effect can do.

---

## 10. Lust — 2 cards (not draftable)

| Card | Cost | Type | Text |
|---|---|---|---|
| **Aching** | 0 | Curse | Playable: gain **2 energy** this turn. Lose **12** HP. |
| **Craving** | 0 | Curse | Playable: draw **3**. Lose **12** HP. |

---

## 11. Distribution

| Group | Count |
|---|---|
| Basics (not draftable) | 5 |
| Core | 4 |
| Freehand (Glazed) | 5 |
| Draining (Mouth) | 11 |
| Squeezing (Ass) | 8 |
| Breeding (Pussy) | 8 |
| Ancient | 1 |
| Lust (not draftable) | 2 |
| **Total** | **38 draftable + 2** |

32 is a floor, not a ceiling.

---

## 12. Open

- *Spit* at a full 50 ml is 25 AoE plus 5 Weak and 5 Vulnerable for 1 energy. High ceiling.
- Ass attacks drain even when they miss the kill, so a Squeezing deck bleeds its bank in fights it
  does not route back. *Milking Grip* and *Edging Form* are both floor-raisers against that; watch
  whether the archetype is unplayable without either.
- *Stretch My Hole Daddy* doubling *Stuffed* (one big HP-cost hit) is a much larger jump than
  adding a fourth small hit. Rare pricing assumes that; check it does not just become "the Stuffed
  card".
- The intake rule is now by card type. Worth auditing every Power in the pool against it — Powers
  filling reservoirs is now legal, so *Throat Training* and *Full Term*, cut earlier for breaking
  the old rule, could legitimately come back.
- ~~An execute is a gap in Breeding.~~ **Closed — there was no gap.** Executes are close to absent
  from StS by design: in a game where you kill everything anyway, an HP-threshold finisher mostly
  saves a turn on something already dying. The closest vanilla analogue is Feed, a kill *reward*
  rather than a threshold — and this character already pays for kills, because kills route intake.
  *Breed Me!*'s +5 ml occupies that slot. An execute would be a second payoff on the same event.
- **Leg-Locked may be strong enough to want a cost.** It converts any kill into Womb fill, which is
  the character's whole progression track, for 1 energy on a card that also blocks. Watch whether
  a Breeding deck simply opens every fight with it.
- **Tier 4 (7 Sigils) is unbalanced on purpose and should not be tuned yet.** The 4–6 Sigils per
  run target is unverified since the elite intake change, so nobody knows whether 7 is a stretch
  goal or unreachable. 1 / 3 / 5 is safe either way.
- *Handful* being Innate means every Glazed draft changes your turn-one routing whether you want
  it to or not. That is the interesting part, but it may be too loud in a focused Ass or Pussy
  deck that drafted the card purely as a cheap opener.
- The Deep Throat family is three cards. If *No Gag Reflex* feels weak, the fix is a fourth family
  member rather than buffing the Power.
- *Balls Deep* returning itself is gated to the draw pile by the "different" clause, so the loop
  costs a draw per replay. Confirm that holds against draw engines like *Afterglow*.
