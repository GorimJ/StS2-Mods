namespace MegaCrit.Sts2.Core.Localization.DynamicVars;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class CardsVar : DynamicVar
{
	public const string defaultName = "Cards";

	public CardsVar(int cards)
		: base("Cards", cards)
	{
	}

	public CardsVar(string name, int cards)
		: base(name, cards)
	{
	}
}
