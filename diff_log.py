import sys, re
def clean_log(filepath):
    with open(filepath, 'r', encoding='utf-8', errors='ignore') as f:
        content = f.read()
    start = content.find('ModelIdSerializationCache initialized')
    if start != -1: content = content[:start]
    
    cleaned = []
    for line in content.splitlines():
        line = re.sub(r'^\s*\[.*?\]\s*', '', line) 
        line = re.sub(r'[A-Za-z]:\\[\w\\\s]+\\mods\\', 'MODDIR ', line)
        line = re.sub(r'[A-Za-z]:\\[/\w\\\s]+\\SlayTheSpire2\.exe', 'STS_PATH', line)
        line = re.sub(r'0x[a-fA-F0-9]+', 'PTR', line)
        line = re.sub(r'\d+ ms', 'TIME', line)
        line = re.sub(r'\d+ms', 'TIME', line)
        line = re.sub(r'[\d,]+ms', 'TIME', line)
        line = re.sub(r'steam/\d+/', 'STEAM_ID/', line)
        line = re.sub(r'Display: \d+ of \d+', 'DISPLAY_TXT', line)
        line = re.sub(r'Size: \(\d+, \d+\)', 'SIZE_TXT', line)
        
        skip = False
        for kw in ["Timestamp", "OS Version", "Distribution", "Device Model", "Data Directory", 
                   "OS Locale", "OS Language", "Free:", "Physical:", "Graphics adapter", "version:",
                   "Memory Info:", "physical:", "free:", "available:", "stack:", "Important Environment",
                   "PATH:", "Executable Path", "Processor Count", "Processor Name", "Rendering device",
                   "Index 0:", "Index 1:", "Index 2:", "Video Memory", "Static Memory", "GODOT_", "HOME", "DYLD", "LD_L",
                   "Setting FULLSCREEN", "Window changed!", "Godot ticks", "SteamStatsManager", "Resource stats", "Wrote ", "D3D12"]:
            if kw in line:
                skip = True
                break
        if skip: continue
        if line.strip() == "": continue
        cleaned.append(line.strip())
    return cleaned

my_lines = clean_log(r"C:\Users\Gorim\Documents\StS Mods\logs6\My logs\godot.log")
momo_lines = clean_log(r"C:\Users\Gorim\Documents\StS Mods\logs6\Momo's logs\godot.log")

print("--- MY NOT MOMO ---")
for l in my_lines:
    if l not in momo_lines: print(l[:120])

print("--- MOMO NOT MY ---")
for l in momo_lines:
    if l not in my_lines: print(l[:120])
