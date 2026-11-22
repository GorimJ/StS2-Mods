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

/// <summary>Milking Throat. The work is in <see cref="Stomach.AfterPrecumGained"/>; It only has to be present.</summary>
public class MilkingThroatPower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Milking Throat",
        "Whenever you gain precum, gain twice as much, then [gold]Gulp[/gold].",
        "Whenever you gain precum, gain twice as much, then [gold]Gulp[/gold].");

    protected override IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> ExtraHoverTips => [UI.Glossary.Tip("GULP")];
}
