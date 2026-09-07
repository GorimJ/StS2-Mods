using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Basic. Climax on purpose — the only card that does.
///
/// [Gorim 2026-09-07] Promoted from Rare into the starting deck in Let Go's slot, so Basic rather
/// than Rare: it should not also turn up as a draft, and every run should open holding one.
///
/// [Gorim 2026-09-07] Replaces <i>Abandon</i>, which was cut. Abandon paid a Lust card for its
/// Climax, which made it a card a cautious deck could never justify: the whole point of wanting a
/// Climax on demand is to switch on the cards that key off one, and paying the run's death clock
/// to do that priced it out of exactly the decks that needed it.
///
/// Seek Relief takes the Sensitive stack and fires everything that listens for a Climax, while
/// paying neither the heal nor the Lust. Draw cut 5 -> 3 and Exhaust added to price the rest.
///
/// [Claude-Code] The cost is real even though nothing visible is spent. Arousal does not reset,
/// and the Sensitive stack raises all arousal you take afterwards by 50% — so this trades a
/// percentage of the bar you have left for tempo, and the more hurt you already are the worse the
/// trade gets. It is a card you play early in a fight, or not at all.
/// </summary>
public class SeekRelief : SeductressCard
{
    public SeekRelief() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    // Keyword lines render automatically from CanonicalKeywords, so Exhaust is not named here.
    public override List<(string, string)>? Localization => new CardLoc(
        "Seek Relief",
        "Climax, but do not heal and do not gain a Lust card.\nDraw {Cards:diff()}.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ClimaxModel.Instance.DoClimax(Owner.Creature, heal: false, addLust: false);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Cards"].UpgradeValueBy(1m);
}
