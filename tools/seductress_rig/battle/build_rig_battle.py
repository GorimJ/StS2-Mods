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
PARTS_SUB = "parts_battle"   # the neutral rig keeps images/character/parts
RES = f"res://Seductress/images/character/{PARTS_SUB}"
PARTS_DIR = f"{OUT}/Seductress/images/character/{PARTS_SUB}"
SCENE = f"{OUT}/Seductress/scenes/seductress_visuals.tscn"
TEX_SCALE = 0.5            # parts are stored at half the master resolution
VIS_SCALE = 0.25           # master px -> game px (matches the static sprite: 1448 px tall -> ~360)
ORIGIN = (615.0, 1296.0)   # master px that sits at the creature origin (between the feet, on the ground) [Claude]

# ---- draw order, back to front (layercut_config.json order + stand-ins) -------------------------------------
DRAW = json.load(open(f"{WORK}/prep_report.json"))["draw"]

# ---- bones: name -> (parent, pivot in master px). One bone per part, same name. [Claude] ---------------------
BONES = {
    "HIPS":                       (None, (625, 690)),
    "LEG_L_thigh":                ("HIPS", (540, 720)),
    "LEG_L_shin_foot":            ("LEG_L_thigh", (492, 945)),
    "LEG_R_thigh":                ("HIPS", (725, 735)),
    "LEG_R_shin_foot":            ("LEG_R_thigh", (820, 897)),
    "SKIRT_rear_left":            ("HIPS", (560, 665)),
    "SKIRT_rear_right":           ("HIPS", (705, 690)),
    "CLOTH_front_skirt_panel":    ("HIPS", (622, 652)),
    "HIP_ornaments_left":         ("HIPS", (548, 648)),
    "HIP_ornaments_right":        ("HIPS", (702, 668)),
    "BELLY_lower_torso_variant":  ("HIPS", (625, 680)),
    "TORSO_upper":                ("BELLY_lower_torso_variant", (630, 565)),
    "BREASTS_plus_wrap_variant":  ("TORSO_upper", (625, 520)),
    "ARM_L_upper":                ("TORSO_upper", (488, 445)),
    "SLEEVE_L":                   ("ARM_L_upper", (440, 510)),
    "ARM_L_forearm_hand":         ("ARM_L_upper", (395, 525)),
    "PROP_censer_and_chains":     ("ARM_L_forearm_hand", (252, 522)),
    "FX_censer_smoke":            ("PROP_censer_and_chains", (215, 690)),
    "ARM_R_upper":                ("TORSO_upper", (720, 470)),
    "SLEEVE_R":                   ("ARM_R_upper", (722, 530)),
    "ARM_R_forearm_hand":         ("ARM_R_upper", (775, 602)),
    "ARM_R_hand":                 ("ARM_R_forearm_hand", (862, 500)),
    "FINGER_R_pinky":             ("ARM_R_hand", (878, 470)),
    "FINGER_R_ring":              ("ARM_R_hand", (889, 463)),
    "FINGER_R_middle":            ("ARM_R_hand", (900, 461)),
    "FINGER_R_index":             ("ARM_R_hand", (910, 459)),
    "HEAD_face_plus_front_hair":  ("TORSO_upper", (615, 402)),
    "HAIR_back":                  ("HEAD_face_plus_front_hair", (520, 330)),
    "HAIR_side_L":                ("HEAD_face_plus_front_hair", (562, 400)),
    "HAIR_side_R":                ("HEAD_face_plus_front_hair", (690, 400)),
    "HAT_main":                   ("HEAD_face_plus_front_hair", (625, 300)),
    "HAT_charm":                  ("HAT_main", (752, 322)),
}

