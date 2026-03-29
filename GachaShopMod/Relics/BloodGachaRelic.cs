using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace GachaShopMod.Relics;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class BloodGachaRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    public override string PackedIconPath => "res://Images/BloodForGacha.png";
    protected override string BigIconPath => "res://Images/BloodForGacha.png";
    protected override string PackedIconOutlinePath => "res://Images/BloodForGacha.png";

    public BloodGachaRelic() : base(true) { }
}
