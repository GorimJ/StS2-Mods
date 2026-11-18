"""Cut Battle1.png into full-canvas part PNGs from labels.npy; hidden edges get a simple fill. [Claude]"""
import numpy as np, cv2, json, os, sys
from PIL import Image
from scipy import ndimage
from classify import load
OUT=sys.argv[1]; os.makedirs(OUT,exist_ok=True)
rgb,cls,fg,dist,bg=load(); lab=np.load('labels.npy'); DRAW=json.load(open('draw.json'))
H,W=lab.shape; master=rgb.astype(np.float32)
GROW={"HAIR_back":40,"SKIRT_rear_left":45,"SKIRT_rear_right":45,"LEG_L_thigh":30,"LEG_R_thigh":30,"LEG_L_shin_foot":20,
      "LEG_R_shin_foot":20,"BELLY_lower_torso_variant":55,"TORSO_upper":60,"ARM_L_upper":30,"SLEEVE_L":20,"SLEEVE_R":20,
      "HEAD_face_plus_front_hair":10,"ARM_R_hand":6,"FINGER_R_index":3,"FINGER_R_middle":3,"FINGER_R_ring":3,"FINGER_R_pinky":3,"BREASTS_plus_wrap_variant":16,"CLOTH_front_skirt_panel":12}
solid=cv2.erode(fg.astype(np.uint8),np.ones((3,3),np.uint8)).astype(bool)
edge_alpha=np.clip(cv2.GaussianBlur(solid.astype(np.float32),(0,0),0.8)*1.15,0,1)     # soft outer silhouette
interior=cv2.erode(fg.astype(np.uint8),np.ones((7,7),np.uint8)).astype(bool)
# feathered overlap: the part also carries a fading copy of the neighbour it hinges on, so the joint never opens.
# name -> (neighbour, reach px, extra mask)
yy,xx=np.mgrid[0:H,0:W]
SK=[DRAW.index("TORSO_upper"),DRAW.index("BELLY_lower_torso_variant")]
FEATHER={"BREASTS_plus_wrap_variant":[(SK,26,(yy<508)&(xx>546)&(xx<705)),(SK,9,(yy>540)&(xx>546)&(xx<705))],
         "ARM_R_hand":[([DRAW.index("ARM_R_forearm_hand")],12,None)],
         "FINGER_R_index":[([DRAW.index("ARM_R_hand")],7,None)],"FINGER_R_middle":[([DRAW.index("ARM_R_hand")],7,None)],
         "FINGER_R_ring":[([DRAW.index("ARM_R_hand")],7,None)],"FINGER_R_pinky":[([DRAW.index("ARM_R_hand")],7,None)]}
SMOKE=DRAW.index("FX_censer_smoke")
rep={}
for i,n in enumerate(DRAW):
    vis=lab==i
    if n=="FX_censer_smoke":
        sm=cv2.dilate(vis.astype(np.uint8),np.ones((9,9),np.uint8)).astype(bool)&((lab<0)|vis)
        a=np.clip((dist-5)/45.0,0,1)*sm; a=cv2.GaussianBlur(a.astype(np.float32),(0,0),1.0)
        a3=np.clip(a,0.06,1)[...,None]
        col=np.clip((master-(1-a3)*bg)/a3,0,255)
        Image.fromarray(np.dstack([col,a*255]).astype(np.uint8),"RGBA").save(f"{OUT}/{n}.png"); continue
    r=GROW.get(n,10)
    k=cv2.getStructuringElement(cv2.MORPH_ELLIPSE,(2*r+1,2*r+1))
    grown=cv2.dilate(vis.astype(np.uint8),k).astype(bool)&(lab>i)&fg&(lab!=DRAW.index("FX_censer_smoke"))
    p=np.zeros((H,W,4),np.float32); p[vis,:3]=master[vis]
    _,(sy,sx)=ndimage.distance_transform_edt(~cv2.erode(vis.astype(np.uint8),np.ones((5,5),np.uint8)).astype(bool),return_indices=True)
    p[grown,:3]=master[sy[grown],sx[grown]]
    w=(vis|grown).astype(np.float32)
    den=np.maximum(cv2.GaussianBlur(w,(0,0),3),1e-3)
    blur=np.dstack([cv2.GaussianBlur(p[...,c]*w,(0,0),3)/den for c in range(3)])
    p[grown,:3]=blur[grown]
    a=np.where(vis|grown,1.0,0.0)
    # 2 px of the neighbours underneath, so no hairline of some third part shows through a cut
    over=cv2.dilate(vis.astype(np.uint8),np.ones((5,5),np.uint8)).astype(bool)&(lab<i)&(lab>=0)&(lab!=SMOKE)
    p[over,:3]=master[over]; a=np.where(over,1.0,a)
    if n in FEATHER:
        d=ndimage.distance_transform_edt(~vis)
        for srcs,reach,msk in FEATHER[n]:
            ext=(d<reach)&np.isin(lab,srcs)&~over
            if msk is not None: ext&=msk
            t=np.clip(1-(d-2)/(reach-2),0,1); fa=t*t*(3-2*t)
            p[ext,:3]=master[ext]; a=np.where(ext,fa,a)
    a=np.where(interior,a,a*edge_alpha)           # hard inside the figure, soft on the outer silhouette
    p[...,3]=a*255
    Image.fromarray(np.clip(p,0,255).astype(np.uint8),"RGBA").save(f"{OUT}/{n}.png")
    rep[n]={"visible":int(vis.sum()),"hidden_fill":int(grown.sum())}
json.dump({"draw":DRAW,"report":rep},open(f"{OUT}/prep_report.json","w"),indent=1)
c=Image.new("RGBA",(W,H),tuple(int(v) for v in bg)+(255,))
for n in DRAW: c.alpha_composite(Image.open(f"{OUT}/{n}.png"))
d=np.abs(np.array(c.convert("RGB")).astype(int)-rgb.astype(int)).sum(2)
print("figure px:",int(fg.sum()),"differing >40 from Battle1:",int(((d>40)&fg).sum()))
c.convert("RGB").save("battle_assembly.png")
