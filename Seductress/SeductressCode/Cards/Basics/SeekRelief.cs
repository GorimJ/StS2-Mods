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
/// Basic. The starting deck's self-damage axis — the blood-magic half of the character, and the
/// cheapest way to meet a Climax on your own terms rather than by being hit.
///
/// [Gorim] <b>Renamed from <i>Let Go</i></b>, and its upgrade now <b>lowers the HP cost</b>
/// (9 to 4) rather than drawing a third card. On a card whose whole function is paying HP for
/// cards, cutting the price is the upgrade that changes how you play it; drawing more just made it
/// a bigger version of the same gamble.
///
/// [Gorim] 9 HP is <b>45% of the 20 HP bar</b>, where the old 20 was 31% of 65 — deliberately
/// harsher, because the bar is meant to be swingy and the rescale was set as though it were 30-40
/// rather than by a clean divide.
///
/// [Claude-Code] Not to be confused with the rest-site <b>Relief</b>, which removes a Lust card.
/// The two share a word and do opposite things: this one moves you toward a Climax, that one pays
/// off the debt a Climax leaves. Worth watching in play as a source of confusion.
/// </summary>
public class SeekRelief : SeductressCard
{
    public SeekRelief() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(9m),
        new CardsVar(2),
        new EnergyVar(1)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Seek Relief",
        "Lose {HpLoss:diff()} HP. Draw {Cards:diff()}. Gain {Energy:energyIcons()}.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner);
        // [Gorim, 2026-11-17, board comment] "I want to add 'Gain 1 energy' to that card."
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.HpLoss.UpgradeValueBy(-5m);
}
