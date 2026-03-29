namespace MegaCrit.Sts2.Core.Localization.DynamicVars;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class CalculationExtraVar : DynamicVar
{
	public const string defaultName = "CalculationExtra";

	public CalculationExtraVar(decimal baseValue)
		: base("CalculationExtra", baseValue)
	{
	}
}
