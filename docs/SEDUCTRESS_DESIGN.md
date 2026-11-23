# The Seductress — mechanical design v3.12

Status: design draft, 2026-09-07. Personal-use mod, single-player only.
**Engine spike complete** — see `SEDUCTRESS_SPIKE_RESULTS.md`. All four risks answered, none
blocked. Nothing in this document had to change to fit the engine.

**[Claude, 2026-11-20] v3.12 rewrites the body to the 20 Nov rules change (§0)**, at [Gorim]'s request: ml figures
are on the x5 scale, §6.2 and §6.2a describe intake from damage, worked examples are recomputed (and say so), and
arguments that only made sense under flat per-kill intake are kept but labelled rather than re-argued. Three cards
from [Gorim]'s board notes are in the build: *Mouthful* (with the **Gulp** keyword), *Hold the Edge*, and *Rehydrate*
at 50; the pool is 40. Register rows 117–126 record all of it. **The change notes from here down to §0 are history
and keep the figures of their day.**

**[Claude, 2026-11-14] v3.11 reconciles this document with the build**, at [Gorim]'s request: the *Gulp*
card is cut and its name and Sensitive removal moved to the 100 ml Stomach milestone; every swallow
heals 1 HP per 2 ml, precum included; §8's pool table is replaced with the built counts; register rows
111–116 record the rulings that were only in code comments and slice results. Older register rows are
left as written and marked where superseded.

Changes from v2.4, all from the slice-3 playtest: the Ass Strength ratio settles at **+1 per 4 ml
lost**, tuned for the Milking Grip plateau rather than the unaided bank; **Ass threshold unlocks**
added as the archetype's progression axis; **precum** promoted from a card clause to a built
system. Changes from v2.3: **Leg-Locked** added as a rules word — it commits a kill's route in advance, overriding both the finishing-card tag and the Skin catch-all. Changes from v2.2: Sigil thresholds move to **1 / 3 / 5 / 7** with a **fixed tier shape**;
the starting deck gains **two class cards**. Changes from v2.1, all from the slice-2 build and playtest: §7.1 corrected to **2 / 4 / 6**
(it still said 3 / 6 — a documentation error, the card list was always right); multi-enemy elites
pay **15 ml each**, not 20; §6.3's reachability table recomputed and its conclusion weakened;
**minions produce no intake**; *Breed Me!*'s multiplier replaces its +5 rather than compounding.

Earlier (v1.8): intake rule redrawn by card type (Powers may generate); Sigil thresholds move to 2/4/6.

Earlier (v1.7): **holes and containers separated** (§6.0); tags are attack-only; **Squeezing rebuilt** — the Ass bank is ammunition spent for Strength (§6.1); *Deep Throat* becomes a card family.

Earlier (v1.6): intake rounding rule; Squeezing stops spending its bank; Sensitive removal re-sited onto *Gulp*.

Earlier (v1.5): Plug now **taxes and auto-removes** rather than locking Ass cards out; the "marginal" verdict on wearing it is corrected (§7.3); *Edging Form* added as the Ancient card.

Earlier (v1.4): **Plug** specced as a common toggle relic (§7.3).

Earlier (v1.3): Mouth reworked — end-of-combat swallow, Total Swallowed track, pre-cum exception (§6.5).

Earlier (v1.2): **Freehand** added — no-hole attacks tagged *Skin*, stronger in exchange for collecting nothing.

Earlier (v1.1): basic attack is **Pump**, tagged *Continuing*. **[Gorim, 2026-11-17]** renamed ***More***: *Continuing*
attacks are named as things she says, so name and art fit any hole. Older entries below still say Pump.

Earlier (v1.0): reservoirs measured in **ml** with per-hole capacities (Mouth 50 / Womb 100 / Ass 200); **cards no longer generate intake** — kills are the only source, and each archetype instead gets an in-combat identity based on its position on the HP bar (§6.2a).

Earlier (v0.9): naming rule adopted — vanilla mechanics keep vanilla words, so Poise and Composure are gone; card text is now written in base-game terms.

Earlier (v0.6-0.8): Sigil interaction settled as **threshold gating**, and the late-game
scaling warning withdrawn — a dominant act 3 is the correct shape for this archetype, not a
failure. Squeezing gains **inflation** (overfill above 100%). Skin's fill sources
reconsidered now that poison is likely out. **Sigil passive defined.** Plugs became a
toggleable family that blocks filling as well as leaking.

---

## 0. Rules change of 20 Nov 2026 — read this first

**[Gorim, 2026-11-20]** decided in the design review session and built the same day. It replaces register
decision 44 ("Attacks and Skills do not generate intake") and the kill table. The sections below were
rewritten to these rules the same day ([Claude]); passages kept for their history are labelled as such and keep their
old-scale figures.

| # | Rule | Whose |
|---|---|---|
| 1 | **Intake.** Every Attack gains cum **1:1 with the HP it actually removes**, into the attack's hole. Nothing for overkill, nothing for damage absorbed by Block. | [Gorim] |
| 2 | **Kill bonus.** **25% of the enemy's Max HP** to the finishing hole, replacing the flat 10 / 15 / 40 / 60 ml. Reduce proportionally in multiplayer if that is ever built. | [Gorim] |
| 3 | **Scale.** Every ml figure is **x5** "for now, and see where that gets us". Mouth 250, Womb stage 500, Ass 1000. Pregnancy at N x 500. Milking Grip at 500 ml Ass, Plug at 1000. Stomach: +1 Max HP per 50 ml; precum milestones at 250, 750, 1250, 1750 (25 ml each); Gulp at 500; clean Indulge at 1000; precum banks at 1500. Card ml amounts x5. | [Gorim] |
| 4 | **Healing.** Swallowing heals **1 HP per 10 ml** (was 1 per 2). | [Gorim] |
| 5 | **Ass rule** stays: lose 10% of Ass per Ass attack, bonus damage on every hit. The bonus damage gains cum like any other damage, and attacks with three or more hits are meant to grow the bank without limit, as the Squeezing direction. | [Gorim] |
| 5a | The Ass ratio is **+1 damage per 3 ml lost** (was per 4), and is **not** multiplied by five. Single-hit decks settle near 15x their hit size. | [Claude], seen but not explicitly confirmed |
| 6 | **Sensitive** no longer raises ml intake. Each stack still raises HP loss taken by 50%, and now raises **Pussy damage** ("Pussy damage only for now"). | [Gorim] |
| 6a | +50% Pussy damage per stack. | [Claude], seen but not explicitly confirmed |
| 7 | **Precum** stays. It may extend to other holes later; *Edging Form* is the first card to do it. | [Gorim] |
| 8 | ***Edging Form*** is reworked, since its old effect is now rule 1: **whenever an Attack gains cum, gain that much again as precum in the same hole. Upgrade: costs 1 less.** | [Gorim] |
| 9 | **Leak** per floor stays. | [Gorim] |

**How it was built — [Claude]'s readings, not yet confirmed:**

- **Minions still pay nothing**, for hits as well as kills (the 2026-09-06 ruling carried over).
- **Hyperfertile** doubles all Pussy cum, hits and kill bonus alike. **Milking Grip** doubles the Ass **kill bonus**
  only, as its text says "Ass kills".
- ***Breed Me!***'s flat kill bonus is 25 ml (x5); its Sigil multipliers apply to the 25%-of-Max-HP kill bonus.
- ***Spit*** deals **10%** of the ml emptied (20% upgraded) and applies its debuffs per **50 ml**, so a full Mouth
  does what it did before.
- **Precum in the Ass and Womb** counts as part of the bank during the combat, is spent first, and is gone at the
  bell, before conception is checked. It never counts toward the Ass high-water mark, so it cannot unlock rewards.
  Edging Form copies the per-hit gain only, not the kill bonus, and does nothing for Skin.
- **Belly bands** on the combat rig scale with the rest: inflation is `floor((Ass - 1000) / 500)`.

**Changes of 21 Nov 2026 — [Gorim], after seeing the 20 Nov build in play.** Built the same day. Where the
table above or an older section disagrees, this is right.

| Change | Whose |
|---|---|
| **Caps cut.** Womb stage **250 ml** (was 500), Ass **500 ml** (was 1000), Mouth **100 ml** (was 250). "Womb seems too hard to fill, lets cut it down to 250ml for now and see how that is." | [Gorim] |
| The Ass rungs follow the cap: Milking Grip at 250 ml, Plug at 500, open rung at 750. Belly inflation is `floor((Ass - 500) / 250)`. Stomach thresholds and card ml amounts are unchanged. | [Claude] |
| ***Breed Me!*** kill extra raised from 25 ml (he said 25 was too small). The new figure, **50 ml**, is not his. | [Gorim] / number [Claude] |
| ***Milking Throat***: "Whenever you gain precum, gain twice as much, then **Gulp**" (swallow everything in the Mouth). Replaces "gain 15 ml more and Eat it". Gulp is explained in a hover pop-out, not on the card; *Mouthful* uses the same pop-out. | [Gorim]; Mouthful matching is [Claude] |
| *Milking Throat* upgrade: costs 0. | [Claude] |
| ***Restraint*** costs **1** (was 2). | [Gorim] |
| ***Blowout*** costs **2** (was 3), "for now". | [Gorim] |
| ***Consummate*** **Exhausts**, and with no Sigil to spend it gives **1 energy and 1 card** instead of being unplayable. | [Gorim] |
| ***Hold the Edge*** lasts until the start of your next turn, so it covers the enemies' attacks. "It should be til start of next turn." It used to end with the turn it was played. | [Gorim] |
| **Mouth overflow.** With the 500 ml Stomach milestone (auto-Gulp), a full Mouth Gulps and the rest then goes in: "If you're on 99 and gain 10, it should be gulp 100 gain 9." Asked whether auto-Gulp should be always on, he kept it behind the milestone. **[Gorim, 2026-11-23]** Cum into a full Mouth first pushes out precum, which splashes onto Skin (23 Nov; Skin holds one total, so it counts as ordinary Skin ml), and cum that still does not fit (after any Gulp) splashes onto Skin. Precum that does not fit is still lost. | [Gorim] |
| **UI pass.** The debug text box is replaced. Run-long counters (Stomach, Skin, Sigils, Womb, Ass, Lust) sit in the top bar on every screen; clicking them opens one milestones screen with a bar track per ladder (he picked tracks "for now"; a body map is planned for the hole pictures on the combat screen). In combat each hole picture has its numbers under it. | [Gorim] chose the three forms; layout and wording [Claude] |
| **Hover pop-outs on cards** for Gulp, Precum, Pussy, Ass, Mouth, Skin, Climax, Sigil, Eat. A card gets a term's pop-out when its text uses the word. Text is in `localization/eng/static_hover_tips.json`. | terms [Gorim]; wording is a [Claude] draft awaiting his edit |
| **Sigil cards show only what they do at your current Sigil count** (22 Nov), one effect per line. The tier ladder is off the card face. Four small Sigil icons on the card mark tiers 1, 3, 5, 7 (pink when active). The right-click view has a 0 to 7 slider that rewrites the card for any count. Milestones screen: the Sigil track is seven Sigil icons, the other tracks are drawn as fluid tubes. | [Gorim]; per-tier wording, icon and slider placement are [Claude] |
| **Sigil orbs carry the Sigil mark** on top of the pink orb (22 Nov). | [Gorim]; size [Claude] |
| **Killing a boss removes every Lust card** from the deck (22 Nov). | [Gorim] |
| **Ass rework** (22 Nov): "too hard to accumulate, and too good when you do". Ordinary Ass attacks no longer spend the bank; they deal **+1 damage per hit per 50 ml held**. *Blowout* and *Overflow* are the spending and **ask Spend or Keep** when played. Spend is sized like the floor leak (5% of the Ass up to 500 ml plus 25% of anything above): Overflow spends one leak's worth, Blowout two. It pays **+1 per 10 ml** on every hit **on top of** the standing bonus (was per 3 ml for an hour). The card states the numbers: "You may spend 15 Ass for 1 bonus damage." The floor leak is unchanged. This replaces the "lose 10% per Ass attack" rule wherever older sections state it. | [Gorim]; the Plug still spilling 10% on any Ass card, and Milking Grip now only affecting the two spenders are [Claude] and open |
| **Strip on Power** (22 Nov): each Power played in a combat takes something off the combat rig, in the order "Sleeves and shoes, then wrap, then skirt"; nude breasts follow pregnancy as "modest / bare / enormous / absurd"; dressed again each combat. A fourth stage waits on a new pose. Appearance only. Console `strip [0-3]` previews a stage. | [Gorim]; built with what the rig has: stage 1 hides only the sleeves and stage 3 only the skirt panels, since bare feet, a bare right upper arm and the body under the skirt are not drawn yet [Claude]. Not seen in a fight |
| **Card wording rule** (22 Nov): card text "shouldn't explain where numbers come from, it should just give numbers." | [Gorim] |
| **Sigil card view** (22 Nov): the slider is vertical with seven Sigil hearts and no caption; the four tier icons sit in a strip under the text so they never overlap it; text is kept short (Ritual Mark states the number, not the maths). **Leg-Locked** has a hover pop-out. | [Gorim]; wording [Claude] |
| ~~Open point: the Gulp milestone's "a swallow over 125 ml removes 1 Sensitive" cannot trigger now that the Mouth holds 100 ml.~~ Resolved 23 Nov by the 250 ml Mouth. | [Claude] noticed |
| **Mouth 250 ml** (23 Nov): "Lets make it 250ml and then I think the milestones are fine as is. The current issue is that on a mouth build you cap mouth basically immediately." Milestones unchanged; the Gulp milestone's 125 ml Sensitive removal can trigger again. | [Gorim] |
| **Milk on the combat rig** shows while Lactating, which lingers one combat after birth (22 Nov; replaces the 20 Nov "while pregnant" rule). | [Gorim] |
| **Sigils deal no damage** (23 Nov, trial): "Lets just not make it scale sigils. Lets actually try taking the 1 damage off sigils too." The start-of-turn 1 damage to ALL enemies is gone and Ritual Mark no longer adds to it. | [Gorim] |
| **Sigil display caps at 10** (23 Nov): the game allows 10 orbs; 11 or more Sigils used to softlock. All Sigils still count for card stages. | [Claude] fix |
| **Ritual Mark** (23 Nov): cards only. [Claude] The bonus steps 2 / 3 / 4 / 5 at 0 / 1 / 3 / 5 Sigils, then +1 per Sigil at 7; copies add together. | [Gorim] rule, [Claude] numbers |
| **No Lust on the next Climax** (23 Nov): playing Rapture or Afterglow gives 1 stack of **Ride It Out** ("should stack as a single buff tracker between those powers. Probably call it 'Ride it out'"); each stack makes one Climax that combat add no Lust card. [Claude] It also stops that Climax being fatal at the Lust limit. | [Gorim], [Claude] reading |
| **Consummate** (23 Nov): "Top stage should no longer exhaust the card, but still the sigil." | [Gorim] |
| **Ward** (23 Nov): the reflect fades at the start of your next turn, not permanent. | [Gorim] note, [Claude] timing |
| **Thighplay** (23 Nov): upgrade is "2 discount", replacing +3 damage. Making the discount lasting was floated, not built. | [Gorim] |
| **Afterglow stages** (23 Nov): 1 = 20 Block, 3 = draw 3, 5 = 30 Block, 7 = Climax also gives 2 Energy. | [Claude] proposal, [Gorim] accepted |
| **Hyperfertile stages** (23 Nov): 1 = 2% per missing HP, 3 = costs 1 less, 5 = 3%, 7 = three times as much Pussy cum. A second copy adds nothing. | [Claude] proposal, [Gorim] accepted |
| **Hole insets** (mouth, womb, ass) in a row along the top of the combat screen: fill by area, thrust on each Attack hit and until the next card or death, big spurt and pull-out when an enemy dies; thickness normal / elite thick / boss massive. | [Gorim]; details in `UI/HoleInsets.cs` |

