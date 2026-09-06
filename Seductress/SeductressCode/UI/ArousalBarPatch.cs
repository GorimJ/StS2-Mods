using System;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.addons.mega_text;

namespace Seductress.SeductressCode.UI;

/// <summary>
/// Spike question 1: can the health bar present inverted, as an ascending Arousal bar?
///
/// <c>HealthBarForecastRegistry</c> can overlay segments without patching but cannot reverse the
/// base fill, so this patches <see cref="NHealthBar"/> directly. Two private methods do all the
/// work: <c>RefreshForeground</c> sets the red bar's right offset from
/// <c>GetFgWidth(CurrentHp)</c>, and <c>RefreshText</c> writes "cur/max". Postfixing both and
/// substituting <c>MaxHp - CurrentHp</c> is the whole inversion.
///
/// The patches are inert unless the local player is the Seductress, so other characters and other
/// mods' characters are untouched.
/// </summary>
internal static class ArousalBar
{
    /// Arousal pink, rather than the vanilla "F1373E" damage red.
    private static readonly Color ArousalColor = new("E0559C");

    private static readonly AccessTools.FieldRef<NHealthBar, Creature> CreatureRef =
        AccessTools.FieldRefAccess<NHealthBar, Creature>("_creature");

    private static readonly AccessTools.FieldRef<NHealthBar, Control> ForegroundRef =
        AccessTools.FieldRefAccess<NHealthBar, Control>("_hpForeground");

    private static readonly AccessTools.FieldRef<NHealthBar, Control> ForegroundContainerRef =
        AccessTools.FieldRefAccess<NHealthBar, Control>("_hpForegroundContainer");

    private static readonly AccessTools.FieldRef<NHealthBar, Control> MiddlegroundRef =
        AccessTools.FieldRefAccess<NHealthBar, Control>("_hpMiddleground");

    private static readonly AccessTools.FieldRef<NHealthBar, float> ExpectedMaxFgWidthRef =
        AccessTools.FieldRefAccess<NHealthBar, float>("_expectedMaxFgWidth");

    private static readonly AccessTools.FieldRef<NHealthBar, MegaLabel> LabelRef =
        AccessTools.FieldRefAccess<NHealthBar, MegaLabel>("_hpLabel");

    /// <summary>
    /// Only invert while the Seductress is the local player. Enemies invert too — an enemy's HP is
    /// their arousal, and they climax and pass out at the end of it (design §6.0) — so this asks the
    /// combat the bar's own creature is in, not the creature itself.
    /// </summary>
    internal static bool Active(Creature creature) =>
        LocalContext.GetMe(creature.CombatState)?.Character is Character.Seductress;

    internal static Creature? CreatureOf(NHealthBar bar)
    {
        try { return CreatureRef(bar); }
        catch (Exception) { return null; }
    }

    private static float MaxFgWidth(NHealthBar bar)
    {
        var expected = ExpectedMaxFgWidthRef(bar);
        return expected > 0f ? expected : ForegroundContainerRef(bar).Size.X;
    }

    /// <summary>Mirror of the game's own GetFgWidth, driven by arousal instead of current HP.</summary>
    internal static void ApplyInvertedFill(NHealthBar bar, Creature creature)
    {
        if (creature.MaxHp <= 0) return;

        var arousal = creature.MaxHp - creature.CurrentHp;
        var maxWidth = MaxFgWidth(bar);
        var width = (float)arousal / creature.MaxHp * maxWidth;
        width = arousal > 0 ? Math.Max(width, 12f) : 0f;

        var fg = ForegroundRef(bar);
        fg.Visible = arousal > 0;
        fg.OffsetRight = width - maxWidth;
        fg.SelfModulate = ArousalColor;
    }

    /// <summary>
    /// The trailing "recent damage" bar reads backwards once the fill is reversed, so it is hidden
    /// rather than re-derived.
    ///
    /// It has to be hidden from RefreshMiddleground's own postfix, not from the foreground one:
    /// RefreshValues calls Foreground then Middleground, and Middleground unconditionally sets
    /// Visible = true. It also only collapses via a tween that fires when HP changes, so on a
    /// creature that has not been hit yet it sits at full width — which is why an untouched enemy
    /// showed a full orange bar behind an empty arousal bar.
    /// </summary>
    internal static void HideMiddleground(NHealthBar bar) => MiddlegroundRef(bar).Visible = false;

    internal static void ApplyInvertedText(NHealthBar bar, Creature creature)
    {
        LabelRef(bar).SetTextAutoSize($"{creature.MaxHp - creature.CurrentHp}/{creature.MaxHp}");
    }
}

[HarmonyPatch(typeof(NHealthBar), "RefreshForeground")]
internal static class ArousalBarForegroundPatch
{
    [HarmonyPostfix]
    private static void Invert(NHealthBar __instance)
    {
        var creature = ArousalBar.CreatureOf(__instance);
        if (creature == null || !ArousalBar.Active(creature)) return;
        ArousalBar.ApplyInvertedFill(__instance, creature);
    }
}

[HarmonyPatch(typeof(NHealthBar), "RefreshMiddleground")]
internal static class ArousalBarMiddlegroundPatch
{
    [HarmonyPostfix]
    private static void Hide(NHealthBar __instance)
    {
        var creature = ArousalBar.CreatureOf(__instance);
        if (creature == null || !ArousalBar.Active(creature)) return;
        ArousalBar.HideMiddleground(__instance);
    }
}

[HarmonyPatch(typeof(NHealthBar), "RefreshText")]
internal static class ArousalBarTextPatch
{
    [HarmonyPostfix]
    private static void Invert(NHealthBar __instance)
    {
        var creature = ArousalBar.CreatureOf(__instance);
        if (creature == null || creature.CurrentHp <= 0 || !ArousalBar.Active(creature)) return;
        ArousalBar.ApplyInvertedText(__instance, creature);
    }
}
