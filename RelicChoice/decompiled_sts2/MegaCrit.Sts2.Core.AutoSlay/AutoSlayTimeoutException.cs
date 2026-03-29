using System;

namespace MegaCrit.Sts2.Core.AutoSlay;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class AutoSlayTimeoutException : TimeoutException
{
	public AutoSlayTimeoutException(string message)
		: base(message)
	{
	}
}