Agreed in principle, **not built**: archetypes defined by events rather than bar position (Draining = healing,
Squeezing = losing HP to your own cards, Breeding = Climaxing); "every Power you play strips an item of clothing".

---

## 1. What this design carries

Both bars are spoken for — enemy HP is their arousal, yours is yours, using the HP variable
literally so vanilla relics, events, heals and damage keep working. That alone is a reskin,
so the mechanics live at the **ends** of the bars:

1. Your own best cards raise your arousal. The bar is fuel as well as a life total.
2. Filling it doesn't kill you — you climax, a costed reset you can trigger deliberately.
3. Emptying an enemy's bar makes them climax and pass out; the fight ends.
4. Their climax fills one of four reservoirs, and which one you fill is your build.

---

## 2. Vocabulary

**[decided] Naming rule: if it already exists in vanilla, it keeps the vanilla word.** Only
genuinely new systems get new names. Renaming Block to "Poise" and Max HP to "Composure" bought
nothing but ambiguity, so both are gone. Card text is written in vanilla terms — *Deal 6 damage*,
*Gain 5 Block*, *Lose 6 HP*, *Heal 8* — and the lewd translation pass happens at the end, once
over the whole set, when it can be done consistently.

Terms below are the new systems only. Everything else means what it means in the base game.

| Term | What it actually is | Notes |
|---|---|---|
| **Arousal** | The *presentation* of `MaxHp - CurrentHp` | The bar, drawn inverted so it fills. Not a separate stat — losing HP is gaining arousal |
| **Climax** | Death interception at 0 HP | §4 |
| **Sensitive** | In-combat stacking state | +50% HP loss taken **and** +50% Pussy damage. It no longer raises intake (§0) |
| **Lust** | Playable curse-type card | The death clock, §5 |
| **Intake** | Four reservoirs | §6 |
| **Conception** | The Womb reservoir's fill | 500 ml ⇒ pregnancy ⇒ Sigil |
| **Stomach** | Lifetime total swallowed | Draining's permanent track; +1 Max HP per 50 ml. **Precum does not count** until the 1500 ml milestone |
| **Precum** | Combat-only fuel; Mouth by default | Spends like cum inside a fight, and first. Heals when swallowed, banks nothing. *Edging Form* also makes it in the Ass and Womb |
| **Eat N** | Rules word | Take N ml in the Spend order and swallow it: heals, and the cum goes to the Stomach |
| **Gulp** | Rules word | Swallow everything in your Mouth now. Also the name of the 500 ml Stomach milestone |
| **Sigil** | Permanent passive earned by pregnancy | §7 |
| **Skin** | External catch-all reservoir | §6.4 |
| **Continuing** | Attack keyword | Uses whichever hole your last Attack used this combat; Skin if none yet. Resets each combat |
| **Freehand** | Attacks tagged *Skin* | Hands, breasts, thighs. No hole used, so nothing is collected — ~40% more damage in exchange |

Prose in this document still says "arousal" where it means the system, because "HP loss
persists between fights as HP does" is circular. **Card text never does** — see
`SEDUCTRESS_CARDS.md`.

**[decided]** No custom debuff keyword. **Weak** and **Vulnerable** are used as-is — both
words translate into the register without any work, and reusing vanilla keywords keeps the
character legible to anyone who already plays the game. Entranced is dropped.

> **Naming collision to resolve.** "Lust cards" are the death clock and "Lust Sigils" would
> be the reward. Two opposite things sharing a name in the same UI will confuse. Proposal:
> curses stay **Lust**, rewards are just **Sigils**. Alternatives if the ritual framing
> needs the adjective: Brands, Marks, Seals.

---

## 3. Core combat loop

Enemy attacks raise arousal. So do your strongest cards — the blood-magic axis, and what
makes escalation a choice rather than something done to you. Block prevents it.

**Max HP 20** — see §6.5.

Arousal persists between fights as HP does. Starting relic reduces it after each combat;
rest sites reduce it via the vanilla heal path.

---

## 4. Climax

At full arousal you do not die:

1. Arousal resets to **0**.
2. **+1 Sensitive** for the rest of combat.
3. **+1 Lust card**, permanently. **[decided]** No exception for the first climax of a run.
4. Voice line.

**[decided] Climax does not end your turn.** The turn-end was never asked for and it fails on
implementation before it fails on design: Climax fires mid-card-resolution when arousal fills, so
ending the turn from inside a multi-hit attack means interrupting a resolution queue that is still
in flight. The cost is already real without it — **+1 Lust is a fifth of the run**, and +1
Sensitive compounds every remaining hit of the combat. Adding a tempo punishment on top taxed the
same event three times.

**Sensitive is +50% per stack** to arousal taken. **[Gorim, 2026-11-20]** It no longer applies to Intake gained; each
stack raises **Pussy damage** instead ([Claude]: by the same +50%). **[decided]** The arousal half includes **your own self-damage**, ignoring `ValueProp.Unpowered` — a deliberate departure
from what `Unpowered` means everywhere else in the game, made because most of your arousal comes
from your own cards and the arithmetic below is false without it. The consequence is a real one:
*Let Go* at 3 stacks is 50 HP on a 65 HP character, so a starting card becomes a self-kill button
late in a long fight. That is the character.



- Two climaxes **halve your effective bar** (`1 + 0.5×2 = 2.0`); a third is nearly immediate.
- ~~One climax against a boss nearly pays a whole pregnancy: 60% × 1.5 = **90%**.~~
- ~~One climax entering an elite with 40% banked completes one: 40% × 1.5 = 60%, +40% = **100%**.~~
  [Claude, 2026-11-20] Written for the flat-intake rule; not re-argued for the new one. Sensitive no longer multiplies intake.

Sensitive clears at end of combat. Lust, Intake and Sigils do not.

---

## 5. Lust cards — the death clock

Playable. Each gives a real benefit and a large chunk of arousal. The card never kills you
directly; it accelerates the loop that does.

- **[decided] Holding 3 Lust ends the run** — stated as a threshold, never as a count of
  climaxes, because that ambiguity has now cost three separate revisions. You gain a card on
  climaxes 1, 2 and 3; the fourth ends the run. Baseline is **3 climaxes per run**.
- **At 2 Lust you cannot afford to climax** without a removal in hand.
- **[decided] A Lust card enters the run deck, not the current combat.** Climaxing repeatedly
  inside one fight is close to free *in that fight* — Sensitive is the in-combat punishment, Lust
  is the between-combats one, and stacking both on the same event taxed it twice. Confirmed in
  code: `AddCurseToDeck` adds to the master deck, so the card cannot be drawn until the next
  combat shuffles it in. The lever, if this ever reads as too soft, is
  `AddGeneratedCardToCombat(PileType.Discard)`.

Draft: cost 0, +12 arousal, benefit around "+2 energy this turn" or "draw 3".

### Relief — Lust removal at rest sites

**[decided]** A `CustomRestSiteOption` that removes **1 Lust card** instead of healing. This is
what makes a 3-card clock survivable rather than brutal, and it is the reason the clock could be
tightened at all: **the cap and the availability of removal are two dials, and only one was being
turned.** A single number cannot serve both act 1, where climaxes are accidents, and act 3, where
Breeding wants them deliberately.

**It is self-limiting through the resource it protects.** For this character HP *is* arousal, so
giving up the heal means entering the next fight already aroused. No per-act cap is needed — the
cost scales naturally with how dangerous the next floor is.

It also gives the rest site a three-way decision where vanilla has two, and the archetypes want
different things from it: Breeding takes relief and enters act 3 permanently under-rested;
Draining almost never does, because staying high on the bar is its whole plan.

**Watch:** three or four rest sites per act means ~10 potential removals a run, which would make a
3-card clock meaningless if the opportunity cost turns out to be soft. If so, the fix is making
relief cost something on top — max HP, or a Sensitive stack carried into the next fight — rather
than limiting how often it appears.

---

## 6. The reservoirs

### 6.0 Holes and containers are different things

**[decided]** The design was conflating the hole a card uses with the container the semen ends up
in. Separating them fixes the naming and clarifies the systems:

| Hole (the tag) | Container | Capacity | Behaviour |
|---|---|---|---|
| **Mouth** | **Mouth** in combat → **Stomach** after | 250 ml → unbounded | Holds cum *and* precum; cum banks, precum evaporates |
| **Pussy** | **Womb** | 500 ml per stage | Leaks 10%/floor; 500 ml ⇒ pregnancy ⇒ Sigil |
| **Ass** | **Ass** | 1000 ml | Both hole and container |
| *(none)* | **Skin** | — | External; **does not leak** — accumulates until Indulged |

*Womb* is no longer a tag — the tag is **Pussy**, and Womb is what it fills. **Stomach** replaces
"Total Swallowed" as the name of Draining's permanent track, which reads far better.

**[decided] Only attacks carry tags.** A tag names the hole the attack's cum goes to: what each hit gains, and the
kill bonus if it finishes the enemy (§6.2).
Skills never finish anything, so they are never tagged no matter how hole-specific their text is.
Consequence: only attacks set the *Continuing* mode, so you lead with a tagged attack and your
Pumps follow it. The earlier claim that tagged skills exist to set the mode is withdrawn.

Every hit routes what it gains via the attack's **Mouth / Ass / Pussy** tag, and when an enemy climaxes the
attack that finished them routes the kill bonus the same way.

| | **Draining** (Mouth) | **Squeezing** (Ass) | **Breeding** (Pussy) |
|---|---|---|---|
| Shape | Defensive / control | Aggro / damage | Scaling / engine |
| Leak per floor | 0% | 5% of current | 10% of current |
| In-combat payoff | Spent for Block, healing, Sensitive removal | **Bank is ammunition** — attacks spend it for Strength | — |
| Run-level payoff | **Stomach** ⇒ permanent Max HP | None — the ammunition loop is its rhythm | Pregnancy ⇒ Sigil |
| Arousal relationship | **Minimises** it — rarely climaxes | **Runs hot** — hits hard, self-arouses | Accepts it; climaxing accelerates |

Draining is self-limiting in a way worth keeping: never climaxing means never gaining
Sensitive, so never getting the intake multiplier *(which no longer exists — §0; [Claude, 2026-11-20] Written for the flat-intake rule; not re-argued for the new one.)*. It is deliberately the safe build — and
its permanent Max HP track is the right reward for it, since more capacity is exactly
what a cautious player wants.

### 6.1 Squeezing: the bank is ammunition

**[decided] The Ass tag carries its own rule:**

> **Ass-tagged attacks** lose **10% of your current Ass** (rounded up), and gain **+1 Strength
> per 3 ml lost** for that attack.

A hole in active use leaks, and the leaking is what powers the hit. This replaces the earlier
"read the reservoir" design entirely and gives Squeezing the in-combat identity it was missing —
its bank is not a stat to protect, it is **ammunition**, spent across a fight and refilled by
the damage it deals: **[Gorim, 2026-11-20]** the bonus damage gains cum like any other damage, and attacks with
three or more hits are meant to grow the bank without limit.

Because Strength applies per hit, the archetype now wants **multi-hit attacks**, which is a real
build signature rather than a number on a card. [Claude, recomputed 2026-11-20 for the 1000 ml cap and +1 per 3 ml]
At 1000 ml an attack loses 100 ml for **+33**; at an inflated 2000 ml it loses 200 for **+66**. On a three-hit card
that is +99 or +198 damage.

**The ratio is on its fourth setting.** [Claude, design review 2026-11-20; seen by Gorim, not explicitly confirmed]
**+1 per 3 ml lost**, which is what lets a three-hit attack put back more than it spends. It is deliberately **not**
multiplied by five with the other ml figures. Single-hit decks settle near 15× their hit size.

