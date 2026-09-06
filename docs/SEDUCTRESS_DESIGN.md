# The Seductress — mechanical design v2.0

Status: design draft, 2026-09-05. Personal-use mod, single-player only.

Changes from v1.9: a **universal spend order** replaces per-card spend clauses; **Deep Throat**
becomes a tag with reminder text carrying auto-swallow; Freehand rebuilt around structural roles.

Earlier (v1.8): intake rule redrawn by card type (Powers may generate); Sigil thresholds move to 2/4/6.

Earlier (v1.7): **holes and containers separated** (§6.0); tags are attack-only; **Squeezing rebuilt** — the Ass bank is ammunition spent for Strength (§6.1); *Deep Throat* becomes a card family.

Earlier (v1.6): intake rounding rule; Squeezing stops spending its bank; Sensitive removal re-sited onto *Gulp*.

Earlier (v1.5): Plug now **taxes and auto-removes** rather than locking Ass cards out; the "marginal" verdict on wearing it is corrected (§7.3); *Edging Form* added as the Ancient card.

Earlier (v1.4): **Plug** specced as a common toggle relic (§7.3).

Earlier (v1.3): Mouth reworked — end-of-combat swallow, Total Swallowed track, pre-cum exception (§6.5).

Earlier (v1.2): **Freehand** added — no-hole attacks tagged *Glazed*, stronger in exchange for collecting nothing.

Earlier (v1.1): basic attack is **Pump**, tagged *Continuing*.

Earlier (v1.0): reservoirs measured in **ml** with per-hole capacities (Mouth 50 / Womb 100 / Ass 200); **cards no longer generate intake** — kills are the only source, and each archetype instead gets an in-combat identity based on its position on the HP bar (§6.2a).

Earlier (v0.9): naming rule adopted — vanilla mechanics keep vanilla words, so Poise and Composure are gone; card text is now written in base-game terms.

Earlier (v0.6-0.8): Sigil interaction settled as **threshold gating**, and the late-game
scaling warning withdrawn — a dominant act 3 is the correct shape for this archetype, not a
failure. Squeezing gains **inflation** (overfill above 100%). Glazed's fill sources
reconsidered now that poison is likely out. **Sigil passive defined.** Plugs became a
toggleable family that blocks filling as well as leaking.

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
| **Sensitive** | In-combat stacking state | +50% HP loss taken **and** +50% intake |
| **Lust** | Playable curse-type card | The death clock, §5 |
| **Intake** | Four reservoirs | §6 |
| **Conception** | The Womb reservoir's fill | 100 ml ⇒ pregnancy ⇒ Sigil |
| **Stomach** | Lifetime total swallowed | Draining's permanent track; 150 ml ⇒ +5 max HP. **Precum never counts** |
| **Precum** | Mouth-only, combat-only fuel | Spends like cum inside a fight; heals nothing and banks nothing |
| **Sigil** | Permanent passive earned by pregnancy | §7 |
| **Glazed** | External catch-all reservoir | §6.4 |
| **Continuing** | Attack keyword | Uses whichever hole your last Attack used this combat; Glazed if none yet. Resets each combat |
| **Freehand** | Attacks tagged *Glazed* | Hands, breasts, thighs. No hole used, so nothing is collected — ~40% more damage in exchange |

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

Max HP **65** proposed (Ironclad is 80) — not yet confirmed.

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

**Sensitive is +50% per stack**, applying to both arousal taken and Intake gained:

- Two climaxes **halve your effective bar** (`1 + 0.5×2 = 2.0`); a third is nearly immediate.
- One climax against a boss nearly pays a whole pregnancy: 60% × 1.5 = **90%**.
- One climax entering an elite with 40% banked completes one: 40% × 1.5 = 60%, +40% = **100%**.

Sensitive clears at end of combat. Lust, Intake and Sigils do not.

---

## 5. Lust cards — the death clock

