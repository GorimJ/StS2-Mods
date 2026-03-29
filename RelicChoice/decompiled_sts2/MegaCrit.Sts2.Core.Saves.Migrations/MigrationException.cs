using System;

namespace MegaCrit.Sts2.Core.Saves.Migrations;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class MigrationException : Exception
{
	public MigrationException(string message)
		: base(message)
	{
	}

	public MigrationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
