"""
Slay the Spire 2 — Mod Installer
Distributable GUI for installing mods and transferring saves.
"""

import os
import sys
import json
import shutil
import pathlib
import re
import tkinter as tk
from tkinter import filedialog, messagebox
from typing import Optional

# ── Path resolution ────────────────────────────────────────────────────────────
# When frozen by PyInstaller, sys.executable is the .exe path.
# The mods/ folder lives alongside the .exe (or alongside installer.py in dev).
if getattr(sys, "frozen", False):
    BASE_DIR = pathlib.Path(sys.executable).parent
else:
    BASE_DIR = pathlib.Path(__file__).parent

MODS_SOURCE_DIR = BASE_DIR / "mods"

# ── STS2 / Steam detection ─────────────────────────────────────────────────────
def _steam_library_paths() -> list[pathlib.Path]:
    """Read Steam's libraryfolders.vdf and return all steamapps paths."""
    roots = [
        pathlib.Path(os.environ.get("ProgramFiles(x86)", "C:/Program Files (x86)")) / "Steam",
        pathlib.Path(os.environ.get("ProgramFiles", "C:/Program Files")) / "Steam",
        pathlib.Path("C:/Program Files (x86)/Steam"),
        pathlib.Path("C:/Program Files/Steam"),
    ]
    for steam in roots:
        vdf = steam / "steamapps" / "libraryfolders.vdf"
        if not vdf.exists():
            continue
        results = [steam / "steamapps"]
        try:
            text = vdf.read_text(encoding="utf-8", errors="ignore")
            for m in re.finditer(r'"path"\s+"([^"]+)"', text):
                p = pathlib.Path(m.group(1).replace("\\\\", "\\")) / "steamapps"
                if p.exists():
                    results.append(p)
        except Exception:
            pass
        return results
    return []


def find_sts2_mods_folder() -> Optional[pathlib.Path]:
    for lib in _steam_library_paths():
        game = lib / "common" / "Slay the Spire 2"
        if game.exists():
            mods = game / "mods"
            mods.mkdir(exist_ok=True)
            return mods
    return None


def find_sts2_save_folder() -> Optional[pathlib.Path]:
    base = pathlib.Path(os.environ["APPDATA"]) / "SlayTheSpire2" / "steam"
    if not base.exists():
        return None
    subdirs = [d for d in base.iterdir() if d.is_dir()]
    if len(subdirs) == 1:
        return subdirs[0]
    return None


# ── Mod discovery ──────────────────────────────────────────────────────────────
def discover_mods() -> list[dict]:
    mods = []
    if not MODS_SOURCE_DIR.exists():
        return mods
    for mod_dir in sorted(MODS_SOURCE_DIR.iterdir()):
        if not mod_dir.is_dir():
            continue
        # Accept mod_manifest.json or <Name>.json
        meta_path = mod_dir / "mod_manifest.json"
        if not meta_path.exists():
            found = list(mod_dir.glob("*.json"))
            meta_path = found[0] if found else None
        if not meta_path:
            continue
        try:
            meta = json.loads(meta_path.read_text(encoding="utf-8"))
        except Exception:
            continue
        mods.append({
            "id":            meta.get("id", mod_dir.name),
            "name":          meta.get("name", mod_dir.name),
            "version":       meta.get("version", ""),
            "author":        meta.get("author", ""),
            "description":   meta.get("description", ""),
            "path":          mod_dir,
            "is_dependency": meta.get("id", "") == "BaseLib",
        })
    # Sort: BaseLib first so it's always installed, then alphabetically
    mods.sort(key=lambda m: (not m["is_dependency"], m["name"].lower()))
    return mods


# ── Install / transfer logic ───────────────────────────────────────────────────
def install_mods(selected_ids: list[str], mods: list[dict], dest: pathlib.Path):
    # Always include BaseLib
    to_install = [m for m in mods if m["id"] in selected_ids or m["is_dependency"]]
    for mod in to_install:
        for f in mod["path"].iterdir():
            if f.is_file():
                shutil.copy2(f, dest / f.name)


def transfer_saves(save_folder: pathlib.Path):
    modded = save_folder / "modded"
    modded.mkdir(exist_ok=True)
    for item in save_folder.iterdir():
        if item.name == "modded":
            continue
        dest = modded / item.name
        if item.is_dir():
            if dest.exists():
                shutil.rmtree(dest)
            shutil.copytree(item, dest)
        else:
            shutil.copy2(item, dest)


# ── Colours & fonts ────────────────────────────────────────────────────────────
BG      = "#12121f"
BG2     = "#1c1c2e"
BG3     = "#252540"
ACCENT  = "#c84b31"
FG      = "#e8e8f0"
FG_DIM  = "#7a7a9a"
GREEN   = "#4ecdc4"
RED     = "#e05454"
BORDER  = "#2e2e4e"

