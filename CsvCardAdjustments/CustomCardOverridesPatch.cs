using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Combat;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Factories;


namespace CsvCardAdjustments
{
    // ----- FALLING STAR OVERRIDE -----
    // Request: Upgraded costs 1 star and deals 10 damage. (Base deals 7)
    // The original `FallingStar.OnUpgrade()` just adds 4. We will replace it via a Harmony Prefix 
    // to add 3 instead and then rewrite the `BaseStarCost` field via Reflection.
    [HarmonyPatch(typeof(FallingStar), "OnUpgrade")]
    public static class FallingStarPatch
    {
        public static bool Prefix(FallingStar __instance)
        {
            // First we need to get the DamageVar without calling the protected base methods.
            // But we know it's a dynamic variable in `__instance.DynamicVars` dictionary.
            var damageVar = (DamageVar)__instance.DynamicVars["Damage"];
            damageVar.UpgradeValueBy(3m); // Upgrades from 7 base to 10

            // BaseStarCost is a property with a private setter we can hit with reflection
            var propInfo = typeof(CardModel).GetProperty("BaseStarCost", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (propInfo != null && propInfo.CanWrite)
            {
                propInfo.SetValue(__instance, 1);
            }

            return false; // Skip original game logic that adds +4 damage natively
        }
    }

    // ----- VENERATE OVERRIDE -----
    // Request: Upgraded retains and only does 2 stars.
    // The original `Venerate.OnUpgrade()` adds +1 star. We'll skip that logic and forcefully 
    // insert the Retain keyword.
    [HarmonyPatch(typeof(Venerate), "OnUpgrade")]
    public static class VeneratePatch
    {
        public static bool Prefix(Venerate __instance)
        {
            // To add a keyword like Retain, Slay the Spire 2 caches keywords. 
            // We use Reflection to insert the Retain keyword into the internal HashSet.
            var keywordField = typeof(CardModel).GetField("_keywords", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (keywordField != null)
            {
                var keywords = keywordField.GetValue(__instance) as System.Collections.Generic.HashSet<CardKeyword>;
                if (keywords != null)
                {
                    keywords.Add(CardKeyword.Retain);
                }
            }

            return false; // Skip the original logic that upgrades `StarsVar` by +1
        }
    }

    // ----- BODYGUARD OVERRIDE -----
    // Request: Upgraded additionally heals Osty for 2.
    // We hook OnPlay postfix to inject the Osty heal if it is upgraded.
    [HarmonyPatch(typeof(Bodyguard), "OnPlay")]
    public static class BodyguardPatch
    {
        public static void Postfix(Bodyguard __instance, System.Threading.Tasks.Task __result)
        {
            // We chain an asynchronous continuation to the original OnPlay Task.
            __result.ContinueWith(async (t) =>
            {
                if (__instance.CurrentUpgradeLevel > 0 && __instance.Owner.Osty != null)
                {
                    // Call the same method Spur uses to heal Osty
                    await MegaCrit.Sts2.Core.Commands.CreatureCmd.Heal(__instance.Owner.Osty, 2);
                }
            }, System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously);
        }
    }

    // ----- UNLEASH OVERRIDE -----
    // Request: Upgraded costs 0 energy but still deals 6 damage. (Base deals 6)
    // The original `Unleash.OnUpgrade()` adds +3 damage. We skip that logic, and forcefully 
    // modify the EnergyCost.
    [HarmonyPatch(typeof(Unleash), "OnUpgrade")]
    public static class UnleashPatch
    {
        public static bool Prefix(Unleash __instance)
        {
            __instance.EnergyCost.SetCustomBaseCost(0); // Cost becomes 0 explicitly
            return false; // Skips the +3 damage upgrade
        }
    }

    // ----- NORMALITY OVERRIDE (ShouldPlay) -----
    // Make it always able to be played itself, even if ShouldPreventCardPlay is true for other cards.
    [HarmonyPatch(typeof(Normality), "ShouldPlay")]
    public static class NormalityPlayPatch
    {
        public static void Postfix(Normality __instance, CardModel card, ref bool __result)
        {
            // If the card being checked is THIS Normality, let the player play it!
            if (card == __instance)
            {
                __result = true;
            }
        }
    }

