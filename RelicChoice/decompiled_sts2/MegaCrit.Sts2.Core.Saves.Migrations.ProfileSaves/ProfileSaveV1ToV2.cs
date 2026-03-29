namespace MegaCrit.Sts2.Core.Saves.Migrations.ProfileSaves;

[Migration(typeof(ProfileSave), 1, 2)]
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ProfileSaveV1ToV2 : MigrationBase<ProfileSave>
{
	protected override void ApplyMigration(MigratingData saveData)
	{
	}
}
