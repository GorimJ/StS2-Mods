# The Seductress — slice 4: the backlog, then pregnancy

Follows `SEDUCTRESS_SLICE3_RESULTS.md`. Built against `SEDUCTRESS_DESIGN.md` **v3.8** and
`SEDUCTRESS_CARDS.md` **v24**.

**Re-read both docs in full.** They moved from v2.4 → v3.8 and v13 → v23; diffing from memory will
miss things, and several rules were *reversed* rather than extended. **Do not edit either** — they
are input. Report anything they get wrong; the register is Gorim's.

**Attribution.** Mark your own notes **[Claude-Code]**, Gorim's **[Gorim]**, and anything traceable
to the design session **[Claude-Cowork]**. The design docs now use the same convention.

### Reversals to watch for specifically

These contradict what earlier drafts said, so a partial read will produce the wrong build:

| Was | Now |
|---|---|
| Max HP 65 | **20** — and every HP cost rescaled |
| Womb locked while pregnant | **Keeps filling** — further intake advances the stage |
| Birth keeps the remainder | **Empties the Womb entirely; the excess is wasted** |
| Overflow at conception → Skin | **Withdrawn** — overflow advances the stage instead |
| Skin leaks 25%/floor | **Does not leak at all** |
| Leak vanishes | **Routes to Skin at 75%** |
| Glazed | **Skin** |
| *Scoop It Up* (card) | **Cut** — became the **Indulge** rest option |
| *Wet Patch* | **Sticky Bandaid** — halves Womb leak |
| Rest site has `HEAL` | **Indulge + Relief**, no `HEAL` |

---

## Order matters

Two backlog items are **load-bearing for things already built**, not polish. Do them first, or the
playtest measures a design nobody has agreed to.

### 1. Precum — blocks the starting deck

*Suckle* is a **starting card** and does not work without it. Spec is design §6.2a:

- Sits in the Mouth. Inside a combat it is indistinguishable from cum — spendable by anything that
  empties the Mouth, and it counts against the 50 ml cap.
- At end of combat it evaporates: **no heal, no Stomach**.
- **Spending order is precum first**, so a precum card is never wasted and bankable cum survives to
  the swallow.

Then *Suckle*: `1 cost, Attack · Mouth. Tease 4 pleasure. Gain 5 ml precum. If this kills, gain
5 ml instead.` Upgrade: 10 ml. **That "instead" is the whole point of the card** — it teaches
precum versus cum on two outcomes of one play, so get the distinction visible in the log and the
HUD.

This also unblocks *Lick Pre-cum* and *Milked Dry*, deferred in slice 2.

### 2. Ass threshold unlocks — load-bearing for the Ass ratio

Design §6.1a. The Strength ratio is now **+1 per 4 ml lost**, and that is tuned for the 200–270 ml
plateau a deck reaches *with* Milking Grip. Without the unlocks a deck sits at 68–100 ml where the
ratio pays +2, which is the inertness the ratio was changed to fix. **Shipping one without the
other is worse than shipping neither.**

- **100 ml** → *Milking Grip* added to the deck free.
- **200 ml** → the **Plug** relic.
- **300 ml** → nothing yet; leave the rung defined but empty.
- Track a **high-water mark**, saved alongside the reservoirs. Reached once, kept for the run —
  the bank leaks, so a live check would yo-yo.
- ***Milking Grip* leaves the draft pool** once it is a threshold reward. **[decided]**

Note `reservoir ass 300` will trip every threshold at once. Fine for testing, remember it when
reading a log.

### 3. Leak-to-Skin and the Scoop rest site — new, design §6.3c

- **All per-floor leak routes to Skin** rather than vanishing, and **Skin itself no longer leaks**
  — the 25%/floor rate is cut. It accumulates until spent. Being gradually covered is intended and
  should eventually be visible on the sprite.
- **Leaked cum arrives at 75%.** Cum leaving the Womb or Ass loses a quarter en route to Skin.
  Output landing on Skin *deliberately* — Freehand kills, unrouted damage — arrives **whole**.
- **With Indulge's ⅔, leaked-and-reclaimed cum is worth exactly half of directly banked cum**
  (`0.75 × ⅔ = 0.5`). That is the point of both numbers: previously a Mouth build banked the same
  total however badly it routed, so routing skill was worth nothing.
- **Ass leak is banded** — 5% of the first 200 plus 25% of the excess. At 300 ml that is **35**, not
  75. Check the existing implementation; the shorthand in §6.2 invited the wrong reading.
