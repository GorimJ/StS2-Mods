using System;

namespace MegaCrit.Sts2.Core.Assets;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class AssetLoadException : Exception
{
	public AssetLoadException(string message)
		: base(message)
	{
	}

	public AssetLoadException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
