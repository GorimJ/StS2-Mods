"""Prepare full-canvas part PNGs for the rig from Gorim's cuts + the master.  [Claude, 2026-11-17]

usage: python prep_parts.py <Seductress folder> <out dir>

What it does, in order:
 1. Loads the 24 aligned cuts.
 2. Splits each sleeve into *_back (the whole sleeve, drawn behind the forearm) and *_front (the sleeve with a
    hole where the master shows the forearm coming out of the opening, drawn in front of the forearm).
 3. Skin pixels of the master that no cut covers (the cuts are narrower than the master in places) are added to
    the nearest skin part.
 4. Whatever is still uncovered becomes the stand-ins HAIR_back / SKIRT_rear_left / SKIRT_rear_right
    (hidden extent from Backhairblob.png, filled with OpenCV inpaint) and FX_censer_smoke (soft matte).
 5. Where a part is the top layer at rest, its colour is taken from the master (so the rest pose equals the
    master: no tone mismatch, no seams). Where it is hidden, its own pixels are kept but tone-matched to the
    master with a smooth gain map.
"""
import sys, os, json
import numpy as np, cv2
from PIL import Image
from scipy import ndimage

SRC, OUT = sys.argv[1], sys.argv[2]
B = f"{SRC}/GorimCuts/Claude_2026-10-24"
os.makedirs(OUT, exist_ok=True)

CUTS = ["LEG_L_shin_foot", "LEG_R_shin_foot", "LEG_L_thigh", "LEG_R_thigh", "PELVIS_base",
        "BELLY_lower_torso_variant", "SKIRT_hip_wrap_left", "TORSO_upper", "ARM_R_upper", "ARM_L_upper",
        "HEAD_face_plus_front_hair", "HAIR_side_L", "HAIR_side_R", "BREASTS_plus_wrap_variant",
        "ARM_L_forearm_hand", "ARM_R_forearm_hand", "SLEEVE_L", "SLEEVE_R", "CLOTH_front_skirt_panel",
        "HIP_ornaments_left", "HIP_ornaments_right", "PROP_censer_and_chains", "HAT_main", "HAT_charm"]
SKIN = {"LEG_L_shin_foot", "LEG_R_shin_foot", "LEG_L_thigh", "LEG_R_thigh", "PELVIS_base",
        "BELLY_lower_torso_variant", "TORSO_upper", "ARM_R_upper", "ARM_L_upper", "ARM_L_forearm_hand",
        "ARM_R_forearm_hand"}
# final draw order, back to front
DRAW = ["HAIR_back", "SKIRT_rear_left", "SKIRT_rear_right", "LEG_L_shin_foot", "LEG_R_shin_foot", "LEG_L_thigh",
        "LEG_R_thigh", "PELVIS_base", "BELLY_lower_torso_variant", "SKIRT_hip_wrap_left", "TORSO_upper",
        "ARM_R_upper", "ARM_L_upper", "HEAD_face_plus_front_hair", "HAIR_side_L", "HAIR_side_R",
        "BREASTS_plus_wrap_variant", "SLEEVE_L_back", "SLEEVE_R_back", "ARM_L_forearm_hand", "ARM_R_forearm_hand",
        "SLEEVE_L_front", "SLEEVE_R_front", "CLOTH_front_skirt_panel", "HIP_ornaments_left",
        "HIP_ornaments_right", "PROP_censer_and_chains", "FX_censer_smoke", "HAT_main", "HAT_charm"]


def load(n):
    for d in ("your_cuts_aligned", "new_layers"):
        p = f"{B}/{d}/{n}.png"
        if os.path.exists(p):
            return np.array(Image.open(p).convert("RGBA")).astype(np.float32)
    raise FileNotFoundError(n)