# ---- idle: per-bone motion as sums of sines. Loop length L seconds; every term has an integer cycle count. ---
L = 6.0
# term: (property, amplitude, cycles over L, phase in turns).  rot in degrees, pos in master px, scale as fraction.
B2 = 2   # breathing: 2 cycles per loop = 3 s
SW = 0.55   # [Gorim 2026-11-17: "too much"] secondary swing scaled down from the first pass
HIP = 6.0   # weight shift: hips sway this many master px side to side, once per loop; feet stay planted
# feet stay planted: thigh + shin angles are solved (two-bone IK) so each ground contact point never moves
FEET = {"LEG_L": (310, 1285), "LEG_R": (880, 1230)}   # ground contact, master px [Claude]
IDLE = {
    # [Gorim 2026-11-17] less vertical bob everywhere except the breasts; small weight shift; fingers move
    "HIPS":                      [("px", HIP, 1, 0.00)],
    "BELLY_lower_torso_variant": [("sy", 0.006, B2, 0.00), ("py", -0.4, B2, 0.02)],
    "TORSO_upper":               [("py", -0.7, B2, 0.00), ("rot", -0.5, 1, 0.00)],
    "BREASTS_plus_wrap_variant": [("py", -3.4, B2, -0.08), ("sx", 0.008, B2, -0.04), ("sy", 0.016, B2, -0.08)],
    "HEAD_face_plus_front_hair": [("rot", 0.5, 1, 0.05), ("py", -0.3, B2, -0.10)],
    "HAT_main":                  [("rot", 0.5, 1, 0.12)],
    "HAT_charm":                 [("rot", 4.0, 3, 0.00), ("rot", 1.4, 1, 0.30)],
    "HAIR_back":                 [("rot", 0.7, 1, 0.08), ("sy", 0.004, B2, -0.12)],
    "HAIR_side_L":               [("rot", 0.9, 1, 0.10), ("rot", 0.3, B2, 0.20)],
    "HAIR_side_R":               [("rot", 0.8, 1, 0.14), ("rot", 0.3, B2, 0.35)],
    "ARM_L_upper":               [("rot", 0.5, B2, -0.05)],
    "ARM_L_forearm_hand":        [("rot", 0.5, B2, -0.15)],
    "SLEEVE_L":                  [("rot", 1.0, B2, -0.22), ("rot", 0.5, 1, 0.1)],
    "PROP_censer_and_chains":    [("rot", 2.6, 3, 0.10), ("rot", 0.9, 1, 0.5)],
    "ARM_R_upper":               [("rot", -0.5, B2, -0.05)],
    "ARM_R_forearm_hand":        [("rot", -1.6, 1, 0.10), ("rot", -0.6, B2, -0.18)],
    "ARM_R_hand":                [("rot", -2.5, 1, 0.20), ("rot", -0.8, B2, -0.05)],
    # fingers curl and open in a wave from the index to the little finger
    "FINGER_R_index":            [("rot", -3.5, 1, 0.28), ("rot", -1.2, B2, 0.10)],
    "FINGER_R_middle":           [("rot", -4.5, 1, 0.34), ("rot", -1.4, B2, 0.18)],
    "FINGER_R_ring":             [("rot", -5.5, 1, 0.40), ("rot", -1.6, B2, 0.26)],
    "FINGER_R_pinky":            [("rot", -6.5, 1, 0.46), ("rot", -1.8, B2, 0.34)],
    "SLEEVE_R":                  [("rot", -1.0, B2, -0.25), ("rot", 0.5, 1, 0.15)],
    "SKIRT_rear_left":           [("rot", 0.6, 1, 0.06), ("rot", 0.2, B2, 0.3)],
    "SKIRT_rear_right":          [("rot", 0.5, 1, 0.14)],
    "CLOTH_front_skirt_panel":   [("rot", 0.6, 1, 0.20)],
    "HIP_ornaments_left":        [("rot", 1.7, 3, 0.20), ("rot", 0.6, 1, 0.0)],
    "HIP_ornaments_right":       [("rot", 1.7, 3, 0.45), ("rot", 0.6, 1, 0.2)],
}
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
# optional pose file (4th argument): {"bone": [rot_deg, dx, dy]} added to the rest pose; the idle plays around it
POSE = json.load(open(sys.argv[4])) if len(sys.argv) > 4 else {}
def pose(b):
    r = POSE.get(b, [0, 0, 0]); return float(r[0]), float(r[1]), float(r[2])


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
        base_pos = (rest[0] - ppiv[0] + pose(bone)[1], rest[1] - ppiv[1] + pose(bone)[2])
        props = {}
        for prop, amp, cyc, ph in terms:
            props.setdefault(prop, []).append((amp, cyc, ph))
        def val(prop, t):
            return sum(a * math.sin(2 * math.pi * (c * t / L + p)) for a, c, p in props.get(prop, []))
        path = f"Rig/{bone_path(bone)}"
        if "rot" in props:
            vals = ", ".join(f(math.radians(val("rot", t) + pose(bone)[0])) for t in times)
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

    # legs: two-bone IK against the hip sway
    def rot(v, a):
        return (v[0] * math.cos(a) - v[1] * math.sin(a), v[0] * math.sin(a) + v[1] * math.cos(a))
    for leg, foot in FEET.items():
        Hp, Kp = BONES[f"{leg}_thigh"][1], BONES[f"{leg}_shin_foot"][1]
        u, w = (Kp[0] - Hp[0], Kp[1] - Hp[1]), (foot[0] - Kp[0], foot[1] - Kp[1])
        t1s, t2s, a1, a2 = [], [], 0.0, 0.0
        for t in times:
            dx = sum(a * math.sin(2 * math.pi * (c * t / L + p)) for pr, a, c, p in IDLE["HIPS"] if pr == "px")
            def err(x1, x2):
                r1, r2 = rot(u, x1), rot(w, x1 + x2)
                return (dx + r1[0] + r2[0] - u[0] - w[0], r1[1] + r2[1] - u[1] - w[1])
            for _ in range(12):                       # Newton, numeric Jacobian
                e = err(a1, a2); h = 1e-6
                e1, e2 = err(a1 + h, a2), err(a1, a2 + h)
                j11, j21, j12, j22 = (e1[0] - e[0]) / h, (e1[1] - e[1]) / h, (e2[0] - e[0]) / h, (e2[1] - e[1]) / h
                det = j11 * j22 - j12 * j21
                a1 -= (j22 * e[0] - j12 * e[1]) / det; a2 -= (-j21 * e[0] + j11 * e[1]) / det
            t1s.append(a1); t2s.append(a2)
        for bone, vals in ((f"{leg}_thigh", t1s), (f"{leg}_shin_foot", t2s)):
            tracks.append((ti, f"Rig/{bone_path(bone)}:rotation", "PackedFloat32Array(" + ", ".join(f(t) for t in times) + ")",
                           "[" + ", ".join(f(v) for v in vals) + "]")); ti += 1
        print(leg, "max thigh/shin deg", round(math.degrees(max(map(abs, t1s))), 2), round(math.degrees(max(map(abs, t2s))), 2))

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
                f"position = Vector2({f(piv[0] - ppiv[0] + pose(b)[1])}, {f(piv[1] - ppiv[1] + pose(b)[2])})",
                f"rotation = {f(math.radians(pose(b)[0]))}", ""]
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
            "anchors_preset = 0", "offset_left = -110.0", "offset_top = -300.0", "offset_right = 100.0",
            "offset_bottom = -10.0", "mouse_filter = 2", "",
            '[node name="CenterPos" type="Marker2D" parent="."]', "unique_name_in_owner = true",
            "position = Vector2(0, -150)", "",
            '[node name="IntentPos" type="Marker2D" parent="."]', "unique_name_in_owner = true",
            "position = Vector2(0, -330)", ""]
    with open(SCENE, "w", encoding="utf-8", newline="\n") as fh:   # no BOM
        fh.write("\n".join(out))

    # ---------------- flat check image with pivots ----------------
    canvas = Image.new("RGBA", (1086, 1448), (75, 75, 85, 255))
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
