using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Screens.TreasureRoomRelic;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rewards;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.GameActions;

namespace RelicChoice.Patches;

[HarmonyPatch(typeof(TreasureRoomRelicSynchronizer), nameof(TreasureRoomRelicSynchronizer.BeginRelicPicking))]
public class ExtraRelicSpawnPatch
{
    // v0.107.1: _votes is now List<PlayerVote> (was List<int?>); we no longer touch it.
    // v0.113.0: the synchronizer holds _runState directly (its _playerCollection field is gone).
    private static void Postfix(TreasureRoomRelicSynchronizer __instance, RelicGrabBag ____sharedGrabBag, Rng ____rng, IRunState ____runState, List<RelicModel> ____currentRelics)
    {
        if (____currentRelics == null || ____currentRelics.Count == 0) return;

        // Generate extra relics based on configuration.
        for (int i = 0; i < RelicChoiceConfig.Instance.AdditionalRelics; i++)
        {
            RelicRarity rarity = RelicFactory.RollRarity(____rng);
            RelicModel item = ____sharedGrabBag.PullFromFront(rarity, ____runState) ?? RelicFactory.FallbackRelic;
            ____currentRelics.Add(item);
        }
    }
}

// v0.113.0: the treasure screen no longer has one pool of holders (_multiplayerHolders). It has one ready-made layout per
// relic count (_holderLayouts[n - 1] is shown for n relics) and uses that layout's holders. With our extra relics the count
// can exceed the layouts the scene ships, in which case vanilla shows nothing - so before InitializeRelics runs we build a
// layout for the count we need, by copying the largest vanilla layout and adding holders to it.
[HarmonyPatch(typeof(NTreasureRoomRelicCollection), nameof(NTreasureRoomRelicCollection.InitializeRelics))]
public class ExtraRelicUIPatch_Initialize
{
    // [Claude's choices, not yet approved by Gorim] at most 5 relics per row; rows as even as possible; shorter last row centred.
    private const int MaxPerRow = 5;
    private const float MaxRowWidth = 1500f;

    private static void Prefix(NTreasureRoomRelicCollection __instance, List<Control> ____holderLayouts)
    {
        int count = RunManager.Instance.TreasureRoomRelicSynchronizer.CurrentRelics?.Count ?? 0;
        if (____holderLayouts == null || ____holderLayouts.Count == 0 || count <= ____holderLayouts.Count) return;

        Control template = ____holderLayouts[____holderLayouts.Count - 1];
        Node parent = template.GetParent();
        var templateHolders = template.GetChildren().OfType<NTreasureRoomRelicHolder>().ToList();
        if (parent == null || templateHolders.Count == 0) return;

        // Centre and spacing of the largest vanilla layout.
        var xs = templateHolders.Select(h => h.Position.X).Distinct().OrderBy(x => x).ToList();
        float centerX = templateHolders.Average(h => h.Position.X);
        float centerY = templateHolders.Average(h => h.Position.Y);
        float spacing = xs.Count > 1 ? (xs[xs.Count - 1] - xs[0]) / (xs.Count - 1) : 280f;

        int rows = (count + MaxPerRow - 1) / MaxPerRow;
        int perRow = (count + rows - 1) / rows;
        float colSpacing = System.Math.Min(spacing, MaxRowWidth / perRow);
        float rowSpacing = spacing;

        var layout = (Control)template.Duplicate();
        layout.Name = "RelicChoiceLayout" + count;
        layout.Visible = false;
        parent.AddChild(layout);
        var holders = layout.GetChildren().OfType<NTreasureRoomRelicHolder>().ToList();
        while (holders.Count < count)
        {
            var extra = (NTreasureRoomRelicHolder)holders[0].Duplicate();
            layout.AddChild(extra);
            holders.Add(extra);
        }
        for (int i = 0; i < holders.Count; i++)
        {
            int row = i / perRow;
            int inThisRow = System.Math.Min(perRow, holders.Count - row * perRow);
            int col = i - row * perRow;
            holders[i].Position = new Vector2(
                centerX + (col - (inThisRow - 1) / 2.0f) * colSpacing,
                centerY + (row - (rows - 1) / 2.0f) * rowSpacing);
        }

        // Vanilla picks the layout at index (count - 1); fill any gap below it with empty placeholders.
        while (____holderLayouts.Count < count - 1)
        {
            var placeholder = new Control { Name = "RelicChoicePlaceholder" + (____holderLayouts.Count + 1), Visible = false };
            parent.AddChild(placeholder);
            ____holderLayouts.Add(placeholder);
        }
        ____holderLayouts.Add(layout);
    }
}

[HarmonyPatch]
public static class CombatRewardRelicChoicePatch
{
    public static Dictionary<NRewardsScreen, RelicReward> PendingRelicClaims = new Dictionary<NRewardsScreen, RelicReward>();
    public static Dictionary<Player, bool> ReadyPlayers = new Dictionary<Player, bool>();

    // Sets whose relic reward we stripped, keyed by the set (shared list instance on the backend).
    public static Dictionary<RewardsSet, RelicReward> StrippedSets = new Dictionary<RewardsSet, RelicReward>();

