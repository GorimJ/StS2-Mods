# The Seductress — slice 4 results

**Written for the design session.** [Gorim] does not read this file; anything he needs to act on
personally goes to him in chat instead. So this is a report from the build to the design: what the
code now actually does, where it diverges from the documents, and what the documents cannot
currently be trusted on.

Built against `SEDUCTRESS_DESIGN.md` **v3.10** and `SEDUCTRESS_CARDS.md` **v24**, both re-read in
full. Follows `SEDUCTRESS_SLICE3_RESULTS.md`.

**Attribution.** [Gorim] = his decision · [Claude-Cowork] = the design session's · [Claude-Code] =
mine, in this session.

Everything below compiles clean and is deployed. **§1-§10 were written before any of it was
played** — the first run is written up at the end, under "Playtest 2026-09-08", and it supersedes
several statements here. Read that section before relying on anything in this one. This slice is
larger than the three before it put together and it changes the character's core numbers, so treat
every figure here as "built to spec", never as measured. **Nothing in this document is evidence
about balance.** Designing on top of any number here means designing on top of an assumption.

---

## 0. What the design session needs to decide

Collected from across the build. None of these are blocking — everything is built and running — but
each is a place where the documents are silent, ambiguous, or describe something the engine cannot
do, and where I picked a reading rather than leaving a hole.

| # | Question | What the code does now |
|---|---|---|
| 2 | **What does *Edging Form* put in the Mouth — cum or precum?** It is a Power, so §6.2a lets it fill real cum; but §6.2a also says the Mouth exception generates *precum*. | Real cum, which quietly makes it a Draining banking engine. One line either way. |
| 3 | **Is *Burning Blood* still the right placeholder?** It heals 6 after every combat: 9% of a 65 HP bar, **30%** of a 20 HP one. Healing is absolute and unchanged by decision, so this is per the design — but it is now a very large per-combat heal, and it partly undoes what removing `HEAL` from the rest site was for. | Unchanged. The starting relic is explicitly undesigned; it wants a number when it gets one. |
| 4 | **Should the universal spend order be able to abort a *carried* pregnancy?** §6.3b makes spending below the threshold before the bell an intended way to *prevent* conception. It does not discuss spending below a stage you are already carrying. | It can. *Wet Work* on a clean skin and empty mouth will drop a stage, and the Sigils it would have paid are gone. |
| 5 | **Should the Ass Strength ratio round in the player's favour?** The drain rounds up and the Strength rounds down, so both run against the player at every non-round bank. | Rounds down. At a 46 ml bank *Clench* pays 5 ml for +1. |
| 6 | **Does *Suckle*'s text mislead?** A Suckle kill banks 5 + 10, not 5 — the card's own 5 ml changes kind, and the routed kill intake lands on top. | As specified. The number on screen is larger than the card implies. |
| 7 | **The Womb's "100 ml capacity" (§6.2) versus uncapped stages (§6.3b).** These cannot both be true. | Uncapped. §6.3b wins, since staging is the newer and more specific rule. |

---

## 0a. Breasts — a new Breeding subcategory [Gorim, 2026-09-08]

Raised by the §0 question about whether *Ritual Mark* should buff *Milk*, and settled by adding a
category rather than a special case.

> **Breasts** — the subcategory of Breeding covering the cards that do **not** feed the Womb.
> *Ritual Mark* buffs Pussy **and** Breasts, by default rather than as a tier.

* `SeductressTags.Breasts` sits beside `DeepThroat`, and **Breasts pairs with the Skin hole**
  exactly as Deep Throat pairs with Mouth — a category tag plus a real hole, not a tag outside the
  routing system.
