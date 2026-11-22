using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Seductress.SeductressCode.Powers;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Core Rare. [Claude draft, 2026-11-14: "Retain. The next time you would Climax this combat,
/// don't. Heal 5 instead. Exhaust."] [Gorim's rework note: "Remove retain, have it 'if you climax
/// this turn, prevent that damage, gain intangible, and deal that damage to all enemies'".]
///
/// [Gorim, 2026-11-21] "It should be til start of next turn": it covers the enemies' attacks
/// after the turn it is played. (It used to end with that turn, my reading of "this turn".)
///
/// [Claude] It fires once. It still Exhausts, which the note did not mention. The upgrade (cost 0) is
/// mine: the draft's "Heal 8" no longer applies.
/// </summary>
public class HoldTheEdge : SeductressCard
{
    public HoldTheEdge() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override List<(string, string)>? Localization => new CardLoc(
        "Hold the Edge",
        "If you would Climax before your next turn, prevent that damage, gain 1 [gold]Intangible[/gold], and deal that damage to ALL enemies.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<HoldTheEdgePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
