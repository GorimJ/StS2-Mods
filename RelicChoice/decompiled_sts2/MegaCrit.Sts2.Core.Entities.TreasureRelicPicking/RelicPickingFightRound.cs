using System.Collections.Generic;

namespace MegaCrit.Sts2.Core.Entities.TreasureRelicPicking;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class RelicPickingFightRound
{
	public List<RelicPickingFightMove?> moves = new List<RelicPickingFightMove?>();
}
