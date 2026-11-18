# The Seductress — card list v25

38 draftable cards plus 5 basics. Companion to `SEDUCTRESS_DESIGN.md` v3.11.

**[Claude, 2026-11-14] v25 brings this document back in line with the build**, at [Gorim]'s request: where
the two disagreed the build was taken as correct. Changes from v24: ***Gulp* is cut** (it became the 100 ml
Stomach milestone, design §6.5); ***Milking Grip* leaves the draft pool** and is granted at 100 ml Ass;
**Ass Strength is +1 per 4 ml lost**, not per 10; **swallowing heals 1 HP per 2 ml**, precum included;
*Suckle*'s upgrade is **cost 0**; *Ritual Mark* covers **Pussy and Breasts** and upgrades to **Innate**;
*Milk* is **heal 4 / deal 2**, tagged **Skin · Breasts**; *Lick Pre-cum* hits **ALL** enemies. Card text
the build got wrong was fixed the same day; §13 records what changed.

Changes from v22: **Max HP is 20** (design §6.5) and every HP cost is rescaled. Changes from v20: ***Scoop It Up* cut** — its job is now the **Scoop** rest-site option (design §6.3c); all leak routes to Skin; birth wastes the whole Womb. Changes from v17: *Glazed* renamed **Skin**; **Milk** added as a generated card; pregnancy is a carried state (design §6.3b). Changes from v13: *Let Go* renamed **Seek Relief** with a new upgrade; *Abandon* replaced by **Forced Orgasm**; *Suckle* now generates **precum**; *Afterglow* gains an engine. Changes from v12: **Leg-Locked** added as a rules word; *Leg Lock* is Breeding's new Uncommon; *Ritual Mark* and *Rapture* rebuilt; *Ward* to Common. Changes from v11: Sigil thresholds **1 / 3 / 5 / 7** on a **fixed tier shape**; Breeding rewritten to it; the starting deck gains *Breed Me!* and *Suckle*. Changes from v10, all from the slice-2 playtest: *Consummate* costs **0**; *Breed Me!*'s Sigil multiplier **replaces** its +5 rather than compounding. Earlier changes from v9: a single **universal spend order** replaces per-card spend clauses (§2);
**Freehand rebuilt** around structural roles rather than bigger numbers; *Scoop It Up* moved out
of Core and into Draining; **Deep Throat** becomes a tag with reminder text carrying auto-swallow
for the whole family; *Balls Deep* reworked into recursion. Earlier: the Mouth exception generates
**precum** (combat-only, never banks); intake rule redrawn by **card type** — Powers may generate,
Attacks and Skills may not; **holes and containers separated** (see §2); **skills are no longer
tagged**; Squeezing rebuilt around **ass attacks spending the bank for Strength**.

---

## 1. Naming rule

**If it exists in vanilla, it keeps the vanilla word.** Damage is damage, Block is Block, HP is
HP. This document and the code are written entirely in those terms.

**[decided] The lewd vocabulary lives in a display-time converter**, specified in design §9a —
Deal→Tease, damage to→pleasure from, Block→Poise, Heal→Soothe, HP→Arousal (inverted). Nothing here needs
rewriting when it lands, and nothing here should anticipate it.

Keeping both registers in the card text is what produced cards reading "Deal 6 damage" beside
"gain 20 arousal", where no one could tell which words were rules terms. One register per layer
fixes that permanently: vanilla words in the design, lewd words on the screen.

## 2. Holes and containers

**[decided]** These are two different things and the design was conflating them.

| Hole (the tag) | Container (the reservoir) | Capacity | Behaviour |
|---|---|---|---|
| **Mouth** | **Mouth** in combat, then **Stomach** | 50 ml → unbounded | Holds cum *and* precum. Empties every combat: cum swallows into the Stomach, precum evaporates |
| **Pussy** | **Womb** | 100 ml | Leaks 10% of current per floor. 100 ml ⇒ pregnancy ⇒ Sigil |
| **Ass** | **Ass** | 200 ml | Both hole and container. Leaks 5% (25% above 200) |
| *(none)* | **Skin** | — | External. Leaks 25% |

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

> **Skin → Mouth → Ass → Womb**
> *(skin, mouth, ass, pussy)*

Reminder text on every spender; no card needs to restate it. Ass may still have dedicated
spenders of its own — the Ass tag rule in §7 is a separate mechanism — but every generic spend
follows this order.

**The order is least-valuable-first, and that is what makes it interesting.** Skin leaks 25% a
floor and does nothing on its own. Mouth cycles out every combat anyway. Ass is a real bank. Womb
is the run's progression track. So a generic spender is cheap for a Skin-heavy deck and
genuinely dangerous for a focused one: play *Wet Work* with nothing on your skin and no mouthful
banked, and it eats your Ass — or worse, your Conception.

