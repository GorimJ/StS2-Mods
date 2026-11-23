using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// [Claude-Code] The power carries the <b>drain percentage</b> as its Amount — 5 normally, 3
/// upgraded — rather than the card hard-coding "half". The upgrade in the card list is "lose 3%",
/// which is not a halving of anything, so a boolean power could not express it.
///
/// The Strength is always paid on the full 10%, which is the whole point of the card: it converts
/// the bank from something you spend into something you hold.
/// </summary>
public class MilkingGripPower : SeductressPower
{
    /// <summary>Normal Ass drain, as a percentage of the bank, for a card with no Milking Grip.</summary>
    public const decimal BaseDrainPercent = 10m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>
    /// The Ass drain this power allows, as a percentage of the bank. 5 normally, 3 upgraded.
    ///
    /// [Claude-Code] <b>Held as a field, not in Amount.</b> Found in play 2026-09-08: I originally
    /// carried it in the power's Amount, which silently became 1 because this is a
    /// <see cref="PowerStackType.Single"/> power and a single-stack power normalises its amount to
    /// 1. That made the drain a tenth rather than a half, so Ass attacks cost almost nothing while
    /// still paying full Strength and Milking Grip's doubled intake — a Blowout that should have
    /// spent 40 ml spent 8, and the bank <i>grew</i> on a card meant to empty it.
    ///
    /// This is the same shape Ritual Mark once used: state the card needs to
    /// hand the power, which does not fit the stack count.
    /// </summary>
    public int DrainPercent { get; private set; } = 5;

    /// <summary>Records the best rate played, so an upgraded copy is never undone by a base one.</summary>
    public void SetDrainPercent(int percent)
    {
        if (percent > 0 && percent < DrainPercent) DrainPercent = percent;
    }

    /// <summary>The share of a card's notional drain actually paid. 0.5 at 5%, 0.3 at 3%.</summary>
    public double DrainShare => DrainPercent / (double)BaseDrainPercent;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Milking Grip", "Ass attacks lose half as much, but gain full Strength. Ass kills gain double intake.", "Ass attacks lose half as much, but gain full Strength. Ass kills gain double intake.");
}
