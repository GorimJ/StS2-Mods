using System.Text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rooms;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// One log line per floor, so a played run can be reviewed as data rather than remembered as an
/// impression. Slice 2 exists to settle numbers that cannot be settled at a desk, and this is how.
///
/// The line is emitted when a floor is *left*, so entry and exit arousal are both known.
/// </summary>
public class FloorRecord
{
    public int Floor;
    public RoomType Room;
    public int ArousalOnEntry;
    public int IntakeGained;
    public int LeakLost;
    public bool ReliefTaken;
    public int ClimaxesOnEntry;
}

public static class FloorLog
{
    private static FloorRecord? _current;

    /// <summary>Total climaxes this run, across combats. Distinct from ClimaxModel's per-combat count.</summary>
    public static int ClimaxesThisRun { get; private set; }

    public static void RecordClimax() => ClimaxesThisRun++;

    public static void RecordIntake(int ml)
    {
        if (_current != null) _current.IntakeGained += ml;
    }

    public static void RecordLeak(int ml)
    {
        if (_current != null) _current.LeakLost += ml;
    }

    public static void RecordRelief()
    {
        if (_current != null) _current.ReliefTaken = true;
    }

    public static void Reset() => _current = null;

    /// <summary>Opens a new floor record. Call <see cref="EndFloor"/> first.</summary>
    public static void BeginFloor(int floor, RoomType room, Player player)
    {
        _current = new FloorRecord
        {
            Floor = floor,
            Room = room,
            ArousalOnEntry = Arousal(player),
            ClimaxesOnEntry = ClimaxesThisRun
        };
    }

    private static int Arousal(Player player) => player.Creature.MaxHp - player.Creature.CurrentHp;

    /// <summary>
    /// Prints the floor we are leaving, and closes it. Safe to call with nothing open.
    ///
    /// Must be called before the next floor's leak is applied, or the snapshot shows reservoir
    /// levels the player never actually left the floor with.
    /// </summary>
    public static void EndFloor(Player player)
    {
        if (_current is null) return;
        var r = Reservoirs.Of(player.RunState);

        var sb = new StringBuilder();
        sb.Append($"FLOOR {_current.Floor} {_current.Room} | ");
        sb.Append($"arousal {_current.ArousalOnEntry}->{Arousal(player)}/{player.Creature.MaxHp} | ");
        sb.Append($"climaxes {ClimaxesThisRun} (+{ClimaxesThisRun - _current.ClimaxesOnEntry}) | ");
        sb.Append($"lust {ClimaxModel.LustCount(player)}/{ClimaxModel.LustLimit} | ");
        sb.Append($"mouth {r.Mouth} ass {r.Ass} womb {r.Womb} glazed {r.Glazed} | ");
        sb.Append($"sigils {r.Sigils} | ");
        sb.Append($"intake +{_current.IntakeGained} | leak -{_current.LeakLost} | ");
        sb.Append($"relief {(_current.ReliefTaken ? "TAKEN" : "no")}");

        MainFile.Logger.Info("[Seductress] " + sb);
        _current = null;
    }
}
