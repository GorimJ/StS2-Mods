using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Seductress.SeductressCode.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.UI;

/// <summary>
/// Spike question 4, display half: is there a slot on screen for five numbers?
///
/// There is no vanilla HUD affordance for custom resources — BaseLib's
/// <c>BasicResourceVisualsHandler.AddDisplay</c> is an empty method, and its resource system is
/// combat-scoped anyway, while the reservoirs are run-scoped. So this does what the Buxom port
/// did: parents a plain Godot <see cref="Control"/> onto <see cref="NCombatUi"/> and draws into
/// the empty gutter left of the energy orb. Crude on purpose — the question is whether a slot
/// exists, not what it should look like.
/// </summary>
public partial class ReservoirHud : Control
{
    private const float X = 26f;
    private const float Y = 690f;
    private const float LineHeight = 17f;
    private const float Width = 190f;

    private static readonly Color Ink = new(1f, 0.92f, 0.96f);
    private static readonly Color Panel = new(0.10f, 0.06f, 0.10f, 0.72f);

    private readonly Label[] _lines = new Label[7];
    private Player? _player;

    public void Initialize(Player player) => _player = player;

    public override void _Ready()
    {
        var bg = new ColorRect
        {
            Position = new Vector2(X - 6f, Y - 6f),
            Size = new Vector2(Width, LineHeight * _lines.Length + 12f),
            Color = Panel
        };
        AddChild(bg);

        for (var i = 0; i < _lines.Length; i++)
        {
            var label = new Label
            {
                Position = new Vector2(X, Y + i * LineHeight),
                Size = new Vector2(Width - 12f, LineHeight)
            };
            label.AddThemeFontSizeOverride("font_size", 12);
            label.AddThemeColorOverride("font_color", Ink);
            label.AddThemeColorOverride("font_shadow_color", new Color(0, 0, 0, 0.9f));
            label.AddThemeConstantOverride("shadow_offset_x", 1);
            label.AddThemeConstantOverride("shadow_offset_y", 1);
            AddChild(label);
            _lines[i] = label;
        }

        Refresh();
    }

    public override void _Process(double delta) => Refresh();

    private void Refresh()
    {
        if (_player is null || _lines[0] is null) return;

        var r = Reservoirs.Of(_player.RunState);
        var climax = ClimaxModel.Instance;
        var sensitive = _player.Creature.GetPowerAmount<SensitivePower>();

        _lines[0].Text = $"Mouth   {r.Mouth} / 50 ml";
        _lines[1].Text = $"Ass     {r.Ass} / 200 ml";
        _lines[2].Text = $"Womb    {r.Womb} / 100 ml";
        _lines[3].Text = $"Glazed  {r.Glazed} ml";
        var exhausted = climax.SigilsExhaustedThisCombat;
        var sigils = exhausted > 0
            ? $"Sigils {Sigils.Effective(_player)}/{r.Sigils} (-{exhausted} exhausted)"
            : $"Sigils {r.Sigils}";
        _lines[4].Text = $"Stomach {r.Stomach} ml   {sigils}";
        _lines[5].Text = $"Continuing: {climax.ContinuingHole}";
        _lines[6].Text = $"Climaxes {climax.ClimaxesThisCombat}  Sensitive {sensitive}  " +
                         $"Lust {ClimaxModel.LustCount(_player)}/{ClimaxModel.LustLimit}";
    }
}

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal static class ReservoirHudPatch
{
    [HarmonyPostfix]
    private static void AddHud(NCombatUi __instance, CombatState state)
    {
        var me = LocalContext.GetMe(state);
        if (me?.Character is not Character.Seductress) return;

        var hud = new ReservoirHud();
        hud.Initialize(me);
        __instance.AddChild(hud);
    }
}

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Deactivate))]
internal static class ReservoirHudCleanupPatch
{
    [HarmonyPostfix]
    private static void RemoveHud(NCombatUi __instance)
    {
        foreach (var child in __instance.GetChildren())
        {
            if (child is ReservoirHud hud) hud.QueueFree();
        }
    }
}
