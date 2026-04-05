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
public class BouncyExercise : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { 
        new DamageVar(10m, (ValueProp)8),
        new DynamicVar("magic", 3m)
    };
    
    public BouncyExercise() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) {}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, null)
            .Execute(choiceContext);
            
        var bounceStatus = CombatState.CreateCard<BigBounceStatus>(Owner);
        if (bounceStatus != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(bounceStatus, PileType.Hand, true, CardPilePosition.Random);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