    // ----- BRIGHTEST FLAME OVERRIDE -----
    // Request: Base draws 1 card, upgraded draws 2 cards.
    // Base modifications are handled in ModelDb.Init. Here, we intercept OnUpgrade to add only +1 draw instead of +1 energy logic.
    [HarmonyPatch(typeof(BrightestFlame), "OnUpgrade")]
    public static class BrightestFlamePatch
    {
        public static bool Prefix(BrightestFlame __instance)
        {
            var cardsVar = (CardsVar)__instance.DynamicVars["Cards"];
            cardsVar.UpgradeValueBy(1m); // Upgrades from our custom 1 base to 2

            // Do not upgrade energy
            return false; // Skip the original logic 
        }
    }

    // ==========================================
    [HarmonyPatch(typeof(CardModel), "get_Pool")]
    public static class PoolOverridePatch
    {
        public static void Postfix(CardModel __instance, ref CardPoolModel __result)
        {
            if (__instance is HuddleUp)
            {
                __result = ModelDb.AllCharacters.FirstOrDefault(c => c is MegaCrit.Sts2.Core.Models.Characters.Silent)?.CardPool;
            }
        }
    }

    [HarmonyPatch(typeof(CardModel), "get_Rarity")]
    public static class RarityOverridePatch
    {
        public static void Postfix(CardModel __instance, ref CardRarity __result)
        {
            if (__instance is HuddleUp)
            {
                __result = CardRarity.Uncommon;
            }
        }
    }

    [HarmonyPatch(typeof(CardModel), "get_RunAssetPaths")]
    public static class RunAssetPathsOverridePatch {
        public static void Postfix(CardModel __instance, ref IEnumerable<string> __result) {
            if (__instance is HuddleUp) {
                __result = new List<string> { "colorless/huddleup" };
            }
        }
    }

    [HarmonyPatch(typeof(CardModel), "get_PortraitPath")]
    public static class PortraitPathOverridePatch {
        public static void Postfix(CardModel __instance, ref string __result) {
        }
    }
    
    [HarmonyPatch(typeof(CardModel), "get_VisualCardPool")]
    public static class VisualCardPoolOverridePatch {
        public static void Postfix(CardModel __instance, ref CardPoolModel __result) {
            if (__instance is HuddleUp) {
                __result = ModelDb.AllCardPools.FirstOrDefault(p => p.IsColorless);
            }
        }
    }

    // Deleted HuddleUpKeywordsPatch here to consolidate into CardModel hooks below

    [HarmonyPatch(typeof(CardModel), "get_CanonicalKeywords")]
    public static class CanonicalKeywordsOverridePatch
    {
        public static void Postfix(CardModel __instance, ref IEnumerable<CardKeyword> __result)
        {
            if (__instance is HuddleUp && __result != null && !__result.Contains(CardKeyword.Exhaust))
            {
                __result = __result.Append(CardKeyword.Exhaust);
            }
        }
    }
    // Deprecated Flanking/Afterlife property clones extracted cleanly into BaseLib Subclassing.







    public static class SnakebiteTracker
    {
        public static System.Runtime.CompilerServices.ConditionalWeakTable<Snakebite, StrongBox<decimal>> AddedPoison = new();

        public class StrongBox<T> { public T Value = default!; }
    }

    [HarmonyPatch(typeof(Snakebite), "OnPlay")]
    public static class SnakebiteOnPlayResetPatch
    {
        public static void Postfix(Snakebite __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref System.Threading.Tasks.Task __result)
        {
            __result = __result.ContinueWith((t) => 
            {
                if (SnakebiteTracker.AddedPoison.TryGetValue(__instance, out var box) && box.Value > 0m)
                {
                    var valueField = typeof(DynamicVar).GetField("_baseValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (valueField != null)
                    {
                        decimal currentVal = __instance.DynamicVars["PoisonPower"].BaseValue;
                        valueField.SetValue(__instance.DynamicVars["PoisonPower"], currentVal - box.Value);
                    }
                    box.Value = 0m;
                    MainFile.Logger.Info("[CsvCardAdjustments] Reset Snakebite added poison after play.");
                }
            }, System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously);
        }
    }
    [HarmonyPatch(typeof(Hook), "AfterCardRetained")]
    public static class SnakebiteOnRetainedPatch
    {
        public static void Postfix(CombatState combatState, CardModel card, ref System.Threading.Tasks.Task __result)
        {
            if (card is Snakebite snakebite)
            {
                __result = __result.ContinueWith(async (t) => 
                {
                    decimal poisonIncrease = snakebite.CurrentUpgradeLevel > 0 ? 2m : 1m;
                    
                    var box = SnakebiteTracker.AddedPoison.GetOrCreateValue(snakebite);
                    box.Value += poisonIncrease;

                    var valueField = typeof(DynamicVar).GetField("_baseValue", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (valueField != null)
                    {
                        decimal currentVal = snakebite.DynamicVars["PoisonPower"].BaseValue;
                        valueField.SetValue(snakebite.DynamicVars["PoisonPower"], currentVal + poisonIncrease);
                    }

                    MainFile.Logger.Info($"[CsvCardAdjustments] Snakebite Retained. Added {poisonIncrease} poison. Total added: {box.Value}");
                    await MegaCrit.Sts2.Core.Commands.Cmd.CustomScaledWait(0.1f, 0.2f);
                }).Unwrap();
            }
        }
    }

