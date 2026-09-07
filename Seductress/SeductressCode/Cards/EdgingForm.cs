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

/// <summary>Ancient. A Power, so it may fill (§3). Best in Squeezing, as a floor under the bank.</summary>
public class EdgingForm : SeductressCard
{
    public EdgingForm() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Ml", 1m)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Edging Form", "Whenever you play an Attack, gain {Ml:diff()} ml in the hole that Attack used.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EdgingFormPower>(
            choiceContext, Owner.Creature, DynamicVars["Ml"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Ml"].UpgradeValueBy(1m);
}
