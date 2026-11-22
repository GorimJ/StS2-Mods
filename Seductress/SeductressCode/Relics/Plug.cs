using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Cards;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Relics;

/// <summary>
/// The Plug (design §7.3), granted by the 1000 ml Ass threshold rather than drafted.
///
/// While in: the Ass does not leak below 500 ml and leaks only 5% above it, the Ass cannot be
/// filled, and you lose 2 HP entering each floor and 1 HP at the start of each turn. Ass-tagged
/// cards cost 1 more and <b>pull the plug when played</b>, spilling 10% of the Ass on top of their
/// own drain.
///
/// [Claude-Cowork] <b>The spilled 10% counts as ml lost by the card that pulled it</b>, so it
/// grants Strength like any other drain. Without that the relic is incoherent: a plugged Ass card
/// would pay +1 energy <i>and</i> double the drain for exactly the same damage — a downside
/// applied twice, to the archetype whose own cards trigger it. With it, pulling the plug is the
/// archetype's burst moment: one card, double the drain, double the Strength.
///
/// [Claude-Cowork] <b>The plug removes itself</b> rather than needing a manual toggle in combat.
/// It is the only version that makes physical sense, and the +1 energy is the old removal cost
/// folded into the card that triggers it.
///
/// [Claude-Code] <b>Divergence: re-plugging is automatic, not a map toggle.</b> Register entry 28
/// says plugs are "toggled on the map between rooms", which needs UI that does not exist. This
/// re-inserts on entering a new floor, which reproduces the intended rhythm — plugged between
/// fights, unplugged to spend — without a click. The cost is that you cannot choose to stay
/// unplugged through a dry stretch to save the 2 HP a floor, so the relic is slightly more
/// expensive and slightly less controllable than specified.
/// </summary>
public class Plug : SeductressRelic
{
    public const double SpillFraction = 0.10;

    public override RelicRarity Rarity => RelicRarity.Common;

    /// <summary>Never in a reward pool — it arrives from the 1000 ml Ass threshold (§6.1a).</summary>
    // Game 0.113: IsAllowed is no longer virtual; IsAllowedInternal is the hook it calls.
    protected override bool IsAllowedInternal(MegaCrit.Sts2.Core.Runs.IRunState runState) => false;

    public override List<(string, string)>? Localization => new RelicLoc(
        "Plug",
        "While in: your Ass does not leak below 500 ml and leaks 5% above it, and cannot be filled. " +
        "Lose 2 HP entering a floor and 1 HP at the start of each turn. " +
        "Ass cards cost 1 more, pull the Plug, and spill 10% of your Ass — gaining Strength for all of it. " +
        "Re-inserted on the next floor.",
        "You have been holding it in.");

    public override Task AfterObtained()
    {
        Reservoirs.Of(Owner.RunState).PlugIn = true;
        MainFile.Logger.Info("[Seductress] Plug obtained and inserted");
        return Task.CompletedTask;
    }

    /// <summary>2 HP entering a floor, and the plug goes back in.</summary>
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        var r = Reservoirs.Of(Owner.RunState);
        if (!r.PlugIn)
        {
            r.PlugIn = true;
            MainFile.Logger.Info("[Seductress] Plug re-inserted on entering a floor");
        }
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, 2m,
            ValueProp.Unblockable | ValueProp.Unpowered, dealer: null);
    }

    /// <summary>1 HP at the start of each of your turns, but only while it is actually in.</summary>
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != CombatSide.Player) return;
        if (!participants.Contains(Owner.Creature)) return;
        if (!Reservoirs.Of(Owner.RunState).PlugIn) return;

        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m,
            ValueProp.Unblockable | ValueProp.Unpowered, dealer: null);
    }

    /// <summary>Ass-tagged cards cost 1 more while the plug is in.</summary>
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (!Reservoirs.Of(Owner.RunState).PlugIn) return false;
        if (card is not SeductressCard { Hole: Hole.Ass }) return false;

        modifiedCost = originalCost + 1m;
        return true;
    }
}