    // ----- CONQUEROR OVERRIDE -----
    // Request: Upgrade applies debuff to all enemies.
    [HarmonyPatch(typeof(Conqueror), "OnUpgrade")]
    public static class ConquerorUpgradePatch
    {
        public static void Postfix(Conqueror __instance)
        {
            var propInfo = typeof(CardModel).GetProperty("TargetType", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (propInfo != null && propInfo.CanWrite)
            {
                propInfo.SetValue(__instance, TargetType.AllEnemies);
            }
        }
    }

    [HarmonyPatch(typeof(Conqueror), "OnPlay")]
    public static class ConquerorPlayPatch
    {
        public static bool Prefix(Conqueror __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref System.Threading.Tasks.Task __result)
        {
            if (__instance.CurrentUpgradeLevel > 0)
            {
                __result = CustomOnPlay(__instance, choiceContext, cardPlay);
                return false;
            }
            return true;
        }

        private static async System.Threading.Tasks.Task CustomOnPlay(Conqueror __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(__instance.Owner.Creature, "Cast", __instance.Owner.Character.CastAnimDelay);
            await MegaCrit.Sts2.Core.Commands.ForgeCmd.Forge(__instance.DynamicVars["Forge"].IntValue, __instance.Owner, __instance);
            
            foreach (var enemy in __instance.CombatState.Enemies)
            {
                if (enemy.IsAlive)
                {
                    await MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.ConquerorPower>(enemy, 1m, __instance.Owner.Creature, __instance);
                }
            }
        }
    }

    // ----- FAN OF KNIVES OVERRIDE -----
    // Request: Gain Innate on upgrade.
    [HarmonyPatch(typeof(FanOfKnives), "OnUpgrade")]
    public static class FanOfKnivesUpgradePatch
    {
        public static bool Prefix(FanOfKnives __instance)
        {
            var keywordField = typeof(CardModel).GetField("_keywords", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (keywordField != null)
            {
                var keywords = keywordField.GetValue(__instance) as System.Collections.Generic.HashSet<CardKeyword>;
                if (keywords != null)
                {
                    keywords.Add(CardKeyword.Innate);
                }
            }

            // Do the normal upgrade
            __instance.DynamicVars["Shivs"].UpgradeValueBy(1m);
            return false; // Skip original
        }
    }

    // ----- EXTERMINATE OVERRIDE -----
    // Request: Hit an additional time (5) instead of increasing damage.
    [HarmonyPatch(typeof(Exterminate), "OnUpgrade")]
    public static class ExterminateUpgradePatch
    {
        public static bool Prefix(Exterminate __instance)
        {
            __instance.DynamicVars["Repeat"].UpgradeValueBy(1m);
            return false; // Skip original damage upgrade
        }
    }

    // ----- BATCH 3 -----

    // ----- FRIENDSHIP OVERRIDE -----
    // Request: Gain Innate on upgrade
    [HarmonyPatch(typeof(Friendship), "OnUpgrade")]
    public static class FriendshipUpgradePatch
    {
        public static void Postfix(Friendship __instance)
        {
            __instance.AddKeyword(CardKeyword.Innate);
            MainFile.Logger.Info("[CsvCardAdjustments] Friendship upgraded: gained Innate.");
        }
    }

