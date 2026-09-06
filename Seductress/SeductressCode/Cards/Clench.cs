using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Squeezing's basic multi-hit. Carries the Ass tag rule: the attack spends 10% of the current
/// Ass and converts it into damage for the duration of the hit (design §6.1).
/// </summary>
public class Clench : SeductressCard
{
    public Clench() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Ass;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new HpLossVar(4m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Clench",
        "Ass. Deal {Damage} damage twice. Lose {HpLoss} HP.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // Ass tag rule: lose 10% of current Ass (rounded up), +1 Strength per 10 ml lost, for this
        // attack only. Applied as flat bonus damage rather than a real Strength stack so it cannot
        // leak into the rest of the turn.
        var reservoirs = Reservoirs.Of(Owner.RunState);
        var spent = (int)Math.Ceiling(reservoirs.Ass * 0.1);
        reservoirs.Ass -= spent;
        var bonus = spent / 10;
        if (spent > 0)
        {
            MainFile.Logger.Info($"[Seductress] Clench spent {spent} ml Ass for +{bonus} damage per hit");
        }

        var damage = DynamicVars.Damage.BaseValue + bonus;
        for (var i = 0; i < 2; i++)
        {
            await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}
