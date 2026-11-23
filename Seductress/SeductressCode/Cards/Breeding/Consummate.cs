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
/// Breeding Rare, 0 cost. [Gorim, 2026-11-23, card note] "Top stage should no longer exhaust the
/// card, but still the sigil": at tier 4 the card loses Exhaust and still spends a Sigil. (Until
/// then tier 4 dropped the Sigil cost instead.) Tier 1 lands exactly when the card becomes playable at all, since it needs a Sigil
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
        new EnergyVar("SmallEnergy", 1),
        new DynamicVar("SmallCards", 1m),
        new BlockVar(10m, ValueProp.Move),
        new DynamicVar("TierBlock", 20m),
        // [Claude, 2026-11-14] Must be an EnergyVar: energyIcons() rejects a plain DynamicVar, and one
        // failed token made the game print the whole description raw.
        new EnergyVar("TierEnergy", 3)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Consummate",
        "{S1:cond:Exhaust a Sigil this combat.\nGain {S2:cond:{TierEnergy:energyIcons()}|{Energy:energyIcons()}}.\nDraw {Cards:diff()}.\nGain {S3:cond:{TierBlock:diff()}|{Block:diff()}} [gold]Block[/gold].|Gain {SmallEnergy:energyIcons()}.\nDraw {SmallCards:diff()}.}");

    /// <summary>
    /// [Gorim, 2026-11-21] "Consummate is too bad with 0 sigils... I'd like to make it do
    /// something. Probably it should exhaust." He picked the small version: with no Sigil to
    /// spend it gives 1 energy and 1 card. It used to be unplayable then.
    /// </summary>
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        RefreshExhaust(); // read before the Sigil is spent, so the play that drops below tier 4 still stays
        if (Sigils.Effective(Owner) <= 0)
        {
            await PlayerCmd.GainEnergy(DynamicVars["SmallEnergy"].BaseValue, Owner);
            await CardPileCmd.Draw(choiceContext, DynamicVars["SmallCards"].BaseValue, Owner);
            return;
        }
        ClimaxModel.Instance.ExhaustSigil();

        var energy = Sigils.At(Owner, Sigils.Tier2) ? DynamicVars["TierEnergy"] : DynamicVars.Energy;
        await PlayerCmd.GainEnergy(energy.BaseValue, Owner);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].BaseValue, Owner);

        if (Sigils.At(Owner, Sigils.Tier1))
        {
            var block = Sigils.At(Owner, Sigils.Tier3) ? DynamicVars["TierBlock"].BaseValue : DynamicVars.Block.BaseValue;
            await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
        }
    }

    /// <summary>Exhaust is on the card below tier 4 and off it at tier 4.</summary>
    private void RefreshExhaust()
    {
        if (Sigils.At(Owner, Sigils.Tier4)) CardCmd.RemoveKeyword(this, CardKeyword.Exhaust);
        else CardCmd.ApplyKeyword(this, CardKeyword.Exhaust);
    }

    public override Task AfterCardEnteredCombat(MegaCrit.Sts2.Core.Models.CardModel card)
    {
        if (ReferenceEquals(card, this)) RefreshExhaust();
        return Task.CompletedTask;
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext,
        MegaCrit.Sts2.Core.Models.CardModel card, bool fromHandDraw)
    {
        if (ReferenceEquals(card, this)) RefreshExhaust();
        return Task.CompletedTask;
    }

    protected override void OnUpgrade() => DynamicVars["Cards"].UpgradeValueBy(1m);
}
