using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.HoverTips;
using BaseLib.Utils;

namespace RelicChoice.Relics;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class PremiumGoldenTicket : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var tips = new List<IHoverTip>
            {
                ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.RuinedHelmet>().HoverTip,
                ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.ToughBandages>().HoverTip,
                ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.IvoryTile>().HoverTip,
                ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.OrangeDough>().HoverTip,
                ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.Metronome>().HoverTip
            };
            return tips;
        }
    }

    public PremiumGoldenTicket() : base(true) { }

    public override async Task AfterObtained()
    {
        var pool = this.Owner.Character.RelicPool.AllRelics.Where(r => r.Rarity == this.Rarity).OrderBy(r => r.Id).ToList();
        if (pool.Count > 2)
        {
            var chosen = pool[2].ToMutable();
            await RelicCmd.Obtain(chosen, this.Owner);
        }
    }
}
