using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// Hyperfertile. The doubled Pussy intake is applied where kills are routed (ClimaxModel); this
/// holds the damage half: Pussy attacks deal 1% more per HP the target is missing.
/// </summary>
public class HyperfertilePower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    /// <summary>
    /// [Gorim, 2026-11-23] "Once per turn triggers obviously happen twice? And 'your next climax'
    /// becomes 'your next 2 climaxes'? Obviously?" A stacked power does its thing once per stack.
    /// [Claude] So each copy doubles (or triples) the Pussy cum again, and adds its own % per HP.
    /// </summary>
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Hyperfertile",
        "Gain twice as much Pussy cum. Your Pussy attacks deal 1% more damage for each HP the enemy is missing.",
        "Gain twice as much Pussy cum. Your Pussy attacks deal 1% more damage for each HP the enemy is missing.");

    /// <summary>Percent more damage per missing HP: 1, 2 or 3 by the card's Sigil stage. Set by the card.</summary>
    public int PercentPerHp { get; set; }

    /// <summary>Pussy cum multiplier: 2, or 3 at the card's 7-Sigil stage. Set by the card.</summary>
    public int CumMultiplier { get; set; } = 1;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Owner != dealer || target is null) return 1m;
        if (!props.IsPoweredAttack()) return 1m;
        if (cardSource is not Cards.SeductressCard card) return 1m;
        if (ClimaxModel.Instance.RouteOf(card) != Hole.Pussy) return 1m;
        var missing = target.MaxHp - target.CurrentHp;
        return missing <= 0 ? 1m : 1m + missing * PercentPerHp / 100m;
    }
}
