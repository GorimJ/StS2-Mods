using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using BuxomModPort.Code.Character;
using BuxomModPort.Code.Powers;

namespace BuxomModPort.Code.Cards;

[Pool(typeof(BuxomCardPool))]
public class BigBounceStatus : CustomCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[]
    {
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    };

    public BigBounceStatus() : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy) {}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "Target");

        decimal capacityAmount = Owner.Creature.GetPowerAmount<CapacityPower>();
        decimal damage = Math.Floor(capacityAmount / 2m);

        if (damage > 0)
        {
            await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt", null, null)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
    }
}
