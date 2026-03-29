using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace MegaCrit.Sts2.Core.Combat.History.Entries;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class CreatureAttackedEntry : CombatHistoryEntry
{
	public IReadOnlyList<DamageResult> DamageResults { get; }

	public override string Description => base.Actor.Name + " attacked";

	public CreatureAttackedEntry(Creature attacker, IReadOnlyList<DamageResult> damageResults, int roundNumber, CombatSide currentSide, CombatHistory history)
		: base(attacker, roundNumber, currentSide, history)
	{
		DamageResults = damageResults;
	}
}
