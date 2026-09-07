using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Seductress.SeductressCode.Cards;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// Deep Throat cards cost 1 less and no longer lose HP. Upgraded, they also draw 1 each.
///
/// The HP half is read by the cards themselves (see <see cref="SeductressCard.SuppressHpLoss"/>);
/// the cost half is applied here, once per card, both to cards already in combat and to any that
/// enter later.
/// </summary>
public class NoGagReflexPower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>Set by the card so the upgraded version can add its draw.</summary>
    public bool DrawsOnDeepThroat { get; set; }

    public override List<(string, string)>? Localization => new PowerLoc(
        "No Gag Reflex",
        "Deep Throat cards cost 1 less and no longer lose HP.",
        "Deep Throat cards cost 1 less and no longer lose HP.");

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (Owner?.Player?.PlayerCombatState is null) return Task.CompletedTask;
        foreach (var pile in Owner.Player.PlayerCombatState.AllPiles)
        {
            foreach (var card in pile.Cards) Discount(card);
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        Discount(card);
        return Task.CompletedTask;
    }

    private static void Discount(CardModel card)
    {
        if (card.Tags.Contains(SeductressTags.DeepThroat)) card.EnergyCost.AddThisCombat(-1, reduceOnly: true);
    }
}
