using System;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Patches.Content;

[HarmonyPatch(typeof(ModelDb), "GetEntry")]
public class PrefixIdPatch
{
	[HarmonyPostfix]
	private static string AdjustID(string __result, Type type)
	{
		if (type.IsAssignableTo(typeof(ICustomModel)))
		{
			return type.GetPrefix() + __result;
		}
		return __result;
	}
}
