using System;

namespace MegaCrit.Sts2.Core.Exceptions;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class SaveException : Exception
{
	public SaveException(string message)
		: base(message)
	{
	}
}
