using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

/// <summary>
/// Ancient. Whenever you play an Attack, gain 1 ml in the hole that Attack used.
///
/// Legal under §3 because it is a Power: Powers may fill. Best in Squeezing, where it is a floor
/// under a bank that bleeds 10% per tagged attack.
/// </summary>
public class EdgingFormPower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Edging Form",
        "Whenever you play an Attack, gain ml in the hole that Attack used.",
        "Whenever you play an Attack, gain ml in the hole that Attack used.");

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack || Owner?.Player is null) return Task.CompletedTask;

        var hole = ClimaxModel.Instance.RouteOf(cardPlay.Card);
        Reservoirs.Of(Owner.Player.RunState).Add(hole, Amount);
        MainFile.Logger.Info($"[Seductress] Edging Form +{Amount} ml -> {hole}");
        return Task.CompletedTask;
    }
}
