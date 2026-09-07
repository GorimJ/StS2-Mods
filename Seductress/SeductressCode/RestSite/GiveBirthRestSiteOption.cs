using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using Seductress.SeductressCode.Extensions;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.RestSite;

/// <summary>
/// Give birth — gain N Sigils, where N is your pregnancy stage (design §6.3b).
///
/// [Claude-Cowork] A <b>free action</b>: taking it should not cost you the rest site's real
/// choice between Indulge, Relief and Smith.
///
/// [Claude-Code] <b>Implemented by returning false from OnSelect</b>, which is the game's own
/// "this option was not successful" path — the same one Smith uses when there is nothing to
/// upgrade — on the expectation that the site then stays open for a real pick. That is a
/// reasonable read of the vanilla code rather than a verified one, and it is the single most
/// likely thing in this slice to behave differently in play than on paper. <b>Watch it:</b> if
/// the site closes anyway, birth costs a rest site and the delivery decision changes completely.
///
/// The alternative — delivering automatically on arrival — was rejected because it removes the
/// decision the mechanic exists for. If you always deliver, "am I close enough to the next
/// threshold to be worth carrying" is never asked.
/// </summary>
public class GiveBirthRestSiteOption(Player owner) : CustomRestSiteOption(owner)
{
    public override string OptionId => "SEDUCTRESS_BIRTH";

    /// <summary>REQUIRED — a missing icon aborts the button's Reload(). See ReliefRestSiteOption.</summary>
    public override string CustomIconPath => "relic.png".RelicImagePath();

    public override bool IsEnabled => Pregnancy.IsPregnant(Reservoirs.Of(Owner.RunState));

    public override LocString Description => new(
        "rest_site_ui",
        IsEnabled ? "OPTION_" + OptionId + ".description" : "OPTION_" + OptionId + ".descriptionDisabled");

    public override Task<bool> OnSelect()
    {
        Pregnancy.GiveBirth(Owner);
        // Deliberately false — see the class note. This is the "free action" claim.
        return Task.FromResult(false);
    }
}
