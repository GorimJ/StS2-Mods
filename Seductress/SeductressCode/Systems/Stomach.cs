using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using Seductress.SeductressCode.Powers;

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
    /// <summary>ml of Stomach per point of Max HP. [Gorim, 2026-11-20] x5 with every other ml figure.</summary>
    public const int MlPerMaxHp = 50;

    /// <summary>Max HP the Stomach has earned in total: 1 per 50 ml.</summary>
    public static int MaxHpEarned(int stomach) => stomach / MlPerMaxHp;

    /// <summary>
    /// Precum granted at the start of each combat: 5 ml per <b>x50</b> milestone passed.
    ///
    /// [Gorim] The milestones are the ones <i>ending</i> in 50 — 50, 150, 250, 350 — not every
    /// fifty and not the x00s, which carry the unique rewards instead.
    ///
    /// [Gorim, 2026-11-20] x5: the milestones are now 250, 750, 1250, 1750 and each grants 25 ml.
    /// </summary>
    public static int CombatStartPrecum(int stomach) => stomach < 250 ? 0 : (stomach + 250) / 500 * 25;

    /// <summary>
    /// 500 ml — <b>Gulp</b>: the Mouth <b>also</b> swallows whenever it is full (250 ml).
    /// (Figures in the notes below predate the 2026-11-20 x5 scale.)
    ///
    /// [Gorim 2026-09-08] The name belongs to this milestone. There is no <i>Gulp</i> card — one
    /// was built from cards doc §6 and has been cut, because the same idea does not want to exist
    /// twice. The cut card's <b>Sensitive removal</b> came here with the name, on the same 25 ml
    /// gate register 39 gave it.
    ///
    /// [Gorim] Possibly a touch strong as one rung. If so the lever is moving it to a higher
    /// milestone rather than weakening it.
    ///
    /// Additive, not a trade — the end-of-combat swallow is unchanged. It uncaps throughput, so a
    /// long fight can bank 100-150 ml where the Mouth's 50 ml capacity previously capped it.
    /// </summary>
    public static bool SwallowsAtFifty(int stomach) => stomach >= 500;

    /// <summary>1000 ml: Indulge stops spilling — 100% reclamation instead of two thirds.</summary>
    public static bool IndulgeIsClean(int stomach) => stomach >= 1000;

    /// <summary>1500 ml: precum counts as cum, and banks instead of evaporating.</summary>
    public static bool PrecumBanks(int stomach) => stomach >= 1500;

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
    /// Default heal rate on a swallow. [Gorim, 2026-11-20] 1 HP per 10 ml (was 1 per 2).
    /// </summary>
    public const int DefaultHealPercent = 10;

    /// <summary>
    /// A swallow this large removes a Sensitive, once the Gulp milestone is reached. Half a full
    /// Mouth: unreachable while the cap was 100 ml, live again at 250 (2026-11-23).
    /// </summary>
    public const int SensitiveThreshold = 125;

    /// <summary>
    /// A swallow (design §6.5). It <b>heals</b>, cum <b>banks</b>, and precum evaporates unless the
    /// 300 ml milestone has turned it into cum.
    ///
    /// [Gorim] Banking and healing are the <b>default</b> behaviour of a swallow, not something a
    /// card enables.
    ///
    /// [Gorim 2026-09-08] <b>Swallowing heals 1 HP per 2 ml, on cum <i>and</i> precum.</b> I had
    /// built the default swallow to heal nothing, reading design §6.5 and register 110 — both of
    /// which list "the heal" among the things <i>Gulp</i> buys. Register entry <b>33</b> says the
    /// opposite: the Mouth "cashes out at end of combat — heal half the ml, bank the full amount".
    /// Both entries are marked locked. This ruling settles it in favour of 33.
    ///
    /// [Claude-Code] It heals on the <b>total</b>, so precum — which banks nothing and evaporates —
    /// still pays out in HP on its way out. That is the thing that makes precum generation worth a
    /// card slot on its own rather than only as fuel for a spender.
    ///
    /// [Claude-Code] Rounds down, matching Indulge's reclamation and the per-floor leak. At an odd
    /// ml you lose the half point.
    /// </summary>
    /// <param name="healPercent">
    /// Share of the swallowed ml healed. 50 everywhere at present; the parameter is kept because
    /// the 200 ml Indulge milestone shows the design does raise reclamation rates over the run,
    /// and a Stomach rung that improved the swallow rate would land here.
    /// </param>
    public static async Task SwallowAll(Player player, string reason, int healPercent = DefaultHealPercent)
    {
        var r = Reservoirs.Of(player.RunState);
        var banksPrecum = PrecumBanks(r.Stomach);
        var (cum, precum) = r.EmptyMouth();

        var total = cum + precum;
        var banked = cum + (banksPrecum ? precum : 0);
        var lost = banksPrecum ? 0 : precum;

        if (total <= 0) return;

        var heal = total * healPercent / 100;
        if (heal > 0)
        {
            await CreatureCmd.Heal(player.Creature, heal);
        }

        // [Gorim 2026-09-08] Sensitive removal moved here when the Gulp card was cut. Register 39
        // put it "on Gulp, gated behind swallowing 25 ml" — Gulp is now the 100 ml Stomach
        // milestone rather than a card, and the 25 ml gate carries over unchanged.
        //
        // [Claude-Code] It only ever bites on a mid-combat GULP swallow: Sensitive clears at the
        // bell anyway, so removing one during the end-of-combat swallow changes nothing. That is
        // the right shape — it makes the milestone's extra swallow the thing that does the work.
        var removedSensitive = false;
        if (SwallowsAtFifty(r.Stomach) && total > SensitiveThreshold &&
            player.Creature.GetPower<Powers.SensitivePower>() is { } sensitive)
        {
            await PowerCmd.ModifyAmount(
                new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                sensitive, -1m, player.Creature, null);
            removedSensitive = true;
        }

        MainFile.Logger.Info(
            $"[Seductress] {reason}: swallowed {total} ml ({cum} cum + {precum} precum) " +
            $"-> healed {heal} ({healPercent}%)" +
            (removedSensitive ? ", -1 Sensitive (Gulp)" : "") +
            (lost > 0 ? $", {lost} ml precum evaporated (banks nothing)" : ""));

        await Bank(player, banked);
    }

    /// <summary>
    /// <b>Gulp</b> — the 500 ml Stomach milestone's extra swallow. Fires whenever the Mouth is
    /// full (<see cref="ReservoirState.MouthCap"/>, 250 ml).
    /// </summary>
    /// <summary>
    /// [Gorim, 2026-11-17] <b>Eat N</b>: take N ml from the reservoirs in Spend order, swallow it.
    /// It heals and goes to the Stomach.
    ///
    /// [Claude] Three readings Gorim accepted with "Yes, build Eat": it heals at the ordinary
    /// swallow rate (1 HP per 2 ml); precum can be eaten and heals but banks nothing, as at the
    /// bell; and with less than N held it eats what there is. Returns the ml actually eaten.
    /// </summary>
    public static async Task<int> Eat(Player player, int ml, string reason)
    {
        if (ml <= 0) return 0;
        var (cum, precum) = Reservoirs.Of(player.RunState).TakeForEat(ml);
        await Digest(player, cum, precum, reason);
        return cum + precum;
    }

    /// <summary>Heals for, and banks, ml that has already left a reservoir (or never entered one).</summary>
    public static async Task Digest(Player player, int cum, int precum, string reason)
    {
        var total = cum + precum;
        if (total <= 0) return;

        var r = Reservoirs.Of(player.RunState);
        var heal = total * DefaultHealPercent / 100;
        if (heal > 0) await CreatureCmd.Heal(player.Creature, heal);

        MainFile.Logger.Info(
            $"[Seductress] {reason}: ate {total} ml ({cum} cum + {precum} precum) -> healed {heal}");
        await Bank(player, cum + (PrecumBanks(r.Stomach) ? precum : 0));
    }

    /// <summary>
    /// Call after any card or power adds <paramref name="ml"/> precum to the Mouth.
    ///
    /// [Gorim, 2026-11-21] <i>Milking Throat</i>: "still needs the bonus precum. Probably gain
    /// twice as much, then gulp."
    ///
    /// [Claude] The second helping is swallowed directly and never enters the Mouth, so the cap
    /// cannot waste it; it is the amount the card asked for, even when the Mouth had no room for
    /// all of the first helping.
    /// </summary>
    public static async Task AfterPrecumGained(Player player, int ml)
    {
        if (player.Creature.GetPower<MilkingThroatPower>() is not { } throat) return;
        // Doubles once per copy: the extra on top of what was gained is ml x (2^copies - 1).
        var extra = ml * ((1 << System.Math.Min(10, (int)throat.Amount)) - 1);
        await Digest(player, 0, extra, "Milking Throat (second helping)");
        await SwallowAll(player, "GULP (Milking Throat)");
    }

    /// <summary>
    /// Call after adding to the Mouth: <paramref name="asked"/> is what the source gave and
    /// <paramref name="fitted"/> what went in. With the Gulp milestone, a full Mouth swallows and
    /// the rest then goes in.
    ///
    /// [Gorim, 2026-11-21] "If you're on 99 and gain 10, it should be gulp 100 gain 9." Asked
    /// whether that should be always on, he kept it behind the 500 ml Stomach milestone; without
    /// the milestone the part that does not fit is still lost.
    /// </summary>
    public static async Task SwallowIfFull(Player player, int asked = 0, int fitted = 0, bool cum = true)
    {
        var r = Reservoirs.Of(player.RunState);
        var left = asked - fitted;

        if (SwallowsAtFifty(r.Stomach))
        {
            while (true)
            {
                if (r.MouthTotal >= ReservoirState.MouthCap) await SwallowAll(player, "GULP (mouth full)");
                if (left <= 0) break;
                var went = cum ? r.AddMouthCum(left) : r.AddPrecum(left);
                if (went <= 0) break;
                left -= went;
                MainFile.Logger.Info($"[Seductress] after the Gulp: +{went} ml {(cum ? "cum" : "precum")} | mouth {r.Mouth}+{r.Precum}p");
            }
        }

        // [Gorim, 2026-11-23] "If you gain cum and your mouth is full, it should splash onto your
        // skin." Cum only; precum that does not fit is still lost.
        if (cum && left > 0)
        {
            r.Skin += left;
            MainFile.Logger.Info($"[Seductress] mouth full: {left} ml cum splashes onto Skin (Skin {r.Skin})");
        }
    }
}
