using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// The Climax payoff, in two halves: a burst when you Climax, and an engine that runs for the
/// rest of the fight scaled by <see cref="SensitivePower"/>.
///
/// [Gorim 2026-09-07, found in play] "Afterglow needs a buff." The burst alone was a Rare that
/// paid out at the one moment it was least useful — you have just reset to zero arousal, so the
/// Block lands when you are furthest from needing it.
///
/// [Claude-Code] Worth being explicit that the engine does not widen the trigger. Sensitive is
/// gained only by Climaxing and is cleared at end of combat, so the engine is live under exactly
/// the same condition the burst always had. What changed is <i>when</i> the value arrives: spread
/// across the turns where you are rebuilding rather than dumped on the turn you reset.
///
/// [Gorim] Rate is 1 draw <b>and</b> 1 Energy per stack, not per two stacks. Deliberately steep:
/// at 3 Sensitive you are taking +150% arousal, so every hit is close to lethal, and the payoff
/// is meant to match that position rather than hedge against it.
/// </summary>
public class AfterglowPower : SeductressPower, IClimaxListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Afterglow",
        "Whenever you Climax, gain Block and draw cards. Each turn, draw a card and gain 1 Energy per stack of Sensitive.",
        "Whenever you Climax, gain Block and draw cards. Each turn, draw a card and gain 1 Energy per stack of Sensitive.");

    /// <summary>Stacks of Sensitive on the owner — i.e. Climaxes so far this combat.</summary>
    private int SensitiveStacks => Owner.GetPowerAmount<SensitivePower>();

    /// <summary>
    /// [Gorim, 2026-11-23] Powers stack. Copies played; the turn engine runs once per copy. The
    /// Block (the power's amount) and the Climax draw and Energy add up per copy as well.
    /// </summary>
    public int Copies { get; set; }

    private int Engine => SensitiveStacks * System.Math.Max(1, Copies);

    /// <summary>The burst. Block per stack; the card applies 15 as the amount.</summary>
    public async Task OnClimax(Creature owner)
    {
        await CreatureCmd.GainBlock(owner, Amount, ValueProp.Move, null);
        if (EnergyOnClimax > 0) await PlayerCmd.GainEnergy(EnergyOnClimax, owner.Player!);
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), DrawOnClimax, owner.Player!);
    }

    /// <summary>Cards drawn on each Climax: 2, or 3 from the card's 3-Sigil stage. Set by the card.</summary>
    public int DrawOnClimax { get; set; }

    /// <summary>Energy on each Climax: 0, or 2 at the card's 7-Sigil stage. Set by the card.</summary>
    public int EnergyOnClimax { get; set; }

    /// <summary>
    /// The engine's draw half. Routed through ModifyHandDraw rather than drawing by hand at turn
    /// start, so it composes with everything else that adjusts the draw — the same hook
    /// <c>DrawCardsNextTurnPower</c> uses.
    /// </summary>
    public override decimal ModifyHandDraw(Player player, decimal count) =>
        player == Owner.Player ? count + Engine : count;

    /// <summary>
    /// The engine's energy half. AfterEnergyReset fires every turn once energy has been restored
    /// to its base, which is where <c>EnergyNextTurnPower</c> adds its own — the difference being
    /// that this one does not remove itself, so it keeps paying for the whole combat.
    /// </summary>
    public override async Task AfterEnergyReset(Player player)
    {
        if (player != Owner.Player) return;
        var stacks = Engine;
        if (stacks > 0) await PlayerCmd.GainEnergy(stacks, player);
    }
}