That self-limits without a single restriction clause. Spenders read as free utility and quietly
cost the most to exactly the decks that can least afford them.

### Eat

**[Gorim, 2026-11-17]** A new rules word, built the same day.

> **Eat N** — take **N ml** in the Spend order (Skin → Mouth → Ass → Womb) and swallow it: it heals you and goes to
> the **Stomach**.

[Claude], accepted by [Gorim]: it heals at the ordinary swallow rate, **1 HP per 2 ml**; eaten precum heals but banks
nothing (until the 300 ml milestone); with less than N held it eats what there is. Unlike Spend, the ml is not lost —
it becomes Stomach progress — so Eat is a way to move Skin and Ass into Draining's track.

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
behaves identically to cum **inside** combat — it can be spent by *Spit* and anything else
that empties the Mouth, and it counts against the 50 ml cap. At end of combat it does not enter the
**Stomach** — but it is swallowed with everything else, and **[Gorim, 2026-09-08] every swallow heals
1 HP per 2 ml, cum and precum together**. So precum heals on its way out and banks nothing (until the
300 ml Stomach milestone, where it banks as cum).

That fixes the economy problem. Mouth cards can generate the fuel they need to function without
inflating Draining's permanent track, so the exception costs nothing structurally.

> **Spending order: precum first.** Anything that spends Mouth uses precum before cum, so a card
> that generates precum is never wasted and your bankable cum is preserved for the swallow.

This also retroactively legitimises *Edging Form*, which was flagged as a rule-break and is now
simply a Power doing what Powers may do.

| Archetype | Position on the HP bar | In-combat game |
|---|---|---|
| **Draining** (Mouth) | Stay **high** | Payoffs above half HP. Weak, Block, healing, Sensitive removal. |
| **Squeezing** (Ass) | Stay **low, don't tip over** | Payoffs below half HP. Spends the bank for Strength. |
| **Breeding** (Pussy) | **Climax repeatedly** | Payoffs for having Climaxed. Wants Sensitive for the intake multiplier. |

---

## 4. Basics — starting deck (not draftable)

4 × More, 4 × Guard, 1 × Seek Relief, 1 × Breed Me!, 1 × Suckle. **Eleven cards.**

**[Gorim, 2026-11-17]** *Pump* is renamed ***More***. *Continuing* attacks are named as things she says,
so the name and art fit whichever hole the card routes to.

| Card | Cost | Type | Text | Upgrade |
|---|---|---|---|---|
| **More** | 1 | Attack · *Continuing* | Deal **6** damage. | 9 damage |
| **Guard** | 1 | Skill | Gain **5** Block. | 8 Block |
| **Seek Relief** | 0 | Skill | Lose **9** HP. Draw **2**. Gain **1** energy. | Lose 4 HP |
| **Breed Me!** | 2 | Attack · *Pussy* | Deal **12** damage. Kills from this card give **5 ml** extra. *1 Sigil:* intake from this card's kills is **doubled**. *3 Sigils:* deal **16** instead. *5 Sigils:* **tripled** instead. *7 Sigils:* repeats **2** times. | 16 damage |
| **Suckle** | 1 | Attack · *Mouth* | Deal **4** damage. Gain **5 ml precum**. If this kills, gain **5 ml** instead. | Costs 0 |

> **Max HP is 20** (design §6.5). Every HP cost below is rescaled against it — as though the bar
> were 30–40 rather than by a clean ÷3.25, because this bar is meant to be swingy. Block and Max HP
> are each worth roughly **4×** what they are in vanilla, since the bar is one life of four.

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
untagged by decision**.

**[revised 2026-09-07] It grants precum, not Block.** As a Block card it was plain and taught
nothing — Guard already covers defence, so the defensive slot was never the one going spare. What
was missing was any starting card that showed the Mouth economy exists. Now it teaches the single
most confusable rule in the character in one line: **precum normally, real cum on a kill.** The
temporary resource and the banking resource, and the difference between them, demonstrated by the
same card on two different outcomes.

**[Gorim, 2026-09-09] The upgrade drops the cost to 0** — "it is not worth it as a damage card". This is
the third setting: 10 ml was cut once swallowing healed (10 ml became 5 HP a play on a 20 HP bar), +3
damage replaced it, and cost 0 replaced that. Base damage stays at 4. At 0 cost it is a free 5 ml of
precum every turn it is drawn, limited by the 50 ml Mouth cap rather than by energy.

