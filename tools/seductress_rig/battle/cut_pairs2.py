"""Cut the version 2 pair results (lower breasts + upper belly repainted) into patches for the rig. [Claude]
usage: python cut_pairs2.py <jobs dir> <out dir> [all]
A patch keeps only the body inside the mask: skin (and, for cloth states, the wrap), plus its outline. Repainted
sleeve, hair and backdrop are dropped, so the rig's own moving sleeve and hair show beside it."""
import sys, json, numpy as np, cv2
from pathlib import Path
from PIL import Image
from scipy import ndimage
JOBS, OUT = Path(sys.argv[1]), Path(sys.argv[2]); OUT.mkdir(parents=True, exist_ok=True)
base = np.array(Image.open('C:/Users/Gorim/Documents/STS2Modding/Seductress/Battle1.png').convert('RGB')).astype(np.float32)
H, W = base.shape[:2]
lab = np.load("labels.npy"); DRAW = json.load(open("draw.json"))
def skinmask(a):
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    return (r > 140) & (r - b > 35) & (r > g) & (g > b)
ref = base[np.isin(lab, [DRAW.index("TORSO_upper"), DRAW.index("BELLY_lower_torso_variant")]) & skinmask(base)]
MU_R, SD_R = ref.mean(0), ref.std(0)
BG = np.median(base[20:120, 20:120].reshape(-1, 3), 0)
PICK = json.load(open("pair2_pick.json")) if Path("pair2_pick.json").exists() else {}
ALL = len(sys.argv) > 3 and sys.argv[3] == "all"
todo = []
for d in sorted(p for p in JOBS.iterdir() if p.name.startswith("PAIR_") and (p / "result_s1.png").exists()):
    fs = sorted(d.glob("result_s*.png"))
    if ALL: todo += [(d, f.name, f"{d.name}__{f.stem[-2:]}") for f in fs if len(fs) > 1]
    else: todo.append((d, PICK.get(d.name, "result_s1.png"), d.name))
done = 0
for d, rf, outname in todo:
    job = json.load(open(d / "job.json")); x0, y0, w, h = job["window"]
    b = d.name[5:].split("__belly")[0]
    res = np.array(Image.open(d / rf).convert('RGB').resize((w, h), Image.LANCZOS)).astype(np.float32)
    full = np.zeros((H, W, 3), np.float32); full[y0:y0 + h, x0:x0 + w] = res
    m = np.zeros((H, W), bool); m[y0:y0 + h, x0:x0 + w] = np.array(Image.open(d / "mask.png").convert('L')) > 128
    sk = skinmask(full) & m
    keep = sk.copy()
    if b == "base" or "wrap" in b:                                   # cloth states: the wrap itself, not the sleeves
        over = np.array(Image.open("parts/BREASTS_plus_wrap_variant.png" if b == "base" else f"parts_breasts/BREASTS_{b}.png"))[..., 3] > 128
        near = cv2.dilate(over.astype(np.uint8), np.ones((29, 29), np.uint8)).astype(bool)
        cloth = (full[..., 2] > full[..., 1] * 0.95) & (full[..., 0] < 150) & (np.abs(full - BG).sum(2) > 45)
        keep |= cloth & near & m
    keep = cv2.morphologyEx(keep.astype(np.uint8), cv2.MORPH_OPEN, np.ones((5, 5), np.uint8))
    k, cc, st, _ = cv2.connectedComponentsWithStats(keep, 8)
    keep = cc == (1 + np.argmax(st[1:, 4]))                           # the body, not stray skin-coloured specks
    keep = cv2.morphologyEx(keep.astype(np.uint8), cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (15, 15))).astype(bool)
    keep = ndimage.binary_fill_holes(keep)
    dark = (full.max(2) < 135) & (np.abs(full - BG).sum(2) > 45)      # its outline: dark pixels hugging it
    keep |= cv2.dilate(keep.astype(np.uint8), np.ones((9, 9), np.uint8)).astype(bool) & dark & m
    if sk.sum() > 500:
        mu, sd = full[sk].mean(0), full[sk].std(0)
        matched = (full - mu) * np.clip(SD_R / np.maximum(sd, 1), 0.75, 1.3) + MU_R
        # full correction deep inside the patch, none at its edge: there NovelAI already matched the surroundings,
        # and correcting it would leave a tone band where the patch meets the belly
        inner = np.clip(ndimage.distance_transform_edt(m) / 45.0, 0, 1)
        soft = (cv2.GaussianBlur(sk.astype(np.float32), (0, 0), 2.0) * inner)[..., None]
        full = np.clip(full * (1 - soft) + matched * soft, 0, 255)
    # soft edge at the top, bottom and her left, where the patch crosses skin. Not on its slanted right side: the
    # belly's outline runs close to it there, and fading it made that outline fuzzy
    m_ext = m.copy()
    for dx in range(1, 40): m_ext[:, dx:] |= m[:, :-dx]
    t = np.clip(ndimage.distance_transform_edt(m_ext) / 20.0, 0, 1)
    alpha = t * t * (3 - 2 * t) * np.clip(cv2.GaussianBlur(keep.astype(np.float32), (0, 0), 0.8) * 1.2, 0, 1)
    Image.fromarray(np.dstack([full, alpha * 255]).astype(np.uint8), "RGBA").save(OUT / f"{outname}.png"); done += 1
print(done, "patches")
