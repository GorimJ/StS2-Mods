using System;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace BaseLib.Patches.Content;

internal class GetCustomLocKey
{
	internal static void Patch(Harmony harmony)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		MethodInfo methodInfo = AccessTools.Method(AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.CardKeywordExtensions"), "GetLocKeyPrefix", (Type[])null, (Type[])null);
		MethodInfo methodInfo2 = AccessTools.Method(typeof(GetCustomLocKey), "UseCustomKeywordMap", (Type[])null, (Type[])null);
		harmony.Patch((MethodBase)methodInfo, new HarmonyMethod(methodInfo2), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
	}

	private static bool UseCustomKeywordMap(CardKeyword keyword, ref string? __result)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected I4, but got Unknown
		return !CustomKeywords.KeywordIDs.TryGetValue((int)keyword, out __result);
	}
}
