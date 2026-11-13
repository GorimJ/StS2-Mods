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

/// <summary>
/// Draining. Untagged despite being an Attack — you are expelling, not receiving, so it fills
/// nothing and never sets the Continuing mode.
/// </summary>
public class Spit : SeductressCard
{
    public Spit() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    /// <summary>
    /// The upgrade moves a number rather than flipping a hidden flag, so it is actually visible on
    /// the card. STS2 has one description key per card and no upgraded variant, so a wording change
    /// cannot be shown — an upgrade has to be expressed as a dynamic var or it is invisible.
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Portion", 50m)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Spit",
        "Empty your Mouth. Deal damage equal to {Portion:diff()}% of the ml to ALL enemies, and apply 1 [gold]Weak[/gold] and 1 [gold]Vulnerable[/gold] per 10 ml emptied.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var r = Reservoirs.Of(Owner.RunState);
        var ml = r.Mouth;
        r.Mouth = 0;
        if (ml <= 0) return;

        var damage = (int)(ml * DynamicVars["Portion"].BaseValue / 100m);
        var stacks = ml / 10;
        var enemies = CombatState!.Enemies.Where(e => e.IsAlive).ToList();

        await DamageCmd.Attack(damage).FromCard(this, cardPlay).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash").SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
        if (stacks > 0)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, enemies, stacks, Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemies, stacks, Owner.Creature, this);
        }
        MainFile.Logger.Info($"[Seductress] Spit emptied {ml} ml for {damage} AoE and {stacks} stacks");
    }

    protected override void OnUpgrade() => DynamicVars["Portion"].UpgradeValueBy(50m);
}
