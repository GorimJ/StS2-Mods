using MegaCrit.Sts2.Core.Models.Potions;

namespace MegaCrit.Sts2.Core.Models.Powers;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class FlexPotionPower : TemporaryStrengthPower
{
	public override AbstractModel OriginModel => ModelDb.Potion<FlexPotion>();
}
