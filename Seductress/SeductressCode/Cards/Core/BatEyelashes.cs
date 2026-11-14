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

/// <summary>Core. Cheap control; the upgrade adds Vulnerable.</summary>
public class BatEyelashes : SeductressCard
{
    private bool _upgraded;

    public BatEyelashes() : base(0, CardType.Skill, CardRarity.Common, TargetType.AllEnemies) { }

    public override List<(string, string)>? Localization => new CardLoc(
        "Bat Eyelashes", "Apply 1 [gold]Weak[/gold]{IfUpgraded:show: and 1 [gold]Vulnerable[/gold]|} to ALL enemies.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = CombatState!.Enemies.Where(e => e.IsAlive).ToList();
        await PowerCmd.Apply<WeakPower>(choiceContext, enemies, 1m, Owner.Creature, this);
        if (_upgraded)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemies, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => _upgraded = true;
}
