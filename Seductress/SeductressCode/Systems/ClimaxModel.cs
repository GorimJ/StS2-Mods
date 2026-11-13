using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Seductress.SeductressCode.Cards;
using Seductress.SeductressCode.Powers;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// Spike questions 2 and 3.
///
/// A <see cref="CustomSingletonModel"/> registered for combat hooks is put into
/// <c>CombatState.IterateHookListeners</c> by BaseLib, which is the same list vanilla relics and
/// potions are iterated from. That means it receives <c>ShouldDie</c> and
/// <c>AfterPreventingDeath</c> — the exact pair Lizard Tail and Fairy in a Bottle use — so death
/// interception needs no Harmony patch at all, and it catches every path that routes through
/// <c>CreatureCmd.Kill</c>, self-damage cards included.
///
/// It also tracks the <i>Continuing</i> routing mode, which is what makes the mod-defined card
/// tags observable in play.
/// </summary>
public class ClimaxModel : CustomSingletonModel
{
    /// <summary>
    /// Set by the constructor. There is exactly one instance, created by <c>ModelDb.Init</c> —
    /// see the note in MainFile for why the mod must not construct one of its own.
    /// </summary>
    public static ClimaxModel Instance { get; private set; } = null!;

    public ClimaxModel() : base(HookType.Combat) => Instance = this;

    /// <summary>
    /// The number of Lust cards at which a Climax becomes fatal instead.
    ///
    /// Holding this many means the next time you hit 0 HP the run ends, so the card that would
    /// have been the (LustLimit + 1)th is never gained — the deck tops out at LustLimit.
    ///
    /// [decided 2026-09-06, Gorim] Three. Climaxing while holding three Lust ends the run, so a
    /// fourth card is never gained. SEDUCTRESS_DESIGN.md §5 still reads "5 Lust in deck ends the
    /// run" and is being reworked separately; the shorter clock is paired with a Lust-removal
    /// option at rest sites, which is not built here. See D3 in SEDUCTRESS_SPIKE_RESULTS.md.
    /// </summary>
    public const int LustLimit = 3;

    /// <summary>
    /// The hole the last tagged Attack used this combat. <i>Continuing</i> cards follow it.
    /// Resets to Skin each combat, so turn-one Pumps route to Skin until you lead with a tag.
    /// </summary>
    public Hole ContinuingHole { get; private set; } = Hole.Skin;

    /// <summary>How many times the player has Climaxed this combat. Also read by Surrender.</summary>
    public int ClimaxesThisCombat { get; private set; }

    /// <summary>Sigils spent by <i>Consummate</i> for the rest of this combat (design §7).</summary>
    public int SigilsExhaustedThisCombat { get; private set; }

    public void ExhaustSigil()
    {
        SigilsExhaustedThisCombat++;
        // You lose that Sigil's passive until the fight ends (§7), so its orb goes with it.
        var me = MegaCrit.Sts2.Core.Context.LocalContext.GetMe(
            MegaCrit.Sts2.Core.Combat.CombatManager.Instance.DebugOnlyGetState());
        if (me?.Character is Character.Seductress) _ = SyncSigilOrbs(me);
    }

    /// <summary>
    /// Enemies present when the combat began. Needed because §6.2 prices an elite differently for
    /// one enemy (40 ml) than for several (20 ml each), and by the time the last one dies the rest
    /// have already been removed from the combat state.
    ///
    /// Counted as "still alive + already killed this combat" rather than sampled at turn
    /// boundaries, so it does not depend on a hook having fired before the first kill — an early
    /// kill on turn one would otherwise see a count of zero and price a multi-enemy elite as solo.
    ///
    /// Waves that spawn mid-fight are included as they arrive, so an elite with a solo first stage
    /// is priced as multi-enemy once the adds appear. Kills landed while it really was alone are
    /// still priced solo, which is the reading that matches "elite, multi-enemy: 20 ml each".
    /// </summary>
    private int _monstersKilledThisCombat;

    /// <summary>
    /// The combat the kill counter belongs to. Comparing against this on every death means the
    /// counter self-resets when the combat changes, so it cannot leak across fights if
    /// BeforeCombatStart is ever missed or runs out of order — which would read a solo elite as
    /// multi and underpay it.
    /// </summary>
    private ICombatState? _countingFor;

    private void EnsureCountingFor(ICombatState? combat)
    {
        if (ReferenceEquals(combat, _countingFor)) return;
        _countingFor = combat;
        _monstersKilledThisCombat = 0;
    }

