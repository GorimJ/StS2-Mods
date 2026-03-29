using Godot;

namespace BaseLib.Utils;

public class ShaderUtils
{
	public static ShaderMaterial GenerateHsv(float h, float s, float v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		ShaderMaterial val = new ShaderMaterial
		{
			Shader = (Shader)((Resource)GD.Load<Shader>("res://shaders/hsv.gdshader")).Duplicate(false)
		};
		val.SetShaderParameter(StringName.op_Implicit("h"), Variant.op_Implicit(h));
		val.SetShaderParameter(StringName.op_Implicit("s"), Variant.op_Implicit(s));
		val.SetShaderParameter(StringName.op_Implicit("v"), Variant.op_Implicit(v));
		return val;
	}
}
