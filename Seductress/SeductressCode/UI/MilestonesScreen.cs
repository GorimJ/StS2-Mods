using System;
using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.UI;

/// <summary>
/// [Gorim, 2026-11-21] "We need to build in the milestone system so I can see this stuff in
/// game." Asked where, he chose one milestones screen over pop-outs or always-visible tracks;
/// asked about an infographic, he chose bar tracks for now (<see cref="MilestoneTrack"/>).
///
/// A panel over the game with one track per ladder: Stomach, Womb, Ass, Sigils. Opened from
/// <see cref="TopBarStrip"/> or the <c>milestones</c> console command; closed by clicking
/// anywhere or pressing Escape.
///
/// [Claude] Layout, colours and wording are mine. Every figure is read from the rules classes,
/// so it cannot drift from what the game does.
/// </summary>
public partial class MilestonesScreen : CanvasLayer
{
    private Player? _player;

    /// <summary>[Claude] Made-up amounts to show instead of the run's, for looking at the screen without touching a run.</summary>
    private ReservoirState? _demo;

    public void Initialize(Player player, ReservoirState? demo = null)
    {
        _player = player;
        _demo = demo;
    }

    public override void _Ready()
    {
        Layer = 60;

        var dim = new ColorRect { Color = new Color(0f, 0f, 0f, 0.72f), MouseFilter = Control.MouseFilterEnum.Stop };
        dim.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        dim.GuiInput += e =>
        {
            if (e is InputEventMouseButton { Pressed: true }) QueueFree();
        };
        AddChild(dim);

        var center = new CenterContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        center.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        AddChild(center);

        var panel = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        var box = new StyleBoxFlat
        {
            BgColor = new Color(0.11f, 0.07f, 0.11f, 0.97f),
            BorderColor = new Color(0.85f, 0.55f, 0.70f),
        };
        box.SetBorderWidthAll(2);
        box.SetCornerRadiusAll(10);
        box.SetContentMarginAll(26);
        panel.AddThemeStyleboxOverride("panel", box);
        center.AddChild(panel);

        var rows = new VBoxContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        rows.AddThemeConstantOverride("separation", 14);
        panel.AddChild(rows);

        if (_player is null) return;
        var r = _demo ?? Reservoirs.Of(_player.RunState);

        // Stomach
        var hp = Stomach.MaxHpEarned(r.Stomach);
        rows.AddChild(Track("Stomach",
            $"{r.Stomach} ml banked from your Mouth.  +{hp} Max HP so far (1 per {Stomach.MlPerMaxHp} ml).",
            r.Stomach, 1750,
            [
                Rung(250, "+25 ml precum\neach combat", r.Stomach),
                Rung(500, "Gulp: full Mouth\nswallows, keeps filling", r.Stomach),
                Rung(750, "+50 ml precum\neach combat", r.Stomach),
                Rung(1000, "Indulge recovers\nall your Skin;\nswallow over 125 ml:\n-1 Sensitive", r.Stomach),
                Rung(1250, "+75 ml precum\neach combat", r.Stomach),
                Rung(1500, "precum is banked\nlike cum", r.Stomach),
                Rung(1750, "+100 ml precum\neach combat", r.Stomach),
            ]));

        // Womb: one rung per stage, always showing at least one stage beyond where you are.
        var stage = r.PregnancyStage;
        var stages = Math.Max(3, Math.Max(stage, Pregnancy.StageFor(r.Womb)) + 1);
        var wombRungs = new List<MilestoneTrack.Rung>();
        for (var s = 1; s <= stages; s++)
        {
            var at = s * ReservoirState.WombStage;
            var waiting = stage < s && r.Womb >= at;
            wombRungs.Add(new MilestoneTrack.Rung(at,
                $"Stage {s}" + (waiting ? "\ntakes hold after a combat" : ""), stage >= s));
        }
        rows.AddChild(Track("Womb",
            $"{r.Womb} ml" + (stage > 0 ? $", pregnant at stage {stage}" : "") +
            ".  Per stage: 1 Sensitive each combat, 1 Milk each turn, 1 Sigil at birth." +
            (r.LactatingStage > 0 ? $"  Lactating at stage {r.LactatingStage}." : ""),
            r.Womb, stages * ReservoirState.WombStage, wombRungs));

        // Ass: the bar is what it holds now; the rungs go by the fullest it has been (gold line).
        rows.AddChild(Track("Ass",
            $"{r.Ass} ml now" + (r.PlugIn ? ", plugged" : "") +
            $".  Rewards go by the fullest it has been this run: {r.AssHighWater} ml (gold line).",
            r.Ass, AssUnlocks.OpenRungAt,
            [
                Rung(AssUnlocks.MilkingGripAt, "Milking Grip\nadded to your deck", r.AssHighWater),
                Rung(AssUnlocks.PlugAt, "the Plug relic", r.AssHighWater),
                Rung(AssUnlocks.OpenRungAt, "nothing yet", r.AssHighWater),
            ], r.AssHighWater));

        // Sigils
        var owned = r.Sigils;
        var usable = _demo is null ? Sigils.Effective(_player) : Math.Max(0, owned - 1);
        var sigilRow = new SigilTrack();
        sigilRow.Setup(
            (usable == owned ? $"{owned}" : $"{usable} of {owned} usable (rest exhausted this combat)") +
            ".  Earned by giving birth. Every Sigil card follows this ladder.",
            usable, owned,
            [
                Rung(Sigils.Tier1, "Sigil cards\nadd an effect", usable),
                Rung(Sigils.Tier2, "their base\nnumber grows", usable),
                Rung(Sigils.Tier3, "the added\neffect grows", usable),
                Rung(Sigils.Tier4, "they repeat, once\nper energy they cost", usable),
            ]);
        rows.AddChild(sigilRow);

        var lust = ClimaxModel.LustCount(_player);
        var foot = new Label
        {
            Text = $"Skin {r.Skin} ml: Indulge at a rest site to scoop it up and eat it.     " +
                   $"Lust {lust} of {ClimaxModel.LustLimit}: each Climax adds a Lust card; at {ClimaxModel.LustLimit}, the next Climax ends the run.\n" +
                   "Click anywhere or press Escape to close.",
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        foot.AddThemeFontSizeOverride("font_size", 16);
        foot.AddThemeColorOverride("font_color", new Color(0.80f, 0.76f, 0.80f));
        rows.AddChild(foot);
    }

    private static MilestoneTrack.Rung Rung(int at, string label, int have) => new(at, label, have >= at);

    private static MilestoneTrack Track(string name, string now, int value, int max,
        IReadOnlyList<MilestoneTrack.Rung> rungs, int marker = -1)
    {
        var track = new MilestoneTrack();
        track.Setup(name, now, value, max, rungs, marker);
        return track;
    }

    public override void _Input(InputEvent e)
    {
        if (!e.IsActionPressed("ui_cancel")) return;
        GetViewport().SetInputAsHandled();
        QueueFree();
    }
}
