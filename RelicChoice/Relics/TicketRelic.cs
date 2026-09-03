using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace RelicChoice.Relics;

/// <summary>
/// "Rainbow" relics that live in the shared pool and turn into a relic from the holder's own character pool.
/// Description and hover tip resolve to the exact relic the current player would receive.
/// </summary>
public abstract class TicketRelic : CustomRelicModel
{
    /// <summary>Which relic of the rarity (sorted by id) this ticket grants.</summary>
    protected abstract int Index { get; }

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new TicketRelicVar(Rarity, Index) };

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            RelicModel? target = TicketResolver.Resolve(TicketResolver.PlayerFor(this), Rarity, Index);
            return target != null ? new List<IHoverTip> { target.HoverTip } : new List<IHoverTip>();
        }
    }

    protected TicketRelic() : base(RelicChoiceConfig.Instance.EnableRainbowRelics) { }

    public override async Task AfterObtained()
    {
        RelicModel? target = TicketResolver.Resolve(Owner, Rarity, Index);
        if (target == null)
        {
            MainFile.Logger.Warn($"{Id}: no {Rarity} relic available for {Owner.Character.Id}; ticket does nothing.");
            return;
        }
        await RelicCmd.Obtain(target.ToMutable(), Owner);
    }
}
