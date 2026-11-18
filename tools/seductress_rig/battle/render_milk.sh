#!/usr/bin/env bash
# usage: bash render_milk.sh <out sub> "<states>" "<bellies>" [frames]   -- idles with the milk layer on
R="C:/Users/Gorim/Documents/STS2Modding"; G="$R/Godot_v4.5.1-stable_mono_win64/Godot_v4.5.1-stable_mono_win64_console.exe"; N=${4:-60}
PARTS_SUB=parts python build_rig_battle.py "$R/Seductress" parts proj >/dev/null
( cd proj && "$G" --headless --path . --import >/dev/null 2>&1 )
for b in $2; do for k in $3; do
  out="$1/${b}__belly${k}"; [ -f "proj/$out/done" ] && continue
  show=""; [ $b != base ] && show="BREASTS_$b"; [ $k != 0 ] && show="$show,BELLY_stage$k"
  MILK=1 PARTS_SUB=parts SHOW=$show python build_rig_battle.py "$R/Seductress" parts proj >/dev/null
  ( cd proj && { mkdir -p "$out"; "$G" --path . zc.tscn --write-movie "$out/f.png" --fixed-fps 10 --quit-after $N --position 3000,3000 2>&1 | grep -i error | head -2; touch "$out/done"; } )
done; done; echo finished
