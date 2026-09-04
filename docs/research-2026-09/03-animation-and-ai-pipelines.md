# 2D animated character for Godot 4.5 (Spine 4.2 runtime or Skeleton2D) — pipeline survey (2026-09-04)

Compiled by a research subagent. GitHub tree pages and some raw READMEs were blocked; Godot docs bodies did not load (statements about them marked *[docs not read]*).

## 1. Getting Spine-3.x / DragonBones data into spine-godot 4.2 without a Spine licence

**What `libspine_godot` implies (read).** The GDExtension binaries are named `libspine_godot.<platform>.<arch>.<ext>` — the game uses the **GDExtension**, not the engine module. GDExtension: no AnimationPlayer timeline integration and no C# bindings (Esoteric blog/forum: "Building a C# binding generator for GDExtensions is a bit outside of our scope"). From a mono build you drive the nodes dynamically (`ClassDB.Instantiate("SpineSprite")`, `.Call("get_animation_state")`) — which is exactly what the game's `MegaCrit.Sts2.Core.Bindings.MegaSpine` wrappers do (verified locally in decompile_v0107: `MegaSprite`, `MegaSkeletonDataResource.FindSkin/GetAnimationNames`, `MegaAnimationState`).
- Godot 4.5 builds: the CI workflow "Build spine-godot GDExtension (All Godot 4.x versions)" is active; a community fork ships 4.4.1/4.5-stable builds for Spine 4.1 data (https://github.com/ClockAED/spine-godot-4.1). One unresolved forum report of a 4.5.1 mono project loading the extension but not importing `.atlas/.skel` (https://forum.godotengine.org/t/spine-gdextension-loads-but-godot-wont-recognize-atlas-or-skel-files-godot-4-5-1-mono/140830).
- Data version must match runtime version; the 4.2 branch README: "works with data exported from Spine 4.2.xx". Runtime accepts `.json` (as `.spine-json`) or `.skel` plus `.atlas`.

**Licence flag (read).** The Spine Runtimes License says each user of the runtimes must hold a Spine Editor licence; Essential **$69**, Professional **$379** (https://esotericsoftware.com/spine-purchase). Essential lacks meshes/IK/constraints but has bones, skins, JSON/binary export. **Trial cannot export** (Esoteric forum, policy).

**What DragonBones exports (read).** DragonBones 5.6.x exports **Spine 3.3-format JSON** (Defold forum threads). Pitfalls: slot/skin naming mismatches, no non-ASCII names, no negative scale, one image per slot. (Buxom's `skeleton.json` is exactly this: keys `skeleton/bones/slots/skins/animations`, no `"spine"` version string, 18 bones, 17 slots, `idle` + `animtion0`.)

**Community converter (read).** https://github.com/wang606/SpineSkeletonDataConverter — C++ CLI, converts skel/json across **3.5–4.2** (auto-detect input, `-v 4.2.11` target, `--remove-curve`), atlas downgrade tool; latest v3.8 July 2026, actively maintained. **PolyForm Noncommercial.** 3.3 is below its floor; bump the `"spine"` version string to 3.5.x and check that bone/slot/timeline keys parse (untested). Also https://github.com/BastienGimbert/SkelToJson (4.2/4.3 skel→json) and https://himerik.github.io/Spine-tools/ (browser, 3.8.75 JSON; frames→Spine animation converter).

**Assessment.** Free path = DragonBones → Spine 3.3 JSON → patch to 3.5 → converter → 4.2 JSON → test in spine-godot. Paid path = $69 Essential: Import Data, re-export 4.2 JSON. Skins in Spine map directly to "power-level body variants".

## 2. Godot-native path

- https://github.com/Daylily-Zeleen/Godot-DragonBones — GDExtension for Godot 4.2+, MIT, DragonBones Pro 5.6 data, `DragonBonesArmatureView`, multiple skins. Last release v2.0.2 (2024). ~1½ years stale against a moving GDExtension ABI; needs rebuild against godot-cpp 4.5. Adds a second animation runtime to a game already shipping Spine — not recommended.
- Spine/DragonBones JSON → Skeleton2D scene converters: effectively none current (mjtorn/spine-to-godot, jkb0o/godot-spine-importer are dead; godot#4312 never landed). For 18 bones + 17 slots, a ~150-line tool that reads the JSON, builds `Skeleton2D/Bone2D` + `Sprite2D` per slot, and bakes bone timelines into `Animation` tracks is a realistic 1-day job. (Gorim's `generate_godot_scene.py` already builds the Skeleton2D/Bone2D hierarchy; only the animation-track baking is missing.)
- Godot 4.5 built-ins *[docs not read]*: `Skeleton2D`, `Bone2D`, `Polygon2D` with bone weights, `RemoteTransform2D`, `AnimationPlayer`, `AnimationTree` state machines; 2D IK via `SkeletonModificationStack2D` is **marked Experimental** in 4.5. Variants = change `Sprite2D.texture`/`Polygon2D.texture` per slot.

## 3. AI-assisted tools (2025–2026)

**(c) Tools that emit Spine rigs directly**
- **God Mode AI — Spine Animation** (https://www.godmodeai.co/ai-spine-animation) — single transparent PNG (700–1024 px) in; auto-splits parts, infers skeleton, retargets ~400 clips; exports `skeleton.json + atlas + png` "in every Spine version from 3.5 to 4.2", plus alpha WebM/GIF. $12/20 credits, $19/mo. Community: outputs "fall notably short" of demos, rig+animation bundled, download issues (BigGo, Oct 2025). Most weekend-friendly option; expect wobbly cut-out motion and per-variant re-splitting.
- **GenielabsOpenSource/spine-animation-ai** (https://github.com/GenielabsOpenSource/spine-animation-ai) — a Claude "skill" (Python/OpenCV) that positions separated part PNGs, builds bones, writes idle/walk/run/attack/jump from **preset templates** into Spine JSON + atlas; free, PolyForm-NC. Fits inputs like Buxom's parts exactly; motion is template-based; output Spine version unverified.
- **Spine2d.net** — agent-generated `.skel/.atlas/.png`; pricing unknown. **Layer.ai Spine Component Generator** — parts only, no rig.

**(a) Single-illustration auto-animate / layer splitting**
- **See-through** (SIGGRAPH 2026, Apache-2.0, ~8–16 GB VRAM) — one anime image → ~23-layer PSD with inpainted occlusions; no rigging. Useful for making variant part sets consistent.
- **Meta Animated Drawings** — MIT, **archived Sept 2025**; BVH retargeting to a detected humanoid; outputs video/GIF, not a rig; janky for anime proportions.
- **Adobe Character Animator** — CC subscription; Adobe Animate EOL March 2026. Not recommended.
- **Image-to-video → sprite sheet.** Kling 3.0 ≈ $0.28–0.61 per 5 s clip; Wan 2.7 ≈ $0.10/s; Seedance 2.0 best quality/cost at 720p (Sorceress test). Reference pipeline https://github.com/chongdashu/ai-game-spritesheets (author: "image gen ≈ 20% of the work, the other 80% is the pipeline"). Frame extraction/chroma-key: DoSprite (free, browser, ≤512 px); Sorceress Auto-Sprite v2 ($49, local GPU, idle/attack/cast/death presets, Godot JSON manifest). Caveat: at ~250 px, i2v models drift in outline/colour; hands/weapons morph; idle and hurt acceptable, attack/cast need 2–4 generations per usable 12-frame cycle.
- **EbSynth** — keyframe propagation; only useful downstream of a driving video.

**(b) AI inbetweening for cut-out rigs.** Nothing credible outputs keyframes for an existing Bone2D/Spine rig; in practice the "AI" is an LLM writing the AnimationPlayer keyframes.

**(d) Generative sprite-sheet SaaS** — Ludo.ai (transparent PNG sheets + JSON, ≤4 s/64 frames, 30 free credits), Spritesheets.ai (attack/hit/cast presets, from $3.50/mo), PixelLab (pixel-art only — wrong style). All i2v under the hood; fidelity at 250 px is the risk.

## 4. Sprite-sheet route in Godot

`AnimatedSprite2D` + `SpriteFrames`. Budget: 5 anims × ~16 frames × ~2× (500 px tall) ≈ 80 frames ≈ **~48 MB uncompressed RGBA** VRAM — fine for a hero on desktop; VRAM-compressed cuts ~4× with alpha-edge artefacts. Keep sheets ≤4096 px a side. Variants = second `SpriteFrames` per variant (multiplies asset count).

## Ranked recommendation (from the subagent)

1. **Spine Essential ($69) + AI motion** — import the DragonBones→Spine JSON, tidy names, export 4.2; God Mode AI for attack/cast/hurt/die reference or whole rig. Works with `libspine_godot` today; skins solve variants. Cons: AI motion mediocre; two rigs may not line up.
2. **Godot-native cut-out, LLM-written keyframes** — importer from the Spine JSON to `Skeleton2D` + `Sprite2D` per slot + `AnimationPlayer`; have an LLM generate the 5 animations as keyframe tables; tune in the editor. Free, variants = texture dictionary, no licence, AnimationTree integration. Cons: a day of tooling; stiff results unless iterated; no mesh deformation.
3. **Sprite sheets from image-to-video** — Seedance/Kling/Wan → DoSprite/Auto-Sprite → `SpriteFrames`. Most "animated-looking" for cast/die; zero rigging. Cons: character drift and cleanup dominate; each variant re-costs the whole set; ~$20–50 in credits per pass.

Avoid: Godot-DragonBones (stale), old Spine→Godot scene converters (dead), Animated Drawings (archived), Character Animator.