FONT_H1   = ("Segoe UI", 20, "bold")
FONT_H2   = ("Segoe UI", 11, "bold")
FONT_BODY = ("Segoe UI", 9)
FONT_SMALL= ("Segoe UI", 8)
FONT_MONO = ("Consolas", 9)


# ── GUI ────────────────────────────────────────────────────────────────────────
class InstallerApp(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("Slay the Spire 2 — Mod Installer")
        self.configure(bg=BG)
        self.resizable(False, False)

        self.mods = discover_mods()
        detected_mods_folder = find_sts2_mods_folder()
        self.mods_folder_var = tk.StringVar(value=str(detected_mods_folder or ""))
        self.save_folder = find_sts2_save_folder()
        self.checkboxes: dict[str, tk.BooleanVar] = {}
        self.status_var = tk.StringVar()

        self._build_ui()
        self.update_idletasks()
        self._center()

    def _center(self):
        w, h = self.winfo_width(), self.winfo_height()
        x = (self.winfo_screenwidth()  - w) // 2
        y = (self.winfo_screenheight() - h) // 2
        self.geometry(f"+{x}+{y}")

    # ── UI construction ────────────────────────────────────────────────────────
    def _build_ui(self):
        self._header()
        body = tk.Frame(self, bg=BG, padx=28, pady=20)
        body.pack(fill="both", expand=True)
        self._mods_folder_row(body)
        self._mod_list(body)
        self._divider(body)
        self._save_section(body)
        self._divider(body)
        self._install_row(body)

    def _header(self):
        h = tk.Frame(self, bg=BG2, pady=20)
        h.pack(fill="x")
        tk.Label(h, text="Slay the Spire 2", font=FONT_SMALL,
                 bg=BG2, fg=FG_DIM).pack()
        tk.Label(h, text="Mod Installer", font=FONT_H1,
                 bg=BG2, fg=FG).pack()
        tk.Label(h, text="Choose which mods to install, then click Install.",
                 font=FONT_BODY, bg=BG2, fg=FG_DIM).pack(pady=(4, 0))

    def _mods_folder_row(self, parent):
        tk.Label(parent, text="Game mods folder", font=("Segoe UI", 8, "bold"),
                 bg=BG, fg=FG_DIM).pack(anchor="w", pady=(0, 3))
        row = tk.Frame(parent, bg=BG)
        row.pack(fill="x", pady=(0, 18))

        detected = bool(self.mods_folder_var.get())
        indicator = "✓ Auto-detected" if detected else "⚠ Not found — browse to set"
        ind_color = GREEN if detected else ACCENT
        tk.Label(row, text=indicator, font=FONT_SMALL, bg=BG, fg=ind_color).pack(anchor="w")

        entry_row = tk.Frame(row, bg=BG)
        entry_row.pack(fill="x", pady=(3, 0))
        entry = tk.Entry(entry_row, textvariable=self.mods_folder_var,
                         font=FONT_MONO, bg=BG2, fg=FG,
                         insertbackground=FG, relief="flat",
                         highlightthickness=1, highlightbackground=BORDER,
                         highlightcolor=ACCENT)
        entry.pack(side="left", fill="x", expand=True, ipady=6, padx=(0, 8))
        self._button(entry_row, "Browse…", self._browse_mods_folder,
                     small=True).pack(side="left")

    def _mod_list(self, parent):
        tk.Label(parent, text="Available Mods", font=("Segoe UI", 8, "bold"),
                 bg=BG, fg=FG_DIM).pack(anchor="w", pady=(0, 4))

        installable = [m for m in self.mods if not m["is_dependency"]]
        if not installable:
            msg = "No mods found." if MODS_SOURCE_DIR.exists() else \
                  f"mods/ folder not found next to installer."
            card = tk.Frame(parent, bg=BG2,
                            highlightthickness=1, highlightbackground=BORDER)
            card.pack(fill="x", pady=(0, 16))
            tk.Label(card, text=msg, bg=BG2, fg=FG_DIM,
                     font=FONT_BODY, pady=20).pack()
            return

        list_frame = tk.Frame(parent, bg=BG2,
                              highlightthickness=1, highlightbackground=BORDER)
        list_frame.pack(fill="x", pady=(0, 16))

        for i, mod in enumerate(installable):
            self._mod_row(list_frame, mod, i)

    def _mod_row(self, parent, mod: dict, index: int):
        bg = BG2 if index % 2 == 0 else BG3
        row = tk.Frame(parent, bg=bg, padx=16, pady=11)
        row.pack(fill="x")

        var = tk.BooleanVar(value=True)
        self.checkboxes[mod["id"]] = var

        tk.Checkbutton(
            row, variable=var, bg=bg, activebackground=bg,
            selectcolor=BG, fg=FG, cursor="hand2",
            relief="flat", highlightthickness=0,
        ).pack(side="left", padx=(0, 10))

        info = tk.Frame(row, bg=bg)
        info.pack(side="left", fill="x", expand=True)

        name_row = tk.Frame(info, bg=bg)
        name_row.pack(anchor="w")
        tk.Label(name_row, text=mod["name"], font=("Segoe UI", 10, "bold"),
                 bg=bg, fg=FG).pack(side="left")
        if mod["version"]:
            tk.Label(name_row, text=f"  v{mod['version']}", font=FONT_SMALL,
                     bg=bg, fg=FG_DIM).pack(side="left")
        if mod["author"]:
            tk.Label(name_row, text=f"  by {mod['author']}", font=FONT_SMALL,
                     bg=bg, fg=FG_DIM).pack(side="left")
        if mod["description"]:
            tk.Label(info, text=mod["description"], font=FONT_BODY,
                     bg=bg, fg=FG_DIM, wraplength=400, justify="left").pack(anchor="w")

    def _save_section(self, parent):
        row = tk.Frame(parent, bg=BG)
        row.pack(fill="x", pady=(14, 4))
        tk.Label(row, text="Save Transfer", font=FONT_H2,
                 bg=BG, fg=FG).pack(side="left")
        found = self.save_folder is not None
        tk.Label(row, text="  ✓ Save folder found" if found else "  ✗ Save folder not found",
                 font=FONT_SMALL, bg=BG,
                 fg=GREEN if found else RED).pack(side="left")

        tk.Label(parent,
                 text="Copies your current (unmodded) save into the modded save slot,\n"
                      "so you can continue your run with mods enabled.",
                 font=FONT_BODY, bg=BG, fg=FG_DIM, justify="left").pack(anchor="w", pady=(0, 10))

        btn = self._button(parent, "Copy current save  →  modded slot",
                           self._transfer_saves)
        btn.pack(anchor="w")
        if not found:
            btn.config(state="disabled")

    def _install_row(self, parent):
        tk.Label(parent, textvariable=self.status_var, font=FONT_BODY,
                 bg=BG, fg=GREEN, wraplength=460).pack(pady=(14, 8))
        self._button(parent, "Install Selected Mods", self._install,
                     primary=True).pack(fill="x", ipady=8)

    def _divider(self, parent):
        tk.Frame(parent, bg=BORDER, height=1).pack(fill="x", pady=4)

    def _button(self, parent, text, command, primary=False, small=False) -> tk.Button:
        bg_color = ACCENT if primary else BG3
        hover    = "#a33a22" if primary else "#303055"
        font     = ("Segoe UI", 10, "bold") if primary else \
                   (FONT_SMALL if small else FONT_BODY)
        b = tk.Button(
            parent, text=text, command=command, font=font,
            bg=bg_color, fg=FG,
            activebackground=hover, activeforeground=FG,
            relief="flat", cursor="hand2",
            padx=16 if not small else 10,
            pady=5  if not small else 3,
        )
        b.bind("<Enter>", lambda _: b.config(bg=hover))
        b.bind("<Leave>", lambda _: b.config(bg=bg_color))
        return b

    # ── Event handlers ─────────────────────────────────────────────────────────
    def _browse_mods_folder(self):
        path = filedialog.askdirectory(title="Select Slay the Spire 2 mods folder")
        if path:
            self.mods_folder_var.set(path)

    def _install(self):
        mods_path = pathlib.Path(self.mods_folder_var.get().strip())
        if not mods_path.parts:
            messagebox.showwarning("No folder set",
                "Please set or browse to your game's mods folder first.")
            return

        selected = [mid for mid, var in self.checkboxes.items() if var.get()]
        if not selected:
            messagebox.showwarning("Nothing selected",
                "Select at least one mod to install.")
            return

        try:
            mods_path.mkdir(parents=True, exist_ok=True)
            install_mods(selected, self.mods, mods_path)
            names = [m["name"] for m in self.mods if m["id"] in selected]
            self.status_var.set("✓ Installed: " + ", ".join(names))
        except Exception as e:
            messagebox.showerror("Install failed", str(e))

    def _transfer_saves(self):
        if not self.save_folder:
            return
        if not messagebox.askyesno(
            "Transfer save?",
            "This will copy your current (unmodded) save files into the modded "
            "save slot.\n\nAny existing modded save data will be overwritten.\n\n"
            "Continue?"
        ):
            return
        try:
            transfer_saves(self.save_folder)
            messagebox.showinfo("Done",
                "Save transferred to modded slot.\n\n"
                "Your existing save should now be available when playing with mods.")
        except Exception as e:
            messagebox.showerror("Transfer failed", str(e))


if __name__ == "__main__":
    app = InstallerApp()
    app.mainloop()
