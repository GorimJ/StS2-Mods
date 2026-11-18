using System;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.UI;

/// <summary>
/// [Claude, 2026-11-18] Shows the right belly on the combat rig.
///
/// The rig (seductress_visuals.tscn) carries six hidden sprites,
/// <c>Visuals/Parts/BELLY_overlay/stage1</c> .. <c>stage6</c>. Band N shows stageN alone; band 0
/// hides all six and leaves the flat belly of the base art.
///
/// [Gorim, 2026-11-18] One stage per band, cap 6:
/// <code>
/// pregnancy = min(stage, 3)
/// inflation = min(floor((Ass - 200) / 100), 3), not below 0
/// band      = min(pregnancy + inflation, 6)
/// </code>
///
/// Driven from <see cref="ReservoirHud"/>'s per-frame refresh rather than from hooks on every
/// place the Womb or Ass can change: the HUD is created once per combat, which is also how often
/// the rig scene is re-instanced, and the check is two integer reads when nothing has changed.
/// </summary>
public sealed class BellyVisuals
{
    public const int MaxBand = 6;
    private const string OverlayPath = "Parts/BELLY_overlay";

    private readonly Player _player;
    private Node? _overlay;
    private int _shown = -1;
    private bool _warned;

    public BellyVisuals(Player player) => _player = player;

    public static int Band(ReservoirState r)
    {
        var pregnancy = Math.Min(r.PregnancyStage, 3);
        var inflation = Math.Clamp((r.Ass - 200) / 100, 0, 3);
        return Math.Min(pregnancy + inflation, MaxBand);
    }

    public void Refresh()
    {
        if (_overlay is null || !GodotObject.IsInstanceValid(_overlay))
        {
            _overlay = FindOverlay();
            _shown = -1;
            if (_overlay is null) return;
        }

        var band = Band(Reservoirs.Of(_player.RunState));
        if (band == _shown) return;

        for (var i = 1; i <= MaxBand; i++)
        {
            if (_overlay.GetNodeOrNull<CanvasItem>($"stage{i}") is { } stage) stage.Visible = i == band;
        }

        MainFile.Logger.Info($"[Seductress] Belly band {_shown} -> {band}");
        _shown = band;
    }

    private Node? FindOverlay()
    {
        var body = NCombatRoom.Instance?.GetCreatureNode(_player.Creature)?.Visuals?.Body;
        if (body is null) return null; // the creature node can appear a frame or two after the HUD

        var overlay = body.GetNodeOrNull(OverlayPath);
        if (overlay is null && !_warned)
        {
            _warned = true;
            MainFile.Logger.Info($"[Seductress] Belly overlay not found at {OverlayPath}; belly stays flat");
        }
        return overlay;
    }
}
