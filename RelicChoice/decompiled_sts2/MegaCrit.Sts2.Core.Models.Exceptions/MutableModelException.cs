using System;

namespace MegaCrit.Sts2.Core.Models.Exceptions;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class MutableModelException : Exception
{
	public MutableModelException(Type t)
		: base($"Mutable model of type {t} used in incorrect place.")
	{
	}
}
