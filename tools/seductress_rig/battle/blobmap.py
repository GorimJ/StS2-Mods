"""Label every figure pixel of Battle1.png with a part name, from material class + coarse regions. [Claude]"""
import numpy as np, cv2, json
from PIL import Image
from scipy import ndimage
from classify import load
rgb,cls,fg,dist,bg=load()
H,W=cls.shape
yy,xx=np.mgrid[0:H,0:W]
SKIN=cls==1; PUR=(cls==4); WARM=np.isin(cls,[2,3,5]); SMK=np.isin(cls,[4,6])
def poly(pts):
    m=np.zeros((H,W),np.uint8); cv2.fillPoly(m,[np.array(pts,np.int32)],1); return m.astype(bool)
def rect(x0,y0,x1,y1): return (xx>=x0)&(xx<x1)&(yy>=y0)&(yy<y1)
DRAW=["HAIR_back","SKIRT_rear_left","SKIRT_rear_right","LEG_R_shin_foot","LEG_R_thigh","LEG_L_shin_foot","LEG_L_thigh",
      "BELLY_lower_torso_variant","TORSO_upper","ARM_L_upper","HEAD_face_plus_front_hair","BREASTS_plus_wrap_variant",
      "HAIR_side_R","HAIR_side_L","SLEEVE_L","SLEEVE_R","ARM_L_forearm_hand","ARM_R_forearm_hand","ARM_R_hand","FINGER_R_pinky","FINGER_R_ring","FINGER_R_middle","FINGER_R_index",
      "CLOTH_front_skirt_panel","HIP_ornaments_left","HIP_ornaments_right","PROP_censer_and_chains","FX_censer_smoke",
      "HAT_main","HAT_charm"]
lab=np.full((H,W),-1,np.int32)
def put(name,mask):
    m=mask&(lab<0)&((fg)|(name=="FX_censer_smoke"))
    lab[m]=DRAW.index(name)
BULK=cv2.morphologyEx(WARM.astype(np.uint8),cv2.MORPH_OPEN,cv2.getStructuringElement(cv2.MORPH_ELLIPSE,(9,9))).astype(bool)
BULK=cv2.dilate(BULK.astype(np.uint8),np.ones((7,7),np.uint8)).astype(bool)&WARM      # hair-like: not a thin gold trim
SLEEVE_R_POLY=poly([(700,478),(965,478),(965,905),(935,897),(800,802),(760,802),(722,640),(700,600)])
SLEEVE_L_POLY=poly([(285,445),(482,445),(482,545),(455,640),(430,700),(410,770),(345,890),(285,890)])
PANEL_POLY=poly([(522,622),(715,640),(718,705),(703,770),(702,1215),(598,1215),(598,800),(575,740),(535,700)])
LOCK_L_POLY=poly([(538,395),(592,395),(592,470),(580,560),(565,690),(455,690),(455,570),(505,490),(538,470)])
put("FX_censer_smoke", SMK&(xx<262)&(yy<730))
put("HAT_charm", WARM&rect(728,318,782,462))
put("HAT_main", (PUR&(yy<400))|(WARM&(yy<258)))
put("HEAD_face_plus_front_hair", (SKIN&rect(585,280,690,398))|(WARM&rect(548,255,745,400)))
put("TORSO_upper", WARM&rect(575,405,625,455))                      # necklace
put("ARM_R_forearm_hand", (SKIN&rect(762,400,945,625))|(WARM&rect(800,500,860,570)))
put("ARM_L_forearm_hand", (SKIN&rect(220,455,400,570))|(WARM&rect(322,490,372,558)))
put("PROP_censer_and_chains", ((~SKIN)&rect(150,500,300,900)&((yy>560)|WARM))|(WARM&rect(300,560,318,800)))
put("CLOTH_front_skirt_panel", rect(602,800,700,1215))
put("ARM_L_upper", SKIN&rect(440,395,538,495))
put("TORSO_upper", SKIN&rect(538,395,765,548))
put("BELLY_lower_torso_variant", SKIN&rect(540,548,706,692))
put("LEG_L_thigh", SKIN&rect(450,620,650,940))
put("LEG_L_shin_foot", (SKIN&poly([(420,940),(650,940),(650,1000),(520,1060),(440,1110),(400,1320),(270,1320),(270,1100),(340,1060)]))|(WARM&rect(265,1060,455,1315)))
put("LEG_R_thigh", SKIN&rect(650,620,850,897))
put("LEG_R_shin_foot", (SKIN&poly([(735,897),(885,897),(905,1060),(960,1255),(790,1255),(790,1060),(745,1000)]))|(WARM&rect(790,1050,960,1255)))
put("HIP_ornaments_left", (WARM&rect(505,615,585,810))|(PUR&rect(508,672,548,765)))
put("HIP_ornaments_right", (WARM&rect(683,640,790,845))|(PUR&rect(716,696,770,768)))
put("SLEEVE_R", (WARM&rect(698,505,745,558))|(PUR&SLEEVE_R_POLY))
put("SLEEVE_L", (WARM&rect(400,474,474,545))|(PUR&SLEEVE_L_POLY))
put("BREASTS_plus_wrap_variant", PUR&poly([(535,478),(700,485),(703,560),(692,603),(555,608),(540,560)]))
put("HAIR_side_L", BULK&LOCK_L_POLY)
put("HAIR_side_R", BULK&rect(628,400,765,645))
put("HAIR_back", BULK&(yy<700)&(xx>300)&(xx<628)&~SLEEVE_L_POLY)
put("CLOTH_front_skirt_panel", PUR&PANEL_POLY)
put("SKIRT_rear_left", PUR&(yy>=600)&(xx<640))
put("SKIRT_rear_right", PUR&(yy>=600)&(xx>=640))
# everything still unlabelled inside the figure takes the nearest label
un=fg&(lab<0)
_,(iy,ix)=ndimage.distance_transform_edt(lab<0,return_indices=True)
lab[un]=lab[iy[un],ix[un]]
# tidy: tiny islands of a label take the surrounding label
for i,n in enumerate(DRAW):
    m=(lab==i).astype(np.uint8); k,cc,st,_=cv2.connectedComponentsWithStats(m,8)
    for j in range(1,k):
        if st[j,4]<120 and n!="FX_censer_smoke":
            lab[cc==j]=-2
