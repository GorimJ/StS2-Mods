using System.Reflection;

namespace MegaCrit.Sts2.Core.Modding;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class Mod
{
	public ModSource modSource;

	public required string pckName;

	public bool wasLoaded;

	public ModManifest? manifest;

	public Assembly? assembly;

	public bool? assemblyLoadedSuccessfully;
}
