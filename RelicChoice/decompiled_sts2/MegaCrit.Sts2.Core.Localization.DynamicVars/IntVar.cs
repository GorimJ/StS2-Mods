namespace MegaCrit.Sts2.Core.Localization.DynamicVars;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class IntVar : DynamicVar
{
	public IntVar(string name, decimal amount)
		: base(name, amount)
	{
	}
}
