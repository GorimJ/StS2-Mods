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
        Stripped = 0;
        return Task.CompletedTask;
    }

    /// <summary>
    /// [Gorim, 2026-11-20] "Every Power you play strips an item of clothing." [Gorim, 2026-11-22]
    /// Order: "Sleeves and shoes, then wrap, then skirt"; she is dressed again each combat. A
    /// fourth stage waits on a new pose. Shown by <see cref="UI.RigVisuals"/>; no rules effect.
    /// </summary>
    public int Stripped { get; set; }

    public const int MaxStripped = 3;

    // ---------------------------------------------------------------- Continuing / tags

    /// <summary>
    /// Only Attacks set the mode, and only tagged ones — that is the whole rule (design §6.0).
    /// A Continuing card inherits the mode rather than setting it, so More never overwrites it.
    ///
    /// This must run <b>before</b> the card resolves, not after. A tagged attack that lands the
    /// killing blow triggers AfterDeath from inside its own resolution, so setting the mode
    /// afterwards routes the card's own kill by the <i>previous</i> tag — which is the common case
    /// and the whole point of the tag. Found in play: a Deep Throat kill banked to Skin.
    /// </summary>
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        UI.HoleInsets.Current?.CardPlayed();
        if (card.Type == CardType.Power && card.Owner?.Character is Character.Seductress && Stripped < MaxStripped)
        {
            Stripped++;
            MainFile.Logger.Info($"[Seductress] {card.Id.Entry} played: strip stage {Stripped}");
        }
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
    ///
    /// [Gorim, 2026-11-23] "Above a certain number of sigils they stop being able to load in
    /// correctly and it softlocks. Maybe about 10?" [Claude] The game caps the orb queue at 10
    /// slots, so with 11 or more Sigils the channel loop below never reached its target and ran
    /// forever. The queue now shows at most <see cref="MaxSigilOrbs"/>; the Sigils past that still
    /// deal their damage, fired by the first orb (<see cref="Orbs.SigilOrb"/>).
    /// </summary>
    public const int MaxSigilOrbs = MegaCrit.Sts2.Core.Entities.Orbs.OrbQueue.maxCapacity;

    private static async Task SyncSigilOrbs(Player player)
    {
        var queue = player.PlayerCombatState?.OrbQueue;
        if (queue is null) return;

        var target = Math.Min(Sigils.Effective(player), MaxSigilOrbs);

        if (queue.Capacity > target) OrbCmd.RemoveSlots(player, queue.Capacity - target);
        if (queue.Capacity < target) await OrbCmd.AddSlots(player, target - queue.Capacity);

        // Bounded, and stops if a channel adds nothing, so it can never spin.
        for (var i = 0; i < MaxSigilOrbs && queue.Orbs.Count < target; i++)
        {
            var before = queue.Orbs.Count;
            await OrbCmd.Channel<Orbs.SigilOrb>(new ThrowingPlayerChoiceContext(), player);
            if (queue.Orbs.Count <= before) break;
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
            await Stomach.SwallowIfFull(me, precum, added, cum: false);
        }

        await Pregnancy.ApplyCombatStartSensitive(me);
    }

    // ---------------------------------------------------------------- Intake

    /// <summary>
    /// [Gorim, 2026-11-20] Share of an enemy's Max HP paid as cum to the finishing hole when it
    /// dies. Replaces the flat 10 / 15 / 40 / 60 ml table. Design note: reduce proportionally in
    /// multiplayer if that is ever built.
    /// </summary>
    public const decimal KillBonusShare = 0.25m;

    /// <summary>Where an enemy's output goes: Leg-Locked claims it for the Pussy, else the attack's hole.</summary>
    private static Hole HoleFor(Creature enemy, Hole attackHole)
    {
        // [Gorim] Leg-Locked overrides where intake goes but never whether it happens. Design §6.3a.
        return enemy.HasPower<Powers.LegLockedPower>() ? Hole.Pussy : attackHole;
    }

    /// <summary>
    /// The one place cum from an enemy enters a reservoir.
    /// </summary>
    /// <param name="mirrorAsPrecum">
    /// True for the per-hit gain, which <i>Edging Form</i> copies as precum. [Claude] The kill bonus
    /// is not copied: Gorim's wording was "per attack, +100% cum as precum", and a quarter of a
    /// boss's Max HP again as precum looked like more than that meant.
    /// </param>
    private static async Task GainCum(Player me, Hole hole, int ml, string why, bool mirrorAsPrecum)
    {
        if (ml <= 0) return;

        // [Gorim, 2026-11-17] Hyperfertile: twice as much Pussy cum.
        if (hole == Hole.Pussy && me.Creature.HasPower<Powers.HyperfertilePower>()) ml *= 2;

        var res = Reservoirs.Of(me.RunState);
        var before = res.Get(hole);
        res.Add(hole, ml);
        var landed = res.Get(hole) - before;

        FloorLog.RecordIntake(ml);
        MainFile.Logger.Info(
            $"[Seductress] Intake {ml} ml -> {hole} ({why})" +
            (landed < ml ? $" — {ml - landed} ml did not fit and is lost" : "") +
            (hole == Hole.Ass && res.PlugIn ? " — BLOCKED, the Plug is in" : ""));

        int edgingAsked = 0, edgingFitted = 0;
        if (mirrorAsPrecum && me.Creature.GetPower<Powers.EdgingFormPower>() is { } edging)
        {
            var pre = ml * (int)edging.Amount;
            var added = hole switch
            {
                Hole.Mouth => res.AddPrecum(pre),
                Hole.Ass => res.AddAssPrecum(pre),
                Hole.Pussy => res.AddWombPrecum(pre),
                _ => 0   // Skin is not a hole; Edging Form does nothing there
            };
            if (added > 0)
            {
                MainFile.Logger.Info($"[Seductress] Edging Form +{added} ml precum -> {hole}");
                if (hole == Hole.Mouth) await Stomach.AfterPrecumGained(me, pre);
            }
            if (hole == Hole.Mouth) edgingAsked = pre;
            if (hole == Hole.Mouth) edgingFitted = added;
        }

        // The Gulp milestone swallows a full Mouth mid-combat, which uncaps throughput. The cum
        // goes first, then Edging Form's precum.
        if (hole == Hole.Mouth)
        {
            await Stomach.SwallowIfFull(me, ml, landed, cum: true);
            await Stomach.SwallowIfFull(me, edgingAsked, edgingFitted, cum: false);
        }
    }

    /// <summary>
    /// [Gorim, 2026-11-20] <b>Every Attack gains cum 1:1 with the HP it actually removes</b>, into
    /// the attack's hole. Nothing for overkill and nothing for damage Block absorbed, which is
    /// exactly what <c>UnblockedDamage</c> reports. Replaces register decision 44 ("Attacks and
    /// Skills do not generate intake").
    ///
    /// The Ass rule's bonus damage is ordinary damage by the time it lands, so it gains cum too.
    ///
    /// [Claude] Minions still pay nothing (the 2026-09-06 ruling), for hits as well as kills;
    /// otherwise a summoner is the same intake fountain that ruling closed.
    /// </summary>
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, MegaCrit.Sts2.Core.ValueProps.ValueProp props, Creature target, CardModel? cardSource)
    {
        if (!IsSeductress(dealer) || cardSource is null || cardSource.Type != CardType.Attack) return;
        if (!target.IsMonster || target.IsSecondaryEnemy) return;

        var ml = result.UnblockedDamage;
        if (ml <= 0) return;

        var hole = HoleFor(target, RouteOf(cardSource));
        UI.HoleInsets.Current?.Hit(hole, target);
        await GainCum(dealer!.Player!, hole, ml, $"{cardSource.Id.Entry} removed {ml} HP", mirrorAsPrecum: true);
    }

    /// <summary>
    /// When an enemy climaxes (dies), a quarter of its Max HP is paid to the finishing hole, on
    /// top of what the hits themselves gained.
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

        var baseMl = (int)System.Math.Ceiling(creature.MaxHp * KillBonusShare);
        var ml = _killIntakeMultiplier > 1 ? baseMl * _killIntakeMultiplier : baseMl + _killIntakeFlat;

        var hole = HoleFor(creature, ContinuingHole);
        // Milking Grip: Ass kills gain double intake (design §7).
        if (hole == Hole.Ass && me.Creature.HasPower<Powers.MilkingGripPower>()) ml *= 2;

        UI.HoleInsets.Current?.Finish(hole, creature);

        return GainCum(me, hole, ml, $"kill bonus, {KillBonusShare:P0} of {creature.MaxHp} Max HP", mirrorAsPrecum: false);
    }
}
