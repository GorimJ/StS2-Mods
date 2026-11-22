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
///   reservoir womb 250        set the Womb (conception resolves at the next end of combat)
///   reservoir precum 20       set the precum held in the Mouth
///   reservoir all 100         set every ml reservoir to 100
/// </code>
///
/// Filling by playing is slow and self-defeating: the per-floor leak and the per-attack drain mean
/// the bank falls while you get there, and hunting encounters costs a floor of leak per roll.
///
/// <b>Note when reading a log:</b> <c>reservoir ass 300</c> trips <b>every</b> Ass threshold at
/// once, because the unlocks read the high-water mark and this moves it. Fine for testing the
/// rewards; useless for testing whether they are reachable.
///
/// Delete alongside <see cref="SigilsConsoleCmd"/> and
/// <see cref="Seductress.SeductressCode.Relics.SigilScaffold"/>.
///
/// The game auto-registers any AbstractConsoleCmd subtype found in a mod assembly.
///
/// ==================================================================================================
/// </summary>
public class ReservoirConsoleCmd : AbstractConsoleCmd
{
    private static readonly string[] Names =
        ["mouth", "precum", "ass", "womb", "skin", "stomach", "all"];

    public override string CmdName => "reservoir";
    public override string Args => "[mouth|precum|ass|womb|skin|stomach|all] [amount:int]";
    public override string Description => "TEST SCAFFOLD: show or set the Seductress reservoirs (ml).";
    public override bool IsNetworked => true;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer is null || !RunManager.Instance.IsInProgress)
        {
            return new CmdResult(success: false, "A run does not appear to be in progress.");
        }

        var r = Reservoirs.Of(issuingPlayer.RunState);

        if (args.Length == 0) return new CmdResult(success: true, Describe(r));

        var name = args[0].ToLowerInvariant();
        if (!Names.Contains(name))
        {
            return new CmdResult(success: false, "Unknown reservoir '" + name + "'. Valid: " + string.Join(", ", Names));
        }
        if (args.Length < 2 || !int.TryParse(args[1], out var amount) || amount < 0)
        {
            return new CmdResult(success: false, "Usage: reservoir <" + string.Join("|", Names) + "> <non-negative amount>");
        }

        // Set past the design capacity deliberately — Ass inflation above its cap and Womb stages
        // above the first are both real states and both need to be testable.
        switch (name)
        {
            case "mouth": r.Mouth = amount; break;
            case "precum": r.Precum = amount; break;
            case "ass": r.Ass = amount; r.TouchAssHighWater(); break;
            case "womb": r.Womb = amount; break;
            case "skin": r.Skin = amount; break;
            case "stomach": r.Stomach = amount; break;
            case "all":
                r.Mouth = r.Ass = r.Womb = r.Skin = amount;
                r.TouchAssHighWater();
                break;
        }

        MainFile.Logger.Info($"[Seductress] SCAFFOLD: reservoir {name} set to {amount}");
        return new CmdResult(success: true, $"Set {name} to {amount}. Now: {Describe(r)}");
    }

    private static string Describe(ReservoirState r) =>
        $"mouth {r.Mouth}+{r.Precum}p/{ReservoirState.MouthCap} · ass {r.Ass}/{ReservoirState.AssCap} (hw {r.AssHighWater}) · womb {r.Womb} " +
        $"· skin {r.Skin} · stomach {r.Stomach} · sigils {r.Sigils} " +
        $"· pregnant S{r.PregnancyStage} · lactating S{r.LactatingStage}{(r.LactationTail ? " tail" : "")} " +
        $"· plug {(r.PlugIn ? "in" : "out")}";

    public override CompletionResult GetArgumentCompletions(Player? player, string[] args)
    {
        if (args.Length <= 1)
        {
            return CompleteArgument(Names.ToList(), Array.Empty<string>(), args.FirstOrDefault() ?? "");
        }
        return new CompletionResult { Type = CompletionType.Argument, ArgumentContext = CmdName };
    }
}
