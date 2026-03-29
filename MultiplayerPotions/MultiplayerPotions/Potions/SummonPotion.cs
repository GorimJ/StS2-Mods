using Godot;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.PotionPools;

using BaseLib.Patches.Content;

namespace MultiplayerPotions.Potions;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.PotionPools.SharedPotionPool))]
    public class SummonPotion : CustomPotionModel
{
    [CustomEnum("MULTIPLAYER")]
    public static PotionRarity MultiplayerRarity;

    public override PotionRarity Rarity => MultiplayerRarity;
    public override TargetType TargetType => TargetType.Self; 
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    
            [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.PotionPools.SharedPotionPool))]
    public class SummonHpCostVar : DynamicVar
    {
        public SummonHpCostVar() : base("Amount", 0) { }
        private int CurrentValue => (_owner is PotionModel p && p.Owner != null && p.Owner.Creature != null) ? (int)System.Math.Floor(p.Owner.Creature.CurrentHp * 0.15m) : 0;
        public override string ToString() => CurrentValue.ToString();
        protected override decimal GetBaseValueForIConvertible() => CurrentValue;
    }

            [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.PotionPools.SharedPotionPool))]
    public class SummonCountVar : DynamicVar
    {
        public SummonCountVar() : base("Summon", 0) { }
        private int CurrentValue => (_owner is PotionModel p && p.Owner != null && p.Owner.Creature != null) ? (int)System.Math.Floor(p.Owner.Creature.CurrentHp * 0.15m) + 5 : 5;
        public override string ToString() => CurrentValue.ToString();
        protected override decimal GetBaseValueForIConvertible() => CurrentValue;
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar> 
    { 
        new SummonHpCostVar(), 
        new SummonCountVar() 
    };

    public override IEnumerable<IHoverTip> ExtraHoverTips 
    {
        get
        {
            if (base.DynamicVars.TryGetValue("Summon", out var summonVar))
            {
                return new List<IHoverTip> { HoverTipFactory.Static(StaticHoverTip.SummonDynamic, summonVar) };
            }
            return new List<IHoverTip>();
        }
    }

    public SummonPotion() : base()
    {
    }

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        int healthCost = (base.Owner?.Creature != null) ? (int)System.Math.Floor(base.Owner.Creature.CurrentHp * 0.15m) : 0;

        
        if (healthCost > 0)
        {
            await CreatureCmd.Damage(choiceContext, base.Owner.Creature, healthCost, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered, null, null);
        }

        int summonAmount = healthCost + 5;

        if (summonAmount > 0)
        {
            var players = base.Owner.RunState?.Players;
            if (players != null)
            {
                foreach (var player in players)
                {
                    await OstyCmd.Summon(choiceContext, player, summonAmount, this);
                }
            }
            else
            {
                await OstyCmd.Summon(choiceContext, base.Owner, summonAmount, this);
            }
        }
    }
}
