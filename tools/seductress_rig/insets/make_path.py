"""[Claude 2026-11-21] Mouth inset: measure the throat tube (mask), the curved centre path from outside the mouth down the
throat, and the tube's half-width along that path.  usage: python make_path.py <proj dir>"""
import sys, json, numpy as np, cv2
from PIL import Image, ImageFilter
from scipy.ndimage import gaussian_filter1d, label, median_filter
STEP = 4
OUT = sys.argv[1]; SRC = "C:/Users/Gorim/Documents/STS2Modding/Seductress/inset_concepts/picks/mouth_C5_v5.png"
im = Image.open(SRC).convert("RGB"); im.save(f"{OUT}/mouth.png")
a = np.array(im).astype(int); r, g, b = a[..., 0], a[..., 1], a[..., 2]
m = ((r > 216) & (g < 180) & (b > 0x70)).astype(np.uint8)
m = cv2.morphologyEx(m, cv2.MORPH_CLOSE, np.ones((7, 7), np.uint8))
lab, n = label(m); m = (lab == lab[500, 400]).astype(np.uint8)
Image.fromarray(cv2.erode(m, np.ones((15, 15), np.uint8)) * 255).filter(ImageFilter.GaussianBlur(1.5)).save(f"{OUT}/mouth_mask.png")
def resample(P, step):
    d = np.r_[0, np.cumsum(np.hypot(*np.diff(P, axis=0).T))]; s = np.arange(0, d[-1], step)
    return np.c_[np.interp(s, d, P[:, 0]), np.interp(s, d, P[:, 1])]
def normals(P):
    t = np.gradient(P, axis=0); t /= np.hypot(*t.T)[:, None]; return np.c_[-t[:, 1], t[:, 0]]
def extent(p, nrm, lim=140):
    out = []
    for sg in (1, -1):
        k = 0
        while k < lim:
            x, y = (p + nrm * sg * (k + 1)).round().astype(int)
            if not (0 <= x < 1024 and 0 <= y < 1024) or not m[y, x]: break
            k += 1
        out.append(k)
    return out
# the tube: a hand-estimated centre line, then re-centred on the measured walls
T = resample(np.array([(640, 318), (600, 330), (440, 425), (320, 630), (210, 885), (195, 930)], float), 8)
for it in range(6):
    N = normals(T)
    for i in range(len(T)):
        if not m[int(round(T[i, 1])), int(round(T[i, 0]))]: continue
        e = extent(T[i], N[i]); T[i] += N[i] * (e[0] - e[1]) / 2
    T = np.c_[gaussian_filter1d(T[:, 0], 6, mode="nearest"), gaussian_filter1d(T[:, 1], 6, mode="nearest")]; T = resample(T, 8)
# lead-in: from outside the picture, through the open mouth, to the tube's first point; blended smoothly into the tube
lead = np.array([(1180, 20), (1010, 95), (855, 180), (740, 255)], float)
tl = T[-1] - T[-6]; tl /= np.hypot(*tl); T = np.vstack([T, T[-1] + tl * 170])     # run on to the bottom of the picture
P = np.vstack([lead, T[1:]]); P = resample(P, 4)
P = np.c_[gaussian_filter1d(P[:, 0], 10, mode="nearest"), gaussian_filter1d(P[:, 1], 10, mode="nearest")]; P = resample(P, STEP)
N = normals(P); L = []; R = []; tube = []
WALL = 14      # px: thickness of the drawn wall; the inside edge is the mask's outer edge minus this
for i in range(len(P)):
    x, y = P[i].round().astype(int)
    ok = 0 <= x < 1024 and 0 <= y < 1024 and m[y, x]
    e = extent(P[i], N[i], 125) if ok else (125, 125); L.append(e[0]); R.append(e[1]); tube.append(ok)
tube = np.array(tube); first = int(np.argmax(tube)); last = len(tube) - 1 - int(np.argmax(tube[::-1]))
def clean(v):      # per side: drop spikes (the mask leaks along the jaw line), smooth, take off the wall
    v = np.array(v, float); seg = v[first:last + 1]
    seg = median_filter(seg, 41, mode="nearest"); seg = gaussian_filter1d(seg, 10, mode="nearest") - WALL
    out = np.full(len(v), 127.0); out[first:last + 1] = seg; out[last:] = seg[-1]; return out
L = clean(L); R = clean(R)
flag = np.zeros(len(P)); flag[first:] = 1; flag = gaussian_filter1d(flag, 6)     # 1 where there is a throat to stretch / hug
L = 127 - (127 - L) * flag; R = 127 - (127 - R) * flag
p = np.zeros((len(P), 4, 4), np.uint8); p[..., 0] = np.clip(L * 2, 0, 255).round()[:, None]; p[..., 1] = np.clip(R * 2, 0, 255).round()[:, None]
p[..., 2] = (flag * 255).round()[:, None]; p[..., 3] = 255
Image.fromarray(p, "RGBA").save(f"{OUT}/mouth_prof.png")
json.dump({"pts": P.round(1).tolist(), "step": STEP, "tube_first": first, "inner_left": L.round(1).tolist(), "inner_right": R.round(1).tolist()}, open(f"{OUT}/mouth_path.json", "w"))
print(len(P), "points, tube starts at s =", first * STEP, "; total length", len(P) * STEP)
print("inner half-width, +normal side:", L[first::12].round(0)); print("inner half-width, other side:  ", R[first::12].round(0))
W = (L + R) / 2
chk = a.astype(np.uint8).copy(); chk[m > 0] = chk[m > 0] // 2 + np.array([0, 110, 0], np.uint8)
for i in range(len(P) - 1): cv2.line(chk, tuple(P[i].round().astype(int)), tuple(P[i + 1].round().astype(int)), (0, 0, 255), 2)
for i in range(0, len(P), 8):
    for sg, v in ((1, L), (-1, R)): cv2.circle(chk, tuple((P[i] + N[i] * sg * v[i]).round().astype(int)), 3, (255, 255, 0), -1)
Image.fromarray(chk).resize((640, 640)).save("mouth_check.png")
