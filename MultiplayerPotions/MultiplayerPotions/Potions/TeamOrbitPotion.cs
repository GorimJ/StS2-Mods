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
using MegaCrit.Sts2.Core.Context;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Runs;
using MultiplayerPotions.Powers;

namespace MultiplayerPotions.Potions;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.PotionPools.SharedPotionPool))]
    public class TeamOrbitPotion : CustomPotionModel
{
    public override PotionRarity Rarity => SummonPotion.MultiplayerRarity;
    public override TargetType TargetType => TargetType.AnyPlayer; 
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar> 
    { 
        new BlockVar(1m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered) // Value isn't used right now, but needed for compilation
    };

    public TeamOrbitPotion() : base()
    {
    }

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target != null)
        {
            await PowerCmd.Apply<TeamOrbitPower>(base.Owner.Creature, 1, target, null);
        }
    }
}
