using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Console;

/// <summary>
/// ============================ TEST SCAFFOLDING — REMOVE BEFORE RELEASE ============================
///
/// <c>reservoir</c> — read the reservoirs, or set one directly.
///
/// <code>
///   reservoir                 list current values
///   reservoir ass 200         set the Ass bank to 200 ml
///   reservoir all 100         set every ml reservoir to 100
/// </code>
///
/// Filling a reservoir by playing is slow and self-defeating: reaching 200 ml of Ass takes about
/// twenty kills routed to Ass, and the per-floor leak plus the per-attack Ass drain means the bank
/// falls while you get there. Hunting encounters with <c>room monster</c> costs a floor of leak per
/// roll on top. Without this, testing the Ass rule at a full bank is impractical.
///
/// Delete alongside <see cref="SigilsConsoleCmd"/> and
/// <see cref="Seductress.SeductressCode.Relics.SigilScaffold"/>.
///
/// The game auto-registers any AbstractConsoleCmd subtype found in a mod assembly, so there is
/// nothing to wire up.
///
/// ==================================================================================================
/// </summary>
public class ReservoirConsoleCmd : AbstractConsoleCmd
{
    private static readonly string[] Names = ["mouth", "ass", "womb", "glazed", "stomach", "all"];

    public override string CmdName => "reservoir";
    public override string Args => "[mouth|ass|womb|glazed|stomach|all] [amount:int]";
    public override string Description => "TEST SCAFFOLD: show or set the Seductress reservoirs (ml).";
    public override bool IsNetworked => true;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer is null || !RunManager.Instance.IsInProgress)
        {
            return new CmdResult(success: false, "A run does not appear to be in progress.");
        }

        var r = Reservoirs.Of(issuingPlayer.RunState);

        if (args.Length == 0)
        {
            return new CmdResult(success: true,
                $"mouth {r.Mouth}/50 · ass {r.Ass}/200 · womb {r.Womb}/100 · " +
                $"glazed {r.Glazed} · stomach {r.Stomach} · sigils {r.Sigils}");
        }

        var name = args[0].ToLowerInvariant();
        if (!Names.Contains(name))
        {
            return new CmdResult(success: false, "Unknown reservoir '" + name + "'. Valid: " + string.Join(", ", Names));
        }
        if (args.Length < 2 || !int.TryParse(args[1], out var amount) || amount < 0)
        {
            return new CmdResult(success: false, "Usage: reservoir <" + string.Join("|", Names) + "> <non-negative amount>");
        }

        // Set past the design capacity deliberately — inflation above 200 Ass is a real state
        // (§6.1) and needs to be testable.
        switch (name)
        {
            case "mouth": r.Mouth = amount; break;
            case "ass": r.Ass = amount; break;
            case "womb": r.Womb = amount; break;
            case "glazed": r.Glazed = amount; break;
            case "stomach": r.Stomach = amount; break;
            case "all":
                r.Mouth = r.Ass = r.Womb = r.Glazed = amount;
                break;
        }

        MainFile.Logger.Info($"[Seductress] SCAFFOLD: reservoir {name} set to {amount}");
        return new CmdResult(success: true,
            $"Set {name} to {amount}. Now: mouth {r.Mouth} · ass {r.Ass} · womb {r.Womb} · glazed {r.Glazed} · stomach {r.Stomach}");
    }

    public override CompletionResult GetArgumentCompletions(Player? player, string[] args)
    {
        if (args.Length <= 1)
        {
            return CompleteArgument(Names.ToList(), Array.Empty<string>(), args.FirstOrDefault() ?? "");
        }
        return new CompletionResult { Type = CompletionType.Argument, ArgumentContext = CmdName };
    }
}
