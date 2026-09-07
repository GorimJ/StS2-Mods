using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Breeding Uncommon. The v10 "stop costing HP at 4 Sigils" clause is gone — removing a downside
/// does not fit any rung of the ladder, so the 2 HP now persists at every tier.
/// </summary>
public class Claim : SeductressCard
{
    public Claim() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Pussy;
    protected override bool RepeatsAtTier4 => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
        new CalculationBaseVar(7m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(BonusCalc),
        new DynamicVar("TierDamage", 12m),
        new HpLossVar(1m)
    ];


    /// <summary>Ritual Mark and the tier-2 swap, live on the card face.</summary>
    protected override decimal PreviewBonus() => TieredPussyBonus();

    public override List<(string, string)>? Localization => new CardLoc(
        "Claim",
        "Pussy. Deal {CalculatedDamage:diff()} damage. Lose {HpLoss:diff()} HP.\n{T1} Sigil: apply 1 [gold]Vulnerable[/gold].\n{T2} Sigils: deal {TierDamage:diff()} instead.\n{T3} Sigils: apply 2 [gold]Vulnerable[/gold] instead.\n{T4} Sigils: repeats 1 time.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);
        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);

        if (Sigils.At(Owner, Sigils.Tier1))
        {
            var stacks = Sigils.At(Owner, Sigils.Tier3) ? 2m : 1m;
            await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, stacks, Owner.Creature, this);
        }

        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
        DynamicVars.CalculatedDamage.RecalculateForUpgradeOrEnchant();
        DynamicVars["TierDamage"].UpgradeValueBy(4m);
    }
}
