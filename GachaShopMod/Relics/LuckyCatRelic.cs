using System;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Relics;
using BaseLib.Utils;

namespace GachaShopMod.Relics;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class LuckyCatRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    public override bool HasUponPickupEffect => false;

    // Custom asset
    public override string PackedIconPath => "res://Images/GachaCat.png";
    protected override string BigIconPath => "res://Images/GachaCat.png";
    protected override string PackedIconOutlinePath => "res://Images/GachaCat.png";

    public LuckyCatRelic() : base(true) { }
}
