using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace SexFixes;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    internal const string ModId = "SexFixes";

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        try
        {
            harmony.PatchAll(typeof(MainFile).Assembly);
            GD.Print("[SexFixes] patches applied");
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"[SexFixes] failed to apply patches: {e}");
        }
    }
}