    // ----- DEATH'S DOOR OVERRIDE -----
    // Request: Upgrade gains block 3 additional times (not 2). Base block stays at 6.
    [HarmonyPatch(typeof(DeathsDoor), "OnUpgrade")]
    public static class DeathsDoorUpgradePatch
    {
        public static void Postfix(DeathsDoor __instance)
        {
            __instance.DynamicVars.Block.UpgradeValueBy(-1m); // Nullify base block upgrade
            __instance.DynamicVars.Repeat.UpgradeValueBy(1m); // Add 1 more repeat for 3 extra triggers on doom
            MainFile.Logger.Info("[CsvCardAdjustments] Death's Door upgraded: Retained 6 block, upgraded Repeat to 3.");
        }
    }

    // ----- SEVERANCE OVERRIDE -----
    // Request: Upgrade adds a soul to exhaust pile as well
    [HarmonyPatch(typeof(Severance), "OnPlay")]
    public static class SeverancePlayPatch
    {
        public static bool Prefix(Severance __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref System.Threading.Tasks.Task __result)
        {
            if (__instance.CurrentUpgradeLevel > 0)
            {
                __result = CustomSeveranceUpgradedOnPlay(__instance, choiceContext, cardPlay);
                return false;
            }
            return true;
        }

        private static async System.Threading.Tasks.Task CustomSeveranceUpgradedOnPlay(Severance __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(__instance.DynamicVars["Damage"].BaseValue).FromCard(__instance).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            
            List<Soul> souls = Soul.Create(__instance.Owner, 4, __instance.CombatState).ToList();
            var drawResult = await CardPileCmd.AddGeneratedCardToCombat(souls[0], PileType.Draw, addedByPlayer: true, CardPilePosition.Random);
            var discardResult = await CardPileCmd.AddGeneratedCardToCombat(souls[1], PileType.Discard, addedByPlayer: true);
            var handResult = await CardPileCmd.AddGeneratedCardToCombat(souls[2], PileType.Hand, addedByPlayer: true);
            var exhaustResult = await CardPileCmd.AddGeneratedCardToCombat(souls[3], PileType.Exhaust, addedByPlayer: true);
            
            CardCmd.PreviewCardPileAdd(new[] { drawResult, discardResult, handResult, exhaustResult });
            MainFile.Logger.Info("[CsvCardAdjustments] Severance upgraded OnPlay: Generated extra Soul into Exhaust pile natively.");
        }
    }

    // ----- CLEANSE OVERRIDE -----
    // Request: Exhaust 2 cards from Hand (Base and Upgraded).
    [HarmonyPatch(typeof(Cleanse), "OnPlay")]
    public static class CleansePlayPatch
    {
        public static bool Prefix(Cleanse __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref System.Threading.Tasks.Task __result)
        {
            __result = CustomCleansePlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async System.Threading.Tasks.Task CustomCleansePlay(Cleanse __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            MainFile.Logger.Info("[CsvCardAdjustments] Custom Cleanse Play Triggered! Targeting Draw Pile, 0 min, 2 max.");
            await MegaCrit.Sts2.Core.Commands.CreatureCmd.TriggerAnim(__instance.Owner.Creature, "Cast", __instance.Owner.Character.CastAnimDelay);

            if (__instance.DynamicVars.TryGetValue("Summon", out var summonVar))
            {
                await MegaCrit.Sts2.Core.Commands.OstyCmd.Summon(choiceContext, __instance.Owner, summonVar.BaseValue, __instance);
            }

            var cardsInPile = PileType.Draw.GetPile(__instance.Owner).Cards.ToList();
            if (cardsInPile.Count > 0)
            {
                int cardsToExhaust = System.Math.Min(2, cardsInPile.Count);
                var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, cardsToExhaust)
                {
                    RequireManualConfirmation = true, // Always require confirmation since Min is 0
                    Cancelable = true // Cancelable since min is 0
                };
                
                var chosenCards = await CardSelectCmd.FromSimpleGrid(choiceContext, cardsInPile, __instance.Owner, prefs);
                
                if (chosenCards != null)
                {
                    foreach (var c in chosenCards)
                    {
                        await CardCmd.Exhaust(choiceContext, c, false);
                    }
                }
            }
        }
    }

    // ----- BATCH 4 -----

