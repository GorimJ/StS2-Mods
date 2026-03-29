namespace MegaCrit.Sts2.Core.Saves.Migrations.PrefsSaves;

[Migration(typeof(PrefsSave), 1, 2)]
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class PrefsSaveV1ToV2 : MigrationBase<PrefsSave>
{
	protected override void ApplyMigration(MigratingData saveData)
	{
	}
}
