using System.Collections.Generic;

namespace MegaCrit.Sts2.Core.Leaderboard;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class LeaderboardEntry
{
	public int rank;

	public required string name;

	public ulong id;

	public int score;

	public List<ulong> userIds = new List<ulong>();
}
