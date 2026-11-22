using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Runs;
using Seductress.SeductressCode.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Integration;

/// <summary>
/// [Claude, 2026-11-14] Publishes this mod's own state to Spire Link, so a test can read the
/// reservoirs, arousal, Sigils and routing over its local API instead of from a screenshot.
///
/// Spire Link (the locally patched build) looks for a public static class with this exact name
/// and a public static <c>Snapshot()</c> in every loaded mod, by reflection. Nothing here
/// references Spire Link, and with it absent this class is simply never called.
///
/// Read-only. Called on the main thread each time Spire Link builds a state, so it must stay
/// cheap and must not throw for a non-Seductress run or outside a run - it returns null entries
/// instead.
/// </summary>
public static class SpireLinkExport
{
    public static Dictionary<string, object?> Snapshot()
    {
        var run = RunManager.Instance.DebugOnlyGetState();
        var player = run == null ? null : LocalContext.GetMe(run);
        var result = new Dictionary<string, object?>
        {
            ["in_run"] = player != null,
            ["is_seductress"] = player?.Character is Character.Seductress,
        };
        if (player == null || player.Character is not Character.Seductress) return result;

        var r = Reservoirs.Of(player.RunState);
        var creature = player.Creature;

        result["arousal"] = new Dictionary<string, object?>
        {
            // The bar is inverted: arousal is HP missing, and a full bar is a Climax.
            ["arousal"] = creature.MaxHp - creature.CurrentHp,
            ["limit"] = creature.MaxHp,
            ["hp"] = creature.CurrentHp,
        };

        result["reservoirs_ml"] = new Dictionary<string, object?>
        {
            ["mouth_cum"] = r.Mouth,
            ["mouth_precum"] = r.Precum,
            ["mouth_total"] = r.MouthTotal,
            ["mouth_cap"] = ReservoirState.MouthCap,
            ["ass"] = r.Ass,
            ["ass_precum"] = r.AssPrecum,
            ["ass_cap"] = ReservoirState.AssCap,
            ["ass_high_water"] = r.AssHighWater,
            ["plug_in"] = r.PlugIn,
            ["womb"] = r.Womb,
            ["womb_precum"] = r.WombPrecum,
            ["womb_locked"] = r.WombLocked,
            ["womb_leakable"] = Pregnancy.LeakableWomb(r),
            ["womb_per_stage"] = ReservoirState.WombStage,
            ["skin"] = r.Skin,
            ["stomach"] = r.Stomach,
        };

        result["stomach"] = new Dictionary<string, object?>
        {
            ["max_hp_earned"] = Stomach.MaxHpEarned(r.Stomach),
            ["combat_start_precum"] = Stomach.CombatStartPrecum(r.Stomach),
            ["swallows_at_fifty"] = Stomach.SwallowsAtFifty(r.Stomach),
            ["indulge_is_clean"] = Stomach.IndulgeIsClean(r.Stomach),
            ["precum_banks"] = Stomach.PrecumBanks(r.Stomach),
        };

        result["pregnancy"] = new Dictionary<string, object?>
        {
            ["stage"] = r.PregnancyStage,
            ["lactating_stage"] = r.LactatingStage,
            ["lactation_tail"] = r.LactationTail,
        };

        var climax = ClimaxModel.Instance;
        result["sigils"] = new Dictionary<string, object?>
        {
            ["owned"] = r.Sigils,
            ["effective"] = Sigils.Effective(player),
            ["exhausted_this_combat"] = climax.SigilsExhaustedThisCombat,
        };

        result["lust"] = new Dictionary<string, object?>
        {
            ["count"] = ClimaxModel.LustCount(player),
            ["limit"] = ClimaxModel.LustLimit,
        };

        result["run_totals"] = new Dictionary<string, object?>
        {
            ["climaxes"] = FloorLog.ClimaxesThisRun,
            ["reliefs"] = FloorLog.ReliefsThisRun,
        };

        if (FloorLog.Current is { } floor)
        {
            result["this_floor"] = new Dictionary<string, object?>
            {
                ["floor"] = floor.Floor,
                ["room"] = floor.Room.ToString(),
                ["arousal_on_entry"] = floor.ArousalOnEntry,
                ["max_hp_on_entry"] = floor.MaxHpOnEntry,
                ["intake_gained"] = floor.IntakeGained,
                ["leak_lost"] = floor.LeakLost,
                ["leak_to_skin"] = floor.LeakToSkin,
                ["relief_taken"] = floor.ReliefTaken,
                ["indulge_skin"] = floor.IndulgeSkin,
                ["indulge_recovered"] = floor.IndulgeRecovered,
                ["birth_stage"] = floor.BirthStage,
                ["birth_wasted"] = floor.BirthWasted,
                ["milk_generated"] = floor.MilkGenerated,
            };
        }

        if (CombatManager.Instance.IsInProgress)
        {
            var combat = new Dictionary<string, object?>
            {
                ["continuing_hole"] = climax.ContinuingHole.ToString(),
                ["climaxes_this_combat"] = climax.ClimaxesThisCombat,
                ["sensitive"] = creature.GetPowerAmount<SensitivePower>(),
                ["last_attack_played"] = climax.LastAttackPlayed?.Id.Entry,
            };
            // Where each card in hand would send its intake if played now, in hand order - the
            // same index Spire Link uses for play_card.
            if (player.PlayerCombatState?.Hand is { } hand)
            {
                combat["hand_routes"] = hand.Cards.Select((card, index) => (object?)new Dictionary<string, object?>
                {
                    ["index"] = index,
                    ["id"] = card.Id.Entry,
                    ["tagged_hole"] = card.TaggedHole().ToString(),
                    ["routes_to"] = climax.RouteOf(card).ToString(),
                }).ToList();
            }
            result["combat"] = combat;
        }

        return result;
    }
}
