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
    public class ShinyGoldenTicket : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var tips = new List<IHoverTip>
            {
                ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.DemonTongue>().HoverTip,
                ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.PaperKrane>().HoverTip,
                ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.Bookmark>().HoverTip,
                ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.MiniRegent>().HoverTip,
                ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.PowerCell>().HoverTip
            };
            return tips;
        }
    }

    public ShinyGoldenTicket() : base(true) { }

    public override async Task AfterObtained()
    {
        var pool = this.Owner.Character.RelicPool.AllRelics.Where(r => r.Rarity == this.Rarity).OrderBy(r => r.Id).ToList();
        if (pool.Count > 1)
        {
            var chosen = pool[1].ToMutable();
            await RelicCmd.Obtain(chosen, this.Owner);
        }
    }
}
