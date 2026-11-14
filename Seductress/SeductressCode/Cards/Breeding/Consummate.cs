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
/// Breeding Rare, 0 cost — so tier 4 sheds its non-energy cost (the Sigil exhaust) rather than
/// repeating. Tier 1 lands exactly when the card becomes playable at all, since it needs a Sigil
/// to exhaust: the first Sigil both unlocks and improves it.
/// </summary>
public class Consummate : SeductressCard
{
    public Consummate() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
        new EnergyVar(2),
        new CardsVar(3),
        new BlockVar(10m, ValueProp.Move),
        new DynamicVar("TierBlock", 20m),
        // [Claude, 2026-11-14] Must be an EnergyVar: energyIcons() rejects a plain DynamicVar, and one
        // failed token made the game print the whole description raw.
        new EnergyVar("TierEnergy", 3)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Consummate",
        "Exhaust a Sigil for this combat: gain {Energy:energyIcons()} and draw {Cards:diff()}.\n{T1} Sigil: gain {Block:diff()} [gold]Block[/gold].\n{T2} Sigils: gain {TierEnergy:energyIcons()} instead.\n{T3} Sigils: gain {TierBlock:diff()} [gold]Block[/gold] instead.\n{T4} Sigils: does not exhaust a Sigil.");

    /// <summary>
    /// Unplayable with no Sigil left to spend — unless tier 4, where it no longer needs one.
    /// CanPlay folds this into BlockedByCardLogic.
    /// </summary>
    protected override bool IsPlayable =>
        base.IsPlayable && (Sigils.At(Owner, Sigils.Tier4) || Sigils.Effective(Owner) > 0);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Tier 4 on a 0-cost card sheds the non-energy cost instead of repeating.
        if (!Sigils.At(Owner, Sigils.Tier4)) ClimaxModel.Instance.ExhaustSigil();

        var energy = Sigils.At(Owner, Sigils.Tier2) ? DynamicVars["TierEnergy"] : DynamicVars.Energy;
        await PlayerCmd.GainEnergy(energy.BaseValue, Owner);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner);

        if (Sigils.At(Owner, Sigils.Tier1))
        {
            var block = Sigils.At(Owner, Sigils.Tier3) ? DynamicVars["TierBlock"].BaseValue : DynamicVars.Block.BaseValue;
            await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Cards"].UpgradeValueBy(1m);
}
