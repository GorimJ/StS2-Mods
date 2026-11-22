"""Cut the strip art (NovelAI results) into rig parts. [Claude, 2026-11-22]
usage: python cut_strip.py <battle work dir> <jobs root (GorimCuts/Claude_2026-11-22_strip)> <out work root>
Writes <out>/parts/ = the battle parts with the filler under sleeves / skirt panel / hip ornaments removed, plus:
  ARM_L_bare                             arm under the left sleeve (always on; the sleeve covers it)
  ARM_R_bare                             arm under the right sleeve (off by default: it is wider than the sleeve's band)
  SLEEVE_L_behind_hair, _skirt           hair and rear skirt that the left sleeve covered (always on; _skirt goes with the skirt)
  LEG_L_shin_foot_bare, LEG_R_shin_foot_bare   barefoot versions of the two shin+foot parts (off by default)
  PELVIS_bare, LEG_L_thigh_bare, LEG_R_thigh_bare   hips, crotch and thighs without the skirt (off by default)"""
import sys, json, numpy as np, cv2
from pathlib import Path
from PIL import Image
from scipy import ndimage
WORK, JOBS, OUT = Path(sys.argv[1]), Path(sys.argv[2]), Path(sys.argv[3])
D = json.load(open(WORK / "draw.json")); lab = np.load(WORK / "labels.npy"); H, W = lab.shape
yy, xx = np.mgrid[0:H, 0:W]
BG = np.array([52, 53, 62], np.float32)
ix = lambda *n: [D.index(k) for k in n]
def dil(m, r): return cv2.dilate(m.astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (2 * r + 1, 2 * r + 1))).astype(bool)
def ero(m, r): return cv2.erode(m.astype(np.uint8), cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (2 * r + 1, 2 * r + 1))).astype(bool)
def target(ctx, res, win):
    t = np.array(Image.open(ctx).convert("RGB")); x0, y0, w, h = win
    t[y0:y0 + h, x0:x0 + w] = np.array(Image.open(res).convert("RGB")); return t.astype(np.float32)
def classes(t):
    """0 background, 1 skin, 2 purple cloth, 3 hair/other; outlines go to the nearest class"""
    u = t.astype(np.uint8); hsv = cv2.cvtColor(u, cv2.COLOR_RGB2HSV_FULL).astype(np.float32)
    Hh, S, V = hsv[..., 0] * 360 / 255, hsv[..., 1], hsv[..., 2]; r, g, b = t[..., 0], t[..., 1], t[..., 2]
    fg = np.sqrt(((t - BG) ** 2).sum(2)) > 22
    skin = fg & (Hh >= 6) & (Hh < 28) & (V > 135) & (S > 60) & (r - b > 50)
    purple = fg & (Hh >= 255) & (Hh <= 345) & (b > g)
    brown = fg & ~skin & ~purple & ((((Hh < 60) | (Hh > 345)) & (V > 45) & (V < 135)) | ((Hh >= 28) & (Hh < 55) & (S > 70)))   # hair, leather, gold
    c = np.zeros(t.shape[:2], np.uint8)
    dist = np.stack([ndimage.distance_transform_edt(~s) for s in (skin, purple, brown)])
    c[fg] = dist[:, fg].argmin(0) + 1
    return c, fg
def piece(t, sel):
    """full-canvas RGBA of the pixels in sel, with a 1 px soft edge"""
    a = np.clip(cv2.GaussianBlur(sel.astype(np.float32), (0, 0), 0.8) * 1.3, 0, 1) * dil(sel, 1)
    # colour under the soft edge comes from the nearest selected pixel, so no backdrop colour bleeds in
    _, (sy, sx) = ndimage.distance_transform_edt(~sel, return_indices=True)
    return np.dstack([t[sy, sx], a * 255]).clip(0, 255).astype(np.uint8)
def big(m, least):
    lbl, n = ndimage.label(m); sizes = ndimage.sum(m, lbl, range(1, n + 1))
    return np.isin(lbl, [i + 1 for i, s in enumerate(sizes) if s >= least])
def biggest(m):
    lbl, n = ndimage.label(m); sizes = ndimage.sum(m, lbl, range(1, n + 1))
    return lbl == (1 + int(np.argmax(sizes)))
def save(arr, name): Image.fromarray(arr, "RGBA").save(OUT / "parts" / f"{name}.png")

