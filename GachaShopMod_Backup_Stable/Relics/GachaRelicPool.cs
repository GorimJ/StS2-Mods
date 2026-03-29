using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace GachaShopMod.Relics;

public class GachaRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "colorless";

    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        return new RelicModel[0];
    }
}
