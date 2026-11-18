"""Lactation pass 3: masks from the nipples downwards, both breasts. [Claude, 2026-11-18]
[Gorim 18 Nov 2026: "you need to widen to make sure you include both nipples. On the larger breasts you're also too far
above the nipples on the mask. I would basically include from the nipple itself downwards basically, to keep as much of
the original breast as possible."  Clothed prompt is his, as he wrote it: "Heavy lactation, milk oozing out, prominent
nipples through fabric, wet fabric, translucent" (it worked for him in the NovelAI UI on wrap stage 2).]
usage: python make_lact_jobs3.py <jobs dir>.   Nude wording = A from trial 2 (his pick)."""
import sys, json, numpy as np, cv2
from pathlib import Path
from PIL import Image
from scipy import ndimage
OUT = Path(sys.argv[1])
G = Path('C:/Users/Gorim/Documents/STS2Modding/Seductress/GorimCuts/Claude_2026-11-18_battle')
base = np.array(Image.open('C:/Users/Gorim/Documents/STS2Modding/Seductress/Battle1.png').convert('RGB'))
x0, y0, x1, y1 = 336, 352, 912, 736; H, W = y1 - y0, x1 - x0
yy, xx = np.mgrid[0:H, 0:W]
rep = json.load(open('parts_breasts/cut_report.json'))
STYLE = "warm tan skin, soft cel shading, anime illustration, clean lineart, dark grey background"
NUDE = ("1girl, solo, upper body, topless, nude, bare breasts, nipples, {s}, lactation, excessive milk, milk dripping from both nipples, "
        "white milk streams running down breasts and stomach, " + STYLE)
CLOTH = "Heavy lactation, milk oozing out, prominent nipples through fabric, wet fabric, translucent"
NEG_N = "hair over breasts, hair strands, bra straps, strap, text, watermark, extra nipples, extra arms, deformed, bad anatomy, milk puddle, dark nipples"
NEG_W = "text, watermark, extra arms, deformed, bad anatomy, milk puddle, dark nipples"
# nipples in window pixels (near = her right breast, far = by the raised arm), found by colour and checked by eye [Claude]
NIP = {"nude__flat": ((247, 129), (352, 135)), "nude__modest": ((267, 130), (364, 142)), "nude__bare": ((258, 134), (380, 153)),
       "nude__enormous": ((235, 140), (398, 160)), "nude__absurd": ((268, 166), (427, 184))}
SIZE = {"nude__flat": "flat chest", "nude__modest": "small breasts", "nude__bare": "large breasts", "nude__enormous": "huge breasts", "nude__absurd": "gigantic breasts"}
def piece(st):
    if st == "base":
        a = cv2.imread('parts/BREASTS_plus_wrap_variant.png', -1)[..., 3].copy(); a[:, 690:] = 0; full = base.copy()
    else:
        r = rep["BREASTS_" + st]; f = G / r["from"] / ("BREASTS_" + st) / r["seed_file"]
        full = base.copy(); full[y0:y1, x0:x1] = np.array(Image.open(f).convert('RGB').resize((W, H), Image.LANCZOS))
        pre = Path(f'parts_breasts_pretrim/BREASTS_{st}.png')
        a = cv2.imread(str(pre) if pre.exists() else f'parts_breasts/BREASTS_{st}.png', -1)[..., 3]
    fill = ndimage.binary_fill_holes(cv2.morphologyEx((a > 128).astype(np.uint8), cv2.MORPH_CLOSE, np.ones((15, 15), np.uint8))).astype(np.uint8)
    return full[y0:y1, x0:x1], fill[y0:y1, x0:x1]
for st in list(NIP) + ["base", "wrap__stage1", "wrap__stage2", "wrap__stage3"]:
    img, fill = piece(st)
    if st in NIP:
        (ax, ay), (bx, by) = NIP[st]
        line = ay + (by - ay) * (xx - ax) / (bx - ax) - 12                 # a little above both nipples
        if st in ("nude__flat", "nude__modest"):
            m = (xx >= ax - 36) & (xx <= bx + 18) & (yy >= line) & (yy <= line + 125)
        else:
            body = cv2.dilate(fill, np.ones((9, 9), np.uint8)).astype(bool) | ((np.abs(xx - bx) <= 16) & (np.abs(yy - by) <= 16))   # far nipple sits on the outline
            m = body & (yy >= line)
            core = cv2.erode(m.astype(np.uint8), np.ones((1, 61), np.uint8)).astype(bool); drip = core.copy()
            for dy in range(1, 51): drip[dy:] |= core[:-dy]
            m = m | drip
        prompt, neg = NUDE.format(s=SIZE[st]), NEG_N
    else:
        # the cloth from just under the top trim down, across both breasts, like Gorim's own mask [boxes by eye, Claude]
        bx0, by0, bx1, by1 = {"base": (208, 165, 390, 228), "wrap__stage1": (190, 170, 412, 226), "wrap__stage2": (215, 158, 425, 243),
                              "wrap__stage3": (160, 180, 448, 258)}[st]
        m = (xx >= bx0) & (xx <= bx1) & (yy >= by0) & (yy <= by1)
        prompt, neg = CLOTH, NEG_W
    m = m & (yy + y0 < 622)
    cells = (m.reshape(H // 8, 8, W // 8, 8).mean(axis=(1, 3)) > 0.4).repeat(8, 0).repeat(8, 1)
    d = OUT / f"LACT3_{st}"; d.mkdir(parents=True, exist_ok=True)
    Image.fromarray(img).save(d / "image.png"); Image.fromarray((cells * 255).astype(np.uint8), "L").save(d / "mask.png")
    json.dump({"layer": d.name, "state": st, "window": [x0, y0, W, H], "prompt": prompt, "negative": neg, "seeds": 2,
               "_about": "Masks from the nipples down, both breasts, as Gorim asked (18 Nov 2026). Clothed prompt is Gorim's own; nude prompt is wording A."},
              open(d / "job.json", "w"), indent=1)
    print(st, int(cells.sum()))
