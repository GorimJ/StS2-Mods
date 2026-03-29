using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using GachaShopMod.Nodes;

namespace GachaShopMod.Patches;

[HarmonyPatch]
public static class ShopScreenPatches
{
    [HarmonyPatch(typeof(NMerchantRoom), nameof(NMerchantRoom._Ready))]
    [HarmonyPostfix]
    public static void PostfixShopReady(NMerchantRoom __instance)
    {
        try 
        {
            GD.Print("Entering PostfixShopReady...\n");

            Texture2D gachaTx = GD.Load<Texture2D>("res://Assets/WildFrostGacha.png");
            if (gachaTx == null) 
            {
                GD.Print("GachaShopMod: Could not load WildFrostGacha.png. Falling back temporarily.\n");
                gachaTx = GD.Load<Texture2D>("res://images/ui/run_info/gold.png");
            }
            
            TextureRect gachaRect = new TextureRect();
            gachaRect.Texture = gachaTx;
            gachaRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            gachaRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
        gachaRect.CustomMinimumSize = new Vector2(256, 256);

        // Extract local player using Harmony Reflection
        var playersList = (System.Collections.Generic.List<MegaCrit.Sts2.Core.Entities.Players.Player>) AccessTools.Field(typeof(NMerchantRoom), "_players").GetValue(__instance);
        var localPlayer = playersList != null ? MegaCrit.Sts2.Core.Context.LocalContext.GetMe(playersList) : null;

        // Attach Logic
        GachaMachineNode gachaLogic = new GachaMachineNode(__instance);
        if (localPlayer != null)
        {
            gachaLogic.LocalPlayer = localPlayer;
        }
        gachaRect.AddChild(gachaLogic);
        
        // Handle Hover Tooltip & Sizing
        gachaRect.PivotOffset = new Vector2(128, 128);
        gachaRect.MouseEntered += () => 
        {
            gachaRect.Scale = new Vector2(1.1f, 1.1f);
            var titleStr = new MegaCrit.Sts2.Core.Localization.LocString("ui", "GACHA_MACHINE_TITLE");
            var titleTip = new MegaCrit.Sts2.Core.HoverTips.HoverTip(titleStr, "A shiny new gacha machine. Pull the lever [RB] and get a random enchantment ball. Price increases by 25 for this shop.", null);
            MegaCrit.Sts2.Core.Nodes.HoverTips.NHoverTipSet.CreateAndShow(gachaRect, titleTip, MegaCrit.Sts2.Core.HoverTips.HoverTipAlignment.Left);
        };
        gachaRect.MouseExited += () => 
        {
            gachaRect.Scale = new Vector2(1.0f, 1.0f);
            MegaCrit.Sts2.Core.Nodes.HoverTips.NHoverTipSet.Remove(gachaRect);
        };

        // Handle Clicks
        gachaRect.MouseFilter = Control.MouseFilterEnum.Stop;
        gachaRect.GuiInput += (InputEvent @event) => 
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                gachaLogic.OnMachineClicked();
            }
        };

        Control charContainer = __instance.GetNodeOrNull<Control>("%CharacterContainer");
        if (charContainer != null)
        {
            charContainer.AddChild(gachaRect);
        }
        else
        {
            __instance.AddChild(gachaRect);
        }

        // Force Absolute screen position regardless of the chosen parent hierarchy
        gachaRect.GlobalPosition = new Vector2(1100, 300);

        GD.Print("Gacha Machine successfully injected into NMerchantRoom.\n");
        }
        catch (System.Exception ex)
        {
            GD.Print("CRASH in ShopScreenPatches: " + ex.ToString() + "\n");
        }
    }
}
