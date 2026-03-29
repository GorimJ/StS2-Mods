using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterPredictions;

[HarmonyPatch]
public static class DamageMathPatches
{
    [HarmonyPatch(typeof(AttackIntent), nameof(AttackIntent.GetSingleDamage))]
    [HarmonyPostfix]
    public static void GetSingleDamage_Postfix(AttackIntent __instance, IEnumerable<Creature> targets, Creature owner, ref int __result)
    {
        try
        {
            if (MainFile.PredictingTurnsAhead <= 0) return;
            if (owner == null) return;
            
            // 1 & 2. Expliring Weak/Vulnerable mathematically solved perfectly via Power Prefix Intercepts below to prevent float truncation!
            int finalDamage = __result;

            // 3. Inject explicit Ritual scaling natively cleanly 
            var ritual = owner.GetPower<RitualPower>();
            if (ritual != null)
            {
                int strengthAdded = ritual.Amount * MainFile.PredictingTurnsAhead;
                finalDamage += strengthAdded;
            }

            // Also inject TerritorialPower which scales mathematically identically to Ritual
            var territorial = owner.GetPower<TerritorialPower>();
            if (territorial != null)
            {
                int strengthAdded = territorial.Amount * MainFile.PredictingTurnsAhead;
                finalDamage += strengthAdded;
            }

            // 4. Inject mathematically tracked dictionary native buff scaling cleanly 
            if (MainFile.SimulatedDictionaryStrength > 0)
            {
                finalDamage += MainFile.SimulatedDictionaryStrength;
            }

            __result = finalDamage;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"MonsterPredictions Math Error: {ex}");
        }
    }
}

[HarmonyPatch(typeof(WeakPower), nameof(WeakPower.ModifyDamageMultiplicative))]
public static class WeakPower_Prediction_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(WeakPower __instance, ref decimal __result)
    {
        if (MainFile.PredictingTurnsAhead > 0 && __instance.Amount <= MainFile.PredictingTurnsAhead)
        {
            __result = 1.0m;
            return false; // Skip native calculation cleanly!
        }
        return true;
    }
}

[HarmonyPatch(typeof(VulnerablePower), nameof(VulnerablePower.ModifyDamageMultiplicative))]
public static class VulnerablePower_Prediction_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(VulnerablePower __instance, ref decimal __result)
    {
        if (MainFile.PredictingTurnsAhead > 0 && __instance.Amount <= MainFile.PredictingTurnsAhead)
        {
            __result = 1.0m;
            return false; // Skip native calculation natively!
        }
        return true;
    }
}

[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Nodes.Combat.NIntent), "UpdateVisuals")]
public static class NIntent_UpdateVisuals_Patch
{
    [HarmonyPrefix]
    public static void Prefix(MegaCrit.Sts2.Core.Nodes.Combat.NIntent __instance)
    {
        if (__instance.HasMeta("PredictingTurnsAhead"))
        {
            MainFile.PredictingTurnsAhead = __instance.GetMeta("PredictingTurnsAhead").AsInt32();
            MainFile.SimulatedDictionaryStrength = __instance.GetMeta("SimulatedStrength").AsInt32();
        }
    }

    [HarmonyPostfix]
    public static void Postfix(MegaCrit.Sts2.Core.Nodes.Combat.NIntent __instance)
    {
        if (__instance.HasMeta("PredictingTurnsAhead"))
        {
            MainFile.PredictingTurnsAhead = 0;
            MainFile.SimulatedDictionaryStrength = 0;
        }
    }
}

[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Nodes.Combat.NIntent), "OnCombatStateChanged")]
public static class NIntent_OnCombatStateChanged_Patch
{
    [HarmonyPrefix]
    public static void Prefix(MegaCrit.Sts2.Core.Nodes.Combat.NIntent __instance)
    {
        if (__instance.HasMeta("PredictingTurnsAhead"))
        {
            MainFile.PredictingTurnsAhead = __instance.GetMeta("PredictingTurnsAhead").AsInt32();
            MainFile.SimulatedDictionaryStrength = __instance.GetMeta("SimulatedStrength").AsInt32();
        }
    }

    [HarmonyPostfix]
    public static void Postfix(MegaCrit.Sts2.Core.Nodes.Combat.NIntent __instance)
    {
        if (__instance.HasMeta("PredictingTurnsAhead"))
        {
            MainFile.PredictingTurnsAhead = 0;
            MainFile.SimulatedDictionaryStrength = 0;
        }
    }
}

[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Nodes.Combat.NIntent), nameof(MegaCrit.Sts2.Core.Nodes.Combat.NIntent.UpdateIntent))]
public static class NIntent_UpdateIntent_Patch
{
    [HarmonyPrefix]
    public static void Prefix(MegaCrit.Sts2.Core.Nodes.Combat.NIntent __instance)
    {
        if (__instance.HasMeta("PredictingTurnsAhead"))
        {
            MainFile.PredictingTurnsAhead = __instance.GetMeta("PredictingTurnsAhead").AsInt32();
            MainFile.SimulatedDictionaryStrength = __instance.GetMeta("SimulatedStrength").AsInt32();
        }
    }

    [HarmonyPostfix]
    public static void Postfix(MegaCrit.Sts2.Core.Nodes.Combat.NIntent __instance)
    {
        if (__instance.HasMeta("PredictingTurnsAhead"))
        {
            MainFile.PredictingTurnsAhead = 0;
            MainFile.SimulatedDictionaryStrength = 0;
        }
    }
}
