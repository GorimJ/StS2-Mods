using MegaCrit.Sts2.Core.Leaderboard;

namespace MegaCrit.Sts2.Core.Platform.Null;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class NullLeaderboardHandle : ILeaderboardHandle
{
	public required NullLeaderboard leaderboard;
}
