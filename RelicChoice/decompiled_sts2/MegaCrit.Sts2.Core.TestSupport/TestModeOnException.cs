using System;

namespace MegaCrit.Sts2.Core.TestSupport;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class TestModeOnException : Exception
{
	public TestModeOnException()
		: base("Never call this in test mode.")
	{
	}
}
