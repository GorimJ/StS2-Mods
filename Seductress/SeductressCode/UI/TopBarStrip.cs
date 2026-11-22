using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Runs;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.UI;

/// <summary>
/// The run-long counters, in the top bar so they show on the map, in shops, at rest sites and in
/// events as well as in combat. [Gorim, 2026-11-21] chose "Yes, in the top bar" when asked
/// whether the readouts should be visible outside combat.
///
/// Two lines of text in the gap between the floor icons and the right-hand buttons. Hovering
/// shows a pop-out; clicking opens <see cref="MilestonesScreen"/>.
///
/// [Claude] Which counters, their order, and the position are mine. The Mouth is left out
/// because it empties at the end of every combat; it is under its picture in combat.
/// </summary>
public partial class TopBarStrip : Control
{
    private const float Left = 940f;
    private const float Top = 14f;
    private const float Width = 340f;
    private const float Height = 54f;

    private static readonly Color Ink = new(1f, 0.92f, 0.96f);
    private static readonly Color Hot = new(1f, 0.82f, 0.48f);

    private Player? _player;
    private Label? _line1, _line2;

    public void Initialize(Player player) => _player = player;

    public override void _Ready()
    {
        Position = new Vector2(Left, Top);
        Size = new Vector2(Width, Height);
        MouseFilter = MouseFilterEnum.Stop;
        MouseDefaultCursorShape = CursorShape.PointingHand;

        _line1 = Line(0f);
        _line2 = Line(26f);

        MouseEntered += () =>
        {
            Tint(Hot);
            NHoverTipSet.CreateAndShow(this, Glossary.Tip("MILESTONES"))
                ?.SetGlobalPosition(GlobalPosition + new Vector2(0f, Size.Y + 24f));
        };
        MouseExited += () =>
        {
            Tint(Ink);
            NHoverTipSet.Remove(this);
        };
        GuiInput += e =>
        {
            if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }) OpenMilestones();
        };
        Current = this;
    }

    /// <summary>The strip of the run in progress, so the <c>milestones</c> console command can open the screen.</summary>
    public static TopBarStrip? Current { get; private set; }

    public override void _ExitTree()
    {
        if (Current == this) Current = null;
    }

    public bool OpenMilestones(ReservoirState? demo = null)
    {
        if (_player is null || GetChildren().OfType<MilestonesScreen>().Any()) return false;
        NHoverTipSet.Remove(this);
        var screen = new MilestonesScreen();
        screen.Initialize(_player, demo);
        AddChild(screen);
        return true;
    }

    private Label Line(float y)
    {
        var label = new Label
        {
            Position = new Vector2(0f, y),
            Size = new Vector2(Width, 24f),
            MouseFilter = MouseFilterEnum.Ignore,
        };
        label.AddThemeFontSizeOverride("font_size", 18);
        label.AddThemeColorOverride("font_color", Ink);
        label.AddThemeColorOverride("font_shadow_color", new Color(0, 0, 0, 0.9f));
        label.AddThemeConstantOverride("shadow_offset_x", 1);
        label.AddThemeConstantOverride("shadow_offset_y", 2);
        AddChild(label);
        return label;
    }

    private void Tint(Color colour)
    {
        _line1?.AddThemeColorOverride("font_color", colour);
        _line2?.AddThemeColorOverride("font_color", colour);
    }

    public override void _Process(double delta)
    {
        if (_player is null || _line1 is null || _line2 is null) return;
        var r = Reservoirs.Of(_player.RunState);

        var usable = Sigils.Effective(_player);
        var sigils = usable == r.Sigils ? $"{r.Sigils}" : $"{usable}/{r.Sigils}";
        _line1.Text = $"Stomach {r.Stomach}    Skin {r.Skin}    Sigils {sigils}";
        _line2.Text = $"Womb {r.Womb}{(r.PregnancyStage > 0 ? $" (stage {r.PregnancyStage})" : "")}" +
                      $"    Ass {r.Ass}{(r.PlugIn ? " (plugged)" : "")}" +
                      $"    Lust {ClimaxModel.LustCount(_player)}/{ClimaxModel.LustLimit}";
    }
}

/// <summary>The top bar is made once per run (or load), so this adds the strip once per run.</summary>
[HarmonyPatch(typeof(NTopBar), nameof(NTopBar.Initialize))]
internal static class TopBarStripPatch
{
    [HarmonyPostfix]
    private static void AddStrip(NTopBar __instance, IRunState runState)
    {
        foreach (var old in __instance.GetChildren().OfType<TopBarStrip>()) old.QueueFree();

        var me = LocalContext.GetMe(runState.Players);
        if (me?.Character is not Character.Seductress) return;

        var strip = new TopBarStrip();
        strip.Initialize(me);
        __instance.AddChild(strip);
    }
}
