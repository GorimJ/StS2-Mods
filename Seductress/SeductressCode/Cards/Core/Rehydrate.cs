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
/// Core Common. [Gorim, 2026-11-17, board comment] Replaces <i>Second Wind</i>, which was "kind of
/// boring, and also literally shares a name with an ironclad card": "Eat 5, your next attack
/// gains 5 precum."
///
/// [Claude] Read as: the next Attack you play this combat adds 5 ml precum to the Mouth, whatever
/// hole it uses. The upgrade (8 and 8) is my number, not his.
/// </summary>
public class Rehydrate : SeductressCard
{
    public Rehydrate() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Ml", 50m)];   // [Gorim, 2026-11-19] "Buff this to 10/10 actually"; 50 on the x5 scale

    public override List<(string, string)>? Localization => new CardLoc(
        "Rehydrate", "Eat {Ml:diff()}. Your next Attack gains {Ml:diff()} ml precum.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var ml = (int)DynamicVars["Ml"].BaseValue;
        await Stomach.Eat(Owner, ml, "Rehydrate");
        await PowerCmd.Apply<RehydratePower>(choiceContext, Owner.Creature, ml, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Ml"].UpgradeValueBy(20m); // [Gorim, 2026-11-23] 70, rounded up from 65
}
