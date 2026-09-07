using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Seductress.SeductressCode.Cards;

/// <summary>The second Lust card. Draw instead of energy; same 12 arousal.</summary>
public class Craving : LustCard
{
    public Craving() : base(0, CardType.Curse, CardRarity.Curse, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(12m),
        new CardsVar(3)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Craving",
        "Playable: draw {Cards:diff()}. Lose {HpLoss:diff()} HP.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }
}
