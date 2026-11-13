using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// Gained on every Climax, +1 per Climax, cleared at end of combat (design §4).
/// +50% per stack to both arousal taken and Intake gained.
/// </summary>
public class SensitivePower : SeductressPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Sensitive",
        "Arousal taken and Intake gained are increased by 50% per stack.",
        "Arousal taken and Intake gained are increased by 50% per stack.");

    /// <summary>The design's <c>1 + 0.5 x Sensitive</c> multiplier.</summary>
    public static decimal MultiplierFor(Creature? creature)
    {
        var stacks = creature?.GetPowerAmount<SensitivePower>() ?? 0;
        return 1m + 0.5m * stacks;
    }

    /// <summary>
    /// SPIKE NOTE: this deliberately ignores <see cref="ValueProp.Unpowered"/>, so the character's
    /// own self-damage cards are amplified too. That is what the design asks for ("two climaxes
    /// halve your effective bar"), but it is a departure from the vanilla meaning of Unpowered.
    /// Flagged in SEDUCTRESS_SPIKE_RESULTS.md as a decision.
    /// </summary>
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, MegaCrit.Sts2.Core.Entities.Cards.CardPlay? cardPlay)
    {
        if (target != Owner) return 1m;
        return 1m + 0.5m * Amount;
    }
}
