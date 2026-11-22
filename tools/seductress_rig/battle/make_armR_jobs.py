"""Second round for her left arm (picture right) under the sleeve. [Claude, 2026-11-22; Gorim: "Left arm is fucked and
needs fixing. Try again with a few approaches."]
usage: python make_armR_jobs.py <battle work dir> <jobs root> <out jobs dir>
A plain painted arm (shoulder -> elbow -> the visible forearm) is put into the picture as a guide. Jobs:
  keep55 / keep70 / keep85   whole arm area masked, the model repaints it at 55 / 70 / 85 % strength over the guide
  ring                       only a ring around the guide's outline is masked (full strength); the guide's middle stays
Also writes guide.png (the painted arm alone, full canvas) as the no-AI fallback."""
import sys, json, shutil, numpy as np, cv2
from pathlib import Path
from PIL import Image
WORK, JOBS, OUT = Path(sys.argv[1]), Path(sys.argv[2]), Path(sys.argv[3])
D = json.load(open(WORK / "draw.json")); lab = np.load(WORK / "labels.npy"); H, W = lab.shape
ix = lambda *n: [D.index(k) for k in n]
ctx = np.array(Image.open(JOBS / "nai_jobs/STRIP_arms/context_full.png").convert("RGB"))
# what stays in front of the arm or beside it and must not be repainted
keep = np.isin(lab, [i for i in range(len(D)) if i > D.index("SLEEVE_R")]) | np.isin(
    lab, ix("TORSO_upper", "BREASTS_plus_wrap_variant", "HAIR_side_R", "HEAD_face_plus_front_hair", "BELLY_lower_torso_variant"))
S, E, F = (702, 490), (768, 574), (792, 560)          # shoulder joint, elbow, where the visible forearm starts [Claude, by eye]
def capsule(img, a, b, ra, rb, val):
    n = 40
    for i in range(n + 1):
        t = i / n; cv2.circle(img, (int(round(a[0] + (b[0] - a[0]) * t)), int(round(a[1] + (b[1] - a[1]) * t))), int(round(ra + (rb - ra) * t)), val, -1)
shape = np.zeros((H, W), np.uint8)
capsule(shape, S, E, 30, 25, 255); capsule(shape, E, F, 25, 23, 255)
shape = (shape > 0) & ~keep
SKIN, SHADE, LINE = np.array([211, 142, 108.]), np.array([186, 116, 88.]), np.array([122, 64, 48.])
yy, xx = np.mgrid[0:H, 0:W]
# shade the underside: distance below the arm's axis, 0 on the upper edge .. 1 on the lower edge
ax = np.array([E[0] - S[0], E[1] - S[1]], float); ax /= np.linalg.norm(ax); nrm = np.array([-ax[1], ax[0]])
side = ((xx - S[0]) * nrm[0] + (yy - S[1]) * nrm[1]) / 28.0
shade = np.clip((side - 0.25) / 0.6, 0, 1)[..., None]
guide = ctx.astype(np.float32).copy()
col = SKIN * (1 - shade) + SHADE * shade
guide[shape] = col[shape]
edge = shape & ~cv2.erode(shape.astype(np.uint8), np.ones((5, 5), np.uint8)).astype(bool)
edge &= ~cv2.dilate(keep.astype(np.uint8), np.ones((7, 7), np.uint8)).astype(bool)      # no outline where it meets shoulder or forearm
guide[edge] = LINE
guide = cv2.GaussianBlur(guide, (0, 0), 0.7) * shape[..., None] + guide * (1 - shape[..., None]) * 1.0
guide = np.where(shape[..., None], guide, ctx).clip(0, 255).astype(np.uint8)
g = np.zeros((H, W, 4), np.uint8); g[..., :3] = guide; g[..., 3] = shape * 255
OUT.mkdir(parents=True, exist_ok=True); Image.fromarray(g, "RGBA").save(OUT / "guide.png")
x0, y0, x1, y1 = 608, 352, 992, 736
def cells(a, fn): return fn(a[y0:y1, x0:x1].reshape(48, 8, 48, 8), axis=(1, 3))
def dil(m, r): return cv2.dilate(m.astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (2 * r + 1, 2 * r + 1))).astype(bool)
area = dil(shape, 14) & ~keep
full = cells(area, np.any) & ~cells(keep, np.all)
core = cv2.erode(shape.astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (25, 25))).astype(bool)
ring = full & ~cells(core, np.all)
PROMPT = ("1girl, solo, bare arm, bare shoulder, upper arm, elbow, bent arm, raised hand, nude arm, "
          "warm tan skin, soft cel shading, anime illustration, clean lineart, dark grey background")
NEG = "sleeves, detached sleeves, wide sleeves, armlet, arm band, cloth, purple cloth, cape, text, watermark, extra arms, deformed, bad anatomy"
for name, m, keepf in (("keep55", full, 0.55), ("keep70", full, 0.70), ("keep85", full, 0.85), ("ring", ring, 1.0)):
    d = OUT / f"ARM_R_{name}"; d.mkdir(exist_ok=True)
    Image.fromarray(guide[y0:y1, x0:x1]).save(d / "image.png")
    Image.fromarray((m.repeat(8, 0).repeat(8, 1) * 255).astype(np.uint8), "L").save(d / "mask.png")
    json.dump({"layer": f"ARM_R_{name}", "window": [x0, y0, x1 - x0, y1 - y0], "prompt": PROMPT, "negative": NEG, "seeds": 2,
               "inpaintImg2ImgStrength": keepf,
               "_about": "Prompt by Claude (22 Nov 2026). Context: rig at rest without sleeves, with a plain painted arm as a guide."},
              open(d / "job.json", "w"), indent=1)
v = guide[y0:y1, x0:x1].copy(); mm = full.repeat(8, 0).repeat(8, 1); v2 = v.copy(); v2[mm] = (v2[mm] * 0.6 + np.array([255, 0, 255]) * 0.4).astype(np.uint8)
Image.fromarray(np.hstack([v, v2])).save(OUT / "preview.png")
src = (JOBS / "nai_jobs/nai_jobs.py").read_text(encoding="utf-8")
src = src.replace('res = L.nai_call(L.nai_payload(job["prompt"], job["negative"], img, mask, ncfg, seed), ncfg)',
                  'nc = dict(ncfg); nc["inpaintImg2ImgStrength"] = job.get("inpaintImg2ImgStrength", nc.get("inpaintImg2ImgStrength", 1))\n'
                  '            res = L.nai_call(L.nai_payload(job["prompt"], job["negative"], img, mask, nc, seed), nc)')
assert "inpaintImg2ImgStrength" in src
(OUT / "nai_jobs.py").write_text(src, encoding="utf-8")
print("jobs ready")
