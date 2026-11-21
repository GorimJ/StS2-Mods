"""Build the three hole insets (mouth, womb, ass) as mod-ready scenes. [Claude, 2026-11-21, on Gorim's request]

usage: python build_insets.py <src dir> [<out dir>]
  src dir: holds womb.png, womb_mask.png, womb_prof.png, ass.png, ass_mask.png, ass_prof.png, mouth.png,
           mouth_mask.png, mouth_prof.png, cock.png, cock_prof.png  (made by make_profile.py / make_path.py from
           Seductress/inset_concepts/picks).  May be the out dir itself, to rebuild only scenes and shaders.
  out dir: default Seductress/Seductress/scenes/insets

Each scene is  Inset (Node2D) > Pic (Sprite2D 1024x1024, top-left at the origin, its own ShaderMaterial)
                              > Anim (AnimationPlayer)
The game sets shader parameters on Pic.material:  fill (0..1), girth (0.7 / 1.0 / 1.35 / 1.8)
and plays animations on Anim, which move  depth  and  spurt:
  out          cock out of the picture (also the state on load)
  attack_in    cock enters, two thrusts, small spurt, stays just inside     (first attack into this hole)
  attack       two thrusts, small spurt, stays just inside                  (later attacks)
  finish       faster thrusts, held deep, three big spurts, pulls out       (enemy finished)
  finish_in    enters first, then as finish
SPURTS below lists when each spurt starts, so the fill can be raised at that moment.
"""
import sys, json, math, shutil
from pathlib import Path

HERE = Path(__file__).resolve().parent
REPO = HERE.parents[2]
SRC = Path(sys.argv[1]).resolve()
OUT = Path(sys.argv[2]).resolve() if len(sys.argv) > 2 else REPO / "Seductress/Seductress/scenes/insets"
RES = "res://Seductress/scenes/insets"
FPS = 30
REST = 0.1           # depth the cock rests at between attacks (tip just inside)
SMALL, BIG = 0.45, 1.0

def smooth(a, b, x):
    x = min(max((x - a) / (b - a), 0.0), 1.0); return x * x * (3 - 2 * x)

def pulse(t, t0, peak):                       # a spurt: quick rise, slower fall
    if t < t0: return 0.0
    return peak * min((t - t0) / 0.06, 1.0) * math.exp(-max(t - t0 - 0.06, 0.0) / 0.16)

def attack(t):                                # 1.1 s
    if t < 0.4:    d = REST + (1 - REST) * (0.5 - 0.5 * math.cos(t / 0.4 * math.tau))
    elif t < 0.6:  d = REST + (1 - REST) * smooth(0.4, 0.6, t)
    elif t < 0.85: d = 1.0 - 0.03 * math.sin((t - 0.6) * math.tau * 4)
    else:          d = 1.0 + (REST - 1.0) * smooth(0.85, 1.1, t)
    return d, pulse(t, 0.62, SMALL)

def finish(t):                                # 2.8 s
    if t < 1.2:
        ph = 4.0 * (0.55 * (t / 1.2) + 0.45 * (t / 1.2) ** 2)       # four thrusts, speeding up
        d = REST + (1 - REST) * (0.5 - 0.5 * math.cos(ph * math.tau))
        d = max(d, smooth(1.0, 1.2, t))                               # ends fully in
    elif t < 2.3:  d = 1.0 - 0.04 * math.sin((t - 1.2) * math.tau * 2.86) * math.exp(-(t - 1.2) * 1.5)
    else:          d = 1.0 - 2.0 * smooth(2.3, 2.8, t)
    return d, max(pulse(t, 1.25, BIG), pulse(t, 1.6, 0.85), pulse(t, 1.95, 0.7))

ENTER = 0.35
def with_enter(fn):
    def g(t):
        if t < ENTER: return -1.0 + (1.0 + REST) * smooth(0.0, ENTER, t), 0.0
        return fn(t - ENTER)
    return g

ANIMS = {"out": (lambda t: (-1.0, 0.0), 0.1), "attack": (attack, 1.1), "attack_in": (with_enter(attack), 1.1 + ENTER),
         "finish": (finish, 2.8), "finish_in": (with_enter(finish), 2.8 + ENTER)}
SPURTS = {"attack": [0.62], "attack_in": [0.62 + ENTER], "finish": [1.25, 1.6, 1.95],
          "finish_in": [1.25 + ENTER, 1.6 + ENTER, 1.95 + ENTER]}

