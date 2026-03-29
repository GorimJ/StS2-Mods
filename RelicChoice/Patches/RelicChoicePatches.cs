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
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ExtraRelicSpawnPatch
{
    private static void Postfix(TreasureRoomRelicSynchronizer __instance, RelicGrabBag ____sharedGrabBag, Rng ____rng, IPlayerCollection ____playerCollection, ref List<RelicModel> ____currentRelics, ref List<int?> ____votes)
    {
        if (CombatRewardRelicChoicePatch.IsPickingCombatRelic)
        {
            ____currentRelics = new List<RelicModel>();
            ____votes.Clear();
            foreach (Player player in ____playerCollection.Players)
            {
                ____votes.Add(null);
                IRunState state = player.RunState;
                RelicRarity r = RelicFactory.RollRarity(____rng);
                RelicModel i = ____sharedGrabBag.PullFromFront(r, state);
                ____currentRelics.Add(i);
            }

            if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer && ____playerCollection.Players.Count > 1)
            {
                for (int i = 1; i < ____playerCollection.Players.Count; i++)
                {
                    ____votes[i] = ____rng.NextInt(____currentRelics.Count + 1);
                }
            }
        }

        if (____currentRelics != null && ____currentRelics.Count > 0)
        {
            var firstPlayer = ____playerCollection.Players[0];
            IRunState runState = firstPlayer.RunState;
            
            // Generate extra relics based on configuration.
            for (int i = 0; i < RelicChoiceConfig.Instance.AdditionalRelics; i++)
            {
                RelicRarity rarity = RelicFactory.RollRarity(____rng);
                RelicModel item = ____sharedGrabBag.PullFromFront(rarity, runState);
                ____currentRelics.Add(item);
            }
        }
    }
}

[HarmonyPatch(typeof(NTreasureRoomRelicCollection), nameof(NTreasureRoomRelicCollection._Ready))]
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ExtraRelicUIPatch_Ready
{
    private static void Postfix(NTreasureRoomRelicCollection __instance, ref List<NTreasureRoomRelicHolder> ____multiplayerHolders)
    {
        // Add additional holders for extra relics if needed.
        if (____multiplayerHolders != null && ____multiplayerHolders.Count > 0)
        {
            int holdersToCreate = System.Math.Max(4, RelicChoiceConfig.Instance.AdditionalRelics);
            var template = ____multiplayerHolders[0];
            for (int i = 0; i < holdersToCreate; i++) {
                var extraHolder = (NTreasureRoomRelicHolder)template.Duplicate();
                template.GetParent().AddChild(extraHolder);
                ____multiplayerHolders.Add(extraHolder);
            }
        }
    }
}

[HarmonyPatch(typeof(NTreasureRoomRelicCollection), nameof(NTreasureRoomRelicCollection.InitializeRelics))]
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ExtraRelicUIPatch_Initialize
{
    private static void Postfix(NTreasureRoomRelicCollection __instance, List<NTreasureRoomRelicHolder> ____holdersInUse)
    {
        if (____holdersInUse != null && ____holdersInUse.Count > 0)
        {
            var visibleHolders = new List<NTreasureRoomRelicHolder>();
            foreach (var holder in ____holdersInUse)
            {
                if (holder.Visible) visibleHolders.Add(holder);
            }

            if (visibleHolders.Count > 0)
            {
                float centerX = __instance.SingleplayerRelicHolder.Position.X;
                float spacing = 280f;

                for (int i = 0; i < visibleHolders.Count; i++)
                {
                    var holder = visibleHolders[i];
                    Vector2 pos = holder.Position;
                    float offsetX = (i - (visibleHolders.Count - 1) / 2.0f) * spacing;
                    pos.X = centerX + offsetX;
                    holder.Position = pos;
                }
            }
        }
    }
}

[HarmonyPatch]
public static class CombatRewardRelicChoicePatch
{
    public static bool IsPickingCombatRelic = false;
    public static Dictionary<NRewardsScreen, RelicReward> PendingRelicClaims = new Dictionary<NRewardsScreen, RelicReward>();
    public static Dictionary<Player, bool> ReadyPlayers = new Dictionary<Player, bool>();

    [HarmonyPatch(typeof(NRewardsScreen), nameof(NRewardsScreen.SetRewards))]
    [HarmonyPrefix]
    public static void SetRewards_Prefix(NRewardsScreen __instance, ref IEnumerable<Reward> rewards, IRunState ____runState)
    {
        if (!RelicChoiceConfig.Instance.EnableAfterElites) return;

        if (____runState.CurrentRoom == null || ____runState.CurrentRoom.RoomType != RoomType.Elite) return;

        var list = rewards.ToList();
        var relicReward = list.OfType<RelicReward>().FirstOrDefault();

        if (relicReward != null)
        {
            list.Remove(relicReward);
            rewards = list;

            PendingRelicClaims[__instance] = relicReward;
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
