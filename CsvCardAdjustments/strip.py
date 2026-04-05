import sys

def process_file(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    out_lines = []
    skip_mode = False
    brace_depth = 0
    skip_until_brace = False
    
    classes_to_remove = [
        "class TagTeamPlayPatch",
        "class TagTeamPlayCountFixedPatch",
        "class ParticleWallPlayPatch",
        "class ParticleWallUpgradePatch",
        "class AfterlifeTargetManagerPatch"
    ]

    i = 0
    while i < len(lines):
        line = lines[i]
        
        # Check if we should start removing a patch class
        start_removing = False
        for cls in classes_to_remove:
            if cls in line:
                start_removing = True
                break
                
        if start_removing:
            # We also want to remove the preceding [HarmonyPatch] which is usually 1-2 lines before
            # the class declaration.
            # Pop lines from out_lines until we hit the [HarmonyPatch] or empty line
            while len(out_lines) > 0:
                last_line = out_lines[-1]
                if '[HarmonyPatch' in last_line:
                    out_lines.pop()
                    if len(out_lines) > 0 and '-----' in out_lines[-1]: # pop preceding comment if exists
                        out_lines.pop()
                    if len(out_lines) > 0 and 'Request:' in out_lines[-1]:
                        out_lines.pop()
                    break
                elif last_line.strip() == '' or last_line.strip().startswith('//'):
                    out_lines.pop()
                else:
                    break
                    
            skip_mode = True
            brace_depth = 0
            # count braces in the current line
            brace_depth += line.count('{')
            brace_depth -= line.count('}')
            if '{' not in line:
                skip_until_brace = True
            i += 1
            continue

        if skip_mode:
            if skip_until_brace and '{' in line:
                skip_until_brace = False
                
            brace_depth += line.count('{')
            brace_depth -= line.count('}')
            
            if brace_depth <= 0 and not skip_until_brace:
                skip_mode = False
            i += 1
            continue
            
        # Also remove if (__instance is HelpfulAdvice) or if (__instance is AllyAfterlife) blocks
        if ('if (__instance is HelpfulAdvice)' in line or 'if (__instance is AllyAfterlife)' in line) and '{' in line:
            # This is a simple 3-line block usually:
            # if (__instance is HelpfulAdvice) {
            #     __result = ...
            # }
            # Let's skip until closing brace
            skip_subblock = True
            temp_i = i
            temp_brace = line.count('{') - line.count('}')
            while True:
                temp_i += 1
                temp_brace += lines[temp_i].count('{') - lines[temp_i].count('}')
                if temp_brace <= 0:
                    break
            i = temp_i + 1
            continue

        out_lines.append(line)
        i += 1

    with open(file_path, 'w', encoding='utf-8') as f:
        f.writelines(out_lines)

process_file('CustomCardOverridesPatch.cs')
