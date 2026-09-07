using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// Squeezing's progression axis (design §6.1a). Each hole progresses differently: Pussy by
/// pregnancies, Mouth by slow accumulation, <b>Ass by rewards granted on reaching bank
/// thresholds</b>.
///
/// <list type="table">
/// <item><term>100 ml</term><description><i>Milking Grip</i> added to the deck free</description></item>
/// <item><term>200 ml</term><description>the <b>Plug</b> relic</description></item>
/// <item><term>300 ml</term><description>open — a genuine stretch rung, deliberately empty</description></item>
/// </list>
///
/// [Claude-Cowork] <b>Why this is load-bearing rather than polish.</b> The Ass bank has a
/// structural fixed point at 100 ml — the spend is a percentage of the bank while normal intake is
/// a flat 10 ml, so they meet exactly there. The Strength ratio is then tuned to +1 per 4 ml
/// against the 200-270 ml plateau a deck reaches <i>with</i> Milking Grip. Without these unlocks a
/// deck sits at the unaided ceiling where that ratio pays +2, which is the inertness the ratio was
/// changed to fix. Shipping either alone is worse than shipping neither.
///
/// And the threshold and the gate line up exactly: 100 ml is the ceiling, Milking Grip is what
/// lets you pass it, so the system hands you the key at the moment you reach the lock.
///
/// [Gorim] <b>Tracked by high-water mark, not the live value.</b> The bank leaks every floor, so a
/// threshold touched at 105 ml would be lost again by the next rest site and the reward would
/// blink on and off.
/// </summary>
public static class AssUnlocks
{
    public const int MilkingGripAt = 100;
    public const int PlugAt = 200;
    public const int OpenRungAt = 300;

    /// <summary>
    /// Called on entering a floor, after the leak.
    ///
    /// Idempotent by construction: it asks whether the player already holds the reward rather than
    /// keeping a "granted" flag, so a repeat call cannot hand out a second copy and a reward
    /// removed at a shop is not silently re-granted... which is worth knowing, because it will be.
    /// </summary>
    public static async Task Check(Player player)
    {
        var r = Reservoirs.Of(player.RunState);
        r.TouchAssHighWater();

        if (r.AssHighWater >= MilkingGripAt && !HasCard<Cards.MilkingGrip>(player))
        {
            var card = player.RunState.CreateCard(ModelDb.Card<Cards.MilkingGrip>(), player);
            await CardPileCmd.Add(card, PileType.Deck);
            MainFile.Logger.Info(
                $"[Seductress] ASS UNLOCK {MilkingGripAt} ml (high water {r.AssHighWater}): Milking Grip added to the deck");
        }

        if (r.AssHighWater >= PlugAt && !player.Relics.Any(x => x is Relics.Plug))
        {
            await RelicCmd.Obtain(ModelDb.Relic<Relics.Plug>().ToMutable(), player);
            MainFile.Logger.Info(
                $"[Seductress] ASS UNLOCK {PlugAt} ml (high water {r.AssHighWater}): Plug relic granted");
        }

        // 300 ml: rung defined, deliberately empty. [Gorim] Left open as a genuine stretch goal.
    }

    private static bool HasCard<T>(Player player) where T : CardModel =>
        player.Deck.Cards.Any(c => c is T);
}
