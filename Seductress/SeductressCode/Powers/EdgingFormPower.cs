using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// Ancient. [Gorim, 2026-11-20] Whenever an Attack gains cum, gain that much again as precum in
/// the same hole. Each stack is another 100%.
///
/// The work is done where cum enters a reservoir (<c>ClimaxModel.GainCum</c>), which reads this
/// power's Amount; the power itself has no hooks.
/// </summary>
public class EdgingFormPower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Edging Form",
        "Whenever an Attack gains cum, gain that much again as precum in the same hole.",
        "Whenever an Attack gains cum, gain {Amount} times that much as precum in the same hole.");
}