master = np.array(Image.open(f"{SRC}/DefaultNeutral.png").convert("RGB")).astype(np.float32)
H, W, _ = master.shape
bg = np.median(master[:40, :40].reshape(-1, 3), axis=0)
dist_bg = np.sqrt(((master - bg) ** 2).sum(2))
fg = dist_bg > 26
skinlike = (master[..., 0] > 140) & (master[..., 0] > master[..., 2] + 35) & (master[..., 1] > master[..., 2])

P = {n: load(n) for n in CUTS}

# ---- 2. sleeves -------------------------------------------------------------------------------------------
report = {}
for side in "LR":
    sl = P.pop(f"SLEEVE_{side}")
    arm = P[f"ARM_{side}_forearm_hand"]
    both = (sl[..., 3] > 128) & (arm[..., 3] > 128)
    emerg = both & skinlike                       # master shows the arm here, although the sleeve cut covers it
    emerg = cv2.morphologyEx(emerg.astype(np.uint8), cv2.MORPH_OPEN, np.ones((3, 3), np.uint8))
    n, cc, st, _ = cv2.connectedComponentsWithStats(emerg, 8)
    keep = np.zeros_like(emerg, bool)
    for i in range(1, n):
        if st[i, 4] >= 150:
            keep |= cc == i
    front = sl.copy()
    hole = cv2.GaussianBlur(keep.astype(np.float32), (0, 0), 0.7)
    front[..., 3] *= (1 - np.clip(hole * 1.5, 0, 1))
    P[f"SLEEVE_{side}_back"] = sl
    P[f"SLEEVE_{side}_front"] = front
    report[f"sleeve_{side}_arm_through_px"] = int(keep.sum())

order_real = [n for n in DRAW if n in P]


def top_map(names):
    top = np.full((H, W), -1, np.int32)
    for i, n in enumerate(names):
        top[P[n][..., 3] > 128] = i
    return top


# ---- 3. give uncovered skin to the nearest skin part ---------------------------------------------------------
top = top_map(order_real)
unc = fg & (top < 0)
d, (iy, ix) = ndimage.distance_transform_edt(top < 0, return_indices=True)
near = top[iy, ix]
near_rgb = master[iy, ix]
ok = unc & skinlike & (d <= 16) & (np.abs(master - near_rgb).sum(2) < 60)
added = {}
for i, n in enumerate(order_real):
    if n not in SKIN:
        continue
    m = ok & (near == i)
    if m.any():
        P[n][m, :3] = master[m]
        P[n][m, 3] = 255
        added[n] = int(m.sum())
report["skin_px_added"] = added

# ---- 3b. the dark outline under the breast wrap belongs to the wrap, not to the torso/belly under it ------------
BR, TO, BE = "BREASTS_plus_wrap_variant", "TORSO_upper", "BELLY_lower_torso_variant"
top = top_map(order_real)
brm = (P[BR][..., 3] > 128)
allowed = ~brm & fg & np.isin(top, [order_real.index(TO), order_real.index(BE)])
yy = np.arange(H)[:, None] * np.ones((1, W))
allowed &= yy > 470                              # lower edge only
# a 7 px skirt of skin + shadow line under the wrap travels with the wrap; its last rows fade out
m = np.zeros((H, W), bool)
for k, a_val in ((4, 255), (5, 215), (6, 175), (7, 140)):
    ring = cv2.dilate(brm.astype(np.uint8), np.ones((2 * k + 1, 1), np.uint8)).astype(bool) & allowed & ~m
    P[BR][ring, :3] = master[ring]; P[BR][ring, 3] = a_val
    m |= ring
report["wrap_outline_px_moved"] = int(m.sum())