def track(i, param, times, vals):
    f = lambda xs: ", ".join(f"{x:.4g}" for x in xs)
    return (f'tracks/{i}/type = "value"\ntracks/{i}/imported = false\ntracks/{i}/enabled = true\n'
            f'tracks/{i}/path = NodePath("Pic:material:shader_parameter/{param}")\ntracks/{i}/interp = 1\n'
            f'tracks/{i}/loop_wrap = true\ntracks/{i}/keys = {{\n"times": PackedFloat32Array({f(times)}),\n'
            f'"transitions": PackedFloat32Array({f([1] * len(times))}),\n"update": 0,\n'
            f'"values": [{", ".join(repr(round(float(v), 4)) for v in vals)}]\n}}\n')

def anims_text():
    out = []
    for name, (fn, length) in ANIMS.items():
        n = max(int(round(length * FPS)), 1)
        times = [k / FPS for k in range(n)] + [length]
        vals = [fn(t) for t in times]
        out.append(f'[sub_resource type="Animation" id="a_{name}"]\nresource_name = "{name}"\nlength = {length:.4g}\n'
                   + track(0, "depth", times, [v[0] for v in vals]) + track(1, "spurt", times, [v[1] for v in vals]))
    out.append('[sub_resource type="AnimationLibrary" id="lib"]\n_data = {\n'
               + ",\n".join(f'&"{n}": SubResource("a_{n}")' for n in ANIMS) + "\n}\n")
    return "\n".join(out)

def scene(name, shader, params):
    ext = {"pic": f"{name}.png", "cock": "cock.png", "sh": shader, "prof": f"{name}_prof.png", "mask": f"{name}_mask.png",
           "cprof": "cock_prof.png"}
    kind = lambda p: "Shader" if p.endswith(".gdshader") else "Texture2D"
    s = "[gd_scene format=3]\n\n" + "".join(f'[ext_resource type="{kind(p)}" path="{RES}/{p}" id="{k}"]\n' for k, p in ext.items())
    s += ('\n[sub_resource type="ShaderMaterial" id="mat"]\nresource_local_to_scene = true\nshader = ExtResource("sh")\n'
          'shader_parameter/profile = ExtResource("prof")\nshader_parameter/mask = ExtResource("mask")\n'
          'shader_parameter/cock_prof = ExtResource("cprof")\nshader_parameter/cock_tex = ExtResource("cock")\n'
          'shader_parameter/depth = -1.0\nshader_parameter/fill = 0.0\nshader_parameter/girth = 1.0\nshader_parameter/spurt = 0.0\n')
    s += "".join(f"shader_parameter/{k} = {v}\n" for k, v in params.items())
    s += "\n" + anims_text()
    s += ('\n[node name="Inset" type="Node2D"]\n\n[node name="Pic" type="Sprite2D" parent="."]\nmaterial = SubResource("mat")\n'
          'texture = ExtResource("pic")\ncentered = false\n\n[node name="Anim" type="AnimationPlayer" parent="."]\n'
          'libraries = {\n&"": SubResource("lib")\n}\n')
    (OUT / f"inset_{name}.tscn").write_text(s, encoding="utf-8", newline="\n")

OUT.mkdir(parents=True, exist_ok=True)
if SRC != OUT:
    for n in ["womb", "ass", "mouth"]:
        for suf in ["", "_mask", "_prof"]: shutil.copyfile(SRC / f"{n}{suf}.png", OUT / f"{n}{suf}.png")
    for n in ["cock.png", "cock_prof.png"]: shutil.copyfile(SRC / n, OUT / n)
for n in ["inset.gdshader", "inset_path.gdshader"]: shutil.copyfile(HERE / n, OUT / n)

path = json.load(open(HERE / "mouth_path.json"))
pts = ", ".join(f"{x:.1f}, {y:.1f}" for x, y in path["pts"])
scene("womb", "inset.gdshader", {"tip_at": "Vector3(990, 815, 690)", "fill_at": "Vector2(705, 505)", "xc": 511.0, "entrance_y": 836.0,
                                 "inner_col": "Color(0.66, 0.33, 0.36, 1)", "rim_col": "Color(0.5, 0.2, 0.24, 1)"})
scene("ass", "inset.gdshader", {"tip_at": "Vector3(990, 650, 385)", "fill_at": "Vector2(395, 262)", "xc": 512.0, "entrance_y": 670.0})
scene("mouth", "inset_path.gdshader", {"pts": f"PackedVector2Array({pts})", "n": len(path["pts"]), "step_len": float(path["step"]),
                                       "tip_at": "Vector3(150, 640, 1165)", "fill_at": "Vector2(1015, 420)"})
print("built", sorted(p.name for p in OUT.iterdir()))
