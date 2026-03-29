using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.Powers;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class EnfeeblingTouchPower : TemporaryStrengthPower
{
	public override AbstractModel OriginModel => ModelDb.Card<EnfeeblingTouch>();

	protected override bool IsPositive => false;
}
