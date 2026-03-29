namespace MegaCrit.Sts2.Core.Localization.DynamicVars;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class StarsVar : DynamicVar
{
	public const string defaultName = "Stars";

	public StarsVar(int stars)
		: this("Stars", stars)
	{
	}

	public StarsVar(string name, int stars)
		: base(name, stars)
	{
	}
}
