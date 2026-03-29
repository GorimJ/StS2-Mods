using MegaCrit.Sts2.Core.Models.Relics;

namespace MegaCrit.Sts2.Core.Models.Powers;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class HelicalDartPower : TemporaryDexterityPower
{
	public override AbstractModel OriginModel => ModelDb.Relic<HelicalDart>();
}
