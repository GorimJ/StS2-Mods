using System;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Seductress.SeductressCode.Systems;

/// <summary>
/// [Gorim] <b>Max HP from outside sources is halved for the Seductress.</b> Because the bar is
/// refilled four times, +1 Max HP is worth ~4x what it is on any other character — so Strawberry,
/// Pear and Mango at full strength would outclass every other relic in the game. At 50% they are
/// still strong and still worth taking.
///
/// [Claude-Code] Two notes on how this is implemented, both divergences worth reading.
///
/// <b>It halves every external source, not only relics.</b> There is no hook on Max HP gain and no
/// source parameter on <c>CreatureCmd.GainMaxHp</c>, so a prefix cannot tell a relic from an event
/// or a potion. Halving all of them is the coherent reading — an event handing out Max HP has
/// exactly the same 4x problem a food relic does — but the design says "relics", so this is wider
/// than the letter of it.
///
/// <b>It rounds up.</b> Rounding down would silently zero any +1, turning a small reward into no
/// reward at all, which is a worse failure than a point of rounding in the player's favour.
/// </summary>
[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.GainMaxHp))]
internal static class MaxHpHalvingPatch
{
    /// <summary>
    /// Set while the character's own progression track grants Max HP, so the Stomach is not taxed
    /// by a rule aimed at relics. Not thread-static: the game's command pipeline is single
    /// threaded and awaits in order, and a thread-static would not survive an await hop anyway.
    /// </summary>
    internal static bool Bypass;

    [HarmonyPrefix]
    private static void Halve(Creature creature, ref decimal amount)
    {
        if (Bypass) return;
        if (creature.Player?.Character is not Character.Seductress) return;
        if (amount <= 0m) return;

        var halved = Math.Ceiling(amount / 2m);
        MainFile.Logger.Info($"[Seductress] Max HP gain halved: {amount} -> {halved}");
        amount = halved;
    }
}

public static class MaxHpPatches
{
    /// <summary>Grants Max HP without the halving above. For the Stomach track only.</summary>
    public static async Task GainMaxHpUnhalved(Creature creature, decimal amount)
    {
        MaxHpHalvingPatch.Bypass = true;
        try
        {
            await CreatureCmd.GainMaxHp(creature, amount);
        }
        finally
        {
            MaxHpHalvingPatch.Bypass = false;
        }
    }
}
