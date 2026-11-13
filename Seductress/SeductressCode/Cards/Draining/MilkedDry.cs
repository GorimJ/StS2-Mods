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
/// Draining Rare. Deferred through slices 2 and 3 pending precum; unblocked now.
///
/// [Claude-Code] 15 ml is 30% of the Mouth's 50 ml cap in one card, so this is the archetype's
/// refuel as much as its AoE — and because precum spends first, it fuels <i>Spit</i> and
/// <i>Wet Work</i> without touching cum already banked for the swallow.
/// </summary>
public class MilkedDry : SeductressCard
{
    public MilkedDry() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

    public override Hole Hole => Hole.Mouth;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar("Ml", 15m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Milked Dry",
        "Mouth. Deal {Damage:diff()} damage to ALL enemies. Gain {Ml:diff()} ml precum.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
            .TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_slash").SpawningHitVfxOnEachCreature().Execute(choiceContext);

        var r = Reservoirs.Of(Owner.RunState);
        var added = r.AddPrecum((int)DynamicVars["Ml"].BaseValue);
        MainFile.Logger.Info($"[Seductress] Milked Dry: +{added} ml precum | mouth {r.Mouth}+{r.Precum}p");
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
