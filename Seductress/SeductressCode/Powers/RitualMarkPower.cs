using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

public class RitualMarkPower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Ritual Mark", "Your Sigils and your Pussy cards deal additional damage.", "Your Sigils and your Pussy cards deal additional damage.");

    /// <summary>Tier 1 onwards: the Sigil passive also gains a bonus. Set by the card.</summary>
    public int SigilBonus { get; set; }

    /// <summary>
    /// Tier 4: both bonuses become +1 per Sigil instead of a flat amount. Set by the card.
    ///
    /// At 7 Sigils this is +7 against the +8 plain doubling would give, so it is a marginal
    /// downgrade at exactly the threshold that unlocks it and only pulls ahead from 8. Kept
    /// deliberately — converting a static buff into a scaling one is the fantasy, and 7 Sigils is
    /// already past the run target.
    /// </summary>
    public bool PerSigil { get; set; }

    public decimal PussyBonus(MegaCrit.Sts2.Core.Entities.Players.Player player) =>
        PerSigil ? Sigils.Effective(player) : Amount;

    /// <summary>
    /// NOT YET LIVE: the Sigil passive (design §7.0, start-of-turn AoE) is out of scope, so there
    /// is nothing for this half to modify. Kept so the card is complete when the passive lands.
    /// </summary>
    public decimal SigilPassiveBonus(MegaCrit.Sts2.Core.Entities.Players.Player player) =>
        PerSigil ? Sigils.Effective(player) : SigilBonus;
}
