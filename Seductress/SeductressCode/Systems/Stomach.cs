using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// Draining's permanent track (design §6.5). The Mouth banks its cum here at the end of every
/// combat, and the total buys Max HP plus four milestone effects.
///
/// [Claude-Cowork] <b>+1 Max HP per 10 ml.</b> Because Max HP is worth ~4x on this character —
/// the bar is refilled four times — 20-25 ml an act reads as +80-100 effective HP, which is
/// deliberately enormous: it is the archetype's entire progression.
///
/// [Claude-Code] Milestones are computed from the total rather than fired as one-shot events, so
/// they cannot desync from the banked figure. The only stateful part is Max HP, which has to be
/// granted incrementally because it is a real stat on the creature.
/// </summary>
public static class Stomach
{
    /// <summary>Max HP the Stomach has earned in total: 1 per 10 ml.</summary>
    public static int MaxHpEarned(int stomach) => stomach / 10;

    /// <summary>
    /// Precum granted at the start of each combat: 5 ml per <b>x50</b> milestone passed.
    ///
    /// [Gorim] The milestones are the ones <i>ending</i> in 50 — 50, 150, 250, 350 — not every
    /// fifty and not the x00s, which carry the unique rewards instead.
    /// </summary>
    public static int CombatStartPrecum(int stomach) => stomach < 50 ? 0 : (stomach + 50) / 100 * 5;

    /// <summary>
    /// 100 ml: the Mouth <b>also</b> swallows whenever it reaches 50 ml.
    ///
    /// Additive, not a trade — the end-of-combat swallow is unchanged. It uncaps throughput, so a
    /// long fight can bank 100-150 ml where the Mouth's 50 ml capacity previously capped it.
    /// </summary>
    public static bool SwallowsAtFifty(int stomach) => stomach >= 100;

    /// <summary>200 ml: Indulge stops spilling — 100% reclamation instead of two thirds.</summary>
    public static bool IndulgeIsClean(int stomach) => stomach >= 200;

    /// <summary>300 ml: precum counts as cum, and banks instead of evaporating.</summary>
    public static bool PrecumBanks(int stomach) => stomach >= 300;

    /// <summary>
    /// Banks ml into the Stomach and pays out any Max HP it just earned.
    ///
    /// Max HP is granted as the difference between what the total has earned and what it had
    /// earned before, so a single bank crossing several tens pays all of them and nothing is
    /// double-counted if this is ever called twice.
    /// </summary>
    public static async Task Bank(Player player, int ml)
    {
        if (ml <= 0) return;

        var r = Reservoirs.Of(player.RunState);
        var before = MaxHpEarned(r.Stomach);
        r.Stomach += ml;
        var after = MaxHpEarned(r.Stomach);

        var gained = after - before;
        if (gained > 0)
        {
            // Deliberately not CreatureCmd.GainMaxHp: that path is patched to halve external Max HP
            // (see MaxHpHalvingPatch), and this is the character's own progression track, not a relic.
            await MaxHpPatches.GainMaxHpUnhalved(player.Creature, gained);
        }

        MainFile.Logger.Info(
            $"[Seductress] Swallowed {ml} ml -> Stomach {r.Stomach}" +
            (gained > 0 ? $" (+{gained} Max HP, now {player.Creature.MaxHp})" : "") +
            $" | precum/combat {CombatStartPrecum(r.Stomach)}" +
            $" | 50ml-swallow {(SwallowsAtFifty(r.Stomach) ? "on" : "off")}" +
            $" | indulge {(IndulgeIsClean(r.Stomach) ? "100%" : "67%")}" +
            $" | precum banks {(PrecumBanks(r.Stomach) ? "yes" : "no")}");
    }

    /// <summary>
    /// The end-of-combat swallow (design §6.5). Cum banks; precum evaporates unless the 300 ml
    /// milestone has turned it into cum.
    ///
    /// [Gorim] This is the <b>default</b>, not something <i>Gulp</i> enables. Gulp buys the
    /// timing, the heal and the Sensitive removal on top of a bank that was always going to happen.
    /// </summary>
    public static async Task SwallowAll(Player player, string reason)
    {
        var r = Reservoirs.Of(player.RunState);
        var banksPrecum = PrecumBanks(r.Stomach);
        var (cum, precum) = r.EmptyMouth();

        var banked = cum + (banksPrecum ? precum : 0);
        var lost = banksPrecum ? 0 : precum;

        if (banked <= 0 && lost <= 0) return;

        if (lost > 0)
        {
            MainFile.Logger.Info($"[Seductress] {reason}: {lost} ml precum evaporated (banks nothing)");
        }
        await Bank(player, banked);
    }

    /// <summary>
    /// The 100 ml milestone's extra swallow. Fires whenever the Mouth is at or above 50 ml, which
    /// with the 50 ml cap means exactly when it is full.
    /// </summary>
    public static async Task SwallowIfFull(Player player)
    {
        var r = Reservoirs.Of(player.RunState);
        if (!SwallowsAtFifty(r.Stomach)) return;
        if (r.MouthTotal < ReservoirState.MouthCap) return;
        await SwallowAll(player, "Mouth full");
    }
}