That makes both class starters intake teachers from opposite ends — *Breed Me!* shows a kill
filling the Womb, *Suckle* shows the Mouth filling whether you kill or not, and only banking if you
do. **[dependency] This makes precum load-bearing for the starting deck**, so it must be built
before the deck is correct; see design §6.2a.

*Seek Relief* (formerly *Let Go*) keeps the deck's self-damage axis, and its upgrade now **lowers
the cost** — 9 HP to 4 — rather than drawing more. On a card whose whole function is paying HP
for cards, cutting the price is the upgrade that changes how you play it; drawing a third card just
made it a bigger version of the same gamble.

**Ass is the wrong starter.** Its rule spends a bank that starts empty and takes most of an act to
fill, so an Ass starter is a plain attack with a dead clause for the entire opening.

**[note] *Breed Me!* leaves the draftable pool**, since starters are not draftable. Breeding is a
card short and wants a new Common — see §12.

> **Continuing** — this card uses whichever hole your last Attack used this combat. If you have
> not used one yet, it routes to **Skin**. Resets each combat.

Because skills are untagged, only attacks set the mode. Turn-one Pumps go to Skin until you
lead with a tagged attack, which keeps Skin live and makes your opening sequence a small
puzzle.

---

## 5. Core — 7 cards

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Rehydrate** | 1 | Skill | Common | **Eat 5.** Your next Attack gains **5** ml precum. | 8 and 8 |
| **Catch Your Breath** | 1 | Skill | Common | Gain **6** Block. **Eat 3.** | 9 Block |
| **Keep Going** | 1 | Attack · *Continuing* | Common | Deal **8** damage. If your last Attack used a hole, draw **1**. | Draw 2 |
| **Faster** | 1 | Attack · *Continuing* | Uncommon | Deal **4** damage twice. | 5 twice |
| **Bat Eyelashes** | 0 | Skill | Common | Apply **1 Weak** to ALL enemies. | Also 1 Vulnerable |
| **Forced Orgasm** | 0 | Skill | Uncommon | **Climax** with no heal and no Lust card. Draw **2**. Gain **1** energy. **Exhaust.** | Draw 3 |
| **Afterglow** | 1 | Power | Rare | Whenever you Climax, gain **15** Block and draw **2**. Each turn, draw **1** and gain **1** energy per stack of **Sensitive**. | Costs 0 |

---

**[revised 2026-09-07] *Afterglow* gains an engine.** The burst is unchanged; on top of it, each
turn you draw 1 and gain 1 energy **per stack of Sensitive**, and the upgrade cuts the cost to 0
rather than raising the Block.

It does not widen the trigger — Sensitive is only gained by Climaxing and clears at end of combat,
so the engine is live under exactly the condition the burst always had. What changed is **when the
value lands**: across the turns you spend rebuilding, instead of dumped on the turn you reset to
zero arousal and need Block least. The rate is per stack and deliberately steep, because at 3
Sensitive you are taking +150% arousal and the payoff should match the position rather than hedge
it.

**[Gorim, 2026-11-17] *Keep Going* and *Faster* added** from the draft board ([Claude] drafts; *Faster* was drafted as
*Rhythm*). A "hole" for *Keep Going* is Mouth, Ass or Pussy, not Skin. **[Gorim] A *Continuing* attack routed to Ass
pays the Ass rule's cum cost and takes its bonus, but has no HP cost** — this applies to *More* as well.
[Claude] Not yet play-tested; neither card has a portrait.

**[Gorim, 2026-11-17] *Rehydrate* replaces *Second Wind*** (boring, and it shared a name with an Ironclad card), and
***Catch Your Breath*** is built as Block 6, Eat 3 rather than the drafted Block-and-draw. [Claude] *Rehydrate* is read as:
the next Attack you play adds 5 ml precum to the Mouth. Its upgrade numbers are [Claude]'s. Not yet play-tested.

***Forced Orgasm* replaces *Abandon*.** Abandon paid a **Lust card** for its Climax, which priced
it out of exactly the decks that wanted it: the reason to want a Climax on demand is to switch on
the cards that key off one, and paying the run's death clock for that is not a trade a cautious
deck can make. Forced Orgasm is Adrenaline at one rarity lower, paid for in Sensitive instead.

**Nothing visible is spent, and the cost is still real.** Arousal does not reset, and the new
Sensitive stack raises everything you take afterwards by 50% — so it trades a percentage of the bar
you have left for tempo, and gets worse the more hurt you already are. A card you play early in a
fight or not at all.

