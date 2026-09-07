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
/// Core, Uncommon. Climax on purpose — the only card that does.
///
/// [Claude-Cowork] Replaces <i>Abandon</i>, which paid a <b>Lust card</b> for its Climax. That
/// priced it out of exactly the decks that wanted it: the reason to want a Climax on demand is to
/// switch on the cards that key off one, and paying the run's death clock for that is not a trade
/// a cautious deck can make. This is Adrenaline at one rarity lower, paid for in Sensitive.
///
/// [Claude-Cowork] <b>The <i>Rapture</i> line is the point, not a risk.</b> With Rapture out this
/// is 0 cost for 4 energy, 2 Strength, 20 Block and 2 cards — accepted, because Rapture was
/// near-dead without a Climax-on-demand card: a 3-cost Rare keyed off the one event you spend the
/// whole run avoiding, so it only ever paid out when things were already going wrong.
///
/// [Claude-Code] Nothing visible is spent and the cost is still real. Arousal does not reset, and
/// the new Sensitive stack raises everything you take afterwards by 50% — so it trades a
/// percentage of the bar you have left for tempo, and gets worse the more hurt you already are. A
/// card you play early in a fight or not at all.
///
/// [Claude-Code] <b>Renamed from the card I built in slice 3 as "Seek Relief".</b> That name now
/// belongs to the renamed <i>Let Go</i> in the starting deck (cards doc §4), so this had to move
/// aside. Same mechanism, one rarity down, minus a card of draw and plus an energy.
/// </summary>
public class ForcedOrgasm : SeductressCard
{
    public ForcedOrgasm() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new EnergyVar(1)
    ];

    // Keyword lines render automatically from CanonicalKeywords, so Exhaust is not named here.
    public override List<(string, string)>? Localization => new CardLoc(
        "Forced Orgasm",
        "Climax, but do not heal and do not gain a Lust card.\nDraw {Cards:diff()}. Gain {Energy:energyIcons()}.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ClimaxModel.Instance.DoClimax(Owner.Creature, heal: false, addLust: false);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Cards"].UpgradeValueBy(1m);
}
