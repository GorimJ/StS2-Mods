using System;
using System.Collections.Generic;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BaseLib.Extensions;

public static class DynamicVarExtensions
{
	public static readonly SpireField<DynamicVar, Func<IHoverTip>> DynamicVarTips = new SpireField<DynamicVar, Func<IHoverTip>>(() => (Func<IHoverTip>?)null);

	public static decimal CalculateBlock(this DynamicVar var, Creature creature, ValueProp props, CardPlay? cardPlay = null, CardModel? cardSource = null)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		decimal baseValue = var.BaseValue;
		if (!CombatManager.Instance.IsInProgress)
		{
			return baseValue;
		}
		if (CombatManager.Instance.IsEnding)
		{
			return baseValue;
		}
		CombatState combatState = creature.CombatState;
		if (combatState == null)
		{
			return baseValue;
		}
		IEnumerable<AbstractModel> enumerable = default(IEnumerable<AbstractModel>);
		baseValue = Hook.ModifyBlock(combatState, creature, baseValue, props, cardSource, cardPlay, ref enumerable);
		return Math.Max(baseValue, 0m);
	}

	public static DynamicVar WithTooltip(this DynamicVar var, string locTable = "static_hover_tips")
	{
		string key = ((object)var).GetType().GetPrefix() + StringHelper.Slugify(var.Name);
		DynamicVarTips[var] = delegate
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			LocString val = new LocString(locTable, key + ".title");
			LocString val2 = new LocString(locTable, key + ".description");
			val.Add(var);
			val2.Add(var);
			return (IHoverTip)(object)new HoverTip(val, val2, (Texture2D)null);
		};
		return var;
	}
}
