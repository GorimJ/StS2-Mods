using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Rooms;
using Seductress.SeductressCode.RestSite;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// Run-scoped hooks: per-floor leak, the rest-site relief option, and the per-floor log line.
///
/// This is a second singleton alongside <see cref="ClimaxModel"/> because BaseLib warns that one
/// model receiving both Combat and Run hook types gets some hooks twice. Combat things live in
/// ClimaxModel; run things live here.
/// </summary>
public class RunModel : CustomSingletonModel
{
    public static RunModel Instance { get; private set; } = null!;

    public RunModel() : base(HookType.Run) => Instance = this;

    private int _lastFloorSeen = -1;

    private static bool IsSeductress(Player? player) => player?.Character is Character.Seductress;

    // ---------------------------------------------------------------- rest site relief

    /// <summary>
    /// Adds Relief — remove 1 Lust instead of healing (design §5).
    ///
    /// No per-act cap by decision: for this character HP <i>is</i> arousal, so giving up the heal
    /// means entering the next fight already aroused, and that opportunity cost scales with how
    /// dangerous the next floor is.
    /// </summary>
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (!IsSeductress(player)) return false;
        options.Add(new ReliefRestSiteOption(player));
        return true;
    }

    // ---------------------------------------------------------------- per-floor leak

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        var me = LocalContext.GetMe(MegaCrit.Sts2.Core.Runs.RunManager.Instance.DebugOnlyGetState());
        if (!IsSeductress(me)) return Task.CompletedTask;

        var floor = me!.RunState.TotalFloor;
        if (floor == _lastFloorSeen) return Task.CompletedTask; // re-entering the same floor
        var isFirstFloorOfRun = _lastFloorSeen < 0;
        _lastFloorSeen = floor;

        // Order matters. Close the old floor first so its line shows the reservoirs as the player
        // left them; then open the new floor; then leak, so the loss is charged to the floor you
        // are arriving on rather than the one you just finished.
        FloorLog.EndFloor(me);
        FloorLog.BeginFloor(floor, room.RoomType, me);
        if (!isFirstFloorOfRun) ApplyLeak(me);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Design §6.2: Mouth 0%, Womb 10% of current, Ass 5% — with 25% on the excess above 200 —
    /// and Glazed 25% (§6.4).
    ///
    /// Leak rounds <b>down</b>, which is the opposite of intake's round-up, and deliberately so:
    /// both round in the player's favour. A consequence worth knowing is that anything below 10 ml
    /// in the Womb never decays at all, so a trickle is sticky.
    /// </summary>
    private static void ApplyLeak(Player player)
    {
        var r = Reservoirs.Of(player.RunState);
        var lost = 0;

        lost += Drain(ref r.Womb, 0.10);
        lost += Drain(ref r.Glazed, 0.25);

        // Ass: 5% of the bank, but the inflated excess above capacity bleeds at 25%.
        const int assCap = 200;
        if (r.Ass > assCap)
        {
            var excess = r.Ass - assCap;
            var excessLost = (int)Math.Floor(excess * 0.25);
            r.Ass -= excessLost;
            lost += excessLost;
        }
        lost += Drain(ref r.Ass, 0.05);

        // Mouth does not leak between floors; it empties at the end of each combat instead.

        if (lost > 0)
        {
            FloorLog.RecordLeak(lost);
            MainFile.Logger.Info($"[Seductress] Leak -{lost} ml (womb {r.Womb} ass {r.Ass} glazed {r.Glazed})");
        }
    }

    private static int Drain(ref int reservoir, double rate)
    {
        var lost = (int)Math.Floor(reservoir * rate);
        reservoir -= lost;
        return lost;
    }

    // ---------------------------------------------------------------- run lifecycle

    public override Task AfterActEntered()
    {
        MainFile.Logger.Info("[Seductress] --- act entered ---");
        return Task.CompletedTask;
    }
}
