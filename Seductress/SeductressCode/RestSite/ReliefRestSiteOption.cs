using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using Seductress.SeductressCode.Cards;
using Seductress.SeductressCode.Extensions;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.RestSite;

/// <summary>
/// Relief — remove 1 Lust card instead of healing (design §5).
///
/// This is what makes a 3-card clock survivable rather than brutal, and it is the reason the clock
/// could be tightened at all: the cap and the availability of removal are two dials.
///
/// Deliberately uncapped. The opportunity cost is the limiter — for this character HP is arousal,
/// so skipping the heal means entering the next fight already aroused.
/// </summary>
public class ReliefRestSiteOption(Player owner) : CustomRestSiteOption(owner)
{
    public override string OptionId => "SEDUCTRESS_RELIEF";

    /// <summary>
    /// REQUIRED, not decoration. RestSiteOption.Icon builds its path from OptionId, so without this
    /// the game looks for res://images/ui/rest_site/option_seductress_relief.png, throws
    /// AssetLoadException inside NRestSiteButton._Ready, and <b>the button never renders</b> — the
    /// option is silently absent rather than broken-looking.
    ///
    /// This cost the whole of slice 2: Relief was never on screen and its opportunity cost was
    /// never testable, while the results doc recorded it as working.
    ///
    /// Placeholder art from the template. Deliberately not a vanilla rest-site icon, which would
    /// read as one of the existing options.
    /// </summary>
    public override string CustomIconPath => "relic.png".RelicImagePath();

    /// <summary>Greyed out but still hoverable when there is nothing to remove.</summary>
    public override bool IsEnabled => LustCards().Any();

    /// <summary>
    /// Swaps to a disabled line when there is no Lust to remove, the way Smith does with no
    /// upgradable cards — so the greyed state says why rather than just looking broken.
    /// </summary>
    public override LocString Description => new(
        "rest_site_ui",
        IsEnabled ? "OPTION_" + OptionId + ".description" : "OPTION_" + OptionId + ".descriptionDisabled");

    private IEnumerable<Cards.LustCard> LustCards() =>
        Owner.Deck.Cards.OfType<Cards.LustCard>();

    public override async Task<bool> OnSelect()
    {
        var card = LustCards().FirstOrDefault();
        if (card is null) return false;

        await CardPileCmd.RemoveFromDeck(card);
        FloorLog.RecordRelief();
        MainFile.Logger.Info(
            $"[Seductress] Relief taken: removed {card.Id.Entry}, " +
            $"lust now {ClimaxModel.LustCount(Owner)}/{ClimaxModel.LustLimit}");
        return true;
    }
}
