using System;
using System.IO;
using System.Text.Json;
using Godot;
using MegaCrit.Sts2.Core.Modding;

namespace RelicChoice;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class RelicChoiceConfig
{
    private static readonly string ConfigPath = ProjectSettings.GlobalizePath("user://modded/RelicChoiceConfig.json");
    
    public static RelicChoiceConfig Instance { get; private set; } = new RelicChoiceConfig();

    // Configuration Properties
    public int AdditionalRelics { get; set; } = 1;
    public bool EnableAfterElites { get; set; } = true;
    public bool EnableRainbowRelics { get; set; } = true;

    public static void Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                string json = File.ReadAllText(ConfigPath);
                Instance = JsonSerializer.Deserialize<RelicChoiceConfig>(json) ?? new RelicChoiceConfig();
                MainFile.Logger.Info("Loaded RelicChoice configuration.");
            }
            else
            {
                MainFile.Logger.Info("No configuration found, generating default RelicChoiceConfig.json.");
                Save();
            }
        }
        catch (Exception ex)
        {
            MainFile.Logger.Error($"Failed to load RelicChoice configuration: {ex.Message}. Falling back to defaults.");
            Instance = new RelicChoiceConfig();
        }
    }

    public static void Save()
    {
        try
        {
            string dir = Path.GetDirectoryName(ConfigPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Instance, options);
            File.WriteAllText(ConfigPath, json);
        }
        catch (Exception ex)
        {
            MainFile.Logger.Error($"Failed to save RelicChoice configuration: {ex.Message}");
        }
    }
}
