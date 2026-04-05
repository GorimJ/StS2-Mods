using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BuxomModPort.Code.Powers;

public class CapacityPower : CustomPowerModel
{
    public override PowerStackType StackType => (PowerStackType)1;
    public override PowerType Type => (PowerType)1;
}
