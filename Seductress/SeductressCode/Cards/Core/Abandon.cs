using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
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
/// Core, Rare. Climax on purpose — the only card that does, and the reason DoClimax is separated
/// from the death hook. Costs a Lust card like any other Climax, so it is a real decision.
/// </summary>
public class Abandon : SeductressCard
{
    public Abandon() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(5)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Abandon", "Climax immediately. Draw {Cards:diff()}.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ClimaxModel.Instance.DoClimax(Owner.Creature);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Cards"].UpgradeValueBy(1m);
}
