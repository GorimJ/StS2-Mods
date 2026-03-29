namespace MegaCrit.Sts2.Core.Saves.Migrations.ProgressSaves;

[Migration(typeof(SerializableProgress), 20, 21)]
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ProgressSaveV20ToV21 : MigrationBase<SerializableProgress>
{
	protected override void ApplyMigration(MigratingData saveData)
	{
	}
}
