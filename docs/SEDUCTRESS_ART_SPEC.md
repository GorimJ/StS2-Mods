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

**[Gorim] Belly bands are additive, capped at 4, and neither source can reach the cap alone.**

```
pregnancy  = min(stage, 3)
inflation  = min(floor((Ass - 200) / 100), 3)     # 100 ml over ≈ one stage
belly band = min(pregnancy + inflation, 4)
```

So Stage 1 at 300 ml Ass reads as band 2; Stage 3 at 300 ml reads as band 4 and is capped there.
**Five states: flat + 4.** Requiring both sources to reach the cap means the fullest belly in the
game is only available to a player pushing both tracks at once, which is the right thing to reserve
for.

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

## 6. Production pipeline — cut, inpaint, rig, render

**[Gorim]** The proposal is to take a static character, split it into a sprite, inpaint variants,
and separate those into layers. **It works, with one step reordered — and the reorder changes the
budget in §0 substantially.**

**Do not inpaint whole images and diff them into layers.** Inpainting bleeds into surrounding
pixels — lighting, colour, edges — so extracting a clean transparent overlay from a variant needs
hand-masking, per variant, per frame. That is the step that would make this unworkable.

**Instead, cut first and swap parts.**

| Step | What | Notes |
|---|---|---|
| 1 | One static character, neutral pose | Commissioned or generated |
| 2 | **Cut into parts** — head, torso, breasts, upper/lower arm, thigh, shin | Inpainting is *good* at the hard part: painting in what sits hidden behind an arm or the torso |
| 3 | **Inpaint alternative parts** — pregnant torsos per band, breasts per stage, cum-covered variants | Per part, in a fixed pose. Not per frame |
| 4 | **Rig and animate once** | Nested `Sprite2D` under an `AnimationPlayer`; no mesh deformation needed |
| 5 | **Swap parts, render the rig to a sheet** | Batch, per state combination |
| 6 | Ship the sheets | |

**Why this matters more than the tier choice in §0.** Animation is authored **once** and reused
across every state, so 240 sheets stops being 240 pieces of art and becomes a batch render. The
cost becomes `1 rig + N part variants` — and the pose-matching tiers largely stop being a budget
question at all.

**Two constraints.**

- **Ship frames, not the rig.** BaseLib's `CustomAnimation` resolves AnimationTree → AnimationPlayer
  → **AnimatedSprite2D**, so a live cutout rig may not be drivable through it. Rendering to sheets
  sidesteps that entirely. *Puppet to author, frames to ship.*
- **Cutout animation has a look** — limbs rotate rather than redraw, so foreshortening and crossing
  limbs are where it shows. That look is essentially StS1's, so it suits the target.

**[Claude-Cowork] The render-to-sheet script is the piece worth automating**, and it is ordinary
Godot tooling rather than art: walk the part-variant combinations, set the visible parts, step the
AnimationPlayer frame by frame, capture the viewport, pack into a sheet with a matching
`SpriteFrames` resource. Written once, it makes every later state addition free.

**What this changes about commissioning.** The brief becomes *"one character, delivered as separable
parts in a neutral pose, plus alternate parts for the listed states"* — rather than *"N animated
sheets"*. That is a smaller, cheaper and far more common commission, and it keeps every future state
in-house.

---

## 7. Division of labour

**[Claude-Cowork]** The commission does not have to cover all of it, and the split is not obvious.

| Best commissioned | Best done in-house |
|---|---|
| Base body + its 16 animations — the hard part, and everything else has to match it | **Cum / drip layers.** Repetitive, low-skill relative to a body, and the layer most likely to want iterating after playtest |
| Belly and breast bands — anatomy that must stay on-model | **UI icons.** Small, static, and design-led rather than art-led |
| Climax, conception, birth | **Sheet assembly and Godot wiring** — mechanical, and needed whoever draws |

**On learning it:** the skill that matters is **rigging and rendering**, not drawing — steps 4 and
5 above. That work exists whoever draws, no commission will do it, and it is a far shorter curve
than figure drawing. Part cutting and inpainting (steps 2–3) is the natural second thing, because
it is repetitive and forgiving.

---

## 8. Open questions

1. **Does the §6 pipeline hold?** If it does, the pose-matching tier in §0 mostly stops being a
   budget question and the commission gets much smaller.
2. **Can the drip loop run free** rather than pose-matched? Still worth asking; it is the one layer
   where a free loop may look *better*.
3. **Are mode transitions directed** (12) or undirected (4)?
4. **Cum bands: 4 or more**, and at what Skin values? The thresholds need the instrumented run.
5. **Sigil orb art** — the tinted Lightning orb is a placeholder that darkens as it tints, and its
   tooltip icon cannot be tinted at all.
