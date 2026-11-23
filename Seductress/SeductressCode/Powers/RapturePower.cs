using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

public class RapturePower : SeductressPower, IClimaxListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Rapture", "Whenever you Climax, gain energy, Strength and Block.", "Whenever you Climax, gain energy, Strength and Block.");

    /// <summary>Block granted on each Climax. 0 until the card's tier 1. Set by the card.</summary>
    public int BlockOnClimax { get; set; }

    /// <summary>Energy on each Climax — 3, or 6 at the card's tier 4. Set by the card.</summary>
    public int EnergyOnClimax { get; set; }

    /// <summary>Strength per stack; Block and energy are configured by the card's tiers.</summary>
    public async Task OnClimax(Creature owner)
    {
        var ctx = new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext();
        await MegaCrit.Sts2.Core.Commands.PlayerCmd.GainEnergy(EnergyOnClimax, owner.Player!);
        await MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.StrengthPower>(
            ctx, owner, Amount, owner, null);
        if (BlockOnClimax > 0)
        {
            await MegaCrit.Sts2.Core.Commands.CreatureCmd.GainBlock(
                owner, BlockOnClimax, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move, null);
        }
    }
}
