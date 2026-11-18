"""Give the large nude size (nude__bare) its far nipple. [Claude, 2026-11-18]
[Gorim: "on the large one the [right] nipple seems entirely missing ... We should maybe add the nipple from the milk one"]
The picked NovelAI picture for this size never had a visible far nipple (only a small highlight on the outline). The
lactation picture (nai_jobs_lact3/LACT3_nude__bare/result_s1.png) painted one, so that patch is copied into the breast
piece, tone-shifted to match, without the white milk. Reads parts_breasts_prenipple/, writes parts_breasts/."""
import numpy as np, cv2
from PIL import Image
J = 'C:/Users/Gorim/Documents/STS2Modding/Seductress/GorimCuts/Claude_2026-11-18_battle/nai_jobs_lact3/LACT3_nude__bare'
x0, y0, w, h = 336, 352, 576, 384
CX, CY, RX, RY = 384, 161, 15, 13                      # window pixels: centre and radii of the patch
piece = np.array(Image.open('parts_breasts_prenipple/BREASTS_nude__bare.png').convert('RGBA')).astype(np.float32)
src = np.array(Image.open(J + '/image.png').convert('RGB')).astype(np.float32)
res = np.array(Image.open(J + '/result_s1.png').convert('RGB').resize((w, h), Image.LANCZOS)).astype(np.float32)
yy, xx = np.mgrid[0:h, 0:w]
d = np.sqrt(((xx - CX) / RX) ** 2 + ((yy - CY) / RY) ** 2)
m = np.clip((1 - d) / 0.35, 0, 1)                                           # soft edge
white = (res.min(2) > 170) & ((res.max(2) - res.min(2)) < 70)               # milk: leave it to the milk layer
white = cv2.dilate(white.astype(np.uint8), np.ones((3, 3), np.uint8)) > 0
bg = np.abs(res - np.float32([62, 64, 74])).sum(2) < 60                     # backdrop grey
dark = res.max(2) < 120                                                     # hair / sleeve behind the breast
m = m * ~white * ~bg * ~dark
win = piece[y0:y0 + h, x0:x0 + w]
ring = (d > 1.0) & (d < 1.6) & (win[..., 3] > 250)
shift = (win[..., :3][ring] - src[ring]).mean(0)                            # the piece was tone-matched; carry that over
patch = np.clip(res + shift, 0, 255)
a_old = win[..., 3:4] / 255.0; mm = m[..., None]
win[..., :3] = np.where(a_old > 0.02, win[..., :3] * (1 - mm) + patch * mm, patch)
win[..., 3] = np.maximum(win[..., 3], m * 255)                              # the nub sticks out past the old outline
piece[y0:y0 + h, x0:x0 + w] = win
Image.fromarray(piece.astype(np.uint8), 'RGBA').save('parts_breasts/BREASTS_nude__bare.png')
print('tone shift', shift.round(1), 'patch px', int((m > 0.5).sum()))
