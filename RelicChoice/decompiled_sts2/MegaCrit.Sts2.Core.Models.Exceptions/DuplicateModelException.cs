using System;

namespace MegaCrit.Sts2.Core.Models.Exceptions;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class DuplicateModelException : Exception
{
	public DuplicateModelException(Type t)
		: base($"Trying to create a duplicate canonical model of type {t}. Don't call constructors on models! Use ModelDb instead.")
	{
	}
}
