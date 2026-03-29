using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal class BigBetaIconPath
{
	[HarmonyPrefix]
	private static bool Custom(PowerModel __instance, ref string? __result)
	{
		if (!(__instance is CustomPowerModel customPowerModel))
		{
			return true;
		}
		__result = customPowerModel.CustomBigBetaIconPath;
		return __result == null;
	}
}
