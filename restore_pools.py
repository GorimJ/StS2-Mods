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
                content = f.read()

            if "    [Pool(" in content:
                content = content.replace("    [Pool(typeof(MegaCrit.Sts2.Core.Models.SharedPotionPool))]\n", "")
                content = content.replace("    [Pool(typeof(MegaCrit.Sts2.Core.Models.SharedRelicPool))]\n", "")
            
            if "public class TeamOrbitPotion" in content or "public class SummonPotion" in content or "public class RagePotion" in content:
                if "[BaseLib.Utils.Pool(" not in content:
                    content = content.replace("public class", "    [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.PotionPools.SharedPotionPool))]\n    public class")
                    with open(filepath, 'w', encoding='utf-8') as f:
                        f.write(content)
            
            elif "public class" in content and ("Relic" in filepath or "Ticket" in filepath):
                if "[BaseLib.Utils.Pool(" not in content:
                    content = content.replace("public class", "    [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]\n    public class")
                    with open(filepath, 'w', encoding='utf-8') as f:
                        f.write(content)
        except Exception as e:
            pass