    /// <summary>
    /// Set by a card while its own attack resolves, so kills it causes pay extra intake
    /// (<i>Breed Me!</i>). Routing itself stays in AfterDeath so there is one place that decides
    /// where ml goes.
    ///
    /// The flat bonus and the multiplier are alternatives, not cumulative: a multiplier above 1
    /// <b>replaces</b> the flat bonus. [clarified 2026-09-06, Gorim] — note this differs from
    /// SEDUCTRESS_CARDS.md §8, which says the tiers multiply "the card's total kill intake
    /// including that 5".
    /// </summary>
    private int _killIntakeFlat;
    private int _killIntakeMultiplier = 1;

    public void SetKillIntakeBonus(int flat, int multiplier)
    {
        _killIntakeFlat = flat;
        _killIntakeMultiplier = multiplier;
    }

    public void ClearKillIntakeBonus()
    {
        _killIntakeFlat = 0;
        _killIntakeMultiplier = 1;
    }

    /// <summary>The last Attack card played this combat, tag included. Reset each combat.</summary>
    public CardModel? LastAttackPlayed { get; private set; }

    private static bool IsSeductress(Creature? creature) =>
        creature is { IsPlayer: true } && creature.Player!.Character is Character.Seductress;

    public override Task BeforeCombatStart()
    {
        ContinuingHole = Hole.Skin;
        ClimaxesThisCombat = 0;
        SigilsExhaustedThisCombat = 0;
        LastAttackPlayed = null;
        _monstersKilledThisCombat = 0;
        return Task.CompletedTask;
    }

    // ---------------------------------------------------------------- Continuing / tags

