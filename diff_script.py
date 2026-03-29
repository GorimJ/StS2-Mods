import sys

def get_dump(path):
    with open(path, 'r', encoding='utf-8', errors='ignore') as f:
        text = f.read()
    start = text.rfind('CATEGORIES')
    if start == -1: return []
    end = text.find('Hash:', start)
    if end == -1: return []
    dump_text = text[start:end]
    lines = [x.strip() for x in dump_text.split('\n') if x.strip()]
    return lines

my_lines = get_dump(r"C:\Users\Gorim\Documents\StS Mods\logs6\My logs\godot.log")
momo_lines = get_dump(r"C:\Users\Gorim\Documents\StS Mods\logs6\Momo's logs\godot.log")
rob_lines = get_dump(r"C:\Users\Gorim\Documents\StS Mods\logs6\Robbie's logs\godot.log")

print(f"My Length: {len(my_lines)} | Momo Length: {len(momo_lines)} | Rob Length: {len(rob_lines)}")

my_set = set(my_lines)
momo_set = set(momo_lines)
rob_set = set(rob_lines)

print("IN MY, NOT MOMO:")
for x in my_set - momo_set: print(x)
print("IN MOMO, NOT MY:")
for x in momo_set - my_set: print(x)

if my_set == momo_set:
    print("\nSETS ARE IDENTICAL! Checking order index by index...")
    for i in range(min(len(my_lines), len(momo_lines))):
        if my_lines[i] != momo_lines[i]:
            print(f"Diff at index {i}: MY='{my_lines[i]}' MOMO='{momo_lines[i]}'")
            break