    // v0.107.1: NRewardsScreen.SetRewards no longer exists; rewards live in RewardsSet.Rewards.
    // Strip the relic reward here because WithRewardsFromRoom runs deterministically on every client for every
    // player, so multiplayer reward-index sync stays consistent.
    [HarmonyPatch(typeof(RewardsSet), nameof(RewardsSet.WithRewardsFromRoom))]
    [HarmonyPostfix]
    public static void WithRewardsFromRoom_Postfix(RewardsSet __instance, AbstractRoom room)
    {
        if (!RelicChoiceConfig.Instance.EnableAfterElites) return;
        if (room == null || room.RoomType != RoomType.Elite) return;

        var relicReward = __instance.Rewards.OfType<RelicReward>().FirstOrDefault();
        if (relicReward != null)
        {
            __instance.Rewards.Remove(relicReward);
            StrippedSets[__instance] = relicReward;
        }
    }

    [HarmonyPatch(typeof(NRewardsScreen), nameof(NRewardsScreen._Ready))]
    [HarmonyPostfix]
    public static void Ready_Postfix(NRewardsScreen __instance, RewardsSet ____rewardsSet)
    {
        if (____rewardsSet != null && StrippedSets.TryGetValue(____rewardsSet, out var relicReward))
        {
            PendingRelicClaims[__instance] = relicReward;
            StrippedSets.Remove(____rewardsSet);
        }
    }

    [HarmonyPatch(typeof(NRewardsScreen), "UpdateScreenState")]
    [HarmonyPostfix]
    public static void UpdateScreenState_Postfix(NRewardsScreen __instance, IRunState ____runState)
    {
        if (PendingRelicClaims.ContainsKey(__instance))
        {
            var proceedButton = __instance.GetNodeOrNull<NProceedButton>("ProceedButton");
            if (proceedButton != null)
            {
                var label = proceedButton.GetNodeOrNull<MegaLabel>("%Label");
                var rewardsContainer = __instance.GetNodeOrNull<Control>("%RewardsContainer");
                
                bool hasUnclaimedRelics = false;
                if (rewardsContainer != null)
                {
                    foreach (var child in rewardsContainer.GetChildren())
                    {
                        if (child is NRewardButton nReward && nReward.Reward is RelicReward)
                        {
                            hasUnclaimedRelics = true;
                            break;
                        }
                    }
                }

                var voteContainer = proceedButton.GetNodeOrNull<NMultiplayerVoteContainer>("RelicVoteContainer");
                if (voteContainer == null && RunManager.Instance.NetService.Type.IsMultiplayer())
                {
                    voteContainer = new NMultiplayerVoteContainer();
                    voteContainer.Name = "RelicVoteContainer";
                    proceedButton.AddChild(voteContainer);
                    voteContainer.SetAnchorsPreset(Control.LayoutPreset.CenterTop);
                    voteContainer.Position = new Vector2(0, -45);
                    voteContainer.Initialize((p) => ReadyPlayers.ContainsKey(p), ____runState.Players);
                    voteContainer.RefreshPlayerVotes(false);
                }

                if (hasUnclaimedRelics)
                {
                    proceedButton.Disable();
                    if (label != null) label.Text = "Proceed";
                    proceedButton.SetPulseState(false);
                }
                else
                {
                    if (__instance.IsComplete || true)
                    {
                        proceedButton.Enable();
                        proceedButton.SetPulseState(true);
                    }
                    if (label != null) label.Text = "Claim Relic";
                }
            }
        }
    }

    [HarmonyPatch(typeof(NRewardsScreen), "OnProceedButtonPressed")]
    [HarmonyPrefix]
    public static bool OnProceedButtonPressed_Prefix(NRewardsScreen __instance, NButton _, IRunState ____runState)
    {
        if (PendingRelicClaims.TryGetValue(__instance, out RelicReward relicReward))
        {
            if (RunManager.Instance.NetService.Type.IsMultiplayer())
            {
                // We use VoteForRelicChoiceAction as our networked signal to proceed.
                RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new VoteForRelicChoiceAction(
                    MegaCrit.Sts2.Core.Context.LocalContext.GetMe(____runState)
                ));
            }
            else
            {
                PendingRelicClaims.Remove(__instance);
                TaskHelper.RunSafely(RunManager.Instance.EnterRoomWithoutExitingCurrentRoom(new TreasureRoom(____runState.CurrentActIndex), true));
            }

            return false;
        }

        return true;
    }


    [HarmonyPatch(typeof(NMultiplayerVoteContainer), nameof(NMultiplayerVoteContainer.RefreshPlayerVotes))]
    [HarmonyPostfix]
    public static void RefreshPlayerVotes_Postfix(NMultiplayerVoteContainer __instance)
    {
        // Only layout the specific RelicVoteContainer on the Proceed button
        if (__instance.Name == "RelicVoteContainer")
        {
            var children = __instance.GetChildren();
            int count = children.Count;
            if (count > 0)
            {
                // We space out the icons by 45px horizontally. 
                // X = (i - (count-1)/2.0) * 45
                for (int i = 0; i < count; i++)
                {
                    if (children[i] is TextureRect rect)
                    {
                        float offsetX = (float)(i - (count - 1) / 2.0f) * 45f;
                        rect.Position = new Vector2(offsetX, rect.Position.Y);
                        rect.Scale = new Vector2(1.5f, 1.5f);
                    }
                }
            }
        }
    }
}
