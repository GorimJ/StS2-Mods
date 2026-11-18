"""Prepare NovelAI inpaint jobs for breast variants on the battle pose. [Claude, 2026-11-18, on Gorim's request]
usage: python make_breast_jobs.py <jobs dir>.  Each job: image.png (window of Battle1), mask.png, job.json."""
import sys, json, numpy as np, cv2
from pathlib import Path
from PIL import Image
OUT = Path(sys.argv[1])
SRC = 'C:/Users/Gorim/Documents/STS2Modding/Seductress/Battle1.png'
WIN = (336, 352, 912, 736)            # x0,y0,x1,y1: 576x384, multiples of 64, well inside the free size
rgb = np.array(Image.open(SRC).convert('RGB'))
H, W = rgb.shape[:2]; yy, xx = np.mgrid[0:H, 0:W]
C = (622, 538)
def mask(a, b, top=450):
    m = (((xx - C[0]) / a) ** 2 + ((yy - C[1]) / b) ** 2 <= 1) & (yy >= top)
    return m
STYLE = "warm tan skin, soft cel shading, anime illustration, clean lineart, dark grey background"
NEG = "hair over breasts, hair strands, bra straps, strap, text, watermark, extra nipples, extra arms, deformed, bad anatomy"
NUDE = "1girl, solo, upper body, topless, nude, bare breasts, nipples, {s}, " + STYLE
WRAP = "1girl, solo, upper body, {s}, cleavage, dark purple bandeau, strapless purple cloth chest wrap with gold trim, bare midriff, " + STYLE
JOBS = [("BREASTS_nude__flat", NUDE.format(s="flat chest"), (100, 78)),
        ("BREASTS_nude__modest", NUDE.format(s="small breasts"), (100, 78)),
        ("BREASTS_nude__bare", NUDE.format(s="large breasts"), (104, 84)),
        ("BREASTS_nude__enormous", NUDE.format(s="huge breasts"), (128, 104)),
        ("BREASTS_nude__absurd", NUDE.format(s="gigantic breasts"), (150, 128)),
        ("BREASTS_wrap__stage1", WRAP.format(s="huge breasts"), (120, 96)),
        ("BREASTS_wrap__stage2", WRAP.format(s="gigantic breasts"), (138, 114)),
        ("BREASTS_wrap__stage3", WRAP.format(s="gigantic breasts, hyper breasts"), (156, 132))]
x0, y0, x1, y1 = WIN
for name, prompt, (a, b) in JOBS:
    d = OUT / name; d.mkdir(parents=True, exist_ok=True)
    Image.fromarray(rgb[y0:y1, x0:x1]).save(d / "image.png")
    m = mask(a, b)[y0:y1, x0:x1]
    cells = m.reshape((y1 - y0) // 8, 8, (x1 - x0) // 8, 8).any(axis=(1, 3)).repeat(8, 0).repeat(8, 1)
    Image.fromarray((cells * 255).astype(np.uint8), "L").save(d / "mask.png")
    json.dump({"layer": name, "window": [x0, y0, x1 - x0, y1 - y0], "ellipse": [C[0], C[1], a, b], "prompt": prompt,
               "negative": NEG, "seeds": 2,
               "_about": "Prompt by Claude on Gorim's request (18 Nov 2026). Context: Battle1.png as is; mask = ellipse over the chest."},
              open(d / "job.json", "w"), indent=1)
print(len(JOBS), "jobs")