- **The Seductress has no `HEAL`.** Her rest site is **Indulge**, **Relief** and Smith:
  - **Indulge** — *"Scoop the cum off your body and eat it."* Recover **⅔ of your Skin** (the rest
    spills): Soothe **1 per ml recovered** and bank that ml into the **Stomach**. No fallback when
    Skin is empty.
  - **Relief** — *"Masturbate furiously to appease your lusts until you pass out."* Remove one Lust
    card. This is the actual rest-equivalent; Indulge is part Rest, part Smith.
- ***Scoop It Up* is cut from the card pool entirely.** Draining goes 11 → 10, total 37 + 2.
- ***Wet Patch* → *Sticky Bandaid***: halves **Womb** leak. The Womb's leak is a bounded 0–10 ml
  sawtooth that resets at each locked threshold and is the same whether filling or carrying — so
  the relic is a flat, permanent acceleration of the progression track rather than a phase-specific
  one.

**Watch in the playtest — this is the most uncertain thing in the slice.** Removing `HEAL` makes
**Climax the primary heal**, which costs a Lust card and could make runs very short on a 3-Lust
clock. How far Skin accumulation offsets that is **not derivable at a desk** — it depends on how
full the reservoirs actually run, which nothing has measured.

Instrument it precisely: Skin at every rest site, what Indulge recovered / healed / banked, which
option was taken, and climaxes per act.

**Do not tune Indulge down further on the strength of this run.** It has already been nerfed twice
(the 75% and the ⅔), and its only advantage over Relief is the **Stomach** — which is unbuilt, so
there is currently nothing to weigh it against. Removing a Lust card is worth a full heal
regardless of current HP, so Relief dominates on health alone.

### 4. Max HP 20 and the Stomach track — new, design §6.5

- **Max HP is 20**, down from 65. `20 × 4 climaxes ≈ 80` effective, Ironclad's pool; ~51 inside one
  fight once Sensitive compounds. **Max HP and Block are each worth ~4× vanilla** here.
- **Rescale every HP cost** per the table in §6.5: 20→9, 12→6, 10→4, 8→3, 6→3, 4→2, 3→2, 2→1. The
  Plug becomes **2 HP** per floor and **1 HP** per turn. Card text is already updated in v23.
- **The Stomach grants +1 Max HP per 10 ml banked.** This is Draining's entire progression track,
  so the Stomach itself is **in scope this slice** — it was deferred in slices 2 and 3.
- **The Mouth banks to the Stomach at the end of every combat, by default.** *Gulp* does not enable
  banking — its "swallow **now**" buys the timing, the heal and the Sensitive removal. Build the
  default first; Gulp is a card on top of it.
- **Milestones:**
  - every **×50** — the milestones *ending* in 50: **50, 150, 250, 350**. Each grants **+5 ml
    precum at combat start**, cumulative, so 350 ml of Stomach means 20 ml. Not every fifty, and
    not the ×00s.
  - **100 ml** — the Mouth **also** swallows whenever it reaches 50 ml. This is **in addition to**
    the end-of-combat swallow, which is unchanged. Uncaps throughput; takes nothing away.
  - **200 ml** — Indulge stops spilling (100% reclamation).
  - **300 ml** — **precum counts as cum** and banks instead of evaporating.
- **Halve Max HP gain from relics**, carried on the character's starting relic. Food relics are
  worth 4× here and would otherwise outclass every other pick in the game.

**Only Max HP is worth 4×** — it is refilled four times. **Block, healing and damage are absolute
and unchanged.** The character is not more fragile than an 80 HP one, it is more granular: the same
pool in four chunks with a Lust card owed at each boundary. Log climaxes and Reliefs per act as
part of the normal per-floor instrumentation; **do not draw balance conclusions from it** — Gorim
reads that himself.

### 5. The rest of the backlog

- **Ass Strength ratio → +1 per 4 ml lost.** Verify against §6.1's table: at 200 ml, Clench 2×10,
  Rut 3×11, Overflow 11 to all, Blowout 4×31 = 124.
- **The Plug's 10% spill counts as ml lost by the card that pulled it**, so it grants Strength
  (§7.3). Without this a plugged Ass card pays +1 energy *and* double drain for the same damage.
- ***Let Go* renamed *Seek Relief***; upgrade lowers the HP cost 20 → 10 rather than drawing more.
- ***Abandon* → *Forced Orgasm***: 0 cost Uncommon, Exhaust. Climax with no heal and no Lust card,
  draw 2, gain 1 energy. Upgrade: draw 3.
