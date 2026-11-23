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
/// Breeding Rare Power. [Gorim, 2026-11-17, reworking the Full Term draft] "You get *bred* twice
/// as fast (You gain twice as much pussy cum). Your pussy attacks deal 1% more damage per enemy
/// missing health. 2 Cost."
///
/// [Claude] Read as +1% per point of HP the target is missing. The upgrade (cost 1) is mine.
/// </summary>
public class Hyperfertile : SeductressCard
{
    public Hyperfertile() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

    /// <summary>
    /// [Gorim, 2026-11-23, card note] "This should have sigil effects". He took my proposal
    /// [Claude]: 1 = 2% per missing HP, 3 = costs 1 less, 5 = 3% per missing HP, 7 = three times
    /// as much Pussy cum. A second copy adds nothing (his other question): it never quadruples.
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Hyperfertile",
        "Gain {S4:cond:three times|twice} as much Pussy cum. Your Pussy attacks deal {S3:cond:3|{S1:cond:2|1}}% more damage for each HP the enemy is missing.{S2:cond:{Live:cond:|\nCosts 1 less.}|}");

    private void RefreshCost() =>
        EnergyCost.SetThisCombat(Sigils.At(Owner, Sigils.Tier2)
            ? Math.Max(0, EnergyCost.Canonical - 1)
            : EnergyCost.Canonical);

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
        var power = await PowerCmd.Apply<HyperfertilePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        if (power is null) return;
        var percent = Sigils.At(Owner, Sigils.Tier3) ? 3 : Sigils.At(Owner, Sigils.Tier1) ? 2 : 1;
        power.PercentPerHp = Math.Max(power.PercentPerHp, percent);
        if (Sigils.At(Owner, Sigils.Tier4)) power.CumMultiplier = 3;
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