* *Milk* is the first member: `Breasts · Skin`. Adding a second costs one line.
* Ritual Mark's coverage is now a card property, `IsRitualMarked` = `Hole == Pussy ||
  Tags.Contains(Breasts)`.
* *Ritual Mark*'s text becomes "Your Pussy and Breasts cards deal N additional damage"; its tier-2
  line becomes "they deal N instead", so the tier-3 "Sigils deal 4 **too**" parity still holds.

**[Gorim] Lactation drags a Breeding deck off its own routing, and that is the point.** Because
Breasts is Skin, playing Milk routes its kills to Skin *and* sets the Continuing mode to Skin — at
Stage 3, three times a turn. I had flagged that as a hazard and built around it; the ruling is that
it is desirable. Multiple pregnancies are now reachable, and each one should make the next harder
to reach. The mechanic fights itself, which is the same shape as Sensitive making the bar you are
filling smaller.

### *Milk* inverted [Gorim, 2026-09-08]

> **It is a heal card first and an attack second.**

`0 cost. Attack · Breasts. Heal 4. Deal 2. Exhaust.`
`1 Sigil: draw 1. · 3: heal 7 instead. · 5: draw 2 instead. · 7: does not Exhaust.`

The card list's 4 damage / 2 heal is reversed, and **the 3-Sigil tier now grows the healing**. The
tier shape is untouched — rung 2 is still "grows the base"; it is the base that moved.

[Claude-Code] This sharpens §6.3b's "Milk heals, it does not Block" rather than softening it: more
sustain, less output, and still nothing that can prevent a one-shot. A Stage 3 carry now tops up
faster while contributing less to ending the fight, so the dilution cost bites harder and the
argument for cashing out gets stronger.

### [Gorim] Build the calculated-heal var — and it paid for itself twice

The tier-2 swap moved onto the healing, which had no calculated var to hang it on. Building one
turned out to remove more scaffolding than it added.

**Ritual Mark now rides `ModifyDamageAdditive`**, the vanilla damage hook Strength uses, instead of
being added by hand inside each card body. That is strictly better than the calculated-var
workaround it replaces: a plain `DamageVar` runs `Hook.ModifyDamage` when it builds its preview, so
**the buff previews itself on the card face for free**. Adding it in the card body — how it worked
until now — put it in exactly the invisible class [Gorim] found in slice 3. Gated like Strength:
owner must be the dealer, powered attacks only, so it never touches self-damage.

That frees the generic calculated slot, so **Milk's heal takes it**: `CalculationBase 4` +
`CalculationExtra 1` × a tier multiplier of 3, giving a live 4 or 7 on the card face. Damage is
back to a plain `DamageVar(2)`.

**Every tier swap in the pool now previews.** Claim, Take Him In and Breed Me! keep
`CalculatedDamageVar` for their damage swap, with Ritual Mark removed from `TieredPussyBonus` so it
cannot double-count.

[Claude-Code] **Caught in passing:** rewriting Milk's description left three tokens (`{Heal}`,
`{CalculatedDamage}`, `{TierHeal}`) pointing at vars that no longer existed. One unresolved token
makes SmartFormat print the *entire* description raw, and it compiles clean — there is nothing to
catch it but reading the card in game. I have added a check over all 46 card files comparing every
`{Token}` in a description against the vars that card actually registers. **All 46 pass now**;
worth re-running after any var-set edit.

---

## 1. Precum — built (brief §1)

The Mouth is now two quantities that behave identically inside a fight and differently at the bell.

* `ReservoirState.Precum` sits alongside `Mouth`, and the **50 ml cap is on the total**.
* **Spending takes precum first** (`SpendMouth`), so a precum card is never wasted and bankable cum
  survives to the swallow.
* At the end of combat precum **evaporates**: no heal, no Stomach.

*Suckle* rebuilt to the spec: `Deal 4. Gain 5 ml precum. If this kills, gain 5 ml instead.` The
kill test reads the target's `IsAlive` after the attack resolves, which is the same test intake
routing uses.

[Claude-Code] Worth knowing when reading the log: a Suckle **kill** banks 5 + 10, not 5. The card's
own 5 ml is what changes kind; the routed intake from the kill was always real cum and lands on
top. The card teaches the distinction correctly, but the number on screen will be larger than the
card text implies.

**Unblocked and built:** *Lick Pre-cum*, *Milked Dry*. **Also built:** *Gulp*, *Wet Work* — so
**the card pool is now complete**, 44 files matching the cards doc group for group.

---

## 2. Ass threshold unlocks and the ratio — built (brief §2)

Shipped together, as the brief required.

* **High-water mark** (`AssHighWater`) saved alongside the reservoirs and touched anywhere the bank
  rises, including the console scaffold.
* **100 ml** → *Milking Grip* added to the deck. **200 ml** → the **Plug**. **300 ml** → rung
  defined, deliberately empty.
* Checked on entering a floor, after the leak. **Idempotent by asking whether the player already
  holds the reward** rather than keeping a granted flag — so a repeat call cannot duplicate, but
  equally, a Milking Grip removed at a shop *will* be re-granted on the next floor. Flagging that
  rather than fixing it, because the alternative (a fired flag) makes the reward vanish forever if
  it is ever lost to something else.
* ***Milking Grip* leaves the draft pool.**
* **Ratio is +1 Strength per 4 ml lost.** Verified against design §6.1's table: at 200 ml Clench
  2×10, Rut 3×11, Overflow 11 to all, Blowout 4×31 = **124**. All four match exactly.

[Claude-Code] **`AssStrength` rounds down, and the drain rounds up**, so both roundings run against
the player. At a 46 ml bank Clench pays 5 ml for +1. That is intended to be weak, but it is weaker
than the design's arithmetic implies at every non-round number.

### [Claude-Code] The Ass cap was silently blocking inflation

`Add(Hole.Ass)` clamped at 200 ml. **Inflation was therefore impossible** — which quietly voided
§6.1's "at an inflated 400 ml", §6.4's banded 25% excess leak (nothing could ever be above 200 to
leak), and the 400 ml parity point §6.1a computes for Milking Grip. The Plug's entire late-game
case rests on carrying an inflated spike, so the relic being granted at 200 ml would have arrived
to protect a bank that could not exist.

Cap removed. 200 is now the nominal capacity, not a ceiling. This is a pre-existing bug from slice
2, not something this slice introduced, and nothing had caught it because no run had reached 200.

---

## 3. Leak-to-Skin and the rest site — built (brief §3)

* ***Glazed* renamed *Skin*** everywhere — enum, tag, reservoir, save keys, HUD, logs, card text.
  A run saved before this build loses its Skin value on load; nothing else moves.
* **All leak routes to Skin at 75%.** Deliberate Skin output (Freehand kills, unrouted damage)
  still arrives whole.
* **Skin no longer leaks at all.**
* **Ass leak is banded** — 5% of the first 200 plus 25% of the excess. At 300 ml that is 35.
  Checked the pre-existing implementation as the brief asked: it was already banded and already
  correct.
* **Womb leaks only the unlocked excess** above the pregnancy threshold.
* **`HEAL` removed** from the rest site. The site is now **Indulge**, **Relief**, Smith, plus
  **Give Birth** when carrying.
* **Indulge** recovers ⅔ of Skin (all of it past 200 ml Stomach): heal 1 per ml and bank that ml.
  Disabled with an empty Skin, no fallback.
* ***Scoop It Up* deleted** from the pool.
* ***Sticky Bandaid*** built — halves Womb leak.

**Instrumented as the brief asked.** Every floor line now carries Skin, what Indulge recovered and
banked, which rest option was taken, reliefs and climaxes per run, pregnancy state, Milk generated,
the Ass high-water mark and which thresholds have fired.

---

## 4. Max HP 20 and the Stomach — built (brief §4)

* **`StartingHp` is 20.**
* **Every HP cost rescaled** per §6.5's table. All twelve mapped exactly onto the table with no
  card left over and no row unused: Aching/Craving 12→6, Seek Relief 20→9, Surrender 10→4,
  Stuffed 8→3, Rut 6→3, Clench/Deep Throat 4→2, Throat Fuck/Balls Deep 3→2, Grind/Claim 2→1.
  Plug to 2 per floor and 1 per turn.
* **The Stomach is built.** +1 Max HP per 10 ml; the Mouth banks at the end of **every** combat by
  default, and *Gulp* buys only the timing, the heal and the Sensitive removal.
* **All four milestones:** ×50 (50/150/250/350) grants +5 ml precum at combat start cumulatively;
  100 ml adds a swallow whenever the Mouth reaches 50; 200 ml removes Indulge's spill; 300 ml makes
  precum bank as cum.
* **Max HP from outside sources is halved.**

### [Claude-Code] Three divergences in this section, all deliberate

**The halving is a Harmony patch on `CreatureCmd.GainMaxHp`, not a relic, and it halves every
external source rather than only relics.** There is no hook on Max HP gain and no source parameter
on the command, so a prefix cannot tell a relic from an event or a potion. Halving all of them is
the coherent reading — an event handing out Max HP has exactly the same 4× problem a food relic
does — but the design says "relics", so this is wider than the letter of it. It also survives the
starting relic being replaced, which carrying it on the relic would not.

**It rounds up.** Rounding down would silently zero any +1, turning a small reward into no reward.

**Watch Burning Blood.** It heals 6 after every combat — 9% of a 65 HP bar, **30%** of a 20 HP one.
Healing is absolute and unchanged by decision, so this is per the design rather than a bug, but it
is now a very large per-combat heal on a character whose tension is arousal accumulating between
fights, and it partly undoes what removing `HEAL` from the rest site was for. Left alone because
the starting relic is explicitly undesigned; it wants a number when it gets one.

---

## 5. The rest of the backlog — built (brief §5)

* ***Let Go* → *Seek Relief*.** 0 cost, lose 9 HP, draw 2; upgrade lowers the cost to 4 rather than
  drawing more. Stays in the starting deck.
* ***Abandon* → *Forced Orgasm*.** 0 cost Uncommon, Exhaust, Climax with no heal and no Lust card,
  draw 2, gain 1 energy; upgrade draws 3.
* ***Afterglow***'s engine was already built in slice 3 and matches the spec.
* **The Plug's 10% spill counts as ml lost by the card that pulled it**, so it grants Strength.
* **Milking Grip's upgrade** now works: the power carries the drain **percentage** (5 normally, 3
  upgraded) rather than a hard-coded halving, because "lose 3%" is not a halving of anything.

### [Claude-Code] The name collision I created in slice 3, now resolved

In slice 3 I built the lust-free heal-free Climax card and, on Gorim's instruction, called it *Seek
Relief* and put it in the starting deck. **v24 assigns that name to the renamed *Let Go*** and
calls the Climax card *Forced Orgasm*. So the slice-3 arrangement is fully reversed: the Climax
card is now a draftable Uncommon, *Let Go* keeps its slot under the new name, and the starting deck
returns to its original shape.

This is exactly the kind of thing the brief's reversal table was warning about, and it would have
been invisible to a diff-from-memory read.

---

## 6. Pregnancy — built (brief §6)

The mechanic the character was built around, existing for the first time.

* **Stage N at N × 100 ml.** The Womb keeps filling; further intake advances the stage.
* **Uncapped** — nothing stops Stage 4.
* **Conception resolves at the end of combat**, so spending the Womb below the threshold before the
  bell prevents it. Preserved as intended agency.
* **Leak applies only above the locked threshold.**
* **Birth** at a rest site: gain N Sigils, the Womb empties completely, the excess is wasted and
  logged so the waste is visible.
* **Lactating** is a separate state that runs **one further combat after the birth**; conceiving
  during the tail re-enters Pregnant and lactation never lapses.
* **N Sensitive at combat start; N Milk shuffled into the draw pile per turn.**
* **Milk** built on the standard Sigil ladder, including the tier-4 case where it stops Exhausting.

### [Claude-Code] Birth as a "free action" is the least certain thing in the slice

Implemented as a rest-site option whose `OnSelect` returns **false** — the game's own
"unsuccessful" path, the same one Smith uses with nothing to upgrade — on the expectation that the
site then stays open for a real pick. That is a reasonable read of the vanilla code, **not a
verified one**. If the site closes anyway, birth silently costs a rest site and the whole delivery
decision changes. Test it first.

The alternative, delivering automatically on arrival, was rejected because it removes the decision
the mechanic exists for: if you always deliver, "am I close enough to the next threshold to be
worth carrying" is never asked.

### [Claude-Code] Milk's tier-4 keyword, and a known gap

Exhaust is declared through `CanonicalKeywords` rather than stripped at play time, so the card
*reads* correctly in hand. `CardModel` caches its keyword set on first access and every Milk is
created fresh that turn, so the tier is read after the card exists. The gap: a Sigil exhausted by
*Consummate* mid-combat will not retroactively put Exhaust back on Milk already in the pile.

### [Claude-Code] The spend order can abort a carried pregnancy

The universal spend order ends at the Womb and does **not** protect the locked threshold, because
spending below it before the bell is a stated feature. The consequence the design does not discuss
is that a generic spender — *Wet Work* with a clean skin and empty mouth — can drop a **carried**
stage, not just prevent a new one. `SpendMl` re-derives the stage downward so the state stays
honest, but the Sigils that stage would have paid are simply gone. Flagging rather than blocking.

---

## 7. What the design docs get wrong

The brief asked for these. Grouped by how much they matter.

### Contradictions that would produce a wrong build

* **Cards §2 table: "Skin — External. Leaks 25%."** Design §6.3c and decision 96 say Skin does not
  leak at all, and the cards doc's own §6 text agrees. The table is stale. **Built to "does not
  leak".**
* **Cards §2 fill rates: "Elite, multi-enemy 20 ml each."** Design §6.2 and decision 66 say **15**.
  **Built to 15.**
* **Cards §7 header: "+1 Strength per 10 ml lost"**, with the worked example "+2 at 200 ml", and
  the *Blowout* commentary "loses 100 ml for +10 Strength, so four hits of 16 — 64 damage". Design
  §6.1 and decision 76 say **+1 per 4 ml**, which makes those 4 × 31 = 124. The whole of cards §7's
  arithmetic is on the superseded ratio. **Built to +1 per 4 ml.**
* **Cards §9a and design §6.3b give Milk as 4 damage / 2 heal with the 3-Sigil tier on the
  damage.** Superseded by [Gorim] on 2026-09-08 — see §0a. Built as 4 heal / 2 damage with the
  tier on the healing, and tagged Breasts.
* **Cards §9a and design §6.3b state Milk in the display lexicon** — "Tease 4 pleasure. Soothe 2" —
  which contradicts cards §1 and design §9a's own rule that documents and code keep the vanilla
  words, and the converter is explicitly not in this slice. **Built in vanilla words**; rendering
  it literally would have printed lexicon text with nothing converted around it.

### Stale text that does not change the build

* **Design §2 vocabulary: Stomach "150 ml ⇒ +5 max HP".** §6.5 says +1 per 10 ml, so 150 ml is +15.
* **Design §4: "*Let Go* at 3 stacks is 50 HP on a 65 HP character."** Stale on both the name and
  the bar.
* **Design §8: "Starting deck: 5 basic attack, 4 basic defend, 1 deliberate-climax card"** and
  "35 cards for v1" with a group table of Core 9 / Draining 8 / Squeezing 8 / Breeding 8. Neither
  matches cards §4 or §11.
* **Design §6.4: "v1 scope: still minimal. A stacking counter, leaks 25%/floor... spent by *Wet
  Work*, *Cheek Full* and *Overflow*."** Skin does not leak, and *Cheek Full* is not in the card
  list.
* **Cards §6 prose: "The 3 HP keeps the family's whole cost pattern intact."** The table says 2 HP
  after the rescale.
* **Cards §8: "It pairs with *Abandon* — 0 cost, Climax immediately, Draw 5."** Abandon is cut;
  this is *Forced Orgasm* now.
* **Cards §11: "Total 37 draftable + 2."** The groups sum to 36 draftable, and the header says "37
  draftable plus 5 basics" while the table lists basics separately. With Milking Grip leaving the
  pool it is 35 offerable.

### The register and §6.5 disagree on whether the default swallow heals

**Found in play by [Gorim], 2026-09-08.** Design §6.5 and register **110** both list "the heal"
among the things *Gulp* buys, which reads as: the default end-of-combat swallow banks but does not
heal. Register **33** says the opposite — the Mouth "cashes out at end of combat — **heal half the
ml**, bank the full amount". Both are marked locked.

I built to §6.5. **[Gorim] has ruled in favour of 33:** swallowing heals **1 HP per 2 ml**, on cum
*and* precum. See §8a.

### The decision register contradicts itself

§12's **Locked** table still carries superseded rows at full strength, with no marking that a later
entry overrides them. A reader taking the table at face value gets: 5 Lust ends the run (9, now 3
per 60), multi-elite 20 ml (11, now 15 per 66), Ass leaks a flat 5% (12, now banded per 103), Skin
leaks (19/20, now none per 96), thresholds 3/6 (15) and 2/4/6 (45), both now 1/3/5/7 per 70, and
the Plug at 4 HP/floor + 2 HP/turn (29, now 2 and 1 per 106).

**[Claude-Code] This is the doc's most dangerous property**, because the register reads as the
authoritative summary — it is where you would look to check a number quickly, and six of its locked
rows are wrong. Struck-through or annotated rows would fix it.

### Version numbering

The brief cites design **v3.8** and says cards moved "v13 → v23". The design file says **v3.10**,
the cards file says **v24 draft** in its title and "Companion to `SEDUCTRESS_DESIGN.md` **v3.9**"
on the next line. Four version claims across three files. Built against what the files actually
say.

The brief's §3 heading also calls it "the Scoop rest site"; it is **Indulge** everywhere else.

---

## 8. Other implementation notes

* **Fertile Sign removed from the starting relics.** Its own removal condition was "delete when
  Conception lands", and Conception has landed — but the reason to act now is measurement: a free
  Sigil from floor 1 turns on tier 1 across the whole Breeding deck before a single pregnancy,
  which is exactly the number this slice exists to report. The class and the `sigils` console
  command are kept for setting a count on purpose.
* **The Plug re-inserts automatically on entering a floor.** Register 28 says plugs are "toggled on
  the map between rooms", which needs UI that does not exist. This reproduces the intended rhythm —
  plugged between fights, unplugged to spend — without a click, at the cost that you cannot choose
  to stay unplugged through a dry stretch to save the 2 HP a floor.
* **`FloorLog` now resets per run.** Its run totals are static, so before this the second run of a
  session inherited the first one's climax and relief counts.
* **Milk and Milking Grip are excluded from drafts via the card pool's epoch filter.** There is no
  per-card "not draftable" flag; pool membership comes from the `[Pool]` attribute at registration
  and BaseLib throws without one, so the pool's own filter is the only hook that runs on every
  draft.
* **[Claude-Code] Open question: what does *Edging Form* put in the Mouth?** It is a Power, so
  §6.2a lets it fill real cum — but §6.2a also says the Mouth exception generates *precum*. As
  built it fills cum, which makes it a quiet Draining banking engine. One line either way; not
  changed without a ruling.

---

## 8a. Swallowing heals — [Gorim, 2026-09-08, found in play]

> **A swallow heals 1 HP per 2 ml, counting cum and precum together.**

Applies to every swallow, not only the end-of-combat one. Three consequences worth the design
session's attention:

* **Precum now pays out on its way out.** It banks nothing and evaporates, but it heals first — so
  precum generation is worth a card slot on its own rather than only as fuel for a spender. This is
  the largest indirect buff in the ruling.
* ***Gulp* changes shape.** It no longer buys the heal, because the heal happens anyway. Base Gulp
  is now pure tempo: it heals exactly what the bell would have healed, bought forward, plus the
  Sensitive removal and the room to refill the Mouth. **Its upgrade is where the raw value is** —
  100% instead of 50% — which is an unusual and rather good shape for a 0-cost Uncommon. It also
  means the card had to stop healing on its own, or it would have paid twice.
* **The 100 ml Stomach milestone got stronger than specified.** "The Mouth also swallows whenever
  it reaches 50 ml" was written as pure throughput — [Claude-Cowork]'s note says it "takes nothing
  away" and uncaps banking. With swallows healing, it is now **also a repeating 25 HP heal** on a
  20 HP bar, every time the Mouth fills. That is a large amount of sustain to arrive from a
  milestone that was described as neutral. Flagging rather than changing it: the rule is
  consistent, but the milestone's power level is no longer what §6.5 assumed.

[Gorim] has already noted 1-per-2 "is maybe high" — recorded as the current spec, not as settled.

[Claude-Code] Rounds down, matching Indulge and the per-floor leak.

---

## 9. Not in scope, as briefed

"If pregnant" cards, twins, birth choices, orbs as a channel/evoke loop, the lexicon converter
(§9a), voice, art, audio.

---

## 10. What is unverified

**[Gorim] gets this list in chat as a test order; it is here so the design session knows what
cannot yet be designed against.** Everything below is unplayed, and the first three change other
results if they are wrong.

1. **Birth as a free action.** Does the rest site stay open after delivering? If not, everything
   about the delivery decision is different from the design.
2. **Max HP 20 in a real fight.** The whole rescale rides on "not more fragile, more granular".
   Climaxes and reliefs per act are logged as instrumentation. **[Gorim] reads the balance himself
   — no conclusion about it should be drawn here or in the design docs from this run.**
3. **Removing `HEAL`.** The most uncertain change in the slice. How far Skin accumulation offsets
   it is not derivable at a desk. Watch Skin at every rest site and what Indulge returns.
   **[Gorim] Do not tune Indulge down on the strength of this run** — it has been nerfed twice
   already and the Stomach is only now built to weigh against it.
4. **Precum vs cum on *Suckle*.** Kill and non-kill, and check the log says which.
5. **Conception and gestation.** `reservoir womb 100`, finish a fight, confirm conception lands at
   the bell and not on crossing. Then `reservoir womb 250` for Stage 2 and check the leak only
   touches the 50.
6. **Milk.** Shuffled per turn at the right count, heals, and does not Block.
7. **Ass thresholds.** `reservoir ass 100` should hand over Milking Grip on the next floor; 200
   should hand over the Plug. Remember `reservoir ass 300` trips every rung at once.
8. **The Plug's burst.** Plugged, play an Ass card: +1 energy, double drain, double Strength, plug
   out, and back in on the next floor.
9. **Inflation.** Now that the cap is gone, check the Ass can actually exceed 200 and that 300 ml
   leaks 35, not 75.

**Carried forward from slice 3, still untested:** tier-4 repeats (`sigils 7`), the Ass rule at a
full bank (**not** *Clench* — though it is fixed now), *Consummate* shrinking the orb count, a
minion banking nothing, and *Spit*'s `{Portion}%` and *Ritual Mark* going Innate on upgrade.

Screenshots to `docs/spike-screenshots/` as files.

---

# Playtest — 2026-09-08, first slice-4 run

Console-assisted run to floor ~25, act 1 into act 2. **This is the first evidence about slice 4;
everything in §0 and §10 above was written before it.**

## Verified working — all first try

Nothing in this list needed a fix.

* **Precum, end to end.** [Gorim] The 50 ml cap is on the combined total (45 + 10 fitted 5 and
  reported the 5 that did not); *Suckle* gives precum on a normal hit and **cum on a kill**; precum
  evaporates at the bell.
* **Pregnancy, end to end.** Conception resolved **at the bell** rather than on crossing; Milk
  shuffled in while carrying; birth granted the Sigil, emptied the Womb and wasted 0 because it was
  delivered at exactly 100; the lactation tail armed correctly.
* **Birth is a free action.** [Gorim] Clicked it, got the Sigil, **the rest site stayed open**. This
  was the single most uncertain thing in the slice — `OnSelect` returning `false` was a read of the
  vanilla code, not a verified fact. It is verified now.
* **The Stomach and all four milestones.** +1 Max HP per 10 ml (258 ml → +25, on a 20 HP base);
  the ×50 rungs paying 15 ml of precum at combat start; the 100 ml Gulp swallow firing mid-combat;
  and **200 ml removing Indulge's spill** — 40 ml of Skin came back as 40, where it would have been
  26.
* **Leak → Skin at 75%, and the banded Ass leak.** At 230 ml the leak was **17** — `200 × 5%` plus
  `30 × 25%` — and **12** reached the Skin. Both exactly as specified.
* **Both Ass thresholds**, off the high-water mark, firing at 130 and 230 rather than the live
  value. Milking Grip into the deck, the Plug granted and inserted.
* **The Plug**, in full: +1 energy on Ass cards, 2 HP a floor, and the doubled drain paying doubled
  Strength — Clench read **15** at a 200 ml bank where unplugged it reads 10.
* **Inflation.** The bank reached 230. The pre-existing cap fixed this slice would have held it at
  200 and hidden the entire Plug payoff.
* **Breasts · Skin, and Ritual Mark on the damage hook.** [Gorim] Milk set the Continuing mode to
  Skin, dragging routing off a Pussy card played two lines earlier — the self-interference working
  as intended, unprompted. Ritual Mark's move onto `ModifyDamageAdditive` previewed correctly.
* **Max HP halving.** Caught in the wild: 258 ml Stomach earned +25 on a 20 base, and the player
  was on **47**, so an external +4 or +5 source arrived as +2.

## Bugs found in play

### *Milking Grip* drained a tenth instead of a half — [Gorim, found in play]

```
SEDUCTRESS-BLOWOUT spent 8 ml Ass (+20 dmg/hit, Milking Grip x0.1)
```

[Claude-Code] Mine, and a good lesson. I stored the drain **percentage in the power's `Amount`** —
but `MilkingGripPower` is `PowerStackType.Single`, and a single-stack power normalises its amount to
1. So 5 became 1, `1 ÷ 10 = 0.1`, and Ass attacks cost a tenth of what they should while still
paying full Strength *and* Milking Grip's doubled intake. A Blowout that should have spent 40 ml
spent 8, and with a 20 ml kill the bank **grew** on the card designed to empty it: 160 → 172.

Fixed by holding the rate in a field the card sets, the same shape as `RitualMarkPower.SigilBonus`.
Audited every other power: the only other single-stack powers are *No Gag Reflex* and *Stretch*, and
neither reads `Amount`.

**Worth generalising:** a `Single` power cannot carry a magnitude. Anything a card needs to hand a
single-stack power belongs in a field.

### *Restraint* and *Stuffed* did not partition the bar — [Gorim, found in play]

Restraint fired above half and Stuffed below it, so **HP exactly half belonged to neither** and both
archetypes' payoffs switched off on the same point. [Gorim] ruled Restraint takes the boundary, so
it now fires at at-or-above half and every HP value belongs to exactly one.

[Claude-Code] This predates slice 4; Max HP 20 only exposed it. One HP was 1.5% of a 65 HP bar and
is 4.5% of 22, and the halfway point is now a number you land on. **Expect one or two more of this
shape** — thresholds that were invisibly wrong at 65.

### The `.pck` had been stale for a day — [Claude-Code, process]

The two new rest-site options rendered as raw localization keys. `dotnet build` copies the DLL; the
`.pck` carrying localization is packed and copied only by `dotnet publish`. I used `build` all slice
— correct for compile checks — and verified only the DLL hash after each deploy, so I had positive
confirmation the code was current while the assets sat a day behind.

Only text was affected; every mechanic was live. **Fix: `publish` is what deploys, and verification
covers both files.**

## Relic interactions — checked exhaustively

Every vanilla relic touching death prevention or rest sites was audited.

* ***Lizard Tail* needs no fix, contrary to what both of us assumed.** `Hook.ShouldDie` polls
  `ShouldDie` across all models first and only reaches `ShouldDieLate` if nobody prevented. Climax
  uses the former and Lizard Tail the latter — so at 0–2 Lust the relic is never consulted and never
  consumed, and at 3 Lust, where Climax steps aside, it catches you. It is inert exactly when it
  should be and an extra life exactly when it should be, purely because Climax rides the vanilla
  death hook.
* ***Regal Pillow* was completely dead**, along with **Stone Humidifier** and **Night Terrors** —
  all three key off rest-site healing, which no longer existed. **Fixed generally**: Indulge now
  fires `Hook.ModifyRestSiteHealAmount` and `Hook.AfterRestSiteHeal`, so anything in vanilla or
  another mod that modifies rest healing composes for free rather than being named individually.
  The heal is boosted; the **ml banked is not** — a relic that improves resting gives you health,
  not more cum.
* **Everything else is fine.** Girya, Meat Cleaver, Miniature Tent, Pael's Growth, Pumpkin Candle
  and Shovel all *add* a rest option rather than depending on Heal existing. One to watch: Miniature
  Tent overrides `ShouldDisableRemainingRestSiteOptions`, so it may grey out Indulge, Relief and
  Birth together — not a bug, but it interacts oddly with birth being a free action.

## The number the slice existed to find

> **[Gorim] Skin accumulates far faster than the design assumed.**

At floor 25 the run was carrying **99 ml of Skin** with the 200 ml Stomach milestone active, i.e.
100% reclamation. That is a **99 HP heal on a 47 HP bar**, plus 99 ml into the Stomach for roughly
**+9 further Max HP**, sitting available at the next rest site. Skin does not leak, so it only rises.

Design §6.3c calls this figure "not derivable at a desk" and dismisses an earlier estimate of ~15 ml
a floor as too optimistic about how full reservoirs run. **On this evidence that estimate was too
pessimistic**, and the 200 ml milestone removing the spill is doing much of the work — it turned a
66 HP heal into a 99 HP one.

[Claude-Code] Recording the measurement only. [Gorim] reads the balance, and the standing
instruction not to tune Indulge down until the Stomach existed to weigh against it is now
satisfiable for the first time.

## *Suckle*'s upgrade, third setting — [Gorim, 2026-09-09]

> **The upgrade drops the cost to 0.** "It is not worth it as a damage card."

Cards doc §4 says the upgrade gives **10 ml**, which is what I built. That was cut on 2026-09-08
once swallowing healed, because 10 ml became 5 HP of sustain per play on a 20 HP bar; **+3 damage**
replaced it, and this replaces that.

**Base damage stays at 4.** [Gorim] Raising it to 5 was considered and declined.

[Claude-Code] The consequence is a change in kind, not degree. At 0 cost this stops being an attack
that happens to make precum and becomes **a free 5 ml every turn it is drawn** — 2.5 HP of sustain
a turn for no energy at the 1-per-2-ml swallow rate, on a card every run starts with. That is an
engine rather than a tempo bump, and it is what Draining wants, since the archetype's plan is
staying high on the bar. It self-limits on the 50 ml Mouth cap rather than on energy, which is the
right limiter for a Mouth card but a soft one.

**Cards doc §4 now disagrees with the build on this card twice over** — the upgrade, and Suckle's
role generally. Worth reconciling when the doc next moves.

---

## Still untested after this run

* The **spend order** (Skin → Mouth → Ass → Womb). *Wet Work* is now the only card that exercises
  it, since *Gulp* was cut.
* **300 ml Stomach** — precum banking as cum. The run reached 258.
* **Swallowing healing**, **Sensitive removal on the Gulp milestone**, **Suckle's +3 damage
  upgrade** and **Restraint's boundary** — all built after the game had launched, so none were in
  the running build.
* Tier-4 repeats, *Consummate* shrinking the orb count, a minion banking nothing, *Spit*'s
  `{Portion}%` and *Ritual Mark* going Innate — all carried forward from slice 3, still untouched.
