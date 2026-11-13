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

/// <summary>Breeding Common. Tier 1 adds draw, 3 grows the damage, 5 grows the draw, 7 repeats.</summary>
public class TakeHimIn : SeductressCard
{
    public TakeHimIn() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Pussy;
    protected override bool RepeatsAtTier4 => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
        new CalculationBaseVar(8m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(BonusCalc),
        new DynamicVar("TierDamage", 12m)
    ];


    /// <summary>Ritual Mark and the tier-2 swap, live on the card face.</summary>
    protected override decimal PreviewBonus() => TieredPussyBonus();

    public override List<(string, string)>? Localization => new CardLoc(
        "Take Him In",
        "Pussy. Deal {CalculatedDamage:diff()} damage.\n{T1} Sigil: draw 1.\n{T2} Sigils: deal {TierDamage:diff()} instead.\n{T3} Sigils: draw 2 instead.\n{T4} Sigils: repeats 1 time.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);
        await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);

        var draw = Sigils.At(Owner, Sigils.Tier3) ? 2 : Sigils.At(Owner, Sigils.Tier1) ? 1 : 0;
        if (draw > 0) await CardPileCmd.Draw(choiceContext, draw, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
        DynamicVars.CalculatedDamage.RecalculateForUpgradeOrEnchant();
        DynamicVars["TierDamage"].UpgradeValueBy(3m);
    }
}
