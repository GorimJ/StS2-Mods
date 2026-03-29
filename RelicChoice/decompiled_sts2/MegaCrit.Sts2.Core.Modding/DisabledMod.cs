using System.Text.Json.Serialization;

namespace MegaCrit.Sts2.Core.Modding;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class DisabledMod
{
	[JsonPropertyName("name")]
	public string Name { get; set; } = "";

	[JsonPropertyName("source")]
	public ModSource Source { get; set; }

	public DisabledMod()
	{
	}

	public DisabledMod(Mod mod)
	{
		Name = mod.pckName;
		Source = mod.modSource;
	}
}
