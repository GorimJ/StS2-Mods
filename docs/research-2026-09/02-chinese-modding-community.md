# Chinese STS2 modding community — survey (2026-09-04)

Compiled by a research subagent. Bilibili and Steam rate-limited (412/429) part-way; those items are flagged. WebFetch summaries of Steam Workshop pages returned "2024" dates and a "removed" banner on almost every item — fetcher artefacts, ignore.

## 1. Chinese tutorials / guides

**A. 杀戮尖塔2mod制作教程 (STS2 Mod-Making Tutorial) — the canonical resource**
- https://tutorials.sts2modding.com/ (source https://github.com/GlitchedReme/SlayTheSpire2ModdingTutorials, 159 stars, 285 commits). Author Reme (GlitchedReme, also wrote the STS1 Chinese tutorial). Started 2026-03-07, actively updated.
- Structure: Basics (env setup, decompiling with gdsdecomp / ILSpy, Harmony, hot reload, console, Workshop upload), a full **RitsuLib** chapter and a full **BaseLib** chapter (cards, relics, powers, monsters, events, characters), Migrations, and a **Visuals (视觉)** chapter. Dev QQ group: **542370192**.
- **卡图&Spine** https://tutorials.sts2modding.com/docs/05-card-art-and-skin-replacement/ — "尖塔使用 4.2.43 版本的Spine，在这之下版本的不能直接使用" (game uses Spine **4.2.43**; lower versions can't be used directly). Pipeline: install the precompiled spine-godot extension (hosted on Baidu Pan), drop `.atlas` + `.skel` + `.png` into the Godot project, create `SpineSkeletonDataResource`. Combat animation names: `idle_loop`, `attack`, `cast`, `hurt`, `die`. Disable "Convert Text Resources to Binary" if export breaks. **Non-Spine route:** patch `CharacterModel.CreateVisuals` to return your own node inheriting `NCreatureVisuals`; scene needs unique-named `Visuals(Node2D)`, `Bounds(Control)`, `IntentPos(Marker2D)`, `CenterPos(Marker2D)`; even 3D via `SubViewportContainer→SubViewport→Camera3D`.
- **添加新人物** https://tutorials.sts2modding.com/docs/04-add-new-character/ — extend `PlaceholderCharacterModel`; `Visuals` "accepts `SpineSprite` for skeletal animation, `Sprite2D` for static images, or `AnimatedSprite2D` for frame sequences"; merchant/rest-site use `relaxed_loop`; energy-counter layout; localization JSON.
- **帧动画的处理** https://tutorials.sts2modding.com/docs/09-01-frame-animation/ — PNG sequences → `AnimatedSprite2D`/`SpriteFrames`.
- **风格原画绘制** https://tutorials.sts2modding.com/docs/06-style-art-drawing/ — hand-drawing guide; design note: fitted clothes and shorter hair "reduce rigging complexity". No AI tools recommended.

**B. sts2-quickRestart README (freude916)** https://github.com/freude916/sts2-quickRestart — "塔1使用 Spine 3.4 且导出格式为 JSON。塔2使用 Spine 4.2 且导出格式为二进制格式（.skel）"; porting STS1 skeletons needs ~10× root-bone scale (Defect example) and renamed animations; STS1 skeletons only have idle/hit while "塔2则规范化地内置了至少6种标准动画" (STS2 standardises ≥6 animations). Use gdsdecomp for extracting the game's `.pck` (auto-splits atlases), Godot 4.5.1 mono for packing.

**C. Steam guide "杀戮尖塔2 Mod 开发指南"** https://steamcommunity.com/app/2868840/discussions/0/806845425982211616/ — Mar 2026, hello-world C# mod, dnlib, GodotPCKExplorer. Nothing on animation.

**D. RitsuLib docs (BAKAOLC / OLC)** https://sts2-ritsulib.ritsukage.com/guide/creature-visuals-and-animation — graded hierarchy: (1) swap `CharacterAssetProfile.Scenes.VisualsPath`; (2) **`VisualCueSet`** "for static frames or sequences without Spine skeletal animation" (`["idle"]="res://…/idle.png", ["hit"]=…`); (3) override `TryCreateCreatureVisuals()`; (4) override `SetupCustomCombatAnimationStateMachine(...)`. Normalised triggers: `Idle, Dead, Hit, Attack, Cast, Relaxed, Revive`. "Provide one visible fallback pose for non-Spine creatures."

**E. Bilibili** — mostly 412s. Found: "塔2卡图mod制作工具分享，零基础上手，全自动打包" (BV1cpju6TE7p, 2026-06-16) — a no-code **card-art** packer. No bilibili 专栏/video specifically on STS2 character animation found.

**F. Not found:** zhihu/tieba mod-making tutorials (install-only).

## 2. How Chinese character/skin mods do their animation

