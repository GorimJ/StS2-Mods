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
/// Breeding Uncommon Power. Bespoke tier 4 (+1 per Sigil) by choice, not because repeating was
/// impossible — converting a static buff into a scaling one is the fantasy.
///
/// NOTE: the Sigil half is inert until the Sigil passive (§7.0) exists.
/// </summary>
public class RitualMark : SeductressCard
{
    public RitualMark() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
        new DynamicVar("Bonus", 2m),
        new DynamicVar("Tier1Bonus", 3m),
        new DynamicVar("Tier2Bonus", 4m),
        new DynamicVar("Tier3Bonus", 5m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Ritual Mark",
        "Your Pussy and Breasts cards deal {S4:cond:{SigilN}|{S3:cond:{Tier3Bonus:diff()}|{S2:cond:{Tier2Bonus:diff()}|{S1:cond:{Tier1Bonus:diff()}|{Bonus:diff()}}}}} additional damage.{S4:cond:\n[i](1 per Sigil)[/i]|}");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // [Gorim, 2026-11-23] Ritual Mark no longer adds to Sigil damage. [Claude] That emptied
        // stages 1 and 3, so the card bonus now steps 2 / 3 / 4 / 5, then +1 per Sigil at the top.
        var bonus = Sigils.At(Owner, Sigils.Tier3) ? DynamicVars["Tier3Bonus"].BaseValue
                  : Sigils.At(Owner, Sigils.Tier2) ? DynamicVars["Tier2Bonus"].BaseValue
                  : Sigils.At(Owner, Sigils.Tier1) ? DynamicVars["Tier1Bonus"].BaseValue
                  : DynamicVars["Bonus"].BaseValue;

        var power = await PowerCmd.Apply<RitualMarkPower>(
            choiceContext, Owner.Creature, bonus, Owner.Creature, this);
        if (power is null) return;

        power.Copies++;
        power.PerSigil |= Sigils.At(Owner, Sigils.Tier4);
    }

    /// <summary>
    /// [Gorim 2026-09-07] The upgrade grants <b>Innate</b> and touches no numbers.
    ///
    /// §8's "+3 base" moved only the Pussy side, which broke the parity the text depends on — at
    /// 5 Sigils it read "Sigils deal 4 too" while Pussy cards were on 7. Lifting the Sigil side as
    /// well fixed that but made the upgrade far too strong: it raised four numbers on a Power that
    /// already scales with the run's Sigil count.
    ///
    /// Innate sidesteps both. It is a real upgrade on a Power — getting the engine online turn one
    /// is worth more than a bigger number on a card you may draw late — and it leaves every tier
    /// value alone, so the "too" parity holds at every upgrade level.
    ///
    /// The keyword line renders automatically from the card's keywords, so the description must
    /// not mention Innate or it prints twice.
    /// </summary>
    protected override void OnUpgrade() => CardCmd.ApplyKeyword(this, CardKeyword.Innate);
}
