using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Seductress.SeductressCode.Relics;

/// <summary>
/// [Claude-Cowork] Breeding's relic. Replaces <i>Wet Patch</i>, which paid 2 HP whenever any
/// reservoir leaked — that became incoherent once leak routed to Skin, because it was paying you
/// for a <b>transfer</b> rather than compensating a loss, and it was never the Pussy relic it was
/// described as since it triggered on everything.
///
/// [Claude-Cowork] <b>It works at every point in the cycle, which is the whole argument for it.</b>
/// The Womb's leak is a bounded 0-10 ml sawtooth that resets at each locked threshold and is
/// identical whether you are filling or carrying — so halving it is a flat, permanent acceleration
/// of the run's progression track. Every threshold arrives sooner, so pregnancies do, so Sigils
/// do, so every gated card in Breeding turns on earlier.
///
/// It is the Plug's counterpart rather than its opposite: the Plug protects the Ass and costs HP
/// per floor, this protects the Womb and costs nothing. The Plug's value grows with how inflated
/// you are and is near-worthless early; this one never outgrows relevance and never spikes.
///
/// [Claude-Code] The effect is read in <see cref="Systems.RunModel"/>'s leak rather than
/// implemented here, because leak is applied in one place for all reservoirs and splitting the
/// rate across two files would make the banded Ass arithmetic harder to check.
/// </summary>
public class StickyBandaid : SeductressRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override List<(string, string)>? Localization => new RelicLoc(
        "Sticky Bandaid",
        "Your Womb leaks half as much.",
        "You manage to reapply it after every fight, keeping the cum inside.");
}
