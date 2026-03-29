namespace MegaCrit.Sts2.Core.Localization.DynamicVars;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class HealVar : DynamicVar
{
	public const string defaultName = "Heal";

	public HealVar(decimal healAmount)
		: base("Heal", healAmount)
	{
	}

	public HealVar(string name, decimal healAmount)
		: base(name, healAmount)
	{
	}
}
