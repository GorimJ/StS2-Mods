using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils.Patching;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace BaseLib.Patches.Content;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class TheBigPatchToCardPileCmdAdd
{
	private static Type? stateMachineType;

	public static void Patch(Harmony harmony)
	{
		harmony.PatchAsyncMoveNext(AccessTools.Method(typeof(CardPileCmd), "Add", new Type[5]
		{
			typeof(IEnumerable<CardModel>),
			typeof(CardPile),
			typeof(CardPilePosition),
			typeof(AbstractModel),
			typeof(bool)
		}, (Type[])null), out stateMachineType, null, null, HarmonyMethod.op_Implicit(AccessTools.Method(typeof(TheBigPatchToCardPileCmdAdd), "BigPatch", (Type[])null, (Type[])null)));
	}

	private static List<CodeInstruction> BigPatch(IEnumerable<CodeInstruction> instructions)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Expected O, but got Unknown
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Expected O, but got Unknown
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected O, but got Unknown
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Expected O, but got Unknown
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Expected O, but got Unknown
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Expected O, but got Unknown
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Expected O, but got Unknown
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Expected O, but got Unknown
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Expected O, but got Unknown
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Expected O, but got Unknown
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Expected O, but got Unknown
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Expected O, but got Unknown
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Expected O, but got Unknown
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Expected O, but got Unknown
		if (stateMachineType == null)
		{
			throw new Exception("Failed to get state machine type for async CardPileCmd.Add");
		}
		Label label;
		List<Label> labels;
		Label label2;
		Label label3;
		object operand;
		object operand2;
		Label label4;
		Label label5;
		return new InstructionPatcher(instructions).Match(new InstructionMatcher().ldfld(stateMachineType.FindStateMachineField("isFullHandAdd")).brtrue_s().ldarg_0()
			.ldfld(stateMachineType.FindStateMachineField("oldPile"))
			.brtrue_s()).Step(-1).GetOperandLabel(out label)
			.Step()
			.Insert((IEnumerable<CodeInstruction>)new _003C_003Ez__ReadOnlyArray<CodeInstruction>((CodeInstruction[])(object)new CodeInstruction[6]
			{
				CodeInstruction.LoadArgument(0, false),
				new CodeInstruction(OpCodes.Ldfld, (object)stateMachineType.FindStateMachineField("targetPile")),
				CodeInstruction.LoadArgument(0, false),
				new CodeInstruction(OpCodes.Ldfld, (object)stateMachineType.FindStateMachineField("card")),
				new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(TheBigPatchToCardPileCmdAdd), "IsPileCustomPileWhereCardShouldBeVisible", (Type[])null, (Type[])null)),
				new CodeInstruction(OpCodes.Brtrue_S, (object)label)
			}))
			.Match(new InstructionMatcher().stloc_s(24).ldloc_s(24).ldc_i4_1()
				.sub()
				.switch_()
				.br_s()
				.ldc_i4_1())
			.Step(-1)
			.GetLabels(out labels)
			.Step(-1)
			.Insert((IEnumerable<CodeInstruction>)new _003C_003Ez__ReadOnlyArray<CodeInstruction>((CodeInstruction[])(object)new CodeInstruction[6]
			{
				CodeInstruction.LoadArgument(0, false),
				new CodeInstruction(OpCodes.Ldfld, (object)stateMachineType.FindStateMachineField("oldPile")),
				CodeInstruction.LoadArgument(0, false),
				new CodeInstruction(OpCodes.Ldfld, (object)stateMachineType.FindStateMachineField("card")),
				new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(TheBigPatchToCardPileCmdAdd), "IsPileCustomPileWithCardNotVisible", (Type[])null, (Type[])null)),
				new CodeInstruction(OpCodes.Brtrue_S, (object)labels[0])
			}))
			.Match(new InstructionMatcher().stloc_s(24).ldloc_s(24).ldc_i4_1()
				.beq_s())
			.Step(-1)
			.GetOperandLabel(out label2)
			.Step()
			.Insert((IEnumerable<CodeInstruction>)new _003C_003Ez__ReadOnlyArray<CodeInstruction>((CodeInstruction[])(object)new CodeInstruction[6]
			{
				CodeInstruction.LoadArgument(0, false),
				new CodeInstruction(OpCodes.Ldfld, (object)stateMachineType.FindStateMachineField("targetPile")),
				CodeInstruction.LoadArgument(0, false),
				new CodeInstruction(OpCodes.Ldfld, (object)stateMachineType.FindStateMachineField("card")),
				new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(TheBigPatchToCardPileCmdAdd), "CustomPileWithoutCustomTransition", (Type[])null, (Type[])null)),
				new CodeInstruction(OpCodes.Brtrue_S, (object)label2)
			}))
			.Match(new InstructionMatcher().ldarg_0().ldfld(AccessTools.Field(stateMachineType, "newPile")).callvirt(AccessTools.PropertyGetter(typeof(CardPile), "Type"))
				.ldc_i4_2()
				.beq_s())
			.Step(-1)
			.GetOperandLabel(out label3)
			.Step()
			.Insert((IEnumerable<CodeInstruction>)new _003C_003Ez__ReadOnlyArray<CodeInstruction>((CodeInstruction[])(object)new CodeInstruction[6]
			{
				CodeInstruction.LoadArgument(0, false),
				new CodeInstruction(OpCodes.Ldfld, (object)AccessTools.Field(stateMachineType, "newPile")),
				CodeInstruction.LoadArgument(0, false),
				new CodeInstruction(OpCodes.Ldfld, (object)stateMachineType.FindStateMachineField("card")),
				new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(TheBigPatchToCardPileCmdAdd), "IsPileCustomPileWhereCardShouldBeVisible", (Type[])null, (Type[])null)),
				new CodeInstruction(OpCodes.Brtrue_S, (object)label3)
			}))
			.Match(new InstructionMatcher().ldloc_s(35).ldloc_s(35).ldfld(null)
				.ldfld(null))
			.Step(-1)
			.GetOperand(out operand)
			.Step(-1)
			.GetOperand(out operand2)
			.Match(new InstructionMatcher().ldloc_s(35).ldfld(null).callvirt(AccessTools.PropertyGetter(typeof(CardModel), "Pile"))
				.callvirt(AccessTools.PropertyGetter(typeof(CardPile), "Type"))
				.stloc_s(24)
				.ldloc_s(24)
				.ldc_i4_1()
				.sub()
				.ldc_i4_2()
				.ble_un_s())
			.Step(-1)
			.GetOperandLabel(out label4)
			.Step()
			.InsertCopy(-10, 2)
			.Insert((IEnumerable<CodeInstruction>)new _003C_003Ez__ReadOnlyArray<CodeInstruction>((CodeInstruction[])(object)new CodeInstruction[2]
			{
				new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(TheBigPatchToCardPileCmdAdd), "CustomPileUseGenericTweenForOtherPlayers", (Type[])null, (Type[])null)),
				new CodeInstruction(OpCodes.Brtrue_S, (object)label4)
			}))
			.Match(new InstructionMatcher().callvirt(AccessTools.Method(typeof(Tween), "TweenCallback", (Type[])null, (Type[])null)).pop().br())
			.Step(-1)
			.GetOperandLabel(out label5)
			.Match(new InstructionMatcher().ldloc_s(35).ldfld(null).callvirt(AccessTools.PropertyGetter(typeof(CardModel), "Pile"))
				.callvirt(AccessTools.PropertyGetter(typeof(CardPile), "Type"))
				.stloc_s(40)
				.ldloc_s(40)
				.ldc_i4_2()
				.sub()
				.switch_())
			.InsertCopy(-9, 2)
			.Insert((IEnumerable<CodeInstruction>)new _003C_003Ez__ReadOnlyArray<CodeInstruction>((CodeInstruction[])(object)new CodeInstruction[7]
			{
				CodeInstruction.LoadLocal(35, false),
				new CodeInstruction(OpCodes.Ldfld, operand2),
				new CodeInstruction(OpCodes.Ldfld, operand),
				CodeInstruction.LoadLocal(37, false),
				CodeInstruction.LoadLocal(2, false),
				new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(TheBigPatchToCardPileCmdAdd), "CustomPileUseCustomTween", (Type[])null, (Type[])null)),
				new CodeInstruction(OpCodes.Brtrue_S, (object)label5)
			}));
	}

	public static bool IsPileCustomPileWhereCardShouldBeVisible(CardPile pile, CardModel card)
	{
		if (pile is CustomPile customPile)
		{
			return customPile.CardShouldBeVisible(card);
		}
		return false;
	}

	public static bool IsPileCustomPileWithCardNotVisible(CardPile pile, CardModel card)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (pile is CustomPile)
		{
			return NCard.FindOnTable(card, (PileType?)pile.Type) == null;
		}
		return false;
	}

	public static bool CustomPileWithoutCustomTransition(CardPile pile, CardModel card)
	{
		if (pile is CustomPile customPile && !customPile.CardShouldBeVisible(card))
		{
			return !customPile.NeedsCustomTransitionVisual;
		}
		return false;
	}

	public static bool CustomPileUseGenericTweenForOtherPlayers(CardModel card)
	{
		if (card.Pile is CustomPile customPile)
		{
			if (!customPile.CardShouldBeVisible(card))
			{
				return !customPile.NeedsCustomTransitionVisual;
			}
			return true;
		}
		return false;
	}

	public static bool CustomPileUseCustomTween(CardModel card, NCard cardNode, CardPile oldPile, Tween tween)
	{
		if (!(card.Pile is CustomPile customPile))
		{
			return false;
		}
		return customPile.CustomTween(tween, card, cardNode, oldPile);
	}
}
