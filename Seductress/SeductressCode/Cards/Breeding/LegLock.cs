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
/// New Breeding Uncommon, taking the draft slot Breed Me! vacated. Applies the Leg-Locked kill
/// claim (design §6.3a) plus vanilla Debilitate at tier 1.
///
/// Tier 4 is bespoke — apply all of it to ALL enemies rather than repeating, since repeating a
/// single-target debuff on the same enemy is mostly wasted.
/// </summary>
public class LegLock : SeductressCard
{
    public LegLock() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
        new BlockVar(6m, ValueProp.Move)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Leg Lock",
        "Gain {Block} [gold]Block[/gold]. Apply 1 [gold]Vulnerable[/gold] and 1 Leg-Locked.\n{T1} Sigil: also apply 1 [gold]Debilitate[/gold].\n{T2} Sigils: apply 2 Leg-Locked and 2 [gold]Vulnerable[/gold].\n{T3} Sigils: apply 2 [gold]Debilitate[/gold] instead.\n{T4} Sigils: apply all of it to ALL enemies.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        var stacks = Sigils.At(Owner, Sigils.Tier2) ? 2m : 1m;

        // Debilitate stacks are duration, not intensity: the multiplier is amount + (amount - 1)
        // applied once however many stacks are held, and it only fires on powered attacks. So
        // tier 3 buys two enemy turns of doubled Vulnerable, not double the doubling.
        var debilitate = Sigils.At(Owner, Sigils.Tier3) ? 2m : 1m;

        var targets = Sigils.At(Owner, Sigils.Tier4)
            ? CombatState!.Enemies.Where(e => e.IsAlive).ToList()
            : [cardPlay.Target!];

        await PowerCmd.Apply<LegLockedPower>(choiceContext, targets, stacks, Owner.Creature, this);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, targets, stacks, Owner.Creature, this);
        if (Sigils.At(Owner, Sigils.Tier1))
        {
            await PowerCmd.Apply<DebilitatePower>(choiceContext, targets, debilitate, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
