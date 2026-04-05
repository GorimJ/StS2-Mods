using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using BuxomModPort.Code.Character;
using BuxomModPort.Code.Powers;

namespace BuxomModPort.Code.Relics;

[Pool(typeof(BuxomRelicPool))]
public class BuxomStarterRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner)
        {
            Flash();
            await PowerCmd.Apply<CapacityPower>(Owner.Creature, 2m, Owner.Creature, null, false);
        }
    }
}