(OUT / "parts").mkdir(parents=True, exist_ok=True)
SLV, PANEL = ix("SLEEVE_L", "SLEEVE_R"), ix("CLOTH_front_skirt_panel", "HIP_ornaments_left", "HIP_ornaments_right")
under_slv, under_pan = np.isin(lab, SLV), np.isin(lab, PANEL)
front = np.isin(lab, [i for i in range(len(D)) if i > D.index("SLEEVE_R")])

# ---- targets: the rest pose with the chosen NovelAI result pasted in ----
A = JOBS / "nai_jobs/STRIP_arms"
tL = target(A / "context_full.png", A / "result_s1.png", (272, 400, 704, 512))
F = JOBS / "nai_jobs/STRIP_feet"; tF = target(F / "context_full.png", F / "result_s1.png", (224, 1024, 768, 320))
P = JOBS / "nai_jobs/STRIP_hips"; tP = target(P / "context_full.png", P / "result_s1.png", (400, 560, 448, 448))

rest = Image.new("RGBA", (W, H), tuple(int(v) for v in BG) + (255,))
for n in D: rest.alpha_composite(Image.open(WORK / "parts" / f"{n}.png"))
restf = np.array(rest.convert("RGB")).astype(np.float32); c0, _ = classes(restf)

# ---- left arm (picture left): skin is the arm; the rest painted there is hair and rear skirt ----
regL = dil(lab == D.index("SLEEVE_L"), 8) & ~front & (xx < 620)
cL, fgL = classes(tL)
coverL = (lab == D.index("SLEEVE_L")) | front            # at rest the sleeve (or something in front) hides all of this
regL &= coverL
armL = big(ndimage.binary_opening(regL & (cL == 1)), 400)
save(piece(tL, armL), "ARM_L_bare")
save(piece(tL, regL & fgL & ~armL & (cL != 2)), "SLEEVE_L_behind_hair")
save(piece(tL, regL & fgL & ~armL & (cL == 2)), "SLEEVE_L_behind_skirt")
# ---- right arm (picture right): second round, a painted guide arm repainted by NovelAI (make_armR_jobs.py) ----
# [Gorim 2026-11-22: "Left arm is fucked and needs fixing"] the first one sat apart from the shoulder with a flat top.
# Used: nai_jobs6 keep70 seed 2 (best of 8), then nai_jobs7 seed 1, a touch-up of the elbow: the visible forearm's
# end carried the shadow the sleeve cast on it, so the first ~35 px of the forearm are repainted too.
tR = target(A / "context_full.png", JOBS / "nai_jobs7/ARM_R_elbow/result_s1.png", (608, 352, 384, 384))
_keep = front | np.isin(lab, ix("TORSO_upper", "BREASTS_plus_wrap_variant", "HAIR_side_R", "HEAD_face_plus_front_hair", "BELLY_lower_torso_variant"))
_fa = lab == D.index("ARM_R_forearm_hand")
_seam = dil(_fa, 1) & dil(lab == D.index("SLEEVE_R"), 1) & (xx < 800)
_d = ndimage.distance_transform_edt(~_seam)
FADE_R = _fa & (_d < 50) & (xx < 806)                 # the forearm part fades out here; the arm piece below carries the repaint
regR = (dil(np.array(Image.open(JOBS / "nai_jobs6/guide.png"))[..., 3] > 0, 14) & ~(_keep & ~_fa)) | (_fa & (_d < 54) & (xx < 810)) | ((_d < 34) & (lab < 0))
cR, _ = classes(tR)
armR = ndimage.binary_fill_holes(biggest(regR & (cR == 1)))
# [Gorim 2026-11-22: "strip 1 has a visible gap"] between the wrap's edge and the arm the backdrop showed: the sleeve
# band used to cover that strip. The arm piece is drawn before the wrap and the hair, so it simply runs on under them
# there, in a slightly shaded skin tone (armpit side of the arm).
# Row by row: from the wrap's side up to the arm's own left edge, only on rows where the arm is, in the arm's colour
# darkened (it reads as her side in the arm's shadow; the arm keeps its outline in front of it).
_gap = np.zeros((H, W), bool); tR = tR.copy()
_solid = np.isin(lab, ix("TORSO_upper", "BELLY_lower_torso_variant"))
for _y in range(480, 560):        # down to the wrap's lower edge; below it the arm really is clear of the body
    _xs = np.nonzero(armR[_y, 690:760])[0]
    if not len(_xs): continue
    _ax = 690 + int(_xs[0])
    _row = np.zeros(W, bool); _row[676:_ax] = True; _row &= ~_solid[_y]
    _gap[_y] = _row; tR[_y, _row] = tR[_y, min(_ax + 7, W - 1)] * 0.8
