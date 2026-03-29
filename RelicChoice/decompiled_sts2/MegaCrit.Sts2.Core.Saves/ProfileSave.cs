using System.Text.Json.Serialization;

namespace MegaCrit.Sts2.Core.Saves;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ProfileSave : ISaveSchema
{
	[JsonPropertyName("schema_version")]
	public int SchemaVersion { get; set; }

	[JsonPropertyName("last_profile_id")]
	public int LastProfileId { get; set; } = 1;
}
