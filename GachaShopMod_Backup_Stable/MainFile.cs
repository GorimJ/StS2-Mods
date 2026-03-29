using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace GachaShopMod;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    internal const string ModId = "GachaShopMod";

    public static void Initialize()
    {
        try 
        {
            GD.Print("\n\n--- NEW LAUNCH ---\nInitializing GachaShopMod...\n");
            // 1. Initialize Harmony for patches
            Harmony harmony = new(ModId);
            harmony.PatchAll();
            GD.Print($"{ModId} - Harmony Patches Applied.");
            GD.Print("Harmony PatchAll() Success!\n");
        }
        catch (System.Exception ex)
        {
            GD.Print("FATAL HARMONY CRASH IN BOOTSTRAP: " + ex.ToString() + "\n");
        }

        try 
        {
            // 2. Register Custom Elements into BaseLib
        new Relics.GachaRelicPool();
        new Relics.AdroitGachaBallRelic();
        new Relics.CloneGachaBallRelic();
        new Relics.CorruptedGachaBallRelic();
        new Relics.FavoredGachaBallRelic();
        new Relics.GlamGachaBallRelic();
        new Relics.GoopyGachaBallRelic();
        new Relics.ImbuedGachaBallRelic();
        new Relics.InstinctGachaBallRelic();
        new Relics.MomentumGachaBallRelic();
        new Relics.NimbleGachaBallRelic();
        new Relics.PerfectFitGachaBallRelic();
        new Relics.RoyallyApprovedGachaBallRelic();
        new Relics.SharpGachaBallRelic();
        new Relics.SlitherGachaBallRelic();
        new Relics.SlumberingEssenceGachaBallRelic();
        new Relics.SoulsPowerGachaBallRelic();
        new Relics.SownGachaBallRelic();
        new Relics.SpiralGachaBallRelic();
        new Relics.SteadyGachaBallRelic();
        new Relics.SwiftGachaBallRelic();
        new Relics.TezcatarasEmberGachaBallRelic();
        new Relics.VigorousGachaBallRelic();
        
        GD.Print($"{ModId} - Successful Initialization.");
        }
        catch (System.Exception ex)
        {
            GD.Print("FATAL RELIC INJECTION CRASH: " + ex.ToString() + "\n");
        }
    }
}
