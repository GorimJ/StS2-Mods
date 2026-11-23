using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    ///
    /// [Gorim, 2026-11-23] "They should go x2 x3 x4 rather than x2 x4 x8. And we should show it as a
    /// stacking power on the bar with correct numbers. When you play it twice it should show 2
    /// stacks of the buff."
    /// The amount is the copies played. Each copy adds x1 to the Pussy cum multiplier (x2 at
    /// 7 Sigils, [Claude] reading), and its own % per missing HP.
    /// </summary>
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Hyperfertile",
        "Gain more Pussy cum. Your Pussy attacks deal more damage for each HP the enemy is missing.",
        "Gain x{Mult} Pussy cum. Your Pussy attacks deal {Pct}% more damage for each HP the enemy is missing.");

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Mult", 1m), new DynamicVar("Pct", 0m)];

    /// <summary>Called by the card after each copy: adds that copy's share and refreshes the tooltip.</summary>
    public void AddCopy(int percent, bool tier4)
    {
        PercentPerHp += percent;
        CumMultiplier += tier4 ? 2 : 1;
        DynamicVars["Mult"].BaseValue = CumMultiplier;
        DynamicVars["Pct"].BaseValue = PercentPerHp;
    }

    /// <summary>Percent more damage per missing HP: 1, 2 or 3 by the card's Sigil stage. Set by the card.</summary>
    public int PercentPerHp { get; set; }

    /// <summary>Pussy cum multiplier: 1 + one per copy (two per copy played at 7 Sigils).</summary>
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