**The Rapture line is the point, not a risk.** With *Rapture* out, Forced Orgasm is 0 cost for
**4 energy**, 2 Strength, 20 Block and 2 cards. **[decided]** That is accepted: *Rapture* was
near-dead without a Climax-on-demand card — a 3-cost Rare keyed off the one event you spend the
whole run trying to avoid, so it only ever paid out when things were already going wrong. Forced
Orgasm turns it from a card you hope to survive into a plan you draft toward. Taking the Uncommon
first and hoping for the Rare is a normal shape for a build.

---

## 5a. Freehand (Skin) — 5 cards

Hands, breasts, thighs, skin. No hole used, so nothing is collected. Rather than paying for that
with raw damage, these now do things the tagged groups cannot.

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Handful** | 0 | Attack · *Skin* | Common | **Innate.** Deal **11** damage. **Exhaust.** | 14 damage |
| **Smothered** | 2 | Attack · *Skin* | Common | Deal **5** damage. Gain **10** Block. Gain **5** Block next turn. | 7 / 13 / 7 |
| **Thighplay** | 1 | Attack · *Skin* | Uncommon | Deal **9** damage. Your next **Ass** or **Pussy** card costs **1 less**. | 12 damage |
| **Show Off** | 0 | Attack · *Skin* | Uncommon | Gain **1 Vulnerable**. Deal **10** damage to ALL enemies and apply **1 Vulnerable**. | 14 damage |
| **Wet Work** | 0 | Skill | Uncommon | **Spend 10 ml.** Gain **10** Block. | Spend 8 ml |

**The group has an identity now.** Previously it was "the same cards but bigger", which is a
weak reason to draft anything. Each of these does something structural instead:

- *Handful* is a free opener, and being **Innate** means it is always in your first hand — which
  drags your *Continuing* mode to Skin on turn one. Playing it is tempo; holding it is routing.
  That tension only exists because it is Innate, and it is the best thing about the card.
- *Smothered* makes Skin the defensive colour, which nothing else covers.
- *Thighplay* is the bridge: a Skin attack that discounts your next Ass or Pussy card, so a
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
means the family is the only way to bank mid-combat — everything else waits for the bell, or for the
100 ml Stomach milestone — which is the actual reason to draft into it rather than just playing big Mouth attacks.

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Throat Fuck** | 1 | Attack · *Mouth* · **Deep Throat** | Common | Deal **9** damage. Lose **2** HP. | 12 damage |
| **Deep Throat** | 2 | Attack · *Mouth* · **Deep Throat** | Common | Deal **16** damage. Lose **2** HP. Draw **1**. | 20 damage |
| **Balls Deep** | 1 | Attack · *Mouth* · **Deep Throat** | Uncommon | Deal **9** damage. Lose **2** HP. Put a card from your discard pile on top of your draw pile. If it is a different **Mouth** card, put it into your hand instead. | Deal 12 |
| **No Gag Reflex** | 1 | Power | Uncommon | **Deep Throat** cards cost **1 less** and no longer lose HP. | Also draw 1 on each |

*Balls Deep* is now recursion rather than a damage spike, and the Mouth clause is a routing
reward rather than a threshold check: it always returns something, and it returns it *faster* if
you are actually playing Mouth cards. The **2** HP keeps the family's whole cost pattern intact,
so *No Gag Reflex* removes something real on all three members.

**"Different"** does the real work. Retrieving *Balls Deep* itself puts it on the draw pile, not
in hand, so the loop always costs a draw — which is what stops it becoming a soft infinite once
*No Gag Reflex* has zeroed the cost and the HP. Self-recursion stays available, it is just never
the fastest line.

### The rest

**[Gorim, 2026-11-17] *Milking Throat*** — 1, Power, Uncommon: *Whenever you gain precum, gain **3** ml more and **Eat** it.*
Upgrade 5 ml. [Claude] The extra ml is eaten directly and never enters the Mouth, so each precum gain also heals 1.
It triggers on cards and powers, not on the Stomach milestone's combat-start precum. Upgrade number is [Claude]'s.

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Lip Service** | 1 | Attack · *Mouth* | Common | Deal **7** damage. | 10 |
| **Lick Pre-cum** | 1 | Skill | Common | Gain **5 ml precum**. Apply **1 Weak** to ALL enemies. | 8 ml |
| **Spit** | 1 | Attack | Uncommon | Empty your Mouth. Deal damage equal to **50%** of the ml to ALL enemies, and apply **1 Weak and 1 Vulnerable per 10 ml** emptied. | 100% |
| **Restraint** | 2 | Power | Uncommon | While your HP is above half, gain **4** Block at the start of your turn. | 6 Block |
| **Milked Dry** | 3 | Attack · *Mouth* | Rare | Deal **10** damage to ALL enemies. Gain **15 ml precum**. | 14 damage |

