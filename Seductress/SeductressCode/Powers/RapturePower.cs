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
        "Rapture", "Whenever you Climax, gain energy, Strength and Block.",
        "Whenever you Climax, gain {Energy} [gold]Energy[/gold], {Strength} [gold]Strength[/gold] and {Block} [gold]Block[/gold].");

    protected override IEnumerable<MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar> CanonicalVars =>
        [new MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar("Energy", 0m),
         new MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar("Strength", 0m),
         new MegaCrit.Sts2.Core.Localization.DynamicVars.DynamicVar("Block", 0m)];

    /// <summary>
    /// [Gorim, 2026-11-23] "When you play it twice it should show 2 stacks of the buff." The
    /// amount is the copies played; each copy adds its Energy, Strength and Block here.
    /// </summary>
    public void AddCopy(int energy, int strength, int block)
    {
        EnergyOnClimax += energy;
        StrengthOnClimax += strength;
        BlockOnClimax += block;
        DynamicVars["Energy"].BaseValue = EnergyOnClimax;
        DynamicVars["Strength"].BaseValue = StrengthOnClimax;
        DynamicVars["Block"].BaseValue = BlockOnClimax;
    }

    public int StrengthOnClimax { get; private set; }

    /// <summary>Block granted on each Climax. 0 until the card's tier 1. Set by the card.</summary>
    public int BlockOnClimax { get; private set; }

    /// <summary>Energy on each Climax — 3, or 6 at the card's tier 4. Set by the card.</summary>
    public int EnergyOnClimax { get; private set; }

    /// <summary>Strength per stack; Block and energy are configured by the card's tiers.</summary>
    public async Task OnClimax(Creature owner)
    {
        var ctx = new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext();
        await MegaCrit.Sts2.Core.Commands.PlayerCmd.GainEnergy(EnergyOnClimax, owner.Player!);
        await MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.StrengthPower>(
            ctx, owner, StrengthOnClimax, owner, null);
        if (BlockOnClimax > 0)
        {
            await MegaCrit.Sts2.Core.Commands.CreatureCmd.GainBlock(
                owner, BlockOnClimax, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move, null);
        }
    }
}
