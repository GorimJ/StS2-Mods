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
/// Core Common. [Claude draft, 2026-11-14; Gorim moved it to Build on the draft board, 2026-11-17,
/// with the note "Upgrade to draw 2".] The first draftable Continuing attack: it pays a card once
/// a tagged attack has set the mode to a hole (Mouth, Ass or Pussy; Skin is not a hole).
/// </summary>
public class KeepGoing : SeductressCard
{
    public KeepGoing() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    public override bool IsContinuing => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(8m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(BonusCalc),
        new CardsVar(1)
    ];

    protected override decimal PreviewBonus() => ContinuingAssNotional();

    public override List<(string, string)>? Localization => new CardLoc(
        "Keep Going",
        "Continuing. Deal {CalculatedDamage:diff()} damage. If your last Attack used a hole, draw {Cards:diff()}.{ContinuingAss:cond:\n[i](+1 per 50 ml in your Ass)[/i]|}");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var route = Route;
        var damage = DynamicVars.CalculationBase.BaseValue + SpendContinuingAss();
        await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (route is Hole.Mouth or Hole.Ass or Hole.Pussy)
            await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Cards"].UpgradeValueBy(1m);
}
