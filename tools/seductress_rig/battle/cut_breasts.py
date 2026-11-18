"""Turn the NovelAI breast results into full-canvas overlay parts for the battle rig. [Claude, 2026-11-18]
usage: python cut_breasts.py <out dir> <jobs dir> [<jobs dir> ...]     (a later jobs dir replaces same-named jobs)
Per job: picks a seed (PICK, else the one closest to Battle1's skin tone), matches its skin tone to the body,
drops painted backdrop, and keeps only what NovelAI really changed (breasts, wrap) with a soft edge."""
import sys, json, numpy as np, cv2
from pathlib import Path
from PIL import Image
from scipy import ndimage
OUT = Path(sys.argv[1]); OUT.mkdir(parents=True, exist_ok=True)
PICK = json.load(open("breast_pick.json")) if Path("breast_pick.json").exists() else {}     # {"job name": "result_s2.png"}
base = np.array(Image.open('C:/Users/Gorim/Documents/STS2Modding/Seductress/Battle1.png').convert('RGB')).astype(np.float32)
H, W = base.shape[:2]; yy, xx = np.mgrid[0:H, 0:W]
lab = np.load("labels.npy"); DRAW = json.load(open("draw.json"))
def skinmask(a):
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    return (r > 140) & (r - b > 35) & (r > g) & (g > b)
ref = base[np.isin(lab, [DRAW.index("TORSO_upper"), DRAW.index("BELLY_lower_torso_variant")]) & skinmask(base)]
MU_R, SD_R = ref.mean(0), ref.std(0)
BG = np.median(base[20:120, 20:120].reshape(-1, 3), 0)
jobs = {}
for jd in sys.argv[2:]:
    for d in sorted(p for p in Path(jd).iterdir() if (p / "job.json").exists()): jobs[d.name] = d
