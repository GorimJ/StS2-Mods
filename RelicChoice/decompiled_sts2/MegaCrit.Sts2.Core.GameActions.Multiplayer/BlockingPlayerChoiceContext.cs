using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Multiplayer;

namespace MegaCrit.Sts2.Core.GameActions.Multiplayer;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class BlockingPlayerChoiceContext : PlayerChoiceContext
{
	public override Task SignalPlayerChoiceBegun(PlayerChoiceOptions options)
	{
		return Task.CompletedTask;
	}

	public override Task SignalPlayerChoiceEnded()
	{
		return Task.CompletedTask;
	}
}
