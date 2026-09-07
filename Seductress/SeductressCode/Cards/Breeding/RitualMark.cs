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
        new DynamicVar("TierBonus", 4m),
        new DynamicVar("SigilBonus", 1m),
        new DynamicVar("TierSigilBonus", 4m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Ritual Mark",
        "Your Pussy cards deal {Bonus:diff()} additional damage.\n{T1} Sigil: your Sigils deal 1 additional damage.\n{T2} Sigils: Pussy cards deal {TierBonus:diff()} instead.\n{T3} Sigils: Sigils deal 4 too.\n{T4} Sigils: both bonuses become +1 per Sigil.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var pussy = Sigils.At(Owner, Sigils.Tier2) ? DynamicVars["TierBonus"].BaseValue
                                                   : DynamicVars["Bonus"].BaseValue;

        var power = await PowerCmd.Apply<RitualMarkPower>(
            choiceContext, Owner.Creature, pussy, Owner.Creature, this);
        if (power is null) return;

        power.SigilBonus = Sigils.At(Owner, Sigils.Tier3) ? (int)DynamicVars["TierSigilBonus"].BaseValue
                         : Sigils.At(Owner, Sigils.Tier1) ? (int)DynamicVars["SigilBonus"].BaseValue
                         : 0;
        power.PerSigil = Sigils.At(Owner, Sigils.Tier4);
    }

    /// <summary>
    /// [Gorim 2026-09-07] The upgrade lifts the Sigil bonuses as well as the Pussy ones. §8's
    /// upgrade note reads "+3 base", which moved only the Pussy side and broke the parity the word
    /// "too" depends on — at 5 Sigils the card claimed "Sigils deal 4 too" while Pussy cards were
    /// on 7. +3 across all four numbers keeps the top rung matching at 7 and 7.
    /// </summary>
    protected override void OnUpgrade()
    {
        DynamicVars["Bonus"].UpgradeValueBy(3m);
        DynamicVars["TierBonus"].UpgradeValueBy(3m);
        DynamicVars["SigilBonus"].UpgradeValueBy(3m);
        DynamicVars["TierSigilBonus"].UpgradeValueBy(3m);
    }
}
