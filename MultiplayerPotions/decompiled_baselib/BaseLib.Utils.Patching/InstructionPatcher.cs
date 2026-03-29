using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;

namespace BaseLib.Utils.Patching;

public class InstructionPatcher
{
	private readonly List<CodeInstruction> code = instructions.ToList();

	private int index = -1;

	private int lastMatchStart = -1;

	public readonly List<string> Log = new List<string>();

	public InstructionPatcher(IEnumerable<CodeInstruction> instructions)
	{
	}

	public static implicit operator List<CodeInstruction>(InstructionPatcher locator)
	{
		return locator.code;
	}

	public InstructionPatcher ResetPosition()
	{
		index = -1;
		lastMatchStart = -1;
		return this;
	}

	public InstructionPatcher Match(params IMatcher[] matchers)
	{
		return Match(DefaultMatchFailure, matchers);
	}

	public InstructionPatcher Match(Action<IMatcher[]> onFailMatch, params IMatcher[] matchers)
	{
		index = 0;
		for (int i = 0; i < matchers.Length; i++)
		{
			if (!matchers[i].Match(Log, code, index, out lastMatchStart, out index))
			{
				onFailMatch(matchers);
				return this;
			}
		}
		Log.Add("Found end of match at " + index + "; last match starts at " + lastMatchStart);
		return this;
	}

	public InstructionPatcher Step(int amt = 1)
	{
		if (index < 0)
		{
			throw new Exception("Attempted to Step without any match found");
		}
		index += amt;
		Log.Add("Stepped to " + index);
		return this;
	}

	public InstructionPatcher GetLabels(out List<Label> labels)
	{
		if (index < 0)
		{
			throw new Exception("Attempted to GetLabels without any match found");
		}
		labels = code[index].labels;
		if (labels.Count == 0)
		{
			if (code[index].operand is Label)
			{
				throw new Exception("Code instruction " + ((object)code[index]).ToString() + " has no labels. Did you mean to use GetOperandLabel instead?");
			}
			throw new Exception("Code instruction " + ((object)code[index]).ToString() + " has no labels");
		}
		return this;
	}

	public InstructionPatcher GetOperandLabel(out Label label)
	{
		if (index < 0)
		{
			throw new Exception("Attempted to GetOperandLabel without any match found");
		}
		if (code[index].operand is Label label2)
		{
			label = label2;
			return this;
		}
		throw new Exception("Code instruction " + ((object)code[index]).ToString() + " does not have a Label parameter");
	}

	public InstructionPatcher GetOperand(out object operand)
	{
		if (index < 0)
		{
			throw new Exception("Attempted to GetOperand without any match found");
		}
		operand = code[index].operand;
		return this;
	}

	public InstructionPatcher ReplaceLastMatch(IEnumerable<CodeInstruction> replacement)
	{
		if (lastMatchStart < 0)
		{
			throw new Exception("Attempted to ReplaceLastMatch without any match found");
		}
		int num = 0;
		foreach (CodeInstruction item in replacement)
		{
			code[lastMatchStart + num] = item;
			num++;
		}
		code.RemoveRange(lastMatchStart + num, index - (lastMatchStart + num));
		index = lastMatchStart + num;
		return this;
	}

	public InstructionPatcher Replace(CodeInstruction replacement, bool keepLabels = true)
	{
		if (index < 0)
		{
			throw new Exception("Attempted to Replace without any match found");
		}
		if (keepLabels)
		{
			CodeInstructionExtensions.MoveLabelsFrom(replacement, code[index]);
		}
		Log.Add($"{code[index]} => {replacement}");
		code[index] = replacement;
		return this;
	}

