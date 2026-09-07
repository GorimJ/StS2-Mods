# The Seductress — slice 2: a playable run

Handoff prompt for a fresh code session. Follows `SEDUCTRESS_SPIKE_RESULTS.md`, which answered the
four engine questions. Read that first — it contains the APIs, the traps, and the build/run loop.

---

## The goal, stated as a test

**Gorim can complete act 1 with a drafted deck, and the run can be lost.**

That is the whole target. Not more systems — a run that survives contact with a player. The spike
proved four mechanisms in isolation; it produced a build that is unwinnable over any distance,
because the Lust clock is 3 with no way to clear it.

Nearly every number still open in `SEDUCTRESS_DESIGN.md` — is a 3-Lust clock right, does relief's
opportunity cost actually bite, is Sensitive-scaled self-damage thrilling or miserable, is Grind's
block-for-ammunition trade worth taking — cannot be settled at a desk and can be settled by one
act of real play. Building further systems before that means building them on guesses.

Read `docs/SEDUCTRESS_DESIGN.md` (v2.1) and `docs/SEDUCTRESS_CARDS.md` (v10). **Do not edit
either.** They are input. If the build proves something in them wrong, report it — the decision
register is Gorim's.

---

## What to build

### 1. Rest-site relief — the unblocker

A `CustomRestSiteOption` that removes **1 Lust card** instead of healing. §5 of the design has the
reasoning. This is first because without it the game cannot be finished.

No per-act cap. The opportunity cost is the limiter: for this character HP *is* arousal, so giving
up the heal means entering the next fight already aroused.

### 2. Per-floor leak

Nothing decays yet. Rates are in §6 — read them there rather than from memory. Leak is what makes
the reservoirs a flow instead of a monotonic counter, and it is a precondition for the Plug and
Wet Patch relics ever meaning anything.

### 3. Max HP and the starting state

Set from §12's proposed values. Flag in the report that these are provisional.

### 4. Enough of the card pool to draft

Card rewards offer three choices; with nine cards implemented there is no draft. Work through
`SEDUCTRESS_CARDS.md` and implement everything that does not depend on an unbuilt system.

**Known dependency, handled by a scaffold.** Most of Breeding's cards gate on Sigil count
(`2 / 4 / 6`), and Sigils come from pregnancy, which is out of scope. Rather than build Conception:

- **A starting relic granting 2 Sigils**, given to the character at run start. Tier 1 is then live
  from floor 1, so every Breeding card reads and functions.
- **A dev console command to set the Sigil count** (e.g. `sigils 6`). Cheap, and it is the only way
  to exercise tiers 4 and 6 without playing a full run to reach them. Do this as well as the relic,
  not instead of it.

Implement the gating checks and the counter — the counter already persists from the spike. Do
**not** build Conception, pregnancy or orbs.

**Both are scaffolding and both come out later**, alongside `Taste Test`. Note the consequence in
the report so nobody mistakes the played run for evidence on this point: with 2 Sigils from floor
1, Breeding is at roughly its act-2 power throughout act 1, so **this run cannot tell us whether
Breeding's intended early-game weakness feels right.** By design Sigils arrive from elite and boss
pregnancies and the archetype should feel thin early; that remains untested.

Defer any card whose text needs Stomach, precum, the spend order, or the full Ass Strength rule,
and list what you deferred.

### 5. Instrumentation — do not skip this

The point of the slice is to learn from a played run, and a run you cannot review afterwards
teaches almost nothing. Log one line per floor with: arousal at entry and exit, climaxes so far,
Lust held, each reservoir, Sigils, intake gained, leak lost, and whether relief was taken.

Prefix `[Seductress]` as before. This turns one playthrough into data instead of an impression,
and it is how every open number in the design gets answered.

### 6. Housekeeping

- Delete `Taste Test`. The tag question has its answer and does not need a witness.
- Mark the Sigil relic and the `sigils` console command clearly as test scaffolding, so they are
  removed when Conception lands rather than surviving into the real mod by inertia.
- Invert the **Poison and Doom bar overlays** (risk 2a). Enemies apply Poison to you regardless of
  what this character's deck contains, so this is nearer-term than it sounds.

---

## Out of scope

- Conception, pregnancy, orbs, the Sigil passive beyond a counter and its thresholds
- The full Ass tag rule as real Strength, and multi-hit scaling
- Stomach, `Gulp`, the swallow path, precum, the Mouth generation exception, the spend order
- The Plug and Wet Patch relics
- Voice hooks, audio, art, portraits
- Any balance pass — that is what the played run is for

---

## Notes carried forward

- **Screenshots must be written to a file path.** The last session captured them to the clipboard
  only and they were lost. Put them in `docs/spike-screenshots/`.
- `dotnet publish Seductress/Seductress.csproj`; Godot's export prints exit code -1 and the `.pck`
  is still correct. Launch from Steam (`steam://rungameid/2868840`) — launching the exe directly
  fails Steam init and loads no mods.
- Do not construct a `CustomSingletonModel` yourself; let `ModelDb` own it.
- Card text: paired colour tags only, `{Energy:energyIcons()}` for the energy icon.

---

## Output

Append to `docs/SEDUCTRESS_SPIKE_RESULTS.md` or start `docs/SEDUCTRESS_SLICE2_RESULTS.md`: what
was built, what was deferred and why, any card text that had to change to be implementable, and
anything the design docs get wrong.

Then tell Gorim it is ready to play, and what to watch for.
