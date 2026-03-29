using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using System;

namespace MonsterPredictions;

[ModInitializer("Initialize")]
public partial class MainFile : Node
{
    public static int PredictingTurnsAhead = 0;
    public static int SimulatedDictionaryStrength = 0;
    
    public static string ModId => "MonsterPredictions";

    public static void Initialize()
    {
        ConfigManager.Load();
        var harmony = new Harmony(ModId);
        
        // Patch standard STS2 hooks via attribute
        // This will pick up DamageMathPatches and IntentPredictor automatically!
        harmony.PatchAll(typeof(MainFile).Assembly);
    }
}