*Spit* is untagged despite being an attack — you are expelling, not receiving, so it fills
nothing and never sets the Continuing mode.

**[Gorim, 2026-09-08] *Gulp* is cut from the pool.** The name now belongs to the **100 ml Stomach
milestone**: the Mouth also swallows whenever it reaches 50 ml. The cut card's Sensitive removal went
with the name — a swallow of **25 ml or more** removes **1 Sensitive** once that milestone is reached.
The idea did not want to exist twice. That leaves *Spit* as the only card that empties the Mouth, and
*Wet Work* as the only card that exercises the spend order.

***Scoop It Up* is cut from the pool.** With every reservoir's leak now landing on Skin (design
§6.3c), a single card converting Skin into Mouth would have been close to mandatory in every deck —
and a card nobody can skip is worse than no card at all. Its job moved to the rest site: **Indulge**
replaces `HEAL` for this character, emptying Skin to Soothe 1 per ml and banking that ml to the
**Stomach**. Draining still gets the Skin→Stomach pipeline; it now runs through the map rather than
the deck, and every archetype feeds it.

---

## 7. Squeezing (Ass) — 7 draftable cards, plus *Milking Grip*

**[decided] The Ass tag has its own rule**, and it is the archetype:

> **Ass-tagged attacks:** lose **10% of your current Ass** (rounded up). This attack gains
> **+1 Strength per 4 ml lost** (rounded down).

**[Gorim, 2026-09-07] Third setting of the ratio.** +1 per 10 ml never paid, because the bank rarely
reached the 100 ml that produced a single point; 1:1 made *Blowout* deal 424 at a full bank. +1 per 4 ml
is tuned for the 200–270 ml a deck holds once *Milking Grip* is in play. The bonus is flat damage added
to every hit of that attack, not a Strength stack, so it cannot leak into the rest of the turn.

A hole in active use leaks, and the leaking is what powers the hit. Because the Strength applies
per hit, this pushes the whole archetype toward **multi-hit attacks** — which is where its
identity now lives, rather than in reading a number.

Worked: at 200 ml a card loses 20 ml and gains **+5** for that attack; at an inflated 400 ml it
loses 40 and gains **+10**. On a three-hit card that is +15 or +30 damage. The bank is ammunition
that depletes across a fight and refills from kills routed back into it.

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Clench** | 1 | Attack · *Ass* | Common | Deal **5** damage twice. Lose **2** HP. | 7 twice |
| **Stuffed** | 2 | Attack · *Ass* | Common | Deal **12** damage. Lose **3** HP. If your HP is below half, hit **one additional time**. | 15 damage |
| **Grind** | 1 | Attack · *Ass* | Uncommon | Deal **6** damage. Gain Block equal to the damage dealt. Lose **1** HP. | 8 damage |
| **Rut** | 2 | Attack · *Ass* | Uncommon | Deal **6** damage three times. Lose **3** HP. | 8 three times |
| **Stretch My Hole Daddy** | 2 | Power | Uncommon | Attacks that cost you HP hit **one additional time**. | Costs 1 |
| **Overflow** | 1 | Attack · *Ass* | Uncommon | Deal **1** damage to ALL enemies and apply **Vulnerable**. This card loses **double** the usual Ass. | Also 1 Weak |
| **Blowout** | 3 | Attack · *Ass* | Rare | This card loses **50%** of your Ass instead of 10%. Deal **6** damage four times. | 8 four times |

**Not draftable — granted free at 100 ml Ass** (high-water mark; design §6.1a):

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Milking Grip** | 2 | Power | Uncommon | Ass attacks lose **half as much** Ass, but gain Strength **as though they had lost the full amount**. Ass kills gain **double** intake. | Lose **30%** as much (3% of the bank) |

**[Gorim] *Milking Grip* left the draft pool** when it became the 100 ml threshold reward: an early pick
would skip the progression and turn the thresholds into decoration. The **Plug** relic follows at 200 ml.

**[decided] The old *Grind* was strictly better than *Clench*** and is gone. Three hits of 3 for
no HP beat two hits of 5 for 4 HP the moment the Ass rule applies, because Strength lands per
hit: at 200 ml that was 5×3 = 15 free against 7×2 = 14 for 4 HP. A common that dominates another
common at the same cost is a dead draft slot, not a choice.

