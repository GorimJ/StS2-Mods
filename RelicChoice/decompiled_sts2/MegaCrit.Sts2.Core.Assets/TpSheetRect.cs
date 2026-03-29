namespace MegaCrit.Sts2.Core.Assets;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class TpSheetRect
{
	public int X { get; set; }

	public int Y { get; set; }

	public int W { get; set; }

	public int H { get; set; }
}