Playable. Each gives a real benefit and a large chunk of arousal. The card never kills you
directly; it accelerates the loop that does.

- **5 Lust in deck** ends the run. Removable at shops and by the usual removal events.
- Shop removal becomes a **trade**, not a free good.
- **At 4 Lust you cannot afford to climax.**

Draft: cost 0, +12 arousal, benefit around "+2 energy this turn" or "draw 3".

---

## 6. The reservoirs

### 6.0 Holes and containers are different things

**[decided]** The design was conflating the hole a card uses with the container the semen ends up
in. Separating them fixes the naming and clarifies the systems:

| Hole (the tag) | Container | Capacity | Behaviour |
|---|---|---|---|
| **Mouth** | **Mouth** in combat → **Stomach** after | 50 ml → unbounded | Holds cum *and* precum; cum banks, precum evaporates |
| **Pussy** | **Womb** | 100 ml | Leaks 10%/floor; 100 ml ⇒ pregnancy ⇒ Sigil |
| **Ass** | **Ass** | 200 ml | Both hole and container |
| *(none)* | **Glazed** | — | External; leaks 25%/floor |

*Womb* is no longer a tag — the tag is **Pussy**, and Womb is what it fills. **Stomach** replaces
"Total Swallowed" as the name of Draining's permanent track, which reads far better.

**[decided] Only attacks carry tags.** A tag means *finishing with this attack fills this hole*.
Skills never finish anything, so they are never tagged no matter how hole-specific their text is.
Consequence: only attacks set the *Continuing* mode, so you lead with a tagged attack and your
Pumps follow it. The earlier claim that tagged skills exist to set the mode is withdrawn.

When an enemy climaxes, the attack that finished them routes their output via its
**Mouth / Ass / Pussy** tag.

| | **Draining** (Mouth) | **Squeezing** (Ass) | **Breeding** (Pussy) |
|---|---|---|---|
| Shape | Defensive / control | Aggro / damage | Scaling / engine |
| Leak per floor | 0% | 5% of current | 10% of current |
| In-combat payoff | Spent for Block, healing, Sensitive removal | **Bank is ammunition** — attacks spend it for Strength | — |
| Run-level payoff | **Stomach** ⇒ permanent Max HP | None — the ammunition loop is its rhythm | Pregnancy ⇒ Sigil |
| Arousal relationship | **Minimises** it — rarely climaxes | **Runs hot** — hits hard, self-arouses | Accepts it; climaxing accelerates |

Draining is self-limiting in a way worth keeping: never climaxing means never gaining
Sensitive, so never getting the intake multiplier. It is deliberately the safe build — and
its permanent Max HP track is the right reward for it, since more capacity is exactly
what a cautious player wants.

### 6.1 Squeezing: the bank is ammunition

**[decided] The Ass tag carries its own rule:**

> **Ass-tagged attacks** lose **10% of your current Ass** (rounded up), and gain **+1 Strength
> per 10 ml lost** for that attack.

A hole in active use leaks, and the leaking is what powers the hit. This replaces the earlier
"read the reservoir" design entirely and gives Squeezing the in-combat identity it was missing —
its bank is not a stat to protect, it is **ammunition**, spent across a fight and refilled by
kills routed back into it.

Because Strength applies per hit, the archetype now wants **multi-hit attacks**, which is a real
build signature rather than a number on a card. At 200 ml an attack loses 20 ml for +2; at an
inflated 400 ml it loses 40 for +4. On a three-hit card that is +6 or +12 damage.

**Inflation** still applies and now matters more: the excess above 200 ml leaks 25% per floor
outside combat, but inside a fight it is simply a deeper magazine. Filling to 400 before an elite
means your first few attacks hit for roughly double.

The counterweight is *Milking Grip*, which halves the drain and halves the Strength — conserve
the bank or convert it, not both.

