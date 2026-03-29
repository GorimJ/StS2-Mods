namespace MegaCrit.Sts2.Core.Assets;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class TpSheetSprite
{
	public string Filename { get; set; } = "";

	public TpSheetRect Region { get; set; } = new TpSheetRect();

	public TpSheetRect Margin { get; set; } = new TpSheetRect();
}
