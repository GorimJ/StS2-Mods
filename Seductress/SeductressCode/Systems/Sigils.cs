using MegaCrit.Sts2.Core.Entities.Players;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// Sigil thresholds and the shared tier ladder (design §7.1).
///
/// [Gorim] Thresholds are 1 / 3 / 5 / 7. The first Sigil must be a spike: at 2 / 4 / 6 the first
/// pregnancy — the achievement the character is built around — changed nothing on any card.
///
/// Every gated card follows the same ladder, so "what does another Sigil get me" is answerable
/// without reading four cards:
///
///   1  adds an effect      3  grows the base
///   5  grows the effect    7  repeats, a number of times equal to its cost
///
/// 0-cost cards shed their non-energy cost at tier 4 instead of repeating.
/// </summary>
public static class Sigils
{
    public const int Tier1 = 1;
    public const int Tier2 = 3;
    public const int Tier3 = 5;
    public const int Tier4 = 7;

    /// <summary>
    /// Sigils available right now — the stored count less any exhausted for this combat by
    /// <i>Consummate</i>.
    /// </summary>
    public static int Effective(Player player) =>
        System.Math.Max(0, Reservoirs.Of(player.RunState).Sigils - ClimaxModel.Instance.SigilsExhaustedThisCombat);

    public static bool At(Player player, int tier) => Effective(player) >= tier;
}
