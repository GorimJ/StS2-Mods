using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// [Gorim] Leg-Locked (design §6.3a) — while an enemy holds this, its death routes its output to
/// your Pussy, whatever killed it. Counts down at the end of its turn.
///
/// It is a claim staked on a kill that has not happened yet, which is why it expires: you are
/// betting on finishing that enemy, not taxing it.
///
/// <b>What it fixes.</b> Routing keys off the finishing card's tag, so a kill delivered by Poison,
/// a Sigil passive, reflect damage or any untagged card falls through to Skin — and Breeding,
/// which needs Womb fill most, is the deck most likely to kill with something that is not an
/// attack. Without this the archetype's own engine leaks its progression into Skin.
///
/// Same shape as <c>DebilitatePower</c>: a Counter that decrements in <c>AfterSideTurnEnd</c> when
/// its owner is among the participants, i.e. at the end of the enemy's own turn.
/// </summary>
public class LegLockedPower : SeductressPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Leg-Locked",
        "When this enemy dies, its output routes to your Pussy whatever killed it. Counts down at the end of its turn.",
        "When this enemy dies, its output routes to your Pussy whatever killed it. Counts down at the end of its turn.");

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Decrement(this);
    }
}
