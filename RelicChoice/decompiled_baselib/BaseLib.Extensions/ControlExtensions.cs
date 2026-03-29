using Godot;

namespace BaseLib.Extensions;

public static class ControlExtensions
{
	public static void DrawDebug(this Control item)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)item).DrawRect(new Rect2(0f, 0f, item.Size), new Color(1f, 1f, 1f, 0.5f), true, -1f, false);
	}

	public static void DrawDebug(this Control artist, Control child)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)artist).DrawRect(new Rect2(child.Position, child.Size), new Color(1f, 1f, 1f, 0.5f), true, -1f, false);
	}
}
