using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;

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

    // Toy Box (added in Sex 0.1.0): the three toy options give a different relic per character,
    // mirroring ToyBox.ActToyA/B/C. Index 0 = top (TOY_A), 1 = middle (TOY_B), 2 = bottom (TOY_C).
    private const string ToyBoxOptionPrefix = "TOY_BOX.pages.INITIAL.options.TOY_";
    private static readonly string[] ToyIronclad    = { "HYPNOSIS_EYE_MASK", "DAIRY_COW_SUIT", "LEWD_FOREPLAY" };
    private static readonly string[] ToySilent      = { "CLITORIS_LEASH", "LEWD_YARN", "PORTABLE_LEWD_POISON" };
    private static readonly string[] ToyDefect      = { "PLEASUREDELAY_PROGRAM", "WILD_SERVICE_PROGRAM", "DARK_ANAL_BEAD_STRING" };
    private static readonly string[] ToyNecrobinder = { "SOUL_LUST_POISON", "LUST_HAND", "DESIRE_COLLAR" };
    private static readonly string[] ToyRegent      = { "LUST_KING_SWORD", "FLATTER_PLUG_TAIL", "BEG_TO_MASTER" };
    private static readonly string[] ToyOther       = { "ROPED_HYPNOSIS", "OVER_DRIVE_SEX_TOY", "ENDLESS_DESIRE" };

    private static string? ToyBoxRelicFor(EventModel ev, string textKey)
    {
        if (!textKey.StartsWith(ToyBoxOptionPrefix, StringComparison.Ordinal) || textKey.Length != ToyBoxOptionPrefix.Length + 1) return null;
        int slot = textKey[^1] - 'A';
        if (slot < 0 || slot > 2) return null;
        var character = ev.Owner?.Character;
        if (character == null) return null;
        string[] set = character switch
        {
            Ironclad => ToyIronclad,
            Silent => ToySilent,
            Defect => ToyDefect,
            Necrobinder => ToyNecrobinder,
            Regent => ToyRegent,
            _ => ToyOther,
        };
        return set[slot];
    }

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

                string? relicId = ToyBoxRelicFor(__instance, option.TextKey);
                if (relicId != null || RelicByOption.TryGetValue(option.TextKey, out relicId))
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
