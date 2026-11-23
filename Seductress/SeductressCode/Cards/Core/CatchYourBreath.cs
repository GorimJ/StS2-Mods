using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Core Common: the neutral Block card. [Claude draft, 2026-11-14, as "6 Block, draw 1"; Gorim's
/// rework note, 2026-11-17: "don't like the base block drawing ... Something like Block 6 eat 3".]
/// </summary>
public class CatchYourBreath : SeductressCard
{
    public CatchYourBreath() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Move),
        new DynamicVar("Ml", 20m) // [Gorim, 2026-11-23] eat amounts round up to tens (was 15)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Catch Your Breath", "Gain {Block:diff()} [gold]Block[/gold]. Eat {Ml:diff()}.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await Stomach.Eat(Owner, (int)DynamicVars["Ml"].BaseValue, "Catch Your Breath");
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