| Mod | Source / evidence | Animation approach |
|---|---|---|
| 若叶睦 Wakaba Mutsumi (FFTYYY) | https://github.com/FFTYYY/sts2-MzmChar-mod (MIT, BaseLib ≥3.3, `MutsumiCharacter : CustomCharacterModel`, packs via MegaDot) | `DEVELOPER_GUIDE.md` references `.skel`/`.atlas` in `pack/MzmChar/characters/` → **Spine 4.2 skeleton**. Best open-source Chinese character mod to clone. |
| Nightveil Nekoninja / Crimson Blade Valkyrie (阿塔Official, with OLC) | Workshop 3759576970 / 3747952763 | Silent skin: model + card art, RitsuLib-toggleable; "No AI-generated content"; CC BY-NC-ND. Uses the game's Spine skeleton path (inference). No source. |
| Ovelle 奥薇乐 (言刃文刀) | Workshop 3749703466, RitsuLib | Custom character with CG pop-ups; comments complain about idle delay after hit → custom animation state machine, likely Spine (inference). No source. |
| The Song of Saya (千导院枫) | Workshop 3747508952 | Art "jointly generated with NAI, Banana, GPT-Images 2… then manually polished"; "2 animated card artworks"; no dependency libs. |
| RegentFX 万象辉星 | Workshop 3747497501 | VFX-only add-on. |
| Watcher (Boninall) + Watcher Beautified | Workshop 3747526116 / 3747800917 | STS1 assets ported; Beautified = redrawn assets + animations. |
| sts1to2 Ironclad/Silent (rayinls) | https://github.com/rayinls/sts-1-to-2-card | Ports STS1 Spine 3.x skeletons — converter not documented; combine with quickRestart's 3.4→4.2 notes. |
| KaguyaRegentMSGKSkin (A0ShiRo / 文和) | Workshop 3783735921 | Credits separate 原画 (illustration) and 动画 (animation) roles — the team model. |

Libraries: **KitLib** (https://github.com/WRXinYue/STS2-KitLib, v0.24.0 2026-07-03) — cheat/debug/script toolkit, no visuals API. **Sts2SkinManager** (https://github.com/ing-gom/Sts2SkinManager) — a "character skin" is a `.pck` overriding `res://animations/characters/{character}/...` (Spine data), mounted via Harmony patch on `ProjectSettings.LoadResourcePack`. "自定义骨骼加载器" (ali213/3DM, Apr 2026) is despite its name a GIF/PNG **card-texture** loader.

## 3. Community hubs
- **QQ dev group 542370192** — the real hub (where Spine builds/Baidu links circulate).
- **sts2.gg/zh/mods** — Chinese index mirroring Nexus; lists character mods (WineFox by OLC, Wakaba Mutsumi, Miyu, Neuvillette…); no dev docs.
- **3DM Mod站** https://mod.3dmgame.com/slaythespire2, **游侠 ali213** https://patch.ali213.net/z/93593/ — download mirrors; Workshop collections 3773357501 / 3747717396 are the de-facto Chinese catalogues.
- Alchyr's BaseLib wiki is what Chinese authors cite.

## 4. AI-assisted 2D animation (Chinese discussion)
- **See-through** (SIGGRAPH 2026, Apache-2.0) https://github.com/shitagaki-lab/see-through — single anime illustration → up-to-23-layer inpainted PSD (12–16 GB VRAM, NF4 for 8 GB); ComfyUI wrapper https://github.com/jtydhr88/ComfyUI-See-through; zhihu write-up https://zhuanlan.zhihu.com/p/2023503283017328213. Not a full image-to-Live2D.
- **Tahou "2D立绘活了：Live2D/Spine 动作的 AI 辅助生成流"** (2026-01-13) https://www.tahou.com/article/191930619786845189 — PS Generative Fill for occlusion fill (补肉), Cubism 5 auto-mesh, Spine Auto Weights, physics for hair; claims 3–5 days → 1 day.
- **SpineAI** (搜狐畅游, 2025-04) — internal Unity tool; not public. **SpineForge** https://shimagame.com/ — site down.
- Sprite-sheet route: https://github.com/ShooflyL/SpriteSheetGenerater (video → frames), FramePacker https://www.framepacker.cn/, 帧灵 FrameSprite https://cn.framesprite.com/character.
- **Directly relevant to the licence problem:** https://github.com/wang606/SpineSkeletonDataConverter (C++, 2026-04-21, PolyForm-NC) converts skel/json across **3.5–4.2** including up-conversion to 4.2 `.skel`; DragonBones Tools' `db2spine` CLI exports DragonBones → Spine 3.x JSON (https://blog.csdn.net/weixin_44338096/article/details/125555475). Plausible licence-free path: DragonBones → db2spine (3.x JSON) → SpineSkeletonDataConverter (4.2 skel); untested.

Couldn't access: GitHub tree/blob listings, most bilibili pages (412), several Steam pages (429), zhihu, CSDN (521).
