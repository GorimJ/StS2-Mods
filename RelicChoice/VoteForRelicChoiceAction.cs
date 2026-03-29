using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using System.Linq;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Rooms;
using Godot;

namespace RelicChoice.Patches;

public struct NetVoteForRelicChoiceAction : INetAction, IPacketSerializable
{
    public void Serialize(PacketWriter writer)
    {
    }

    public void Deserialize(PacketReader reader)
    {
    }

    public GameAction ToGameAction(Player player)
    {
        return new VoteForRelicChoiceAction(player);
    }
}

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class VoteForRelicChoiceAction : GameAction
{
    private Player _player;

    public override ulong OwnerId => _player.NetId;

    public override GameActionType ActionType => GameActionType.NonCombat;

    public VoteForRelicChoiceAction(Player player)
    {
        _player = player;
    }

    protected override Task ExecuteAction()
    {
        // Networked actions run on all clients regardless of local UI state.
        // We cannot rely on NOverlayStack.Instance.Peek() because a player might have a different
        // window open (like Deck or Settings) when another player clicks Claim Relic.
        
        NRewardsScreen screen = null;
        if (NOverlayStack.Instance != null)
        {
            foreach (var overlay in NOverlayStack.Instance.GetChildren())
            {
                if (overlay is NRewardsScreen rs && CombatRewardRelicChoicePatch.PendingRelicClaims.ContainsKey(rs))
                {
                    screen = rs;
                    break;
                }
            }
        }

        // Even if the screen isn't found locally (e.g. they haven't opened it yet for some reason),
        // we still record the ready vote to avoid desyncs. The state update must be unconditional.
        CombatRewardRelicChoicePatch.ReadyPlayers[_player] = true;

        if (screen != null)
        {
            var btn = screen.GetNodeOrNull<NProceedButton>("ProceedButton");
            var vc = btn?.GetNodeOrNull<NMultiplayerVoteContainer>("RelicVoteContainer");
            vc?.RefreshPlayerVotes(true);
        }

        // Fetch live online players
        // We use RunManager.Instance.RunLobby.ConnectedPlayerIds.Count to ensure we don't wait for dropped dead players.
        int requiredPlayers = _player.RunState.Players.Count;
        if (RunManager.Instance.RunLobby != null)
        {
            requiredPlayers = RunManager.Instance.RunLobby.ConnectedPlayerIds.Count;
        }

        if (CombatRewardRelicChoicePatch.ReadyPlayers.Count >= requiredPlayers)
        {
            CombatRewardRelicChoicePatch.ReadyPlayers.Clear();
            if (screen != null)
            {
                CombatRewardRelicChoicePatch.PendingRelicClaims.Remove(screen);
            }
            TaskHelper.RunSafely(RunManager.Instance.EnterRoomWithoutExitingCurrentRoom(new TreasureRoom(_player.RunState.CurrentActIndex), true));
        }
        
        return Task.CompletedTask;
    }

    public override INetAction ToNetAction()
    {
        return default(NetVoteForRelicChoiceAction);
    }
}
