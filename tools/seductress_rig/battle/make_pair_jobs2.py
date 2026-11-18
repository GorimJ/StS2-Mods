"""Second version of the pair jobs: NovelAI repaints the lower breasts AND the upper half of the belly, so the belly
can start right under the bust and the breasts can rest on it. [Claude, 2026-11-18; Gorim: belly starts too low,
pinch on her left, leftovers on her right]      usage: python make_pair_jobs2.py <jobs dir>"""
import sys, json, numpy as np, cv2
from pathlib import Path
from PIL import Image
OUT = Path(sys.argv[1])
WIN = (336, 352, 912, 864)                      # 576x512: tall enough for the biggest bellies, still a free size
base = np.array(Image.open('C:/Users/Gorim/Documents/STS2Modding/Seductress/Battle1.png').convert('RGB'))
BG = tuple(int(v) for v in np.median(base[20:120, 20:120].reshape(-1, 3), 0))
lab = np.load("labels.npy"); DRAW = json.load(open("draw.json"))
H, W = lab.shape; yy, xx = np.mgrid[0:H, 0:W]
P = {n: Image.open(f'parts/{n}.png').convert('RGBA') for n in DRAW}
def composite(b, k):
    c = Image.new('RGBA', (W, H), BG + (255,))
    for n in DRAW:
        if not (b and n == 'BREASTS_plus_wrap_variant'): c.alpha_composite(P[n])
        if n == "HIP_ornaments_right":
            if b: c.alpha_composite(Image.open(f'parts_breasts/BREASTS_{b}__under.png').convert('RGBA'))
            c.alpha_composite(Image.open(f'parts_var/BELLY_stage{k}.png').convert('RGBA'))
            c.alpha_composite(Image.open(f'parts_breasts/BREASTS_{b}.png' if b else 'parts_breasts/BREASTS_base_hem.png').convert('RGBA'))
    return np.array(c.convert('RGB'))
SIZE = {"nude__flat": "flat chest", "nude__modest": "small breasts", "nude__bare": "large breasts", "nude__enormous": "huge breasts",
        "nude__absurd": "gigantic breasts", "wrap__stage1": "huge breasts", "wrap__stage2": "gigantic breasts",
        "wrap__stage3": "gigantic breasts, hyper breasts", None: "large breasts"}
BELLY = {1: "pregnant, small baby bump", 2: "pregnant, round pregnant belly", 3: "pregnant, big round pregnant belly",
         4: "pregnant, big round pregnant belly", 5: "pregnant, huge pregnant belly, hyper pregnancy", 6: "pregnant, huge pregnant belly, hyper pregnancy"}
STYLE = "warm tan skin, soft cel shading, anime illustration, clean lineart, dark grey background"
NEG = ("belly chain, waist chain, jewelry on belly, piercing, frills, peplum, flounce, skirt under breasts, babydoll, shirt, dress, tabard, apron, long top, cloth covering belly, covered belly, "
       "flat stomach, abs, waist, narrow waist, hourglass, hair over breasts, hair strands, bra straps, strap, text, watermark, "
       "extra nipples, extra navel, extra arms, deformed, bad anatomy")
n = 0
for b in [None] + [k for k in SIZE if k]:
    over = np.array(Image.open(f'parts_breasts/BREASTS_{b}.png' if b else 'parts/BREASTS_plus_wrap_variant.png'))[..., 3] > 128
    low = np.where(over.any(0), H - 1 - np.argmax(over[::-1], 0), -1); low = low[low >= 0]
    y_hi, y_lo = np.percentile(low, 15), np.percentile(low, 95)
    cloth = b is None or "wrap" in b
    top = (np.percentile(low, 50) - 14) if cloth else (y_hi - 34)
    xr_top = min(int(np.nonzero(over.any(0))[0].max()) + 8, 760)
    for k in range(1, 7):
        belly = np.array(Image.open(f'parts_var/BELLY_stage{k}.png'))[..., 3] > 128
        rows = np.nonzero(belly.any(1))[0]
        xr = np.array([np.nonzero(belly[y])[0].max() for y in rows])
        y_w, x_w = int(rows[np.argmax(xr)]), int(xr.max())                 # the belly's widest point
        y_bot = min(y_w + 6, 846)
        # right edge: a straight line from beside the breasts down to just outside the belly's widest point, so
        # there is room to redraw the belly's upper outline but the raised arm stays out of the mask
        line = xr_top + (x_w + 14 - xr_top) * np.clip((yy - top) / max(y_bot - top, 1), 0, 1)
        m = (yy >= top) & (yy <= y_bot) & (xx > 530) & (xx < line) & (xx < 905)
        img = composite(b, k)
        x0, y0, x1, y1 = WIN
        cells = m[y0:y1, x0:x1].reshape((y1 - y0) // 8, 8, (x1 - x0) // 8, 8).any(axis=(1, 3)).repeat(8, 0).repeat(8, 1)
        bel = BELLY[k]
        if b is None: what = f"{SIZE[b]}, cleavage, short crossed dark purple cloth chest wrap, belly starting directly under the chest wrap, wrap resting on top of belly, bare belly, exposed belly, bare skin, navel"
        elif cloth: what = f"{SIZE[b]}, cleavage, short dark purple strapless bandeau, belly starting directly under the bandeau, breasts resting on top of belly, bare belly, exposed belly, bare skin, navel"
        elif b in ("nude__flat", "nude__modest"): what = f"topless, nude, {SIZE[b]}, nipples, belly starting directly under the chest, bare skin, navel"
        else: what = f"topless, nude, {SIZE[b]}, heavy breasts resting on top of belly, breasts pressed against belly, belly starting directly under breasts, bare skin, navel"
        prompt = f"1girl, solo, upper body, {bel}, {what}, {STYLE}"
        name = f"PAIR_{b or 'base'}__belly{k}"
        d = OUT / name; d.mkdir(parents=True, exist_ok=True)
        Image.fromarray(img[y0:y1, x0:x1]).save(d / "image.png")
        Image.fromarray((cells * 255).astype(np.uint8), "L").save(d / "mask.png")
        json.dump({"layer": name, "window": [x0, y0, x1 - x0, y1 - y0], "prompt": prompt, "negative": NEG, "seeds": 1,
                   "_about": "Prompt by Claude for Gorim (18 Nov 2026), version 2: mask now includes the upper half of the belly."},
                  open(d / "job.json", "w"), indent=1)
        n += 1
print(n, "jobs")
