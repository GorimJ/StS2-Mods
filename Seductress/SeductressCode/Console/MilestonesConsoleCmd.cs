using System.Linq;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using Seductress.SeductressCode.UI;

namespace Seductress.SeductressCode.Console;

/// <summary>
/// [Claude] <c>milestones</c>: opens the milestones screen without the mouse, and reports which
/// hover pop-outs each card in the deck would show, so both can be checked from a script.
/// </summary>
public class MilestonesConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "milestones";
    public override string Args => "[demo]";
    public override string Description => "Seductress: open the milestones screen and list each deck card's hover pop-outs.";
    public override bool IsNetworked => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer is null || !RunManager.Instance.IsInProgress)
        {
            return new CmdResult(success: false, "A run does not appear to be in progress.");
        }

        // "milestones demo" shows made-up amounts and leaves the run alone.
        var demo = args.Length > 0 && args[0] == "demo"
            ? new Systems.ReservoirState { Stomach = 620, Womb = 305, Ass = 180, AssHighWater = 260, Skin = 40, Sigils = 4, PregnancyStage = 1 }
            : null;
        var opened = TopBarStrip.Current?.OpenMilestones(demo) ?? false;

        // Building every pop-out proves its text loaded.
        var loaded = Glossary.AllKeys.Select(k => $"{k}='{((MegaCrit.Sts2.Core.HoverTips.HoverTip)Glossary.Tip(k)).Title}'");
        var cards = issuingPlayer.Deck.Cards
            .GroupBy(c => c.GetType())
            .Select(g => $"{g.Key.Name}: {string.Join(",", g.First().HoverTips.Select(t => t.Id.Split('.').Last().Replace(" and Description=static_hover_tips", "")))}");

        return new CmdResult(success: true,
            $"screen {(opened ? "opened" : "not opened")} | tips {string.Join(" ", loaded)} | {string.Join(" ; ", cards)}");
    }
}
