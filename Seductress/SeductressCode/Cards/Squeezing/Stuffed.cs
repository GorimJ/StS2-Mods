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

/// <summary>Squeezing. Rewards being low on the bar, which is where the archetype lives.</summary>
public class Stuffed : SeductressCard
{
    public Stuffed() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Ass;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(12m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(BonusCalc),
        new HpLossVar(3m)
    ];


    /// <summary>The live Ass bonus, so the card face shows what it will actually deal.</summary>
    protected override decimal PreviewBonus() => AssNotional();

    public override List<(string, string)>? Localization => new CardLoc(
        "Stuffed",
        "Ass. Deal {CalculatedDamage:diff()} damage. Lose {HpLoss:diff()} HP.\nIf your HP is below half, hit one additional time.{InCombat:\n(+1 per 50 ml in your Ass)|}");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = StandingAssBonus();
        var belowHalf = Owner.Creature.CurrentHp * 2 < Owner.Creature.MaxHp;
        var hits = 1 + (belowHalf ? 1 : 0); // Stretch My Hole Daddy adds its hit through StretchPower

        for (var i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(DynamicVars.CalculationBase.BaseValue + bonus).FromCard(this, cardPlay)
                .Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        }
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
        DynamicVars.CalculatedDamage.RecalculateForUpgradeOrEnchant();
    }
}