**Squeezing still gets no permanent track.** Three progression systems is too many, and the
ammunition loop gives it a within-run rhythm that Draining's Stomach and Breeding's Sigils do not
have.

### 6.2 Capacities and fill rates

**[decided]** Reservoirs are measured in **ml**, with different capacities per hole:

| Reservoir | Capacity | Leak / floor | Fills in |
|---|---|---|---|
| Mouth | **50 ml** | 0% | ~5 normal kills |
| Womb | **100 ml** | 10% of current | ~10 normal kills |
| Ass | **200 ml** | 5% (25% on the excess above 200) | ~20 normal kills |

| Source | Fill | |
|---|---|---|
| Normal enemy | 10 ml | |
| Elite, multi-enemy | 20 ml each | |
| Elite, single | 40 ml | × `(1 + 0.5 × Sensitive)` |
| Boss | 60 ml | |

**[decided] Sensitive multipliers on intake round UP.** 5 ml at one stack is 7.5 → **8 ml**.
Rounding in the player's favour stops attrition quietly undercutting the "should I Climax before
this fight" calculation, which is the decision the whole system hangs on.

Womb capacity of 100 ml means every calculation below **carries over unchanged** — the old "%"
was already a percentage of 100.

The differing capacities do real work for free. Mouth cycles fast and is meant to be spent
repeatedly; Womb is the calibrated progression track; Ass takes most of an act to fill and is a
genuine long-term bank. That is the pacing each archetype wants, with no extra rules.

Keeping 50/100/200 rather than 500/1000/2000: the sense of excess is better bought from
**inflation** than from the baseline. An Ass at 400 ml reads as absurd precisely because 200 is
normal; raise the baseline and 400 becomes unremarkable, while mid-fight arithmetic gets harder,
which StS deliberately avoids.

### 6.2a Attacks and Skills do not generate intake — Powers may

**[decided]** The rule is drawn by **card type**, because the problem was never generation, it was
**repeatability**. An Attack or Skill can be replayed every turn and every shuffle; a Power resolves
once, costs a slot and the energy up front, and cannot be spammed.

- **Attacks and Skills** may spend or read a reservoir; they may not fill it.
- **Powers may fill.**
- **On-kill bonuses are fine on any type** — they are already gated by the kill.

**[decided] The Mouth exception generates precum, not cum.** Precum sits in the Mouth and is
indistinguishable from cum *inside* a combat — spendable by *Gulp*, *Spit* and anything else that
empties the Mouth, counting against the 50 ml cap. At the end of combat it evaporates: no heal, no
Stomach.

This is what makes the exception free. Mouth cards can manufacture the fuel they need to function
without inflating Draining's permanent track, so the generation is bounded to the fight it happens
in. **Spending order is precum first**, so a precum card is never wasted and bankable cum survives
to the swallow.

*Scoop It Up* also survives as a **transfer** rather than a generator — and what it moves is real
cum, so it banks normally.

This retroactively legitimises *Edging Form*, and it re-opens the door to Powers cut earlier for
breaking the old blanket rule.

Each archetype still needs an **in-combat** identity that is not "fill my reservoir":

| Archetype | Position on the HP bar | In-combat game |
|---|---|---|
| **Draining** | Stay **high** | Payoffs above half HP. Weak, Block, healing, Sensitive removal. |
| **Squeezing** | Stay **low, without tipping over** | Payoffs below half HP. The bank is ammunition. |
| **Breeding** | **Climax repeatedly** | Payoffs for having Climaxed. Wants Sensitive for the multiplier. |

### 6.3 Reachability

Proportional leak means a reservoir drifts toward `gain ÷ leak`, but **pregnancy triggers on
a peak, not on the drift level**. A single encounter delivers:

| Encounter | 0 stacks | 1 stack | 2 stacks |
|---|---|---|---|
| Single elite | 40% | **60%** | **80%** |
| Elite, 3 enemies | 60% | **90%** | **120%** |
| Boss | 60% | **90%** | **120%** |