*History, old scale:* **[decided 2026-09-07]** the ratio was tuned for the plateau, not the unaided bank. §6.1
originally said +1 per 10 ml lost, slice 3 raised it to 1:1 because the bank never reached the 100 ml that produced a
single point of Strength, and 1:1 then made *Blowout* deal 424 at a full bank. +1 per 4 ml lost was set against where
an Ass deck settled once *Milking Grip* was in play — 200–270 ml.

[Claude, recomputed 2026-11-20] What the build does at a full and at a doubled bank:

| Card | Drain | At 1000 ml | At 2000 ml |
|---|---|---|---|
| Clench | 10% | +33 → 2 × 38 | +66 → 2 × 71 |
| Rut | 10% | +33 → 3 × 39 | +66 → 3 × 72 |
| Overflow | 20% | +66 → 67 to all | +133 → 134 to all |
| Blowout | 50% | +166 → 4 × 172 = **688** | +333 → 4 × 339 = **1356** |

These are much larger than the old table (*Blowout* was 124 and 224) because the bank is five times bigger while
the ratio got tighter instead of looser. Whether a deck actually reaches a full bank under 1:1 intake is not yet
known, and none of these figures has been looked at in play. [verified, 2026-11-20] At 300 ml *Clench* lost 30 ml
for +10 a hit and its two 15-damage hits put 30 ml back.

*History, old scale:* this choice depended on the unlock system below. At 100 ml the ratio paid only +2 or +3, so a
deck stuck at the unaided ceiling barely felt the mechanic. That was acceptable *only* because
reaching 100 ml granted *Milking Grip*, which moved the settled bank into the range the
ratio was tuned for. [Claude, 2026-11-20] Written for the flat-intake rule; not re-argued for the new one.

**Inflation** still applies and now matters more: the excess above 1000 ml leaks 25% per floor
outside combat, but inside a fight it is simply a deeper magazine. Filling to 2000 before an elite
means your first few attacks get roughly double the bonus.

The counterweight is *Milking Grip*, which halves the drain while paying the **full** Strength and
doubling the Ass kill bonus.

### 6.1a The Ass economy had a fixed point — and unlocks were the answer

> [Claude, 2026-11-20] Written for the flat-intake rule; not re-argued for the new one.
> The fixed-point argument (the next three paragraphs) depends on intake being a flat 10 ml per kill against a
> percentage spend. Intake is now proportional to damage, so the bank no longer has that ceiling. The paragraphs are
> kept as history with their old-scale figures. **The unlock system itself stands**, with thresholds ×5.

*(historical)* **[found in play, 2026-09-07]** The bank cannot climb past **100 ml** against normal enemies in
steady state, and this is structural rather than a tuning miss: the spend is a **percentage** (10%
of the bank) while normal intake is a **flat** 10 ml, so they meet at exactly 100. The 5% floor
leak pulls the real figure lower. Observed floors 1–6: `0 → 19 → 19 → 27 → 38 → 48 → 46`.

*(historical)* *Milking Grip* halves the spend **and** doubles the intake, moving parity to `0.05 × B = 20`, i.e.
**400 ml** — clamped by §6.4's 25% excess leak to a settled **215–275 ml** across every play
pattern.

*(historical)* **The revealing figure is the spread, not the average.** Without Milking Grip the settled bank
swings from 177 down to 68 purely on how often you play your Ass cards — so *using the archetype is
what stops you having a bank*. With it, the settled value barely moves. Milking Grip is therefore
not "more Ass": it converts the bank from something you spend into something you hold, and that is
why it reads as mandatory rather than strong.

**[decided] Ass progresses by threshold unlocks**, giving each hole a distinct progression shape:

| Hole | Progression |
|---|---|
| **Pussy** | Pregnancies — spiky, event-driven |
| **Mouth** | Permanent slow accumulation (the Stomach) |
| **Ass** | **Rewards granted on reaching bank thresholds** |

| Threshold | Reward |
|---|---|
| **500 ml** | ***Milking Grip*** added to your deck free |
| **1000 ml** | The **Plug** relic |
| **1500 ml** | Open — a genuine stretch rung |

**Why this is the cheap fix.** The complaint was never that Milking Grip is too strong; it is that
a whole build depends on drafting one specific Uncommon. Granting it at a threshold converts a
draft dependency into a system reward without touching the card's numbers. *(Historical, old scale: the threshold and
the gate lined up exactly — 100 ml was the unaided ceiling, and Milking Grip was the thing that let
you pass it. [Claude, 2026-11-20] Written for the flat-intake rule; not re-argued for the new one.)*

**The Plug at 1000 ml resolves its own tension.** The Plug blocks Ass cards while worn, which reads
backwards as a reward for an Ass deck — but it is a toggle, and stopping leak between floors is
exactly what a deck at 1000 ml needs to push into inflation. Plugged between fights, unplugged to
spend. It is the inflation enabler, handed over at the point inflation becomes the goal.

**[decided] Unlocks track a high-water mark, not the live value.** The bank leaks every floor, so a
threshold touched at 525 ml would otherwise be lost by the next rest site. Reached once, kept for
the run. One extra saved int alongside the reservoirs. [Claude, as built 2026-11-20] Precum in the Ass (*Edging Form*)
never counts toward the mark.

**[decided] *Milking Grip* leaves the draft pool** once it is a threshold reward. If it stayed, an
early draft would skip the progression entirely and the thresholds would become decoration.

**Squeezing still gets no permanent track.** Three progression systems is too many, and the
ammunition loop gives it a within-run rhythm that Draining's Stomach and Breeding's Sigils do not
have.

### 6.2 Capacities and intake

**[decided]** Reservoirs are measured in **ml**, with different capacities per hole. **[Gorim, 2026-11-20]** Every
ml figure is five times what it was, "for now, and see where that gets us":

| Reservoir | Capacity | Leak / floor | Fills from |
|---|---|---|---|
| Mouth | **250 ml** | 0% | 250 HP of Mouth damage, less kill bonuses |
| Womb | **500 ml** per stage | 10% of the unlocked ml | 500 HP of Pussy damage, less kill bonuses |
| Ass | **1000 ml** | **Banded** — 5% of the first 1000, 25% of the excess | 1000 HP of Ass damage, less kill bonuses and what its own attacks spend |

**[Gorim, 2026-11-20] Intake comes from damage:**

| Source | Fill | Goes to |
|---|---|---|
| **Every Attack** | **1 ml per HP it actually removes.** Nothing for overkill; nothing for damage Block absorbed | The attack's hole |
| **A kill** | **25% of the enemy's Max HP**, rounded up | The finishing hole |
| **Minion** | **0 ml**, for hits and for the kill | — |

- **Leg-Locked** (§6.3a) claims both hits and the kill bonus for the Pussy. [Claude, as built]
- ***Hyperfertile*** doubles all Pussy cum; ***Milking Grip*** doubles the Ass **kill bonus** only. [Claude readings]
- **Sensitive does not touch intake** any more (§4).
- **Design note [Gorim]:** reduce the kill bonus proportionally in multiplayer if that is ever built.

[verified, 2026-11-20] In game: a 12-damage *Breed Me!* gained 12 ml; a 4-damage *Suckle* on an enemy at 1 HP gained
1 ml for the hit; killing a 39 Max HP enemy paid a 10 ml kill bonus.

*What this section said until 20 Nov 2026, replaced by [Gorim]:* a flat fill per enemy climax — normal 10 ml,
multi-enemy elite 15 each (**[decided]**, down from 20), single elite 40, boss 60 — multiplied by
`(1 + 0.5 × Sensitive)` and **rounded up** (**[decided]**). Encounter size, the 45-against-40 elite comparison and the
rounding rule went with it.

**[decided] Minions produce no intake.** Without this, any summoner is an intake fountain: minions are cheap to kill
and often respawn, so they would pay better per unit of effort than the enemy summoning them, and Breeding would
want to farm adds rather than kill elites. Implemented against `Creature.IsSecondaryEnemy` — the game's own
definition — rather than a hand-maintained list of monster ids. [Claude, 2026-11-20] Carried over to the new rule
for hits as well as kills; the old "does not count toward encounter size" half is moot.

**[clarified] Ass leak is banded, not a flat rate on the total.** At 1500 ml it is `5% × 1000 = 50`
plus `25% × 500 = 125`, so **175 ml** — not 375. The 25% never applies to the whole bank, only to what
sits above 1000.

The differing capacities do real work for free. Mouth cycles fast and is meant to be spent
repeatedly; Womb is the calibrated progression track; Ass is the largest and a genuine long-term bank.
[Claude, 2026-11-20] How long each takes to fill under 1:1 intake has not been measured.

*Historical, superseded by the x5 scale:* the earlier position was to keep 50/100/200 rather than 500/1000/2000, on
the grounds that the sense of excess is better bought from **inflation** than from the baseline — an Ass at 400 ml
reads as absurd precisely because 200 is normal — and that bigger numbers make mid-fight arithmetic harder, which
StS deliberately avoids. [Gorim, 2026-11-20] chose 250/500/1000 because intake now tracks damage; the arithmetic
concern was not re-argued.

### 6.2a Intake comes from damage; precum is the extra

*(Until 20 Nov 2026 this section was "Attacks and Skills do not generate intake — Powers may": the rule was drawn by
card type, because the problem was held to be repeatability rather than generation. [Gorim, 2026-11-20] replaced it;
register 44 is retired and 117 is the rule.)*

**[Gorim, 2026-11-20] Every Attack gains cum 1:1 with the HP it actually removes** (§6.2). Skills and Powers gain
nothing by themselves; they spend, read, or make **precum**.

**[decided] Precum is combat-only fuel.** It sits in the Mouth and is
indistinguishable from cum *inside* a combat — spendable by *Spit* and anything else that
empties the Mouth, counting against the 250 ml cap. At the end of combat it evaporates and does not
reach the Stomach. **It does heal**: every swallow heals 1 HP per 10 ml, cum and precum together
([Gorim, 2026-11-20]; 1 per 2 from 2026-09-08, register 111), so precum pays out on its way out.

Mouth cards can manufacture the fuel they need to function
without inflating Draining's permanent track, so the generation is bounded to the fight it happens
in. **Spending order is precum first**, so a precum card is never wasted and bankable cum survives
to the swallow.

**[Gorim, 2026-11-20] Precum may extend to other holes later; *Edging Form* is the first card to do it**: whenever an
Attack gains cum, it gains that much again as precum in the same hole. [Claude, as built] In the Ass and Womb precum
counts as part of the bank during the combat, is spent first, and is gone at the bell, before conception is checked.
It copies the per-hit gain only, not the kill bonus, and does nothing for Skin.

Each archetype still needs an **in-combat** identity that is not "fill my reservoir":

| Archetype | Position on the HP bar | In-combat game |
|---|---|---|
| **Draining** | Stay **high** | Payoffs above half HP. Weak, Block, healing, Sensitive removal. |
| **Squeezing** | Stay **low, without tipping over** | Payoffs below half HP. The bank is ammunition. |
| **Breeding** | **Climax repeatedly** | Payoffs for having Climaxed. Sensitive now raises Pussy damage, not intake. |

[Gorim, 2026-11-20] Agreed in principle and **not built**: defining the archetypes by events rather than bar position
(Draining = healing, Squeezing = losing HP to your own cards, Breeding = Climaxing). The table above is the older framing.

### 6.3 Reachability

> [Claude, 2026-11-20] Written for the flat-intake rule; not re-argued for the new one.
> Everything in this section is computed from the flat per-kill table and the Sensitive intake multiplier, against a
> 100 ml Womb. Under the built rule a stage is **500 ml**, filled by HP removed with Pussy-routed attacks plus 25% of
> Max HP per kill, and Sensitive adds nothing to it. Which encounters conceive from empty, and how many pregnancies a
> run holds, has **not** been re-derived or measured. The text below is kept as history.

Proportional leak means a reservoir drifts toward `gain ÷ leak`, but **pregnancy triggers on
a peak, not on the drift level**. A single encounter delivers:

Against a **100 ml** Womb, per kill, × `(1 + 0.5 × Sensitive)`, rounded up:

| Encounter | 0 stacks | 1 stack | 2 stacks |
|---|---|---|---|
| Single elite | 40 | 60 | 80 |
| Elite, 3 enemies | 45 | **69** | 90 |
| Boss | 60 | **90** | **120** |

**[revised 2026-09-06, and weaker than it was.]** At the old 20 ml multi rate a three-enemy elite
with one Sensitive stack delivered 90 and all but completed a pregnancy on its own. At 15 it
delivers **69**. The consequence is worth stating plainly: **no encounter now conceives from empty
except a boss at two Sensitive stacks.** Everything else requires a bank carried in.

That is arguably the better shape — pregnancy becomes the reward for routing discipline across a
whole act rather than for turning up at an elite — but it is a change, not a restatement.

**The "1–2 pregnancies per act, 4–6 Sigils per run" target was derived from the old rate and no
longer follows from anything.** It is now unverified. The instrumented run is what settles it, and
until then no downstream number that depends on Sigil count — the 2 / 4 / 6 thresholds above all —
should be treated as calibrated.

**Limit on what the log can show:** overflow above 100 ml is currently discarded, since pregnancy
is not implemented. Fill *rate* is measurable; the size of an overshoot is not.

### 6.3a Leg-Locked — the kill claim

**[decided]** A rules word, applied by cards:

> **Leg-Locked** — while an enemy is Leg-Locked, its death routes its output to your **Pussy**,
> whatever killed it. Counts down at the end of its turn.

[Claude, as built 2026-11-20] With intake now per hit, cum from **hits** on a Leg-Locked enemy goes to the Pussy as
well as its kill bonus.

It is a **claim staked on a kill that has not happened yet**, which is why it expires: you are
betting on finishing that enemy, not taxing it.

**This closes a hole §6.0 has carried from the start.** Routing keys off the finishing card's tag,
so a kill delivered by Poison, a Sigil passive, reflect damage or any untagged card falls through
to Skin (§6.4). Breeding needs Womb fill more than any other archetype and is also the deck most
likely to kill things with something that is not an attack — the Sigil passive is literally
start-of-turn AoE. Without Leg-Locked the archetype's own engine leaks its progression into Skin.

