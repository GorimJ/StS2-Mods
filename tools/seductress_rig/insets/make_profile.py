"""[Claude 2026-11-20] Measure each inset picture: which pixels are the inside of the canal/cavity (mask), and per row the
canal's centre and half-width (profile).  Also the cock sprite's half-width per row.  usage: python make_profile.py <proj dir>"""
import sys, numpy as np, cv2
from PIL import Image
from scipy.ndimage import gaussian_filter1d, label
OUT = sys.argv[1]; PICKS = "C:/Users/Gorim/Documents/STS2Modding/Seductress/inset_concepts/picks/"
INS = {"ass":  dict(src="ass_D3_v45.png", top=235, ent=672, seed=(512, 300), test=lambda r, g, b: (g < 150) & (r > 150), xs=(440, 590)),
       "womb": dict(src="womb_A5_v5.png", top=500, ent=838, seed=(512, 560), test=lambda r, g, b: (g < 125) & (r > 120), xs=(440, 592))}
for name, c in INS.items():
    im = Image.open(PICKS + c["src"]).convert("RGB"); im.save(f"{OUT}/{name}.png")
    a = np.array(im).astype(int); r, g, b = a[..., 0], a[..., 1], a[..., 2]
    m = c["test"](r, g, b); m[:c["top"]] = False; m[c["ent"]:] = False; m[:, :c["xs"][0]] = False; m[:, c["xs"][1]:] = False
    m = cv2.morphologyEx(m.astype(np.uint8), cv2.MORPH_OPEN, np.ones((3, 3), np.uint8))
    if name == "ass":   # near the opening the shaded skin is as red as the canal: keep only a narrow strip there
        m[560:, :502] = 0; m[560:, 521:] = 0
    # join rows: close vertically so thin breaks in the canal do not split it
    mc = cv2.morphologyEx(m, cv2.MORPH_CLOSE, np.ones((25, 3), np.uint8))
    lab, n = label(mc); keep = lab == lab[c["seed"][1], c["seed"][0]]; m = (m > 0) & keep
    Image.fromarray((m * 255).astype(np.uint8)).filter(__import__("PIL.ImageFilter", fromlist=["x"]).GaussianBlur(1.2)).save(f"{OUT}/{name}_mask.png")
    cen = np.full(1024, np.nan); hw = np.full(1024, np.nan)
    for y in range(1024):
        xs = np.nonzero(m[y])[0]
        if len(xs): cen[y] = (xs[0] + xs[-1]) / 2; hw[y] = (xs[-1] - xs[0] + 1) / 2
    ok = ~np.isnan(cen); ys = np.arange(1024)
    cen = np.interp(ys, ys[ok], cen[ok]); hw = np.interp(ys, ys[ok], hw[ok])
    cen = gaussian_filter1d(cen, 1.5); hw = np.maximum(gaussian_filter1d(hw, 1.5), 2.5)   # light smoothing only: the stretch must follow the real wavy walls to straighten them
    p = np.zeros((1024, 4, 4), np.uint8); p[..., 0] = np.clip(cen - 512 + 128, 0, 255).round()[:, None]; p[..., 1] = np.clip(hw * 2, 0, 255).round()[:, None]; p[..., 3] = 255
    Image.fromarray(p, "RGBA").save(f"{OUT}/{name}_prof.png")
    print(name, "rows", ys[ok].min(), ys[ok].max(), "entrance centre", cen[ys[ok].max()].round(1))
    for y in range(ys[ok].min(), ys[ok].max(), 30): print("  y", y, "centre", round(cen[y], 1), "half-width", round(hw[y], 1))
ck = np.array(Image.open(f"{OUT}/cock.png").convert("RGBA"))[..., 3] > 60
w = gaussian_filter1d(ck.sum(1) / 2.0, 2.5)
p = np.zeros((ck.shape[0], 4, 4), np.uint8); p[..., 0] = np.clip(w, 0, 255).round()[:, None]; p[..., 3] = 255
Image.fromarray(p, "RGBA").save(f"{OUT}/cock_prof.png"); print("cock half-width px (unscaled) every 60 rows:", w[::60].round(0))