hole=lab==-2
_,(iy,ix)=ndimage.distance_transform_edt((lab<0),return_indices=True)
lab[hole]=lab[iy[hole],ix[hole]]
# the wrap must be one swappable piece: everything inside it that is not skin belongs to the breast part,
# and so does the wrap's dark outline where it was filed under torso/belly [Claude]
BR=DRAW.index("BREASTS_plus_wrap_variant")
core=poly([(560,488),(697,488),(700,592),(548,606),(548,545),(560,545)])
lab[core&fg&~SKIN]=BR
ring=cv2.dilate((lab==BR).astype(np.uint8),np.ones((9,9),np.uint8)).astype(bool)
dark=rgb.max(2)<175
lab[ring&dark&np.isin(lab,[DRAW.index("TORSO_upper"),DRAW.index("BELLY_lower_torso_variant")])]=BR
# raised hand: split hand from forearm at the wrist, then the four fingers from the hand [Claude, by eye]
A=DRAW.index("ARM_R_forearm_hand")
arm=lab==A
lab[arm&(((xx-862)*0.65-(yy-500)*0.76)>0)]=DRAW.index("ARM_R_hand")
FING={"FINGER_R_index":[(904,457),(905,447),(909,430),(913,412),(920,402),(932,406),(930,452),(918,461)],
      "FINGER_R_middle":[(894,463),(893,443),(886,435),(873,424),(872,408),(884,407),(898,422),(905,439),(906,463)],
      "FINGER_R_ring":[(886,468),(877,456),(871,450),(856,449),(848,446),(850,437),(870,435),(889,439),(893,443),(894,463)],
      "FINGER_R_pinky":[(872,474),(866,461),(852,460),(843,454),(846,447),(856,449),(871,450),(877,456),(886,468),(880,475)]}
for n,pts in FING.items():
    lab[(lab==DRAW.index("ARM_R_hand"))&poly(pts)]=DRAW.index(n)
np.save('labels.npy',lab); json.dump(DRAW,open('draw.json','w'))
rng=np.random.RandomState(4); pal=(rng.rand(len(DRAW),3)*200+55).astype(np.uint8)
out=np.full((H,W,3),40,np.uint8); out[lab>=0]=pal[lab[lab>=0]]
mix=(0.6*out+0.4*rgb).astype(np.uint8)
edges=np.zeros((H,W),bool); edges[:-1]|=lab[:-1]!=lab[1:]; edges[:,:-1]|=lab[:,:-1]!=lab[:,1:]
mix[edges]=[255,255,255]
Image.fromarray(mix).save('blob_preview.png')
print({n:int((lab==i).sum()) for i,n in enumerate(DRAW)})
