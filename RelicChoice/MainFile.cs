using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace RelicChoice;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    internal const string ModId = "RelicChoice";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
        
        // Load Mod Configuration
        RelicChoiceConfig.Load();

        // Register Rainbow Relics
        if (RelicChoiceConfig.Instance.EnableRainbowRelics)
        {
            new Relics.BronzeTicket();
            new Relics.SilverTicket();
            new Relics.ShinySilverTicket();
            new Relics.GoldenTicket();
            new Relics.ShinyGoldenTicket();
            new Relics.PremiumGoldenTicket();
            Logger.Info("Rainbow Relics registered.");
        }
        else
        {
            Logger.Info("Rainbow Relics registration skipped by user config.");
        }

        Logger.Info("RelicChoice initialized and patches applied!");
    }
}
