# The Seductress — card list v10 draft

38 draftable cards plus 3 basics. Companion to `SEDUCTRESS_DESIGN.md` v1.9.

Changes from v9: a single **universal spend order** replaces per-card spend clauses (§2);
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

5 × Pump, 4 × Guard, 1 × Let Go.

| Card | Cost | Type | Text | Upgrade |
|---|---|---|---|---|
| **Pump** | 1 | Attack · *Continuing* | Deal **6** damage. | 9 damage |
| **Guard** | 1 | Skill | Gain **5** Block. | 8 Block |
| **Let Go** | 0 | Skill | Lose **20** HP. Draw **2**. | Draw 3 |

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

Finishers, Conception, and the Sigil payoff. **Sigil gating at 2 / 4 / 6** — three tiers rather
than two, which fits a 4–6 Sigil run: tier one lands early, tier two mid-run, tier three only at
the top end.

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Take Him In** | 1 | Attack · *Pussy* | Common | Deal **8** damage. *2 Sigils:* draw **1**. *4 Sigils:* draw **2** instead. *6 Sigils:* this costs **0**. | 11 damage |
| **Breed Me!** | 2 | Attack · *Pussy* | Common | Deal **15** damage. Kills from this card give **5 ml** extra. *2 Sigils:* intake from this card's kills is **doubled**. *4 Sigils:* **tripled**. *6 Sigils:* and this hits **two more times**. | 19 damage |
| **Surrender** | 0 | Skill | Common | Lose **10** HP. Draw **1**, plus **1** for each time you have Climaxed this combat. | Lose 8 HP |
| **Claim** | 1 | Attack · *Pussy* | Uncommon | Deal **7** damage. Apply **1 Vulnerable**. Lose **2** HP. *2 Sigils:* deal **12** instead. *4 Sigils:* lose no HP and apply **1** more Vulnerable. *6 Sigils:* hits twice. | 10 / 16 |
| **Ward** | 1 | Skill | Uncommon | Gain **6** Block. *2 Sigils:* gain **6** more. *4 Sigils:* reflect **5** damage to attackers this turn. *6 Sigils:* gain **10** more. | 9 base |
| **Ritual Mark** | 2 | Power | Uncommon | Your Sigils **and your Pussy cards** deal **1** additional damage. | 2 |
| **Consummate** | 2 | Skill | Rare | **Exhaust a Sigil** for this combat: gain **2** energy and draw **3**. | Draw 4 |
| **Rapture** | 3 | Power | Rare | Whenever you Climax, gain **3** energy, **2 Strength** and **20** Block. | 3 Strength |

*Breed Me!*'s **+5 ml** is a kill reward rather than generation, so it does not break the Attacks-
don't-generate rule. The Sigil clauses double or triple the card's **total** kill intake including
that 5 — which is what makes the tiers read as a real escalation instead of a rounding difference.

*Surrender* at 0 cost is a Climax-count payoff rather than a binary one. Before your first Climax
it is a strictly worse *Let Go*; after three it is Draw 4 for free and 10 HP. That is the right
shape for Breeding, where Climaxing repeatedly *is* the plan — it rewards the thing the archetype
already wants to do rather than asking for a second condition.

*Claim* now costs HP at base and stops costing it at **4 Sigils**. The tier is doing two jobs at
once — a damage floor early, then a maintenance discount later — which is more interesting than
another number going up, and it means the card gets *cheaper to play* as the run goes on rather
than only bigger.

*Rapture* puts the energy **on the Climax**, not on play — so a Climax you cause on your own turn
refunds 3 energy and a Climax the enemy causes on theirs gives you nothing but the Strength and
Block. **[decided]** That asymmetry is the point: Climaxing is otherwise something that happens
*to* you, and this is the card that makes choosing the moment worth real tempo. It also gives
*Abandon* — 0 cost, Climax immediately, Draw 5 — a genuine combo partner. It is **not** free: the
Climax is a fifth of the death clock and Lust is expensive to remove, so *Abandon* + *Rapture* is
3 energy, 5 cards, 2 Strength and 20 Block bought with permanent run resource. That is the right
price, and it is why the pair does not need watching — the cost is on the meta layer where the
character's whole tension already lives.

Strength persists as vanilla Strength does; the **20** Block is ordinary Block and decays at end
of turn. A permanent 20 per Climax would make Breeding unkillable in exactly the fights where it
Climaxes most.

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
- Nothing punishes an enemy for being at low HP. An execute is still a gap in Breeding.
- *Handful* being Innate means every Glazed draft changes your turn-one routing whether you want
  it to or not. That is the interesting part, but it may be too loud in a focused Ass or Pussy
  deck that drafted the card purely as a cheap opener.
- The Deep Throat family is three cards. If *No Gag Reflex* feels weak, the fix is a fourth family
  member rather than buffing the Power.
- *Balls Deep* returning itself is gated to the draw pile by the "different" clause, so the loop
  costs a draw per replay. Confirm that holds against draw engines like *Afterglow*.
