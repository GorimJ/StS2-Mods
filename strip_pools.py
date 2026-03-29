import os
import glob

directories = [
    r"C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\GachaShopMod",
    r"C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\RelicChoice",
    r"C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\MultiplayerPotions"
]

for d in directories:
    for filepath in glob.glob(os.path.join(d, '**', '*.cs'), recursive=True):
        try:
            with open(filepath, 'r', encoding='utf-8') as f:
                lines = f.readlines()
            
            new_lines = []
            modified = False
            for line in lines:
                if "[Pool(" in line:
                    modified = True
                    continue
                new_lines.append(line)
                
            if modified:
                with open(filepath, 'w', encoding='utf-8') as f:
                    f.writelines(new_lines)
                print(f"Purged from {filepath}")
        except Exception as e:
            pass
