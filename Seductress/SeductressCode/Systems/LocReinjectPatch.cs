using BaseLib.Abstracts;
using BaseLib.Patches.Localization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// Puts this mod's in-code text back after the game rebuilds its loc tables.
///
/// BaseLib adds <see cref="ILocalizationProvider"/> text exactly once, in a postfix on
/// <c>ModelDb.Init</c>. Anything that calls <c>LocManager.SetLanguage</c> afterwards - the language
/// dropdown, or another mod reloading text (SkinChanger does it when it applies a skin) - swaps in
/// freshly loaded tables without it, and every card, power, relic and orb here falls back to its
/// raw key ("SEDUCTRESS-PUMP.title"). Seen on game v0.113.0, 2026-11-13.
///
/// Only this assembly's models are touched; other mods' text is theirs to restore.
/// </summary>
[HarmonyPatch(typeof(LocManager), "SetLanguageInternal")]
internal static class LocReinjectPatch
{
    // Same table choice BaseLib's ModelLocPatch makes, for the model kinds this mod defines.
    private static readonly Dictionary<string, string> CategoryToLocTable = new()
    {
        { ModelId.SlugifyCategory(nameof(CardModel)), "cards" },
        { ModelId.SlugifyCategory(nameof(CharacterModel)), "characters" },
        { ModelId.SlugifyCategory(nameof(OrbModel)), "orbs" },
        { ModelId.SlugifyCategory(nameof(PotionModel)), "potions" },
        { ModelId.SlugifyCategory(nameof(PowerModel)), "powers" },
        { ModelId.SlugifyCategory(nameof(RelicModel)), "relics" },
    };

    [HarmonyPostfix]
    private static void Reinject(LocManager __instance)
    {
        try
        {
            var mine = typeof(LocReinjectPatch).Assembly;
            int models = 0;
            // Empty on the very first load (ModelDb.Init has not run yet); BaseLib covers that one.
            foreach (var model in ModelDb.All)
            {
                if (model.GetType().Assembly != mine || model is not ILocalizationProvider provider) continue;
                var loc = provider.Localization;
                if (loc == null) continue;
                string? tableName = provider.LocTable ?? CategoryToLocTable.GetValueOrDefault(model.Id.Category);
                if (tableName == null)
                {
                    MainFile.Logger.Warn($"[Seductress] no loc table known for {model.Id}; its text was not restored");
                    continue;
                }
                var entries = new Dictionary<string, string>();
                foreach (var (key, text) in loc)
                    entries[$"{model.Id.Entry}.{key}"] = SimpleLoc.TrySimplify(text);
                __instance.GetTable(tableName).MergeWith(entries);
                models++;
            }
            if (models > 0) MainFile.Logger.Info($"[Seductress] loc tables reloaded; restored text for {models} models");
        }
        catch (Exception ex)
        {
            MainFile.Logger.Error($"[Seductress] restoring text after a loc reload failed: {ex}");
        }
    }
}
