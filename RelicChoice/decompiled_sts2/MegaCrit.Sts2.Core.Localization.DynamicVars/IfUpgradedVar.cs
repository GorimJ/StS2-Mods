namespace MegaCrit.Sts2.Core.Localization.DynamicVars;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class IfUpgradedVar : DynamicVar
{
	public const string defaultName = "IfUpgraded";

	public UpgradeDisplay upgradeDisplay;

	public IfUpgradedVar(UpgradeDisplay upgradeDisplay)
		: base("IfUpgraded", (int)upgradeDisplay)
	{
		this.upgradeDisplay = upgradeDisplay;
	}

	public IfUpgradedVar(string name, decimal amount)
		: base(name, amount)
	{
	}
}
