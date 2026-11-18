"""Keep the breast stomach pieces and the pair patches off the skirt and the hip ring. [Claude]
[Gorim 2026-11-18] the largest size "overlaps weirdly with the skirt without the belly", and with a belly a
half-deleted scrap with an outline was left there.
 - BREASTS_nude__*__under.png: no pixel may sit on the skirt, front panel or hip ornaments of the base art.
 - PAIR_*.png: within 10 px of the skirt/ring on her right hip, nude states lose everything that is not plain skin
   (and anything on the ring or rear skirt); clothed states lose only repainted pieces of the gold ring.
   Scraps no longer joined to the patch are dropped.
Originals are copied once to <folder>_pretrim and always re-read from there.   usage: python trim_hip.py
"""
import glob, os, shutil, numpy as np, cv2
from PIL import Image
from scipy import ndimage as ndi
lab = np.load("labels.npy")
cloth = np.isin(lab, [1, 2, 23, 24, 25])
yy, xx = np.mgrid[0:lab.shape[0], 0:lab.shape[1]]
hip = (xx < 610) & (yy > 575)
near = ndi.binary_dilation(cloth, iterations=10) & hip          # only right beside the skirt and ring
def soft(al, cut, r=3.5):
    k = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (9, 9))
    cut = cv2.morphologyEx(cut.astype(np.uint8), cv2.MORPH_CLOSE, k) > 0      # round off the blocky label shapes
    keep = (~cut).astype(np.float32)
    keep = cv2.morphologyEx(keep, cv2.MORPH_OPEN, k)
    keep = np.minimum(keep, cv2.GaussianBlur(cv2.erode(keep, np.ones((5, 5), np.uint8)), (0, 0), r))
    return (al * keep).astype(np.uint8)
for folder, pat, mode in (("parts_breasts", "*__under.png", "under"), ("parts_pairs2", "PAIR_*.png", "pair")):
    bak = folder + "_pretrim"; os.makedirs(bak, exist_ok=True)
    for p in sorted(glob.glob(f"{folder}/{pat}")):
        b = f"{bak}/{os.path.basename(p)}"
        if not os.path.exists(b): shutil.copy(p, b)
        im = np.array(Image.open(b).convert("RGBA")); al = im[..., 3]; r, g, bl = [im[..., i].astype(np.float32) for i in range(3)]
        nude = "nude__" in p
        if mode == "under":
            cut = cloth & hip if nude else np.zeros_like(cloth)     # a hanging wrap may cover the skirt; skin may not
        else:
            lum = 0.3 * r + 0.59 * g + 0.11 * bl
            skin = (lum > 125) & (bl / np.maximum(r, 1) > 0.42) & (r - bl > 35)
            gold = (r > 150) & (bl / np.maximum(r, 1) < 0.42) & (lum > 110)
            # nude states: anything that is not skin there is a scrap. Clothed states: only repainted ring pieces.
            # clothed states: the same, but only below the wrap's reach
            cut = near & ((~skin) | np.isin(lab, [1, 24])) & (nude | (yy > 612))
            # [Gorim 2026-11-18] "something clipping in" at the bottom right of the breasts on larger bellies: a strip
            # of repainted sleeve (purple cloth, gold trim, bits of arm) kept along the patch's right side. On that
            # side, whatever is neither over the belly stage nor over the breast piece and is not plain skin goes;
            # skin scraps it leaves stranded are dropped below.
            nm = os.path.basename(p)[5:-4]; bname, k = nm.split("__belly")
            bp = "parts/BREASTS_plus_wrap_variant.png" if bname == "base" else f"parts_breasts_pretrim/BREASTS_{bname}__under.png"
            body = (np.array(Image.open(f"parts_var/BELLY_stage{k}.png"))[..., 3] > 100)
            if bname == "base": body |= np.array(Image.open(bp))[..., 3] > 100
            else: body |= np.array(Image.open(f"parts_breasts/BREASTS_{bname}.png"))[..., 3] > 100
            body_raw = ndi.binary_dilation(body, iterations=2)
            body = ndi.binary_dilation(body, iterations=8)
            # keep a 4 px rim beside the body's own skin: that is the drawn outline
            # skin islands on that side that do not touch the body at all are bits of arm
            l3, n3 = ndi.label(ndi.binary_opening(skin & (al > 0), iterations=1))
            touch = np.unique(l3[body]); main = np.isin(l3, touch[touch > 0])
            stray = (l3 > 0) & ~main & (xx > 690)
            rim = ndi.binary_dilation(main, iterations=2)
            gold = (r > 150) & (bl / np.maximum(r, 1) < 0.42) & (lum > 110)
            right = (xx > 690) & ~body & (al > 0)
            cut = cut | (right & ~skin & ~rim) | (right & gold) | ndi.binary_dilation(stray, iterations=4) & right
            if nude:      # a wedge of sleeve caught between breast and belly: non-skin thicker than any drawn outline
                # (only outside the belly stage and breast piece themselves, so navels and outlines are safe)
                out = (xx > 690) & ~body_raw & (al > 0)
                thick = ndi.binary_opening(~skin & out, iterations=3)
                cut = cut | (ndi.binary_dilation(thick, iterations=3) & ~skin & out) | (out & gold)
            cut = ndi.binary_dilation(cut & (al > 0), iterations=1)
        new = soft(al, cut)
        if mode == "pair":                       # scraps cut loose from the patch
            l2, n = ndi.label(new > 60)
            if n > 1:
                sz = ndi.sum(new > 60, l2, range(1, n + 1)); big = ndi.binary_dilation(l2 == 1 + int(np.argmax(sz)), iterations=4)
                new = np.where(big, new, 0).astype(np.uint8)
        d = int((al > 60).sum() - (new > 60).sum())
        if d: print(os.path.basename(p), "removed", d, "px")
        im[..., 3] = new; Image.fromarray(im).save(p)
