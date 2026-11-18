using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

/// <summary>Rehydrate: the next Attack you play adds this much precum, then the power is spent.</summary>
public class RehydratePower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Rehydrate", "Your next Attack gains precum.", "Your next Attack gains precum.");

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack || Owner?.Player is null) return;
        if (cardPlay.Card.Owner != Owner.Player) return;

        var player = Owner.Player;
        var r = Reservoirs.Of(player.RunState);
        var added = r.AddPrecum((int)Amount);
        MainFile.Logger.Info($"[Seductress] Rehydrate: +{added} ml precum | mouth {r.Mouth}+{r.Precum}p");
        await PowerCmd.Remove(this);
        await Stomach.AfterPrecumGained(player);
    }
}
