"""Prepare NovelAI inpaint jobs for the strip art on the battle pose. [Claude, 2026-11-22; asked for by the mod-code
session with Gorim's approval: bare arms under the sleeves, bare feet, body under the skirt]
usage: python make_strip_jobs.py <battle work dir with parts/, labels.npy, draw.json> <jobs dir>
Context is the rig at rest with the clothes of that strip stage hidden and every filler under them removed."""
import sys, json, numpy as np, cv2
from pathlib import Path
from PIL import Image
WORK, OUT = Path(sys.argv[1]), Path(sys.argv[2])
D = json.load(open(WORK / "draw.json")); lab = np.load(WORK / "labels.npy"); H, W = lab.shape
BG = (52, 53, 62)
idx = lambda *names: [D.index(n) for n in names]
def comp(hide, strip_under, extra=()):
    """rest pose without the parts in `hide`; all other parts lose what lies under the labels in `strip_under`"""
    under = np.isin(lab, idx(*strip_under))
    c = Image.new("RGBA", (W, H), BG + (255,))
    for n in D:
        if n == "HIP_ornaments_right":
            for e in extra: c.alpha_composite(Image.open(WORK / e))
        if n in hide: continue
        p = np.array(Image.open(WORK / "parts" / f"{n}.png")); p[under & (lab != D.index(n)), 3] = 0
        c.alpha_composite(Image.fromarray(p, "RGBA"))
    return np.array(c.convert("RGB"))
def grid8(m): return m.reshape(m.shape[0] // 8, 8, m.shape[1] // 8, 8).any(axis=(1, 3)).repeat(8, 0).repeat(8, 1)
def dil(m, r): return cv2.dilate(m.astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (2 * r + 1, 2 * r + 1))).astype(bool)
STYLE = "warm tan skin, soft cel shading, anime illustration, clean lineart, dark grey background"
SLEEVES = ["SLEEVE_L", "SLEEVE_R"]
SKIRT = ["CLOTH_front_skirt_panel", "SKIRT_rear_left", "SKIRT_rear_right", "HIP_ornaments_left", "HIP_ornaments_right"]
jobs = []
# 1. arms: what the two sleeves cover (arm, and the rear skirt behind the hanging cloth)
ctx = comp(SLEEVES, SLEEVES)
front = np.isin(lab, [i for i in range(len(D)) if i > D.index("SLEEVE_R")])
m = dil(np.isin(lab, idx(*SLEEVES)), 8) & ~front
jobs.append(("STRIP_arms", ctx, m, (272, 400, 976, 912),
             "1girl, solo, bare arms, bare shoulders, sleeveless, outstretched arms, bracelets, long purple skirt with gold trim, " + STYLE,
             "sleeves, detached sleeves, wide sleeves, armlet, arm band, cloth on arms, cape, text, watermark, extra arms, deformed, bad anatomy"))
# 2. feet: both shoes
ctx = comp(SLEEVES, SLEEVES)
yy, xx = np.mgrid[0:H, 0:W]
foot = (np.isin(lab, idx("LEG_L_shin_foot")) & (yy > 1070)) | (np.isin(lab, idx("LEG_R_shin_foot")) & (yy > 1055))
jobs.append(("STRIP_feet", ctx, dil(foot, 16) & (yy > 1050), (224, 1024, 992, 1344),
             "1girl, solo, barefoot, bare feet, toes, ankles, standing, legs apart, feet on the ground, " + STYLE,
             "shoes, sandals, high heels, heels, straps, anklet, socks, text, watermark, extra toes, deformed, bad anatomy"))
# 3. hips: what the skirt and its ornaments cover, with nude breasts above (the wrap comes off before the skirt)
hide = SLEEVES + SKIRT + ["BREASTS_plus_wrap_variant"]
ctx = comp(hide, SLEEVES + SKIRT, ["parts_breasts/BREASTS_nude__bare__under.png", "parts_breasts/BREASTS_nude__bare.png"])
m = dil(np.isin(lab, idx("CLOTH_front_skirt_panel", "HIP_ornaments_left", "HIP_ornaments_right")), 10) & (yy < 990) & (yy > 600)
jobs.append(("STRIP_hips", ctx, m, (400, 560, 848, 1008),
             "1girl, solo, completely nude, bottomless, pussy, navel, wide hips, thick thighs, standing, legs apart, " + STYLE,
             "skirt, loincloth, panties, cloth, belt, jewelry, censored, text, watermark, extra legs, deformed, bad anatomy"))
for name, ctx, m, (x0, y0, x1, y1), prompt, neg in jobs:
    d = OUT / name; d.mkdir(parents=True, exist_ok=True)
    Image.fromarray(ctx).save(d / "context_full.png"); Image.fromarray((m * 255).astype(np.uint8)).save(d / "mask_full.png")
    Image.fromarray(ctx[y0:y1, x0:x1]).save(d / "image.png")
    Image.fromarray((grid8(m[y0:y1, x0:x1]) * 255).astype(np.uint8), "L").save(d / "mask.png")
    json.dump({"layer": name, "window": [x0, y0, x1 - x0, y1 - y0], "prompt": prompt, "negative": neg, "seeds": 3,
               "_about": "Prompt by Claude (22 Nov 2026) for the strip stages Gorim decided. Context: battle rig at rest with that stage's clothes hidden."},
              open(d / "job.json", "w"), indent=1)
    v = ctx[y0:y1, x0:x1].copy(); mm = grid8(m[y0:y1, x0:x1]); v[mm] = (v[mm] * 0.5 + np.array([255, 0, 255]) * 0.5).astype(np.uint8)
    Image.fromarray(v).save(OUT / f"preview_{name}.png")
print(len(jobs), "jobs")
