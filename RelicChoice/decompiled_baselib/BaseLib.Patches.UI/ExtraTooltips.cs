using System;
using System.Collections.Generic;
using BaseLib.Extensions;
using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Patches.UI;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ExtraTooltips
{
	[HarmonyTranspiler]
	private static List<CodeInstruction> AddCustomTips(IEnumerable<CodeInstruction> instructions)
	{
		return new InstructionPatcher(instructions).Match(new InstructionMatcher().ldarg_0().callvirt(AccessTools.PropertyGetter(typeof(CardModel), "ExtraHoverTips")).call(null)
			.stloc_0()).Insert((IEnumerable<CodeInstruction>)new _003C_003Ez__ReadOnlyArray<CodeInstruction>((CodeInstruction[])(object)new CodeInstruction[3]
		{
			CodeInstruction.LoadLocal(0, false),
			CodeInstruction.LoadArgument(0, false),
			CodeInstruction.Call(typeof(ExtraTooltips), "AddTips", (Type[])null, (Type[])null)
		}));
	}

	public static void AddTips(List<IHoverTip> tips, CardModel card)
	{
		foreach (DynamicVar value in card.DynamicVars.Values)
		{
			_ = DynamicVarExtensions.DynamicVarTips[value];
			IHoverTip val = DynamicVarExtensions.DynamicVarTips[value]?.Invoke();
			if (val != null)
			{
				tips.Add(val);
			}
		}
	}
}
