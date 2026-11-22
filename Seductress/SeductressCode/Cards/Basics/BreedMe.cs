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
/// Basic, Pussy half of the starting pair. Teaches intake explicitly — the flat 5 ml makes the
/// reservoir move visibly on a kill — and its tiers begin firing on your first Sigil.
///
/// Damage 15 -> 12 for the starter slot: 15 at 2 energy is well above Bash's 8 and Eruption's 9,
/// and the card already carries more text than either.
/// </summary>
public class BreedMe : SeductressCard
{
    public BreedMe() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Pussy;
    protected override bool RepeatsAtTier4 => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
        new CalculationBaseVar(12m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(BonusCalc),
        new DynamicVar("TierDamage", 16m)
    ];


    /// <summary>Ritual Mark and the tier-2 swap, live on the card face.</summary>
    protected override decimal PreviewBonus() => TieredPussyBonus();

    public override List<(string, string)>? Localization => new CardLoc(
        "Breed Me!",
        "Pussy. Deal " + TieredDamage + " damage.\nKills give 50 ml extra{S3:cond: and triple intake|{S1:cond: and double intake|}}.{S4:cond:\nRepeats 2 times.|}");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = DynamicVars.CalculatedDamage.Calculate(cardPlay.Target);

        // The multiplier replaces the flat +50 ([Gorim, 2026-11-21]: 25 was too small; 50 is [Claude]'s number) rather than compounding — [Gorim], slice 2 D5.
        var multiplier = Sigils.At(Owner, Sigils.Tier3) ? 3 : Sigils.At(Owner, Sigils.Tier1) ? 2 : 1;
        ClimaxModel.Instance.SetKillIntakeBonus(50, multiplier);
        try
        {
            await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        }
        finally
        {
            ClimaxModel.Instance.ClearKillIntakeBonus();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(4m);
        DynamicVars.CalculatedDamage.RecalculateForUpgradeOrEnchant();
        DynamicVars["TierDamage"].UpgradeValueBy(4m);
    }
}