save(piece(tR, armR | _gap), "ARM_R_bare")     # wider than the sleeve's band, so it is off until the sleeve is

# ---- feet: the shin+foot part with the foot repainted bare ----
cF, _ = classes(tF)
for leg, top, side in (("LEG_L_shin_foot", 1070, xx < 600), ("LEG_R_shin_foot", 1055, xx >= 600)):
    own = lab == D.index(leg)
    reg = dil(own & (yy > top), 16) & (yy > top - 20) & side
    foot = ndimage.binary_fill_holes(biggest(reg & (cF == 1)))
    p = np.array(Image.open(WORK / "parts" / f"{leg}.png")).astype(np.float32)
    p[reg, 3] = 0                                              # the shoe goes
    new = piece(tF, foot).astype(np.float32); a = new[..., 3:4] / 255
    p[..., :3] = new[..., :3] * a + p[..., :3] * (1 - a); p[..., 3] = np.maximum(p[..., 3], new[..., 3])
    save(p.clip(0, 255).astype(np.uint8), leg + "_bare")

# ---- hips: a crotch piece on the hip bone, and bare versions of both thighs. All off until the skirt is. ----
cP, _ = classes(tP)
regP = dil(under_pan, 10) & (yy > 600) & (yy < 990)
body = big(regP & (cP == 1), 600)
spk = regP & ndimage.binary_fill_holes(body) & ~body           # dark specks left from the ornaments, inside the skin
tP8 = cv2.inpaint(tP.astype(np.uint8), dil(spk, 2).astype(np.uint8) * 255, 4, cv2.INPAINT_TELEA).astype(np.float32)
body = ndimage.binary_fill_holes(body)
_v = tP8.max(2).astype(np.uint8)                                # ornament tips the model kept, drawn on the skin
_dark = ero(body, 6) & (_v < 0.8 * cv2.medianBlur(_v, 31)) & dil(np.isin(lab, ix("HIP_ornaments_left", "HIP_ornaments_right")), 14)
tP8 = cv2.inpaint(tP8.astype(np.uint8), dil(_dark, 3).astype(np.uint8) * 255, 5, cv2.INPAINT_TELEA).astype(np.float32)
pel = piece(tP8, body).astype(np.float32); pel[..., 3] *= np.clip((840 - yy) / 40.0, 0, 1)   # ends between y 800 and 840
save(pel.astype(np.uint8), "PELVIS_bare")

# ---- every other part: drop the filler under sleeves, panel and ornaments ----
# Pixels filed under the wrong part by the first cut (sleeve cuff on the forearms, a strip of the right sleeve among
# the right hip ornaments) move to the sleeve they belong to, so they hide with it.
MOVE = {}                                     # cloth part -> mask of pixels it gains
def move(src, sel, dst):
    sel = sel & (lab == D.index(src)); MOVE.setdefault(dst, np.zeros((H, W), bool)); MOVE[dst] |= sel; return sel
LOST = {}
LOST["ARM_L_forearm_hand"] = move("ARM_L_forearm_hand", big((c0 != 1) & (xx > 372), 12), "SLEEVE_L")
LOST["ARM_R_forearm_hand"] = move("ARM_R_forearm_hand", (big((c0 != 1) & (xx < 790), 12) | (xx < 773)) & (yy > 520), "SLEEVE_R")
LOST["HIP_ornaments_right"] = move("HIP_ornaments_right", (xx > 758) & (yy < 745), "SLEEVE_R")
MOVE["SLEEVE_R"] = MOVE.get("SLEEVE_R", np.zeros((H, W), bool)) | FADE_R      # the sleeve keeps a copy of the faded forearm end
# loose bits of the right rear skirt beside the hip are really the right sleeve's tail: they go with the sleeve
_sk = np.array(Image.open(WORK / "parts" / "SKIRT_rear_right.png"))[..., 3] > 0
_sk &= ~(under_slv & (lab != D.index("SKIRT_rear_right")))
TAIL_R = _sk & ~big(_sk, 1500) & (xx > 690) & (yy < 760)
MOVE["SLEEVE_R"] |= TAIL_R
SKIRTS = ("SKIRT_rear_left", "SKIRT_rear_right", "CLOTH_front_skirt_panel", "HIP_ornaments_left", "HIP_ornaments_right")
def scrub(p, gone):
    """remove `gone` from a part: paint over where it lies inside the part, cut away where it is on the edge"""
    inner = ndimage.binary_fill_holes((p[..., 3] > 128) & ~gone) & gone
    rgb = cv2.inpaint(np.ascontiguousarray(p[..., :3]), dil(inner, 2).astype(np.uint8) * 255, 4, cv2.INPAINT_TELEA)
    p[inner, :3] = rgb[inner]; p[gone & ~inner, 3] = 0