    // ----- PARRY OVERRIDE -----
    // Request: Give block equal to 1/4 of Sovereign Blade's attack when played. Upgrades to 1/2.
    // Instead of using the native 6 / 9 ParryPower, we'll store the multiplier in Amount. 
    // Unupgraded = 25 (1/4). Upgraded = 50 (1/2).
    [HarmonyPatch(typeof(Parry), "OnUpgrade")]
    public static class ParryUpgradePatch
    {
        public static bool Prefix(Parry __instance)
        {
            __instance.DynamicVars["ParryPower"].UpgradeValueBy(20m); // 20 + 20 = 40%
            return false; // Skip original +3 upgrade
        }
    }

    [HarmonyPatch(typeof(Parry), "OnPlay")]
    public static class ParryPlayPatch
    {
        public static bool Prefix(Parry __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref System.Threading.Tasks.Task __result)
        {
            __result = CustomParryOnPlay(__instance, choiceContext, cardPlay);
            return false; // Skip original which applies default 6
        }

        private static async System.Threading.Tasks.Task CustomParryOnPlay(Parry __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.TriggerAnim(__instance.Owner.Creature, "Cast", __instance.Owner.Character.CastAnimDelay);
            
            // Unupgraded base is 6. If not upgraded and someone else hasn't modified it to 25, let's just force 25 for 1/4. 
            // Upgraded brings it to 50 via the OnUpgrade hook.
            decimal multiplier = __instance.CurrentUpgradeLevel > 0 ? 40m : 20m; 
            
            await PowerCmd.Apply<ParryPower>(__instance.Owner.Creature, multiplier, __instance.Owner.Creature, __instance);
        }
    }

    [HarmonyPatch(typeof(ParryPower), "AfterSovereignBladePlayed")]
    public static class ParryPowerInterceptPatch
    {
        public static bool Prefix(ParryPower __instance, Creature dealer, IEnumerable<DamageResult> damageResults, ref System.Threading.Tasks.Task __result)
        {
            __result = CustomAfterSovereignBladePlayed(__instance, dealer, damageResults);
            return false; // Skip original flat block
        }

        private static async System.Threading.Tasks.Task CustomAfterSovereignBladePlayed(ParryPower __instance, Creature dealer, IEnumerable<DamageResult> damageResults)
        {
            if (dealer != null && dealer == __instance.Owner)
            {
                __instance.GetType().GetMethod("Flash", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.Invoke(__instance, null);
                
                decimal baseAttack = 0m;
                var firstHit = damageResults.FirstOrDefault();
                if (firstHit != null)
                {
                    baseAttack = firstHit.TotalDamage; 
                }

                decimal multiplier = __instance.Amount / 100m; // 25 for 1/4, 50 for 1/2
                decimal blockGained = Math.Floor(baseAttack * multiplier);

                if (blockGained > 0)
                {
                    await CreatureCmd.GainBlock(dealer, blockGained, ValueProp.Unpowered, null);
                    MainFile.Logger.Info($"[CsvCardAdjustments] ParryPower proc'd: Attack was {baseAttack}, multiplier {multiplier}. Granted {blockGained} Block.");
                }
            }
        }
    }

    // ----- HEIRLOOM HAMMER OVERRIDE -----
    // Request: Upgraded version upgrades the colorless card it copies (original + copy).
    [HarmonyPatch(typeof(HeirloomHammer), "OnPlay")]
    public static class HeirloomHammerPlayPatch
    {
        public static bool Prefix(HeirloomHammer __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref System.Threading.Tasks.Task __result)
        {
            if (__instance.CurrentUpgradeLevel > 0)
            {
                __result = CustomHeirloomHammerPlay(__instance, choiceContext, cardPlay);
                return false;
            }
            return true;
        }

        private static async System.Threading.Tasks.Task CustomHeirloomHammerPlay(HeirloomHammer __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await DamageCmd.Attack(__instance.DynamicVars.Damage.BaseValue).FromCard(__instance).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
                .Execute(choiceContext);
            
            var promptObj = typeof(CardModel).GetProperty("SelectionScreenPrompt", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(__instance);
            var prompt = promptObj is MegaCrit.Sts2.Core.Localization.LocString ls ? ls : MegaCrit.Sts2.Core.CardSelection.CardSelectorPrefs.ExhaustSelectionPrompt;
            CardModel selection = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(prompt, 1), context: choiceContext, player: __instance.Owner, filter: (CardModel c) => c.VisualCardPool.IsColorless, source: __instance)).FirstOrDefault();
            