So **elites and bosses are the pregnancy moments**, and Sensitive is what converts them. An
even-split deck drifts around 63% and conceives at elites and bosses. A committed Breeding
deck (≈75% routing, 14.25%/floor, drift 142%) conceives from ordinary fights as well, roughly
doubling the rate. Target: **1–2 pregnancies per act, 4–6 Sigils per run.**

### 6.4 Glazed — the catch-all

**[decided]** Output with no finishing card — thorns, a Sigil passive, a damage-over-time
effect — does not vanish and is not split three ways. It lands on you as **Glazed**.

Splitting was thematic nonsense; a "last tag played" memory would be gameable by playing one
tagged card and letting something else finish. Glazed is exact, closes the hole, and seeds
the fourth archetype.

**Traffic warning.** With poison likely out of the character's kit, unrouted kills are rare —
thorns, Sigil passives and cross-mod effects only. Glazed therefore **cannot be fed mainly by
accidents**, and its two real sources are:

- **Squeezing overfill draining down** (§6.1) — the deliberate, recurring source.
- **Cards that glaze on purpose** — which is what the v2 archetype has to be built from.

**[decided] Freehand gives Glazed a real source.** Attacks that use no hole — hands, breasts,
thighs — are tagged **Glazed** and hit around 40% harder than their tagged equivalents, because
they collect nothing. This makes *every* attack in the pool tagged one of four ways: no
"untagged" special case, no accidental routing, and Glazed stops depending on incidental kills
now that poison is out.

It also creates the interaction that makes *Continuing* bite. A Freehand attack sets your
Continuing mode to Glazed, so Pumps played after it route to Glazed until you re-establish a
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

It also feeds the system that produced it. The multi-enemy elite is the best Conception source
(20% each, ×1.5 with one Sensitive stack = 90% from three), and passive AoE is exactly what
helps you close out three enemies at once.

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

> **Ward** — Block 6.
> *3 Sigils:* Block +6.
> *6 Sigils:* and reflect damage.

Thresholds at **3 and 6**, against a target of 4–6 Sigils per run. Tier 1 lands mid-run on
most Breeding decks; tier 2 is aspirational and only a committed run reaches it. The card is
still playable at zero Sigils, so it is never a dead draw in a non-Breeding deck.

This also removes the double-multiplier problem without capping anything: passives scale
linearly with Sigil count, card effects step, and the two do not compound.

### 7.2 On the late game

A dominant act 3 is the **correct** shape here, not a balance failure. Breeding is the
high-risk scaling build — it pays for its ceiling in acts 1 and 2, where it is drafting cards
that do nothing yet and taking Lust cards to reach the Sensitive multipliers that convert
elites. Silent's poison, Defect's focus and Ironclad's strength all run the same curve: rough
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
> **While worn:** your Ass does not leak below 200 ml, and leaks only **5%** above it. Ass cannot
> be filled. Lose **4 HP** on entering each floor and **2 HP** at the start of each turn.
> **Ass-tagged cards cost 1 more and remove the Plug when played.**
> **Removing it** spills **10%** of your Ass.

**[decided] The plug removes itself.** Playing an Ass card while plugged costs 1 extra energy,
pulls the plug, and spills the 10%. No manual toggle needed in combat, and it is the only version
that makes physical sense — you cannot use that hole without taking the thing out of it first.
The +1 energy *is* the old removal cost, folded into the card that triggers it, which removes a
whole clause from the relic and a whole click from the turn.

### What it is actually for

The 4 HP per floor prices the item precisely, and the arithmetic says something specific:

Unplugged, Ass leaks 5% of the first 200 ml plus 25% of everything above it. Plugged, the first
200 ml does not leak at all and the excess leaks 5%.

