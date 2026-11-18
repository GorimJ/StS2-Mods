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
/// Draining Uncommon Power. [Gorim, 2026-11-17, reworking the Throat Training draft] "Whenever you
/// gain precum, gain 3 more and eat it."
///
/// [Claude] Read as: the extra 3 ml is what gets eaten, so each precum gain also heals 1 and the
/// Mouth is untouched. Triggers on card and power gains, not on the Stomach's combat-start precum.
/// The upgrade (5 ml, so heal 2) is my number.
/// </summary>
public class MilkingThroat : SeductressCard
{
    public MilkingThroat() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Ml", 3m)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Milking Throat", "Whenever you gain precum, gain {Ml:diff()} ml more and Eat it.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MilkingThroatPower>(
            choiceContext, Owner.Creature, DynamicVars["Ml"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Ml"].UpgradeValueBy(2m);
}
