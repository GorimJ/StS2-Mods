using Godot;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Context;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;

using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MultiplayerPotions.Powers;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.PotionPools.SharedPotionPool))]
    public class RagePotionPower : CustomPowerModel
{


    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override string? CustomPackedIconPath => "res://MultiplayerPotions/Powers/rageaurapower.png";
    public override string? CustomBigIconPath => "res://MultiplayerPotions/Powers/rageaurapower.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar> 
    { 
        new BlockVar(1m, MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered) 
    };

    public RagePotionPower() : base()
    {
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner.Creature == base.Owner)
        {
            var players = base.Owner?.Player?.RunState?.Players;
            if (players != null)
            {
                foreach (var player in players)
                {
                    await CreatureCmd.GainBlock(player.Creature, base.Amount, (MegaCrit.Sts2.Core.ValueProps.ValueProp)0, null, false);
                }
            }
            else if (base.Owner != null)
            {
                await CreatureCmd.GainBlock(base.Owner, base.Amount, (MegaCrit.Sts2.Core.ValueProps.ValueProp)0, null, false);
            }
        }
    }
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await PowerCmd.Remove(this);
    }
}
