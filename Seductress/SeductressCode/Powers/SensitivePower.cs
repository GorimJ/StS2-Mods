using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// Gained on every Climax, +1 per Climax, cleared at end of combat (design §4).
///
/// [Gorim, 2026-11-20] It no longer raises ml intake. Each stack still raises HP loss taken by
/// 50%, and now raises <b>Pussy damage</b> instead ("Pussy damage only for now"). [Claude] +50%
/// per stack, the same step as the cost.
/// </summary>
public class SensitivePower : SeductressPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Sensitive",
        "Arousal taken is increased by 50% per stack. Your Pussy attacks deal 50% more damage per stack.",
        "Arousal taken is increased by 50% per stack. Your Pussy attacks deal 50% more damage per stack.");

    /// <summary>
    /// SPIKE NOTE: this deliberately ignores <see cref="ValueProp.Unpowered"/>, so the character's
    /// own self-damage cards are amplified too. That is what the design asks for ("two climaxes
    /// halve your effective bar"), but it is a departure from the vanilla meaning of Unpowered.
    /// Flagged in SEDUCTRESS_SPIKE_RESULTS.md as a decision.
    /// </summary>
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, MegaCrit.Sts2.Core.Entities.Cards.CardPlay? cardPlay)
    {
        if (target == Owner) return 1m + 0.5m * Amount;

        // Dealt by the owner with a Pussy-routed attack: the same step, in her favour.
        if (dealer == Owner && props.IsPoweredAttack() && cardSource is Cards.SeductressCard card &&
            Systems.ClimaxModel.Instance.RouteOf(card) == Systems.Hole.Pussy)
        {
            return 1m + 0.5m * Amount;
        }
        return 1m;
    }
}
