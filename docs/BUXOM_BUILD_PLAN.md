# Buxom build plan (2026-09-04)

Full write-up with options/pros/cons: Cowork artifact "Buxom Build Plan" (claude.ai artifact gallery).
Research behind it: `docs/research-2026-09/`.

## Facts
- STS1 original: 92 cards, 15 relics, 27 powers, 6 potions, 4 events (`reference_BuxomMod_JavaSource`).
- Port so far: 5 cards, 1 relic, 2 powers (Buxom/Capacity), gauge HUD patch, static combat PNG (`Buxom Mod Port`, old csproj layout).
- Art: `LehmanaSprite8` (DragonBones .dbproj → Spine-3.3-style JSON, `reference_BuxomMod_Assets/.../images/char/character/`) is ONE rig, 21 bones / 21 slots / 34 attachments, holding every bust size + exposed variants as attachments, with idle/idle2/idle3/idle4, *_ex, grow, expose animations. Use it as the only source.
- Game: Spine 4.2.43 via libspine_godot GDExtension, wrapped in C# by `MegaCrit.Sts2.Core.Bindings.MegaSpine` (MegaSprite, MegaSkeletonDataResource.FindSkin, ...). BaseLib 3.4.5 also accepts Godot AnimationPlayer/AnimationTree animations named idle/attack/cast/hurt/die.

## Decisions
1. Library: stay on BaseLib (recommended); RitsuLib is the alternative (VisualCueSet, secondary-resource UI).
2. Animation: Godot cut-out from the existing rig (recommended, free); Spine Essential $69 + converter; or AI-video sprite frames.
3. Hosting: Workshop SFW build + full build elsewhere, or Nexus-only. Gate exposure behind a config flag from day one.

## Phases
0. Port shell onto the template layout (Buxom/ like the other five; CustomCharacterModel; static PNG) — 1 evening, unattended.
1. Gauge rules re-derived from the Java powers; single size-threshold → visual function; console script to drive Buxom — 1–2 evenings.
2. Content in batches: cards 4×~20 by dependency, then relics+potions, then events; keep PORT_LEDGER.md — 3–4 evenings.
3. Animation: extend generate_godot_scene.py to bake DragonBones timelines into AnimationPlayer; per-slot texture table for sizes; draft attack/cast/hurt/die keyframes (timings from vanilla skeleton; AttackAnimDelay 0.15, CastAnimDelay 0.25, DeathAnimTime 1.5); AnimationTree hook-up; judge; Spine Essential as fallback — 2 weekends.
4. Ship: select-screen assets, config gate, sts2-mod-uploader (tags permanent, image ≤1 MB, BaseLib dep 3737335127).

Expect one API-diff pass when public reaches 0.111.
