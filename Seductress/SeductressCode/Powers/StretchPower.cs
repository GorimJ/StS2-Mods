using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// Stretch My Hole Daddy: attacks that cost you HP hit one additional time.
///
/// [Gorim, 2026-11-23] "All attacks that lose hp should be counted ideally. Including attacks
/// from other characters that lose hp. Can we not check generically?" and earlier the same day,
/// an attack still counts "even if the hp loss is prevented/removed" (No Gag Reflex).
///
/// [Claude] The test is the card, not what happened on this play: an Attack whose variables
/// include an HP-loss variable (<see cref="HpLossVar"/>) above 0. Every base-game card that
/// costs you HP declares one (Hemokinesis and Breakthrough are the attacks), as does every one of
/// ours, and No Gag Reflex leaves the variable alone, so it still counts. Watching for HP actually
/// lost would miss prevented losses and comes too late, since the loss usually follows the hits.
/// Misses: a card from another mod that costs HP without declaring the variable.
///
/// The extra hit rides the game's own hit-count hook, so it works on attacks whose code we do
/// not own. A card that attacks in several separate commands gets the extra hit once per play,
/// on its first command.
/// </summary>
public class StretchPower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    /// <summary>[Gorim, 2026-11-23] Powers stack: each copy adds one more hit.</summary>
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Stretch My Hole Daddy", "Attacks that cost you HP hit additional times.", "Attacks that cost you HP hit {Amount} additional time(s).");

    private CardPlay? _lastBoosted;

    public static bool CostsHp(CardModel card) =>
        card.Type == CardType.Attack
        && card.DynamicVars.Values.Any(v => v is HpLossVar && v.BaseValue > 0m);

    public override int ModifyAttackHitCount(AttackCommand attack, int hitCount)
    {
        if (Owner is null || attack.Attacker != Owner) return hitCount;
        if (attack.ModelSource is not CardModel card || attack.CardPlay is not { } play) return hitCount;
        if (ReferenceEquals(play, _lastBoosted) || !CostsHp(card)) return hitCount;
        _lastBoosted = play;
        return hitCount + (int)Amount;
    }
}
