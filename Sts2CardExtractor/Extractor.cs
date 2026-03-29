using Godot;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

public partial class Extractor : Node
{
    public override void _Ready()
    {
        try {
            GD.Print("Loading PCK...");
            bool pckLoaded = ProjectSettings.LoadResourcePack("A:/SteamLibrary/steamapps/common/Slay the Spire 2/SlayTheSpire2.pck");
            if (!pckLoaded) {
                GD.PrintErr("Failed to load sts2.pck");
                GetTree().Quit();
                return;
            }
            GD.Print("Loaded PCK. Reading raw JSONs...");
            var file = Godot.FileAccess.Open("res://localization/eng/cards.json", Godot.FileAccess.ModeFlags.Read);
            if (file == null) {
                GD.Print("Failed to open cards.json");
                GetTree().Quit();
                return;
            }
            string jsonContent = file.GetAsText();
            file.Close();
            
            var cardDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
            
            GD.Print("Extracting cards...");
            var cardsAssembly = typeof(CardModel).Assembly;
            var cardTypes = cardsAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CardModel)));
            
            using var writer = new StreamWriter("C:/Users/Gorim/.gemini/antigravity/Slay the Spire 2 Modding/Sts2CardExtractor/card_comparison.csv");
            writer.WriteLine("Card Name,Base Energy Cost,Upgraded Energy Cost,Base Star Cost,Upgraded Star Cost,Base Variables,Upgraded Variables,Base Text,Upgraded Text");
            
            foreach (var type in cardTypes) {
                try {
                    var card = (CardModel)Activator.CreateInstance(type);
                    if (card == null) continue;
                    
                    string entry = card.Id.Entry; // typically simple class name like 'StrikeIronclad'
                    string baseName = cardDict.TryGetValue(entry + ".title", out var title) ? title : type.Name;
                    
                    string cost = "?";
                    try { 
                        cost = card.EnergyCost.CostsX ? "X" : card.EnergyCost.Canonical.ToString();
                    } catch {}
                    
                    string starCost = "";
                    try {
                        starCost = card.BaseStarCost >= 0 ? card.BaseStarCost.ToString() : "";
                    } catch {}
                    
                    string baseText = "";
                    cardDict.TryGetValue(entry + ".description", out baseText);
                    
                    var dynamicVarsProp = typeof(CardModel).GetProperty("DynamicVars", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    var baseVarsList = new List<string>();
                    var baseVarsMap = new Dictionary<string, decimal>();
                    try {
                        var dynSet = dynamicVarsProp?.GetValue(card) as IEnumerable<KeyValuePair<string, DynamicVar>>;
                        if (dynSet != null) {
                            foreach (var kvp in dynSet) {
                                baseVarsList.Add($"{kvp.Key}: {kvp.Value.BaseValue}");
                                baseVarsMap[kvp.Key] = kvp.Value.BaseValue;
                            }
                        }
                    } catch {}
                    string baseVariablesStr = string.Join("\n", baseVarsList);
                    
                    // Upgrade the card to get upgraded props, usually base cost doesn't change but text does
                    string costUpgraded = cost;
                    string starCostUpgraded = starCost;
                    try {
                        var isMutableProp = typeof(AbstractModel).GetProperty("IsMutable");
                        isMutableProp?.SetValue(card, true);
                        
                        var method = typeof(CardModel).GetMethod("UpgradeInternal", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        method?.Invoke(card, null);
                        costUpgraded = card.EnergyCost.CostsX ? "X" : card.EnergyCost.Canonical.ToString();
                        starCostUpgraded = card.BaseStarCost >= 0 ? card.BaseStarCost.ToString() : "";
                    } catch (Exception ex) {
                        GD.Print($"Error during upgrade of {baseName}: {ex.InnerException?.Message ?? ex.Message}");
                    }
                    
                    var upgradedVarsList = new List<string>();
                    try {
                        var dynSet = dynamicVarsProp?.GetValue(card) as IEnumerable<KeyValuePair<string, DynamicVar>>;
                        if (dynSet != null) {
                            foreach (var kvp in dynSet) {
                                decimal baseVal = baseVarsMap.ContainsKey(kvp.Key) ? baseVarsMap[kvp.Key] : -9999;
                                decimal upVal = kvp.Value.BaseValue;
                                if (baseVal == upVal) {
                                    upgradedVarsList.Add($"{kvp.Key}: {upVal}");
                                } else {
                                    upgradedVarsList.Add($"{kvp.Key}: {upVal} (was {baseVal})");
                                }
                            }
                        }
                    } catch {}
                    
                    string upgradedVariablesStr = string.Join("\n", upgradedVarsList);
                    
                    string upgradedText = "";
                    if (!cardDict.TryGetValue(entry + "+.description", out upgradedText)) {
                        cardDict.TryGetValue(entry + "_plus.description", out upgradedText);
                    }
                    if (string.IsNullOrEmpty(upgradedText)) {
                        upgradedText = baseText; // Often card text doesn't change on upgrade!
                    }
                    
                    writer.WriteLine($"{EscapeCsv(baseName)},{EscapeCsv(cost)},{EscapeCsv(costUpgraded)},{EscapeCsv(starCost)},{EscapeCsv(starCostUpgraded)},{EscapeCsv(baseVariablesStr)},{EscapeCsv(upgradedVariablesStr)},{EscapeCsv(baseText)},{EscapeCsv(upgradedText)}");
                } catch (Exception ex) {
                    GD.Print($"Failed to process {type.Name}: {ex.Message}");
                }
            }
            GD.Print("Extraction Complete! card_comparison.csv created.");
        } catch(Exception e) {
            GD.Print(e.ToString());
        }
        GetTree().Quit();
    }
    
    private string EscapeCsv(string input) {
        if (string.IsNullOrEmpty(input)) return "";
        input = input.Replace("\r", "");
        if (input.Contains(",") || input.Contains("\"") || input.Contains("\n")) {
            return $"\"{input.Replace("\"", "\"\"")}\"";
        }
        return input;
    }
}
