import os

relics = [
    'rainbow_relic_common_1',
    'rainbow_relic_uncommon_1',
    'rainbow_relic_uncommon_2',
    'rainbow_relic_rare_1',
    'rainbow_relic_rare_2',
    'rainbow_relic_rare_3'
]

content = """[gd_resource type="AtlasTexture" load_steps=2 format=3]

[ext_resource type="Texture" path="res://icon.svg" id="1_1"]

[resource]
atlas = ExtResource("1_1")
"""

base = r"C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\RelicChoice\images\atlases"
dirs = [
    os.path.join(base, "relic_atlas.sprites"),
    os.path.join(base, "relic_outline_atlas.sprites")
]

for d in dirs:
    for r in relics:
        path = os.path.join(d, f"relicchoice-{r}.tres")
        with open(path, "w") as f:
            f.write(content)

print("Generated 12 .tres files.")
