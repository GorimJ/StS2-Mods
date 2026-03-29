using System;
using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System.Linq;

namespace MonsterPredictions;

[HarmonyPatch(typeof(NCreature))]
public static class IntentPredictor
{
    [HarmonyPatch(nameof(NCreature.UpdateIntent))]
    [HarmonyPostfix]
    public static void UpdateIntent_Postfix(NCreature __instance, IEnumerable<Creature> targets)
    {
        try
        {
            if (__instance.Entity?.Monster?.MoveStateMachine == null) return;
            if (__instance.IntentContainer == null) return;

            Node parentContainer = __instance.IntentContainer.GetParent();
            if (parentContainer == null) return;

            // Create a completely isolated Node2D to hold prediction intents safely
            PredictionContainerNode predictionContainer = parentContainer.GetNodeOrNull<PredictionContainerNode>("PredictionContainer");
            if (predictionContainer == null)
            {
                predictionContainer = new PredictionContainerNode();
                predictionContainer.Name = "PredictionContainer";
                predictionContainer.CreatureNode = __instance;
                parentContainer.AddChild(predictionContainer);
            }
            
            // Safely mark the newest native move so _Process doesn't immediately infinite loop!
            predictionContainer.LastRecordedMoveId = __instance.Entity?.Monster?.NextMove?.Id ?? "";

            // Using RemoteTransform2D to natively force predictionContainer to perfectly 
            // track the monster's exact IntentPos origin every frame (including breathing animations).
            // This bypasses entirely all coordinate-space nesting differences!
            Marker2D intentPos = __instance.GetNodeOrNull<Marker2D>("IntentPos");
            if (intentPos != null)
            {
                RemoteTransform2D remote = intentPos.GetNodeOrNull<RemoteTransform2D>("PredictionRemote");
                if (remote == null)
                {
                    remote = new RemoteTransform2D();
                    remote.Name = "PredictionRemote";
                    intentPos.AddChild(remote);
                    remote.RemotePath = remote.GetPathTo(predictionContainer);
                }
            }
            else
            {
                // Absolute fallback just in case
                predictionContainer.GlobalPosition = __instance.IntentContainer.GlobalPosition + new Vector2(__instance.IntentContainer.Size.X / 2f, 0);
            }

            // Clear out old prediction intents safely
            foreach (Node child in predictionContainer.GetChildren())
            {
                predictionContainer.RemoveChild(child);
                child.QueueFree();
            }

            if (!ConfigManager.Instance.EnableMod) return;

            MonsterModel monster = __instance.Entity.Monster;
            var sm = monster.MoveStateMachine;

            // Clone RNG
            uint seed = monster.RunRng?.MonsterAi?.Seed ?? Rng.Chaotic.Seed;
            int counter = monster.RunRng?.MonsterAi?.Counter ?? Rng.Chaotic.Counter;
            Rng clonedRng = new Rng(seed, counter);

            MonsterState currentState = monster.NextMove;
            
            // Cache targets enumeration
            List<Creature> targetList = targets.ToList(); 
            
            int ascension = LocalContext.GetMe(__instance.Entity.CombatState)?.RunState?.AscensionLevel ?? 0;
            int accumulatedStrength = MonsterDictionary.GetStrengthGainFromMove(monster.GetType().Name, monster.NextMove.Id, ascension);

            for (int turnAhead = 1; turnAhead <= ConfigManager.Instance.TurnsToPredict; turnAhead++)
            {
                if (currentState == null || !currentState.CanTransitionAway) break;

                bool foundMove = false;
                try
                {
                    int safetyLimit = 100;
                    while (safetyLimit-- > 0)
                    {
                        string nextId = currentState.GetNextState(__instance.Entity, clonedRng);
                        if (string.IsNullOrEmpty(nextId) || !sm.States.TryGetValue(nextId, out var nextState))
                        {
                            break;
                        }
                        currentState = nextState;
                        if (currentState.IsMove)
                        {
                            foundMove = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"MonsterPredictions error simulating move: {ex.Message}");
                    break;
                }

                if (!foundMove || currentState is not MoveState moveState) break;

                // Dictionary Tracking: If this projected action buffs strength natively, track it!
                int strGain = MonsterDictionary.GetStrengthGainFromMove(monster.GetType().Name, moveState.Id, ascension);
                GD.Print($"MonsterPred Dict | Monster={monster.GetType().Name} | Move={moveState.Id} | Asc={ascension} | Gain={strGain}");
                
                for (int i = 0; i < moveState.Intents.Count; i++)
                {
                    AbstractIntent nextIntent = moveState.Intents[i];
                    NIntent visualIntent = NIntent.Create(turnAhead * 0.5f);
                    visualIntent.SetMeta("PredictingTurnsAhead", turnAhead);
                    visualIntent.SetMeta("SimulatedStrength", accumulatedStrength);

                    // Add to SceneTree FIRST so Godot executes _Ready()
                    predictionContainer.AddChild(visualIntent);

                    try
                    {
                        visualIntent.UpdateIntent(nextIntent, targetList, __instance.Entity);
                    }
                    finally
                    {
                        // The Harmony patch handles resetting these values
                    }
                    
                    if (ConfigManager.Instance.HideDamageNumbersOnPredictions)
                    {
                        var valLabel = visualIntent.GetNodeOrNull<Control>("%Value");
                        if (valLabel != null) valLabel.Visible = false;
                    }
                    
                    // Modulate fades gracefully progressively depending identically on the layer depth organically
                    visualIntent.Modulate = new Color(1, 1, 1, ConfigManager.Instance.IntentOpacity / turnAhead);
                    
                    // The predictionContainer perfectly tracks the CENTER origin of the monster's intents.
                    // Because predictionContainer is centered precisely at IntentPos, we simply negative-offset half the visualIntent's built-in width to perfectly stack it horizontally!
                    float halfWidth = visualIntent.Size.X > 0 ? visualIntent.Size.X / 2f : 50f;
                    visualIntent.Position = new Vector2(-halfWidth, ConfigManager.Instance.IntentYOffset * turnAhead);
                }
                
                // Add the newly acquired strength, so that the NEXT prediction block inherits it natively for its math computations
                accumulatedStrength += strGain;
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"MonsterPredictions FATAL CRASH: {e}");
        }
    }
}

public partial class PredictionContainerNode : Node2D
{
    public NCreature CreatureNode;
    public string LastRecordedMoveId = "";
    
    public override void _Process(double delta)
    {
        if (CreatureNode?.Entity?.Monster?.NextMove == null) return;
        
        string currentMoveId = CreatureNode.Entity.Monster.NextMove.Id;
        if (currentMoveId != LastRecordedMoveId)
        {
            LastRecordedMoveId = currentMoveId;
            IntentPredictor.UpdateIntent_Postfix(CreatureNode, CreatureNode.Entity.CombatState.Players.Select(p => p.Creature).ToList());
        }
    }
}
