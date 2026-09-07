using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

public class RestraintPower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Restraint", "While your HP is above half, gain Block at the start of your turn.", "While your HP is above half, gain Block at the start of your turn.");

    public override async Task AfterSideTurnStart(MegaCrit.Sts2.Core.Combat.CombatSide side,
        IReadOnlyList<Creature> participants, MegaCrit.Sts2.Core.Combat.ICombatState combatState)
    {
        if (side != MegaCrit.Sts2.Core.Combat.CombatSide.Player) return;
        if (Owner is null || Owner.CurrentHp * 2 <= Owner.MaxHp) return;
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.GainBlock(
            Owner, Amount, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move, null);
    }
}
