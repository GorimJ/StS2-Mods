using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;

namespace GachaShopMod.Patches;

[HarmonyPatch]
public static class RelicCollectionPatches
{
    [HarmonyPatch(typeof(MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection.NRelicCollectionCategory), "LoadRelicNodes")]
    [HarmonyPrefix]
    public static void PrefixLoadRelicNodes(IEnumerable<RelicModel> relics, HashSet<RelicModel> seenRelics, HashSet<RelicModel> unlockedRelics)
    {
        if (relics == null || seenRelics == null || unlockedRelics == null) return;
        
        foreach (var relic in relics)
        {
            if (relic.GetType().Namespace != null && relic.GetType().Namespace.Contains("GachaShopMod"))
            {
                seenRelics.Add(relic);
                unlockedRelics.Add(relic);
            }
        }
    }
}
