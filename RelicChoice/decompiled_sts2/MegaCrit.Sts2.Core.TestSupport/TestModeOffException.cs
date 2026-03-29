using System;

namespace MegaCrit.Sts2.Core.TestSupport;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class TestModeOffException : Exception
{
	public TestModeOffException()
		: base("Only call this in test mode.")
	{
	}
}
