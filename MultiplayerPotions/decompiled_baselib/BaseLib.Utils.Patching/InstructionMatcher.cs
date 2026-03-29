using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BaseLib.Extensions;
using HarmonyLib;

namespace BaseLib.Utils.Patching;

public class InstructionMatcher : IMatcher
{
	private readonly List<CodeInstruction> _target = new List<CodeInstruction>();

	public bool Match(List<string> log, List<CodeInstruction> code, int startIndex, out int matchStart, out int matchEnd)
	{
		log.Add("Starting InstructionMatcher");
		matchStart = startIndex;
		matchEnd = matchStart;
		int num = 0;
		for (int i = startIndex; i < code.Count; i++)
		{
			if (code[i].opcode == _target[num].opcode)
			{
				if (_target[num].operand == null || object.Equals(ComparisonOperand(code[i]), _target[num].operand))
				{
					log.Add($"Instruction match {code[i]}");
					num++;
					if (num >= _target.Count)
					{
						matchEnd = i + 1;
						matchStart = matchEnd - _target.Count;
						return true;
					}
					continue;
				}
				log.Add($"Opcode match but operand mismatch {code[i].opcode} | [{code[i].operand?.GetType() ?? null}]{code[i].operand} vs {_target[num].operand}");
			}
			if (num > 0)
			{
				log.Add($"Match ended, opcodes do not match ({code[i].opcode}, {_target[num].opcode})");
				num = 0;
			}
		}
		return false;
	}

	private object ComparisonOperand(CodeInstruction codeInstruction)
	{
		short value = codeInstruction.opcode.Value;
		if ((uint)(value - 17) <= 2u)
		{
			return ((LocalBuilder)codeInstruction.operand).LocalIndex;
		}
		return codeInstruction.operand;
	}

	public override string ToString()
	{
		return "InstructionMatcher:\n" + _target.AsReadable("\n");
	}

