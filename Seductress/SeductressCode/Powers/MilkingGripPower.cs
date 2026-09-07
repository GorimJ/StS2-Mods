using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// [Claude-Code] The power carries the <b>drain percentage</b> as its Amount — 5 normally, 3
/// upgraded — rather than the card hard-coding "half". The upgrade in the card list is "lose 3%",
/// which is not a halving of anything, so a boolean power could not express it.
///
/// The Strength is always paid on the full 10%, which is the whole point of the card: it converts
/// the bank from something you spend into something you hold.
/// </summary>
public class MilkingGripPower : SeductressPower
{
    /// <summary>Normal Ass drain, as a percentage of the bank, for a card with no Milking Grip.</summary>
    public const decimal BaseDrainPercent = 10m;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>The share of a card's notional drain actually paid. 0.5 at 5%, 0.3 at 3%.</summary>
    public double DrainShare => (double)(Amount <= 0 ? 5m : Amount) / (double)BaseDrainPercent;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Milking Grip", "Ass attacks lose half as much, but gain full Strength. Ass kills gain double intake.", "Ass attacks lose half as much, but gain full Strength. Ass kills gain double intake.");
}
