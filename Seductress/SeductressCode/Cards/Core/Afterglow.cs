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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(15m, ValueProp.Move)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Afterglow",
        "Whenever you Climax, gain {Block:diff()} [gold]Block[/gold] and draw 2.\nAt the start of your turn, draw 1 card and gain 1 [gold]Energy[/gold] for each stack of [gold]Sensitive[/gold].");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AfterglowPower>(
            choiceContext, Owner.Creature, DynamicVars.Block.BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
