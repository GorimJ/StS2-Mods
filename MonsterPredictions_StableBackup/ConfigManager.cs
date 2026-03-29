using System.IO;
using System.Text.Json;
using Godot;

namespace MonsterPredictions
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int TurnsToPredict { get; set; } = 2;
        public float IntentOpacity { get; set; } = 0.4f;
        public float IntentYOffset { get; set; } = -70f;
        public bool HideDamageNumbersOnPredictions { get; set; } = false;
    }

    public static class ConfigManager
    {
        public static ModConfig Instance { get; private set; } = new ModConfig();

        public static string ConfigPath 
        {
            get 
            {
                string exeDir = System.IO.Path.GetDirectoryName(Godot.OS.GetExecutablePath());
                string modsDir = System.IO.Path.Combine(exeDir, "mods");
                return System.IO.Path.Combine(modsDir, "MonsterPredictions_Config.json");
            }
        }

        public static void Load()
        {
            if (File.Exists(ConfigPath))
            {
                try
                {
                    string json = File.ReadAllText(ConfigPath);
                    Instance = JsonSerializer.Deserialize<ModConfig>(json) ?? new ModConfig();
                    Save(); // Resave cleanly to ensure new configuration fields are physically flushed to disk
                    GD.Print("MonsterPredictions: Successfully loaded user configurations.");
                }
                catch (System.Exception e)
                {
                    GD.PrintErr($"MonsterPredictions: Failed to cleanly load config. Using defaults. Exception: {e.Message}");
                    Instance = new ModConfig();
                }
            }
            else
            {
                GD.Print("MonsterPredictions: No native config file found. generating generic template.");
                Save();
            }
        }

        public static void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Instance, options);
                File.WriteAllText(ConfigPath, json);
                GD.Print($"MonsterPredictions: Saved new JSON config payload natively to {ConfigPath}");
            }
            catch (System.Exception e)
            {
                GD.PrintErr($"MonsterPredictions: Failed to locally save config. Exception: {e.Message}");
            }
        }
    }
}
