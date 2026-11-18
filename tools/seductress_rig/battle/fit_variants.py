"""Fit the neutral-pose belly variants onto the battle pose: clean, tone-match, scale, common crop box. [Claude]
usage: python fit_variants.py <Seductress folder> <out dir for full-canvas PNGs>"""
import sys, os, numpy as np, cv2
from PIL import Image
SRC, OUT = sys.argv[1], sys.argv[2]; os.makedirs(OUT, exist_ok=True)
V = f"{SRC}/GorimCuts/Claude_2026-10-24/variants"
S = 0.85; A = (628, 543); Bp = (650, 621)        # neutral navel -> battle navel, uniform scale, by eye
neu = np.array(Image.open(f"{SRC}/DefaultNeutral.png").convert("RGB")).astype(float)
bat = np.array(Image.open(f"{SRC}/Battle1.png").convert("RGB")).astype(float)
GAIN = np.median(bat[585:612, 600:690].reshape(-1, 3), 0) / np.median(neu[495:525, 575:670].reshape(-1, 3), 0)
def fit(name):
    a = np.array(Image.open(f"{V}/{name}.png").convert("RGBA")).astype(np.float32)
    r, g, b, al = a[..., 0], a[..., 1], a[..., 2], a[..., 3]
    skin = (al > 0) & (r - b > 38) & (r > g) & (b > 0.36 * r)          # drops wrap/dark scraps and gold bits
    k, cc, st, _ = cv2.connectedComponentsWithStats(skin.astype(np.uint8), 8)
    main = cc == (1 + np.argmax(st[1:, 4]))                             # keep the one big piece
    main = cv2.morphologyEx(main.astype(np.uint8), cv2.MORPH_CLOSE, np.ones((5, 5), np.uint8)).astype(bool) & (al > 0)
    core = cv2.morphologyEx(main.astype(np.uint8), cv2.MORPH_OPEN, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (19, 19)))
    main &= cv2.dilate(core, np.ones((7, 7), np.uint8)).astype(bool)        # trims thin tabs left from the neutral cut
    a[..., 3] = np.where(main, al, 0)
    a[..., :3] = np.clip(a[..., :3] * GAIN, 0, 255)
    im = Image.fromarray(a.astype(np.uint8), "RGBA")
    return im.transform(im.size, Image.AFFINE, (1 / S, 0, A[0] - Bp[0] / S, 0, 1 / S, A[1] - Bp[1] / S), Image.BICUBIC)
if __name__ == "__main__":
    for k in range(1, 7):
        im = fit(f"BELLY_lower_torso_variant__Preg{k}"); im.save(f"{OUT}/BELLY_stage{k}.png"); print(k, im.getbbox())
