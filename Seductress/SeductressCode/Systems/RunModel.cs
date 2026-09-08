using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Rooms;
using Seductress.SeductressCode.RestSite;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// Run-scoped hooks: the per-floor leak, the rest-site options, the end-of-combat swallow and
/// conception, and the per-floor log line.
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

    // ---------------------------------------------------------------- rest site

    /// <summary>
    /// [Claude-Cowork] <b>The Seductress has no HEAL.</b> Her rest site is Indulge, Relief and
    /// Smith — three different things rather than a heal plus two alternatives. Removing the heal
    /// is what makes Climax her most reliable recovery, which pushes the character toward the loop
    /// she is built on.
    ///
    /// Birth is added on top as a free action rather than as a fourth competing choice.
    /// </summary>
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (!IsSeductress(player)) return false;

        foreach (var heal in options.OfType<HealRestSiteOption>().ToList())
        {
            options.Remove(heal);
        }

        options.Add(new IndulgeRestSiteOption(player));
        options.Add(new ReliefRestSiteOption(player));
        if (Pregnancy.IsPregnant(Reservoirs.Of(player.RunState)))
        {
            options.Add(new GiveBirthRestSiteOption(player));
        }
        return true;
    }

    // ---------------------------------------------------------------- end of combat

    /// <summary>
    /// The bell. Three things resolve here, in this order:
    ///
    /// 1. <b>The swallow.</b> The Mouth banks its cum to the Stomach and its precum evaporates.
    ///    [Gorim] Both are the default behaviour of a swallow, not card-enabled.
    /// 2. <b>Conception.</b> Checked on the end-of-combat Womb value, so spending it back down
    ///    before the bell prevents conception — intended agency (design §6.3b).
    /// 3. <b>The lactation tail</b> ends, unless step 2 just re-entered pregnancy.
    ///
    /// Swallow first so a Mouth build's banking is never contingent on what pregnancy does, and
    /// conception before the tail so conceiving during the tail keeps lactation running.
    /// </summary>
    public override async Task AfterCombatEnd(CombatRoom room)
    {
        var me = LocalContext.GetMe(MegaCrit.Sts2.Core.Runs.RunManager.Instance.DebugOnlyGetState());
        if (!IsSeductress(me)) return;

        await Stomach.SwallowAll(me!, "End of combat");
        Pregnancy.ResolveEndOfCombat(me!);
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

        // The run totals are static, so without this the second run of a session inherits the
        // first one's climax and relief counts and every log line reads wrong.
        if (isFirstFloorOfRun) FloorLog.Reset();

        // Order matters. Close the old floor first so its line shows the reservoirs as the player
        // left them; then open the new floor; then leak, so the loss is charged to the floor you
        // are arriving on rather than the one you just finished.
        FloorLog.EndFloor(me);
        FloorLog.BeginFloor(floor, room.RoomType, me);
        if (!isFirstFloorOfRun) ApplyLeak(me);
        return AssUnlocks.Check(me);
    }

    /// <summary>
    /// Design §6.3c. Womb 10% of the <b>unlocked</b> excess, Ass banded at 5% of the first 200 plus
    /// 25% of what sits above it, Mouth 0%.
    ///
    /// [Claude-Cowork] <b>Leak no longer vanishes — it lands on your Skin, at 75%.</b> Cum leaving
    /// the Womb or Ass loses a quarter en route. Output that lands on Skin deliberately (Freehand
    /// kills, unrouted damage) arrives whole; only accidental loss is taxed, or Freehand would be
    /// penalised for doing its job on purpose.
    ///
    /// [Claude-Cowork] <b>Skin itself does not leak.</b> The old 25%/floor is cut — it is a pure
    /// accumulator now, emptied only by Indulging.
    ///
    /// Leak rounds <b>down</b>, the opposite of intake's round-up, and deliberately so: both round
    /// in the player's favour. A consequence worth knowing is that anything below 10 ml in the Womb
    /// never decays at all.
    /// </summary>
    private static void ApplyLeak(Player player)
    {
        var r = Reservoirs.Of(player.RunState);
        var leaked = 0;

        // Womb: only the excess above the locked pregnancy threshold can bleed away.
        var leakable = Pregnancy.LeakableWomb(r);
        var wombRate = player.Relics.Any(x => x is Relics.StickyBandaid) ? 0.05 : 0.10;
        var wombLost = (int)Math.Floor(leakable * wombRate);
        r.Womb -= wombLost;
        leaked += wombLost;

        // Ass: banded. 5% of the first 200 plus 25% of the excess — at 300 ml that is 35, not 75.
        var excess = Math.Max(0, r.Ass - ReservoirState.AssCap);
        var band = Math.Min(r.Ass, ReservoirState.AssCap);
        var assLost = (int)Math.Floor(band * 0.05) + (int)Math.Floor(excess * 0.25);
        r.Ass -= assLost;
        leaked += assLost;

        // Mouth does not leak between floors; it empties at the end of each combat instead.

        // 75% of what leaked reaches the Skin. The missing quarter is the routing tax.
        var toSkin = (int)Math.Floor(leaked * 0.75);
        r.Skin += toSkin;

        if (leaked > 0)
        {
            FloorLog.RecordLeak(leaked, toSkin);
            MainFile.Logger.Info(
                $"[Seductress] Leak -{leaked} ml (womb -{wombLost}, ass -{assLost}) -> skin +{toSkin} (75%), " +
                $"lost {leaked - toSkin} | womb {r.Womb} ass {r.Ass} skin {r.Skin}");
        }
    }

    // ---------------------------------------------------------------- run lifecycle

    public override Task AfterActEntered()
    {
        MainFile.Logger.Info("[Seductress] --- act entered ---");
        return Task.CompletedTask;
    }
}
