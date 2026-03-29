using Godot;
using System;
using System.Linq;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Runs;

namespace GachaShopMod.Nodes;

public partial class GachaMachineNode : Control
{
    public MegaCrit.Sts2.Core.Entities.Players.Player LocalPlayer { get; set; }
    private NMerchantRoom _shopRoom;
    private int currentCost = 75;
    private int costIncrement = 25;
    private MegaCrit.Sts2.addons.mega_text.MegaLabel costLabel;
    
    public GachaMachineNode(NMerchantRoom room)
    {
        _shopRoom = room;
    }
    
    public GachaMachineNode() {}

    public override void _Input(InputEvent @event)
    {
        if (IsVisibleInTree())
        {
            bool isRb = false;
            
            if (@event.IsActionPressed("mega_view_exhaust_pile_and_tab_right")) 
            {
                isRb = true;
            }
            else if (@event is InputEventJoypadButton joy && joy.ButtonIndex == JoyButton.RightShoulder && joy.Pressed && !joy.IsEcho())
            {
                isRb = true;
            }

            if (isRb)
            {
                OnMachineClicked();
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public override void _Ready()
    {
        try {
            GD.Print("Entering GachaMachineNode._Ready...\n");

            SetAnchorsPreset(LayoutPreset.FullRect);

            // Harvest true styling from the actual Godot Shop structures
            HBoxContainer costContainer = null;
            if (_shopRoom != null && _shopRoom.Inventory != null)
            {
                Control sampleSlot = (Control)FindFirstMerchantRelic(_shopRoom.Inventory) ?? (Control)FindFirstMerchantCard(_shopRoom.Inventory);
                if (sampleSlot != null)
                {
                    var originalLabel = sampleSlot.GetNodeOrNull<MegaCrit.Sts2.addons.mega_text.MegaLabel>("%CostLabel");
                    if (originalLabel != null)
                    {
                        var parentContainer = originalLabel.GetParent() as HBoxContainer;
                        if (parentContainer != null)
                        {
                            costContainer = (HBoxContainer)parentContainer.Duplicate();
                        }
                    }
                }
            }

            if (costContainer != null)
            {
                costContainer.Name = "GachaCostContainer";
                costContainer.UniqueNameInOwner = false;
                costContainer.CustomMinimumSize = new Vector2(256, 40);
                costContainer.Alignment = BoxContainer.AlignmentMode.Center;
                costContainer.Position = new Vector2(-48, 260); // Math: Shift 48 pixels to the left
                AddChild(costContainer);

                costLabel = costContainer.GetNodeOrNull<MegaCrit.Sts2.addons.mega_text.MegaLabel>("CostLabel") ?? costContainer.GetNodeOrNull<MegaCrit.Sts2.addons.mega_text.MegaLabel>("%CostLabel"); 
                if (costLabel != null)
                {
                    costLabel.Name = "GachaCostLabel";
                    costLabel.UniqueNameInOwner = false;
                    costLabel.Text = $"{currentCost}";
                    costLabel.SetTextAutoSize($"{currentCost}");
                    costLabel.Modulate = StsColors.cream;
                    
                    int currentFs = costLabel.GetThemeFontSize("font_size");
                    if (currentFs <= 0) currentFs = 48; // Fallback
                    costLabel.AddThemeFontSizeOverride("font_size", (int)(currentFs * 0.75f));
                }
            }
            else
            {
                // Fallback simply for error logging if the StS2 UI radically changes
                 GD.Print("WARNING: Failed to Clone Cost Container UI\n");
            }
            
            if (LocalPlayer != null)
            {
                LocalPlayer.GoldChanged += UpdateCostLabel;
            }
            
            UpdateCostLabel();
            GD.Print("GachaMachineNode._Ready success.\n");
        } 
        catch (System.Exception ex) 
        {
            GD.Print("CRASH in GachaMachineNode._Ready: " + ex.ToString() + "\n");
        }
    }

    public override void _ExitTree()
    {
        if (LocalPlayer != null)
        {
            LocalPlayer.GoldChanged -= UpdateCostLabel;
        }
        base._ExitTree();
    }

    private MegaCrit.Sts2.Core.Nodes.Screens.Shops.NMerchantRelic FindFirstMerchantRelic(Node root)
    {
        if (root == null) return null;
        if (root is MegaCrit.Sts2.Core.Nodes.Screens.Shops.NMerchantRelic r) return r;
        foreach(Node c in root.GetChildren())
        {
            var res = FindFirstMerchantRelic(c);
            if (res != null) return res;
        }
        return null;
    }

    private MegaCrit.Sts2.Core.Nodes.Screens.Shops.NMerchantCard FindFirstMerchantCard(Node root)
    {
        if (root == null) return null;
        if (root is MegaCrit.Sts2.Core.Nodes.Screens.Shops.NMerchantCard c) return c;
        foreach(Node child in root.GetChildren())
        {
            var res = FindFirstMerchantCard(child);
            if (res != null) return res;
        }
        return null;
    }

    public void ResetCost()
    {
        currentCost = 75;
        UpdateCostLabel();
    }

    private void UpdateCostLabel()
    {
        if (costLabel != null)
        {
            costLabel.Text = $"{currentCost}";
            costLabel.SetTextAutoSize($"{currentCost}");
            if (LocalPlayer != null)
            {
                if (LocalPlayer.Gold >= currentCost)
                {
                    costLabel.Modulate = StsColors.cream;
                }
                else
                {
                    bool hasBloodGacha = LocalPlayer.Relics.Any(r => r is GachaShopMod.Relics.BloodGachaRelic);
                    if (hasBloodGacha)
                    {
                        int hpCost = Mathf.CeilToInt(currentCost / 10f);
                        if (LocalPlayer.Creature.CurrentHp > hpCost)
                        {
                            costLabel.Modulate = StsColors.purple;
                            costLabel.Text = $"{hpCost} HP";
                            costLabel.SetTextAutoSize($"{hpCost} HP");
                            return;
                        }
                    }
                    costLabel.Modulate = StsColors.red;
                }
            }
        }
    }

    public async void OnMachineClicked()
    {
        if (LocalPlayer == null) return;
        
        if (LocalPlayer.Gold >= currentCost)
        {
            PlayGachaSound();
            await PlayerCmd.LoseGold(currentCost, LocalPlayer, MegaCrit.Sts2.Core.Entities.Gold.GoldLossType.Spent);
            
            if (RunManager.Instance != null && RunManager.Instance.RewardSynchronizer != null)
            {
                RunManager.Instance.RewardSynchronizer.SyncLocalGoldLost(currentCost);
            }
            
            currentCost += 25;
            UpdateCostLabel();
            GrantGachaBall(LocalPlayer);
        }
        else
        {
            bool hasBloodGacha = LocalPlayer.Relics.Any(r => r is GachaShopMod.Relics.BloodGachaRelic);
            if (hasBloodGacha)
            {
                int hpCost = Mathf.CeilToInt(currentCost / 10f);
                if (LocalPlayer.Creature.CurrentHp > hpCost)
                {
                    PlayGachaSound();
                    LocalPlayer.Creature.LoseHpInternal(hpCost, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered);
                    
                    currentCost += 25;
                    UpdateCostLabel();
                    GrantGachaBall(LocalPlayer);
                    return;
                }
            }
            GD.Print("GachaShopMod: Not enough gold or HP!");
        }
    }
    
    private void PlayGachaSound()
    {
        try 
        {
            AudioStream gachaSound = GD.Load<AudioStream>("res://Assets/GachaNoise.wav");
            if (gachaSound != null)
            {
                AudioStreamPlayer sfxPlayer = new AudioStreamPlayer();
                sfxPlayer.Stream = gachaSound;
                sfxPlayer.VolumeDb = 0.0f;
                AddChild(sfxPlayer);
                sfxPlayer.Play();
                
                // Cleanup memory after playback
                sfxPlayer.Finished += () => sfxPlayer.QueueFree();
            }
        } 
        catch (System.Exception ex) 
        {
            GD.Print("Audio Engine Error: " + ex.ToString() + "\n");
        }
    }

    private async void GrantGachaBall(MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        string[] allGachaRelics;
        if (player.Relics.Any(r => r is GachaShopMod.Relics.LuckyCatRelic))
        {
            allGachaRelics = GachaShopMod.Relics.GachaLootTable.LuckyPool;
        }
        else
        {
            allGachaRelics = GachaShopMod.Relics.GachaLootTable.Pool;
        }
        
        // Use System.Random instead of RunState.Rng to avoid Multiplayer Deterministic Desyncs
        var random = new System.Random();
        var randomId = allGachaRelics[random.Next(allGachaRelics.Length)];
        
        Type relicType = Type.GetType($"GachaShopMod.Relics.{randomId}");
        if (relicType != null)
        {
            var method = typeof(ModelDb).GetMethod("Relic", Type.EmptyTypes);
            if (method != null)
            {
                var generic = method.MakeGenericMethod(relicType);
                var relicModel = generic.Invoke(null, null) as RelicModel;
                
                if (relicModel != null)
                {
                    var mutableRelic = (RelicModel)relicModel.ToMutable();
                    await MegaCrit.Sts2.Core.Commands.RelicCmd.Obtain(mutableRelic, player);
                    
                    if (RunManager.Instance != null && RunManager.Instance.RewardSynchronizer != null)
                    {
                        RunManager.Instance.RewardSynchronizer.SyncLocalObtainedRelic(mutableRelic);
                    }
                }
            }
        }
    }
}
