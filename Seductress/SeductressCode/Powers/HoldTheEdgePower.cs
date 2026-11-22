using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// <i>Hold the Edge</i>, until the start of the owner's next turn: the HP loss that would cause a
/// Climax is prevented, the owner gains 1 Intangible, and that much damage goes to every enemy.
///
/// [Gorim, 2026-11-21] "I played it, climaxed to their attack, and still died. It should be til
/// start of next turn." It used to end with the turn it was played, which was [Claude]'s
/// reading of "this turn" and missed the enemies' attacks.
///
/// Built on the pair of hooks vanilla's Buffer uses: the Late HP-loss modifier, so anything else
/// that reduces the loss goes first, then the follow-up hook for the effects that need awaiting.
/// </summary>
public class HoldTheEdgePower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Hold the Edge",
        "If you would Climax before your next turn, prevent that damage, gain 1 Intangible, and deal that damage to ALL enemies.",
        "If you would Climax before your next turn, prevent that damage, gain 1 Intangible, and deal that damage to ALL enemies.");

    private decimal _prevented;

    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || _prevented > 0m) return amount;
        if (amount <= 0m || amount < Owner.CurrentHp) return amount;   // would not Climax

        _prevented = amount;
        return 0m;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        if (_prevented <= 0m || Owner is null) return;

        var amount = _prevented;
        var ctx = new ThrowingPlayerChoiceContext();
        MainFile.Logger.Info($"[Seductress] Hold the Edge: prevented {amount} HP loss, Intangible, {amount} to all enemies");

        await PowerCmd.Apply<IntangiblePower>(ctx, Owner, 1m, Owner, null);
        var enemies = Owner.CombatState?.Enemies.Where(e => e.IsAlive).ToList() ?? [];
        if (enemies.Count > 0)
        {
            await CreatureCmd.Damage(ctx, enemies, amount, ValueProp.Unpowered, Owner);
        }
        await PowerCmd.Remove(this);
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Player) await PowerCmd.Remove(this);
    }
}
