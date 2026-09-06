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
    /// Resets to Glazed each combat, so turn-one Pumps route to Glazed until you lead with a tag.
    /// </summary>
    public Hole ContinuingHole { get; private set; } = Hole.Glazed;

    /// <summary>How many times the player has Climaxed this combat. Debug readout only for now.</summary>
    public int ClimaxesThisCombat { get; private set; }

    /// <summary>The last Attack card played this combat, tag included. Reset each combat.</summary>
    public CardModel? LastAttackPlayed { get; private set; }

    private static bool IsSeductress(Creature? creature) =>
        creature is { IsPlayer: true } && creature.Player!.Character is Character.Seductress;

    public override Task BeforeCombatStart()
    {
        ContinuingHole = Hole.Glazed;
        ClimaxesThisCombat = 0;
        LastAttackPlayed = null;
        return Task.CompletedTask;
    }

    // ---------------------------------------------------------------- Continuing / tags

    /// <summary>
    /// Only Attacks set the mode, and only tagged ones — that is the whole rule (design §6.0).
    /// A Continuing card inherits the mode rather than setting it, so Pump never overwrites it.
    /// </summary>
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
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
        var player = creature.Player!;

        ClimaxesThisCombat++;
        MainFile.Logger.Info($"[Seductress] CLIMAX #{ClimaxesThisCombat} — arousal reset, +1 Sensitive, +1 Lust");

        // 1. Arousal resets to 0 — i.e. HP back to full, since arousal is MaxHp - CurrentHp.
        await CreatureCmd.Heal(creature, creature.MaxHp);

        // 2. +1 Sensitive for the rest of combat.
        await PowerCmd.Apply<SensitivePower>(new ThrowingPlayerChoiceContext(), creature, 1m, creature, null);

        // 3. +1 Lust card, permanently. No exception for the first climax of a run.
        await CardPileCmd.AddCurseToDeck<Aching>(player);
    }

    public static int LustCount(Player player) =>
        player.Deck.Cards.Count(c => c is Aching);

    // ---------------------------------------------------------------- Intake

    /// <summary>
    /// When an enemy climaxes (dies), their output routes via the finishing attack's tag.
    ///
    /// SPIKE SIMPLIFICATION: this reads the current Continuing mode rather than the specific card
    /// that landed the killing blow, and pays a flat normal-enemy 10 ml with no elite/boss tiers.
    /// Enough to exercise routing, the Sensitive multiplier and the display.
    /// </summary>
    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature,
        bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || !creature.IsMonster) return Task.CompletedTask;

        var me = MegaCrit.Sts2.Core.Context.LocalContext.GetMe(creature.CombatState);
        if (me?.Character is not Character.Seductress) return Task.CompletedTask;

        var multiplier = SensitivePower.MultiplierFor(me.Creature);
        // Sensitive multipliers on intake round UP, in the player's favour (design §6.2).
        var ml = (int)System.Math.Ceiling(10m * multiplier);

        var hole = ContinuingHole;
        Reservoirs.Of(me.RunState).Add(hole, ml);
        MainFile.Logger.Info($"[Seductress] Intake {ml} ml -> {hole}");
        return Task.CompletedTask;
    }
}
