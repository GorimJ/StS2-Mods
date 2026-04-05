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

        Logger.Info(RelicChoiceConfig.Instance.EnableRainbowRelics
            ? "Rainbow Relics enabled."
            : "Rainbow Relics disabled by config.");

        Logger.Info("RelicChoice initialized and patches applied!");
    }
}