# ---- 4. stand-ins -----------------------------------------------------------------------------------------
top = top_map(order_real)
cover = top >= 0
ext = np.array(Image.open(f"{SRC}/Backhairblob.png").convert("RGB")).astype(np.int32)
k5 = np.ones((5, 5), np.uint8)
m8 = master.astype(np.uint8)
for name, col in (("HAIR_back", (0x80, 0x80, 0)), ("SKIRT_rear_left", (0x80, 0, 0x80)),
                  ("SKIRT_rear_right", (0x80, 0, 0xFF))):
    ex = np.abs(ext - np.array(col)).sum(2) < 45
    ex = cv2.morphologyEx(ex.astype(np.uint8), cv2.MORPH_CLOSE, k5).astype(bool)
    vis = ex & fg & ~cv2.dilate(cover.astype(np.uint8), np.ones((3, 3), np.uint8)).astype(bool) & ~skinlike
    n, cc, st, _ = cv2.connectedComponentsWithStats(vis.astype(np.uint8), 8)
    for i in range(1, n):
        if st[i, 4] < 60:
            vis[cc == i] = False
    shape = cv2.morphologyEx((vis | (ex & cover)).astype(np.uint8), cv2.MORPH_CLOSE, k5).astype(bool) & ex
    need = shape & ~vis
    rgb = cv2.inpaint(np.ascontiguousarray(m8[..., ::-1]), need.astype(np.uint8) * 255, 7, cv2.INPAINT_TELEA)[..., ::-1]
    a = np.clip(cv2.GaussianBlur(shape.astype(np.float32), (0, 0), 0.8), 0, 1)
    P[name] = np.dstack([rgb.astype(np.float32), a * 255])
    report[name] = {"visible": int(vis.sum()), "filled": int(need.sum())}

topb = np.array(Image.open(f"{SRC}/Toplevelblobs.png").convert("RGB")).astype(np.int32)
sm = np.abs(topb - np.array((0x80, 0x80, 0xFF))).sum(2) < 45
sm = cv2.dilate(sm.astype(np.uint8), np.ones((15, 15), np.uint8)).astype(bool) & ~cover
alpha = np.clip((dist_bg - 5) / 50.0, 0, 1) * sm
alpha = cv2.GaussianBlur(alpha.astype(np.float32), (0, 0), 1.0)
a3 = np.clip(alpha, 0.06, 1)[..., None]
smoke_rgb = np.clip((master - (1 - a3) * bg) / a3, 0, 255)
P["FX_censer_smoke"] = np.dstack([smoke_rgb, alpha * 255])

# ---- 4b. close hairline cracks between parts inside the figure ---------------------------------------------------
interior = cv2.erode(fg.astype(np.uint8), np.ones((7, 7), np.uint8)).astype(bool)
solid_names = [n for n in DRAW if n != "FX_censer_smoke"]
def top_solid():
    t = np.full((H, W), -1, np.int32)
    for i, n in enumerate(DRAW):
        if n != "FX_censer_smoke":
            t[P[n][..., 3] > 128] = i
    return t
top = top_solid()
d, (iy, ix) = ndimage.distance_transform_edt(top < 0, return_indices=True)
crack = interior & (top < 0) & (d <= 6)
near = top[iy, ix]
for i, n in enumerate(DRAW):
    m = crack & (near == i)
    if m.any():
        P[n][m, :3] = master[m]; P[n][m, 3] = 255
report["crack_px_closed"] = int(crack.sum())

# ---- 5. master colour where visible, tone-matched own colour where hidden --------------------------------------
top = top_solid()
for i, n in enumerate(DRAW):
    if n == "FX_censer_smoke":
        continue
    p = P[n]
    seen = (top == i) & fg
    if n in ("HAIR_back", "SKIRT_rear_left", "SKIRT_rear_right"):
        p[seen & interior, 3] = 255
        p[seen, :3] = master[seen]
        continue
    vis = seen
    if vis.sum() < 200:
        continue
    # gain map: blurred master / blurred part over the visible area (normalised convolution)
    v = vis.astype(np.float32)
    sig = 18
    wv = cv2.GaussianBlur(v, (0, 0), sig)
    gain = np.ones((H, W, 3), np.float32)
    glob = master[vis].mean(0) / np.maximum(p[vis][:, :3].mean(0), 1)
    for c in range(3):
        num = cv2.GaussianBlur(master[..., c] * v, (0, 0), sig)
        den = cv2.GaussianBlur(p[..., c] * v, (0, 0), sig)
        local = num / np.maximum(den, 1e-3)
        conf = np.clip(wv / 0.15, 0, 1)
        gain[..., c] = np.clip(conf * local + (1 - conf) * glob[c], 0.6, 1.6)
    hidden = (p[..., 3] > 0) & ~vis
    p[hidden, :3] = np.clip(p[hidden, :3] * gain[hidden], 0, 255)
    p[vis, :3] = master[vis]
    p[vis & interior, 3] = 255
    report.setdefault("tone_gain", {})[n] = [round(float(x), 3) for x in glob]

