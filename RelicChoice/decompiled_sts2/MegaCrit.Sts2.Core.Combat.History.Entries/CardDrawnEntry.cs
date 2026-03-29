using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Combat.History.Entries;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class CardDrawnEntry : CombatHistoryEntry
{
	public CardModel Card { get; }

	public bool FromHandDraw { get; }

	public override string Description => base.Actor.Player.Character.Id.Entry + " discarded " + Card.Id.Entry;

	public CardDrawnEntry(CardModel card, int roundNumber, CombatSide currentSide, bool fromHandDraw, CombatHistory history)
		: base(card.Owner.Creature, roundNumber, currentSide, history)
	{
		Card = card;
		FromHandDraw = fromHandDraw;
	}
}
