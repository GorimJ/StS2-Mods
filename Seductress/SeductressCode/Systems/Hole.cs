using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// The hole an attack uses. A tag means "finishing with this attack fills this hole"
/// (design §6.0). Skills are never tagged.
/// </summary>
public enum Hole
{
    /// Untagged: cards that are not attacks, and attacks that expel rather than receive.
    None = 0,
    /// Freehand — hands, breasts, thighs. Collects nothing; routes to the external Glazed pool.
    Glazed,
    Mouth,
    Ass,
    Pussy
}

/// <summary>
/// Spike question 3: can a mod define its own card tags?
///
/// <see cref="CardTag"/> is a plain enum, so it cannot be extended at runtime — but it is also
/// documented as "behaviour-less metadata that other models can use to look things up", nothing
/// in the game switches over it or enumerates its members, and <see cref="CardModel.Tags"/> is a
/// <c>HashSet&lt;CardTag&gt;</c>. So mod-defined members are just unused integers cast into the
/// enum: they store, compare and hash exactly like vanilla ones, and vanilla lookups
/// (Perfected Strike, Pandora's Box) never match them because they ask for their own values.
///
/// Numbers start at 1000 to leave MegaCrit room to add vanilla tags without colliding.
/// </summary>
public static class SeductressTags
{
    public const CardTag Glazed = (CardTag)1001;
    public const CardTag Mouth = (CardTag)1002;
    public const CardTag Ass = (CardTag)1003;
    public const CardTag Pussy = (CardTag)1004;

    /// <summary>Marks the Deep Throat card family (design: a tag, not a card).</summary>
    public const CardTag DeepThroat = (CardTag)1010;

    public static CardTag ToTag(this Hole hole) => hole switch
    {
        Hole.Glazed => Glazed,
        Hole.Mouth => Mouth,
        Hole.Ass => Ass,
        Hole.Pussy => Pussy,
        _ => CardTag.None
    };

    public static Hole ToHole(this CardTag tag) => tag switch
    {
        Glazed => Hole.Glazed,
        Mouth => Hole.Mouth,
        Ass => Hole.Ass,
        Pussy => Hole.Pussy,
        _ => Hole.None
    };

    /// <summary>The hole this card is tagged with, read back off the vanilla Tags collection.</summary>
    public static Hole TaggedHole(this CardModel card)
    {
        foreach (var tag in card.Tags)
        {
            var hole = tag.ToHole();
            if (hole != Hole.None) return hole;
        }
        return Hole.None;
    }
}
