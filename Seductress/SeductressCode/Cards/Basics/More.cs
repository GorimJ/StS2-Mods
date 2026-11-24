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

namespace Seductress.SeductressCode.Cards.Basics;

/// <summary>
/// Basic attack, four in the starting deck. Carries no tag of its own: it Continues.
///
/// [Gorim, 2026-11-17] Renamed from Pump. Continuing attacks are named as things she says, so the
/// name and art fit whichever hole the card routes to. Routed to Ass it pays the Ass rule's cum
/// cost and takes its bonus, with no HP cost.
/// </summary>
public class More : SeductressCard
{
    public More() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    public override bool IsContinuing => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(6m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(BonusCalc)
    ];

    protected override decimal PreviewBonus() => ContinuingAssNotional();

    public override List<(string, string)>? Localization => new CardLoc(
        "More",
        "Continuing. Deal {CalculatedDamage:diff()} damage.\n[i](+1 per 50 ml in your Ass, when continuing Ass)[/i]");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        MainFile.Logger.Info($"[Seductress] More routing to {Route}");
        var damage = DynamicVars.CalculationBase.BaseValue + SpendContinuingAss();
        await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
        DynamicVars.CalculatedDamage.RecalculateForUpgradeOrEnchant();
    }
}
