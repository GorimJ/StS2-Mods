#!/usr/bin/env python3
"""
Build-time check for card text defects that the C# compiler cannot see.

Both defects below compile clean, deploy clean, and only surface when somebody happens to look at
the card in game — which is how they have historically been found here.

  1. UNRESOLVED TOKEN. A description references {Something} that the card does not register as a
     DynamicVar. SmartFormat then prints the *entire* description raw, so one stale token wrecks
     the whole card rather than one number. Found in slice 4 after a var-set rewrite left three
     tokens pointing at deleted vars.

  2. UNPAIRED COLOUR TAG. [gold]...[/gold] must be paired; the tags are not self-closing. An
     unclosed one bleeds its colour through the rest of the card.

Exits 1 on any failure, so the build stops before the mod is copied to the game.

Usage:  python tools/check_card_text.py <card-source-root> [more roots...]
"""

import os
import re
import sys

# DynamicVar subclasses whose registered name is implied by the type rather than passed as a string.
IMPLICIT_VAR_NAMES = {
    "DamageVar": "Damage",
    "HealVar": "Heal",
    "BlockVar": "Block",
    "CardsVar": "Cards",
    "HpLossVar": "HpLoss",
    "EnergyVar": "Energy",
    "MaxHpVar": "MaxHp",
    "GoldVar": "Gold",
    "StarsVar": "Stars",
    "SummonVar": "Summon",
    "ForgeVar": "Forge",
    "RepeatVar": "Repeat",
    "StrengthVar": "Strength",
    "ExtraDamageVar": "ExtraDamage",
    "OstyDamageVar": "OstyDamage",
    "CalculationBaseVar": "CalculationBase",
    "CalculationExtraVar": "CalculationExtra",
    "CalculatedDamageVar": "CalculatedDamage",
    "CalculatedBlockVar": "CalculatedBlock",
}

# Tokens the game supplies itself rather than the card registering them.
ENGINE_PROVIDED = {"energyPrefix"}

COLOUR_TAGS = ("gold", "blue", "red", "green", "purple")


def registered_vars(source):
    """Every DynamicVar name this card registers, by either naming convention."""
    names = set()
    # new SomethingVar("Name", ...)  — explicit name
    for match in re.finditer(r'new\s+\w*Var\s*\(\s*"([^"]+)"', source):
        names.add(match.group(1))
    # new DamageVar(6m, ...)  — name implied by the type
    for match in re.finditer(r"new\s+(\w+Var)\s*\(\s*(?!\")", source):
        implied = IMPLICIT_VAR_NAMES.get(match.group(1))
        if implied:
            names.add(implied)
    return names


def description_blocks(source):
    """The text passed to CardLoc(...), which is where card descriptions live."""
    blocks = []
    for match in re.finditer(r"CardLoc\(", source):
        depth, i, start = 1, match.end(), match.end()
        while i < len(source) and depth:
            if source[i] == "(":
                depth += 1
            elif source[i] == ")":
                depth -= 1
            i += 1
        blocks.append(source[start:i - 1])
    return blocks


def string_literals(block):
    """String literals inside a block, so comments and identifiers are not scanned."""
    return re.findall(r'"((?:[^"\\]|\\.)*)"', block)


def check_file(path):
    source = open(path, encoding="utf-8").read()
    if "CardLoc(" not in source:
        return []

    known = registered_vars(source) | ENGINE_PROVIDED
    failures = []

    for block in description_blocks(source):
        text = " ".join(string_literals(block))

        for token in sorted(set(re.findall(r"\{([A-Za-z_][A-Za-z0-9_]*)", text))):
            if token not in known:
                failures.append(
                    "unresolved token {%s} - the whole description will print raw. "
                    "Registered: %s" % (token, ", ".join(sorted(known - ENGINE_PROVIDED)) or "none")
                )

        for tag in COLOUR_TAGS:
            opens = text.count("[%s]" % tag)
            closes = text.count("[/%s]" % tag)
            if opens != closes:
                failures.append(
                    "unpaired [%s] colour tag - %d open, %d close" % (tag, opens, closes)
                )

    return failures


def main(roots):
    files = []
    for root in roots:
        for directory, _, names in os.walk(root):
            files.extend(os.path.join(directory, n) for n in names if n.endswith(".cs"))

    total = 0
    for path in sorted(files):
        for failure in check_file(path):
            print("%s: error : %s" % (path, failure))
            total += 1

    if total:
        print("card text check FAILED: %d problem(s) across %d files" % (total, len(files)))
        return 1

    print("card text check passed (%d files)" % len(files))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:] or ["Seductress/SeductressCode/Cards"]))
