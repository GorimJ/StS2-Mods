"""Cut the lactation pictures into milk layers for the battle rig. [Claude, 2026-11-18, on Gorim's request]
usage: python cut_milk.py <out dir> <nai_jobs_lact3 dir>
A milk layer is only what NovelAI changed (result minus the picture it was given), so it sits over the breast piece already
on the rig and can be switched on and off. Per state b (base, nude__*, wrap__stage*):
  MILK_<b>.png          milk on the breasts (for the two split sizes: MILK_<b>__near.png / __far.png, one per breast)
  MILK_<b>__drip.png    milk that ran past the breasts onto the stomach (only makes sense with no belly showing)
Nude: alpha follows how much whiter a pixel got, so repainted skin (whose tone differs from the rig's) is left out.
Clothed: alpha follows any change, because wet cloth is darker, not whiter."""
import sys, json, numpy as np, cv2
from pathlib import Path
from PIL import Image
OUT = Path(sys.argv[1]); OUT.mkdir(parents=True, exist_ok=True); J = Path(sys.argv[2])
PICK = json.load(open("milk_pick.json"))
CW, CH = 1086, 1448
# [Gorim 2026-11-18: flat - "the left milk is right of the nipple and needs moving a little left"; large - "needs to get
#  closer to the nipple"]  state -> (shift x of the near stream, window y its top should reach or None) [numbers: Claude]
FIX = {"nude__flat": (-14, None), "nude__bare": (-4, 138)}
def A(p): return cv2.imread(str(p), -1)[..., 3] > 128
for st, pick in PICK.items():
    if st.startswith("_"): continue
    d = J / f"LACT3_{st}"; job = json.load(open(d / "job.json")); x0, y0, w, h = job["window"]
    src = np.array(Image.open(d / "image.png").convert("RGB")).astype(np.float32)
    res = np.array(Image.open(d / pick).convert("RGB").resize((w, h), Image.LANCZOS)).astype(np.float32)
    mask = np.array(Image.open(d / "mask.png").convert("L")) > 0
    if st.startswith("nude"):
        gain = res.min(2) - src.min(2)                                   # whiter = every channel up
        # [Gorim 2026-11-18: "The bottom of the nude breasts is problematic"] NovelAI redrew the breast underside a pixel or
        # two off, and "got whiter" alone kept that as a pale band over the old outline. Milk must also BE white now.
        white = np.clip((res.min(2) - 165) / 45.0, 0, 1) * np.clip((75 - (res.max(2) - res.min(2))) / 30.0, 0, 1)
        g = np.clip((gain - 10) / 35.0, 0, 1)
        soft = np.clip((res.min(2) - 120) / 40.0, 0, 1) * np.clip((100 - (res.max(2) - res.min(2))) / 30.0, 0, 1)   # shaded milk edges
        nearwhite = cv2.dilate(((g * white) > 0.5).astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (7, 7)))
        a = np.maximum(g * white, g * soft * nearwhite)
    else:
        a = np.clip((np.abs(res - src).sum(2) - 18) / 50.0, 0, 1)
    a = a * cv2.erode(mask.astype(np.uint8), np.ones((5, 5), np.uint8))
    core = cv2.morphologyEx((a > 0.5).astype(np.uint8), cv2.MORPH_OPEN, np.ones((2, 2), np.uint8))
    k, lab, stt, _ = cv2.connectedComponentsWithStats(core, 8)
    keep = np.isin(lab, [i for i in range(1, k) if stt[i, 4] >= 12])      # drop specks
    a = a * (cv2.dilate(keep.astype(np.uint8), np.ones((7, 7), np.uint8)) > 0)
    a = cv2.GaussianBlur(a.astype(np.float32), (0, 0), 0.6)
    win = np.dstack([res, a * 255]).astype(np.float32)
    if st in FIX:       # NovelAI redrew the near nipple somewhere else; bring that stream back to the rig's nipple
        dx, top_to = FIX[st]; nearcol = np.arange(w)[None, :] < 300
        nm = win.copy(); nm[..., 3] *= nearcol
        ys = np.where((nm[..., 3] > 128).any(1))[0]; yt, yb = ys.min(), ys.max()
        sy = (yb - top_to) / (yb - yt) if top_to is not None else 1.0          # stretch upwards, bottom end stays put
        M = np.float32([[1, 0, dx], [0, sy, yb - sy * yb]])
        nm[..., :3] *= nm[..., 3:4] / 255.0                                    # premultiply so the warp does not smear skin in
        nm = cv2.warpAffine(nm, M, (w, h), flags=cv2.INTER_LINEAR)
        nm[..., :3] = nm[..., :3] * 255.0 / np.clip(nm[..., 3:4], 1, 255)
        win[..., 3] *= ~nearcol; sel = nm[..., 3] > win[..., 3]; win[sel] = nm[sel]
    full = np.zeros((CH, CW, 4), np.float32); full[y0:y0 + h, x0:x0 + w] = win
    near = Path(f"parts_breasts/BREASTS_{st}__near.png")
    if st == "base":
        breast = A("parts/BREASTS_plus_wrap_variant.png")
    elif near.exists():
        breast = A(near) | A(f"parts_breasts/BREASTS_{st}__far.png")
    else:
        breast = A(f"parts_breasts/BREASTS_{st}.png")
    breast = cv2.dilate(breast.astype(np.uint8), np.ones((5, 5), np.uint8)) > 0
    def save(name, sel):
        o = full.copy(); o[..., 3] *= sel
        if (o[..., 3] > 40).sum() < 30: return 0
        Image.fromarray(o.astype(np.uint8), "RGBA").save(OUT / f"{name}.png"); return int((o[..., 3] > 40).sum())
    if near.exists():
        n = A(near); rep = (save(f"MILK_{st}__near", breast & n), save(f"MILK_{st}__far", breast & ~n))
    else:
        rep = (save(f"MILK_{st}", breast),)
    print(st, pick, "breast px", rep, "drip px", save(f"MILK_{st}__drip", ~breast))
