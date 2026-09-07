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
    /// <param name="fraction">Share of the bank this card spends. 0.1 for a normal Ass attack.</param>
    protected int SpendAssForDamage(double fraction = 0.10)
    {
        var reservoirs = Reservoirs.Of(Owner.RunState);
        var milking = Owner.Creature.GetPower<Powers.MilkingGripPower>();

        var notional = (int)Math.Ceiling(reservoirs.Ass * fraction);
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
    /// The Ass Strength ratio: <b>+1 per 4 ml lost</b>.
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
    protected static int AssStrength(int mlLost) => mlLost / 4;

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

    /// <summary><i>Ritual Mark</i>: your Pussy cards deal additional damage.</summary>
    protected decimal RitualMarkBonus =>
        Hole == Hole.Pussy
            ? Owner.Creature.GetPower<Powers.RitualMarkPower>()?.PussyBonus(Owner) ?? 0m
            : 0m;

    /// <summary>
    /// The Ass bonus this card <i>would</i> pay right now, without spending it. Preview only —
    /// <see cref="SpendAssForDamage"/> is what actually moves the bank.
    ///
    /// Milking Grip halves the spend but pays the full bonus, so the preview ignores it.
    /// </summary>
    protected decimal AssNotional(double fraction = 0.10)
    {
        var r = Reservoirs.Of(Owner.RunState);
        var notional = (int)Math.Ceiling(r.Ass * fraction);
        var spill = r.PlugIn ? (int)Math.Ceiling(r.Ass * Relics.Plug.SpillFraction) : 0;
        return AssStrength(notional + spill);
    }

    /// <summary>
    /// Ritual Mark, plus the tier-2 damage swap expressed as a delta over the printed base.
    /// The three tiered Pussy attacks show one live top-line number rather than making the player
    /// work out which of two printed damages is currently in force.
    /// </summary>
    protected decimal TieredPussyBonus()
    {
        var bonus = RitualMarkBonus;
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
