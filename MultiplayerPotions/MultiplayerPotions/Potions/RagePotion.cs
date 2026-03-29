using Godot;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.PotionPools;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MultiplayerPotions.Powers;

namespace MultiplayerPotions.Potions;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.PotionPools.SharedPotionPool))]
    public class RagePotion : CustomPotionModel
{
    public override PotionRarity Rarity => SummonPotion.MultiplayerRarity;
    public override TargetType TargetType => TargetType.AnyPlayer;
    public override PotionUsage Usage => PotionUsage.CombatOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar> { new BlockVar(3m, (MegaCrit.Sts2.Core.ValueProps.ValueProp)0) };
    public override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip> { HoverTipFactory.Static(StaticHoverTip.Block, base.DynamicVars.Block) };

    public RagePotion() : base()
    {
    }

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        Creature actualTarget = target ?? base.Owner.Creature;
        await PowerCmd.Apply<RagePotionPower>(base.Owner.Creature, (int)base.DynamicVars.Block.BaseValue, actualTarget, null);
    }
}
