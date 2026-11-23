using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Core, Rare Power. Turns Climaxing from pure cost into tempo.
///
/// [Gorim 2026-09-07] Buffed in play. Keeps the burst and adds an engine: each turn, draw 1 and
/// gain 1 Energy per stack of Sensitive. Upgrade now cuts the cost to 0 rather than raising the
/// Block, since the Block was never the reason to play it.
///
/// Diverges from SEDUCTRESS_CARDS.md line 173, which still reads "gain 15 Block and draw 2 /
/// upgrade: 20 Block". The register is Gorim's to update.
/// </summary>
public class Afterglow : SeductressCard
{
    public Afterglow() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    /// <summary>
    /// [Gorim, 2026-11-23, card note] "Should probably have sigil scaling". He took my proposal
    /// [Claude]: 1 = 20 Block, 3 = draw 3, 5 = 30 Block, 7 = the Climax also gives 2 Energy.
    /// Read when the card is played, like the other Powers.
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
        new BlockVar(15m, ValueProp.Move),
        new DynamicVar("Tier1Block", 20m),
        new DynamicVar("Tier3Block", 30m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Afterglow",
        "Whenever you Climax, gain {S3:cond:{Tier3Block:diff()}|{S1:cond:{Tier1Block:diff()}|{Block:diff()}}} [gold]Block[/gold]{S4:cond:, 2 [gold]Energy[/gold]|} and draw {S2:cond:3|2}.\nAt the start of your turn, draw 1 card and gain 1 [gold]Energy[/gold] for each stack of [gold]Sensitive[/gold].\nYour next Climax adds no Lust.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var block = Sigils.At(Owner, Sigils.Tier3) ? DynamicVars["Tier3Block"].BaseValue
                  : Sigils.At(Owner, Sigils.Tier1) ? DynamicVars["Tier1Block"].BaseValue
                  : DynamicVars.Block.BaseValue;
        var power = await PowerCmd.Apply<AfterglowPower>(choiceContext, Owner.Creature, block, Owner.Creature, this);
        if (power is not null)
        {
            if (Sigils.At(Owner, Sigils.Tier2)) power.DrawOnClimax = 3;
            if (Sigils.At(Owner, Sigils.Tier4)) power.EnergyOnClimax = 2;
        }
        Systems.ClimaxModel.Instance.LustSkips++; // [Gorim, 2026-11-23]
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
