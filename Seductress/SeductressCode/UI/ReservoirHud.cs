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
/// The combat-screen host for the character's own UI: the three hole pictures with their
/// readouts (<see cref="HoleInsets"/>) and the rig's belly, breast and milk layers.
///
/// [Gorim, 2026-11-21] The eight-line debug text box that used to live here is replaced: the
/// run-long counters are in the top bar (<see cref="TopBarStrip"/>), the hole numbers are under
/// their pictures, and the ladders are on the milestones screen.
/// </summary>
public partial class ReservoirHud : Control
{
    private Player? _player;
    private RigVisuals? _belly;

    public void Initialize(Player player)
    {
        _player = player;
        _belly = new RigVisuals(player);
    }

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        if (_player is null) return;

        var insets = new HoleInsets();
        insets.Initialize(_player);
        AddChild(insets);
    }

    public override void _Process(double delta) => _belly?.Refresh();
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