for n in D:
    p = np.array(Image.open(WORK / "parts" / f"{n}.png"))
    own = lab == D.index(n)
    if n not in ("SLEEVE_L", "SLEEVE_R"): p[under_slv & ~own, 3] = 0
    if n not in SKIRTS: p[under_pan & ~own, 3] = 0
    if n in LOST and LOST[n].any(): scrub(p, LOST[n])
    if n == "ARM_R_forearm_hand":                     # see FADE_R; the sleeve keeps a copy so the dressed pose is unchanged
        p[FADE_R, 3] = (p[FADE_R, 3] * np.clip(np.maximum((_d[FADE_R] - 38) / 12.0, (xx[FADE_R] - 796) / 10.0), 0, 1)).astype(np.uint8)
    if n in MOVE:
        got = MOVE[n]; p[got, :3] = restf[got].astype(np.uint8); p[got, 3] = 255
    if n in ("SKIRT_rear_left", "SKIRT_rear_right"):                      # loose shards of filler left by the trimming
        p[(p[..., 3] > 0) & ~big(p[..., 3] > 0, 1500) & (~own | TAIL_R), 3] = 0
    if n in ("LEG_L_thigh", "LEG_R_thigh"):
        # dressed: real body under the skirt panel and ornaments instead of filler, for when they swing aside
        hid = body & under_pan & ((xx < 645) if n == "LEG_L_thigh" else (xx >= 645))
        p[hid, :3] = tP8[hid].astype(np.uint8); p[hid, 3] = 255
    save(p, n)
    if n in ("LEG_L_thigh", "LEG_R_thigh"):
        # bare version: cloth and ornament bits that were filed under the thigh go, and the new art comes in
        scrub(p, big(own & np.isin(c0, (2, 3)), 12))
        mine = body & ((xx < 645) if n == "LEG_L_thigh" else (xx >= 645)) & (yy > 690)
        new = piece(tP8, mine); use = new[..., 3] > 0
        p[use, :3] = new[use, :3]; p[..., 3] = np.maximum(p[..., 3], new[..., 3])
        cb, _ = classes(p[..., :3].astype(np.float32))
        scrub(p, big((p[..., 3] > 128) & np.isin(cb, (2, 3)), 6))          # anything left on it that is not skin
        v = p[..., :3].max(2).astype(np.float32)
        dark = ero(p[..., 3] > 128, 6) & (v < 0.8 * cv2.medianBlur(p[..., :3].max(2), 31)) & dil(np.isin(lab, ix("HIP_ornaments_left", "HIP_ornaments_right")), 14)
        scrub(p, dil(dark, 2) & (p[..., 3] > 128))                        # ornament tips drawn on the thigh
        p[(p[..., 3] > 0) & ~biggest(p[..., 3] > 0), 3] = 0                # loose slivers
        save(p, n + "_bare")
# ---- draw order ----
draw = list(D)
def after(name, new): draw.insert(draw.index(name) + 1, new)
after("ARM_L_upper", "ARM_R_bare")
for n in ("SLEEVE_L_behind_skirt", "SLEEVE_L_behind_hair", "ARM_L_bare"): draw.insert(draw.index("SLEEVE_L"), n)
for n in ("LEG_L_shin_foot", "LEG_R_shin_foot", "LEG_L_thigh", "LEG_R_thigh"): after(n, n + "_bare")
after("BELLY_lower_torso_variant", "PELVIS_bare")
json.dump({"draw": draw, "off_by_default": ["ARM_R_bare", "LEG_L_shin_foot_bare", "LEG_R_shin_foot_bare", "LEG_L_thigh_bare",
           "LEG_R_thigh_bare", "PELVIS_bare"]}, open(OUT / "parts" / "prep_report.json", "w"), indent=1)
print(draw)