*Stuffed* takes the vacated Common slot. The new *Grind* is the archetype's only defensive card,
and deliberately **single-hit** — which inverts everything else here. The Ass rule still applies,
so at 200 ml it is 11 damage and **11** Block for 1 energy and 1 HP, and the Block scales with the
bank exactly like the damage does. *Clench* is not dominated in return: multi-hit still wins on
raw output the fuller you are, so the two commons split cleanly into "spend the bank for damage"
and "spend the bank for a turn of safety".

*Grind* drains the bank to produce Block, which is ammunition spent on defence — but Squeezing has
to survive at low HP, so it needs a defensive card and this is the only one that scales with the
thing the archetype is already hoarding. Paying ammunition for it is the correct cost.

*Blowout* at 200 ml loses 100 ml for +25 Strength, so four hits of 31 — 124 damage, at the cost of
half the bank. That is the archetype's ceiling and it should hurt to use.

*Milking Grip* is the archetype's engine rather than a compromise: half the drain, **full**
Strength, and double intake on Ass kills. It solves Squeezing's structural problem — a bank that
depletes faster than kills refill it — which is why it is now handed out at the 100 ml threshold
rather than left to the draft.

*Stretch My Hole Daddy* multiplies everything, because Strength applies per hit. *Clench* becomes
5×3, *Rut* becomes 6×4, *Stuffed* becomes 12×2 (×3 below half), and *Grind* becomes 6×2 — which,
if Block is per instance of damage, doubles the Block too. **[decided]** *Grind* grants Block
**per hit**, so under the Power it is 6×2 damage and 6×2 Block. That is intentional: the Power is
a 2-cost Uncommon that only pays off in a deck built around HP-cost attacks, and this is one of
the payoffs it exists for.

*Overflow* is the cheap bank-dump: base damage of 1 is almost irrelevant, since the Ass rule at
double drain does the work. At 200 ml it loses 40 ml for +10 Strength, so 11 damage and Vulnerable to
everything for 1 energy; at an inflated 400 ml it is 21 to everything. The drain is the real cost,
not the energy — which is why it is Uncommon at 1 rather than Rare at 2.

---

## 8. Breeding (Pussy) — 9 cards

Finishers, Conception, and the Sigil payoff. **Sigil gating at 1 / 3 / 5 / 7**, on the fixed tier
shape from design §7.1:

> **1** adds an effect · **3** grows the base · **5** grows the effect · **7** repeats, a number of
> times equal to its cost. **0-cost cards lose their non-energy cost at 7** instead of repeating.

| Card | Cost | Type | Rarity | Text | Upgrade |
|---|---|---|---|---|---|
| **Take Him In** | 1 | Attack · *Pussy* | Common | Deal **8** damage. *1:* draw **1**. *3:* deal **12** instead. *5:* draw **2** instead. *7:* repeats **1** time. | 11 damage |
| **Surrender** | 0 | Skill | Common | Lose **4** HP. Draw **1**, plus **1** per Climax this combat. *1:* gain **5** Block. *3:* draw **2** plus per Climax. *5:* gain **10** Block instead. *7:* lose no HP. | Lose 3 HP |
| **Ward** | 1 | Skill | Common | Gain **6** Block. *1:* reflect **5** damage to attackers this turn. *3:* gain **12** Block instead. *5:* reflect **10** instead. *7:* repeats **1** time. | 9 base |
| **Claim** | 1 | Attack · *Pussy* | Uncommon | Deal **7** damage. Lose **1** HP. *1:* apply **1 Vulnerable**. *3:* deal **12** instead. *5:* apply **2 Vulnerable** instead. *7:* repeats **1** time. | 10 / 16 |
| **Leg Lock** | 1 | Skill | Uncommon | Gain **6** Block. Apply **1 Vulnerable** and **1 Leg-Locked**. *1:* also apply **1 Debilitate**. *3:* apply **2 Leg-Locked** and **2 Vulnerable**. *5:* apply **2 Debilitate** instead. *7:* apply all of it to ALL enemies. | 9 Block |
| **Ritual Mark** | 1 | Power | Uncommon | Your **Pussy** and **Breasts** cards deal **2** additional damage. *1:* your Sigils deal **1** additional damage. *3:* they deal **4** instead. *5:* Sigils deal **4** too. *7:* both bonuses become **+1 per Sigil**. | Innate |
| **Consummate** | 0 | Skill | Rare | **Exhaust a Sigil** for this combat: gain **2** energy and draw **3**. *1:* gain **10** Block. *3:* gain **3** energy instead. *5:* gain **20** Block instead. *7:* does not exhaust a Sigil. | Draw 4 |
| **Rapture** | 3 | Power | Rare | Gain **10** Block. Whenever you Climax, gain **3** energy and **2 Strength**. *1:* also gain **10** Block whenever you Climax. *3:* this costs **1 less**. *5:* both Block values become **20**. *7:* **double** the energy gained. | 3 Strength |

