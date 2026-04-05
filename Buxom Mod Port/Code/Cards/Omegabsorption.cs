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
using BuxomModPort.Code.Powers;

namespace BuxomModPort.Code.Cards;

[Pool(typeof(BuxomCardPool))]
public class Omegabsorption : CustomCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { 
        new DynamicVar("magic", 1m),
        new DynamicVar("secondary_magic", 3m)
    };
    
    public Omegabsorption() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self) {}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CapacityPower>(Owner.Creature, DynamicVars["secondary_magic"].BaseValue, Owner.Creature, this, false);
        await CardPileCmd.Draw(choiceContext, DynamicVars["magic"].BaseValue, Owner, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["magic"].UpgradeValueBy(1m);
        DynamicVars["secondary_magic"].UpgradeValueBy(1m);
    }
}
