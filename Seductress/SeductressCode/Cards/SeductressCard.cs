using System.Linq;
using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Seductress.SeductressCode.Character;
using Seductress.SeductressCode.Extensions;
using Seductress.SeductressCode.Systems;
using System;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Base class for the character's cards. Adds the hole-tag layer on top of the vanilla
/// <see cref="CardTag"/> set — see <see cref="SeductressTags"/> for why mod-defined members work.
/// </summary>
[Pool(typeof(SeductressCardPool))]
public abstract class SeductressCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    /// <summary>
    /// The hole this attack uses. Tags go on Attacks only: a Skill never finishes anything, so it
    /// is never tagged however hole-specific its text is (design §6.0).
    /// </summary>
    public virtual Hole Hole => Hole.None;

    /// <summary>
    /// <i>Continuing</i> — this card uses whichever hole your last Attack used this combat.
    /// A Continuing card inherits the routing mode and never sets it.
    /// </summary>
    public virtual bool IsContinuing => false;

    /// <summary>
    /// [Gorim, 2026-11-21] Hover pop-outs for the character's own words (Gulp, Precum, Mouth...).
    /// A card gets the pop-out of every term its text uses; see <see cref="UI.Glossary"/>.
    /// </summary>
    protected override IEnumerable<MegaCrit.Sts2.Core.HoverTips.IHoverTip> ExtraHoverTips =>
        UI.Glossary.TipsFor(GetType(), () => (Localization ?? []).Select(pair =>
            SigilToken.IsMatch(pair.Item2) ? pair.Item2 + " Sigil" : pair.Item2));

    // ---- Sigil text ----------------------------------------------------------------------
    //
    // [Gorim, 2026-11-22] "This is too much text on a card. Perhaps the sigil stuff is only
    // visible in the right click menu? Could do a sliding bar of activating sigils to see how the
    // card changes?" Asked what the card itself should show, he chose what it does at your
    // current Sigil count, "but different effects need different sentences": one effect per line.
    //
    // [Claude] How: every description gets four switches, S1 to S4, one per tier. A card's text
    // uses them as {S1:cond:text with the tier|text without}, so one description covers every
    // Sigil count. The right-click view sets SigilPreview on its copy of the card to show any
    // count (UI.SigilSliderPatch); four icons on the card face show the tiers (UI.SigilMarks).

    private static readonly System.Text.RegularExpressions.Regex SigilToken = new(@"\{S[1-4]:");
    private static readonly Dictionary<Type, bool> TieredTypes = [];

    /// <summary>The right-click view's slider value, on its copy of the card only. Null in play.</summary>
    public int? SigilPreview;

    /// <summary>The Sigil count this card's text is written for.</summary>
    public int SigilCount => SigilPreview ?? (Owner is null ? 0 : Sigils.Effective(Owner));

    /// <summary>Whether this card's text changes with Sigils.</summary>
    public bool HasSigilTiers
    {
        get
        {
            var type = GetType();
            if (!TieredTypes.TryGetValue(type, out var tiered))
            {
                tiered = (Localization ?? []).Any(pair => SigilToken.IsMatch(pair.Item2));
                TieredTypes[type] = tiered;
            }
            return tiered;
        }
    }

    protected override void AddExtraArgsToDescription(MegaCrit.Sts2.Core.Localization.LocString description)
    {
        var sigils = SigilCount;
        description.Add("SigilN", (decimal)sigils);
        description.Add("S1", sigils >= Sigils.Tier1);
        description.Add("S2", sigils >= Sigils.Tier2);
        description.Add("S3", sigils >= Sigils.Tier3);
        description.Add("S4", sigils >= Sigils.Tier4);
        // True when the card's calculated numbers are live (in a combat pile, not a preview).
        // Elsewhere the text states the tier's number itself.
        description.Add("Live", SigilPreview is null
            && MegaCrit.Sts2.Core.Combat.CombatManager.Instance.IsInProgress
            && (Pile?.IsCombatPile ?? false));

        // [Gorim, 2026-11-22] Card text gives numbers, not where they come from: "You may spend
        // X Ass for X/10 bonus damage."
        if (AssSpendLeaks > 0)
        {
            var (lost, bonus) = AssSpendQuote(AssSpendLeaks);
            description.Add("AssSpend", (decimal)lost);
            description.Add("AssSpendBonus", (decimal)bonus);
        }
    }

    private bool _sigilDiscounted;

    /// <summary>
    /// "Costs 1 less" from a Sigil stage, applied once per combat as a reduction on top of
    /// whatever the card costs.
    ///
    /// [Gorim, 2026-11-23] "Getting Rapture for free still costs 2, presumably because of how
    /// we've set the reduced cost." [Claude] It was: the old code set the cost outright each time
    /// the card was drawn or entered combat, and a cost set later wins over an earlier "free".
    /// It also ignored the upgraded cost. Once given, the discount stays for the combat even if
    /// the Sigil count drops.
    /// </summary>
    protected void SigilDiscount(int sigils)
    {
        if (_sigilDiscounted || Owner is null || !Sigils.At(Owner, sigils)) return;
        _sigilDiscounted = true;
        EnergyCost.AddThisCombat(-1, reduceOnly: true);
    }

    /// <summary>How many floor-leaks of Ass this card may spend when played. 0: not a spender.</summary>
    protected virtual int AssSpendLeaks => 0;

    /// <summary>What Spend would cost and pay right now: ml lost, and bonus damage per hit on top of standing.</summary>
    protected (int Lost, int Bonus) AssSpendQuote(int leaks)
    {
        if (Owner is null) return (0, 0);
        var reservoirs = Reservoirs.Of(Owner.RunState);
        var notional = Math.Min(reservoirs.Ass, leaks * Systems.RunModel.AssLeakOf(reservoirs.Ass));
        var milking = Owner.Creature?.GetPower<Powers.MilkingGripPower>();
        var lost = milking is null ? notional : (int)Math.Ceiling(notional * milking.DrainShare);
        var spill = reservoirs.PlugIn ? (int)Math.Ceiling(reservoirs.Ass * Relics.Plug.SpillFraction) : 0;
        return (lost + spill, AssStrength(notional + spill));
    }

    /// <summary>The damage of the three tiered Pussy attacks: live in combat, the tier's printed number elsewhere.</summary>
    protected const string TieredDamage =
        "{Live:cond:{CalculatedDamage:diff()}|{S2:cond:{TierDamage:diff()}|{CalculatedDamage:diff()}}}";

    /// <summary>Extra mod tags beyond the hole, e.g. the Deep Throat family.</summary>
    protected virtual IEnumerable<CardTag> ExtraTags => [];

    protected override HashSet<CardTag> CanonicalTags
    {
        get
        {
            var tags = new HashSet<CardTag>(ExtraTags);
            var tag = Hole.ToTag();
            if (tag != CardTag.None) tags.Add(tag);
            return tags;
        }
    }

    /// <summary>Where this card's kills would route right now.</summary>
    protected Hole Route => ClimaxModel.Instance.RouteOf(this);

    /// <summary>
    /// The Ass tag rule (design §6.1): an Ass-tagged attack loses a share of the current Ass and
    /// converts it into <b>+1 damage per 1 ml lost, on every hit</b>.
    ///
    /// [Gorim 2026-09-07] One-to-one, not the "+1 per 10 ml" §6.1 currently states — 10 ml is
    /// +1 Strength, 200 ml is +20. At 1:10 the bank never paid off, because Ass leaks per floor
    /// *and* per Ass attack, so it rarely reaches the 100 ml where the old ratio produced its
    /// first point.
    ///
    /// Applied as flat per-hit bonus damage rather than a real Strength stack. That is equivalent
    /// for this card — Strength adds to each hit too — and it cannot leak into the rest of the
    /// turn, which "for that attack" requires.
    ///
    /// <i>Milking Grip</i> halves the drain but pays Strength as though it were full.
    /// </summary>
    /// <param name="notional">Ml this card spends before Milking Grip halves the loss.</param>
    /// <remarks>
    /// [Gorim, 2026-11-22] Only the spender cards (Blowout, Overflow) call this now, and only
    /// when the player picks Spend; see <see cref="AssSpendOrKeep"/>. Every other Ass attack uses
    /// <see cref="StandingAssBonus"/>.
    /// </remarks>
    protected int SpendAssForDamage(int notional)
    {
        var reservoirs = Reservoirs.Of(Owner.RunState);
        var milking = Owner.Creature.GetPower<Powers.MilkingGripPower>();

        var actual = milking is null ? notional : (int)Math.Ceiling(notional * milking.DrainShare);

        // The Plug: playing an Ass card pulls it and spills 10% on top of the card's own drain,
        // and that spill grants Strength like any other ml lost (design §7.3).
        var spill = 0;
        if (reservoirs.PlugIn)
        {
            spill = (int)Math.Ceiling(reservoirs.Ass * Relics.Plug.SpillFraction);
            reservoirs.PlugIn = false;
        }

        reservoirs.Ass -= actual + spill;
        if (reservoirs.Ass < 0) reservoirs.Ass = 0;

        var bonus = AssStrength(notional + spill);
        if (actual + spill > 0)
        {
            MainFile.Logger.Info(
                $"[Seductress] {Id.Entry} spent {actual + spill} ml Ass (+{bonus} dmg/hit" +
                (milking is not null ? $", Milking Grip x{milking.DrainShare:0.##}" : "") +
                (spill > 0 ? $", Plug pulled spilling {spill}" : "") + ")");
        }
        return bonus;
    }

    /// <summary>
    /// The Ass spend ratio: <b>+1 per 10 ml lost</b>.
    ///
    /// [Gorim, 2026-11-22] Fifth setting: "maybe lets make it X/10 damage instead", now that
    /// spending is optional and sits on top of the standing bonus. The history below is older.
    ///
    /// [Gorim, 2026-11-20] Fourth setting. The Ass rule stays (lose 10% per Ass attack, bonus per
    /// hit), the bonus damage now gains cum like any other damage, and attacks with three or more
    /// hits are meant to grow the bank without limit as the Squeezing direction. [Claude, design
    /// review] That needs 1:3 rather than 1:4, and this ratio is deliberately NOT multiplied by
    /// five with the other ml figures. Single-hit decks settle near 15x their hit size.
    ///
    /// [Gorim 2026-09-07] Third setting of this number. §6.1 originally said +1 per 10 ml; slice 3
    /// raised it to 1:1 because the bank never reached the 100 ml that produced a single point,
    /// and 1:1 then made <i>Blowout</i> deal 424 at a full bank. 1:4 is tuned against where an Ass
    /// deck actually settles <b>once Milking Grip is in play</b> — 200-270 ml — rather than
    /// against the unaided fixed point.
    ///
    /// [Claude-Code] That makes it dependent on the threshold unlocks landing: at the unaided
    /// 100 ml ceiling this pays +2 or +3, which is the inertness the change was made to fix.
    ///
    /// Rounds down, so a small bank pays nothing. The drain already rounds up, so both roundings
    /// run against the player here — worth watching at low banks, where the mechanic is meant to
    /// be weak but not literally free to ignore.
    /// </summary>
    protected static int AssStrength(int mlLost) => mlLost / 10;

    /// <summary>
    /// True when No Gag Reflex should cancel this card's HP cost. Deep Throat only.
    /// </summary>
    protected bool SuppressHpLoss =>
        Tags.Contains(SeductressTags.DeepThroat) && Owner.Creature.HasPower<Powers.NoGagReflexPower>();

    /// <summary>
    /// <i>Stretch My Hole Daddy</i>: attacks that cost you HP hit one additional time.
    /// Only this character's cards consult it, which is all the design needs.
    /// </summary>
    protected int StretchBonusHits(bool costsHp) =>
        costsHp && Owner.Creature.HasPower<Powers.StretchPower>() ? 1 : 0;

    /// <summary>
    /// Tier 4 (7 Sigils): the card repeats a number of times equal to its <b>current</b> cost.
    ///
    /// [Gorim] This rides <c>ModifyCardPlayCount</c> — the hook <c>DuplicationPower</c> uses —
    /// rather than being hand-rolled per card, so Powers repeat too: a repeated Power simply
    /// applies its buff twice. Hand-rolled <c>for</c> loops around DamageCmd cannot do that.
    ///
    /// Paying out against the current cost makes the tier self-correcting: a card that reduced its
    /// own cost at a lower tier has already banked that value and correctly repeats fewer times.
    /// 0-cost cards repeat zero times, so they shed a non-energy cost instead — handled per card.
    /// </summary>
    protected virtual bool RepeatsAtTier4 => false;

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (!RepeatsAtTier4 || !ReferenceEquals(card, this) || Owner is null) return playCount;
        if (!Sigils.At(Owner, Sigils.Tier4)) return playCount;

        var cost = EnergyCost.GetResolved();
        return cost > 0 ? playCount + cost : playCount;
    }

    /// <summary>
    /// Whether <i>Ritual Mark</i> buffs this card: <b>Pussy</b> and <b>Breasts</b>.
    ///
    /// [Gorim 2026-09-08] Breasts added as a second category by default, not as a tier. The two
    /// halves of Breeding are the cards that feed the Womb and the cards that do not, and the
    /// archetype's Power should pay for both.
    ///
    /// [Claude-Code] The card only declares whether it qualifies; the bonus itself is applied by
    /// <see cref="Powers.RitualMarkPower.ModifyDamageAdditive"/> on the vanilla damage hook, so it
    /// previews on the card face for free and no card adds it by hand any more.
    /// </summary>
    public bool IsRitualMarked =>
        Hole == Hole.Pussy || Tags.Contains(SeductressTags.Breasts);

    /// <summary>
    /// The Ass bonus this card <i>would</i> pay right now, without spending it. Preview only —
    /// <see cref="SpendAssForDamage"/> is what actually moves the bank.
    ///
    /// Milking Grip halves the spend but pays the full bonus, so the preview ignores it.
    /// </summary>
    protected decimal AssNotional() => Owner is null ? 0m : AssStanding(Reservoirs.Of(Owner.RunState).Ass);

    // ---- Ass: standing bonus, optional spend ---------------------------------------------
    //
    // [Gorim, 2026-11-22] "Ass needs some rethinking on balance. It's both too hard to
    // accumulate, and too good when you do." He chose: the bank gives a standing bonus, +1 damage
    // per hit per 50 ml held, and nothing is spent by ordinary Ass attacks. The cards that spent
    // extra (Blowout 50%, Overflow 20%) are the spending, and they ask Spend or Keep when played.
    // The floor leak stays as it was.
    //
    // [Gorim, 2026-11-22, later] "Spend should be lower now that not every attack spends.
    // Similar to leak maybe?" He chose: Overflow spends one floor's leak (5% of the Ass up to
    // 500 ml plus 25% of anything above), Blowout spends two, and spending pays on top of the
    // standing bonus: +1 per 3 ml at first, +1 per 10 ml from his next message.
    //
    // [Claude] The
    // Plug is still pulled by any Ass card and still spills 10%; on an ordinary attack that spill
    // now buys nothing. Milking Grip now only matters to the two spenders. All three are mine to
    // flag, not his decisions.

    /// <summary>Ml of Ass held per +1 damage on every hit.</summary>
    public const int AssMlPerDamage = 50;

    private static int AssStanding(int ml) => ml / AssMlPerDamage;

    /// <summary>What an ordinary Ass attack gets: the standing bonus. Pulls the Plug; spends nothing else.</summary>
    protected int StandingAssBonus()
    {
        var reservoirs = Reservoirs.Of(Owner.RunState);
        var bonus = AssStanding(reservoirs.Ass);
        if (reservoirs.PlugIn)
        {
            var spill = (int)Math.Ceiling(reservoirs.Ass * Relics.Plug.SpillFraction);
            reservoirs.PlugIn = false;
            reservoirs.Ass = Math.Max(0, reservoirs.Ass - spill);
            MainFile.Logger.Info($"[Seductress] {Id.Entry} pulled the Plug, spilling {spill} ml");
        }
        if (bonus > 0) MainFile.Logger.Info($"[Seductress] {Id.Entry} Ass standing bonus +{bonus} dmg/hit");
        return bonus;
    }

    /// <summary>
    /// A spender card asks Spend or Keep. Spend loses <paramref name="leaks"/> floor-leaks' worth
    /// of the bank for +1 damage per 10 ml on every hit, on top of the standing bonus; Keep plays
    /// the card with the standing bonus alone.
    /// </summary>
    protected async System.Threading.Tasks.Task<int> AssSpendOrKeep(
        MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext choiceContext, int leaks)
    {
        var reservoirs = Reservoirs.Of(Owner.RunState);
        var standing = AssStanding(reservoirs.Ass);

        var notional = Math.Min(reservoirs.Ass, leaks * Systems.RunModel.AssLeakOf(reservoirs.Ass));
        var (lost, extra) = AssSpendQuote(leaks);
        var spendBonus = standing + extra;

        // Nothing to gain by spending: do not ask.
        if (spendBonus <= standing || CombatState is null) return StandingAssBonus();

        var spend = CombatState.CreateCard(ModelDb.Card<AssSpendOption>(), Owner);
        spend.DynamicVars["Ml"].BaseValue = lost;
        spend.DynamicVars["Bonus"].BaseValue = spendBonus;
        var keep = CombatState.CreateCard(ModelDb.Card<AssKeepOption>(), Owner);
        keep.DynamicVars["Bonus"].BaseValue = standing;

        var picked = await MegaCrit.Sts2.Core.Commands.CardSelectCmd.FromChooseACardScreen(
            choiceContext, [spend, keep], Owner);
        return ReferenceEquals(picked, spend) ? standing + SpendAssForDamage(notional) : StandingAssBonus();
    }

    /// <summary>
    /// [Gorim, 2026-11-17] A <i>Continuing</i> attack routed to Ass takes the Ass rule's cum cost
    /// (and so its bonus), but never an HP cost: "Continuing ass shouldn't have an HP cost, but it
    /// should still have the cum cost."
    ///
    /// [Claude] The preview half. Zero for any other route, so a Continuing card's face only
    /// moves once an Ass attack has set the mode.
    /// </summary>
    protected decimal ContinuingAssNotional() =>
        IsContinuing && Owner is not null && Route == Hole.Ass ? AssNotional() : 0m;

    /// <summary>The spending half of <see cref="ContinuingAssNotional"/>. Returns the per-hit bonus.</summary>
    protected int SpendContinuingAss() =>
        IsContinuing && Route == Hole.Ass ? StandingAssBonus() : 0;

    /// <summary>
    /// The tier-2 damage swap, expressed as a delta over the printed base, so the three tiered
    /// Pussy attacks show one live top-line number rather than making the player work out which of
    /// two printed damages is currently in force.
    ///
    /// [Claude-Code] Ritual Mark used to be added here too. It now rides the vanilla damage hook
    /// instead, so it previews on its own and adding it here as well would double it.
    /// </summary>
    protected decimal TieredPussyBonus()
    {
        var bonus = 0m;
        if (Sigils.At(Owner, Sigils.Tier2))
        {
            bonus += DynamicVars["TierDamage"].BaseValue - DynamicVars.CalculationBase.BaseValue;
        }
        return bonus;
    }

    /// <summary>
    /// Extra damage beyond the printed base that comes from run state the preview pipeline cannot
    /// see by itself — the Ass bank, Ritual Mark, the tier-2 swap.
    ///
    /// [Gorim 2026-09-07, found in play] Ass and Pussy cards did not show these on the card face.
    /// <c>DamageVar</c> only previews hook-driven modifiers — Strength, Vulnerable and friends —
    /// so anything computed inside <c>OnPlay</c> is invisible until the card resolves. That is
    /// fatal for the Ass rule in particular, which is a decision mechanic: you are meant to look
    /// at the bank and choose, and you cannot choose against a number you cannot see.
    ///
    /// Routing it through <c>CalculatedDamageVar</c> — the machinery Body Slam and Perfected
    /// Strike use — puts the number on the card, and Strength still layers on top, because
    /// CalculatedDamageVar runs Hook.ModifyDamage over the calculated total.
    /// </summary>
    protected virtual decimal PreviewBonus() => 0m;

    /// <summary>
    /// The multiplier handed to every CalculatedDamageVar here. <c>CalculatedVar</c> rejects a
    /// calc whose Target is an AbstractModel, so it has to stay static and reach the card through
    /// its argument rather than closing over it.
    /// </summary>
    protected static readonly Func<CardModel, Creature?, decimal> BonusCalc =
        static (card, _) => card is SeductressCard s ? s.PreviewBonus() : 0m;

    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
