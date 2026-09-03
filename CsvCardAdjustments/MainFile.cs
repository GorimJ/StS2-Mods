using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace CsvCardAdjustments;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    internal const string ModId = "CsvCardAdjustments";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        int ok = 0, failed = 0;
        // Patch class-by-class so one card override that no longer matches the game build
        // doesn't take the whole mod down.
        foreach (var type in typeof(MainFile).Assembly.GetTypes())
        {
            if (!type.GetCustomAttributes(typeof(HarmonyPatch), true).Any()) continue;
            try
            {
                harmony.CreateClassProcessor(type).Patch();
                ok++;
            }
            catch (System.Exception ex)
            {
                failed++;
                Logger.Error($"Skipped patch class {type.Name}: {ex.Message}");
            }
        }
        Logger.Info($"Applied {ok} patch classes, skipped {failed}.");
    }
}
