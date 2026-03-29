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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MultiplayerPotions.Potions;

namespace MultiplayerPotions.Powers;

public sealed class TeamOrbitPower : CustomPowerModel
{
    private class Data
    {
        public int energySpent;
        public int triggerCount;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override int DisplayAmount => 4 - GetInternalData<Data>().energySpent % 4;



    public override bool IsInstanced => true;

    public override string? CustomPackedIconPath => "res://MultiplayerPotions/Powers/TeamOrbit.png";
    public override string? CustomBigIconPath => "res://MultiplayerPotions/Powers/TeamOrbit.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar> { new EnergyVar(4) };

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (card.Owner.Creature == base.Owner && amount > 0)
        {
            Data data = GetInternalData<Data>();
            data.energySpent += amount;
            int triggers = data.energySpent / 4 - data.triggerCount;
            
            if (triggers > 0)
            {
                Flash();
                // Grant energy to every active player in the run
                var players = base.Owner?.Player?.RunState?.Players;
                if (players != null)
                {
                    foreach (var player in players)
                    {
                        if (player.Creature != null && player.Creature.IsAlive)
                        {
                            await PlayerCmd.GainEnergy(base.Amount * triggers, player);
                        }
                    }
                }
                
                data.triggerCount += triggers;
            }
            InvokeDisplayAmountChanged();
        }
    }

    public override async System.Threading.Tasks.Task AfterTurnEnd(MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Combat.CombatSide side)
    {
        if (side == MegaCrit.Sts2.Core.Combat.CombatSide.Player) 
        {
            await MegaCrit.Sts2.Core.Commands.PowerCmd.Remove(this);
        }
    }
}
