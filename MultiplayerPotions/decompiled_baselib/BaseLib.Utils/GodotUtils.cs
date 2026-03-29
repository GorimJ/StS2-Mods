using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace BaseLib.Utils;

public static class GodotUtils
{
	public static NCreatureVisuals CreatureVisualsFromScene(string path)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		NCreatureVisuals val = new NCreatureVisuals();
		TransferNodes((Node)(object)val, PreloadManager.Cache.GetScene(path).Instantiate((GenEditState)0), "Visuals", "Bounds", "IntentPos", "CenterPos", "OrbPos", "TalkPos");
		return val;
	}

	private static void TransferNodes(Node target, Node source, params string[] names)
	{
		TransferNodes(target, source, uniqueNames: true, names);
	}

	private static void TransferNodes(Node target, Node source, bool uniqueNames, params string[] names)
	{
		target.Name = source.Name;
		List<string> list = names.ToList();
		foreach (Node child in source.GetChildren(false))
		{
			list.Remove(StringName.op_Implicit(child.Name));
			source.RemoveChild(child);
			if (uniqueNames)
			{
				child.UniqueNameInOwner = true;
			}
			target.AddChild(child, false, (InternalMode)0);
			child.Owner = target;
		}
		if (list.Count > 0)
		{
			MainFile.Logger.Warn("Created " + ((object)target).GetType().FullName + " missing required children " + string.Join(" ", list), 1);
		}
		source.QueueFree();
	}
}
