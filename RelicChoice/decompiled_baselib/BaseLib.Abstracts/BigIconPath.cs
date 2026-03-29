using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal class BigIconPath
{
	[HarmonyPrefix]
	private static bool Custom(PowerModel __instance, ref string? __result)
	{
		if (!(__instance is CustomPowerModel customPowerModel))
		{
			return true;
		}
		__result = customPowerModel.CustomBigIconPath;
		return __result == null;
	}
}
