"""Build the Seductress cut-out rig: cropped half-res parts + seductress_visuals.tscn with an idle loop.

usage: python build_rig.py <Seductress folder> <work dir with stand-ins> <out project dir>

Structure of the generated scene (no scripts):
  SeductressVisuals (Node2D)
    Visuals (Node2D, %unique)            scale S: master-pixel coordinates inside
      Parts (Node2D)                     flat list of Sprite2D in draw order (no z_index used)
      Rig (Node2D)                       bone tree of Node2D; each bone has a RemoteTransform2D -> its sprite
      AnimationPlayer                    "idle" (autoplay, loop)
    Bounds / CenterPos / IntentPos       same contract as the static scene
All coordinates below are master pixels (DefaultNeutral.png, 1086x1448). [Claude] = estimated by eye.
"""
import sys, os, json, math
import numpy as np
from PIL import Image

SRC, WORK, OUT = sys.argv[1], sys.argv[2], sys.argv[3]
CUTS = f"{SRC}/GorimCuts/Claude_2026-10-24"
RES = "res://Seductress/images/character/parts"
PARTS_DIR = f"{OUT}/Seductress/images/character/parts"
SCENE = f"{OUT}/Seductress/scenes/seductress_visuals.tscn"
TEX_SCALE = 0.5            # parts are stored at half the master resolution
VIS_SCALE = 0.25           # master px -> game px (matches the static sprite: 1448 px tall -> ~360)
ORIGIN = (596.0, 1372.0)   # master px that sits at the creature origin (between the feet, on the ground) [Claude]

# ---- draw order, back to front (layercut_config.json order + stand-ins) -------------------------------------
DRAW = json.load(open(f"{WORK}/prep_report.json"))["draw"]

# ---- bones: name -> (parent, pivot in master px). One bone per part, same name. [Claude] ---------------------
BONES = {
    "HIPS":                       (None, (596, 640)),
    "PELVIS_base":                ("HIPS", (596, 640)),
    "LEG_L_thigh":                ("HIPS", (522, 640)),
    "LEG_L_shin_foot":            ("LEG_L_thigh", (478, 892)),
    "LEG_R_thigh":                ("HIPS", (672, 645)),
    "LEG_R_shin_foot":            ("LEG_R_thigh", (735, 897)),
    "SKIRT_rear_left":            ("HIPS", (540, 600)),
    "SKIRT_rear_right":           ("HIPS", (670, 600)),
    "SKIRT_hip_wrap_left":        ("HIPS", (505, 580)),
    "CLOTH_front_skirt_panel":    ("HIPS", (590, 560)),
    "HIP_ornaments_left":         ("HIPS", (518, 578)),
    "HIP_ornaments_right":        ("HIPS", (702, 588)),
    "BELLY_lower_torso_variant":  ("HIPS", (600, 625)),
    "TORSO_upper":                ("BELLY_lower_torso_variant", (598, 525)),
    "BREASTS_plus_wrap_variant":  ("TORSO_upper", (590, 430)),
    "ARM_L_upper":                ("TORSO_upper", (468, 372)),
    "ARM_L_forearm_hand":         ("ARM_L_upper", (378, 512)),
    "SLEEVE_L_back":              ("ARM_L_upper", (425, 440)),
    "SLEEVE_L_front":             ("ARM_L_upper", (425, 440)),
    "PROP_censer_and_chains":     ("ARM_L_forearm_hand", (212, 620)),
    "FX_censer_smoke":            ("ARM_L_forearm_hand", (160, 770)),   # follows the hand, not the swinging censer
    "ARM_R_upper":                ("TORSO_upper", (688, 388)),
    "ARM_R_forearm_hand":         ("ARM_R_upper", (762, 540)),
    "SLEEVE_R_back":              ("ARM_R_upper", (722, 452)),
    "SLEEVE_R_front":             ("ARM_R_upper", (722, 452)),
    "HEAD_face_plus_front_hair":  ("TORSO_upper", (590, 345)),
    "HAIR_back":                  ("HEAD_face_plus_front_hair", (585, 290)),
    "HAIR_side_L":                ("HEAD_face_plus_front_hair", (525, 235)),
    "HAIR_side_R":                ("HEAD_face_plus_front_hair", (675, 235)),
    "HAT_main":                   ("HEAD_face_plus_front_hair", (590, 235)),
    "HAT_charm":                  ("HAT_main", (746, 240)),
}

