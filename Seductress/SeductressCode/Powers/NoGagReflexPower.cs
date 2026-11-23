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
    /// <summary>
    /// [Gorim, 2026-11-23] "No gag reflex doesn't stack? I have 2 in play but my deep throat cards
    /// only cost 1 less." [Claude] It was a single-instance power that took a flat 1 off each card.
    /// It now counts copies, and the cost hook takes off one per copy. The HP-loss half is on or
    /// off, so a second copy adds nothing there.
    /// </summary>
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>Set by the card so the upgraded version can add its draw.</summary>
    public bool DrawsOnDeepThroat { get; set; }

    public override List<(string, string)>? Localization => new PowerLoc(
        "No Gag Reflex",
        "Deep Throat cards cost less and no longer lose HP.",
        "Deep Throat cards cost {Amount} less and no longer lose HP.");

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (Owner?.Player is null || card.Owner != Owner.Player) return false;
        if (!card.Tags.Contains(SeductressTags.DeepThroat) || Amount <= 0m) return false;
        modifiedCost = System.Math.Max(0m, originalCost - Amount);
        return true;
    }
}
