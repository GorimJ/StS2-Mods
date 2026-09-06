using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Seductress.SeductressCode.Character;
using Seductress.SeductressCode.Extensions;
using Seductress.SeductressCode.Systems;
using MegaCrit.Sts2.Core.Entities.Cards;

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

    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
