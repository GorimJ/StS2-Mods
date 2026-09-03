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
    private static void Postfix(TreasureRoomRelicSynchronizer __instance, RelicGrabBag ____sharedGrabBag, Rng ____rng, IPlayerCollection ____playerCollection, List<RelicModel> ____currentRelics)
    {
        if (____currentRelics == null || ____currentRelics.Count == 0) return;

        var firstPlayer = ____playerCollection.Players[0];
        IRunState runState = firstPlayer.RunState;

        // Generate extra relics based on configuration.
        for (int i = 0; i < RelicChoiceConfig.Instance.AdditionalRelics; i++)
        {
            RelicRarity rarity = RelicFactory.RollRarity(____rng);
            RelicModel item = ____sharedGrabBag.PullFromFront(rarity, runState) ?? RelicFactory.FallbackRelic;
            ____currentRelics.Add(item);
        }
    }
}

[HarmonyPatch(typeof(NTreasureRoomRelicCollection), nameof(NTreasureRoomRelicCollection._Ready))]
public class ExtraRelicUIPatch_Ready
{
    // Layout of the vanilla multiplayer holders, captured before we add extras.
    public static float CenterX = 0f;
    public static float Spacing = 280f;

    private static void Postfix(NTreasureRoomRelicCollection __instance, ref List<NTreasureRoomRelicHolder> ____multiplayerHolders)
    {
        if (____multiplayerHolders == null || ____multiplayerHolders.Count == 0) return;

        var xs = ____multiplayerHolders.Select(h => h.Position.X).OrderBy(x => x).ToList();
        CenterX = xs.Average();
        if (xs.Count > 1) Spacing = (xs[xs.Count - 1] - xs[0]) / (xs.Count - 1);

        // Add additional holders for extra relics if needed.
        int holdersToCreate = System.Math.Max(4, RelicChoiceConfig.Instance.AdditionalRelics);
        var template = ____multiplayerHolders[0];
        for (int i = 0; i < holdersToCreate; i++)
        {
            var extraHolder = (NTreasureRoomRelicHolder)template.Duplicate();
            template.GetParent().AddChild(extraHolder);
            ____multiplayerHolders.Add(extraHolder);
        }
    }
}

[HarmonyPatch(typeof(NTreasureRoomRelicCollection), nameof(NTreasureRoomRelicCollection.InitializeRelics))]
public class ExtraRelicUIPatch_Initialize
{
    private static void Postfix(NTreasureRoomRelicCollection __instance, List<NTreasureRoomRelicHolder> ____holdersInUse)
    {
        if (____holdersInUse == null || ____holdersInUse.Count == 0) return;

        var visibleHolders = ____holdersInUse.Where(h => h.Visible).ToList();
        if (visibleHolders.Count == 0) return;

        // Re-centre the visible holders on the vanilla layout's centre, shrinking the gap when there are many.
        float spacing = System.Math.Min(ExtraRelicUIPatch_Ready.Spacing, 1500f / System.Math.Max(1, visibleHolders.Count));
        for (int i = 0; i < visibleHolders.Count; i++)
        {
            var holder = visibleHolders[i];
            Vector2 pos = holder.Position;
            pos.X = ExtraRelicUIPatch_Ready.CenterX + (i - (visibleHolders.Count - 1) / 2.0f) * spacing;
            holder.Position = pos;
        }
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
