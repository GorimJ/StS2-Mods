namespace MegaCrit.Sts2.Core.Localization.DynamicVars;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class CalculationBaseVar : DynamicVar
{
	public const string defaultName = "CalculationBase";

	public CalculationBaseVar(decimal baseValue)
		: base("CalculationBase", baseValue)
	{
	}
}
