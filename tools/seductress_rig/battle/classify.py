import numpy as np, cv2
from PIL import Image
SRC='C:/Users/Gorim/Documents/STS2Modding/Seductress/Battle1.png'
def load():
    rgb=np.array(Image.open(SRC).convert('RGB'))
    hsv=cv2.cvtColor(rgb,cv2.COLOR_RGB2HSV_FULL).astype(np.float32)   # H 0..255
    H=hsv[...,0]*360/255; S=hsv[...,1]; V=hsv[...,2]
    r,g,b=[rgb[...,i].astype(np.float32) for i in range(3)]
    bg=np.median(rgb[:40,:40].reshape(-1,3),axis=0).astype(np.float32)
    dist=np.sqrt(((rgb.astype(np.float32)-bg)**2).sum(2))
    warm=(r-b)   # bg and its shadow are bluish: r-b < 0
    fg=(warm>6)|((r-g>10)&(dist>25))
    fg=cv2.morphologyEx(fg.astype(np.uint8),cv2.MORPH_OPEN,np.ones((3,3),np.uint8))
    # keep big components, fill small holes
    n,cc,st,_=cv2.connectedComponentsWithStats(fg,8)
    keep=np.zeros_like(fg,bool)
    for i in range(1,n):
        if st[i,4]>=400: keep|=cc==i
    inv=(~keep).astype(np.uint8); n,cc,st,_=cv2.connectedComponentsWithStats(inv,4)
    for i in range(1,n):
        if st[i,4]<300: keep[cc==i]=True
    fg=keep
    smoke=(~fg)&(b>r)&(r>g+6)&(dist>14)
    cls=np.zeros(rgb.shape[:2],np.uint8)        # 0 bg 1 skin 2 gold 3 hair/brown 4 purple 5 dark/other 6 smoke
    hue_warm=(H<60)|(H>345)
    skin=fg&(H>=8)&(H<26)&(V>135)&(S>70)&(r-b>55)
    gold=fg&(H>=26)&(H<52)&(V>95)&(S>70)
    purple=fg&((H>=255)&(H<=345))&(b>g)
    brown=fg&~skin&~gold&~purple&hue_warm&(V>45)
    cls[fg]=5; cls[brown]=3; cls[purple]=4; cls[gold]=2; cls[skin]=1; cls[smoke]=6
    return rgb,cls,fg,dist,bg
if __name__=='__main__':
    rgb,cls,fg,dist,bg=load()
    pal=np.array([[40,40,40],[255,180,140],[255,220,0],[150,80,30],[160,60,200],[0,0,0],[120,160,255]],np.uint8)
    Image.fromarray(pal[cls]).resize((652,869),Image.NEAREST).save('classmap.png')
    print({i:int((cls==i).sum()) for i in range(7)})
