using MegaCrit.Sts2.Core.Entities.Cards;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// A Lust card — the death clock (design §5). Playable, each gives a real benefit and a large
/// chunk of arousal; the card never kills you directly, it accelerates the loop that does.
///
/// Holding <see cref="Systems.ClimaxModel.LustLimit"/> of these ends the run. Rest-site Relief
/// removes one. Not draftable; only Climax produces them.
///
/// The shared base exists so Relief and the Lust count work on the family rather than naming a
/// specific card, which also means a third Lust card costs nothing to add.
/// </summary>
public abstract class LustCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : SeductressCard(cost, type, rarity, target);
