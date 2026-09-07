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

/// <summary>Squeezing's ceiling. Half the bank for four hits — it should hurt to use.</summary>
public class Blowout : SeductressCard
{
    public Blowout() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Ass;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(6m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(BonusCalc)
    ];


    /// <summary>The live Ass bonus, so the card face shows what it will actually deal.</summary>
    protected override decimal PreviewBonus() => AssNotional(0.50);

    public override List<(string, string)>? Localization => new CardLoc(
        "Blowout", "Ass. This card loses 50% of your Ass instead of 10%.\nDeal {CalculatedDamage:diff()} damage four times.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = SpendAssForDamage(0.50);
        var hits = 4;

        for (var i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(DynamicVars.CalculationBase.BaseValue + bonus).FromCard(this)
                .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2m);
        DynamicVars.CalculatedDamage.RecalculateForUpgradeOrEnchant();
    }
}
