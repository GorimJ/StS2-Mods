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
    public override PowerStackType StackType => PowerStackType.Single;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Hyperfertile",
        "Gain twice as much Pussy cum. Your Pussy attacks deal 1% more damage for each HP the enemy is missing.",
        "Gain twice as much Pussy cum. Your Pussy attacks deal 1% more damage for each HP the enemy is missing.");

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Owner != dealer || target is null) return 1m;
        if (!props.IsPoweredAttack()) return 1m;
        if (cardSource is not Cards.SeductressCard card) return 1m;
        if (ClimaxModel.Instance.RouteOf(card) != Hole.Pussy) return 1m;
        var missing = target.MaxHp - target.CurrentHp;
        return missing <= 0 ? 1m : 1m + missing / 100m;
    }
}
