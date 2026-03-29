namespace MegaCrit.Sts2.Core.Saves.Migrations.SettingsSaves;

[Migration(typeof(SettingsSave), 3, 4)]
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class SettingsSaveV3ToV4 : MigrationBase<SettingsSave>
{
	protected override void ApplyMigration(MigratingData saveData)
	{
	}
}
