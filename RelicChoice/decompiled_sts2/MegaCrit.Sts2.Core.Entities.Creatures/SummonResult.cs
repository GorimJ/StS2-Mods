namespace MegaCrit.Sts2.Core.Entities.Creatures;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class SummonResult
{
	public Creature? Creature { get; init; }

	public decimal Amount { get; init; }

	public SummonResult(Creature? creature, decimal amount)
	{
		Creature = creature;
		Amount = amount;
	}
}
