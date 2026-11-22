using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Runs;
using Seductress.SeductressCode.Cards;

namespace Seductress.SeductressCode.Console;

/// <summary>
/// [Claude] <c>sigilcard</c>: opens the right-click card view on every card whose text changes
/// with Sigils, and prints each one's text at 0, 1, 3, 5 and 7 Sigils. Read-only: the cards are
/// made for the view and never join a pile, so the run is not changed.
/// </summary>
public class SigilCardConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "sigilcard";
    public override string Args => "[card id] [sigils]";
    public override string Description => "Seductress: show Sigil cards in the card view and print their text at each tier.";
    public override bool IsNetworked => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer is null || !RunManager.Instance.IsInProgress)
        {
            return new CmdResult(success: false, "A run does not appear to be in progress.");
        }

        var scope = RunManager.Instance.DebugOnlyGetState();
        var wanted = args.Length > 0 ? args[0].ToUpperInvariant() : null;
        var cards = ModelDb.AllCards
            .Where(c => c is SeductressCard { HasSigilTiers: true } && (wanted is null || c.Id.Entry == wanted))
            .Select(c => scope.CreateCard(c, issuingPlayer))
            .ToList();
        if (cards.Count == 0) return new CmdResult(success: false, "No Sigil card matched.");

        var lines = new List<string>();
        foreach (var card in cards.Cast<SeductressCard>())
        {
            foreach (var n in new[] { 0, 1, 3, 5, 7 })
            {
                card.SigilPreview = n;
                lines.Add($"{card.Id.Entry}@{n}: {card.GetDescriptionForPile(PileType.None).Replace("\n", " / ")}");
            }
            card.SigilPreview = null;
        }

        var screen = NGame.Instance?.GetInspectCardScreen();
        screen?.Open(cards, 0);
        if (args.Length > 1 && int.TryParse(args[1], out var show))
        {
            screen?.GetNodeOrNull<UI.SigilSlider>("SigilSlider")?.Set(show);
        }
        return new CmdResult(success: true, string.Join("\n", lines));
    }
}
