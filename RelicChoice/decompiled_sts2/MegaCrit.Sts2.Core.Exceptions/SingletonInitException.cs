using System;

namespace MegaCrit.Sts2.Core.Exceptions;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class SingletonInitException : Exception
{
	public SingletonInitException()
		: base("The singleton was used before initialization.")
	{
	}
}
