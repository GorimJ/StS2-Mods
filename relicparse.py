import os
relics = {
    'Ironclad': ['Brimstone', 'BurningBlood', 'CharonsAshes', 'DemonTongue', 'PaperPhrog', 'RedSkull', 'RuinedHelmet', 'SelfFormingClay'],
    'Silent': ['HelicalDart', 'NinjaScroll', 'PaperKrane', 'RingOfTheSnake', 'SneckoSkull', 'Tingsha', 'ToughBandages', 'TwistedFunnel'],
    'Defect': ['CrackedCore', 'DataDisk', 'EmotionChip', 'GoldPlatedCables', 'PowerCell', 'Metronome', 'RunicCapacitor', 'SymbioticVirus'],
    'Necrobinder': ['BigHat', 'BoneFlute', 'BookRepairKnife', 'Bookmark', 'BoundPhylactery', 'FuneraryMask', 'IvoryTile', 'UndyingSigil'],
    'Regent': ['DivineRight', 'FencingManual', 'GalacticDust', 'LunarPastry', 'MiniRegent', 'OrangeDough', 'Regalite', 'VitruvianMinion']
}
base_path = r'C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\decompile_out\MegaCrit.Sts2.Core.Models.Relics'

for cls, class_relics in relics.items():
    print(f'--- {cls} ---')
    for r in class_relics:
        p = os.path.join(base_path, r + '.cs')
        if os.path.exists(p):
            with open(p, 'r') as f:
                for line in f:
                    if 'public override RelicRarity Rarity' in line:
                        print(f'{r}: {line.split(".")[-1].strip()[:-1]}')
                        break
