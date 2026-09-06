using System.Runtime.CompilerServices;
using BaseLib.Patches.Saves;
using MegaCrit.Sts2.Core.Runs;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// Spike question 4: do custom resources persist?
///
/// The four ml containers plus the Sigil count, held per run and persisted through BaseLib's
/// <see cref="ExtendedSaveTypes"/>. Registration targets <c>IRunState</c>, which BaseLib patches
/// on <c>RunManager.ToSave</c> / <c>RunState.FromSerializable</c>, so these ride along with the
/// ordinary run save rather than needing a file of our own.
/// </summary>
public class ReservoirState
{
    /// Mouth: 50 ml cap, no leak. Empties every combat (cum banks to Stomach, precum evaporates).
    public int Mouth;
    /// Ass: 200 ml cap, both hole and container.
    public int Ass;
    /// Womb: 100 ml cap, filled by the Pussy tag. 100 ml is a pregnancy.
    public int Womb;
    /// Glazed: external, uncapped, the catch-all.
    public int Glazed;
    /// Stomach: Draining's permanent track. Unbounded.
    public int Stomach;
    /// One per pregnancy, kept for the run.
    public int Sigils;

    public int Capacity(Hole hole) => hole switch
    {
        Hole.Mouth => 50,
        Hole.Ass => 200,
        Hole.Pussy => 100,
        _ => int.MaxValue
    };

    public int Get(Hole hole) => hole switch
    {
        Hole.Mouth => Mouth,
        Hole.Ass => Ass,
        Hole.Pussy => Womb,
        _ => Glazed
    };

    /// <summary>Routes <paramref name="ml"/> into the container the hole fills, capped.</summary>
    public void Add(Hole hole, int ml)
    {
        var cap = Capacity(hole);
        switch (hole)
        {
            case Hole.Mouth: Mouth = System.Math.Min(Mouth + ml, cap); break;
            case Hole.Ass: Ass = System.Math.Min(Ass + ml, cap); break;
            case Hole.Pussy: Womb = System.Math.Min(Womb + ml, cap); break;
            default: Glazed += ml; break;
        }
    }
}

public static class Reservoirs
{
    private static readonly ConditionalWeakTable<object, ReservoirState> States = new();

    /// <summary>Per-run state. Keyed off the run itself so a new run starts empty.</summary>
    public static ReservoirState Of(IRunState? runState)
    {
        if (runState is null) return Fallback;
        return States.GetValue(runState, _ => new ReservoirState());
    }

    /// Used before a run exists (menus, early hooks) so callers never have to null-check.
    private static readonly ReservoirState Fallback = new();

    /// <summary>
    /// Registers each field as an extended saved value on the run. Called once at mod init.
    /// </summary>
    public static void RegisterSaves()
    {
        Register("Seductress.Mouth", s => s.Mouth, (s, v) => s.Mouth = v);
        Register("Seductress.Ass", s => s.Ass, (s, v) => s.Ass = v);
        Register("Seductress.Womb", s => s.Womb, (s, v) => s.Womb = v);
        Register("Seductress.Glazed", s => s.Glazed, (s, v) => s.Glazed = v);
        Register("Seductress.Stomach", s => s.Stomach, (s, v) => s.Stomach = v);
        Register("Seductress.Sigils", s => s.Sigils, (s, v) => s.Sigils = v);
    }

    private static void Register(string id, System.Func<ReservoirState, int> get, System.Action<ReservoirState, int> set)
    {
        ExtendedSaveTypes.RegisterSavedValue<IRunState, int>(
            id,
            runState => get(Of(runState)),
            (runState, value) => set(Of(runState), value),
            (value, writer) => writer.WriteInt(value),
            reader => reader.ReadInt());
    }
}
