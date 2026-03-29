namespace MegaCrit.Sts2.Core.Saves.Migrations;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class MigrationPathGapException : InvalidMigrationPathException
{
	public MigrationPathGapException(string message)
		: base(message)
	{
	}
}
