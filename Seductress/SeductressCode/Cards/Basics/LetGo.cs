using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Seductress.SeductressCode.Cards.Basics;

/// <summary>
/// The self-damage starter. 1 in the deck, and the cheapest way to prove death interception
/// catches the character's own cards: 20 HP a play will Climax you soon enough.
/// </summary>
public class LetGo : SeductressCard
{
    public LetGo() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(20m),
        new CardsVar(2)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Let Go",
        "Lose {HpLoss} HP. Draw {Cards}.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Cards"].UpgradeValueBy(1m);
}
