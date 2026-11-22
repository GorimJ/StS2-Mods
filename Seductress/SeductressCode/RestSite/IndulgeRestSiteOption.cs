using System;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using Seductress.SeductressCode.Extensions;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.RestSite;

/// <summary>
/// Indulge — scoop the cum off your body and eat it (design §6.3c).
///
/// Empties the Skin, recovering <b>two thirds</b> of it: heal 1 per ml recovered and bank that ml
/// into the Stomach. The other third spills.
///
/// [Claude-Cowork] This is what replaced <i>Scoop It Up</i>. As a card, a single Skin→Mouth
/// transfer would have been close to mandatory in every deck once all leak routed to Skin — and a
/// card nobody can skip is worse than no card. Moving the job to the rest site gives Draining the
/// same pipeline through the map instead of the deck, and every archetype feeds it.
///
/// [Claude-Cowork] The two thirds is one half of a matched pair: leaked cum arrives at 75% and is
/// reclaimed at 67%, so <c>0.75 x 2/3 = 0.5</c> exactly. Cum banked directly is worth double cum
/// leaked and reclaimed, which is what makes routing skill worth anything on the permanent track.
/// Before both numbers existed, a Mouth deck banked the same total however badly it routed.
///
/// [Claude-Code] It fires the vanilla <b>rest-site heal hooks</b>, which is how relics that key
/// off resting keep working: with HEAL removed, <i>Regal Pillow</i>, <i>Stone Humidifier</i> and
/// <i>Night Terrors</i> would otherwise never trigger at all on this character. Done through the
/// hook rather than by naming Regal Pillow, so anything else in vanilla or in another mod that
/// modifies rest healing composes for free.
///
/// [Gorim] <b>Do not tune this down further.</b> It has been nerfed twice already on the strength
/// of a Stomach payoff that until this slice did not exist, and Relief dominates it on the health
/// axis at every point — removing a Lust card is worth a full bar regardless of current HP.
/// </summary>
public class IndulgeRestSiteOption(Player owner) : CustomRestSiteOption(owner)
{
    public override string OptionId => "SEDUCTRESS_INDULGE";

    /// <summary>
    /// REQUIRED. RestSiteOption.Icon derives its path from OptionId, and a missing file throws
    /// inside NRestSiteButton._Ready — which aborts Reload() before the label is written, so the
    /// button silently wears whichever face the prefab last had. That cost the whole of slice 2.
    /// </summary>
    public override string CustomIconPath => "relic.png".RelicImagePath();

    /// <summary>
    /// [Claude-Cowork] No fallback when the Skin is empty. Indulge is not a heal with a bonus, it
    /// is a way of spending a resource — so with nothing on you it does nothing, and Relief or
    /// Smith is the right pick.
    /// </summary>
    public override bool IsEnabled => Reservoirs.Of(Owner.RunState).Skin > 0;

    public override LocString Description => new(
        "rest_site_ui",
        IsEnabled ? "OPTION_" + OptionId + ".description" : "OPTION_" + OptionId + ".descriptionDisabled");

    /// <summary>
    /// Two thirds, or all of it once the Stomach passes 200 ml — the track improving its own
    /// input, using a number introduced one milestone earlier rather than inventing one.
    /// </summary>
    /// <summary>ml of Skin recovered per HP healed.</summary>
    public const int MlPerHp = 5;

    private int Recovered(int skin) =>
        Stomach.IndulgeIsClean(Reservoirs.Of(Owner.RunState).Stomach)
            ? skin
            : (int)Math.Floor(skin * 2.0 / 3.0);

    public override async Task<bool> OnSelect()
    {
        var r = Reservoirs.Of(Owner.RunState);
        var skin = r.Skin;
        if (skin <= 0) return false;

        var recovered = Recovered(skin);
        r.Skin = 0;

        // Route the heal through the vanilla rest-site hooks. Removing HEAL left every relic that
        // keys off resting with nothing to fire on — Regal Pillow (+15 HP), Stone Humidifier
        // (+Max HP) and Night Terrors are all dead weight on this character otherwise. Indulge is
        // the heal option, so it is the one that should carry them.
        var runState = Owner.RunState;
        // [Gorim, 2026-11-20] Indulge heals 1 HP per 5 ml recovered: the x5 ml scale applied, so it
        // heals what it did before, and twice what a swallow now does (1 per 10).
        var baseHeal = recovered / MlPerHp;
        var healed = (int)Hook.ModifyRestSiteHealAmount(runState, Owner.Creature, baseHeal);

        MainFile.Logger.Info(
            $"[Seductress] Indulge: skin {skin} ml -> recovered {recovered} ml " +
            $"({(Stomach.IndulgeIsClean(r.Stomach) ? "100%" : "67%")}), spilled {skin - recovered}" +
            $", healing {baseHeal}" + (healed != baseHeal ? $" ({healed} after relics)" : ""));

        await CreatureCmd.Heal(Owner.Creature, healed);

        // Banks the ml recovered, not the boosted HP. A relic that improves resting gives you
        // health, not more cum — the two numbers deliberately diverge here.
        await Stomach.Bank(Owner, recovered);

        await Hook.AfterRestSiteHeal(runState, Owner, isMimicked: false);

        FloorLog.RecordIndulge(skin, recovered);
        return true;
    }
}