**[Gorim, 2026-11-17] *Hyperfertile*** — 2, Power, Rare: *Gain twice as much Pussy cum. Your Pussy attacks deal **1%** more
damage for each HP the enemy is missing.* Upgrade costs 1 ([Claude]'s number). Not tiered by Sigils.

> **Leg-Locked** — while an enemy is Leg-Locked, its death routes its output to your **Pussy**,
> whatever killed it. Counts down at the end of its turn.

**Leg-Locked is a kill claim** — a claim staked on a kill that has not happened yet, which is why
it counts down. It is a rules word rather than just this card's text, and it solves a problem the
design has carried since §6.0. Routing keys off the *finishing card's* tag, so a kill delivered by Poison, a
Sigil passive, reflect damage, or any untagged card falls through to Skin (§6.4) — and Breeding,
which needs Womb fill more than any other archetype, is exactly the deck most likely to kill things
with something other than an attack. Leg-Locked lets you **commit the route in advance**, which is
a decision rather than a correction.

It **overrides both** the finishing-card tag and the Skin catch-all. Because it counts down like
Debilitate, marking an enemy you then fail to kill is wasted — so the card asks you to read the
fight, and stacking it is buying time rather than buying power.

**Debilitate is vanilla and generic**, not Necrobinder-locked — a `PowerModel` with
`StackType.Counter` that Vulnerable and Weak both query. **Its stacks are duration, not intensity**:
the multiplier is `amount + (amount - 1)` applied once however many stacks are on, decrementing at
the end of the owner's turn. So *Leg Lock*'s tier 3 buys two enemy turns of doubled Vulnerable, not
double the doubling. It also only fires on powered attacks, so it never touches your own
self-damage.

**Every gated card now reads the same way**, so a Breeding deck holds one ladder rather than a
dozen unrelated clauses. The scale lives in the shape; only the specifics differ.

**[Gorim, 2026-09-07] *Ritual Mark*'s upgrade is Innate and touches no numbers.** "+3 base" moved only
the Pussy side and broke the "too" parity at 5 Sigils; lifting both sides made it far too strong. Getting
the engine online on turn one is a real upgrade for a Power and leaves every tier value alone.
**[Gorim, 2026-09-08] It also buffs Breasts cards** — the Breeding subcategory for cards that do not feed
the Womb, of which *Milk* is so far the only one.

**Not every card is gated.** *Ritual Mark* and *Rapture* carry no tiers — *Ritual Mark* already
scales with Sigils through its effect rather than through a threshold, and *Rapture* keys off
Climax, which is the other axis. Forcing tiers onto them would add text without adding a decision.

*Claim* loses the "at 4 Sigils, stop costing HP" clause it carried in v10. Removing a downside does
not fit any rung of the ladder, and the shape is worth more than that one clause. The 1 HP now
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
choosing the moment worth real tempo. It pairs with *Forced Orgasm* (§5), which replaced *Abandon*: a Climax on
demand with no Lust card, paid for in Sensitive instead of the death clock.

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

## 9a. Milk — generated, not draftable

Shuffled into your **combat draw pile** while **Lactating** — **N cards per turn**, where N is your
pregnancy stage. Lactation begins with pregnancy and continues one combat past the birth (design
§6.3b). Never enters the master deck. Not in any pool and not draftable.

| Card | Cost | Type | Text | Upgrade |
|---|---|---|---|---|
| **Milk** | 0 | Attack · *Skin* · **Breasts** | Heal **4**. Deal **2** damage. **Exhaust.** *1:* draw **1**. *3:* heal **7** instead. *5:* draw **2** instead. *7:* does not Exhaust. | — |

**[Gorim, 2026-09-08] It is a heal card first and an attack second**, so the numbers are inverted from
the earlier 4 damage / 2 heal, and the 3-Sigil rung grows the **healing**. **Breasts is Skin** the way
Deep Throat is Mouth — a category tag paired with a real hole — so Milk routes its kills to Skin and
drags the *Continuing* mode to Skin when played. At Stage 3 that is three a turn pulling a Breeding
deck off its own routing, which is intended: each pregnancy should make the next harder to reach.

It rides the standard Sigil ladder rather than inventing a scale, and the 0-cost tier-4 rule pays
off unusually well here: a 0-cost card sheds its **non-energy** cost at 7 Sigils, and Milk's
non-energy cost is **Exhaust**. At the top of the ladder Milk stops exhausting and starts piling
up — producing faster than you can spend, from a rule that already existed.

**Into the combat pile, never the master deck**, so nothing persists between fights. The pile is
deliberately weaker than the hand: Milk competes with your real cards for draws, and at Stage 3
that is three cards a turn, so a long fight becomes mostly Milk. The dilution is a cost, and it is
one more thing arguing for cashing out.

**It heals rather than Blocks, and that is what makes gestation a gamble.** Healing cannot prevent
a one-shot: at Stage 3 you take **+150%**, so even topping up to full every turn you are raising
the chance a single enemy intent kills you outright. The carried state is a rising probability of
sudden death that no amount of sustain insures against — Block would have made it merely slow.

---

## 10. Lust — 2 cards (not draftable)

| Card | Cost | Type | Text |
|---|---|---|---|
| **Aching** | 0 | Curse | Playable: gain **2 energy** this turn. Lose **6** HP. |
| **Craving** | 0 | Curse | Playable: draw **3**. Lose **6** HP. |

Climaxing while holding **3** Lust cards ends the run (design §5), so a fourth is never gained.

---

## 11. Distribution

| Group | Count |
|---|---|
| Basics (not draftable) | 5 |
| Core | 4 |
| Freehand (Skin) | 5 |
| Draining (Mouth) | 9 |
| Squeezing (Ass) | 7 |
| Breeding (Pussy) | 8 |
| **Draftable total** | **33** |
| Ancient | 1 |
| Milking Grip (100 ml Ass reward) | 1 |
| Milk (generated) | 1 |
| Lust (not draftable) | 2 |

By rarity the 33 are **13 Common / 15 Uncommon / 5 Rare**; by type **18 Attacks / 9 Skills / 6 Powers**.

| Group | Common | Uncommon | Rare |
|---|---|---|---|
| Core | 2 | 1 | 1 |
| Freehand | 2 | 3 | 0 |
| Draining | 4 | 4 | 1 |
| Squeezing | 2 | 4 | 1 |
| Breeding | 3 | 3 | 2 |

32 is a floor, not a ceiling — and 33 is well short of a vanilla pool. See §14.

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
- *Handful* being Innate means every Skin draft changes your turn-one routing whether you want
  it to or not. That is the interesting part, but it may be too loud in a focused Ass or Pussy
  deck that drafted the card purely as a cheap opener.
- The Deep Throat family is three cards. If *No Gag Reflex* feels weak, the fix is a fourth family
  member rather than buffing the Power.
- *Balls Deep* returning itself is gated to the draw pile by the "different" clause, so the loop
  costs a draw per replay. Confirm that holds against draw engines like *Afterglow*.

---

## 13. Where the build's card text was wrong

[Claude, 2026-11-14] Fixed the same day and checked in game through Spire Link's card catalogue:

- *Bat Eyelashes+*, *Overflow+* and *No Gag Reflex+* now print their extra effect, using the game's
  `{IfUpgraded:show:upgraded|base}` text form. `tools/check_card_text.py` knows that token.
- *Lick Pre-cum* says "to ALL enemies"; *Handful* prints "Skin."
- *Suckle* reads "If this kills, gain 5 ml of cum instead", so the two 5s are no longer identical.
- *Milking Grip+* is wired: 3% of the bank instead of 5%, printed as "only 30% as much". Verified:
  at 200 ml an upgraded Grip made *Clench* spend 6 ml for +5 damage a hit.
- The character-select blurb says three Lust ends the run (not checked on screen).

Still open:

- *Milking Grip*'s power tooltip says "half as much" even when the upgraded card set it to 30%.
- The starting relic is still Ironclad's *Burning Blood*; the character's own relic (design §6.5, §8) is
  not built. There are no character potions.

## 14. Gaps in the pool

[Claude, 2026-11-14] From an audit of the 33 draftable cards. Proposals, not decisions.

- **Rares.** Five in total; Freehand has none, Draining and Squeezing one each.
- **Core.** (Written at four cards; *Keep Going* and *Faster* have since been built.) No draftable Block outside the archetypes and no draftable *Continuing*
  attack — the rule lives only on *More*.
- **Squeezing** has no Skills and one Block card (*Grind*), and only *Stuffed* pays off being below half HP.
- **Draining** has only *Restraint* paying off being above half HP. With *Gulp* gone, *Spit* is the only
  card that uses a full Mouth, and no card removes Sensitive.
- **Skin** only accumulates: *Wet Work* and the rest-site Indulge are all that use it.
- **Powers that fill a reservoir** are legal (§3) but only *Edging Form* does it.
- **Breasts** is a tag with one card. No pregnancy-gated cards exist.
- **Leg-Locked** is a rules word carried by one card.