# ---- idle: per-bone motion as sums of sines. Loop length L seconds; every term has an integer cycle count. ---
L = 6.0
# term: (property, amplitude, cycles over L, phase in turns).  rot in degrees, pos in master px, scale as fraction.
B2 = 2   # breathing: 2 cycles per loop = 3 s
SW = 0.55   # [Gorim 2026-11-17: "too much"] secondary swing scaled down from the first pass
HIP = 8.0   # weight shift: hips sway this many master px side to side, once per loop; feet stay planted
LEG_L, LEG_R = 600.0, 585.0   # hip-to-ankle height, used to counter-rotate each leg so the ankle stays put
IDLE = {
    "HIPS":                      [("px", HIP, 1, 0.0)],
    "LEG_L_thigh":               [("rot", math.degrees(HIP / LEG_L), 1, 0.0)],
    "LEG_R_thigh":               [("rot", math.degrees(HIP / LEG_R), 1, 0.0)],
    "BELLY_lower_torso_variant": [("sy", 0.010, B2, 0.00), ("rot", -0.55, 1, 0.0)],
    "TORSO_upper":               [("py", -2.6, B2, 0.00), ("rot", -0.25, 1, 0.03)],
    "BREASTS_plus_wrap_variant": [("py", -1.8, B2, -0.06), ("sx", 0.010, B2, -0.03), ("sy", 0.012, B2, -0.06)],
    "HEAD_face_plus_front_hair": [("rot", 0.9 * SW, 1, 0.20), ("py", -0.6, B2, -0.10)],
    "HAT_main":                  [("rot", 0.9 * SW, 1, 0.08)],
    "HAT_charm":                 [("rot", 7.0 * SW, 3, 0.00), ("rot", 2.5 * SW, 1, 0.30)],
    "HAIR_back":                 [("rot", 1.1 * SW, 1, 0.02), ("sy", 0.006, B2, -0.12)],
    "HAIR_side_L":               [("rot", 1.6 * SW, 1, -0.02), ("rot", 0.5 * SW, B2, 0.20)],
    "HAIR_side_R":               [("rot", 1.4 * SW, 1, 0.04), ("rot", 0.5 * SW, B2, 0.35)],
    "ARM_L_upper":               [("rot", 1.1 * SW, B2, -0.05)],
    "ARM_L_forearm_hand":        [("rot", 0.9 * SW, B2, -0.15)],
    "PROP_censer_and_chains":    [("rot", 4.5 * SW, 3, 0.10), ("rot", 1.5 * SW, 1, 0.5)],
    "ARM_R_upper":               [("rot", -1.0 * SW, B2, -0.05)],
    "ARM_R_forearm_hand":        [("rot", -0.8 * SW, B2, -0.18)],
    "SKIRT_rear_left":           [("rot", 0.9 * SW, 1, 0.00)],
    "SKIRT_rear_right":          [("rot", 0.8 * SW, 1, 0.08)],
    "CLOTH_front_skirt_panel":   [("rot", 0.7 * SW, 1, 0.15)],
    "HIP_ornaments_left":        [("rot", 3.0 * SW, 3, 0.20), ("rot", 1.0 * SW, 1, 0.0)],
    "HIP_ornaments_right":       [("rot", 3.0 * SW, 3, 0.45), ("rot", 1.0 * SW, 1, 0.2)],
}
for _s, _sign, _ph in (("L", 1, 0.0), ("R", -1, 0.03)):       # both halves of a sleeve move as one
    for _h in ("back", "front"):
        IDLE[f"SLEEVE_{_s}_{_h}"] = [("rot", _sign * 1.8 * SW, B2, -0.22 - _ph), ("rot", 0.8 * SW, 1, 0.1 + _ph)]
SMOKE_SHADER = """shader_type canvas_item;
// the drawn smoke, made to drift: waves travel up the plume and grow with height; the base stays on the censer
void fragment() {
	float h = 1.0 - UV.y;
	vec2 uv = UV;
	uv.x += (sin(UV.y * 9.0 + TIME * 1.1) * 0.030 + sin(UV.y * 4.0 + TIME * 0.6) * 0.035) * h;
	uv.y += sin(UV.x * 5.0 + TIME * 0.8) * 0.012 * h;
	vec4 c = texture(TEXTURE, uv);
	float breathe = 0.82 + 0.18 * sin(TIME * 0.7 + UV.y * 5.0);
	c.a *= breathe * smoothstep(0.0, 0.10, UV.y) * smoothstep(0.0, 0.06, 1.0 - UV.y + 0.06);
	COLOR = c;
}
"""
STEP = 0.1  # key spacing, seconds


def find(name):
    return f"{WORK}/{name}.png"          # all parts come from prep_parts.py


def f(x):
    s = f"{x:.4f}".rstrip("0").rstrip(".")
    return "0" if s in ("-0", "") else s


