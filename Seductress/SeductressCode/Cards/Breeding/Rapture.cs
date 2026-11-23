using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
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
/// Breeding Rare Power. Base Block moved off the Climax and onto casting, so the card does
/// something the turn you play it rather than sitting inert — which matters on a 3-cost Rare.
/// Tier 1 then puts Block back on the Climax as well.
///
/// The energy is on the Climax, not on play, so a Climax you cause refunds tempo and one the enemy
/// causes gives only Strength and Block. Bespoke tier 4 (doubled energy) by choice.
/// </summary>
public class Rapture : SeductressCard
{
    public Rapture() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
        new BlockVar(10m, ValueProp.Move),
        new DynamicVar("TierBlock", 20m),
        new DynamicVar("Strength", 2m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Rapture",
        "Gain {S3:cond:{TierBlock:diff()}|{Block:diff()}} [gold]Block[/gold].\nWhenever you Climax, gain {S4:cond:6|3} [gold]Energy[/gold]{S1:cond:,| and} {Strength:diff()} [gold]Strength[/gold]{S1:cond: and {S3:cond:{TierBlock:diff()}|{Block:diff()}} [gold]Block[/gold]|}.{S2:cond:{Live:cond:|\nCosts 1 less.}|}\nYour next Climax adds no Lust.");

    /// <summary>Tier 2 reduces the cost, which also lowers what tier 4 would repeat — see §7.1.</summary>
    private void RefreshCost() =>
        SigilDiscount(Sigils.Tier2);

    public override Task AfterCardEnteredCombat(MegaCrit.Sts2.Core.Models.CardModel card)
    {
        if (ReferenceEquals(card, this)) RefreshCost();
        return Task.CompletedTask;
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext,
        MegaCrit.Sts2.Core.Models.CardModel card, bool fromHandDraw)
    {
        if (ReferenceEquals(card, this)) RefreshCost();
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var block = Sigils.At(Owner, Sigils.Tier3) ? DynamicVars["TierBlock"].BaseValue : DynamicVars.Block.BaseValue;
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);

        var power = await PowerCmd.Apply<RapturePower>(
            choiceContext, Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
        ClimaxModel.Instance.LustSkips++; // [Gorim, 2026-11-23]
        if (power is null) return;

        // [Gorim, 2026-11-23] copies stack: each adds its own Block and Energy on Climax.
        power.BlockOnClimax += Sigils.At(Owner, Sigils.Tier1) ? (int)block : 0;
        power.EnergyOnClimax += Sigils.At(Owner, Sigils.Tier4) ? 6 : 3;
    }

    protected override void OnUpgrade() => DynamicVars["Strength"].UpgradeValueBy(1m);
}
