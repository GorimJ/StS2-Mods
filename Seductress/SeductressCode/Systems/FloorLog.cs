using System.Collections.Generic;
using System.Text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rooms;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// One log line per floor, so a played run can be reviewed as data rather than remembered as an
/// impression. The slices exist to settle numbers that cannot be settled at a desk, and this is how.
///
/// The line is emitted when a floor is *left*, so entry and exit arousal are both known.
///
/// [Claude-Code] Extended for slice 4 with the things that slice makes uncertain: <b>Skin at every
/// rest site</b> and what Indulge recovered (removing HEAL is the most uncertain change in the
/// slice, and how far Skin offsets it is not derivable at a desk), plus pregnancy state, Milk
/// generated, the Ass high-water mark and which thresholds have fired.
/// </summary>
public class FloorRecord
{
    public int Floor;
    public RoomType Room;
    public int ArousalOnEntry;
    public int MaxHpOnEntry;
    public int IntakeGained;
    public int LeakLost;
    public int LeakToSkin;
    public bool ReliefTaken;
    public int IndulgeSkin;
    public int IndulgeRecovered;
    public int BirthStage;
    public int BirthWasted;
    public int MilkGenerated;
    public int ClimaxesOnEntry;
}

public static class FloorLog
{
    private static FloorRecord? _current;

    /// <summary>The floor being recorded right now, or null between floors. Read by SpireLinkExport.</summary>
    public static FloorRecord? Current => _current;

    /// <summary>Total climaxes this run, across combats. Distinct from ClimaxModel's per-combat count.</summary>
    public static int ClimaxesThisRun { get; private set; }

    /// <summary>Total Reliefs taken this run — the other half of the death-clock picture.</summary>
    public static int ReliefsThisRun { get; private set; }

    public static void RecordClimax() => ClimaxesThisRun++;

    public static void RecordIntake(int ml)
    {
        if (_current != null) _current.IntakeGained += ml;
    }

    public static void RecordLeak(int ml, int toSkin)
    {
        if (_current is null) return;
        _current.LeakLost += ml;
        _current.LeakToSkin += toSkin;
    }

    public static void RecordRelief()
    {
        ReliefsThisRun++;
        if (_current != null) _current.ReliefTaken = true;
    }

    public static void RecordIndulge(int skin, int recovered)
    {
        if (_current is null) return;
        _current.IndulgeSkin += skin;
        _current.IndulgeRecovered += recovered;
    }

    public static void RecordBirth(int stage, int wasted)
    {
        if (_current is null) return;
        _current.BirthStage = stage;
        _current.BirthWasted = wasted;
    }

    public static void RecordMilk(int count)
    {
        if (_current != null) _current.MilkGenerated += count;
    }

    public static void Reset()
    {
        _current = null;
        ClimaxesThisRun = 0;
        ReliefsThisRun = 0;
    }

    /// <summary>Opens a new floor record. Call <see cref="EndFloor"/> first.</summary>
    public static void BeginFloor(int floor, RoomType room, Player player)
    {
        _current = new FloorRecord
        {
            Floor = floor,
            Room = room,
            ArousalOnEntry = Arousal(player),
            MaxHpOnEntry = player.Creature.MaxHp,
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
        sb.Append($"reliefs {ReliefsThisRun} | ");
        sb.Append($"mouth {r.Mouth}+{r.Precum}p ass {r.Ass} womb {r.Womb} skin {r.Skin} | ");
        sb.Append($"stomach {r.Stomach} (maxhp {_current.MaxHpOnEntry}->{player.Creature.MaxHp}) | ");
        sb.Append($"sigils {r.Sigils} | ");

        sb.Append(r.PregnancyStage > 0
            ? $"pregnant S{r.PregnancyStage} (locked {r.WombLocked}, leakable {Pregnancy.LeakableWomb(r)}) | "
            : "pregnant no | ");
        if (r.LactatingStage > 0) sb.Append($"lactating S{r.LactatingStage}{(r.LactationTail ? " TAIL" : "")} | ");
        if (_current.MilkGenerated > 0) sb.Append($"milk +{_current.MilkGenerated} | ");
        if (_current.BirthStage > 0) sb.Append($"BIRTH S{_current.BirthStage} wasted {_current.BirthWasted} | ");

        sb.Append($"assHW {r.AssHighWater} [{Unlocks(r)}] | ");
        sb.Append($"intake +{_current.IntakeGained} | leak -{_current.LeakLost} (+{_current.LeakToSkin} skin) | ");
        sb.Append($"relief {(_current.ReliefTaken ? "TAKEN" : "no")}");
        if (_current.IndulgeSkin > 0)
        {
            sb.Append($" | INDULGE {_current.IndulgeSkin} skin -> {_current.IndulgeRecovered} healed+banked");
        }

        MainFile.Logger.Info("[Seductress] " + sb);
        _current = null;
    }

    /// <summary>Which Ass rungs have fired, so a log line says what the run has actually unlocked.</summary>
    private static string Unlocks(ReservoirState r)
    {
        var hit = new List<string>();
        if (r.AssHighWater >= AssUnlocks.MilkingGripAt) hit.Add($"{AssUnlocks.MilkingGripAt}:grip");
        if (r.AssHighWater >= AssUnlocks.PlugAt) hit.Add($"{AssUnlocks.PlugAt}:plug");
        if (r.AssHighWater >= AssUnlocks.OpenRungAt) hit.Add($"{AssUnlocks.OpenRungAt}:-");
        return hit.Count == 0 ? "none" : string.Join(",", hit);
    }
}
