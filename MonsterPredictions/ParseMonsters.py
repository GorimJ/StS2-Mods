import os
import re

path = r"C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\decompile_out\MegaCrit.Sts2.Core.Models.Monsters"
output = "MonsterDictionary.cs"

with open(output, 'w', encoding='utf-8') as out:
    out.write("using Godot;\n")
    out.write("namespace MonsterPredictions;\n")
    out.write("public static class MonsterDictionary {\n")
    out.write("    public static int GetStrengthGainFromMove(string monsterId, string stateId, int ascensionLevel) {\n")
    out.write("        switch(monsterId) {\n")

    for file in os.listdir(path):
        if not file.endswith(".cs"): continue
        monster_id = file[:-3]
        with open(os.path.join(path, file), 'r', encoding='utf-8') as f:
            content = f.read()
            
        move_map = {}
        for match in re.finditer(r'new MoveState\(\s*"([^"]+)"\s*,\s*([A-Za-z0-9_]+)', content):
            move_map[match.group(2)] = match.group(1)
            
        if not move_map: continue
        
        printed_monster = False
        for method, state in move_map.items():
            method_idx = content.find(f" {method}(")
            if method_idx == -1: continue
            
            next_private = content.find("private ", method_idx + 10)
            next_public = content.find("public ", method_idx + 10)
            next_protected = content.find("protected ", method_idx + 10)
            
            end_idx = len(content)
            for n in [next_private, next_public, next_protected]:
                if n != -1 and n < end_idx:
                    end_idx = n
                    
            method_body = content[method_idx:end_idx]
            
            strength_match = re.search(r'Apply<StrengthPower>\s*\([^,]+,\s*([A-Za-z0-9_\.]+)m?\s*,', method_body)
            gain_strength_match = re.search(r'CreatureCmd\.GainStrength\([^,]+,\s*([A-Za-z0-9_\.]+)', method_body)
            
            amount_str = None
            if strength_match:
                amount_str = strength_match.group(1)
            elif gain_strength_match:
                amount_str = gain_strength_match.group(1)
                
            if amount_str:
                amount_str = amount_str.strip()
                if amount_str.endswith('m'):
                    amount_str = amount_str[:-1]
                
                if not printed_monster:
                    out.write(f'            case "{monster_id}":\n')
                    printed_monster = True
                    
                if amount_str.isdigit():
                    out.write(f'                if (stateId == "{state}") return {int(amount_str)};\n')
                else:
                    prop_match = re.search(rf'{amount_str}\s*=>\s*AscensionHelper\.GetValueIfAscension[^,]+,\s*([0-9]+)\s*,\s*([0-9]+)', content)
                    if prop_match:
                        out.write(f'                if (stateId == "{state}") return ascensionLevel >= 2 ? {prop_match.group(1)} : {prop_match.group(2)};\n')
                    else:
                        prop_match2 = re.search(rf'{amount_str}\s*=>\s*([0-9]+)', content)
                        if prop_match2:
                            out.write(f'                if (stateId == "{state}") return {prop_match2.group(1)};\n')
                        else:
                            out.write(f'                // MISSING PROP RESOLUTION: if (stateId == "{state}") return {amount_str};\n')

        if printed_monster:
            out.write('                break;\n')

    out.write('            case "Nibbit":\n')
    out.write('                if (stateId == "HISS_MOVE") return ascensionLevel >= 2 ? 3 : 2;\n')
    out.write('                break;\n')
    out.write('            case "TurretOperator":\n')
    out.write('                if (stateId == "RELOAD_MOVE") return ascensionLevel >= 2 ? 3 : 2;\n')
    out.write('                break;\n')

    out.write("        }\n")
    out.write("        return 0;\n")
    out.write("    }\n")
    out.write("}\n")
