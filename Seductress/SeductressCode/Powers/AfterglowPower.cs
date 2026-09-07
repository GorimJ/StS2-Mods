using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

public class AfterglowPower : SeductressPower, IClimaxListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Afterglow", "Whenever you Climax, gain Block and draw cards.", "Whenever you Climax, gain Block and draw cards.");

    /// <summary>Block per stack; the card applies 15 (20 upgraded) as the amount.</summary>
    public async Task OnClimax(Creature owner)
    {
        await MegaCrit.Sts2.Core.Commands.CreatureCmd.GainBlock(
            owner, Amount, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move, null);
        await MegaCrit.Sts2.Core.Commands.CardPileCmd.Draw(
            new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(), 2m, owner.Player!);
    }
}
