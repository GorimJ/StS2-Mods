using MegaCrit.Sts2.Core.Achievements;

namespace MegaCrit.Sts2.Core.Platform.Null;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class NullAchievementStrategy : IAchievementStrategy
{
	public void Unlock(Achievement achievement)
	{
	}

	public void Revoke(Achievement achievement)
	{
	}

	public bool IsUnlocked(Achievement achievement)
	{
		return false;
	}
}
