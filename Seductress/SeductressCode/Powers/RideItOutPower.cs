using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// [Gorim, 2026-11-23] "The 'next climax doesn't give you a lust card' should be a separate buff
/// the powers that apply it apply, and should stack as a single buff tracker between those
/// powers. Probably call it 'Ride it out'."
///
/// [Claude] While it has stacks, a Climax adds no Lust card (so it is also not fatal at the Lust
/// limit) and uses one stack. Read and spent by <see cref="Systems.ClimaxModel"/>. A Climax that
/// Hold the Edge prevents never reaches it, so it spends nothing. Gone at the end of combat, as
/// powers are.
/// </summary>
public class RideItOutPower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Ride It Out",
        "Your next Climaxes add no Lust.",
        "Your next {Amount} Climax(es) add no Lust.");
}
