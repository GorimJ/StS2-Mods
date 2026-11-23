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
    /// <summary>
    /// [Gorim, 2026-11-23] "Once per turn triggers obviously happen twice? And 'your next climax'
    /// becomes 'your next 2 climaxes'? Obviously?" A stacked power does its thing once per stack.
    ///
    /// [Gorim, 2026-11-23] "They should go x2 x3 x4 rather than x2 x4 x8. And we should show it as a
    /// stacking power on the bar with correct numbers. When you play it twice it should show 2
    /// stacks of the buff."
    /// The loss is divided by (copies + 1) and Ass kill intake multiplied by (copies + 1); an
    /// upgraded copy's better rate scales the same way ([Claude] reading).
    /// </summary>
    public override PowerStackType StackType => PowerStackType.Counter;

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
    /// This is the same shape as <see cref="RitualMarkPower.SigilBonus"/>: state the card needs to
    /// hand the power, which does not fit the stack count.
    /// </summary>
    public int DrainPercent { get; private set; } = 5;

    /// <summary>Records the best rate played, so an upgraded copy is never undone by a base one.</summary>
    public void SetDrainPercent(int percent)
    {
        if (percent > 0 && percent < DrainPercent) DrainPercent = percent;
    }

    /// <summary>The share of a card's notional drain actually paid. 0.5 at 5%, 0.3 at 3%.</summary>
    public double DrainShare => DrainPercent / (double)BaseDrainPercent * 2.0 / (System.Math.Max(1, (int)Amount) + 1);

    /// <summary>Ass kill intake multiplier: x(copies + 1).</summary>
    public int KillMultiplier => System.Math.Max(1, (int)Amount) + 1;

    protected override IEnumerable<MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar> CanonicalVars =>
        [new MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar("Loss", 50m),
         new MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar("Mult", 2m)];

    public void Refresh()
    {
        DynamicVars["Loss"].BaseValue = (decimal)System.Math.Round(DrainShare * 100);
        DynamicVars["Mult"].BaseValue = KillMultiplier;
    }

    public override List<(string, string)>? Localization => new PowerLoc(
        "Milking Grip", "Ass attacks lose less, but gain full Strength. Ass kills gain more intake.", "Ass attacks lose {Loss}% as much, but gain full Strength. Ass kills gain x{Mult} intake.");
}
