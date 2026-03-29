using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Entities.Potions;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class PotionProcureResult
{
	public bool success;

	public PotionModel potion;

	public PotionProcureFailureReason failureReason;
}
