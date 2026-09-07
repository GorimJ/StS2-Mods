using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Seductress.SeductressCode.Cards.Basics;

/// <summary>Basic attack. 5 in the starting deck. Carries no tag of its own — it Continues.</summary>
public class Pump : SeductressCard
{
    public Pump() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    public override bool IsContinuing => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Pump",
        "Continuing. Deal {Damage:diff()} damage.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        MainFile.Logger.Info($"[Seductress] Pump routing to {Route}");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
