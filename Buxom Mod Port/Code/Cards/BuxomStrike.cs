using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using BuxomModPort.Code.Character;

namespace BuxomModPort.Code.Cards;

[Pool(typeof(BuxomCardPool))]
public class BuxomStrike : CustomCardModel
{
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { (CardTag)1 };
    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new DamageVar(6m, (ValueProp)8) };
    
    public BuxomStrike() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) {}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash", null, null)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
