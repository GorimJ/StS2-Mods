using System.Collections.Generic;

namespace MegaCrit.Sts2.Core.Assets;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class TpSheetData
{
	public List<TpSheetTexture> Textures { get; set; } = new List<TpSheetTexture>();
}