# ---- 5b. underbust: what the breathing uncovers must be plain skin -------------------------------------------------
def own_skin(p):
    return (p[..., 3] > 128) & (p[..., 0] > 140) & (p[..., 0] > p[..., 2] + 35) & (p[..., 1] > p[..., 2])
top = top_solid()
band = (yy > 470) & (yy < 570)
for n in (TO, BE):
    p = P[n]; i = DRAW.index(n)
    src = own_skin(p)
    _, (sy, sx) = ndimage.distance_transform_edt(~src, return_indices=True)
    fix = (p[..., 3] > 0) & ~src & band & (top != i)          # hidden non-skin (baked outlines, shadows)
    if n == BE:                                                # grow the belly 20 px up, only under torso/breasts
        grow = cv2.dilate((p[..., 3] > 128).astype(np.uint8), np.ones((41, 1), np.uint8)).astype(bool)
        grow &= (yy < 560) & ~(p[..., 3] > 128) & interior & cv2.dilate(((P[TO][..., 3] > 128) | (P[BR][..., 3] > 128)).astype(np.uint8), np.ones((5, 5), np.uint8)).astype(bool) & (top > i)
        p[grow, 3] = 255
        fix |= grow
        report["belly_grown_px"] = int(grow.sum())
    p[fix, :3] = p[sy[fix], sx[fix], :3]
    sm_rgb = cv2.GaussianBlur(p[..., :3], (0, 0), 2.0)       # soften the copied streaks
    p[fix, :3] = sm_rgb[fix]
    # remaining faint traces of the old outline: hidden pixels darker than their smooth surroundings -> smooth skin
    w = (p[..., 3] > 128).astype(np.float32)
    den = np.maximum(cv2.GaussianBlur(w, (0, 0), 6), 1e-3)
    blur = np.dstack([cv2.GaussianBlur(p[..., c] * w, (0, 0), 6) / den for c in range(3)])
    hid = (p[..., 3] > 128) & band & (top != i)
    dark = hid & (p[..., :3].sum(2) < blur.sum(2) - 9)
    dark = cv2.dilate(dark.astype(np.uint8), np.ones((3, 3), np.uint8)).astype(bool) & hid
    p[dark, :3] = blur[dark]
    if n == BE:                                   # the grown strip: smooth skin, fully opaque up to the old edge
        p[grow, :3] = blur[grow]
        edge = cv2.dilate(grow.astype(np.uint8), np.ones((7, 7), np.uint8)).astype(bool) & (p[..., 3] > 0)
        p[edge, 3] = 255

# inside the figure, drop faint edge pixels (alpha <= 128): they only draw dark hairlines over the part below
for n in DRAW:
    if n != "FX_censer_smoke":
        a = P[n][..., 3]
        a[(a <= 128) & interior] = 0

for n in DRAW:
    Image.fromarray(np.clip(P[n], 0, 255).astype(np.uint8), "RGBA").save(f"{OUT}/{n}.png")
json.dump({"draw": DRAW, "report": report}, open(f"{OUT}/prep_report.json", "w"), indent=1)
print(json.dumps(report, indent=1))
