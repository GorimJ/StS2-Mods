using System;

namespace MegaCrit.Sts2.Core.Models.Exceptions;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ModelNotFoundException : Exception
{
	public ModelNotFoundException(ModelId id)
		: base($"Model id={id} not found")
	{
	}
}
