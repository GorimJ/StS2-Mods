using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace BaseLib.Extensions;

public static class HarmonyExtensions
{
	public static void PatchAsyncMoveNext(this Harmony harmony, MethodInfo asyncMethod, HarmonyMethod? prefix = null, HarmonyMethod? postfix = null, HarmonyMethod? transpiler = null, HarmonyMethod? finalizer = null)
	{
		MethodInfo method = asyncMethod.StateMachineType().GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.NonPublic);
		harmony.Patch((MethodBase)method, prefix, postfix, transpiler, finalizer);
	}

	public static void PatchAsyncMoveNext(this Harmony harmony, MethodInfo asyncMethod, out Type stateMachineType, HarmonyMethod? prefix = null, HarmonyMethod? postfix = null, HarmonyMethod? transpiler = null, HarmonyMethod? finalizer = null)
	{
		AsyncStateMachineAttribute customAttribute = asyncMethod.GetCustomAttribute<AsyncStateMachineAttribute>();
		if (customAttribute == null)
		{
			throw new ArgumentException("MethodInfo " + GeneralExtensions.FullDescription((MethodBase)asyncMethod) + " passed to PatchAsync is not an async method");
		}
		stateMachineType = customAttribute.StateMachineType;
		MethodInfo method = stateMachineType.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.NonPublic);
		harmony.Patch((MethodBase)method, prefix, postfix, transpiler, finalizer);
	}
}
