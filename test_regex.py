import re

filepath = r'C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\decompile_out\MegaCrit.Sts2.Core.Models.Monsters\FuzzyWurmCrawler.cs'
content = open(filepath, 'r', encoding='utf-8').read()

print("File Read:", len(content))
print("Model check:", ": MonsterModel" in content)
print("SM check:", "GenerateMoveStateMachine()" in content)
print("Str check:", "StrengthPower" in content or "GainStrength" in content)

class_match = re.search(r'class\s+([^:\s]+)\s*:', content)
print("Class Match:", class_match.group(1) if class_match else "FAILED")

# Extract MoveStates
state_mapping = {}
for line in content.split('\n'):
    match = re.search(r'new MoveState\("([^"]+)",\s*([A-Za-z0-9_]+)', line)
    if match:
        state_mapping[match.group(2)] = match.group(1)
        print("Mapped state:", match.group(2), "->", match.group(1))

print("Total mappings:", len(state_mapping))

for method, state_id in state_mapping.items():
    method_pattern = r'Task\s+' + method + r'\s*\(.*?\{(.*?)\n\s*\}'
    method_match = re.search(method_pattern, content, re.DOTALL)
    if method_match:
        method_content = method_match.group(1)
        print("\nFound method content for", method, "length:", len(method_content))
        
        strength_match = re.search(r"PowerCmd\.Apply<StrengthPower>\(b?a?s?e?\.?Creature,\s*([^,]+)", method_content)
        if strength_match:
            print("  -> Strength pattern matched:", strength_match.group(1))
        else:
            print("  -> Strength pattern DID NOT match!")
    else:
        print("\nCould not find method block for", method)

