using Godot;
using HarmonyLib;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace CsvCardAdjustments
{
    [HarmonyPatch(typeof(ModelDb), "Init")]
    public static class CsvModInjectorInitPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            MainFile.Logger.Info("[CsvCardAdjustments] Applying manual base overrides to ModelDb...");

            // ----- MANUAL BASE OVERRIDES -----
            // 5. Normality Base Override (Cost 2, Not Unplayable)
            var normalityModel = ModelDb.Card<Normality>();
            if (normalityModel != null)
            {
                var baseCostField = typeof(CardEnergyCost).GetField("_base", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (baseCostField != null)
                {
                    baseCostField.SetValue(normalityModel.EnergyCost, 2);
                    MainFile.Logger.Info("[CsvCardAdjustments] Set Normality base cost to 2 via reflection.");
                }
            }

            var venerateModel = ModelDb.Card<Venerate>();
            if (venerateModel != null)
            {
                MainFile.Logger.Info($"[CsvCardAdjustments] Injecting Venerate Cost.");
                var baseCostField = typeof(CardEnergyCost).GetField("_base", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (baseCostField != null)
                {
                    baseCostField.SetValue(venerateModel.EnergyCost, 1); // Make it cost 1 always
                }
            }

            var apotheosisModel = ModelDb.Card<Apotheosis>();
            if (apotheosisModel != null)
            {
                MainFile.Logger.Info($"[CsvCardAdjustments] Injecting Apotheosis Cost.");
                var baseCostField = typeof(CardEnergyCost).GetField("_base", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (baseCostField != null)
                {
                    baseCostField.SetValue(apotheosisModel.EnergyCost, 1); // Fix base cost to 1. OnUpgrade subtracts 1 natively.
                }
            }

            // 6. Brightest Flame Base Override (Draw 1 instead of 2)
            var brightestFlameModel = ModelDb.Card<BrightestFlame>();
            if (brightestFlameModel != null)
            {
                var cardsVar = (CardsVar)brightestFlameModel.DynamicVars["Cards"];
                var valueField = typeof(DynamicVar).GetField("_baseValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (valueField != null)
                {
                    valueField.SetValue(cardsVar, 1m);
                    MainFile.Logger.Info("[CsvCardAdjustments] Reduced Brightest Flame base draw to 1 via Field Reflection.");
                }
            }

            // 7. The Smith Base Override (Base Forge 15 instead of whatever it is)
            var theSmithModel = ModelDb.Card<TheSmith>();
            if (theSmithModel != null)
            {
                if (theSmithModel.DynamicVars.TryGetValue("Forge", out var forgeVar))
                {
                    var valueField = typeof(DynamicVar).GetField("_baseValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (valueField != null)
                    {
                        valueField.SetValue(forgeVar, 15m);
                        MainFile.Logger.Info("[CsvCardAdjustments] Set The Smith base forge to 15 via Field Reflection.");
                    }
                }
            }

            // 8. LegionOfBone Base Override (Summon 10 instead of 6)
            var legionModel = ModelDb.Card<LegionOfBone>();
            if (legionModel != null)
            {
                if (legionModel.DynamicVars.TryGetValue("Summon", out var summonVar))
                {
                    var valueField = typeof(DynamicVar).GetField("_baseValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (valueField != null)
                    {
                        valueField.SetValue(summonVar, 10m);
                        MainFile.Logger.Info("[CsvCardAdjustments] Set LegionOfBone base summon to 10 via Field Reflection.");
                    }
                }
            }

            // 9. Huddle Up Base Override (Move to Silent Pool, set to Uncommon)
            var huddleUpModel = ModelDb.Card<HuddleUp>();
            if (huddleUpModel != null)
            {
                var silentPool = ModelDb.AllCharacters.FirstOrDefault(c => c is MegaCrit.Sts2.Core.Models.Characters.Silent)?.CardPool;
                if (silentPool != null)
                {
                    typeof(CardModel).GetField("_pool", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(huddleUpModel, silentPool);
                    typeof(CardModel).GetField("<Rarity>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(huddleUpModel, CardRarity.Uncommon);
                    MainFile.Logger.Info("[CsvCardAdjustments] Intercepted HuddleUp memory bindings (Silent Pool, Uncommon).");
                }
            }
            // 10. Hide default Flanking
            var flankingModel = ModelDb.Card<Flanking>();
            if (flankingModel != null)
            {
                typeof(CardModel).GetField("<Rarity>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.SetValue(flankingModel, CardRarity.Token);
                MainFile.Logger.Info("[CsvCardAdjustments] Flanking rarity forced to Special natively.");
            }
            
            // 12. Register BaseLib Custom Models Natively (AUTOMATED VIA MODELDB PRELOAD)
            
            // ----- BATCH 2 TEXT INJECTIONS -----
            // Text overlays are now provided natively via the CsvCardAdjustments/localization/eng/cards.json file for ModManager merging!

            // Dump cards
            LocDumper.DumpCards();
        }
    }

    // Explicitly remove "Unplayable" from Normality's CanonicalKeywords
    [HarmonyPatch(typeof(Normality), "get_CanonicalKeywords")]
    public static class NormalityKeywordsPatch
    {
        public static bool Prefix(ref IEnumerable<CardKeyword> __result)
        {
            // By returning an empty list, we remove the Unplayable keyword
            __result = new List<CardKeyword>();
            return false; // Skip original getter
        }
    }
}
