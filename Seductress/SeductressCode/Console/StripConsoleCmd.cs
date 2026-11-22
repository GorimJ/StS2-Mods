using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Console;

/// <summary>
/// [Claude] <c>strip [0-3]</c>: sets how much the combat rig has taken off, to look at each stage
/// without playing Powers. Appearance only; it resets at the next combat like the real count.
/// </summary>
public class StripConsoleCmd : AbstractConsoleCmd
{
    public override string CmdName => "strip";
    public override string Args => "[0-3]";
    public override string Description => "Seductress: show or set the combat rig's strip stage.";
    public override bool IsNetworked => false;

    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (args.Length > 0 && int.TryParse(args[0], out var stage))
        {
            ClimaxModel.Instance.Stripped = Math.Clamp(stage, 0, ClimaxModel.MaxStripped);
        }
        return new CmdResult(success: true, $"strip stage {ClimaxModel.Instance.Stripped}");
    }
}
