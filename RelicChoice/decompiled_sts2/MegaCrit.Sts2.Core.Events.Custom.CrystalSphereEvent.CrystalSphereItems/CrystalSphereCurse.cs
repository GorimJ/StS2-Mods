using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Events.Custom.CrystalSphereEvent.CrystalSphereItems;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class CrystalSphereCurse : CrystalSphereItem
{
	public override Vector2I Size => new Vector2I(2, 2);

	public override bool IsGood => false;

	public override async Task RevealItem(Player owner)
	{
		await base.RevealItem(owner);
		await CardPileCmd.AddCurseToDeck<Doubt>(owner);
	}
}