It **overrides both** the finishing-card tag and the Skin catch-all. It counts down like
Debilitate, so marking an enemy you then fail to kill is wasted — the card asks you to read the
fight rather than being a free tax on every turn.

**Watch:** it converts any kill into Womb fill for 1 energy on a card that also blocks. If a
Breeding deck simply opens every fight with it, it wants a real cost.

### 6.3b Pregnancy — staged gestation

**[decided]** Crossing **500 ml** in the Womb conceives (100 ml before the x5 scale). Until v3.1 that was all pregnancy was: a
counter increment with a delay before a Sigil arrived. **Nothing happened in between**, and nothing
distinguished a pregnancy carried one floor from one carried five — so the staging that carries most
of the breeding flavour, mild to heavy, had no mechanical existence at all.

> **Pregnant (Stage N)** — reached at **N × 500 ml** in the Womb. At the start of each combat gain
> **N Sensitive**; at the start of each of your turns shuffle **N Milk** cards into your draw pile.
> **Give birth** at any rest site: gain **N Sigils**, and the Womb empties.

**The Womb keeps filling while you carry.** Further intake does not conceive again — it advances
the stage. That is what makes gestation a *decision* rather than a wait: every fight you stay
pregnant is a fight you could have cashed out instead, and both the cost and the reward climb
together.

**[decided] The leak only touches the excess above the locked threshold.** A stage's `N × 500 ml`
is committed and cannot bleed away; the Womb's 10% per floor applies to what sits above it.

| State | Womb | Locked | Leaks 10% of |
|---|---|---|---|
| Not pregnant | 300 ml | 0 | 300 |
| Stage 1 | 700 ml | 500 | 200 |
| Stage 2 | 1250 ml | 1000 | 250 |
| Stage 3 | 1700 ml | 1500 | 200 |

**[decided] Birth empties the Womb completely, and the excess is wasted.** Delivering at Stage 3
with 1700 ml gives 3 Sigils and loses all 1700 — it goes on the floor, not to Skin, not to the
Stomach. **This is the one place in the design where output is destroyed rather than recycled**,
and it is deliberate.

**It is what makes delivery a real decision.** Because everything is lost, there are natural
delivery points at exactly `N × 500`: cashing out at 1499 ml wastes 499 for two Sigils, cashing out at 1500 wastes
nothing and pays three. So the question stops being a continuous "carry or deliver" and
becomes **"am I close enough to the next threshold to be worth the risk"** — which is a much
sharper thing to ask every rest site.

**[reading — confirm] Stages are uncapped.** Nothing stops Stage 4 at 2000 ml. That follows from
declining to resume the leak above 1500 on the grounds that pushing further should be *possible but
risky* — a cap would make pushing pointless rather than dangerous. If 3 was meant as full term with
the excess simply carried past the birth, that is a one-line change.

**[decided] Milk heals; it does not Block.** This is what makes the gamble real. Healing cannot
prevent a one-shot: at Stage 3 you are taking **+150%**, so even topping up to full every turn you
are steadily raising the chance that a single enemy intent kills you outright. The carried state is
not "safe but slow" — it is a rising probability of sudden death that no amount of sustain insures
against. Block would have made it merely grindy.

**[decided] Conception resolves at the end of combat, not the moment you cross.**

- You do **not** count as pregnant during the fight in which you cross, so no lactation begins
  mid-combat and the new Sensitive stack does not land on the fight that earned it.
- You **are** pregnant on leaving the room: the leak rule above applies immediately, and the next
  rest site can deliver.
- It gives conception a clean moment to hang an animation or image on, at a room boundary rather
  than buried inside a damage resolution.
- **Consequence, intended:** because the check reads the end-of-combat value, spending your Womb
  down below the threshold before the bell **prevents** conception. The universal spend order ends
  at the Womb, so that is a real out for a player who does not want to be carrying into the next
  stretch of map — agency, not an exploit.

> **Lactating** — shuffle **N Milk** cards into your draw pile at the start of each turn, where N is
> your stage. Begins with pregnancy and **continues for one combat after the birth**, at the stage
> you delivered at.

**Two states, not one.** *Pregnant* carries the Sensitive stack and the committed ml; *Lactating*
produces the Milk, and only the first ends at the birth. The fight after you deliver still produces
Milk without the fragility that paid for it — the payoff outliving its cost, which makes birth a
transition rather than a switch flip. Conceiving again during the tail simply re-enters Pregnant;
lactation never lapses.

> **Milk** — *0 cost. Attack.* Tease **4** pleasure. Soothe **2**. **Exhaust.**
> *1 Sigil:* draw **1**. *3:* Tease **7** instead. *5:* draw **2** instead. *7:* does not Exhaust.

**Milk shuffles into the combat draw pile, and is gone at the bell.** It never touches the master
deck, so there is no permanent dilution — and the draw pile is deliberately the weaker of the two
options: into hand it would be a guaranteed extra card every turn, while into the pile it competes
with your real cards for draws. At Stage 3 that is three cards a turn, so a long fight becomes
mostly Milk. That dilution is a cost, and it is another thing arguing for cashing out.

**Milk uses the standard Sigil ladder**, so it needs no new rules, and the 0-cost tier-4 case lands
somewhere unusually good: a 0-cost card sheds its **non-energy** cost at 7 Sigils, and Milk's
non-energy cost is **Exhaust**. So at 7 Sigils Milk stops exhausting and accumulates — producing
faster than you can spend, from a rule that already existed.

**Open:** whether "if pregnant" cards should exist as a separate axis. If they do, keep them off the
Sigil ladder entirely — **a card is tier-gated or pregnancy-gated, never both** — or Breeding gets
back the legibility problem the fixed shape was written to solve.

### 6.3c The rest site — Indulge and Relief

**[decided] Leak lands on your Skin.** Every reservoir's per-floor leak — Womb, Ass, Mouth —
routes to Skin rather than vanishing.

**[decided] Skin does not leak at all.** The 25%/floor rate is cut. Skin is a pure accumulator —
being gradually covered is the point, and it should be visible on the sprite. Nothing leaves Skin
except by Indulging.

**[decided] Leaked cum arrives at 75%.** Cum leaving the **Womb or Ass** loses a quarter on the way
to your Skin. Output that lands on Skin *deliberately* — Freehand kills, unrouted damage (§6.4) —
arrives whole; only accidental loss is taxed, or Freehand would be penalised for doing its job on
purpose.

**[decided] Indulge reclaims two thirds.** Scooping is messy and spills. You recover **⅔ of your
Skin**; the rest is wasted.

**Together these make routing matter, which it did not before.** A dedicated Mouth build previously
banked the same total however well it played, because anything it failed to route leaked to Skin
and came back at the next rest site — so skill was worth nothing on the permanent track. Now the
two multipliers compose to exactly **half**: `0.75 × ⅔ = 0.5`. Cum banked directly is worth
**double** cum leaked and reclaimed.

**Future hook:** a relic improving reclamation is the obvious upgrade path, and it has a clean
number to move.

**[decided] The Seductress has no `HEAL`.** Her rest site offers **Indulge** and **Relief**
alongside Smith, and they are three different things rather than a heal plus two alternatives.

> **Indulge** — Scoop the cum off your body and eat it. Empty your Skin: **Soothe 1 per 5 ml** and
> bank that ml into your **Stomach**.

**[Gorim, 2026-11-20] Indulge heals 1 HP per 5 ml recovered.** The 20 Nov change did not mention Indulge and the
build was left healing 1 HP per ml, which on the new scale made almost any Indulge a full heal. [Claude] flagged it
during the doc rewrite; [Gorim] chose 1 per 5 ml, the x5 scale applied, so Indulge heals what it did before and twice
what a swallow does. The ml recovered still banks to the Stomach in full. Built the same day; not yet tried in game.

> **Relief** — Masturbate furiously to appease your lusts until you pass out. Wake with **one
> fewer Lust card**.

**Relief is the actual rest-equivalent** — you sleep it off and recover run resource. **Indulge is
the odd one**: part Rest because it heals, part Smith because the Stomach banking is permanent. So
the site poses a real three-way question rather than "heal or don't".

**[decided] *Scoop It Up* is cut from the card pool entirely.** It was the only Skin→Mouth
transfer, and leak-to-Skin would have made it close to mandatory — a card nobody can skip is worse
than no card. Turning it into **Indulge** solves that and four other things at once:
the cum has somewhere to go, the rest site gains the character's flavour, Smith and the other
options become genuinely competitive when Skin is low, and the character is pushed toward
Climaxing, because Climax is now her most reliable heal.

**Why 1 HP per ml is not too generous.** Vanilla `HEAL` is 30% of Max HP, and a full heal here
looks far stronger by comparison — but **the bar is only a quarter of a life.** Resetting arousal
to zero restores roughly 25% of the character's *total* life resource, against vanilla's 30% of
all of it. Comparable, and the Stomach banking is what pays for the difference.

### The Womb's leak is bounded and cyclical; the Ass's is unbounded

**[corrected twice, v3.3 and v3.4]** The Womb does **not** leak less while carrying. The unlocked
excess sweeps 0→500 exactly as the pre-conception Womb does, so both phases produce the **same
0–50 ml sawtooth**, resetting each time a threshold locks. Earlier drafts claimed a filling/carrying
distinction; there is none, and the claim came from reading a single point (Stage 3 at 1700 ml, so
20 ml) as though it were the whole curve.

The real contrast is with the Ass:

| | Leak shape | Bound |
|---|---|---|
| **Womb** | 10% of unlocked ml — sawtooths 0→50, resets at each threshold | **Capped at 50 ml/floor, forever** |
| **Ass** | 5% of the bank, 25% of the excess above 1000 | **Unbounded** — grows as the bank grows |

That is what makes halving the Womb's leak a good relic: the cost it removes is **always present
and never outgrows relevance**, at every point in every cycle. The Plug is the opposite shape — its
value scales with how inflated you are, so it is near-worthless early and decisive late.

**Squeezing feeds Skin the most**, since the biggest bank leaks the most — a cross-archetype
consequence worth keeping rather than tuning away.

### Relief and Indulge are not close, and that is fine

**Removing a Lust card is worth a full heal, always.** Each Lust card is one of the ~3 bar-fulls the
run has left, so Relief hands back an entire bar you can afford to lose — **regardless of your
current HP**. Indulge is capped by what you are actually missing. So Relief dominates on the health
axis at every point, and **Indulge's only edge is the Stomach**.

That is a real choice — survival now against permanent progression — but only while the Stomach is
worth something. Right now it is not: the Mouth's permanent payoff is still unspecified. So
**Indulge has been nerfed twice (the 75% and the ⅔) on the strength of a track that does not yet
exist.** That is the correct fix for the routing problem, but it means: **do not tune Indulge down
any further until the Stomach payoff is built and can be weighed against it.**

Relief is also unavailable at zero Lust cards, so early on Indulge is the only option and the
comparison does not arise.

**[unknown] How much Skin actually accumulates is not derivable at a desk.** An earlier draft put
inflow at ~15 ml a floor (old scale) and concluded Indulge would be a guaranteed full heal from early act 1 —
that assumed reservoirs full enough to leak at those rates, which is not true for most of act 1
when everything is near-empty. The instrumented run supplies this number; nothing should be tuned
against a guess at it.

### Two arguments elsewhere in this document that leak-to-Skin weakens

Both were written when leaking was pure loss. Neither is fatal; both need revisiting.

- **The Plug (§7.3).** Its case is *"HP is recoverable and semen is not"*. Leaked Ass now becomes
  HP at the next rest site, so the trade is 4 HP a floor against ml that was going to return a few
  HP anyway. The Plug is still right for carrying an inflated spike to a chosen elite; the
  dry-stretch argument is much weaker.
- **Wet Patch (§7.3) is replaced by *Sticky Bandaid*** — see below.

### 6.5 Max HP 20, and the Stomach track

**[decided] Max HP is 20**, not 65.

**The bar is one life of four.** 20 × 4 climaxes ≈ **80 effective HP**, which is Ironclad's
starting pool. Within a single fight it is less, because Sensitive compounds: `20 + 13 + 10 + 8 ≈
51`. Both numbers are defensible starting points, and the second is the one that matters in a boss
fight.

**[corrected] Only Max HP multiplies.** +1 Max HP is +1 in each of four bars, so it is worth **4×**
vanilla. Nothing else scales that way:

| | Scales with bar size? | Why |
|---|---|---|
| **Max HP** | **Yes, 4×** | Refilled four times, so every point is spent four times |
| **Block** | **No** | 5 Block prevents 5 damage, here as anywhere. Spent once |
| **Healing, damage dealt** | **No** | Absolute, spent once |
| **HP costs** | **Yes, inversely** | 3 HP is a larger fraction of the way to a Climax, and a Climax costs a Lust card |

An earlier draft claimed Block was also worth 4× on the grounds that 5 Block is a quarter of a 20 HP
bar. That measures the bar, not the Block.

**[decided] HP costs rescale as though the bar were 30–40, not 20.** A clean ÷3.25 would make the
blood-magic axis timid, and this bar is meant to be swingy.

| Was | Now | Cards |
|---|---|---|
| 20 | **9** | Seek Relief *(upgrade: 4)* |
| 12 | **6** | Aching, Craving |
| 10 | **4** | Surrender |
| 8 | **3** | Stuffed |
| 6 | **3** | Rut |
| 4 | **2** | Clench, Deep Throat |
| 3 | **2** | Throat Fuck, Balls Deep |
| 2 | **1** | Grind, Claim |

The **Plug** rescales the same way: **2 HP** on entering a floor, **1 HP** per turn.

*Seek Relief* at 9 is **45%** of the bar, where 20 was 31% of 65 — deliberately harsher. **Known
cost:** the Deep Throat family's internal ordering collapses, since 4 and 3 both land on 2. Energy
cost still separates them.

