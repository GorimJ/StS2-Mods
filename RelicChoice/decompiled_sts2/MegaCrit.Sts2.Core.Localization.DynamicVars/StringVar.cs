namespace MegaCrit.Sts2.Core.Localization.DynamicVars;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class StringVar : DynamicVar
{
	public string StringValue { get; set; }

	public StringVar(string name, string baseValue = "")
		: base(name, 0m)
	{
		StringValue = baseValue;
	}

	public override string ToString()
	{
		return StringValue;
	}
}