    /// <summary>
    /// Only Attacks set the mode, and only tagged ones — that is the whole rule (design §6.0).
    /// A Continuing card inherits the mode rather than setting it, so Pump never overwrites it.
    ///
    /// This must run <b>before</b> the card resolves, not after. A tagged attack that lands the
    /// killing blow triggers AfterDeath from inside its own resolution, so setting the mode
    /// afterwards routes the card's own kill by the <i>previous</i> tag — which is the common case
    /// and the whole point of the tag. Found in play: a Deep Throat kill banked to Skin.
    /// </summary>
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Type != CardType.Attack) return Task.CompletedTask;
        LastAttackPlayed = card;
        if (card is SeductressCard { IsContinuing: true }) return Task.CompletedTask;

        var hole = card.TaggedHole();
        if (hole != Hole.None)
        {
            ContinuingHole = hole;
            MainFile.Logger.Info($"[Seductress] Continuing mode set to {hole} by {card.Id.Entry}");
        }
        return Task.CompletedTask;
    }

    /// <summary>The hole a card actually routes to right now: its own tag, or the Continuing mode.</summary>
    public Hole RouteOf(CardModel card)
    {
        if (card is SeductressCard { IsContinuing: true }) return ContinuingHole;
        var hole = card.TaggedHole();
        return hole == Hole.None ? ContinuingHole : hole;
    }

    // ---------------------------------------------------------------- Climax

    /// <summary>
    /// Vanilla's own death-prevention hook. Returning false here routes the kill into
    /// <see cref="AfterPreventingDeath"/> instead of ending the run.
    /// </summary>
    public override bool ShouldDie(Creature creature)
    {
        if (!IsSeductress(creature)) return true;                 // enemies die normally
        if (LustCount(creature.Player!) >= LustLimit) return true; // the death clock ran out
        return false;
    }

    /// <summary>
    /// Climax (design §4): arousal resets to 0, +1 Sensitive for the combat, +1 Lust permanently.
    ///
    /// Healing to full is not decoration — <c>KillWithoutCheckingWinCondition</c> zeroes HP before
    /// asking, then re-enters itself if the creature is still dead afterwards, so a preventer that
    /// does not restore HP recurses ten times and throws.
    /// </summary>
    public override async Task AfterPreventingDeath(Creature creature)
    {
        if (!IsSeductress(creature)) return;
        await DoClimax(creature);
    }

    /// <summary>
    /// The Climax itself, separated from the death hook so a card can trigger one on purpose.
    /// Everything that reaches a Climax goes through here.
    ///
    /// [Gorim 2026-09-07] The two costs are separable. <i>Seek Relief</i> takes the Sensitive and
    /// the listener triggers while paying neither the heal nor the Lust card, which is what makes
    /// a cautious deck able to turn on its Climax cards at all without spending the run's death
    /// clock to do it.
    ///
    /// [Claude-Code] Note what dropping the heal actually means: arousal stays exactly where it
    /// is, and the new Sensitive stack raises everything you take afterwards by 50%. So a Climax
    /// without the reset is not free — it is a percentage cut to the bar you have left, taken now
    /// and paid for the rest of the fight.
    /// </summary>
    /// <param name="heal">Reset arousal to 0. False for a Climax that only carries the cost.</param>
    /// <param name="addLust">Add a permanent Lust card — the run's death clock.</param>
    public async Task DoClimax(Creature creature, bool heal = true, bool addLust = true)
    {
        var player = creature.Player!;

        ClimaxesThisCombat++;
        var shape = (heal ? "arousal reset" : "NO reset") + ", +1 Sensitive, " + (addLust ? "+1 Lust" : "NO Lust");
        MainFile.Logger.Info($"[Seductress] CLIMAX #{ClimaxesThisCombat} — {shape}");

        // 1. Arousal resets to 0 — i.e. HP back to full, since arousal is MaxHp - CurrentHp.
        if (heal) await CreatureCmd.Heal(creature, creature.MaxHp);

        // 2. +1 Sensitive for the rest of combat. Never optional: this is the Climax.
        await PowerCmd.Apply<SensitivePower>(new ThrowingPlayerChoiceContext(), creature, 1m, creature, null);

        // 3. +1 Lust card, permanently. No exception for the first climax of a run.
        // Which of the two Lust cards is drawn from the run's Niche stream, so it stays seeded.
        if (addLust)
        {
            if (player.RunState.Rng.Niche.NextBool())
                await CardPileCmd.AddCurseToDeck<Aching>(player);
            else
                await CardPileCmd.AddCurseToDeck<Craving>(player);
        }

        FloorLog.RecordClimax();

        // 4. Powers that trigger on Climax. No vanilla hook exists for our own event, so we walk
        //    the owner's powers. ToList() because a listener may add or remove powers.
        foreach (var listener in creature.Powers.OfType<IClimaxListener>().ToList())
        {
            await listener.OnClimax(creature);
        }
    }

    public static int LustCount(Player player) =>
        player.Deck.Cards.Count(c => c is LustCard);

    // ---------------------------------------------------------------- Sigil orbs

    /// <summary>
    /// Keeps the orb queue equal to the effective Sigil count — same number of orbs as slots, so
    /// there are never empty slots implying a cap. §7 rules Sigils are not Defect orbs; the queue
    /// is borrowed as a display only.
    ///
    /// Run before the queue's own turn-start trigger so the passive fires on turn one, and it also
    /// picks up mid-combat changes: <i>Consummate</i> exhausting a Sigil removes an orb, and the
    /// test console command is reflected immediately.
    ///
    /// Shrinking goes through RemoveSlots, which trims orbs past the new capacity itself.
    /// </summary>
    private static async Task SyncSigilOrbs(Player player)
    {
        var queue = player.PlayerCombatState?.OrbQueue;
        if (queue is null) return;

        var target = Sigils.Effective(player);

        if (queue.Capacity > target) OrbCmd.RemoveSlots(player, queue.Capacity - target);
        if (queue.Capacity < target) await OrbCmd.AddSlots(player, target - queue.Capacity);

        while (queue.Orbs.Count < target)
        {
            await OrbCmd.Channel<Orbs.SigilOrb>(new ThrowingPlayerChoiceContext(), player);
        }
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player) return;
        var me = MegaCrit.Sts2.Core.Context.LocalContext.GetMe(combatState);
        if (me?.Character is not Character.Seductress) return;

        await SyncSigilOrbs(me);
        await Pregnancy.ShuffleMilk(me);
    }

    /// <summary>
    /// Combat-start grants that need the creature to exist: the Stomach's precum milestone and the
    /// pregnancy Sensitive stack. Late rather than early so powers applied here are not wiped by
    /// the ordinary combat setup.
    /// </summary>
    public override async Task BeforeCombatStartLate()
    {
        var me = MegaCrit.Sts2.Core.Context.LocalContext.GetMe(
            MegaCrit.Sts2.Core.Runs.RunManager.Instance.DebugOnlyGetState());
        if (me?.Character is not Character.Seductress) return;

        var r = Reservoirs.Of(me.RunState);

        var precum = Stomach.CombatStartPrecum(r.Stomach);
        if (precum > 0)
        {
            var added = r.AddPrecum(precum);
            MainFile.Logger.Info($"[Seductress] Stomach milestone: +{added} ml precum at combat start (Stomach {r.Stomach})");
        }

        await Pregnancy.ApplyCombatStartSensitive(me);
    }

    // ---------------------------------------------------------------- Intake

    /// <summary>
    /// Base intake per enemy climax, from design §6.2. This was a flat 10 through the spike and
    /// the first half of slice 2, which underpaid every elite and boss — and since §6.3 makes
    /// elites and bosses "the pregnancy moments", that quietly broke the whole pregnancy pacing.
    /// </summary>
    private int BaseIntakeFor(Player me, ICombatState? combat)
    {
        // Minions are excluded, so an elite that summons adds stays a solo elite at 40 rather
        // than being demoted to the cheaper multi rate.
        var alive = combat?.Enemies.Count(e => e.IsAlive && !e.IsSecondaryEnemy) ?? 0;
        var encounterSize = alive + _monstersKilledThisCombat;

        return me.RunState.CurrentRoom?.RoomType switch
        {
            RoomType.Boss => 60,
            // [decided 2026-09-06, Gorim] Multi-enemy elites pay 15, not the 20 in §6.2 — so a
            // three-enemy elite is 45 rather than 60, below a solo elite's 40 per kill but still
            // the better total.
            RoomType.Elite => encounterSize > 1 ? 15 : 40,
            _ => 10
        };
    }

    /// <summary>
    /// When an enemy climaxes (dies), their output routes via the finishing attack's tag.
    ///
    /// SPIKE SIMPLIFICATION: this reads the current Continuing mode rather than the specific card
    /// that landed the killing blow, and pays a flat normal-enemy 10 ml with no elite/boss tiers.
    /// Enough to exercise routing, the Sensitive multiplier and the display.
    /// </summary>
    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        CardModel? source, bool wasRemovalPrevented, float deathAnimLength)   // 0.113 added `source` (unused here)
    {
        if (wasRemovalPrevented || !creature.IsMonster) return Task.CompletedTask;

        // [decided 2026-09-06, Gorim] Minions produce no intake. Not in either design doc; see
        // SEDUCTRESS_SLICE2_RESULTS.md. IsSecondaryEnemy is the game's own definition — an enemy
        // carrying MinionPower or IllusionPower, which cannot survive without a living primary.
        //
        // Without this, any summoner is an intake fountain: minions are individually cheap to kill
        // and often respawn, so they would pay better per effort than the enemy summoning them.
        if (creature.IsSecondaryEnemy)
        {
            MainFile.Logger.Info($"[Seductress] No intake from minion {creature.Monster?.Id.Entry}");
            return Task.CompletedTask;
        }

        var me = MegaCrit.Sts2.Core.Context.LocalContext.GetMe(creature.CombatState);
        if (me?.Character is not Character.Seductress) return Task.CompletedTask;

        EnsureCountingFor(creature.CombatState);
        _monstersKilledThisCombat++;

        var multiplier = SensitivePower.MultiplierFor(me.Creature);
        var baseMl = BaseIntakeFor(me, creature.CombatState);
        // Sensitive multipliers on intake round UP, in the player's favour (design §6.2).
        var ml = (int)System.Math.Ceiling(baseMl * multiplier);


        ml = _killIntakeMultiplier > 1 ? ml * _killIntakeMultiplier : ml + _killIntakeFlat;

        // [Gorim] Precedence: minions bank nothing at all (that test already returned above);
        // then Leg-Locked, which overrides where intake goes but never whether it happens; then
        // the finishing attack's tag; then the Skin catch-all. Design §6.3a.
        var legLocked = creature.HasPower<Powers.LegLockedPower>();
        var hole = legLocked ? Hole.Pussy : ContinuingHole;
        if (legLocked) MainFile.Logger.Info("[Seductress] Leg-Locked claim: routing to Pussy");
        // Milking Grip: Ass-tagged kills gain double intake (design §7).
        if (hole == Hole.Ass && me.Creature.HasPower<Powers.MilkingGripPower>()) ml *= 2;

        var res = Reservoirs.Of(me.RunState);
        var before = res.Get(hole);
        res.Add(hole, ml);
        var landed = res.Get(hole) - before;

        FloorLog.RecordIntake(ml);
        MainFile.Logger.Info(
            $"[Seductress] Intake {ml} ml -> {hole} (base {baseMl}, sensitive x{multiplier}, " +
            $"{me.RunState.CurrentRoom?.RoomType})" +
            (landed < ml ? $" — {ml - landed} ml did not fit and is lost" : "") +
            (hole == Hole.Ass && res.PlugIn ? " — BLOCKED, the Plug is in" : ""));

        // The 100 ml Stomach milestone swallows a full Mouth mid-combat, which uncaps throughput.
        return hole == Hole.Mouth ? Stomach.SwallowIfFull(me) : Task.CompletedTask;
    }
}