	public InstructionPatcher IncrementIntPush()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		if (index < 0)
		{
			throw new Exception("Attempted to Replace without any match found");
		}
		return code[index].opcode.Value switch
		{
			21 => Replace(new CodeInstruction(OpCodes.Ldc_I4_0, (object)null)), 
			22 => Replace(new CodeInstruction(OpCodes.Ldc_I4_1, (object)null)), 
			23 => Replace(new CodeInstruction(OpCodes.Ldc_I4_2, (object)null)), 
			24 => Replace(new CodeInstruction(OpCodes.Ldc_I4_3, (object)null)), 
			25 => Replace(new CodeInstruction(OpCodes.Ldc_I4_4, (object)null)), 
			26 => Replace(new CodeInstruction(OpCodes.Ldc_I4_5, (object)null)), 
			27 => Replace(new CodeInstruction(OpCodes.Ldc_I4_6, (object)null)), 
			28 => Replace(new CodeInstruction(OpCodes.Ldc_I4_7, (object)null)), 
			29 => Replace(new CodeInstruction(OpCodes.Ldc_I4_8, (object)null)), 
			30 => throw new Exception("Instruction " + ((object)code[index])?.ToString() + " cannot be incremented"), 
			_ => throw new Exception("Instruction " + ((object)code[index])?.ToString() + " is not an int push instruction that can be incremented"), 
		};
	}

	public InstructionPatcher IncrementIntPush(out CodeInstruction replacedPush)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Expected O, but got Unknown
		if (index < 0)
		{
			throw new Exception("Attempted to Replace without any match found");
		}
		switch (code[index].opcode.Value)
		{
		case 21:
			replacedPush = new CodeInstruction(OpCodes.Ldc_I4_M1, (object)null);
			return Replace(new CodeInstruction(OpCodes.Ldc_I4_0, (object)null));
		case 22:
			replacedPush = new CodeInstruction(OpCodes.Ldc_I4_0, (object)null);
			return Replace(new CodeInstruction(OpCodes.Ldc_I4_1, (object)null));
		case 23:
			replacedPush = new CodeInstruction(OpCodes.Ldc_I4_1, (object)null);
			return Replace(new CodeInstruction(OpCodes.Ldc_I4_2, (object)null));
		case 24:
			replacedPush = new CodeInstruction(OpCodes.Ldc_I4_2, (object)null);
			return Replace(new CodeInstruction(OpCodes.Ldc_I4_3, (object)null));
		case 25:
			replacedPush = new CodeInstruction(OpCodes.Ldc_I4_3, (object)null);
			return Replace(new CodeInstruction(OpCodes.Ldc_I4_4, (object)null));
		case 26:
			replacedPush = new CodeInstruction(OpCodes.Ldc_I4_4, (object)null);
			return Replace(new CodeInstruction(OpCodes.Ldc_I4_5, (object)null));
		case 27:
			replacedPush = new CodeInstruction(OpCodes.Ldc_I4_5, (object)null);
			return Replace(new CodeInstruction(OpCodes.Ldc_I4_6, (object)null));
		case 28:
			replacedPush = new CodeInstruction(OpCodes.Ldc_I4_6, (object)null);
			return Replace(new CodeInstruction(OpCodes.Ldc_I4_7, (object)null));
		case 29:
			replacedPush = new CodeInstruction(OpCodes.Ldc_I4_7, (object)null);
			return Replace(new CodeInstruction(OpCodes.Ldc_I4_8, (object)null));
		case 30:
			replacedPush = new CodeInstruction(OpCodes.Ldc_I4_8, (object)null);
			return Replace(new CodeInstruction(OpCodes.Ldc_I4_S, (object)(sbyte)9));
		default:
			throw new Exception("Instruction " + ((object)code[index])?.ToString() + " is not an int push instruction that can be incremented");
		}
	}

	public InstructionPatcher Insert(CodeInstruction instruction)
	{
		if (index < 0)
		{
			throw new Exception("Attempted to Insert without any match found");
		}
		code.Insert(index, instruction);
		index++;
		return this;
	}

	public InstructionPatcher Insert(IEnumerable<CodeInstruction> insert)
	{
		if (index < 0)
		{
			throw new Exception("Attempted to Insert without any match found");
		}
		code.InsertRange(index, insert);
		index += insert.Count();
		return this;
	}

	public InstructionPatcher InsertCopy(int startOffset, int copyLength)
	{
		if (index < 0)
		{
			throw new Exception("Attempted to InsertCopy without any match found");
		}
		int num = index + startOffset;
		if (num < 0)
		{
			throw new Exception($"startIndex of InsertCopy less than 0 ({num})");
		}
		List<CodeInstruction> list = new List<CodeInstruction>();
		for (int i = 0; i < copyLength; i++)
		{
			Log.Add("Inserting Copy: " + (object)code[num + i]);
			list.Add(code[num + i].Clone());
		}
		return Insert((IEnumerable<CodeInstruction>)list);
	}

	public InstructionPatcher PrintLog(Logger logger)
	{
		logger.Info(Log.AsReadable("\n"), 1);
		return this;
	}

	public InstructionPatcher PrintResult(Logger logger)
	{
		logger.Info("----- RESULT -----\n" + ((List<CodeInstruction>)this).NumberedLines(), 1);
		return this;
	}

	private void DefaultMatchFailure(IMatcher[] matchers)
	{
		throw new Exception("Failed to find match:\n" + matchers.AsReadable("\n---------\n") + "\nLOG:\n" + Log.AsReadable("\n"));
	}
}
