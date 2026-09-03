using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;

namespace RelicChoice.Relics;

/// <summary>
/// Works out which relic a ticket resolves to for a given player/character.
/// Tickets pick the Nth relic (sorted by id) of a rarity from the character's own relic pool, which works for
/// modded characters too because BaseLib registers [Pool]-tagged relics into the character's pool. If the pool
/// is too small (or every candidate is already owned) it wraps around, then falls back to the shared pool.
/// </summary>
public static class TicketResolver
{
    public static RelicModel? Resolve(Player? player, RelicRarity rarity, int index)
    {
        CharacterModel? character = player?.Character;
        if (character == null) return null;

        HashSet<ModelId> owned = player!.Relics.Select(r => r.Id).ToHashSet();

        var candidates = character.RelicPool.AllRelics
            .Where(r => r.Rarity == rarity)
            .OrderBy(r => r.Id.ToString())
            .ToList();
        var unowned = candidates.Where(r => !owned.Contains(r.Id)).ToList();
        if (unowned.Count > 0) return unowned[index % unowned.Count];

        // Character pool exhausted (or empty, e.g. a placeholder modded character): shared pool of that rarity.
        var shared = ModelDb.RelicPool<SharedRelicPool>().AllRelics
            .Where(r => r.Rarity == rarity && !owned.Contains(r.Id))
            .OrderBy(r => r.Id.ToString())
            .ToList();
        if (shared.Count > 0) return shared[index % shared.Count];

        return null;
    }

    /// <summary>The relic's owner if it has one, otherwise the local player of the current run (for previews).</summary>
    public static Player? PlayerFor(RelicModel relic)
    {
        try
        {
            if (relic.IsMutable && relic.Owner != null) return relic.Owner;
        }
        catch (Exception) { /* canonical model, fall through */ }

        try
        {
            RunState? state = Traverse.Create(RunManager.Instance).Property<RunState?>("State").Value;
            if (state != null) return LocalContext.GetMe(state.Players);
        }
        catch (Exception) { }
        return null;
    }
}

/// <summary>Dynamic var that renders as the title of the relic this ticket would give the current player.</summary>
public class TicketRelicVar : StringVar
{
    private readonly RelicRarity _rarity;
    private readonly int _index;

    public TicketRelicVar(RelicRarity rarity, int index) : base("RelicName")
    {
        _rarity = rarity;
        _index = index;
    }

    public override string ToString()
    {
        try
        {
            if (_owner is RelicModel relic)
            {
                RelicModel? target = TicketResolver.Resolve(TicketResolver.PlayerFor(relic), _rarity, _index);
                if (target != null) return target.Title.GetFormattedText();
            }
        }
        catch (Exception ex)
        {
            MainFile.Logger.Warn($"TicketRelicVar failed to resolve: {ex.Message}");
        }
        return $"a {_rarity.ToString().ToLowerInvariant()} relic from your character's pool";
    }
}
