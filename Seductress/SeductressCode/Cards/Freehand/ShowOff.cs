using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>Freehand. Buys AoE with self-Vulnerable — escalation as a cost.</summary>
public class ShowOff : SeductressCard
{
    public ShowOff() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    public override Hole Hole => Hole.Skin;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Show Off",
        "Skin. Gain 1 [gold]Vulnerable[/gold].\nDeal {Damage:diff()} damage to ALL enemies and apply 1 [gold]Vulnerable[/gold].");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VulnerablePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        var enemies = CombatState!.Enemies.Where(e => e.IsAlive).ToList();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash").SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, enemies, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
