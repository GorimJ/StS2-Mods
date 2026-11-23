using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// [Gorim, 2026-11-23, card note on Ward] "Reflecting damage is currently permanent, doesn't fade
/// end of turn." [Claude] Ward gives Thorns and this marker for the same amount; at the start of
/// the owner's next turn the marker takes that much Thorns back off. Start of next turn rather
/// than end of this one, so the reflect is still up while the enemies attack.
/// </summary>
public class WardReflectPower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Ward",
        "Your reflected damage from Ward fades at the start of your next turn.",
        "Lose {Amount} [gold]Thorns[/gold] at the start of your next turn.");

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player || Owner is null) return;
        var lose = System.Math.Min(Amount, Owner.GetPowerAmount<ThornsPower>());
        if (lose > 0) await PowerCmd.Apply<ThornsPower>(choiceContext, Owner, -lose, Owner, null);
        await PowerCmd.Remove(this);
    }
}
