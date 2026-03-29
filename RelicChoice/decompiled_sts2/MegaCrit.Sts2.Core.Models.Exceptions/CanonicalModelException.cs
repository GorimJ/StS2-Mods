using System;

namespace MegaCrit.Sts2.Core.Models.Exceptions;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class CanonicalModelException : Exception
{
	public CanonicalModelException(Type t)
		: base($"Canonical model of type {t} used in incorrect place.")
	{
	}
}
