using System;

namespace MegaCrit.Sts2.Core.Saves.Migrations;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class InvalidMigrationPathException : Exception
{
	protected InvalidMigrationPathException(string message)
		: base(message)
	{
	}
}
