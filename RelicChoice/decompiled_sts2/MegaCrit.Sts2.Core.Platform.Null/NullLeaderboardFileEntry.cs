using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MegaCrit.Sts2.Core.Platform.Null;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class NullLeaderboardFileEntry
{
	[JsonPropertyName("name")]
	public required string name;

	[JsonPropertyName("score")]
	public int score;

	[JsonPropertyName("id")]
	public ulong id;

	[JsonPropertyName("other_ids")]
	public List<ulong> userIds = new List<ulong>();
}
