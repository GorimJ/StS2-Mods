using System.Collections.Generic;
using System.Linq;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

public abstract class CustomCardModel : CardModel, ICustomModel
{
	public override bool GainsBlock => ((IEnumerable<KeyValuePair<string, DynamicVar>>)((CardModel)this).DynamicVars).Any(delegate(KeyValuePair<string, DynamicVar> dynVar)
	{
		DynamicVar value = dynVar.Value;
		return (value is BlockVar || value is CalculatedBlockVar) ? true : false;
	});

	public virtual Texture2D? CustomFrame => null;

	public CustomCardModel(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true, bool autoAdd = true)
		: base(baseCost, type, rarity, target, showInCardLibrary)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		if (autoAdd)
		{
			CustomContentDictionary.AddModel(((object)this).GetType());
		}
	}
}
