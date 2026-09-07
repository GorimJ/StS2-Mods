using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Seductress.SeductressCode.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Draining Uncommon. Swallow <b>now</b>.
///
/// [Gorim] <b>Gulp does not enable banking.</b> The Mouth banks to the Stomach at the end of every
/// combat by default (design §6.5) — what this card buys is the <b>timing</b>, the <b>heal</b> and
/// the <b>Sensitive removal</b>. That was only ever implied, by §6.2a saying precum is the thing
/// that evaporates, and the omission had already caused one misreading.
///
/// [Claude-Code] Two consequences of it being timing rather than permission, both real:
/// emptying the Mouth mid-fight makes room to fill it again, so on a long fight Gulp is throughput
/// as well as sustain — and the precum in your Mouth heals you here even though it would never
/// have banked, because the heal is on the ml swallowed rather than on what reaches the Stomach.
/// </summary>
public class Gulp : SeductressCard
{
    public Gulp() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    /// <summary>
    /// The share of the swallowed ml healed. The upgrade moves this number rather than flipping a
    /// hidden flag, because STS2 has one description key per card and no upgraded variant — an
    /// upgrade that is not a dynamic var is invisible on the card.
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Portion", 50m),
        new DynamicVar("Threshold", 25m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Gulp",
        "Swallow now: heal {Portion:diff()}% of your Mouth in ml and bank the cum into your Stomach, then empty it.\nIf you swallowed more than {Threshold} ml, remove 1 [gold]Sensitive[/gold].");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var r = Reservoirs.Of(Owner.RunState);
        var total = r.MouthTotal;
        if (total <= 0)
        {
            MainFile.Logger.Info("[Seductress] Gulp: nothing in the Mouth");
            return;
        }

        var heal = (int)(total * DynamicVars["Portion"].BaseValue / 100m);
        MainFile.Logger.Info(
            $"[Seductress] Gulp: swallowing {total} ml ({r.Mouth} cum + {r.Precum} precum), healing {heal}");

        await CreatureCmd.Heal(Owner.Creature, heal);
        await Stomach.SwallowAll(Owner, "Gulp");

        if (total > DynamicVars["Threshold"].BaseValue &&
            Owner.Creature.GetPower<SensitivePower>() is { } sensitive)
        {
            // ModifyAmount with a negative offset is the game's own "remove a stack" path;
            // PowerCmd.Remove would strip every stack, which is not what the card says.
            await PowerCmd.ModifyAmount(choiceContext, sensitive, -1m, Owner.Creature, this);
            MainFile.Logger.Info("[Seductress] Gulp: removed 1 Sensitive");
        }
    }

    protected override void OnUpgrade() => DynamicVars["Portion"].UpgradeValueBy(50m);
}
