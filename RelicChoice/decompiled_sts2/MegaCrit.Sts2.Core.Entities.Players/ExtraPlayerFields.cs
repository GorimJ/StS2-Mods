using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Entities.Players;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ExtraPlayerFields
{
	public int CardShopRemovalsUsed { get; set; }

	public int WongoPoints { get; set; }

	public SerializableExtraPlayerFields ToSerializable()
	{
		return new SerializableExtraPlayerFields
		{
			CardShopRemovalsUsed = CardShopRemovalsUsed,
			WongoPoints = WongoPoints
		};
	}

	public static ExtraPlayerFields FromSerializable(SerializableExtraPlayerFields save)
	{
		return new ExtraPlayerFields
		{
			CardShopRemovalsUsed = save.CardShopRemovalsUsed,
			WongoPoints = save.WongoPoints
		};
	}
}