**[corrected] The character is not more fragile, it is more granular.** A Jaw Worm hitting for 11
is 11 damage against **80 effective HP** — the same 14% it takes from Ironclad's 80. An earlier
draft called it "55% of the bar" and concluded enemies would be overwhelming; that compares against
one bar rather than the pool. The same total damage is absorbed, delivered in four chunks with a
Lust card owed at each boundary. What changes is the **rhythm**, not the durability.

### The Stomach — Draining's permanent track

**[decided] +1 Max HP per 50 ml banked** (10 ml before the x5 scale). It should be enormous;
it is the archetype's entire progression. *The earlier estimate — roughly 20–25 Max HP per act on a dedicated build,
so +80–100 effective HP per act — was made for flat per-kill intake.* [Claude, 2026-11-20] Written for the flat-intake rule; not re-argued for the new one. [verified,
2026-11-20] Banking 50 ml and then 100 ml gave +1 and +2 Max HP.

| Milestone | Reward |
|---|---|
| every **50 ml** | **+1 Max HP** |
| every **×250** (250, 750, 1250, 1750 …) | **+25 ml precum** at combat start, cumulative |
| **500 ml** — *Gulp* | **The Mouth also swallows whenever it is full** (250 ml) |
| **1000 ml** | **Indulge stops spilling** — 100% reclamation |
| **1500 ml** | **Precum counts as cum** — it banks instead of evaporating |

**[clarified] The Mouth banks to the Stomach at the end of every combat, by default.**
**[Gorim, 2026-09-08] The *Gulp* card is cut** (register 112): the name belongs to the 500 ml milestone
above, and a swallow of more than 125 ml removes 1 Sensitive once that milestone is reached. **[Gorim, 2026-11-19]**
**Gulp** is also a card keyword — swallow everything in your Mouth now — carried by *Mouthful* (register 124). The card
never enabled banking — its "swallow **now**" bought the *timing*, the heal and the Sensitive
removal, and the heal now happens on every swallow anyway. This was only ever implied, by §6.2a saying precum is the thing that evaporates, and the
omission has already caused one misreading.

**500 ml is additive, not a trade.** The full-Mouth swallow happens **in addition to** the end-of-combat
one, which is unchanged. It **uncaps throughput** — fill, bank, fill again, so a long fight can bank
500–750 ml where the Mouth's 250 ml capacity previously capped it — and nothing is taken away. The
track is a reward; it should not introduce a way to lose cum you had already collected.

*(An earlier draft made the full-Mouth swallow replace the end-of-combat bank, so a fight ending with a
partial Mouth banked nothing. That is a trade rather than an upgrade, and it implied carrying cum
between fights, which does not fit.)*

**1500 ml compounds with the ×250 rung.** The ×250 milestones are 250, 750, 1250, 1750 (the old ones *ending* in
50, times five) — so at 1750 ml Stomach you have passed four and open each combat with **100 ml** of precum,
which at 1500+ banks as real cum. [Claude, recomputed] That is **+2 Max HP a fight**, or roughly +20 across an act, from
the milestone alone — the same as before the rescale.

The ×250s are the generic rung and the ×500s are the unique ones. Precum at combat start ties the
permanent track back into the Mouth's in-combat economy, which is the half of Draining that
otherwise never touches the other.

**1000 ml removing the spillage is the track improving its own input** — your body gets better at
this — and it uses a number introduced one version earlier rather than inventing one.

**[decided] Max HP from relics is halved**, carried on the character's starting relic. Food relics
give 4× value here, so Strawberry / Pear / Mango at full strength would outclass every other pick
in the game. At 50% they are still strong and still worth taking.

**[Gorim, 2026-11-23] Max HP loss is halved too:** "Max HP gain is reduced currently, but Max HP loss is not. We should fix that." [Claude] Same rule both ways: half, rounded up, so a loss of 1 stays 1. It covers every vanilla source (events, relics, Brightest Flame, Paper Cuts, the Night Terrors and Terminal modifiers). **[Gorim, 2026-11-23]** "Can we fix shown numbers generically": event, relic, card and potion text now shows the halved Max HP number for her through one display hook (`MaxHpTextPatch`). Still printed in full: Cook, the Blitz merchant, Night Terrors, Terminal, Paper Cuts, Drowning Beacon, Sere Talon, and Tablet of Truth (kept full on purpose: its lethal check uses the full number).

### 6.4 Skin — the catch-all

**[decided]** Output with no finishing card — thorns, a Sigil passive, a damage-over-time
effect — does not vanish and is not split three ways. It lands on your **Skin**.

Splitting was thematic nonsense; a "last tag played" memory would be gameable by playing one
tagged card and letting something else finish. Skin is exact, closes the hole, and seeds
the fourth archetype.

**Traffic warning.** With poison likely out of the character's kit, unrouted kills are rare —
thorns, Sigil passives and cross-mod effects only. Skin therefore **cannot be fed mainly by
accidents**, and its two real sources are:

- **Squeezing overfill draining down** (§6.1) — the deliberate, recurring source.
- **Cards that glaze on purpose** — which is what the v2 archetype has to be built from.

**[decided] Freehand gives Skin a real source.** Attacks that use no hole — hands, breasts,
thighs — are tagged **Skin** and hit around 40% harder than their tagged equivalents, because
they collect nothing. This makes *every* attack in the pool tagged one of four ways: no
"untagged" special case, no accidental routing, and Skin stops depending on incidental kills
now that poison is out.

It also creates the interaction that makes *Continuing* bite. A Freehand attack sets your
Continuing mode to Skin, so Pumps played after it route to Skin until you re-establish a
hole. Playing the big off-brand attack costs you your routing, which turns ordering within a
turn into a decision.

**v1 scope: still minimal.** A stacking counter, leaks 25%/floor, small passive while held, spent
by *Wet Work*, *Cheek Full* and *Overflow*. Five Freehand cards, but no dedicated payoff
archetype yet.

**v2:** the fourth archetype — a build about deliberately covering yourself, fed by Squeezing
overflow and its own cards rather than waiting for incidental kills.

---

## 7. Sigils

**[decided]** Sigils replace orbs, and deliberately do not work like Defect orbs. No slots,
no cap, no channelling, no evoking as a core loop.

- Earned one per pregnancy, at the next rest site as a free action.
- **You keep every Sigil you make**, permanently, for the rest of the run.
- Each gives a **small passive** benefit, always on.
- Some cards can **exhaust a Sigil for the combat** for a major effect — you lose its passive
  until the fight ends. That is the interesting decision, and it gives Sigils an active
  dimension without turning the character into an orb-management puzzle.
- **v1 has one Sigil type.** Varieties are a later expansion.

### 7.0 The Sigil passive

> **Sigil** — at the start of your turn, deal **1 damage to ALL enemies**.

Chosen over "1 damage to a random enemy": random creates variance without a decision, and it
overlaps Defect's Lightning. Hitting everything is cleaner, and it makes "Sigils strike twice"
or "Sigils also apply Entranced" obvious build-around powers later.

It also feeds the system that produced it: passive AoE is exactly what helps you close out several enemies at
once. *(Historical: under the flat table the multi-enemy elite was the best Conception source — 15 ml each, ×1.5
with one Sensitive stack = 69 ml from three.)* [Claude, 2026-11-20] Note the passive is not an Attack, so the HP it
removes gains no cum; a kill it lands still pays the kill bonus — as built, to whichever hole the *Continuing* mode currently names
(not Skin, which §6.4 would suggest), unless the enemy is Leg-Locked.

**[resolved]** The earlier worry that this hollows out a Breeding deck's attack cards does not
hold. Five Sigils is a late act 2 / act 3 state, and by then cutting attacks *is* the correct
deck shape for a scaling build. Nor can you cut them entirely: passive AoE clears crowds but
leaves you chipping a single tanky enemy at 5 a turn, so single-target damage stays load-bearing
exactly where the passive is weakest.

**Terminology note:** the passive should read as plain **damage**, not "inflict 1 Lust". Enemy
arousal *is* their HP, so damage is already the right word, and "Lust" is spoken for by the
curse cards. Keeping the keyword count low is the StS house style — the fiction lives in the
visuals and voice lines, not in a third overloaded noun.

### 7.1 Cards gate on Sigils, they do not multiply by them

**[decided]** Sigil-aware cards read a base effect plus **threshold tiers**, not a per-Sigil
multiplier:

**[decided] Thresholds are 1 / 3 / 5 / 7.** Not 2 / 4 / 6, and not the 3 / 6 this section carried
through v2.1.

**The first Sigil must be a spike.** At 2 / 4 / 6 your *first pregnancy* — the achievement the
entire character is built around — changed nothing on any card. Earning the signature mechanic and
receiving a blank reward is the worst payoff curve available. At 1 / 3 / 5 the first Sigil turns on
tier 1 across your whole Breeding deck at once.

### The tier shape

**[decided]** Every Sigil-aware card follows the same ladder, so "what does another Sigil get me"
is answerable without reading four cards:

| Tier | Sigils | What it does |
|---|---|---|
| 1 | **1** | **Adds an effect** — a rider the base card did not have |
| 2 | **3** | **Stronger base** — the card's main number grows |
| 3 | **5** | **Stronger effect** — the tier-1 rider grows |
| 4 | **7** | **Repeats**, a number of times equal to its cost |

A shared resource scale should carry a shared meaning. Before this, each Breeding card's tiers did
something structurally unrelated — draw, damage, extra hits, Block, reflect — so a Breeding deck
asked you to hold a dozen unconnected conditional clauses and a tier could be misread as raising
damage when it did not.

**Tier 4 applies to Powers too.** Repeating a Power is enormous; at 7 Sigils you *should* be
wildly powerful, and a tier that rare has no reason to be modest.

**Powers can repeat, and repeating is the default.** `DuplicationPower` is vanilla and generic —
`ModifyCardPlayCount` returns `playCount + 1` for any card the owner plays, Powers included — so a
repeated Power simply applies its buff twice. Tier 4 should ride that hook rather than being
hand-rolled per card.

**The real exception is a non-stacking effect**, Barricade-style, where applying it twice changes
nothing. **Nothing in this pool is non-stacking**, so nothing here needs an exception on those
grounds.

*Ritual Mark*'s `+1 per Sigil` and *Rapture*'s doubled energy are therefore **design choices, not
workarounds** — each was written because it is more interesting than doubling, not because
doubling was impossible. A future Power can take the plain repeat.

**Why the repeat count is the cost.** Expensive cards need *superlinear* value to justify their
slot, so the payoff scales with what you paid. That makes tier 4 self-correcting: a card that
reduced its own cost at tier 2 has already banked that value in energy and correctly repeats fewer
times. The tier pays out against the card's **current** cost, so the ladder stays comparable across
the whole group regardless of what the lower tiers did.

**[decided] 0-cost cards lose their non-energy cost at tier 4** instead of repeating, since
repeating zero times is nothing. *Consummate* stops exhausting a Sigil; *Surrender* stops costing
HP. One rule, so any future 0-cost Pussy card already knows what its tier 4 is.

**Tier 4 is provisional and the others are not.** The 4–6 Sigils per run target is currently
unverified (see §6.3), so whether 7 is a stretch goal or unreachable is unknown. 1 / 3 / 5 is safe
either way; the fourth tier should not be balanced against until the instrumented run reports a
real pregnancy rate.

Every card is still playable at zero Sigils, so none is a dead draw in a non-Breeding deck.

This also removes the double-multiplier problem without capping anything: passives scale
linearly with Sigil count, card effects step, and the two do not compound.

### 7.2 On the late game

A dominant act 3 is the **correct** shape here, not a balance failure. Breeding is the
high-risk scaling build — it pays for its ceiling in acts 1 and 2, where it is drafting cards
that do nothing yet and taking Lust cards to reach the Sensitive multipliers that convert
elites *(Sensitive now multiplies Pussy damage rather than intake — §0)*. Silent's poison, Defect's focus and Ironclad's strength all run the same curve: rough
early, silly late. That is the deal the player signed.

The risk is genuinely priced, too — it is denominated in Lust cards, which is the one currency
that can actually end the run. A Breeding player is buying act 3 with proximity to death.

The only residual worry is *trivialising* rather than *dominating*: if fights end on turn two,
the arousal system and the voice lines never get to breathe. With 4–6 Sigils and stepped tiers
that is unlikely, but it is the thing to watch in playtest rather than power level as such.

---

## 7.3 The Plug — common relic

**[decided]**

> **Plug** — *Common. Toggle.*
> **While worn:** your Ass does not leak below 1000 ml, and leaks only **5%** above it. Ass cannot
> be filled. Lose **2 HP** on entering each floor and **1 HP** at the start of each turn.
> **Ass-tagged cards cost 1 more and remove the Plug when played**, spilling **10%** of your Ass
> **in addition to their own drain — and gaining Strength for all of it.**

**[decided] The plug removes itself.** Playing an Ass card while plugged costs 1 extra energy,
pulls the plug, and spills the 10%. No manual toggle needed in combat, and it is the only version
that makes physical sense — you cannot use that hole without taking the thing out of it first.
The +1 energy *is* the old removal cost, folded into the card that triggers it, which removes a
whole clause from the relic and a whole click from the turn.

### What it is actually for

The 2 HP per floor prices the item precisely, and the arithmetic says something specific:

Unplugged, Ass leaks 5% of the first 1000 ml plus 25% of everything above it. Plugged, the first
1000 ml does not leak at all and the excess leaks 5%. [Claude, 2026-11-20] The table and the dry-stretch example are
recomputed ×5; the HP figures in them are as originally written.