	public InstructionMatcher nop()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Nop, (object)null));
		return this;
	}

	public InstructionMatcher Break()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Break, (object)null));
		return this;
	}

	public InstructionMatcher ldarg_0()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldarg_0, (object)null));
		return this;
	}

	public InstructionMatcher ldarg_1()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldarg_1, (object)null));
		return this;
	}

	public InstructionMatcher ldarg_2()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldarg_2, (object)null));
		return this;
	}

	public InstructionMatcher ldarg_3()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldarg_3, (object)null));
		return this;
	}

	public InstructionMatcher ldloc_0()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldloc_0, (object)null));
		return this;
	}

	public InstructionMatcher ldloc_1()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldloc_1, (object)null));
		return this;
	}

	public InstructionMatcher ldloc_2()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldloc_2, (object)null));
		return this;
	}

	public InstructionMatcher ldloc_3()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldloc_3, (object)null));
		return this;
	}

	public InstructionMatcher stloc_0()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Stloc_0, (object)null));
		return this;
	}

	public InstructionMatcher stloc_1()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Stloc_1, (object)null));
		return this;
	}

	public InstructionMatcher stloc_2()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Stloc_2, (object)null));
		return this;
	}

	public InstructionMatcher stloc_3()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Stloc_3, (object)null));
		return this;
	}

	public InstructionMatcher ldloc_s(int index)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldloc_S, (object)index));
		return this;
	}

	public InstructionMatcher ldloca_s(int index)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldloca_S, (object)index));
		return this;
	}

	public InstructionMatcher stloc_s(int index)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Stloc_S, (object)index));
		return this;
	}

	public InstructionMatcher stloc_s()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Stloc_S, (object)null));
		return this;
	}

	public InstructionMatcher ldnull()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldnull, (object)null));
		return this;
	}

	public InstructionMatcher ldc_i4_m1()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldc_I4_M1, (object)null));
		return this;
	}

	public InstructionMatcher ldc_i4_0()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldc_I4_0, (object)null));
		return this;
	}

	public InstructionMatcher ldc_i4_1()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldc_I4_1, (object)null));
		return this;
	}

	public InstructionMatcher ldc_i4_2()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldc_I4_2, (object)null));
		return this;
	}

	public InstructionMatcher ldc_i4_3()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldc_I4_3, (object)null));
		return this;
	}

	public InstructionMatcher ldc_i4_4()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldc_I4_4, (object)null));
		return this;
	}

	public InstructionMatcher ldc_i4_5()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldc_I4_5, (object)null));
		return this;
	}

	public InstructionMatcher ldc_i4_6()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldc_I4_6, (object)null));
		return this;
	}

	public InstructionMatcher ldc_i4_7()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldc_I4_7, (object)null));
		return this;
	}

	public InstructionMatcher ldc_i4_8()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldc_I4_8, (object)null));
		return this;
	}

	public InstructionMatcher dup()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Dup, (object)null));
		return this;
	}

	public InstructionMatcher pop()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Pop, (object)null));
		return this;
	}

	public InstructionMatcher call(Type declaringType, string methodName, Type[]? parameters = null, Type[]? generics = null)
	{
		return call(AccessTools.Method(declaringType, methodName, parameters, generics));
	}

	public InstructionMatcher call(MethodInfo? method)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Call, (object)method));
		return this;
	}

	public InstructionMatcher ret()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ret, (object)null));
		return this;
	}

	public InstructionMatcher br_s(Label label)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Br_S, (object)label));
		return this;
	}

	public InstructionMatcher br_s()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Br_S, (object)null));
		return this;
	}

	public InstructionMatcher brfalse_s(Label label)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Brfalse_S, (object)label));
		return this;
	}

	public InstructionMatcher brfalse_s()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Brfalse_S, (object)null));
		return this;
	}

	public InstructionMatcher brtrue_s(Label label)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Brtrue_S, (object)label));
		return this;
	}

	public InstructionMatcher brtrue_s()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Brtrue_S, (object)null));
		return this;
	}

	public InstructionMatcher beq_s(Label label)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Beq_S, (object)label));
		return this;
	}

	public InstructionMatcher beq_s()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Beq_S, (object)null));
		return this;
	}

	public InstructionMatcher ble_un_s(Label label)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ble_Un_S, (object)label));
		return this;
	}

	public InstructionMatcher ble_un_s()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ble_Un_S, (object)null));
		return this;
	}

	public InstructionMatcher br(Label label)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Br, (object)label));
		return this;
	}

	public InstructionMatcher br()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Br, (object)null));
		return this;
	}

	public InstructionMatcher switch_()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Switch, (object)null));
		return this;
	}

	public InstructionMatcher add()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Add, (object)null));
		return this;
	}

	public InstructionMatcher sub()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Sub, (object)null));
		return this;
	}

	public InstructionMatcher mul()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Mul, (object)null));
		return this;
	}

	public InstructionMatcher div()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Div, (object)null));
		return this;
	}

	public InstructionMatcher callvirt(Type declaringType, string methodName, Type[]? parameters = null, Type[]? generics = null)
	{
		return callvirt(AccessTools.Method(declaringType, methodName, parameters, generics));
	}

	public InstructionMatcher callvirt(MethodInfo? method)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Callvirt, (object)method));
		return this;
	}

	public InstructionMatcher ldfld(Type declaringType, string fieldName)
	{
		return ldfld(AccessTools.Field(declaringType, fieldName));
	}

	public InstructionMatcher ldfld(FieldInfo? field)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Ldfld, (object)field));
		return this;
	}

	public InstructionMatcher stfld(Type declaringType, string fieldName)
	{
		return stfld(AccessTools.Field(declaringType, fieldName));
	}

	public InstructionMatcher stfld(FieldInfo? field)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Stfld, (object)field));
		return this;
	}

	public InstructionMatcher newarr(Type? type)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Newarr, (object)type));
		return this;
	}

	public InstructionMatcher stelem_ref()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_target.Add(new CodeInstruction(OpCodes.Stelem_Ref, (object)null));
		return this;
	}
}
