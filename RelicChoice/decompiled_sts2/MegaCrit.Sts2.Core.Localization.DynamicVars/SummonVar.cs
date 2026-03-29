using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Localization.DynamicVars;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class SummonVar : DynamicVar
{
	public const string defaultName = "Summon";

	public SummonVar(decimal summonAmount)
		: base("Summon", summonAmount)
	{
	}

	public SummonVar(string name, decimal summonAmount)
		: base(name, summonAmount)
	{
	}

	public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
	{
		if (runGlobalHooks)
		{
			base.PreviewValue = Hook.ModifySummonAmount(card.CombatState, card.Owner, base.BaseValue, card);
		}
	}
}
