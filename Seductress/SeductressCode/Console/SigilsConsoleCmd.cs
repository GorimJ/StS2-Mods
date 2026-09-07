using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Console;

/// <summary>
/// ============================ TEST SCAFFOLDING — DELETE WITH CONCEPTION ============================
///
/// <c>sigils &lt;n&gt;</c> — set the Sigil count directly.
///
/// The only practical way to exercise Breeding's tier-2 and tier-3 gates, which otherwise need a
/// full run of pregnancies to reach. Delete alongside
/// <see cref="Seductress.SeductressCode.Relics.SigilScaffold"/> when Conception lands.
///
/// The game auto-registers any AbstractConsoleCmd subtype found in a mod assembly
/// (<c>ReflectionHelper.GetSubtypesInMods</c>), so there is nothing to wire up.
///
/// ==================================================================================================
/// </summary>
public class SigilsConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "sigils";
    public override string Args => "<count:int>";
    public override string Description => "TEST SCAFFOLD: set the Seductress Sigil count.";
    public override bool IsNetworked => true;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (issuingPlayer is null || !RunManager.Instance.IsInProgress)
        {
            return new CmdResult(success: false, "A run does not appear to be in progress.");
        }
        if (args.Length == 0 || !int.TryParse(args[0], out var count) || count < 0)
        {
            return new CmdResult(success: false, "Usage: sigils <non-negative count>");
        }

        Reservoirs.Of(issuingPlayer.RunState).Sigils = count;
        MainFile.Logger.Info($"[Seductress] SCAFFOLD: sigils set to {count}");
        return new CmdResult(success: true, $"Sigils set to {count}.");
    }
}
