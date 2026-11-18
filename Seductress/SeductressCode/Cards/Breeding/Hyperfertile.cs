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

    public override List<(string, string)>? Localization => new CardLoc(
        "Hyperfertile",
        "Gain twice as much Pussy cum. Your Pussy attacks deal 1% more damage for each HP the enemy is missing.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<HyperfertilePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
