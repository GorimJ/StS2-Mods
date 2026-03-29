namespace MegaCrit.Sts2.Core.Saves.Migrations.SerializableRuns;

[Migration(typeof(SerializableRun), 12, 13)]
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class SerializableRunV12ToV13 : MigrationBase<SerializableRun>
{
	protected override void ApplyMigration(MigratingData saveData)
	{
	}
}
