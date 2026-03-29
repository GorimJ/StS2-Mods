using System;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BaseLib.Patches.UI;

[HarmonyPatch(typeof(DynamicVar), "Clone")]
internal class CloneTooltips
{
	[HarmonyPostfix]
	private static DynamicVar Copy(DynamicVar __result, DynamicVar __instance)
	{
		Func<IHoverTip> func = DynamicVarExtensions.DynamicVarTips[__instance];
		if (func != null)
		{
			DynamicVarExtensions.DynamicVarTips[__result] = func;
		}
		return __result;
	}
}
