using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "Seductress";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();

        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);

        // ClimaxModel is NOT constructed here. ModelDb.Init instantiates every registered model
        // type itself, and CustomSingletonModel subscribes for combat hooks in its constructor —
        // so constructing one here as well produces a second instance whose subscription is
        // rejected ("id already been used") while still overwriting ClimaxModel.Instance, leaving
        // the live Instance unsubscribed. Let ModelDb own it.

        Reservoirs.RegisterSaves();

        new Harmony(ModId).PatchAll(assembly);

        Logger.Info("[Seductress] spike initialised");
    }
}
