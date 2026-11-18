"""Fit the neutral-pose belly variants onto the battle pose: clean, tone-match, scale, common crop box. [Claude]
usage: python fit_variants.py <Seductress folder> <out dir for full-canvas PNGs>"""
import sys, os, json, numpy as np, cv2
from scipy import ndimage
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
    core = cv2.morphologyEx(main.astype(np.uint8), cv2.MORPH_OPEN, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (45, 45)))
    main &= cv2.dilate(core, np.ones((3, 3), np.uint8)).astype(bool)        # trims thin tabs left from the neutral cut
    # notches left where the neutral pose's wrap tail and ornaments overlapped the belly: close them and paint them in
    closed = cv2.morphologyEx(main.astype(np.uint8), cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (61, 61))).astype(bool)
    hole = closed & ~main
    if hole.any():
        rgb8 = cv2.inpaint(a[..., :3].astype(np.uint8), cv2.dilate(hole.astype(np.uint8), np.ones((5, 5), np.uint8)), 6, cv2.INPAINT_TELEA)
        a[..., :3] = np.where(hole[..., None], rgb8, a[..., :3]); al = np.where(hole, 255, al); main = closed
    a[..., 3] = np.where(main, al, 0)
    a[..., :3] = np.clip(a[..., :3] * GAIN, 0, 255)
    im = Image.fromarray(a.astype(np.uint8), "RGBA")
    return im.transform(im.size, Image.AFFINE, (1 / S, 0, A[0] - Bp[0] / S, 0, 1 / S, A[1] - Bp[1] / S), Image.BICUBIC)
def blend(im, lab, DRAW):
    """Fade the belly out where it meets the base body's skin, so no join shows."""
    a = np.array(im).astype(np.float32); m = a[..., 3] > 8
    yy, xx = np.mgrid[0:a.shape[0], 0:a.shape[1]]
    skin = np.isin(lab, [DRAW.index("BELLY_lower_torso_variant"), DRAW.index("TORSO_upper")])
    wrap = lab == DRAW.index("BREASTS_plus_wrap_variant")
    # above the waist the belly may only sit on her torso: its top-right corner (hidden under the wrap in the neutral
    # art) otherwise pokes out over the sleeve and hair beside her
    # a smooth line down her right side (by eye from Battle1), opening out to the full belly by y=640
    edge = 696 + np.clip(yy - 575, 0, None) * 0.15 + np.clip(yy - 600, 0, None) ** 2 * 0.045
    a[..., 3] *= np.clip((edge - xx) / 2.0 + 0.5, 0, 1)
    # give that new edge the same dark outline the rest of the belly has
    ol = np.clip(1 - np.abs(edge - xx - 1.5) / 2.5, 0, 1) * (yy < 640) * (yy > 566) * 0.8
    a[..., :3] = a[..., :3] * (1 - ol[..., None]) + np.array([118, 62, 48], np.float32) * ol[..., None]
    m = a[..., 3] > 128
    near = cv2.dilate(m.astype(np.uint8), np.ones((7, 7), np.uint8)).astype(bool) & ~m
    # fade wherever the belly meets the body: skin on its flank, and its whole top edge (the wrap hem, the breasts
    # or the painted stomach sit there). Its right-hand silhouette past x=715 stays hard.
    top = np.where(m.any(0), np.argmax(m, 0), 10 ** 6)
    above = near & (yy <= top[None, :] + 1) & (xx < 678)        # the clipped side edge stays crisp
    t = np.clip(ndimage.distance_transform_edt(~((near & (skin | wrap)) | above)) / 18.0, 0, 1)
    fade = t * t * (3 - 2 * t)
    a[..., 3] *= fade        # the top edge is covered by the wrap's hem (BREASTS_base_hem) or by a breast variant
    return Image.fromarray(a.astype(np.uint8), "RGBA")
if __name__ == "__main__":
    lab = np.load("labels.npy"); DRAW = json.load(open("draw.json"))
    for k in range(1, 7):
        im = blend(fit(f"BELLY_lower_torso_variant__Preg{k}"), lab, DRAW); im.save(f"{OUT}/BELLY_stage{k}.png"); print(k, im.getbbox())
