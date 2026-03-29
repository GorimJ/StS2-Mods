using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Combat.History.Entries;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class OrbChanneledEntry : CombatHistoryEntry
{
	public OrbModel Orb { get; }

	public override string Description => base.Actor.Player.Character.Id.Entry + " channeled " + Orb.Id.Entry;

	public OrbChanneledEntry(OrbModel orb, int roundNumber, CombatSide currentSide, CombatHistory history)
		: base(orb.Owner.Creature, roundNumber, currentSide, history)
	{
		Orb = orb;
	}
}