report = {}
for name, d in jobs.items():
    job = json.load(open(d / "job.json")); x0, y0, w, h = job["window"]; cx, cy, a, b = job["ellipse"]
    ell = (((xx - cx) / a) ** 2 + ((yy - cy) / b) ** 2 <= 1) & (yy >= 450)
    best = None
    for f in sorted(d.glob("result_s*.png")):
        if name in PICK and f.name != PICK[name]: continue
        res = np.array(Image.open(f).convert('RGB').resize((w, h), Image.LANCZOS)).astype(np.float32)
        full = base.copy(); full[y0:y0 + h, x0:x0 + w] = res
        sk = skinmask(full) & ell
        err = float(np.abs(full[sk].mean(0) / MU_R - 1).sum()) if sk.sum() > 500 else 9
        if best is None or err < best[0]: best = (err, f.name, full)
    err, fname, full = best
    # what changed: breasts and wrap. The repainted midriff is left out so a belly stage can show under the breasts.
    changed = np.abs(full - base).sum(2) > 60
    m = (changed | ((lab == DRAW.index("BREASTS_plus_wrap_variant")) & (xx < 680))) & ell     # not the knot tail on her right
    m = cv2.morphologyEx(m.astype(np.uint8), cv2.MORPH_OPEN, np.ones((5, 5), np.uint8))
    m = cv2.morphologyEx(m, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (31, 31)))
    k, cc, st, _ = cv2.connectedComponentsWithStats(m, 8)
    m = ndimage.binary_fill_holes(cc == (1 + np.argmax(st[1:, 4])))
    m = cv2.morphologyEx(m.astype(np.uint8), cv2.MORPH_OPEN, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (41, 41))).astype(bool)   # drop small tags (repainted hair wisps, sleeve edge)
    up = m.copy()
    for dy in range(1, 34): up[:-dy] |= m[dy:]                       # reach upward, past the old wrap's top line
    m = cv2.dilate(up.astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (17, 17)))
    m = (cv2.GaussianBlur(m.astype(np.float32), (0, 0), 7) > 0.5) & ell      # smooth outline
    # skin tone: bring the repainted skin's mean and spread to the body's
    sk = skinmask(full) & m
    mu, sd = full[sk].mean(0), full[sk].std(0)
    scale = np.clip(SD_R / np.maximum(sd, 1), 0.75, 1.3)
    matched = (full - mu) * scale + MU_R
    soft = cv2.GaussianBlur((skinmask(full) & ell).astype(np.float32), (0, 0), 2.0)[..., None]
    full = np.clip(full * (1 - soft) + matched * soft, 0, 255)
    t = np.clip(ndimage.distance_transform_edt(m) / 12.0, 0, 1); alpha = t * t * (3 - 2 * t)
    notbg = (np.abs(full - BG).sum(2) > 45).astype(np.uint8)          # painted backdrop would show as a smudge
    notbg = cv2.morphologyEx(notbg, cv2.MORPH_OPEN, np.ones((3, 3), np.uint8))
    alpha = alpha * cv2.GaussianBlur(notbg.astype(np.float32), (0, 0), 1.0)
    # two pieces [Gorim 2026-11-18: bellies go under the new breasts, but over the stomach NovelAI painted]:
    #   <name>__under.png = everything repainted (stomach included), drawn below the belly stages
    #   <name>.png        = the breasts alone, cut along their lower outline, drawn above the belly stages
    te = np.clip(ndimage.distance_transform_edt(ell) / 14.0, 0, 1)
    a_under = te * te * (3 - 2 * te) * cv2.GaussianBlur(notbg.astype(np.float32), (0, 0), 1.0)
    Image.fromarray(np.dstack([full, a_under * 255]).astype(np.uint8), "RGBA").save(OUT / f"{name}__under.png")
    strong = ((full.max(2) < 125) | ((full[..., 2] > full[..., 1]) & (full[..., 0] < 150))) & m & (yy < cy + 0.8 * b)
    yb = np.where(strong.any(0), H - 1 - np.argmax(strong[::-1], 0), -1).astype(np.float32)   # lowest outline/cloth px
    if "nude" in name:
        # a breast's outline is a few px thick; a tall dark run under it is repainted hair or sleeve, so stop at
        # the top of such a run
        for x in np.nonzero(yb >= 0)[0]:
            y = int(yb[x]); top = y
            while top > 0 and strong[top - 1, x]: top -= 1
            if y - top > 8: yb[x] = top + 4
    cols = np.nonzero(m.any(0))[0]; have = cols[yb[cols] >= 0]
    line = np.full(W, -1, np.float32)
    line[cols] = np.interp(cols, have, yb[have])
    line[cols] = ndimage.median_filter(line[cols], 25, mode="nearest")
    mid = cols[len(cols) // 5: -len(cols) // 5]                     # the sides must not dip below the breasts' lowest
    line[cols] = np.minimum(line[cols], np.percentile(line[mid], 90 if "nude" in name else 55))   # point: that is repainted hair or sleeve
    line[cols] = ndimage.gaussian_filter1d(line[cols], 7, mode="nearest")
    # only a few px below the outline: anything more is stomach shading or repainted hair and belongs underneath
    over = m & (yy <= line[None, :] + (3 if "nude" in name else 5))
    t = np.clip(ndimage.distance_transform_edt(over) / 5.0, 0, 1)
    a2 = t * t * (3 - 2 * t) * cv2.GaussianBlur(notbg.astype(np.float32), (0, 0), 1.0)
    Image.fromarray(np.dstack([full, a2 * 255]).astype(np.uint8), "RGBA").save(OUT / f"{name}.png")
    report[name] = {"from": str(d.parent.name), "seed_file": fname, "tone_error_before": round(err, 3)}
    print(name, report[name])
# the base wrap's lower hem as its own piece: it is drawn over the belly stages so their top edge is hidden
p = np.array(Image.open("parts/BREASTS_plus_wrap_variant.png").convert("RGBA")).astype(np.float32)
wrap = lab == DRAW.index("BREASTS_plus_wrap_variant")
yb = np.where(wrap.any(0), H - 1 - np.argmax(wrap[::-1], 0), -1)            # lowest wrap pixel per column
yb = ndimage.maximum_filter1d(yb, 9)
dist = yb[None, :] - yy                                                    # px above the hem's lower edge
t = np.clip((44 - dist) / 14.0, 0, 1) * (yb[None, :] >= 0)
p[..., 3] *= t * t * (3 - 2 * t) * np.clip((674 - xx) / 10.0, 0, 1)        # without the ragged knot tail
Image.fromarray(p.astype(np.uint8), "RGBA").save(OUT / "BREASTS_base_hem.png")
json.dump(report, open(OUT / "cut_report.json", "w"), indent=1)
