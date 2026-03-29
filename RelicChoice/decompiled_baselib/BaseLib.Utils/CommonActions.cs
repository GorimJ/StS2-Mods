using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Utils;

public static class CommonActions
{
	public static AttackCommand CardAttack(CardModel card, CardPlay play, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
	{
		return CardAttack(card, play.Target, hitCount, vfx, sfx, tmpSfx);
	}

	public static AttackCommand CardAttack(CardModel card, Creature? target, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
	{
		decimal baseValue = ((DynamicVar)card.DynamicVars.Damage).BaseValue;
		return CardAttack(card, target, baseValue, hitCount, vfx, sfx, tmpSfx);
	}

	public static AttackCommand CardAttack(CardModel card, Creature? target, decimal damage, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected I4, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		AttackCommand val = DamageCmd.Attack(damage).WithHitCount(hitCount).FromCard(card);
		CombatState combatState = card.CombatState;
		TargetType targetType = card.TargetType;
		switch (targetType - 2)
		{
		case 0:
			if (target == null)
			{
				return val;
			}
			val.Targeting(target);
			break;
		case 1:
			if (combatState == null)
			{
				return val;
			}
			val.TargetingAllOpponents(combatState);
			break;
		case 2:
			if (combatState == null)
			{
				return val;
			}
			val.TargetingRandomOpponents(combatState, true);
			break;
		default:
			throw new Exception($"Unsupported AttackCommand target type {card.TargetType} for card {card.Title}");
		}
		if (vfx != null || sfx != null || tmpSfx != null)
		{
			val.WithHitFx(vfx, sfx, tmpSfx);
		}
		return val;
	}

	public static async Task<decimal> CardBlock(CardModel card, CardPlay play)
	{
		return await CardBlock(card, card.DynamicVars.Block, play);
	}

	public static async Task<decimal> CardBlock(CardModel card, BlockVar blockVar, CardPlay play)
	{
		return await CreatureCmd.GainBlock(card.Owner.Creature, blockVar, play, false);
	}

	public static async Task<IEnumerable<CardModel>> Draw(CardModel card, PlayerChoiceContext context)
	{
		return await CardPileCmd.Draw(context, ((DynamicVar)card.DynamicVars.Cards).BaseValue, card.Owner, false);
	}

	public static async Task<T?> Apply<T>(Creature target, CardModel? card, decimal amount, bool silent = false) where T : PowerModel
	{
		return await PowerCmd.Apply<T>(target, amount, (card != null) ? card.Owner.Creature : null, card, silent);
	}

	public static async Task<T?> ApplySelf<T>(CardModel card, decimal amount, bool silent = false) where T : PowerModel
	{
		return await PowerCmd.Apply<T>(card.Owner.Creature, amount, card.Owner.Creature, card, silent);
	}

	public static async Task<IEnumerable<CardModel>> SelectCards(CardModel card, LocString selectionPrompt, PlayerChoiceContext context, PileType pileType, int count = 1)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CardSelectorPrefs val = default(CardSelectorPrefs);
		((CardSelectorPrefs)(ref val))._002Ector(selectionPrompt, count);
		CardPile pile = PileTypeExtensions.GetPile(pileType, card.Owner);
		return await CardSelectCmd.FromSimpleGrid(context, pile.Cards, card.Owner, val);
	}

	public static async Task<IEnumerable<CardModel>> SelectCards(CardModel card, LocString selectionPrompt, PlayerChoiceContext context, PileType pileType, int minCount, int maxCount)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CardSelectorPrefs val = default(CardSelectorPrefs);
		((CardSelectorPrefs)(ref val))._002Ector(selectionPrompt, minCount, maxCount);
		CardPile pile = PileTypeExtensions.GetPile(pileType, card.Owner);
		return await CardSelectCmd.FromSimpleGrid(context, pile.Cards, card.Owner, val);
	}

	public static async Task<CardModel?> SelectSingleCard(CardModel card, LocString selectionPrompt, PlayerChoiceContext context, PileType pileType)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CardSelectorPrefs val = default(CardSelectorPrefs);
		((CardSelectorPrefs)(ref val))._002Ector(selectionPrompt, 1);
		CardPile pile = PileTypeExtensions.GetPile(pileType, card.Owner);
		return (await CardSelectCmd.FromSimpleGrid(context, pile.Cards, card.Owner, val)).FirstOrDefault();
	}
}