def main():
    os.makedirs(PARTS_DIR, exist_ok=True)
    os.makedirs(os.path.dirname(SCENE), exist_ok=True)
    parts = {}
    for name in DRAW:
        im = Image.open(find(name)).convert("RGBA")
        a = np.array(im)[..., 3]
        ys, xs = np.nonzero(a > 8)
        pad = 36 if name == "FX_censer_smoke" else 2
        x0, y0 = max(xs.min() - pad, 0), max(ys.min() - pad, 0)
        x1, y1 = min(xs.max() + 1 + pad, im.width), min(ys.max() + 1 + pad, im.height)
        x0 -= x0 % 2; y0 -= y0 % 2; x1 += x1 % 2; y1 += y1 % 2          # even box -> exact half-res
        crop = im.crop((x0, y0, x1, y1))
        # premultiply before resampling so transparent pixels' colour doesn't bleed into the edges
        arr = np.array(crop).astype(np.float32)
        arr[..., :3] *= arr[..., 3:4] / 255.0
        # exact 2x2 box average (Lanczos overshoots at hard alpha edges and leaves a pale fringe)
        small = arr.reshape(crop.height // 2, 2, crop.width // 2, 2, 4).mean(axis=(1, 3))
        al = np.clip(small[..., 3:4], 1, 255)
        small[..., :3] = np.clip(small[..., :3] * 255.0 / al, 0, 255)
        Image.fromarray(small.astype(np.uint8), "RGBA").save(f"{PARTS_DIR}/{name}.png")
        parts[name] = {"box": [int(x0), int(y0), int(x1), int(y1)], "pivot": BONES[name][1], "parent": BONES[name][0]}
    json.dump({"origin": ORIGIN, "tex_scale": TEX_SCALE, "vis_scale": VIS_SCALE, "draw": DRAW,
               "bones": {k: {"parent": v[0], "pivot": v[1]} for k, v in BONES.items()}, "parts": parts},
              open(f"{OUT}/parts.json", "w"), indent=1)

    # ---------------- scene text ----------------
    ext, nodes = [], []
    for i, name in enumerate(DRAW):
        ext.append(f'[ext_resource type="Texture2D" path="{RES}/{name}.png" id="t{i}"]')

    def bone_path(b):
        chain = []
        while b:
            chain.append(b); b = BONES[b][0]
        return "/".join(reversed(chain))

    # animation tracks
    tracks, ti = [], 0
    times = [round(i * STEP, 4) for i in range(int(round(L / STEP)) + 1)]
    for bone, terms in IDLE.items():
        rest = BONES[bone][1]; par = BONES[bone][0]
        ppiv = BONES[par][1] if par else ORIGIN
        base_pos = (rest[0] - ppiv[0], rest[1] - ppiv[1])
        props = {}
        for prop, amp, cyc, ph in terms:
            props.setdefault(prop, []).append((amp, cyc, ph))
        def val(prop, t):
            return sum(a * math.sin(2 * math.pi * (c * t / L + p)) for a, c, p in props.get(prop, []))
        path = f"Rig/{bone_path(bone)}"
        if "rot" in props:
            vals = ", ".join(f(math.radians(val("rot", t))) for t in times)
            tracks.append((ti, f"{path}:rotation", "PackedFloat32Array(" + ", ".join(f(t) for t in times) + ")",
                           "[" + vals + "]")); ti += 1
        if "px" in props or "py" in props:
            vals = ", ".join(f"Vector2({f(base_pos[0] + val('px', t))}, {f(base_pos[1] + val('py', t))})" for t in times)
            tracks.append((ti, f"{path}:position", "PackedFloat32Array(" + ", ".join(f(t) for t in times) + ")",
                           "[" + vals + "]")); ti += 1
        if "sx" in props or "sy" in props:
            vals = ", ".join(f"Vector2({f(1 + val('sx', t))}, {f(1 + val('sy', t))})" for t in times)
            tracks.append((ti, f"{path}:scale", "PackedFloat32Array(" + ", ".join(f(t) for t in times) + ")",
                           "[" + vals + "]")); ti += 1

    anim = ['[sub_resource type="Animation" id="anim_idle"]', 'resource_name = "idle"', f"length = {f(L)}", "loop_mode = 1"]
    for i, path, tarr, varr in tracks:
        trans = "PackedFloat32Array(" + ", ".join(["1"] * len(times)) + ")"
        anim += [f'tracks/{i}/type = "value"', f"tracks/{i}/imported = false", f"tracks/{i}/enabled = true",
                 f'tracks/{i}/path = NodePath("{path}")', f"tracks/{i}/interp = 1", f"tracks/{i}/loop_wrap = true",
                 f"tracks/{i}/keys = {{\n\"times\": {tarr},\n\"transitions\": {trans},\n\"update\": 0,\n\"values\": {varr}\n}}"]
    lib = ['[sub_resource type="AnimationLibrary" id="lib"]', '_data = {\n&"idle": SubResource("anim_idle")\n}']

    shader = ['[sub_resource type="Shader" id="sh_smoke"]', 'code = "' + SMOKE_SHADER.replace('"', '\\"') + '"', "",
              '[sub_resource type="ShaderMaterial" id="mat_smoke"]', 'shader = SubResource("sh_smoke")']
    out = [f"[gd_scene load_steps={len(ext) + 5} format=3]", ""] + ext + [""] + shader + [""] + anim + [""] + lib + [""]
    out += ['[node name="SeductressVisuals" type="Node2D"]', "",
            '[node name="Visuals" type="Node2D" parent="."]', "unique_name_in_owner = true",
            f"scale = Vector2({f(VIS_SCALE)}, {f(VIS_SCALE)})", "",
            '[node name="Parts" type="Node2D" parent="Visuals"]', ""]
    for i, name in enumerate(DRAW):
        x0, y0, _, _ = parts[name]["box"]; px, py = BONES[name][1]
        out += [f'[node name="{name}" type="Sprite2D" parent="Visuals/Parts"]'] +                (['material = SubResource("mat_smoke")'] if name == "FX_censer_smoke" else []) + [
                f"position = Vector2({f(px - ORIGIN[0])}, {f(py - ORIGIN[1])})",
                f"scale = Vector2({f(1 / TEX_SCALE)}, {f(1 / TEX_SCALE)})",
                f'texture = ExtResource("t{i}")', "centered = false",
                f"offset = Vector2({f((x0 - px) * TEX_SCALE)}, {f((y0 - py) * TEX_SCALE)})", ""]
    out += ['[node name="Rig" type="Node2D" parent="Visuals"]', ""]
    # bones, parents before children (dict order already satisfies this)
    for b, (par, piv) in BONES.items():
        ppiv = BONES[par][1] if par else ORIGIN
        parent_path = "Visuals/Rig" + ("/" + bone_path(par) if par else "")
        out += [f'[node name="{b}" type="Node2D" parent="{parent_path}"]',
                f"position = Vector2({f(piv[0] - ppiv[0])}, {f(piv[1] - ppiv[1])})", ""]
        if b in parts:
            depth = bone_path(b).count("/") + 1
            up = "/".join([".."] * (depth + 2))          # RT -> bone ... -> Rig -> Visuals
            out += [f'[node name="RT" type="RemoteTransform2D" parent="{parent_path}/{b}"]',
                    f"scale = Vector2({f(1 / TEX_SCALE)}, {f(1 / TEX_SCALE)})",
                    f'remote_path = NodePath("{up}/Parts/{b}")', ""]
    out += ['[node name="AnimationPlayer" type="AnimationPlayer" parent="Visuals"]',
            'libraries = {\n&"": SubResource("lib")\n}', 'autoplay = "idle"', ""]
    # contract nodes, same numbers as the static scene
    out += ['[node name="Bounds" type="Control" parent="."]', "unique_name_in_owner = true", "layout_mode = 3",
            "anchors_preset = 0", "offset_left = -100.0", "offset_top = -350.0", "offset_right = 100.0",
            "offset_bottom = -10.0", "mouse_filter = 2", "",
            '[node name="CenterPos" type="Marker2D" parent="."]', "unique_name_in_owner = true",
            "position = Vector2(0, -170)", "",
            '[node name="IntentPos" type="Marker2D" parent="."]', "unique_name_in_owner = true",
            "position = Vector2(0, -370)", ""]
    with open(SCENE, "w", encoding="utf-8", newline="\n") as fh:   # no BOM
        fh.write("\n".join(out))

    # ---------------- flat check image with pivots ----------------
    canvas = Image.new("RGBA", (1086, 1448), (107, 108, 111, 255))
    for name in DRAW:
        canvas.alpha_composite(Image.open(find(name)).convert("RGBA"))
    from PIL import ImageDraw
    d = ImageDraw.Draw(canvas)
    for b, (par, piv) in BONES.items():
        if par:
            d.line([BONES[par][1], piv], fill=(0, 255, 255, 255), width=2)
    for b, (par, piv) in BONES.items():
        d.ellipse([piv[0] - 5, piv[1] - 5, piv[0] + 5, piv[1] + 5], fill=(255, 255, 0, 255), outline=(0, 0, 0, 255))
    canvas.convert("RGB").save(f"{OUT}/CHECK_assembly_pivots.png")
    total = sum(os.path.getsize(f"{PARTS_DIR}/{n}.png") for n in DRAW)
    print(f"{len(DRAW)} parts, {len(BONES)} bones, {len(tracks)} tracks, parts total {total / 1024:.0f} KB")


main()
