using System.Collections.Generic;
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Patches.Content;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public class ModelDbSharedCardPoolsPatch
{
	private static readonly List<CardPoolModel> customSharedPools = new List<CardPoolModel>();

	[HarmonyPostfix]
	private static IEnumerable<CardPoolModel> AddCustomPools(IEnumerable<CardPoolModel> __result)
	{
		List<CardPoolModel> list = new List<CardPoolModel>();
		list.AddRange(__result);
		list.AddRange(customSharedPools);
		return new _003C_003Ez__ReadOnlyList<CardPoolModel>(list);
	}

	public static void Register(CustomCardPoolModel pool)
	{
		customSharedPools.Add((CardPoolModel)(object)pool);
	}
}
