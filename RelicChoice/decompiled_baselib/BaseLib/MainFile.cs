using System;
using System.Runtime.InteropServices;
using BaseLib.Patches.Content;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace BaseLib;

[ModInitializer("Initialize")]
public static class MainFile
{
	public const string ModId = "BaseLib";

	private static nint _holder;

	public static Logger Logger { get; } = new Logger("BaseLib", (LogType)0);

	public static void Initialize()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		Libgcc();
		Harmony val = new Harmony("BaseLib");
		GetCustomLocKey.Patch(val);
		TheBigPatchToCardPileCmdAdd.Patch(val);
		val.PatchAll();
	}

	[DllImport("libdl.so.2")]
	private static extern nint dlopen(string filename, int flags);

	[DllImport("libdl.so.2")]
	private static extern nint dlerror();

	[DllImport("libdl.so.2")]
	private static extern nint dlsym(nint handle, string symbol);

	private static void Libgcc()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			Logger.Info("Running on Linux, manually dlopen libgcc for Harmony", 1);
			_holder = dlopen("libgcc_s.so.1", 258);
			if (_holder == IntPtr.Zero)
			{
				Logger.Info("Or Nor: " + Marshal.PtrToStringAnsi(dlerror()), 1);
			}
		}
	}
}