| Holding | Leaks unplugged | Leaks plugged | Prevented | Cost | Worth it? |
|---|---|---|---|---|---|
| 1000 ml (full, not inflated) | 50 ml | 0 ml | **50 ml** | 4 HP | Marginal — 50 ml is a few hits' worth |
| 1500 ml (inflated) | 50 + 125 = 175 ml | 25 ml | **150 ml** | 4 HP | Clearly yes |
| 2000 ml (heavily inflated) | 50 + 250 = 300 ml | 50 ml | **250 ml** | 4 HP | Emphatically yes |

**Correction to the "marginal" verdict above.** That table compares one floor's leak against one
floor's HP, and that framing is wrong, because the two costs are not the same kind of thing:
**the HP is recoverable and the semen is not.** Over a dry stretch — `? → chest → ? → campfire`,
no combat to refill — a 1000 ml bank unplugged decays 1000 → 950 → 903 → 858 → 816, losing ~184 ml.
Plugged it costs 16 HP and loses nothing, and the campfire at the end gives the HP back. Trading
16 refundable HP for 184 unrecoverable ml is a good deal, not a marginal one, and this character's
HP is deliberately volatile enough to spend that way.

So the Plug has **two** uses, not one: carrying an inflated spike to a chosen elite, and simply
surviving a combat-free stretch of map with your bank intact. The second is the more common play
and I under-rated it.

### Why the tax beats the lockout

The earlier "Ass-tagged cards are unplayable" version made this a build-hostile common — a
Squeezing player reads "your archetype's cards don't work" before they read anything good, and
StS commons are normally simple and straightforwardly positive. Taxing and auto-removing keeps
the intent without bricking the hand: the plug is still incompatible with anal, you just resolve
that incompatibility by taking it out, which is what would happen.

It also makes plugged combat a real option rather than a dead one. You can fight with it in,
paying 2 HP a turn, and pull it the moment you want an Ass card — so the per-turn drip now does
actual work instead of being near-dead text.

The **10% spill on removal** is the anti-spam clause and it does its job: at an inflated 2000 ml
that is 200 ml gone, so flipping it casually is expensive. Removal cost scales with how much you
have to lose, which is the right shape.

### [decided 2026-09-07] The spill feeds the Ass rule

**The spilled 10% counts as ml lost by the card that caused it**, so it grants Strength like any
other drain.

Without this the relic is incoherent. A plugged Ass card would pay **+1 energy and double the
drain for exactly the same damage** — a downside applied twice, on the archetype whose own cards
trigger it. That reads as a punishment for playing your deck, which is the same failure the
"Ass-tagged cards are unplayable" version had, arriving by a quieter route.

With it, pulling the plug is **the archetype's burst moment**: one card, double the drain, double
the Strength. [Claude, recomputed 2026-11-20] At 1500 ml, on the +1-per-3-ml ratio:

| | Drain | Strength | *Blowout* (4 hits of 6) |
|---|---|---|---|
| Unplugged | 150 ml (10%) | +50 | 4 × 56 = 224 |
| Unplugged, *Blowout* | 750 ml (50%) | +250 | 4 × 256 = 1024 |
| **Plugged** | 300 ml (20%) | **+100** | 4 × 106 = 424 |
| **Plugged, *Blowout*** | 900 ml (60%) | **+300** | 4 × 306 = **1224** |

That is what inflating the bank was *for*, and it reads correctly in fiction as well: you have been
holding it in, and it comes out all at once.

It also settles the +1 energy. As a pure tax it was a cost you resented; as the price of a
double-strength hit it is a cost you plan around, which does more for the "build-hostile common"
problem than the auto-removal did on its own.

### Sticky Bandaid — uncommon relic

> **Sticky Bandaid** — *Uncommon.* Your **Womb** leaks **half** as much. *You manage to reapply it
> after every fight, keeping the cum inside.*

**[decided] Replaces *Wet Patch***, which paid 2 HP whenever any reservoir leaked. That relic
became incoherent once leak routed to Skin — it was paying you for a **transfer** rather than
compensating a loss — and it was never the Pussy relic it was described as, since it triggered on
everything.

**This is a Pussy-build relic and it works at every point in the cycle.** The Womb's leak is a
bounded 0–50 ml sawtooth that resets at each threshold and never grows (§6.3c), so halving it is a
flat, permanent acceleration of the run's progression track: every threshold arrives sooner, so
pregnancies do, so Sigils do, so every gated card in Breeding turns on earlier.

Drawn outside a Pussy build it is a weak pick rather than a dead one, which is the normal shape for
an archetype relic and is further softened by RelicChoice's pick-one-of-two.

2 HP per leaking reservoir per floor, so 4 with Womb and Ass both filled, 6 with Skin on top.
It rewards the two builds that actually hold liquid between fights, and it converts the design's
main attrition — leakage, which is otherwise pure loss — into slow sustain.

**It is the Plug's counterpart rather than its opposite.** The Plug protects the Ass and costs HP
per floor; Sticky Bandaid protects the Womb and costs nothing. One is an active toggle bought with
attrition, the other a passive that quietly raises the ceiling on how fast you conceive — the two
archetypes' banks defended in the two ways those archetypes actually play.

### Womb cover

Deferred. Conception is the run's actual progression track, so an item that protects it is
strictly stronger and should not be a common. Specced with the fourth archetype.

---

## 8. Card pool — 40 draftable cards as built

**[Claude, 2026-11-14]** The table below was the v1 plan (35 cards, nine of them Core) and no longer
matched anything. It now shows what is built; `SEDUCTRESS_CARDS.md` v26 is the card-by-card record and
lists the gaps in its §14.

Starting deck (11): 4 × More, 4 × Guard, Seek Relief, Breed Me!, Suckle.
Starting relic: reduce arousal by 6 after each combat, and halve Max HP from relics (§6.5).
**The relic is not built** — the character still starts with Ironclad's *Burning Blood*. The halving itself is built, as a patch on Max HP gain.

| Group | Count | Role |
|---|---|---|
| **Core** | 8 | *Rehydrate* (Eat), neutral Block, Weak, two *Continuing* attacks, Climax on demand, the Climax engine, and *Hold the Edge* (a Climax refused and thrown back). |
| **Freehand** (Skin) | 5 | No-hole attacks that do structural things instead of collecting; one generic spender. |
| **Draining** (Mouth) | 11 | The Deep Throat family, precum generation, *Milking Throat*, *Spit*, *Mouthful* (Block from a full Mouth, then **Gulp**), one above-half-HP payoff. |
| **Squeezing** (Ass) | 7 | Multi-hit attacks that spend the bank for Strength. *Milking Grip* is the 500 ml reward, not draftable. |
| **Breeding** (Pussy) | 9 | *Hyperfertile*, Sigil-tiered cards on the 1 / 3 / 5 / 7 ladder, Leg-Locked, Climax payoffs. |
| **Ancient** | 1 | *Edging Form* — attacks gain their cum again as precum (reworked 2026-11-20). |
| **Generated** | 1 | *Milk*, into the combat pile while Lactating. |
| **Lust** | 2 | The curse cards (not draftable). |

---

## 9. Voice hooks

Own triggers on `BaseLib.Audio` (`AutoModAudio.PlaySfx`).

Card played (chance + cooldown) · arousal thresholds 25/50/75/90% · self climax · enemy
climax **per reservoir** (four variants incl. Skin) · Sensitive gained · combat start ·
victory · rest-site relief · birth.

≈13 categories × 5 variants ≈ **65 lines** for v1. Card Play Voices scaled a small dataset
with GPT-SoVITS; same route for per-card lines later, and its `voice_manifest.json` schema
(profile / intensity / pace + delivery prompt) is a good direction sheet.

---

## 9a. The lexicon — a presentation converter

**[decided]** The design keeps saying **damage**, **Block** and **HP**. The *game* says something
else. A converter rewrites text at display time when the local player is the Seductress; nothing
mechanical changes, and every number, hook and calculation stays vanilla.

**Design documents and code keep the vanilla words.** This section is the only place the lewd
vocabulary is normative. That is deliberate — every previous attempt to carry both registers
through the design produced cards where "Deal 6 damage" sat beside "gain 20 arousal" and nobody
could tell which were rules terms.

### The substitution table

**[decided] Substitution only, syntax untouched.** This is the only form that can safely rewrite
vanilla relics, potions, enemy intents and other characters' cards, because it never has to parse
a sentence.

**[decided] Rules are applied in order, longest phrase first.** This is load-bearing, not tidiness:
a bare `damage → pleasure` rule running first would eat `damage to` and produce *"pleasure to ALL
enemies"*, and a bare `HP` rule would turn `Max HP` into `Max Arousal` only by accident and
`Lose 20 HP` into the wrong thing entirely.

| # | Vanilla | Seductress |
|---|---|---|
| 1 | Deal | **Tease** |
| 2 | Deals | **Teases** |
| 3 | **damage to** | **pleasure from** |
| 4 | damage taken | **pleasure experienced** |
| 5 | take / takes / taking damage | **experience / experiences / experiencing pleasure** |
| 6 | damage | **pleasure** |
| 7 | Max HP | **Max Arousal** |
| 8 | Lose *N* HP | **Gain *N* Arousal** |
| 9 | Heal *N* / Gain *N* HP | **Soothe *N*** |
| 10 | HP | **Arousal** |
| 11 | Block | **Poise** |

So *"Deal 9 damage to ALL enemies"* → *"Tease 9 pleasure from ALL enemies"*, and *"Deal 6 damage"*
→ *"Tease 6 pleasure"*.

**[decided] Extraction, not transfer — and that is why rule 3 exists.** Every verb that fits
*"VERB N pleasure **to** X"* is a transfer verb, and transfer is commerce: give, provide, serve,
grant and gift all read like a waiter, structurally rather than by accident. Flipping the
preposition moves the phrase into the extraction class, which is where the sensual verbs live, and
it fixes the fiction at the same time — you are not handing an enemy a gift, you are drawing it out
of them.

**It survives contact with vanilla text better than it looks.** *"Vulnerable: take 50% more
damage"* becomes *"take 50% more pleasure"* — parseable, and more correct for this character than
the original. Thorns, reflect and retaliate all read naturally.

**Rules 4 and 5 exist because "take pleasure" is idiomatic English for enjoying something**, so
*"Whenever you take pleasure, gain 1 Strength"* reads as a mood rather than a trigger.
**Experience** removes the ambiguity without losing the register.

**No false positive found for rule 3.** `damage to` was expected to misfire on constructions like
*"reduce damage taken to 0"* — it does not, because `taken` sits between the tokens and rule 3
never matches. *"Deal damage to yourself"* → *"Tease pleasure from yourself"* reads fine, and
*"deals damage to you"* → *"Teases pleasure from you"* is correct for this character rather than
merely tolerable. The residual risk is not vanilla text at all; it is **third-party mod prose**
using these words in ordinary English.

**Every rule needs all its inflections.** Rule 5 is three forms of one swap, and `Deal` needs
`Deals`/`Dealt`/`Dealing` alongside it. A missing form produces a half-converted sentence, which
reads worse than no conversion — so the table is really a list of inflected forms, and it should be
built from the actual strings in the decompile rather than from guesses about which forms exist.

### Later: the verb scales with the number

**[proposed, not for v1]** A 6-damage jab and a 45-damage *Blowout* want different verbs, and the
game already does exactly this — enemy intents pick an icon by damage band. The converter can
capture the number out of `Deal {N} damage` and choose:

| Band | Verb |
|---|---|
| 1–7 | Tease |
| 8–15 | Coax |
| 16–29 | Draw out |
| 30+ | **Wring** |

This is the most fun the lexicon can be, and it costs the single-rule simplicity, so it is a later
pass. **Fallback:** *"Deal damage equal to…"* has no literal number to capture and must keep the
default verb, so the ladder needs a defined base case rather than assuming one is always present.

### HP inverts, and that part is phrase-level

**[decided]** HP is the one axis where a token swap is not enough, because the bar runs backwards:
losing HP is **gaining** arousal. Rules 7–10 above are therefore phrase pairs, and *"Whenever you
lose HP"* becomes *"Whenever you gain Arousal"*.

**The known-hard class is comparisons**, and they will read backwards if fed through the same
machinery. *"If you end combat with less than 50% HP"* must become *"more than 50% Arousal"* — the
operator flips, which no substitution table can do. These need a hand-written allow-list, and
anything not on it should be **left in vanilla wording rather than converted wrong**. A relic that
reads backwards is worse than one that says HP.

### Where it hooks, and the risk

Card descriptions, keyword tooltips, hover tips, relic and potion text, and **enemy intents** —
intents are where damage-language is most visible, since the number over an enemy's head is the
thing you read every turn.

**Gate it on the local player being the Seductress**, exactly as the health bar patch is, so no
other character or mod is touched.

**Risk:** the converter rewrites text it did not author, including text from the 40+ workshop mods
loaded here. Substitution is blind, so a mod using "Block" as an English verb will produce
nonsense. Acceptable for a personal-use mod, and the failure mode is cosmetic — but it argues for
keeping the table **small and specific** rather than comprehensive.

---

## 10. Implementation risks

1. ~~**Death interception must catch every HP-loss path.**~~ **Retired.** `CustomSingletonModel`
   receives the same `ShouldDie` / `AfterPreventingDeath` hook Lizard Tail and Fairy in a Bottle
   use — no patching. Interception happens at the death hook rather than in damage bookkeeping, so
   anything routing through `CreatureCmd.Kill` is covered, including events, curses, Poison and
   Doom. Enemy damage, console `die` and the character's own self-damage all confirmed, and the
   run still ends at the Lust limit. **Residual:** `Kill(force: true)` skips the hook by design;
   nothing in vanilla calls it on the player outside a real game over, but that is a claim about
   vanilla and this install runs 40+ workshop mods. First place to look if a death ever escapes.
   **Also:** a preventer that does not restore HP hangs the combat —
   `KillWithoutCheckingWinCondition` re-enters itself and throws after ten attempts. Healing to
   full is load-bearing, which is also what §4 wants.
