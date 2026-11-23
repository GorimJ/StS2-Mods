using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TextEffects;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// [Gorim, 2026-11-23] "Can we fix shown numbers generically, rather than per event/relic?"
/// [Claude] Text shows a dynamic variable's display value; the game applies its base value. So
/// this changes only the display of Max HP amounts, to what <see cref="MaxHpHalvingPatch"/> and
/// <see cref="MaxHpLossHalvingPatch"/> will apply (half, rounded up). The real amount is
/// untouched, so nothing is halved twice.
///
/// Covered: every <see cref="MaxHpVar"/> (food relics, Fruit Juice, Feed, Brightest Flame, Leafy
/// Poultice and the events that use one), plus two events whose Max HP number is a plain
/// variable with an unambiguous name: Stone of All Time ("DrinkMaxHpGain") and Unrest Site
/// ("MaxHpLoss"). Shown only while the local player in a run is the Seductress, so the
/// compendium and other characters keep the printed numbers.
///
/// Not covered: text with no variable (Cook at the rest site, the Blitz merchant, Night
/// Terrors, Terminal, Paper Cuts), Drowning Beacon and Sere Talon (their variable is "HpLoss",
/// which other cards use for ordinary HP loss), and Tablet of Truth, left alone on purpose: its
/// lethal check uses the full number, so a halved figure would hide when it kills.
/// Wuppy's MaxHp variable is the Wuppy's own HP and is skipped.
/// </summary>
[HarmonyPatch]
internal static class MaxHpTextPatch
{
    private static readonly AccessTools.FieldRef<DynamicVar, AbstractModel?> Owner =
        AccessTools.FieldRefAccess<DynamicVar, AbstractModel?>("_owner");

    private static readonly Func<RunManager, RunState?> State =
        AccessTools.MethodDelegate<Func<RunManager, RunState?>>(
            AccessTools.PropertyGetter(typeof(RunManager), "State"));

    private static bool Applies(DynamicVar v)
    {
        var named = v is MaxHpVar || v.Name is "DrinkMaxHpGain" or "MaxHpLoss";
        if (!named) return false;
        if (Owner(v) is MegaCrit.Sts2.Core.Models.Relics.Wuppy) return false;
        if (v.Name == "MaxHpLoss" && Owner(v) is not MegaCrit.Sts2.Core.Models.Events.UnrestSite) return false;
        var state = State(RunManager.Instance);
        return state is not null && LocalContext.GetMe(state)?.Character is Character.Seductress;
    }

    private static int Halve(int value) => value <= 0 ? value : (value + 1) / 2;

    [HarmonyPatch(typeof(DynamicVar), nameof(DynamicVar.ToString), new Type[0])]
    [HarmonyPostfix]
    private static void ToStringPostfix(DynamicVar __instance, ref string __result)
    {
        if (Applies(__instance) && int.TryParse(__result, out var n)) __result = Halve(n).ToString();
    }

    [HarmonyPatch(typeof(DynamicVar), nameof(DynamicVar.GetSourceValue))]
    [HarmonyPostfix]
    private static void SourceValuePostfix(DynamicVar __instance, ref object __result)
    {
        if (Applies(__instance) && __result is int n) __result = Halve(n);
    }

    /// <summary>Halves both sides of the comparison so the colour still means "upgraded or not".</summary>
    [HarmonyPatch(typeof(DynamicVar), nameof(DynamicVar.ToHighlightedString))]
    [HarmonyPrefix]
    private static bool HighlightedPrefix(DynamicVar __instance, bool inverse, ref string __result)
    {
        if (!Applies(__instance)) return true;
        var shown = Halve((int)__instance.PreviewValue);
        var value = Halve((int)__instance.EnchantedValue);
        var compare = __instance.WasJustUpgraded ? 1 : !inverse ? shown.CompareTo(value) : value.CompareTo(shown);
        __result = StsTextUtilities.HighlightChangeText(shown.ToString(System.Globalization.CultureInfo.InvariantCulture), compare);
        return false;
    }
}