            if (selection != null)
            {
                // Upgrade the original card in hand
                if (selection.CurrentUpgradeLevel < selection.MaxUpgradeLevel)
                {
                    selection.UpgradeInternal();
                    selection.FinalizeUpgradeInternal();
                }

                for (int i = 0; i < __instance.DynamicVars.Repeat.IntValue; i++)
                {
                    CardModel card = selection.CreateClone();
                    // Just in case clone didn't inherit the mid-combo upgrade correctly, upgrade it too.
                    if (card.CurrentUpgradeLevel < card.MaxUpgradeLevel && card.CurrentUpgradeLevel < selection.CurrentUpgradeLevel)
                    {
                        card.UpgradeInternal();
                        card.FinalizeUpgradeInternal();
                    }
                    await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
                }
                MainFile.Logger.Info("[CsvCardAdjustments] Heirloom Hammer upgraded OnPlay: Upgraded selected source and clones.");
            }
        }
    }

    // ----- TAG TEAM OVERRIDE -----


    // ----- SEEKER STRIKE OVERRIDE -----
    // Request: Upgrade picks from 5 cards instead of base (3).
    [HarmonyPatch(typeof(SeekerStrike), "OnUpgrade")]
    public static class SeekerStrikeUpgradePatch
    {
        public static void Postfix(SeekerStrike __instance)
        {
            // Upgrades natively increase Damage by 3. We additionally increase Cards fetched by 2.
            __instance.DynamicVars["Cards"].UpgradeValueBy(2m);
            MainFile.Logger.Info("[CsvCardAdjustments] Seeker Strike upgraded: Increased Cards var by 2.");
        }
    }
    // ----- THE SMITH OVERRIDE -----
    // Request: Forges 15 (base) or 25 (upgraded) and upgrades Sovereign Blade everywhere.
    // Instead of completely reconstructing OnPlay, we modify the base Forge value in CsvModInjector
    // and just override OnUpgrade to add +10 Forge (skipping native upgrade behavior).
    [HarmonyPatch(typeof(TheSmith), "OnUpgrade")]
    public static class TheSmithUpgradePatch
    {
        public static bool Prefix(TheSmith __instance)
        {
            if (__instance.DynamicVars.TryGetValue("Forge", out var forgeVar))
            {
                forgeVar.UpgradeValueBy(10m);
            }
            return false; // Suppress original upgrade logic
        }
    }

    [HarmonyPatch(typeof(TheSmith), "OnPlay")]
    public static class TheSmithPlayPatch
    {
        public static bool Prefix(TheSmith __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref System.Threading.Tasks.Task __result)
        {
            __result = CustomOnPlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async System.Threading.Tasks.Task CustomOnPlay(TheSmith __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            decimal forgeAmount = __instance.DynamicVars["Forge"].IntValue;
            var forgedCards = await ForgeCmd.Forge(forgeAmount, __instance.Owner, __instance);

            // Now upgrade the blades.
            if (forgedCards != null)
            {
                foreach (var card in forgedCards)
                {
                    if (card is SovereignBlade) CardCmd.Upgrade(card);
                }
            }
            foreach (var card in __instance.Owner.Deck.Cards)
            {
                if (card is SovereignBlade) CardCmd.Upgrade(card);
            }
        }
    }

    // ----- PARTICLE WALL OVERRIDE -----


    // ----- FOREGONE CONCLUSION OVERRIDE -----
    // Request: Both base and upgraded seek 2. Upgraded retains.
    [HarmonyPatch(typeof(ForegoneConclusion), "OnUpgrade")]
    public static class ForegoneConclusionUpgradePatch
    {
        public static bool Prefix(ForegoneConclusion __instance)
        {
            var keywordField = typeof(CardModel).GetField("_keywords", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (keywordField != null)
            {
                var keywords = keywordField.GetValue(__instance) as System.Collections.Generic.HashSet<CardKeyword>;
                if (keywords != null) keywords.Add(CardKeyword.Retain);
            }
            return false; // Skip original logic (prevents Cards from scaling to 3)
        }
    }

    // ==========================================
    // ----- BATCH 6 FATAL OVERRIDES -----
    // ==========================================

