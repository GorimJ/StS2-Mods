using System;
using System.Runtime.CompilerServices;
using BaseLib.Patches.Saves;
using MegaCrit.Sts2.Core.Runs;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// The ml containers, held per run and persisted through BaseLib's <see cref="ExtendedSaveTypes"/>.
/// Registration targets <c>IRunState</c>, which BaseLib patches on <c>RunManager.ToSave</c> /
/// <c>RunState.FromSerializable</c>, so these ride along with the ordinary run save.
///
/// [Claude-Code] The reservoir formerly called <b>Glazed</b> is now <b>Skin</b> throughout (design decision 90), including the save keys.
/// A run saved before this build loses its Skin value on load; nothing else moves.
/// </summary>
public class ReservoirState
{
    /// <summary>
    /// Mouth, <b>cum only</b>. Banks to the Stomach at the end of every combat (design §6.5).
    /// The 250 ml cap is on <see cref="MouthTotal"/>, not on this alone.
    /// </summary>
    public int Mouth;

    /// <summary>
    /// Mouth, <b>precum</b> (design §6.2a). Inside a combat it is indistinguishable from cum —
    /// spendable by anything that empties the Mouth, and counting against the same 250 ml cap. At
    /// the end of combat it evaporates: no heal, no Stomach.
    ///
    /// [Claude-Code] Held as a separate field rather than a flag on the Mouth because the two
    /// differ only at two moments — the spend order takes precum first, and the swallow banks cum
    /// only — and both of those need the split, while everything in between wants the total.
    /// </summary>
    public int Precum;

    /// <summary>
    /// Ass: 1000 ml nominal cap, both hole and container. Inflates above it. Includes
    /// <see cref="AssPrecum"/>.
    ///
    /// [Claude, 2026-11-20] A property so that precum is spent first without every spender having
    /// to know about it: any drop in the bank comes out of the precum share before the cum.
    /// </summary>
    public int Ass
    {
        get => _ass;
        set
        {
            if (value < _ass) AssPrecum = Math.Max(0, AssPrecum - (_ass - value));
            _ass = value;
        }
    }
    private int _ass;

    /// <summary>
    /// [Gorim, 2026-11-20] Precum outside the Mouth, granted by <i>Edging Form</i>. The share of
    /// <see cref="Ass"/> that is precum: usable like cum during the combat, gone at the bell.
    /// [Claude] It never counts toward the high-water mark, so it cannot unlock rewards.
    /// </summary>
    public int AssPrecum;

    /// <summary>
    /// The highest Ass the run has ever held (design §6.1a). Threshold unlocks read this rather
    /// than the live value, because the bank leaks every floor and a live check would grant and
    /// revoke a reward on alternating floors.
    /// </summary>
    public int AssHighWater;

    /// <summary>
    /// Womb: filled by the Pussy tag. 500 ml conceives; stages sit at N x 500. Includes
    /// <see cref="WombPrecum"/>, which is spent first, as with <see cref="Ass"/>.
    /// </summary>
    public int Womb
    {
        get => _womb;
        set
        {
            if (value < _womb) WombPrecum = Math.Max(0, WombPrecum - (_womb - value));
            _womb = value;
        }
    }
    private int _womb;

    /// <summary>The share of <see cref="Womb"/> that is precum. Gone at the bell, before conception.</summary>
    public int WombPrecum;

    /// <summary>
    /// Skin: external catch-all. Uncapped, and <b>does not leak</b> — a pure accumulator that only
    /// empties by Indulging at a rest site (design §6.3c).
    /// </summary>
    public int Skin;

    /// <summary>Stomach: Draining's permanent track. Unbounded. +1 Max HP per 50 ml (§6.5).</summary>
    public int Stomach;

    /// <summary>One per pregnancy stage delivered, kept for the run.</summary>
    public int Sigils;