| Holding | Leaks unplugged | Leaks plugged | Prevented | Cost | Worth it? |
|---|---|---|---|---|---|
| 200 ml (full, not inflated) | 10 ml | 0 ml | **10 ml** | 4 HP | Marginal — 10 ml is one normal kill |
| 300 ml (inflated) | 10 + 25 = 35 ml | 5 ml | **30 ml** | 4 HP | Clearly yes |
| 400 ml (heavily inflated) | 10 + 50 = 60 ml | 10 ml | **50 ml** | 4 HP | Emphatically yes |

**Correction to the "marginal" verdict above.** That table compares one floor's leak against one
floor's HP, and that framing is wrong, because the two costs are not the same kind of thing:
**the HP is recoverable and the semen is not.** Over a dry stretch — `? → chest → ? → campfire`,
no combat to refill — a 200 ml bank unplugged decays 200 → 190 → 180 → 172 → 163, losing ~37 ml.
Plugged it costs 16 HP and loses nothing, and the campfire at the end gives the HP back. Trading
16 refundable HP for 37 unrecoverable ml is a good deal, not a marginal one, and this character's
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

The **10% spill on removal** is the anti-spam clause and it does its job: at an inflated 400 ml
that is 40 ml gone, so flipping it casually is expensive. Removal cost scales with how much you
have to lose, which is the right shape.

### Wet Patch — uncommon relic

> **Wet Patch** — *Uncommon.* Whenever a reservoir leaks outside combat, **heal 2 HP**.

2 HP per leaking reservoir per floor, so 4 with Womb and Ass both filled, 6 with Glazed on top.
It rewards the two builds that actually hold liquid between fights, and it converts the design's
main attrition — leakage, which is otherwise pure loss — into slow sustain.

**It is the Plug's opposite, and that is the good part.** The Plug pays HP to stop leaking; Wet
Patch pays you HP *for* leaking. Holding both is close to nonsense, which turns a relic pickup
into a genuine fork in the run rather than an accumulation. Two items pulling against each other
on the same axis is worth more than two that stack.

Watch that it does not blunt the pressure it is meant to soften: 2 HP is small against losing
20 ml, so the leak should still hurt. If it starts feeling like leaking is *good*, the fix is
capping it at one trigger per floor rather than one per reservoir.

### Womb cover

Deferred. Conception is the run's actual progression track, so an item that protects it is
strictly stronger and should not be a common. Specced with the fourth archetype.

---

## 8. Card pool — 35 cards for v1

Starting deck: 5 basic attack, 4 basic defend, 1 deliberate-climax card.
Starting relic: reduce arousal by 6 after each combat.

| Group | Count | Role |
|---|---|---|
| **Core** | 9 | Untagged attacks and Block; the neutral spine. Includes deliberate-climax cards and one or two Glazed spenders. |
| **Draining** (Mouth) | 8 | Low-arousal attacks, arousal reduction, Sensitive cleansing, Entranced application. |
| **Squeezing** (Ass) | 8 | High-damage attacks with self-arousal; payoffs scaling on Ass fullness; Glazed conversion. |
| **Breeding** (Womb) | 8 | Finishers and executes; Sigil-count scaling; Sigil exhaust effects. |
| **Lust** | 2 | The curse cards (not draftable). |

---

## 9. Voice hooks

Own triggers on `BaseLib.Audio` (`AutoModAudio.PlaySfx`).

Card played (chance + cooldown) · arousal thresholds 25/50/75/90% · self climax · enemy
climax **per reservoir** (four variants incl. Glazed) · Sensitive gained · combat start ·
victory · rest-site relief · birth.

≈13 categories × 5 variants ≈ **65 lines** for v1. Card Play Voices scaled a small dataset
with GPT-SoVITS; same route for per-card lines later, and its `voice_manifest.json` schema
(profile / intensity / pace + delivery prompt) is a good direction sheet.

---

## 10. Implementation risks

1. **Death interception must catch every HP-loss path** — events, curses, self-damage,
   Poison, Doom.
