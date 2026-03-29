using MegaCrit.Sts2.Core.Leaderboard;
using Steamworks;

namespace MegaCrit.Sts2.Core.Platform.Steam;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class SteamLeaderboardHandle : ILeaderboardHandle
{
	public SteamLeaderboard_t leaderboard;
}