    /// <summary>Pregnancy stage: 0 not pregnant, N at N x 500 ml committed (design §6.3b).</summary>
    public int PregnancyStage;

    /// <summary>
    /// Lactation stage. Begins with pregnancy and runs <b>one further combat after the birth</b> at
    /// the stage delivered at, so the payoff outlives the fragility that paid for it.
    /// </summary>
    public int LactatingStage;

    /// <summary>Set at a birth so the following combat is the lactation tail, then cleared.</summary>
    public bool LactationTail;

    /// <summary>
    /// Whether the Plug is actually in. Playing an Ass card pulls it; entering a floor puts it
    /// back. Only meaningful while the relic is held.
    /// </summary>
    public bool PlugIn;

    // ---------------------------------------------------------------- Mouth

    /// <summary>Everything in the Mouth right now, cum and precum together. Capped at MouthCap.</summary>
    public int MouthTotal => Mouth + Precum;

    // [Gorim, 2026-11-20] Every ml figure is five times what it was ("for now, and see where
    // that gets us"), because attacks now gain cum 1:1 with the HP they remove.
    //
    // [Gorim, 2026-11-21] After seeing it in play: "Womb seems too hard to fill, lets cut it down
    // to 250ml for now and see how that is. Probably cut the other two down as well by half.
    // Maybe mouth to 100ml actually." So Womb 500 -> 250, Ass 1000 -> 500, Mouth 250 -> 100.
    // [Claude] The Ass unlock rungs and the Plug follow the Ass cap; the Stomach's thresholds
    // and every card's ml amount are unchanged.
    public const int MouthCap = 100;
    public const int WombStage = 250;
    public const int AssCap = 500;

    /// <summary>Nominal capacity. The Ass and Womb both overfill past theirs by design.</summary>
    public int Capacity(Hole hole) => hole switch
    {
        Hole.Mouth => MouthCap,
        Hole.Ass => int.MaxValue,
        Hole.Pussy => int.MaxValue,   // the Womb inflates past 500; stages are thresholds, not caps
        _ => int.MaxValue
    };

    public int Get(Hole hole) => hole switch
    {
        Hole.Mouth => MouthTotal,
        Hole.Ass => Ass,
        Hole.Pussy => Womb,
        _ => Skin
    };

    /// <summary>
    /// Adds real cum to the Mouth, respecting the combined cap. Returns what actually went in.
    ///
    /// [Gorim, 2026-11-23] "Gaining cum when it fills your mouth and you can't gain more cum
    /// should displace any precum that is there." Free room first, then precum is pushed out to
    /// make room. [Claude] Pushed-out precum is simply gone; what still does not fit is the
    /// caller's to splash onto Skin (<see cref="Stomach.SwallowIfFull"/>), after any Gulp.
    /// </summary>
    public int AddMouthCum(int ml)
    {
        var room = Math.Max(0, MouthCap - MouthTotal);
        var added = Math.Min(ml, room);
        var displaced = Math.Min(Precum, ml - added);
        Precum -= displaced;
        added += displaced;
        Mouth += added;
        return added;
    }

    /// <summary>
    /// Adds precum, respecting the same combined cap. Returns what actually fitted.
    /// </summary>
    public int AddPrecum(int ml)
    {
        var room = Math.Max(0, MouthCap - MouthTotal);
        var added = Math.Min(ml, room);
        Precum += added;
        return added;
    }

    /// <summary>
    /// Spends from the Mouth, <b>precum first</b> (design §6.2a). Returns how much was actually
    /// taken, which may be less than asked.
    ///
    /// Precum-first is what makes the Mouth exception free: a card that manufactures precum is
    /// never wasted, and bankable cum survives to the swallow.
    /// </summary>
    public int SpendMouth(int ml)
    {
        var fromPrecum = Math.Min(Precum, ml);
        Precum -= fromPrecum;
        var fromCum = Math.Min(Mouth, ml - fromPrecum);
        Mouth -= fromCum;
        return fromPrecum + fromCum;
    }