2. **Inverted bar needs a Harmony patch** on `NHealthBar`. `HealthBarForecastRegistry` can
   overlay `FromLeft` segments without patching, but not reverse the base fill.
3. **Enemy "climax and pass out" must not break on-death triggers.**
4. **Lust cards must not be launderable** by exhaust, purge or transform.
5. **Sigils are uncapped permanents** — they need to survive save/load via
   `ExtendedSaveTypes`, and their passives must not stack into something degenerate.
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

### Locked

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
| 12 | Leak per floor, proportional: Womb 10% / Ass 5% / Mouth 0% / Glazed 25% |
| 13 | Womb 100% ⇒ pregnancy ⇒ Sigil at next rest site, free action |
| 14 | **Sigils**: uncapped, permanent, small always-on passive; some cards exhaust one for the combat. One type in v1 |
| 15 | Sigil-aware cards **gate on thresholds (3 / 6)**, never multiply per Sigil |
| 16 | A dominant act 3 for Breeding is intended, not a balance problem — the risk is priced in Lust cards |
| 17 | Target 4–6 Sigils per run, 1–2 per act |
| 18 | **Draining** banks toward permanent Max HP |
| 19 | **Squeezing** gets no permanent track — persistence via 5% leak plus **inflation** (overfill past 100%, excess leaks 25%/floor) |
| 20 | **Glazed** catches unrouted output, and is fed deliberately by Squeezing overfill draining down. Minimal in v1, fourth archetype in v2 |
| 21 | Multiplayer out of scope for this mod only |
| 22 | v1 = 35 cards |
| 23 | Voice: own implementation on `BaseLib.Audio` |
| 24 | **No custom debuff keyword** — vanilla Weak and Vulnerable only; Entranced dropped |
| 25 | HUD layout deferred to playtest |
| 26 | Poison is likely **not** in the character's kit — so unrouted kills are rare |
| 27 | **Sigil passive**: at the start of your turn, deal 1 damage to ALL enemies |
| 28 | Plugs **block filling as well as leaking**, and are toggled on the map between rooms |
| 29 | **Plug** is a **common** toggle relic: no leak below 200 ml / 5% above, Ass cannot fill, 4 HP per floor + 2 HP per turn. **Ass-tagged cards cost 1 more and remove it when played**; removal spills 10%. Used both for carrying an inflated spike and for surviving a combat-free stretch with the bank intact. Womb cover deferred |
| 31 | Basic attack is **Pump**, tagged ***Continuing*** — routes to the last hole used this combat, Glazed if none. Resets each combat. No fixed tag on the starter, so no archetype bias from the character-select screen |
| 32 | **Freehand** attacks (hand / breasts / thighs) are tagged *Glazed*, hit ~40% harder, and flip the *Continuing* mode to Glazed. Every attack is now tagged one of four ways |
| 33 | **Mouth cashes out at end of combat** — heal half the ml, bank the full amount to Total Swallowed. 150 ml swallowed ⇒ +5 permanent max HP |
| 34 | **Pre-cum**: Mouth alone may gain ml from cards without a kill (~5 ml), as a bounded exception to §6.2a. Preserving defeated monsters rejected |
| 35 | **Edging Form** (1, Power, **Ancient**): whenever you play an Attack, gain 1 ml in the hole it used. A deliberate exception to §6.2a, safe because it comes from a single Ancient source |
| 36 | Pool size: **32 is a floor, not a ceiling** — cards may be added without cutting others |
| 37 | Sensitive multipliers on intake **round up** |
| 38 | Squeezing **reads its bank rather than spending it** — *Tight Fit* and *Ruinous* cut; *Overflow* spends a proportion, not a flat amount |
| 39 | Sensitive removal sits on *Gulp*, gated behind swallowing 25 ml |
| 40 | **Holes ≠ containers.** Tags are Mouth / Ass / Pussy / Glazed; containers are Mouth→Stomach, Womb, Ass, Glazed |
| 41 | **Only attacks carry tags** — skills never do, so only attacks set *Continuing* |
| 42 | **Ass-tagged attacks lose 10% of the bank (rounded up) and gain +1 Strength per 10 ml lost** — the bank is ammunition, and the archetype wants multi-hit cards |
| 43 | *Deep Throat* is a **card family** (vanilla `CardTag.Strike` pattern), so *No Gag Reflex* buffs a group rather than one card |
| 44 | Intake rule is by **card type**: Powers may generate, Attacks and Skills may not — the issue was repeatability, not generation |
| 45 | Sigil gating uses **2 / 4 / 6**, three tiers, not 3 / 6 |
| 46 | *Edging Form* is **strongest in Squeezing** — it raises the floor exactly where the Ass drain bites hardest |
| 47 | The Mouth exception generates **precum** — combat-only, spent first, never heals or banks |
| 48 | **Wet Patch** (uncommon relic): heal 2 HP whenever a reservoir leaks outside combat. Deliberate opposite of the Plug |
| 49 | Card list drafted — see `SEDUCTRESS_CARDS.md` (38 draftable + 3 basics) |
| 50 | **Universal spend order**: any card that spends ml takes from **Glazed → Mouth → Ass → Womb**. Never written per card; reminder text only. Least-valuable-first, so a generic spender is cheap for a Glazed deck and expensive for a focused one — self-limiting without a restriction clause |
| 51 | **Deep Throat** is a tag with its own reminder text — "Climaxes caused by this card are swallowed immediately" — carried by every member rather than written on each card. Makes the family the only mid-combat banking route |
| 52 | Freehand's job is **structural roles**, not larger numbers: an Innate opener that also routes *Continuing*, the only Block attack, a cost-discount bridge into Ass/Pussy, self-Vulnerable AoE, and the generic ml spender |
| 53 | *Scoop It Up* lives under Draining, untagged. It transfers Glazed into Mouth, so it only functions in a Mouth deck even without the tag |
| 54 | Old *Grind* (3×3, no HP) strictly dominated *Clench* under the Ass Strength rule and is cut. *Stuffed* takes the Common slot; the new *Grind* is Uncommon and **single-hit**, trading bank for Block that scales with the bank |
| 55 | *Grind*'s Block is **per hit**, so *Stretch My Hole Daddy* doubles it. Intentional — that Power exists to be built around |
| 56 | Squeezing gets a real defensive card. Spending ammunition on Block is the correct cost, given the archetype has to survive at low HP |
| 57 | *Rapture* gives **3 energy on each Climax**, not on play — so a Climax you cause on your turn is worth real tempo and one the enemy causes is not. Rewards Climaxing on your own terms. Its Block is ordinary decaying Block, not permanent |
| 58 | *Ritual Mark* buffs Sigils **and Pussy cards**, so it is a payoff for the archetype rather than only for the Sigil count |
| 59 | **Climax does not end your turn.** Cut — it fires mid-card-resolution, so it would interrupt an in-flight queue, and +1 Lust plus +1 Sensitive is already sufficient cost |

### Proposed, not confirmed

- Max HP 65
- Lust card statline (0 cost, +12 arousal, uncommon-strength benefit)
- Draining Max HP thresholds and increment size
- Glazed leak rate (25%/floor) and its v1 passive
- Sigil passive strength; threshold values 3 / 6
- Inflation excess leak rate (25%/floor)
- Ass Plug relic statline, and whether it raises the inflation ceiling
- Card group split 4 / 5 / 11 / 8 / 8 / 1 / 2

### Open

- Resolve the Lust / Lust Sigil naming collision
- Is 5 Lust right, or should it scale with ascension?
- Does Squeezing's fullness buff need a cap, given it only leaks 5%?
- What exactly does a Sigil's passive do, in v1's single type?
