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
/// [Gorim] <b>Gulp does not enable banking, and it does not enable the heal either.</b> The Mouth
/// banks to the Stomach <i>and</i> heals half the ml at the end of every combat by default
/// (design §6.5, and see Stomach.SwallowAll). What this card buys is the <b>timing</b>, the
/// <b>Sensitive removal</b>, and — once upgraded — a <b>better rate</b>: all the ml instead of half.
///
/// [Claude-Code] So an unupgraded Gulp is pure tempo. It heals exactly what the bell would have
/// healed anyway; you are paying a card to have it now, and to empty the Mouth so it can refill.
/// On a long fight that is throughput as much as sustain. The upgrade is where the raw value is,
/// which is an unusual and rather good shape for a 0-cost Uncommon.
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

        // The swallow itself heals, so this must not heal separately or it pays twice. What Gulp
        // buys is the rate — its upgrade takes the swallow from half the ml to all of it.
        await Stomach.SwallowAll(Owner, "Gulp", (int)DynamicVars["Portion"].BaseValue);

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
