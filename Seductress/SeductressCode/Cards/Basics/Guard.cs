using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Seductress.SeductressCode.Cards.Basics;

/// <summary>Basic block. 4 in the starting deck. A Skill, so never tagged.</summary>
public class Guard : SeductressCard
{
    public Guard() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Guard",
        "Gain {Block} [gold]Block[/gold].");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
