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
        try
        {
            Harmony harmony = new(ModId);
            harmony.PatchAll();
        }
        catch (System.Exception ex)
        {
            Logger.Info("FATAL HARMONY ERROR: " + ex.ToString());
            System.IO.File.WriteAllText(@"C:\Users\Gorim\.gemini\antigravity\Slay the Spire 2 Modding\err.txt", ex.ToString() + "\n\n" + ex.InnerException?.ToString());
        }

        try
        {
            var tex = Godot.ResourceLoader.Load<Godot.Texture2D>("res://images/atlases/potion_atlas.png");
            if (tex != null)
            {
                var img = tex.GetImage();
                string path = "potion_atlas_raw.png";
                img.SavePng(path);
                Logger.Info($"Successfully extracted full potion atlas to {path}!");
            }
            else
            {
                Logger.Info($"Failed to find res://images/atlases/potion_atlas.png");
            }
        }
        catch (System.Exception ex)
        {
            Logger.Info($"Failed to extract full potion atlas: {ex}");
        }
    }
}
