using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Draining Common. Deferred through slices 2 and 3 because precum did not exist; unblocked now.
///
/// [Claude-Code] Untagged, because it is a Skill — Skills are never tagged, however hole-specific
/// their text is (design §6.0). So it fills the Mouth without setting the Continuing mode, which
/// is the correct shape: it is fuel, not a finisher.
/// </summary>
public class LickPrecum : SeductressCard
{
    public LickPrecum() : base(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Ml", 25m)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Lick Pre-cum",
        "Gain {Ml:diff()} ml precum. Apply 1 [gold]Weak[/gold] to ALL enemies.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var r = Reservoirs.Of(Owner.RunState);
        var added = r.AddPrecum((int)DynamicVars["Ml"].BaseValue);
        MainFile.Logger.Info($"[Seductress] Lick Pre-cum: +{added} ml precum | mouth {r.Mouth}+{r.Precum}p");
        await Stomach.SwallowIfFull(Owner, (int)DynamicVars["Ml"].BaseValue, added, cum: false);
        await Stomach.AfterPrecumGained(Owner, (int)DynamicVars["Ml"].BaseValue);

        var enemies = CombatState!.Enemies.Where(e => e.IsAlive).ToList();
        if (enemies.Count > 0)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, enemies, 1m, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Ml"].UpgradeValueBy(15m);
}
