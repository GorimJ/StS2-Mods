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
    /// converts it into <b>+1 damage per 10 ml lost, on every hit</b>.
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
        var milking = Owner.Creature.HasPower<Powers.MilkingGripPower>();

        var notional = (int)Math.Ceiling(reservoirs.Ass * fraction);
        var actual = milking ? (int)Math.Ceiling(notional / 2.0) : notional;

        reservoirs.Ass -= actual;
        var bonus = notional / 10;
        if (actual > 0)
        {
            MainFile.Logger.Info(
                $"[Seductress] {Id.Entry} spent {actual} ml Ass (+{bonus} dmg/hit{(milking ? ", Milking Grip" : "")})");
        }
        return bonus;
    }

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

    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
