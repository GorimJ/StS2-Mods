using Godot;
using HarmonyLib;
using System.IO;
using System.Text.Json;
using MegaCrit.Sts2.Core.Localization;
using System.Collections.Generic;

namespace CsvCardAdjustments;

public static class LocDumper
{
    public static void DumpCards()
    {
        try
        {
            var table = LocManager.Instance.GetTable("cards");
            if (table != null)
            {
                var dictField = typeof(LocTable).GetField("_translations", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (dictField != null)
                {
                    var dict = dictField.GetValue(table) as Dictionary<string, string>;
                    if (dict != null)
                    {
                        var json = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText("sts2_vanilla_cards.json", json);
                        MainFile.Logger.Info("[CsvCardAdjustments] Successfully dumped vanilla cards.json");
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            MainFile.Logger.Error($"[CsvCardAdjustments] Failed to dump loc: {ex.Message}");
        }
    }
}