    [HarmonyPatch(typeof(Feed), "OnPlay")]
    public static class FeedPlayPatch
    {
        public static void Postfix(Feed __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, System.Threading.Tasks.Task __result)
        {
            __result.ContinueWith(async (t) => 
            {
                if (cardPlay.Target != null && cardPlay.Target.IsDead && cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal()))
                {
                    foreach (var player in __instance.CombatState.Players)
                    {
                        if (player != __instance.Owner)
                        {
                            await CreatureCmd.GainMaxHp(player.Creature, __instance.DynamicVars.MaxHp.IntValue);
                        }
                    }
                }
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    [HarmonyPatch(typeof(HandOfGreed), "OnPlay")]
    public static class HandOfGreedPlayPatch
    {
        public static void Postfix(HandOfGreed __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, System.Threading.Tasks.Task __result)
        {
            __result.ContinueWith(async (t) => 
            {
                if (cardPlay.Target != null && cardPlay.Target.IsDead && cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal()))
                {
                    foreach (var player in __instance.CombatState.Players)
                    {
                        if (player != __instance.Owner)
                        {
                            await PlayerCmd.GainGold(__instance.DynamicVars["Gold"].IntValue, player);
                        }
                    }
                }
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    [HarmonyPatch(typeof(TheHunt), "OnPlay")]
    public static class TheHuntPlayPatch
    {
        public static void Postfix(TheHunt __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, System.Threading.Tasks.Task __result)
        {
            __result.ContinueWith(async (t) => 
            {
                if (cardPlay.Target != null && cardPlay.Target.IsDead && cardPlay.Target.Powers.All(p => p.ShouldOwnerDeathTriggerFatal()))
                {
                    var room = __instance.CombatState.RunState.CurrentRoom;
                    foreach (var player in __instance.CombatState.Players)
                    {
                        if (player != __instance.Owner)
                        {
                            if (room is MegaCrit.Sts2.Core.Rooms.CombatRoom cr)
                            {
                                cr.AddExtraReward(player, new MegaCrit.Sts2.Core.Rewards.CardReward(MegaCrit.Sts2.Core.Runs.CardCreationOptions.ForRoom(player, cr.RoomType), 3, player));
                            }
                            await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.TheHuntPower>(player.Creature, 1m, player.Creature, __instance);
                        }
                    }
                }
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }
    }


    [HarmonyPatch(typeof(LegionOfBone), "OnUpgrade")]
    public static class LegionOfBoneUpgradePatch
    {
        public static bool Prefix(LegionOfBone __instance)
        {
            if (__instance.DynamicVars.TryGetValue("Summon", out var summonVar))
            {
                summonVar.UpgradeValueBy(5m); // Jumps from Base 10 to Upgraded 15 (native is +2).
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Largesse), "OnPlay")]
    public static class LargessePlayPatch
    {
        public static bool Prefix(Largesse __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref System.Threading.Tasks.Task __result)
        {
            __result = CustomLargessePlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async System.Threading.Tasks.Task CustomLargessePlay(Largesse __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            System.ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await CreatureCmd.TriggerAnim(__instance.Owner.Creature, "Cast", __instance.Owner.Character.CastAnimDelay);

            var allyPlayer = cardPlay.Target.Player;
            if (allyPlayer == null) return;

            var colorlessPool = ModelDb.AllCardPools.FirstOrDefault(p => p.IsColorless);
            if (colorlessPool == null) return;

            var unlockedCards = colorlessPool.GetUnlockedCards(__instance.Owner.UnlockState, __instance.Owner.RunState.CardMultiplayerConstraint);
            var combatCardGeneration = __instance.Owner.RunState.Rng.CombatCardGeneration;
            var distinctCards = CardFactory.GetDistinctForCombat(__instance.Owner, unlockedCards, 3, combatCardGeneration).ToList();

            if (__instance.IsUpgraded)
            {
                foreach (var c in distinctCards) CardCmd.Upgrade(c);
            }

            var chosenCard = await CardSelectCmd.FromChooseACardScreen(choiceContext, distinctCards, __instance.Owner, false);

            if (chosenCard != null)
            {
                var ownerField = typeof(MegaCrit.Sts2.Core.Models.CardModel).GetField("_owner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (ownerField != null)
                {
                    ownerField.SetValue(chosenCard, allyPlayer);
                }
                else
                {
                    chosenCard.Owner = allyPlayer;
                }

                await CardPileCmd.AddGeneratedCardToCombat(chosenCard, PileType.Hand, true, CardPilePosition.Random);
            }
        }
    }


}

