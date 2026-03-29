using System;
using System.Collections.Generic;
using System.Reflection;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Patches.Content;

[HarmonyPatch(typeof(ModelDb), "InitIds")]
public static class CustomContentDictionary
{
	private static readonly List<Type> _customTypes;

	private static readonly Dictionary<Type, Type> _poolTypes;

	static CustomContentDictionary()
	{
		_customTypes = new List<Type>();
		_poolTypes = new Dictionary<Type, Type>();
		_poolTypes.Add(typeof(CardPoolModel), typeof(CardModel));
		_poolTypes.Add(typeof(RelicPoolModel), typeof(RelicModel));
		_poolTypes.Add(typeof(PotionPoolModel), typeof(PotionModel));
	}

	public static void AddModel(Type modelType)
	{
		_customTypes.Add(modelType);
		PoolAttribute poolAttribute = modelType.GetCustomAttribute<PoolAttribute>() ?? throw new Exception("Model " + modelType.FullName + " must be marked with a PoolAttribute to determine which pool to add it to.");
		if (!IsValidPool(modelType, poolAttribute.PoolType))
		{
			throw new Exception($"Model {modelType.FullName} is assigned to incorrect type of pool {poolAttribute.PoolType.FullName}.");
		}
		ModHelper.AddModelToPool(poolAttribute.PoolType, modelType);
	}

	private static bool IsValidPool(Type modelType, Type poolType)
	{
		Type baseType = poolType.BaseType;
		while (baseType != null)
		{
			if (_poolTypes.TryGetValue(baseType, out Type value))
			{
				return modelType.IsAssignableTo(value);
			}
			baseType = baseType.BaseType;
		}
		throw new Exception($"Model {modelType.FullName} is assigned to {poolType.FullName} which is not a valid pool type.");
	}

	[HarmonyPostfix]
	private static void ConvertTypesToModels()
	{
		MainFile.Logger.Info($"Custom Models: {_customTypes.Count}", 1);
	}
}
