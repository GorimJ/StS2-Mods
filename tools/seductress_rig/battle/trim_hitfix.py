"""[Claude 2026-11-18] Art trims for the hit animation (Gorim: fabric clipping at the belly's right at the hit peak).
Reads the untouched pieces from parts_prehitfix/, writes parts/.
- BELLY_lower_torso_variant: drop the strip that sat hidden under the right sleeve.
- LEG_R_thigh: drop everything hidden under the right sleeve at rest, then keep only the main piece.
Both showed as tan streaks through the sleeve when it swings."""
import cv2, numpy as np
b = cv2.imread('parts_prehitfix/BELLY_lower_torso_variant.png', -1); b[586:, 690:, 3] = 0
cv2.imwrite('parts/BELLY_lower_torso_variant.png', b)
t = cv2.imread('parts_prehitfix/LEG_R_thigh.png', -1); s = cv2.imread('parts/SLEEVE_R.png', -1)
t[s[..., 3] > 20, 3] = 0
k, lab, st, _ = cv2.connectedComponentsWithStats((t[..., 3] > 20).astype(np.uint8), connectivity=8)
keep = 1 + int(np.argmax(st[1:, 4])); t[lab != keep, 3] = 0
print("thigh comps", k - 1, "kept", st[keep])
cv2.imwrite('parts/LEG_R_thigh.png', t)
