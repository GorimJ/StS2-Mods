using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal class CustomCardPoolMaterialPatch
{
	[HarmonyPrefix]
	private static bool UseCustomMaterial(CardPoolModel __instance, ref Material __result)
	{
		if (__instance is CustomCardPoolModel customCardPoolModel)
		{
			if (!((CardPoolModel)customCardPoolModel).CardFrameMaterialPath.Equals("card_frame_red"))
			{
				return true;
			}
			__result = (Material)(object)ShaderUtils.GenerateHsv(customCardPoolModel.H, customCardPoolModel.S, customCardPoolModel.V);
			return false;
		}
		return true;
	}
}
