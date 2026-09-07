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

/// <summary>
/// Squeezing's engine. Half the drain, full Strength, double intake on Ass kills — it solves the
/// archetype's structural problem of a bank that depletes faster than kills refill it.
/// </summary>
public class MilkingGrip : SeductressCard
{
    public MilkingGrip() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    public override List<(string, string)>? Localization => new CardLoc(
        "Milking Grip",
        "Ass attacks lose half as much Ass, but gain [gold]Strength[/gold] as though they had lost the full amount.\nAss kills gain double intake.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MilkingGripPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }
}
