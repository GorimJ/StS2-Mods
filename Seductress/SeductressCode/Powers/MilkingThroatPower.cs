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
    /// <summary>
    /// [Gorim, 2026-11-23] "Once per turn triggers obviously happen twice? And 'your next climax'
    /// becomes 'your next 2 climaxes'? Obviously?" A stacked power does its thing once per stack.
    /// [Claude] Each copy doubles the precum again (2 copies = 4 times as much); the Gulp after
    /// it happens once, since a second Gulp of an empty Mouth does nothing.
    /// </summary>
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Milking Throat",
        "Whenever you gain precum, gain twice as much, then [gold]Gulp[/gold].",
        "Whenever you gain precum, gain twice as much, then [gold]Gulp[/gold].");

    protected override IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> ExtraHoverTips => [UI.Glossary.Tip("GULP")];
}
