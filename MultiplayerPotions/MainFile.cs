using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MultiplayerPotions.Potions;

namespace MultiplayerPotions;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    internal const string ModId = "TheWatcher";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();

        try
        {
            var tex = Godot.ResourceLoader.Load<Godot.Texture2D>("res://images/atlases/potion_atlas.png");
            if (tex != null)
            {
                var img = tex.GetImage();
                string path = "C:/Users/Gorim/Downloads/potion_atlas_raw.png";
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
