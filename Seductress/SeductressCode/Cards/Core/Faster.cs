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
/// Core Uncommon, two hits. [Claude draft as "Rhythm", 2026-11-14; Gorim moved it to Build and
/// renamed it Faster, 2026-11-17.] Multi-hit is where the Ass bonus pays most, which is why this
/// is the card his note about Continuing Ass was written on.
/// </summary>
public class Faster : SeductressCard
{
    public Faster() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    public override bool IsContinuing => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(4m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(BonusCalc)
    ];

    protected override decimal PreviewBonus() => ContinuingAssNotional();

    public override List<(string, string)>? Localization => new CardLoc(
        "Faster",
        "Continuing. Deal {CalculatedDamage:diff()} damage twice.{ContinuingAss:cond:\n[i](+1 per 50 ml in your Ass)[/i]|}");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var damage = DynamicVars.CalculationBase.BaseValue + SpendContinuingAss();
        for (var i = 0; i < 2; i++)
        {
            await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(1m);
        DynamicVars.CalculatedDamage.RecalculateForUpgradeOrEnchant();
    }
}