2. ~~**Inverted bar needs a Harmony patch** on `NHealthBar`.~~ **Retired.** Three postfixes on
   `RefreshForeground`, `RefreshText` and `RefreshMiddleground`; substituting `MaxHp - CurrentHp`
   is the whole change. `HealthBarForecastRegistry` was confirmed unusable as predicted. Enemies
   invert too. Gated on the local player being the Seductress, so nothing else is touched.
   **Residual:** the vanilla trailing-damage middleground must be hidden from its *own* postfix —
   `RefreshValues` calls it after the foreground and it sets `Visible = true` unconditionally.
2a. **Poison and Doom bar overlays are still positioned from `CurrentHp`**, so they sit in the
   wrong place on an inverted bar. Nearer-term than it sounds: enemies apply Poison to you whether
   or not this character's own deck has any. Same fix method as risk 2.
3. **Enemy "climax and pass out" must not break on-death triggers.**
4. **Lust cards must not be launderable** by exhaust, purge or transform.
5. **Sigils are uncapped permanents** — their passives must not stack into something degenerate.
   The persistence half is **retired**: `ExtendedSaveTypes.RegisterSavedValue` rides the ordinary
   run save, proven by reading `current_run.save` after the process exited and then resuming.
6. HUD legibility — deferred to playtest by decision.
7. Multiplayer out of scope; the rule still stands for the other mods here.

---

## 11. Art status

