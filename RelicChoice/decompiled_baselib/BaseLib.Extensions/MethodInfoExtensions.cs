using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace BaseLib.Extensions;

public static class MethodInfoExtensions
{
	public static Type StateMachineType(this MethodInfo methodInfo)
	{
		return (methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>() ?? throw new ArgumentException("MethodInfo " + GeneralExtensions.FullDescription((MethodBase)methodInfo) + " is not an async method")).StateMachineType;
	}
}