    /// <summary>Empties the Mouth entirely. Returns (cum, precum) as they stood.</summary>
    public (int Cum, int Precum) EmptyMouth()
    {
        var cum = Mouth;
        var pre = Precum;
        Mouth = 0;
        Precum = 0;
        return (cum, pre);
    }

    // ---------------------------------------------------------------- routing

    /// <summary>Routes <paramref name="ml"/> into the container the hole fills. Returns what went in.</summary>
    public int Add(Hole hole, int ml)
    {
        switch (hole)
        {
            case Hole.Mouth: return AddMouthCum(ml);
            // The Plug blocks filling as well as leaking (design §7.3).
            case Hole.Ass when PlugIn: return 0;

            // [Claude-Code] Deliberately NOT capped at 200. The old code clamped here, which
            // silently made inflation impossible — and inflation is a stated mechanic (§6.1) that
            // §6.4's banded 25% excess leak exists to price, and that Milking Grip's 400 ml parity
            // point assumes. 200 is the nominal capacity, not a ceiling.
            case Hole.Ass: Ass += ml; TouchAssHighWater(); break;
            case Hole.Pussy: Womb += ml; break;
            default: Skin += ml; break;
        }
        return ml;
    }

    /// <summary>
    /// Records a new Ass peak. Called anywhere the bank rises, including the console scaffold, so
    /// the high-water mark cannot drift from the value it is supposed to be shadowing.
    /// </summary>
    public void TouchAssHighWater()
    {
        var cum = Ass - AssPrecum;
        if (cum > AssHighWater) AssHighWater = cum;
    }

    /// <summary>Adds precum to the Ass. Blocked by the Plug like any other filling. Returns what was added.</summary>
    public int AddAssPrecum(int ml)
    {
        if (PlugIn || ml <= 0) return 0;
        _ass += ml;
        AssPrecum += ml;
        return ml;
    }

    /// <summary>Adds precum to the Womb. Returns what was added.</summary>
    public int AddWombPrecum(int ml)
    {
        if (ml <= 0) return 0;
        _womb += ml;
        WombPrecum += ml;
        return ml;
    }

    /// <summary>The bell: whatever precum is left in the Ass and Womb is gone. Returns (ass, womb) lost.</summary>
    public (int Ass, int Womb) EvaporateHolePrecum()
    {
        var lost = (AssPrecum, WombPrecum);
        Ass -= AssPrecum;
        Womb -= WombPrecum;
        AssPrecum = 0;
        WombPrecum = 0;
        return lost;
    }

    /// <summary>
    /// The ml committed to the current pregnancy stage, which cannot leak away (design §6.3b).
    /// </summary>
    public int WombLocked => PregnancyStage * WombStage;

    /// <summary>
    /// The universal spend order (cards doc §2): <b>Skin -> Mouth -> Ass -> Womb</b>. Returns what
    /// was actually taken, which may be less than asked for.
    ///
    /// [Claude-Cowork] Least-valuable-first, and that is what makes it interesting rather than
    /// bookkeeping: Skin does nothing on its own and the Mouth cycles out every combat anyway,
    /// while the Ass is a real bank and the Womb is the run's progression track. So a generic
    /// spender is cheap for a Skin-heavy deck and genuinely dangerous for a focused one — play
    /// <i>Wet Work</i> with a clean skin and an empty mouth and it eats your Conception. That
    /// self-limits without a single restriction clause.
    ///
    /// [Claude-Code] The Womb is taken last and is not protected by the pregnancy lock. Spending
    /// below the locked threshold is exactly the "prevent conception before the bell" agency the
    /// design asks for (§6.3b), so blocking it here would remove a stated feature — but note it
    /// also means a generic spender can drop a <b>carried</b> stage, which the design does not
    /// discuss.
    /// </summary>
    public int SpendMl(int ml)
    {
        var remaining = ml;

        var fromSkin = Math.Min(Skin, remaining);
        Skin -= fromSkin;
        remaining -= fromSkin;

        remaining -= SpendMouth(remaining);

        var fromAss = Math.Min(Ass, remaining);
        Ass -= fromAss;
        remaining -= fromAss;

        var fromWomb = Math.Min(Womb, remaining);
        Womb -= fromWomb;
        remaining -= fromWomb;

        // A carried stage can be spent out from under itself; keep the stage honest if so.
        var stage = Womb / WombStage;
        if (stage < PregnancyStage) PregnancyStage = stage;

        return ml - remaining;
    }

