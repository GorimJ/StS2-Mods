#!/usr/bin/env bash
# [Claude, 2026-11-14] Build Seductress.pck from ONLY the mod's own asset folder and install it.
#
# Why this exists: `dotnet publish` runs Godot's export over the whole Seductress project folder,
# which also holds hundreds of MB of loose working art (cut passes, references, .pdn files). That
# produced a 451 MB pck where the real one is about 500 KB. This stages a clean copy with just
# project.godot, export_presets.cfg and Seductress/Seductress (images, localization, scenes) and
# exports that.
#
# The game must be closed first (tools/sts2.ps1 stop) or the copy into mods/ fails.
#
#   bash tools/pack_seductress_pck.sh [--no-install]
set -euo pipefail

REPO="$(cd "$(dirname "$0")/.." && pwd)"
GODOT="$REPO/Godot_v4.5.1-stable_mono_win64/Godot_v4.5.1-stable_mono_win64_console.exe"
MODS="A:/SteamLibrary/steamapps/common/Slay the Spire 2/mods/Seductress"
STAGE="$(mktemp -d)"
trap 'rm -rf "$STAGE"' EXIT

cp "$REPO/Seductress/project.godot" "$REPO/Seductress/export_presets.cfg" "$STAGE/"
cp -r "$REPO/Seductress/Seductress" "$STAGE/Seductress"
mkdir -p "$STAGE/out"

# Import first so every texture has its .ctex, then export the pack.
"$GODOT" --headless --path "$STAGE" --import >"$STAGE/import.log" 2>&1 || true
"$GODOT" --headless --path "$STAGE" --export-pack "BasicExport" "$STAGE/out/Seductress.pck" >"$STAGE/export.log" 2>&1 || {
    tail -20 "$STAGE/export.log"; echo "pack: export FAILED"; exit 1; }

# New assets get their .import file from the import above; keep those in the repo beside the asset,
# as the existing ones are. Never overwrites one that is already there.
(cd "$STAGE/Seductress" && find . -name "*.import" | while read -r f; do
    [ -e "$REPO/Seductress/Seductress/$f" ] || { cp "$f" "$REPO/Seductress/Seductress/$f"; echo "pack: new $f"; }
done)

SIZE=$(stat -c %s "$STAGE/out/Seductress.pck")
if [ "$SIZE" -gt 20000000 ]; then
    echo "pack: REFUSING - pck is $SIZE bytes, which means stray files got in"; exit 1
fi
echo "pack: built Seductress.pck ($SIZE bytes)"

if [ "${1:-}" != "--no-install" ]; then
    cp "$STAGE/out/Seductress.pck" "$MODS/Seductress.pck"
    echo "pack: installed to $MODS"
fi
