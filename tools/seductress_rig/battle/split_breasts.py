"""Split the two largest nude breast pieces into a near breast (her right, drawn on top) and a far breast.
[Gorim 2026-11-18: at the larger nude sizes the individual breasts should move independently]
The cut follows the near breast's drawn outline. The far piece gets painted skin continued under the near one,
so nothing opens when they move apart.   usage: python split_breasts.py   -> parts_breasts/<name>__near.png, __far.png
"""
import numpy as np, cv2
from PIL import Image
D = "parts_breasts"
SEED = {"BREASTS_nude__enormous": (646, 520), "BREASTS_nude__absurd": (662, 520)}   # a point on the dividing outline [Claude]
UNDERLAP = 16
TAIL = {"BREASTS_nude__absurd": (545, 620)}    # a point inside the flank strip to remove [Claude]
JUNC = {"BREASTS_nude__enormous": 548, "BREASTS_nude__absurd": 584}   # row where the far breast's lower outline branches off [Claude]
for name, (sx, sy) in SEED.items():
    im = np.array(Image.open(f"{D}/{name}.png").convert("RGBA"))
    if name in TAIL:
        # [Gorim 2026-11-18] "a weird hanging bit on the bottom left": a strip of flank skin below the near breast.
        # Remove the skin region holding the seed, plus the dark line around it that no other skin touches.
        from scipy import ndimage as ndi
        al = im[..., 3] > 40
        dark = (im[..., :3].astype(np.float32) @ np.array([0.3, 0.59, 0.11], np.float32)) < 120
        lab, _ = ndi.label(al & ~dark)
        tx, ty = TAIL[name]; tail = lab == lab[ty, tx]
        other = ndi.binary_dilation(al & ~dark & ~tail, iterations=4)
        kill = tail | (ndi.binary_dilation(tail, iterations=8) & al & ~other)
        im[..., 3] = np.where(kill, 0, im[..., 3])
        # small skin scraps left beside it (shadow tones), then anything no longer joined to the breasts
        for i in range(1, lab.max() + 1):
            c = lab == i
            if 0 < c.sum() < 400 and (c & ndi.binary_dilation(kill, iterations=6)).any(): im[..., 3][c] = 0
        keep = ndi.binary_opening(im[..., 3] > 40, iterations=2)
        l2, n2 = ndi.label(keep); big = l2 == (1 + np.argmax(ndi.sum(keep, l2, range(1, n2 + 1))))
        im[..., 3] = np.where(ndi.binary_dilation(big, iterations=3), im[..., 3], 0)
        print(name, "tail removed:", int(tail.sum()), "px skin,", int(kill.sum()), "px in all")
    a = im[..., 3] > 40
    lum = im[..., :3].astype(np.float32) @ np.array([0.3, 0.59, 0.11], np.float32)
    lum[~a] = 255
    H, W = a.shape
    line = np.full(H, -1, int); onl = np.zeros(H, bool)
    Y0, Y1 = np.nonzero(a.any(1))[0][[0, -1]]
    def track(y, x, step):
        dx_hist = []
        while Y0 <= y <= Y1:
            x = max(3, min(W - 4, x)); xs = np.arange(x - 2, x + 3)
            v = lum[y, xs]
            if v.min() > 120:                       # outline ended: go straight on in the last direction
                d = int(round(np.mean(dx_hist[-12:]))) if dx_hist else 0
                d = max(-1, min(1, d)); x += d
            else:
                nx = int(xs[np.argmin(v + 0.5 * abs(xs - x))])
                if step > 0 and y >= JUNC[name]:
                    line[y:Y1 + 1] = x; return           # the far breast's lower outline branches off here: cut straight down
                dx_hist.append(nx - x); x = nx; onl[y] = True
            line[y] = x; y += step
    x0 = sx + int(np.argmin(lum[sy, sx - 8:sx + 9])) - 8
    track(sy, x0, 1); track(sy - 1, x0, -1)
    xx = np.arange(W)[None, :]
    near_m = a & (xx <= line[:, None] + 2)
    far_m = a & ~near_m
    near = im.copy(); near[..., 3] = np.where(near_m, im[..., 3], 0)
    # where the cut runs through plain skin (no drawn outline), fade the near piece out over a few px
    ramp = np.clip((line[:, None] + 2 - xx) / 8.0, 0, 1)
    near[..., 3] = np.where(onl[:, None], near[..., 3], (near[..., 3] * ramp).astype(np.uint8))
    # far: its own pixels + skin continued under the near breast
    grow = cv2.dilate(far_m.astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (2 * UNDERLAP + 1,) * 2)) > 0
    fill = grow & near_m
    # inpaint away the outline too (3 px beyond the cut on the far side), so the fill is plain skin
    hole = (fill | (a & (xx > line[:, None] + 2) & (xx <= line[:, None] + 5) & grow)).astype(np.uint8) * 255
    rgb = cv2.inpaint(im[..., :3][..., ::-1].copy(), hole, 6, cv2.INPAINT_TELEA)[..., ::-1]
    far = np.dstack([rgb, np.where(far_m | fill, 255, 0).astype(np.uint8)])
    far[..., 3] = np.where(far_m, im[..., 3], far[..., 3])
    # soften the hidden fill's inner edge
    al = far[..., 3].astype(np.float32); bl = cv2.GaussianBlur(al, (0, 0), 2)
    far[..., 3] = np.where(fill, np.minimum(al, bl * 1.0), al).astype(np.uint8)
    Image.fromarray(near).save(f"{D}/{name}__near.png"); Image.fromarray(far).save(f"{D}/{name}__far.png")
    ys = np.nonzero(a.any(1))[0]
    print(name, "line", [(int(y), int(line[y])) for y in range(ys.min(), ys.max() + 1, 20)])
    # check sheet
    box = (470, 440, 790, 660)
    def on(bg, *layers):
        c = Image.new("RGBA", (W, H), bg)
        for l in layers: c.alpha_composite(Image.fromarray(l))
        return c.crop(box).resize(((box[2] - box[0]) * 2, (box[3] - box[1]) * 2))
    sh = np.roll(near, -6, axis=1)
    tiles = [on((60, 120, 60, 255), near), on((60, 120, 60, 255), far), on((60, 120, 60, 255), far, near), on((60, 120, 60, 255), far, sh)]
    s = Image.new("RGBA", (tiles[0].width * 2, tiles[0].height * 2))
    for i, t in enumerate(tiles): s.paste(t, ((i % 2) * t.width, (i // 2) * t.height))
    s.save(f"split/{name}_split.png")
