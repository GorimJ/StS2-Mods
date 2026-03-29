using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens.PotionLab;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Unlocks;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Potions;
using MultiplayerPotions.Potions;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerPotions.Patches;

[HarmonyPatch(typeof(NPotionLab), nameof(NPotionLab.OnSubmenuOpened))]
public static class CustomPotionLabPatch
{
    private static NPotionLabCategory? _multiplayerCategory;

    [HarmonyPostfix]
    public static void Postfix(NPotionLab __instance)
    {
        // Retrieve the ScreenContents scroll view
        var screenContents = __instance.GetNode<NScrollableContainer>("%ScreenContents");
        if (screenContents == null) return;

        // The base game categories (_common, _uncommon, etc.) are children of a VBox. Use the Special category to find it.
        var specialCategory = __instance.GetNode<NPotionLabCategory>("%Special");
        if (specialCategory == null) return;

        var contentVBox = specialCategory.GetParent();
        if (contentVBox == null) return;

        // If we haven't created the category before (or if it was disposed), clone it
        if (_multiplayerCategory == null || !GodotObject.IsInstanceValid(_multiplayerCategory))
        {
            _multiplayerCategory = (NPotionLabCategory)specialCategory.Duplicate();
            // Don't duplicate the children (the potions that were already loaded)
            foreach (var child in _multiplayerCategory.GetNode("%PotionsContainer").GetChildren())
            {
                child.QueueFree();
            }
            _multiplayerCategory.Name = "Multiplayer";
            contentVBox.AddChild(_multiplayerCategory);
            
            // Move it to the bottom
            contentVBox.MoveChild(_multiplayerCategory, -1);
        }

        // We need to re-run the category load logic for our custom rarity since OnSubmenuOpened already called the others
        UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
        HashSet<PotionModel> allUnlockedPotions = unlockState.Potions.ToHashSet();
        HashSet<PotionModel> seenPotions = SaveManager.Instance.Progress.DiscoveredPotions.Select(ModelDb.GetByIdOrNull<PotionModel>).OfType<PotionModel>().ToHashSet();

        _multiplayerCategory.Modulate = Colors.White;
        _multiplayerCategory.Show();
        _multiplayerCategory.ClearPotions();
        _multiplayerCategory.LoadPotions(SummonPotion.MultiplayerRarity, 
                                          new LocString("potion_lab", "MULTIPLAYER"), 
                                          seenPotions, unlockState, allUnlockedPotions);

        // Re-calculate the gamepad focus neighbors just like the base game does
        // For simplicity, we can let Godot's built-in focus mode handle most of it unless it explicitly breaks. 
        // The base game method handles the neighbor wiring, but since this is a mouse-first mod we'll rely on Godot UI defaults.
    }
}