    /// <summary>
    /// [Gorim, 2026-11-17] <b>Eat</b> takes ml in the Spend order (Skin, Mouth, Ass, Womb) and
    /// swallows it. [Claude] The same walk as <see cref="SpendMl"/>, but it reports cum and precum
    /// apart, because only cum banks to the Stomach. May return less than asked.
    /// </summary>
    public (int Cum, int Precum) TakeForEat(int ml)
    {
        var remaining = ml;

        var fromSkin = Math.Min(Skin, remaining);
        Skin -= fromSkin;
        remaining -= fromSkin;

        var fromPrecum = Math.Min(Precum, remaining);
        Precum -= fromPrecum;
        remaining -= fromPrecum;

        var fromMouth = Math.Min(Mouth, remaining);
        Mouth -= fromMouth;
        remaining -= fromMouth;

        var fromAss = Math.Min(Ass, remaining);
        Ass -= fromAss;
        remaining -= fromAss;

        var fromWomb = Math.Min(Womb, remaining);
        Womb -= fromWomb;

        var stage = Womb / WombStage;
        if (stage < PregnancyStage) PregnancyStage = stage;

        return (fromSkin + fromMouth + fromAss + fromWomb, fromPrecum);
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

    /// <summary>Registers each field as an extended saved value on the run. Called once at init.</summary>
    public static void RegisterSaves()
    {
        Register("Seductress.Mouth", s => s.Mouth, (s, v) => s.Mouth = v);
        Register("Seductress.Precum", s => s.Precum, (s, v) => s.Precum = v);
        Register("Seductress.Ass", s => s.Ass, (s, v) => s.Ass = v);
        Register("Seductress.AssHighWater", s => s.AssHighWater, (s, v) => s.AssHighWater = v);
        Register("Seductress.Womb", s => s.Womb, (s, v) => s.Womb = v);
        Register("Seductress.AssPrecum", s => s.AssPrecum, (s, v) => s.AssPrecum = v);
        Register("Seductress.WombPrecum", s => s.WombPrecum, (s, v) => s.WombPrecum = v);
        Register("Seductress.Skin", s => s.Skin, (s, v) => s.Skin = v);
        Register("Seductress.Stomach", s => s.Stomach, (s, v) => s.Stomach = v);
        Register("Seductress.Sigils", s => s.Sigils, (s, v) => s.Sigils = v);
        Register("Seductress.PregnancyStage", s => s.PregnancyStage, (s, v) => s.PregnancyStage = v);
        Register("Seductress.LactatingStage", s => s.LactatingStage, (s, v) => s.LactatingStage = v);
        Register("Seductress.LactationTail", s => s.LactationTail ? 1 : 0, (s, v) => s.LactationTail = v != 0);
        Register("Seductress.PlugIn", s => s.PlugIn ? 1 : 0, (s, v) => s.PlugIn = v != 0);
    }

    private static void Register(string id, Func<ReservoirState, int> get, Action<ReservoirState, int> set)
    {
        ExtendedSaveTypes.RegisterSavedValue<IRunState, int>(
            id,
            runState => get(Of(runState)),
            (runState, value) => set(Of(runState), value),
            (value, writer) => writer.WriteInt(value),
            reader => reader.ReadInt());
    }
}