- ***Afterglow*** gains its engine: each turn, draw 1 and gain 1 energy **per stack of Sensitive**.
  Upgrade cuts cost to 0 rather than raising Block.
- ***Glazed* is renamed *Skin*** everywhere — tag, reservoir, HUD, card text, logs.

---

## 6. Pregnancy — staged gestation

Design §6.3b. This is the mechanic the whole character was built around and it has never existed.
**Read §6.3b in full** — it changed substantially in v3.2 and the staged model is not what earlier
drafts described.

**Conception:** the Womb crosses 100 ml, **checked at the end of combat, not on crossing.** You are
not pregnant during the fight that earned it — no lactation mid-combat, no Sensitive stack landing
on that fight — but you *are* pregnant on leaving the room. Spending the Womb back below the
threshold before the bell prevents conception; that is intended agency, not a bug to close.

**Stages.** Stage N at **N × 100 ml**. The Womb **keeps filling** while you carry — further intake
advances the stage rather than conceiving again. Per stage:

| | Stage 1 | Stage 2 | Stage 3 |
|---|---|---|---|
| Sensitive at combat start | 1 | 2 | 3 |
| Milk shuffled in per turn | 1 | 2 | 3 |
| Sigils on birth | 1 | 2 | 3 |

**Uncapped** — nothing stops Stage 4 at 400 ml. Pushing further is meant to be possible and
dangerous rather than blocked.

**Leak applies only above the locked threshold.** `N × 100 ml` is committed and cannot bleed away;
the Womb's 10% per floor applies to the excess. At Stage 2 holding 250 ml, 200 is locked and 5 ml
leaks.

**Birth:** at any rest site, free action. Gain N Sigils. **The Womb empties completely and the
excess is wasted** — on the floor, not to Skin, not to the Stomach. This is the only place in the
design where output is destroyed, and it is what creates natural delivery points at exactly
N × 100: cashing out at 299 ml wastes 99, at 300 wastes nothing.

**Lactating** is a separate state: it produces the Milk, begins with pregnancy, and runs **one
further combat after the birth** at the stage you delivered at. Conceiving during the tail
re-enters Pregnant and lactation never lapses.

### Milk

`0 cost. Attack. Tease 4 pleasure. Soothe 2. Exhaust.`
`1 Sigil: draw 1. · 3: Tease 7 instead. · 5: draw 2 instead. · 7: does not Exhaust.`

- **Shuffled into the combat draw pile, never the master deck.** Nothing persists between fights.
  The pile is deliberately weaker than the hand: Milk competes with your real cards for draws.
- It rides the **standard Sigil ladder** — no new scale. At tier 4 the existing 0-cost rule ("sheds
  its non-energy cost") means Milk **stops Exhausting** and accumulates. That is intended.
- **It heals; it must not Block.** That distinction is load-bearing — healing cannot prevent a
  one-shot, which is what makes a high stage dangerous rather than merely slow.

### Not in scope

- **"If pregnant" cards.** Undecided. If they land later, they go on their own axis — a card is
  tier-gated or pregnancy-gated, never both.
- Twins, multiple simultaneous pregnancies, birth choices, orbs as a channel/evoke loop.
- The lexicon converter (design §9a) — specified but explicitly **not** this slice.
- Voice, art, audio.

**Moved *into* scope since the earlier draft of this brief:** the **Stomach** and **Gulp** (the
Stomach is now the Max HP track), and with precum built, **Lick Pre-cum** and **Milked Dry** are
unblocked. Only **Wet Work** stays deferred, on the full spend order — build that too if it is
cheap, and the card pool is then complete.

**Sticky Bandaid** (halves Womb leak) is in scope: it is one rule, and it is the Pussy archetype's
only relic.

---

## Also carried forward

From slice 3, still untested and worth clearing in the same run: tier-4 repeats (`sigils 7`, Take
Him In plays twice, Breed Me! three times), the Ass rule at a full bank (`reservoir ass 200`, then
Blowout or Rut — **not** Clench), *Consummate* shrinking the orb count, a minion banking nothing,
and *Spit*'s `{Portion}%` and *Ritual Mark* going Innate on upgrade.

**Keep the per-floor instrumentation** and extend it: pregnancy state, Milk generated, high-water
Ass mark, and which thresholds have fired. The whole reason this slice is worth playing is that the
numbers come back readable.

**Screenshots to `docs/spike-screenshots/`** as files, not the clipboard.

---

## Output

`docs/SEDUCTRESS_SLICE4_RESULTS.md`, in the same shape as slice 3 — attributed decisions, what was
built, what was deferred and why, bugs found, and anything the design docs get wrong.
