# The Seductress — art requirements

Derived from `SEDUCTRESS_DESIGN.md` v3.10 and `SEDUCTRESS_CARDS.md` v24. Written to be handed to an
artist. Every item exists because a mechanic needs it.

**[Gorim]** = his decision · **[Claude-Cowork]** = this session's, for review · **[open]** = neither.

**Not in this document:** card portraits. **[Gorim]** does those himself with AI generation. That
removes 45 of the ~90 assets an earlier draft counted, and it removes the largest item by volume.

---

## 0. The cost driver: which layers are pose-matched

**[Gorim] The sprite is layered**, not composed — the state axes multiply too fast otherwise, and
Purple Forest cannot supply layers anyway. Commissioning is the route.

The question that sets the budget is **not** how many layers. It is **how many animations each
overlay has to line up with.**

| Approach | Overlay sheets | Looks |
|---|---|---|
| Every layer pose-matched to every animation | **~240** | Correct always |
| Pose-matched to **Idle only** | **15** | Correct at rest, absent mid-action |
| Pose-matched to **Idle + Relaxed + Climax** | **~45** | Correct wherever you actually look |

**[Claude-Cowork] The middle tier is the recommendation.** Idle is what is on screen for the large
majority of a combat; Relaxed carries the map and rest sites; Climax is the character's defining
moment and the one you *want* covered in cum. Attack and Hit are brief and busy, and an overlay
dropping out for four frames of an attack will not read as a bug.

**Ask the artist to deliver layers as separate PNG sheets with identical frame counts and frame
timing to the base**, so Godot can drive several `AnimatedSprite2D` siblings from one animation
name. That requirement is cheap to state up front and expensive to retrofit.

---

## 1. Player sprite — base body

### 1a. Animations

| Animation | Source | Notes |
|---|---|---|
| Idle | engine | The default; on screen most of the time |
| Relaxed | engine | Map and rest sites |
| Hit | engine | |
| Cast | engine | Powers |
| **Dead** | engine | **[Gorim] Does play** — on gaining the 4th Lust card the run ends. Looks different from a normal death |
| revive | engine | Unused, still required |
| **Climax** | design | 3–4 times a run minimum. **Cannot reuse `Dead`**, which now has its own job |
| **Gulp** | design | **[Gorim]** Mid-combat swallow |
| **Attack ×4** | design | **[Gorim] One per tag** — Skin, Mouth, Ass, Pussy |
| **Transition ×4** | design | **[Gorim]** Played when the *Continuing* mode changes; one into each mode |

**16 base animations.** The four attacks and four transitions replace what the template treats as a
single `Attack`, and they are the largest single increase over a stock character.

**[open] Are transitions directed?** Four "into mode X" transitions is 4 sheets; every ordered pair
is 12. Undirected is almost certainly enough.

### 1b. State layers

| Layer | States | Driven by | Notes |
|---|---|---|---|
| **Cum / Skin** | **clean + 4, possibly more** | Skin ml | **[Gorim]** Must **escalate and drip**. Animated, not a static decal — see 1c |
| **Belly** | ~6 bands | **[Gorim] Shared** by Ass inflation and pregnancy stage | One layer, two sources |
| **Breasts** | 4 | **[Gorim]** Scale with pregnancy stage | Replaces the binary "lactating" layer from the earlier draft |
| **Mouth fill** | 3 | Mouth ml | **[Gorim]** Visible filling |
| **Plug** | 2 | Plug relic | Low priority; also readable from the relic bar |

**[open] Belly conflict.** Inflation and pregnancy both push the belly and can be simultaneous — a
Stage 2 pregnancy at 350 ml Ass. Do the bands **add**, or does the layer take the **higher** of the
two? Adding is more accurate and needs more art; taking the max is free.

### 1c. The dripping cum animation

**[Gorim] Required despite being mechanically inaccurate** — nothing leaks in combat — because it
carries too much of the theme to skip.

This is what makes the cum layer expensive: it is not one overlay per band, it is **an animation per
band**, escalating in volume and speed. It also wants to run over `Idle` continuously rather than
being a pose-matched still, which is the one layer where a free-running loop may look *better* than
strict frame matching.

**[Claude-Cowork]** Worth asking the artist for the drip as a **loop that does not need frame
matching**, on the argument that dripping is independent of the body's pose. If that works it
collapses the most expensive layer to 4–5 sheets regardless of the tier chosen in §0.

---

## 2. UI — on screen (combat)

