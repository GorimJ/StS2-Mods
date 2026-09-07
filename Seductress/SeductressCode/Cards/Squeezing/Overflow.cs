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
/// Squeezing. The cheap bank-dump — base damage of 1 is almost irrelevant, the double drain does
/// the work. The drain is the real cost, not the energy.
/// </summary>
public class Overflow : SeductressCard
{
    private bool _upgraded;

    public Overflow() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    public override Hole Hole => Hole.Ass;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1m, ValueProp.Move)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Overflow",
        "Ass. Deal {Damage:diff()} damage to ALL enemies and apply 1 [gold]Vulnerable[/gold].\nThis card loses double the usual Ass.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var bonus = SpendAssForDamage(0.20);
        var enemies = CombatState!.Enemies.Where(e => e.IsAlive).ToList();

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash").SpawningHitVfxOnEachCreature().Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, enemies, 1m, Owner.Creature, this);
        if (_upgraded)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, enemies, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => _upgraded = true;
}