PoC (`SpriteCharTest`) validated in-game: a Godot `AnimatedSprite2D` scene works as a
character body, no Spine rig needed. Currently the 64×63 Purple Forest gameplay sprite at
×5.5 (~347px against vanilla's ~320px). Resolution is the known unsolved problem; accepted
for now.

---

## 12. Decision register

**Attribution.** **[Gorim]** = his decision. **[Claude-Cowork]** = this design session's, where he
said so explicitly. Entries before v3.x are unattributed — most were his, but the record was not
kept at the time and guessing now would be worse than leaving it blank.

### Locked

**[Claude, 2026-11-20]** Rows are kept as decided at the time, and **ml figures in rows 1–116 are on the old scale**
(multiply by five). Rows **117–126** are the 20 Nov changes; they replace or amend rows 8, 11, 35, 37, 44, 46, 66, 76,
77, 107, 108, 111, 112 and 116, which are left as written.

| # | Decision |
|---|---|
| 1 | Core fantasy: **seductress** |
| 2 | Enemy HP = their arousal; emptying it makes them climax and pass out, ending the fight |
| 3 | Your HP = your arousal, literally the HP variable. Bar drawn inverted |
| 4 | Arousal persists between fights; relic reduces after combat; rest sites reduce via vanilla heal |
| 5 | Your strongest cards cost arousal |
| 6 | Climax replaces death: reset to 0, +1 Sensitive, +1 Lust, turn ends |
| 7 | **First climax of a run is not free** — always costs a Lust card |
| 8 | Sensitive = +50% per stack, to arousal taken and intake gained; clears at end of combat |
| 9 | Lust cards are playable; 5 in deck ends the run; removable at shops |
| 10 | Intake routed by the finishing card's tag: Mouth / Ass / Womb |
| 11 | Fill: normal 10%, multi-elite 20% each, single elite 40%, boss 60%, × (1 + 0.5 × Sensitive) |
| 12 | Leak per floor, proportional: Womb 10% / Ass 5% / Mouth 0% / **Skin 0% (cut, v3.4)** |
| 13 | Womb 100% ⇒ pregnancy ⇒ Sigil at next rest site, free action |
| 14 | **Sigils**: uncapped, permanent, small always-on passive; some cards exhaust one for the combat. One type in v1 |
| 15 | Sigil-aware cards **gate on thresholds (3 / 6)**, never multiply per Sigil |
| 16 | A dominant act 3 for Breeding is intended, not a balance problem — the risk is priced in Lust cards |
| 17 | Target 4–6 Sigils per run, 1–2 per act |
| 18 | **Draining** banks toward permanent Max HP |
| 19 | **Squeezing** gets no permanent track — persistence via 5% leak plus **inflation** (overfill past 100%, excess leaks 25%/floor) |
| 20 | **Skin** catches unrouted output, and is fed deliberately by Squeezing overfill draining down. Minimal in v1, fourth archetype in v2 |
| 21 | Multiplayer out of scope for this mod only |
| 22 | v1 = 35 cards |
| 23 | Voice: own implementation on `BaseLib.Audio` |
| 24 | **No custom debuff keyword** — vanilla Weak and Vulnerable only; Entranced dropped |
| 25 | HUD layout deferred to playtest |
| 26 | Poison is likely **not** in the character's kit — so unrouted kills are rare |
| 27 | **Sigil passive**: at the start of your turn, deal 1 damage to ALL enemies |
| 28 | Plugs **block filling as well as leaking**, and are toggled on the map between rooms |
| 29 | **Plug** is a **common** toggle relic: no leak below 200 ml / 5% above, Ass cannot fill, 4 HP per floor + 2 HP per turn. **Ass-tagged cards cost 1 more and remove it when played**; removal spills 10%. Used both for carrying an inflated spike and for surviving a combat-free stretch with the bank intact. Womb cover deferred |
| 31 | Basic attack is **Pump**, tagged ***Continuing*** — routes to the last hole used this combat, Skin if none. Resets each combat. No fixed tag on the starter, so no archetype bias from the character-select screen |
| 32 | **Freehand** attacks (hand / breasts / thighs) are tagged *Skin*, hit ~40% harder, and flip the *Continuing* mode to Skin. Every attack is now tagged one of four ways |
| 33 | **Mouth cashes out at end of combat** — heal half the ml, bank the full amount to Total Swallowed. 150 ml swallowed ⇒ +5 permanent max HP |
| 34 | **Pre-cum**: Mouth alone may gain ml from cards without a kill (~5 ml), as a bounded exception to §6.2a. Preserving defeated monsters rejected |
| 35 | **Edging Form** (1, Power, **Ancient**): whenever you play an Attack, gain 1 ml in the hole it used. A deliberate exception to §6.2a, safe because it comes from a single Ancient source |
| 36 | Pool size: **32 is a floor, not a ceiling** — cards may be added without cutting others |
| 37 | Sensitive multipliers on intake **round up** |
| 38 | Squeezing **reads its bank rather than spending it** — *Tight Fit* and *Ruinous* cut; *Overflow* spends a proportion, not a flat amount |
| 39 | Sensitive removal sits on *Gulp*, gated behind swallowing 25 ml — **[superseded by 112: *Gulp* is now a Stomach milestone, same 25 ml gate]** |
| 40 | **Holes ≠ containers.** Tags are Mouth / Ass / Pussy / Skin; containers are Mouth→Stomach, Womb, Ass, Skin |
| 41 | **Only attacks carry tags** — skills never do, so only attacks set *Continuing* |
| 42 | **Ass-tagged attacks lose 10% of the bank (rounded up) and gain +1 Strength per 10 ml lost** — the bank is ammunition, and the archetype wants multi-hit cards — **[ratio superseded by 76: +1 per 4 ml]** |
| 43 | *Deep Throat* is a **card family** (vanilla `CardTag.Strike` pattern), so *No Gag Reflex* buffs a group rather than one card |
| 44 | ~~Intake rule is by **card type**: Powers may generate, Attacks and Skills may not — the issue was repeatability, not generation~~ **Replaced 20 Nov 2026 by §0** [Gorim] |
| 45 | Sigil gating uses **2 / 4 / 6**, three tiers, not 3 / 6 |
| 46 | *Edging Form* is **strongest in Squeezing** — it raises the floor exactly where the Ass drain bites hardest |
| 47 | The Mouth exception generates **precum** — combat-only, spent first, never heals or banks |
| 48 | **Wet Patch** (uncommon relic): heal 2 HP whenever a reservoir leaks outside combat. Deliberate opposite of the Plug |
| 49 | Card list drafted — see `SEDUCTRESS_CARDS.md` (38 draftable + 3 basics) |
| 50 | **Universal spend order**: any card that spends ml takes from **Skin → Mouth → Ass → Womb**. Never written per card; reminder text only. Least-valuable-first, so a generic spender is cheap for a Skin deck and expensive for a focused one — self-limiting without a restriction clause |
| 51 | **Deep Throat** is a tag with its own reminder text — "Climaxes caused by this card are swallowed immediately" — carried by every member rather than written on each card. Makes the family the only mid-combat banking route |
| 52 | Freehand's job is **structural roles**, not larger numbers: an Innate opener that also routes *Continuing*, the only Block attack, a cost-discount bridge into Ass/Pussy, self-Vulnerable AoE, and the generic ml spender |
| 53 | *Scoop It Up* lives under Draining, untagged. It transfers Skin into Mouth, so it only functions in a Mouth deck even without the tag |
| 54 | Old *Grind* (3×3, no HP) strictly dominated *Clench* under the Ass Strength rule and is cut. *Stuffed* takes the Common slot; the new *Grind* is Uncommon and **single-hit**, trading bank for Block that scales with the bank |
| 55 | *Grind*'s Block is **per hit**, so *Stretch My Hole Daddy* doubles it. Intentional — that Power exists to be built around |
| 56 | Squeezing gets a real defensive card. Spending ammunition on Block is the correct cost, given the archetype has to survive at low HP |
| 57 | *Rapture* gives **3 energy on each Climax**, not on play — so a Climax you cause on your turn is worth real tempo and one the enemy causes is not. Rewards Climaxing on your own terms. Its Block is ordinary decaying Block, not permanent |
| 58 | *Ritual Mark* buffs Sigils **and Pussy cards**, so it is a payoff for the archetype rather than only for the Sigil count |
| 59 | **Climax does not end your turn.** Cut — it fires mid-card-resolution, so it would interrupt an in-flight queue, and +1 Lust plus +1 Sensitive is already sufficient cost |
| 60 | **Holding 3 Lust ends the run.** Stated as a threshold, never as a count of climaxes. Baseline 3 climaxes per run |
| 61 | **Rest-site relief** removes 1 Lust instead of healing. Self-limiting, because for this character HP is arousal — relief means entering the next fight already aroused. Pairs with the tightened cap: the cap and removal availability are two dials, not one |
| 62 | **A Lust card enters the run deck, not the current combat.** Sensitive punishes the fight, Lust punishes the run; keeping them on separate layers is what makes both legible |
| 63 | **Sensitive multiplies your own self-damage**, ignoring `ValueProp.Unpowered`. Deliberate departure from vanilla convention — §4's arithmetic is false without it |
| 64 | **Custom card tags work as plain casts** — `(CardTag)1001` upward. The enum cannot be extended, but tags are behaviour-less metadata and nothing switches over them. The card list's tags stand as written |
| 65 | **Minions produce no intake and do not count toward encounter size.** Prevents summoners becoming intake fountains that pay better than the elite summoning them |
| 66 | **Multi-enemy elites pay 15 ml each, not 20.** Makes §6.3 materially weaker — no encounter conceives from empty except a 2-stack boss. Decided without data; the instrumented run is what tests it |
| 67 | ***Breed Me!*'s Sigil multiplier replaces its +5** rather than compounding with it. The flat +5 applies only below tier 1 |
| 68 | **§7.1 corrected to 2 / 4 / 6.** It read 3 / 6 through v2.1 — a documentation error; the card list was right from v9 |
| 84 | **A presentation converter rewrites vanilla vocabulary at display time**: Deal→Tease, **damage to→pleasure from**, **take damage→experience pleasure**, damage→pleasure, Block→Poise, Heal→Soothe, HP→Arousal. **Applied in order, longest phrase first.** Flipping the preposition is what moves the phrase from the transfer class (commerce) to the extraction class (sensual). Word-for-word with syntax untouched, so it can safely rewrite relics, intents and other characters' cards. Design docs and code keep the vanilla words |
| 87 | **Pregnancy is a carried state, not an instant.** Womb neither fills nor leaks; +1 Sensitive at combat start; ends at the next rest site with a Sigil. **Lactating** is a separate state that shuffles a **Milk** card into the combat draw pile each turn and **continues one combat past the birth**. Turns Breeding into a cycle and makes the per-act pacing self-enforcing |
| 88 | ~~Overflow at conception is discarded to Skin.~~ **Withdrawn** — overflow now advances the stage |
| 95 | **Birth empties the Womb entirely and the excess is wasted** — the one place output is destroyed rather than recycled. Creates natural delivery points at exactly N×100, so the question is "am I close to the next threshold", not "carry or not" |
| 96 | **All leak routes to Skin**, and **Skin itself no longer leaks** — a pure accumulator, visible on the sprite |
| 101 | **Leaked cum arrives at 75%**; **Indulge reclaims ⅔**. Composed: leaked-and-reclaimed cum is worth exactly **half** what direct banking is. This is what makes routing skill matter — previously a Mouth build banked the same total however badly it routed |
| 102 | **Deliberate Skin output arrives whole** — Freehand kills and unrouted damage are not taxed, only accidental leak |
| 103 | **Ass leak is banded**: 5% of the first 200 plus 25% of the excess. At 300 ml that is 35, not 75 |
| 105 | **[Gorim] Max HP is 20.** 20 × 4 climaxes ≈ 80 effective, Ironclad's pool; ~51 within one fight once Sensitive compounds. **Only Max HP is worth 4×** — it is refilled four times. Block, healing and damage are absolute and unchanged; HP *costs* scale inversely |
| 106 | **HP costs rescale as though the bar were 30–40**, not by a clean ÷3.25 — the bar is meant to be swingy. 20→9, 12→6, 10→4, 8→3, 6→3, 4→2, 3→2, 2→1; Plug to 2/floor and 1/turn |
| 107 | **The Stomach gives +1 Max HP per 10 ml banked** — ~20–25 Max HP an act, i.e. +80–100 effective, which is Draining's whole progression |
| 108 | **[Gorim] Stomach milestones**: every ×50 grants +5 ml precum at combat start (cumulative); **100 ml** swaps end-of-combat swallowing for swallowing at 50 ml; **200 ml** removes Indulge's spillage; **300 ml** makes precum bank as cum |
| 110 | **[clarified] The Mouth banks to the Stomach at the end of every combat by default.** *Gulp* buys timing, the heal and Sensitive removal — not the banking itself — **[the card is cut; see 111, 112]** |
| 109 | **Max HP from relics is halved**, carried on the starting relic — food relics would otherwise be worth 4× here and outclass every other pick. **[Gorim, 2026-11-23]** Max HP loss is halved the same way. |
| 104 | **Removing a Lust card is worth a full heal regardless of current HP**, so Relief dominates Indulge on the health axis and Indulge's only edge is the Stomach. **Do not tune Indulge down further until the Stomach payoff exists** |
| 97 | **The Seductress has no `HEAL`.** Her rest site offers **Indulge** (empty Skin: Soothe 1 per ml, bank it to the Stomach), **Relief** (remove a Lust card — the actual rest-equivalent) and Smith. Indulge is part Rest, part Smith. ***Scoop It Up* is cut from the pool**; leak-to-Skin would have made it mandatory, and a card nobody can skip is worse than no card |
| 98 | **1 HP per ml is not over-generous because the bar is a quarter of a life** — a full heal restores ~25% of total life resource against vanilla HEAL's 30% of all of it |
| 99 | **The Womb's leak is a bounded 0–10 ml sawtooth** that resets at each locked threshold — identical while filling and while carrying. The Ass's leak is **unbounded** and grows with the bank. That difference, not a phase distinction, is why halving Womb leak is a good relic and the Plug is a late-game one |
| 100 | ***Wet Patch* → *Sticky Bandaid***: halves **Womb** leak. A Pussy relic that targets the filling phase, so pregnancies and therefore Sigils arrive sooner. Wet Patch's all-reservoir heal became incoherent once leak routed to Skin |
| 92 | **Conception resolves at end of combat**, not on crossing. No lactation mid-fight; pregnant on leaving the room; a clean moment for an animation. Spending the Womb below the threshold before the bell prevents it — intended agency |
| 93 | **Milk heals rather than Blocks.** Healing cannot prevent a one-shot, so at high Sensitive the carried state is a rising chance of sudden death rather than a grind. Block would have made it merely slow |
| 94 | ***Milking Grip* leaves the draft pool** once it is a 100 ml threshold reward |
| 89 | **Milk** is 0 cost, Tease 4 / Soothe 2, Exhaust, on the standard Sigil ladder. Shuffled into the **combat** draw pile — never the master deck, and deliberately the weaker of pile-vs-hand. At 7 Sigils the 0-cost rule sheds its non-energy cost — which for Milk means it stops Exhausting and accumulates |
| 90 | ***Glazed* is renamed *Skin*.** Less poetic, clearer, and parallel with the body-part naming everything else uses |
| 91 | **A card is tier-gated or pregnancy-gated, never both** |
| 86 | **The verb should later scale with the damage number**, as enemy intents already band their icons: Tease / Coax / Draw out / Wring. Needs a defined base case for "damage equal to…", which has no literal number |
| 85 | **HP inverts** — "Lose N HP" → "Gain N Arousal" — so HP is phrase-level, not word-level. **Comparisons flip their operator and need a hand-written allow-list**; anything not on it stays in vanilla wording rather than being converted wrong |
| 82 | **The Plug's 10% spill counts as ml lost by the card that pulled it**, so it grants Strength. Otherwise a plugged Ass card pays +1 energy and double the drain for the same damage — a downside applied twice to the archetype that triggers it |
| 83 | ***Rapture* + *Forced Orgasm* is an accepted build, not a problem.** Rapture was near-dead without a Climax-on-demand card: a 3-cost Rare keyed off the event you spend the run avoiding. Forced Orgasm turns it into a plan you draft toward |
| 76 | **Ass Strength is +1 per 4 ml lost**, tuned for the 200–270 ml Milking Grip plateau rather than the unaided bank. Depends on the unlocks landing; without them it wants +1 per 2 ml |
| 77 | **The Ass bank has a structural fixed point at 100 ml** — percentage spend against flat intake. *Milking Grip* moves it to a leak-clamped 215–275 |
| 78 | **Ass progresses by threshold unlocks**: 100 ml grants *Milking Grip*, 200 ml grants the **Plug**. Tracked by **high-water mark**, kept for the run. Converts a draft dependency into a system reward |
| 79 | **Precum is a built system, not a card clause** — it is now load-bearing for a starting card |
| 80 | **Let Go is renamed *Seek Relief***; its upgrade lowers the HP cost 20 → 10 rather than drawing more |
| 81 | ***Abandon* is cut and replaced by *Forced Orgasm*** — 0 cost Uncommon, Exhaust: Climax with no heal and no Lust card, draw 2, gain 1 energy. Adrenaline at one rarity lower, priced in Sensitive |
| 74 | **Leg-Locked** commits a kill's route to the Pussy in advance, overriding the finishing-card tag and the Skin catch-all. Closes the hole where Poison, reflect and the Sigil passive leaked Breeding's own progression into Skin |
| 75 | **Powers repeat like anything else at tier 4** — `DuplicationPower`/`ModifyCardPlayCount` is vanilla and generic, and a repeated buff applies twice. The exception is a non-stacking (Barricade-style) effect, of which this pool has none. *Ritual Mark* and *Rapture* take bespoke tier 4s by choice, not necessity |
| 70 | **Sigil thresholds are 1 / 3 / 5 / 7.** The first Sigil must be a spike — at 2/4/6 the first pregnancy changed nothing on any card |
| 71 | **Fixed tier shape**: 1 adds an effect, 3 grows the base, 5 grows the effect, 7 repeats a number of times equal to cost. A shared scale must carry a shared meaning |
| 72 | **0-cost cards lose their non-energy cost at tier 4** rather than repeating. Tier 4 applies to Powers as well as Attacks |
| 73 | **The starting deck carries two class cards**, per Silent/Defect/Watcher rather than Ironclad: *Breed Me!* and a Mouth attack granting Block. Ass is the wrong starter — its bank starts empty — **[the Mouth starter is *Suckle*, which grants precum rather than Block; see 115]** |
| 69 | ***Consummate* costs 0**, down from 2. At 2 its net energy was zero, making a Rare strictly worse than a vanilla Common. May want to be 1 once the Sigil passive exists |
| 111 | **[Gorim, 2026-09-08] Every swallow heals 1 HP per 2 ml, cum and precum together** — the end-of-combat swallow and the 50 ml one alike. Precum therefore heals on its way out though it banks nothing. [Gorim] noted the rate "is maybe high"; current spec, not settled |
| 112 | **[Gorim, 2026-09-08] The *Gulp* card is cut.** The name belongs to the 100 ml Stomach milestone (the Mouth also swallows at 50 ml), and the Sensitive removal came with it: a swallow of 25 ml or more removes 1 Sensitive once the milestone is reached. The same idea did not want to exist twice |
| 113 | **[Gorim, 2026-09-07] *Ritual Mark* upgrades to Innate** and changes no numbers; **[Gorim, 2026-09-08]** it buffs **Breasts** cards as well as Pussy |
| 114 | **[Gorim, 2026-09-08] Breasts is a category tag paired with Skin**, as Deep Throat is paired with Mouth. *Milk* carries it, is **heal 4 / deal 2** with the 3-Sigil rung growing the heal, and drags *Continuing* to Skin — pregnancy is meant to fight the routing that caused it |
| 115 | **[Gorim, 2026-09-09] *Suckle*'s upgrade is cost 0.** Third setting, after 10 ml and +3 damage. Base damage stays 4 |
| 116 | **[Claude, 2026-11-14, from the build] The draft pool is 33 cards**: Core 4, Freehand 5, Draining 9, Squeezing 7, Breeding 8; 13 Common / 15 Uncommon / 5 Rare. Gaps are listed in the card list §14 — **[superseded by 126]** |
| 117 | **[Gorim, 2026-11-20] Every Attack gains cum 1:1 with the HP it actually removes**, into the attack's hole — no overkill, nothing absorbed by Block. Replaces 44 and the flat table in 11. [Claude readings, unconfirmed] Minions pay nothing for hits either (65 carried over); Leg-Locked claims hits as well as the kill |
| 118 | **[Gorim, 2026-11-20] Kill bonus: 25% of the enemy's Max HP to the finishing hole**, replacing the flat 10 / 15 / 40 / 60 ml (11, 66). Reduce proportionally in multiplayer if that is ever built. [Claude] Rounded up; *Breed Me!*'s flat bonus is 25 ml and its Sigil multipliers apply to this bonus (67 unchanged); *Milking Grip* doubles the Ass kill bonus only; *Hyperfertile* doubles all Pussy cum |
| 119 | **[Gorim, 2026-11-20] Every ml requirement ×5, "for now, and see where that gets us."** Capacities 250 / 500 / 1000; pregnancy at N × 500; *Milking Grip* at 500, Plug at 1000; Stomach +1 Max HP per 50 ml with milestones at ×250, 500, 1000, 1500; leak bands; Spend, Eat and precum amounts on cards. [Claude] *Spit* is 10% (20% upgraded) with debuffs per 50 ml; belly bands inflate per 500 ml over 1000. **Indulge heals 1 HP per 5 ml recovered ([Gorim], see §6.3c)** |
| 120 | **[Gorim, 2026-11-20] Swallowing heals 1 HP per 10 ml** (was 1 per 2, row 111) |
| 121 | **[Gorim, 2026-11-20] The Ass rule stays**, its bonus damage gains cum, and attacks with three or more hits should grow the bank without limit — the Squeezing direction. **[Claude, seen but not explicitly confirmed] The ratio is +1 damage per 3 ml lost**, not ×5 (supersedes 76). The fixed-point argument in 77 no longer applies |
| 122 | **[Gorim, 2026-11-20] Sensitive no longer raises ml intake; each stack raises Pussy damage instead** ("Pussy damage only for now"), and still +50% HP loss taken. [Claude, unconfirmed] +50% Pussy damage per stack. Amends 8; retires 37 |
| 123 | **[Gorim, 2026-11-20] *Edging Form* is reworked**: whenever an Attack gains cum, gain that much again as precum in the same hole; upgrade costs 1 less. Its old effect (35) is now row 117. **Precum stays, and may extend to other holes later.** [Claude, as built] Ass and Womb precum is spent first, gone at the bell before conception, and never counts toward the Ass high-water mark; the copy covers hits, not the kill bonus, and not Skin |
| 124 | **[Gorim, 2026-11-19] *Mouthful* is built with "then Gulp"** — "Gulp meaning eat all in your mouth, heal for half that as normal." **Gulp** is a card keyword: swallow everything in your Mouth now. [Claude] It heals at the current swallow rate (row 120), not half; the Block step is 1 per 25 ml |
| 125 | **[Gorim] *Hold the Edge* reworked from [Claude]'s draft**: "Remove retain, have it 'if you climax this turn, prevent that damage, gain intangible, and deal that damage to all enemies'." [Claude readings] It lasts until the end of the turn it is played, fires once, still Exhausts, and upgrades to cost 0. **[Gorim, 2026-11-19] *Rehydrate* buffed "to 10/10"** — 50 and 50 on the new scale |
| 126 | **[Claude, 2026-11-20, from the build] The draft pool is 40 cards**: Core 8, Freehand 5, Draining 11, Squeezing 7, Breeding 9; 16 Common / 17 Uncommon / 7 Rare; 20 Attacks / 12 Skills / 8 Powers. **[verified in game, 2026-11-20]** rows 117, 118, 120, 121's ratio, 122, 123, and the three cards in 124–125 |
| — | **[Gorim, 2026-11-20] Leak per floor stays.** Agreed in principle, not built: archetypes defined by events (Draining = healing, Squeezing = losing HP to your own cards, Breeding = Climaxing); "every Power you play strips an item of clothing" |

### Proposed, not confirmed

- Lust card statline (0 cost, +12 arousal, uncommon-strength benefit)
- Skin's v1 passive (its leak rate is now settled: none)
- Sigil passive strength; threshold values 3 / 6
- Inflation excess leak rate (25%/floor)
- Ass Plug relic statline, and whether it raises the inflation ceiling
- Card group split 4 / 5 / 11 / 8 / 8 / 1 / 2

### Open

- Resolve the Lust / Lust Sigil naming collision
- Is 3 Lust right (§5), or should it scale with ascension?
- Does Squeezing's fullness buff need a cap, given it only leaks 5%?
- What exactly does a Sigil's passive do, in v1's single type?
- **[resolved] The starting deck carries two class cards.** See the card list §4. Ironclad's single
  Bash is the outlier; Silent, Defect and Watcher all start with two, and Watcher's pair is the
  closest analogue — each card demonstrates a different half of the mechanic. Here that is
  *Breed Me!* (Pussy, the involved mechanic, and it teaches intake explicitly through its flat ml)
  and a Mouth attack — as built, *Suckle*, granting precum rather than Block (the other long progression path, and the archetype whose
  identity is staying high on the bar). **Ass is the wrong starter**: its rule spends a bank that
  starts empty and takes most of an act to fill, so an Ass starter is a plain attack with a dead
  clause for the whole opening. Because **Skills are untagged by decision**, the defensive starter
  had to be an Attack that grants Block rather than a Skill — no rules exception needed.
- ~~**Does the starting deck need a tagged card?**~~ Resolved above. As designed it had none — Pump is *Continuing*,
  which defaults to Skin with no tagged attack to inherit from — so nothing routes anywhere until
  the first draft and the character's central mechanic is invisible for the opening floors. The
  build has *Take Him In* in the starting deck as scaffolding. Adding one permanently means picking
  a default archetype, which is a real statement about the character rather than a convenience.
- ~~**Breeding's Sigil tiers do not read as a group.**~~ Resolved by the fixed tier shape in §7.1.