| Asset | Count | Notes |
|---|---|---|
| Reservoir icons | 4 | Skin, Mouth, Ass, Womb — currently a debug text readout |
| Reservoir fill bar | 1 reusable | Or a fill mask on the icon |
| Stomach icon | 1 | Counter, and the Max HP track |
| **Sigil orb** | 1 | Currently a vanilla Lightning orb tinted pink via `Modulate`, which darkens as it tints. Its **tooltip icon cannot be tinted at all** and still reads as lightning |
| Sensitive status icon | 1 | Stacking, on the player |
| Leg-Locked status icon | 1 | On enemies |
| Continuing mode indicator | 4 | Which hole the next tagged attack routes to |
| Pregnancy stage indicator | 1 + states | Or read it off the belly layer |
| Lust / death clock | 1 | 3 is the threshold; the run's most important number |
| Precum distinction | — | Shares the Mouth readout. A colour or a second pip, not a second icon |
| Relic icons | 3 | Plug, Sticky Bandaid, starting relic |
| Rest-site option icons | 2 | **Indulge**, **Relief**. A missing icon previously left the button wearing vanilla's "Dig" face |

**≈19 pieces.** Small, and mostly static — the cheapest category here and the one that most improves
legibility per unit of work.

---

## 3. UI — status / meta screens

| Asset | Count | Notes |
|---|---|---|
| Character select portrait | 1 | Plus name plate |
| Character select splash | 1 | Optional |
| Run-end screen | 1 | The run ends at 3 Lust, not 0 HP — vanilla's death screen is thematically wrong |
| Stomach track panel | 1 | Milestones at ×50 / 100 / 200 / 300 — a ladder the player cannot otherwise see |
| Sigil detail panel | 1 | Count, effect, exhausted state |
| Pregnancy panel | 1 | Stage, ml, what the next threshold gives |

**[Claude-Cowork]** The Stomach and Sigil panels are the two where a bare number genuinely fails.
The rest can be text.

---

## 4. Sex and related animations

Distinct from §1's sprite states: these are **events**, played once and returning to the previous
state.

### 4a. Required

| Event | Frequency | Why |
|---|---|---|
| **Climax (player)** | 3–4+ per run | The defining event and the death clock |
| **Conception** | 1–2 per act | **[Gorim]** explicitly wants an animation or image at the end-of-combat check |
| **Birth** | 1–2 per act | The Sigil payoff, and the end of a multi-floor state |
| **Indulge** | most rest sites | Scooping and eating |
| **Relief** | most rest sites | The other option |
| **Enemy climax** | **every fight** | Enemies climax and pass out rather than dying. The most-seen event in the mod — though it can be an effect over the vanilla enemy sprite rather than bespoke art |
| **Gulp** | frequent in Mouth decks | Listed in §1a as a base animation; noted here so it is not counted twice |

### 4b. Optional

| Event | Notes |
|---|---|
| Intake per hole | Three variants; the routing is already legible from the HUD and the attack animations |
| Plug insert / remove | The removal is the archetype's burst turn |
| Sigil creation | At the rest site, on birth |
| Per-stage pregnancy idles | Distinct idles rather than a swapped belly layer |

---

## 5. Volume

| Category | Count |
|---|---|
| Base body animations | **16** |
| State-layer sheets, Idle-only tier | 15 |
| State-layer sheets, **recommended tier** | **~45** |
| State-layer sheets, everything | ~240 |
| Combat UI | ~19 |
| Status screens | 6 |
| Event animations, required | 6 |
| Event animations, optional | 4 |

**Recommended target: ≈90 pieces**, of which 61 are sprite work and the rest is UI. Card portraits
are excluded and handled separately.

---

## 6. Division of labour

**[Claude-Cowork]** The commission does not have to cover all of it, and the split is not obvious.

| Best commissioned | Best done in-house |
|---|---|
| Base body + its 16 animations — the hard part, and everything else has to match it | **Cum / drip layers.** Repetitive, low-skill relative to a body, and the layer most likely to want iterating after playtest |
| Belly and breast bands — anatomy that must stay on-model | **UI icons.** Small, static, and design-led rather than art-led |
| Climax, conception, birth | **Sheet assembly and Godot wiring** — mechanical, and needed whoever draws |

**On learning it:** the highest-leverage skill here is not drawing. It is **assembly and rigging** —
laying out sheets, keeping frame counts aligned across layers, and wiring several
`AnimatedSprite2D` siblings so they play as one. That work is needed no matter who draws, it is the
part a commission will not do for you, and it is a much shorter learning curve than figure drawing.
Overlay authoring (drips, splatter) is the natural second step, because it is repetitive and
forgiving.

---

## 7. Open questions

1. **Which pose-matching tier?** §0. Sets the budget more than any other choice.
2. **Can the drip loop run free** rather than pose-matched? Collapses the most expensive layer.
3. **Belly: add inflation and pregnancy, or take the higher?**
4. **Are mode transitions directed** (12) or undirected (4)?
5. **Cum bands: 4 or more**, and at what Skin values? The thresholds need the instrumented run.
6. **Sigil orb art** — the tinted Lightning orb is a placeholder that darkens as it tints, and its
   tooltip icon cannot be tinted at all.
