using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace HandSmoother;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    internal const string ModId = "HandSmoother";

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
}
