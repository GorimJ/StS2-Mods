using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Seductress.SeductressCode.Powers;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// Staged gestation (design §6.3b) — the mechanic the whole character was built around, and until
/// this slice it had never existed.
///
/// <b>Stage N at N x 100 ml in the Womb.</b> Per stage: N Sensitive at combat start, N Milk
/// shuffled into the draw pile per turn, N Sigils on birth.
///
/// [Claude-Cowork] <b>The Womb keeps filling while you carry.</b> Further intake advances the
/// stage rather than conceiving again, which is what makes gestation a decision rather than a
/// wait: every fight you stay pregnant is a fight you could have cashed out instead, and both the
/// cost and the reward climb together.
///
/// [Claude-Cowork] <b>Stages are uncapped.</b> Nothing stops Stage 4 at 400 ml. Pushing further is
/// meant to be possible and dangerous rather than blocked.
/// </summary>
public static class Pregnancy
{
    /// <summary>The stage a given Womb volume supports. 250 ml is Stage 2, not Stage 2.5.</summary>
    public static int StageFor(int womb) => womb / ReservoirState.WombStage;

    public static bool IsPregnant(ReservoirState r) => r.PregnancyStage > 0;
    public static bool IsLactating(ReservoirState r) => r.LactatingStage > 0;

    /// <summary>
    /// The portion of the Womb that can leak: everything above the locked threshold (§6.3b).
    ///
    /// [Claude-Cowork] A stage's N x 100 ml is committed and cannot bleed away. The consequence is
    /// that the Womb's leak is a bounded 0-10 ml sawtooth that resets at every threshold — the
    /// same whether you are filling or carrying, which two earlier drafts got wrong.
    /// </summary>
    public static int LeakableWomb(ReservoirState r) => Math.Max(0, r.Womb - r.WombLocked);

    /// <summary>
    /// Resolved at the <b>end of combat</b>, not on crossing (design §6.3b).
    ///
    /// [Claude-Cowork] You do not count as pregnant during the fight that earned it — no lactation
    /// mid-combat, and the new Sensitive stack does not land on the fight that paid for it — but
    /// you are pregnant on leaving the room. The intended consequence is that spending the Womb
    /// back below the threshold before the bell <b>prevents</b> conception. That is agency, not an
    /// exploit to close.
    /// </summary>
    public static void ResolveEndOfCombat(Player player)
    {
        var r = Reservoirs.Of(player.RunState);
        var stage = StageFor(r.Womb);

        if (stage > r.PregnancyStage)
        {
            var was = r.PregnancyStage;
            r.PregnancyStage = stage;
            r.LactatingStage = stage;
            r.LactationTail = false;   // conceiving during a tail re-enters Pregnant; lactation never lapses
            MainFile.Logger.Info(
                was == 0
                    ? $"[Seductress] CONCEPTION — Stage {stage} at {r.Womb} ml"
                    : $"[Seductress] GESTATION advanced {was} -> {stage} at {r.Womb} ml");
            return;
        }

        // The lactation tail: one further combat after the birth, at the stage delivered at.
        if (r.LactationTail)
        {
            MainFile.Logger.Info($"[Seductress] Lactation ends (tail combat over, was stage {r.LactatingStage})");
            r.LactatingStage = 0;
            r.LactationTail = false;
        }
    }

    /// <summary>
    /// Birth, taken at a rest site as a free action.
    ///
    /// [Claude-Cowork] <b>The Womb empties completely and the excess is wasted</b> — on the floor,
    /// not to Skin, not to the Stomach. This is the one place in the design where output is
    /// destroyed rather than recycled, and it is what creates natural delivery points at exactly
    /// N x 100: cashing out at 299 ml wastes 99, at 300 wastes nothing. The question stops being a
    /// continuous "carry or deliver" and becomes "am I close enough to the next threshold".
    /// </summary>
    public static void GiveBirth(Player player)
    {
        var r = Reservoirs.Of(player.RunState);
        var stage = r.PregnancyStage;
        if (stage <= 0) return;

        var wasted = r.Womb - r.WombLocked;

        r.Sigils += stage;
        r.Womb = 0;
        r.PregnancyStage = 0;
        r.LactatingStage = stage;   // lactation continues one combat, at the stage delivered at
        r.LactationTail = true;

        MainFile.Logger.Info(
            $"[Seductress] BIRTH at stage {stage}: +{stage} Sigils (now {r.Sigils}), " +
            $"Womb emptied, {wasted} ml wasted");
        FloorLog.RecordBirth(stage, wasted);
    }

    /// <summary>
    /// N Sensitive at the start of each combat while carrying. Applied as the ordinary power, so
    /// it compounds with climax stacks exactly as any other Sensitive does.
    /// </summary>
    public static async Task ApplyCombatStartSensitive(Player player)
    {
        var r = Reservoirs.Of(player.RunState);
        if (r.PregnancyStage <= 0) return;

        await PowerCmd.Apply<SensitivePower>(
            new ThrowingPlayerChoiceContext(), player.Creature, r.PregnancyStage, player.Creature, null);
        MainFile.Logger.Info($"[Seductress] Pregnant stage {r.PregnancyStage}: +{r.PregnancyStage} Sensitive at combat start");
    }

    /// <summary>
    /// N Milk shuffled into the <b>combat</b> draw pile at the start of each of your turns.
    ///
    /// [Claude-Cowork] The draw pile rather than the hand is deliberate and is the cost half of
    /// the mechanic: into hand it would be a guaranteed extra card every turn, while into the pile
    /// it competes with your real cards for draws. At Stage 3 that is three a turn, so a long
    /// fight becomes mostly Milk — dilution that is one more thing arguing for cashing out.
    /// </summary>
    public static async Task ShuffleMilk(Player player)
    {
        var r = Reservoirs.Of(player.RunState);
        if (r.LactatingStage <= 0) return;

        await CardPileCmd.AddToCombatAndPreview<Cards.Milk>(
            player.Creature, PileType.Draw, r.LactatingStage, player, CardPilePosition.Random);
        FloorLog.RecordMilk(r.LactatingStage);
        MainFile.Logger.Info($"[Seductress] Lactating stage {r.LactatingStage}: shuffled {r.LactatingStage} Milk into the draw pile");
    }
}
