using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace SexFixes;

/// <summary>
/// The Sex mod builds every EventOption with an empty hover-tip list, so the player cannot see
/// what a relic or potion does before choosing it. EventModel.SetEventState is called both for
/// the initial page and for every later page, so a single prefix covers all of them.
/// Everything is keyed by localisation text key / model id strings: no compile-time reference
/// to Sex.dll, and a missing model simply means no tip is added.
/// </summary>
[HarmonyPatch(typeof(EventModel), "SetEventState")]
public static class EventOptionTipsPatch
{
    // option text key -> relic model entry id (class name in SCREAMING_SNAKE_CASE)
    private static readonly Dictionary<string, string> RelicByOption = new()
    {
        ["BUTT_WALL.pages.ALL.options.LEFT_THING"] = "AMETHYST_AUBERGINE",
        ["BUTT_WALL.pages.ALL.options.RIGHT_THING"] = "SHOCK_EGG",
        ["JUICE_DRINKING.pages.INITIAL.options.DRINK_IT"] = "POTENT_LUST_POTION",
        ["JUICE_DRINKING.pages.INITIAL.options.THAT_CUP"] = "ENDLESS_JUICE_CUP",
        ["JUICE_DRINKING.pages.INITIAL.options.THAT_CUP_LOCKED"] = "ENDLESS_JUICE_CUP",
        ["LOST_GOD_TEMPLE.pages.INITIAL.options.THE_MIRROR"] = "LUST_MIRROR",
        ["LOST_GOD_TEMPLE.pages.INITIAL.options.THE_MIRROR_LOCKED"] = "LUST_MIRROR",
        ["LOST_MIRROR.pages.ALL.options.BREAK_IT"] = "BROKEN_MIRROR",
        ["MILK_GIVER.pages.ALL.options.STILL_DRINKING"] = "BREAST_PUMP",
        ["SPECIAL_MEDICINE.pages.INITIAL.options.THE_RED"] = "GIANT_BODY",
        ["SPECIAL_MEDICINE.pages.INITIAL.options.THE_BLUE"] = "LITTLE_CUTE",
    };

    private static readonly Dictionary<string, string> PotionByOption = new()
    {
        ["SECRET_ROOM.pages.INITIAL.options.JUST_RIGHT"] = "THICK_SEMEN_POTION",
    };

    private const string MilkGiverSecondDrinkOption = "MILK_GIVER.pages.ALL.options.STILL_DRINKING";
    private const string MilkGiverSecondDrinkPage = "MILK_GIVER.pages.ALL.KEEP_DRINKING.description";

    [HarmonyPrefix]
    public static void Prefix(EventModel __instance, ref LocString description, ref IEnumerable<EventOption> eventOptions)
    {
        try
        {
            if (eventOptions == null) return;
            List<EventOption> options = eventOptions.ToList();
            eventOptions = options;
            if (options.Count == 0) return;

            // Only touch the Sex mod's events.
            string eventName = __instance.GetType().FullName ?? "";
            if (!eventName.StartsWith("Sex.Core.Models.Events.", StringComparison.Ordinal)) return;

            foreach (EventOption option in options)
            {
                if (option.TextKey == null) continue;
                if (option.HoverTips != null && option.HoverTips.Any()) continue;

                if (RelicByOption.TryGetValue(option.TextKey, out string? relicId))
                {
                    RelicModel? canonical = ModelDb.AllRelics.FirstOrDefault(r => r.Id.Entry == relicId);
                    if (canonical == null) continue;
                    RelicModel relic = canonical.ToMutable();
                    if (__instance.Owner != null) relic.Owner = __instance.Owner;
                    option.HoverTips = HoverTipFactory.FromRelic(relic).ToList();
                    option.WithRelic(relic);
                }
                else if (PotionByOption.TryGetValue(option.TextKey, out string? potionId))
                {
                    PotionModel? potion = ModelDb.AllPotions.FirstOrDefault(p => p.Id.Entry == potionId);
                    if (potion == null) continue;
                    option.HoverTips = new List<IHoverTip> { HoverTipFactory.FromPotion(potion) };
                }
            }

            // Milk Giver: the author wrote a distinct page for the second drink but KeepDrinking()
            // re-uses the first drink's page. The second-drink page is the only one whose options
            // include STILL_DRINKING, so swap the description there.
            if (eventName.EndsWith(".MilkGiver", StringComparison.Ordinal)
                && options.Any(o => o.TextKey == MilkGiverSecondDrinkOption))
            {
                description = new LocString(__instance.LocTable, MilkGiverSecondDrinkPage);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"[SexFixes] SetEventState patch failed: {e}");
        }
    }
}
