using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Squeezing's basic multi-hit, and the card most players meet the Ass rule through.
///
/// [Claude-Code 2026-09-07] This card used to carry its own inline copy of the Ass rule rather
/// than calling <c>SpendAssForDamage</c>, and the copy was never updated when the ratio changed —
/// so it was still paying the old +1 per 10 ml while every other Ass card paid 1:1, and it ignored
/// Milking Grip's halved drain entirely. At a full 200 ml bank it gave +2 where it should give
/// +20. Now it goes through the shared rule like the rest.
/// </summary>
public class Clench : SeductressCard
{
    public Clench() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Ass;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(5m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(BonusCalc),
        new HpLossVar(2m)
    ];

    /// <summary>The live Ass bonus, so the card face shows what it will actually deal.</summary>
    protected override decimal PreviewBonus() => AssNotional();

    public override List<(string, string)>? Localization => new CardLoc(
        "Clench",
        "Ass. Deal {CalculatedDamage:diff()} damage twice. Lose {HpLoss:diff()} HP.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // Spend first, then deal: the bonus returned is the notional spend, which is exactly the
        // number the card face was showing a moment ago.
        var bonus = StandingAssBonus();
        var damage = DynamicVars.CalculationBase.BaseValue + bonus;

        for (var i = 0; i < 2; i++)
        {
            await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2m);
        DynamicVars.CalculatedDamage.RecalculateForUpgradeOrEnchant();
    }
}
