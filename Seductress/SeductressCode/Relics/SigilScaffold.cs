using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Runs;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Relics;

/// <summary>
/// ============================ TEST SCAFFOLDING — DELETE WITH CONCEPTION ============================
///
/// Grants 2 Sigils at run start so Breeding's tier-1 gates are live from floor 1 and every Breeding
/// card reads and functions. Sigils are supposed to come from pregnancy, which is out of scope for
/// this slice.
///
/// <b>This distorts the very thing it makes testable.</b> By design Sigils arrive from elite and
/// boss pregnancies and Breeding should feel thin early; with 2 from floor 1 the archetype sits at
/// roughly its act-2 power throughout act 1. A run played with this relic therefore says nothing
/// about whether Breeding's intended early-game weakness feels right.
///
/// Remove this class, <see cref="Seductress.SeductressCode.Console.SigilsConsoleCmd"/> and the
/// entry in the character's StartingRelics when Conception lands.
///
/// ==================================================================================================
/// </summary>
public class SigilScaffold : SeductressRelic
{
    // [Claude-Cowork, per Gorim] Dropped 2 -> 1 so the relic demonstrates exactly tier 1 rather
    // than overshooting it. Tier 1 is now the first Sigil, so 1 is all that is needed.
    public const int GrantedSigils = 1;

    public override RelicRarity Rarity => RelicRarity.Starter;

    /// <summary>Never appears in any reward pool; it is only ever handed out at run start.</summary>
    // Game 0.113: IsAllowed is no longer virtual; IsAllowedInternal is the hook it calls.
    protected override bool IsAllowedInternal(IRunState runState) => false;

    public override List<(string, string)>? Localization => new RelicLoc(
        "Fertile Sign",
        $"TEST SCAFFOLDING. Start with {GrantedSigils} Sigils. Will be replaced by Conception.",
        "A placeholder, and it knows it.");

    public override Task AfterObtained()
    {
        var r = Reservoirs.Of(Owner.RunState);
        if (r.Sigils < GrantedSigils) r.Sigils = GrantedSigils;
        MainFile.Logger.Info($"[Seductress] SCAFFOLD: Fertile Sign granted {r.Sigils} Sigils");
        return Task.CompletedTask;
    }
}
