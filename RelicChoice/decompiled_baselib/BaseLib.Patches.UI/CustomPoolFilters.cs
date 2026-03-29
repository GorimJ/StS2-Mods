using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BaseLib.Abstracts;
using BaseLib.Utils;
using BaseLib.Utils.Patching;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace BaseLib.Patches.UI;

[HarmonyPatch(typeof(NCardLibrary), "_Ready")]
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class CustomPoolFilters
{
	private const float baseSize = 64f;

	[HarmonyTranspiler]
	private static List<CodeInstruction> AddFilters(IEnumerable<CodeInstruction> instructions)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		return new InstructionPatcher(instructions).Match(new InstructionMatcher().ldfld(AccessTools.DeclaredField(typeof(NCardLibrary), "_regentFilter")).callvirt(null)).Insert((IEnumerable<CodeInstruction>)new _003C_003Ez__ReadOnlyArray<CodeInstruction>((CodeInstruction[])(object)new CodeInstruction[7]
		{
			CodeInstruction.LoadArgument(0, false),
			CodeInstruction.LoadArgument(0, false),
			new CodeInstruction(OpCodes.Ldfld, (object)AccessTools.DeclaredField(typeof(NCardLibrary), "_poolFilters")),
			CodeInstruction.LoadArgument(0, false),
			new CodeInstruction(OpCodes.Ldfld, (object)AccessTools.DeclaredField(typeof(NCardLibrary), "_cardPoolFilters")),
			CodeInstruction.LoadLocal(0, false),
			CodeInstruction.Call(typeof(CustomPoolFilters), "GenerateCustomFilters", (Type[])null, (Type[])null)
		}));
	}

	public static void GenerateCustomFilters(NCardLibrary library, Dictionary<NCardPoolFilter, Func<CardModel, bool>> filtering, Dictionary<CharacterModel, NCardPoolFilter> characterFilters, Callable updateFilter)
	{
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		if (characterFilters.Count == 0)
		{
			throw new Exception("Attempted to generate custom filters at wrong time");
		}
		NCardPoolFilter miscPoolFilter = library._miscPoolFilter;
		Func<CardModel, bool> oldFilter = filtering[miscPoolFilter];
		filtering[miscPoolFilter] = (CardModel c) => false || oldFilter(c);
		Node parent = ((Node)characterFilters[(CharacterModel)(object)ModelDb.Character<Ironclad>()]).GetParent();
		foreach (CustomCharacterModel customCharacter in ModelDbCustomCharacters.CustomCharacters)
		{
			NCardPoolFilter filter = GenerateFilter(customCharacter);
			parent.AddChild((Node)(object)filter, true, (InternalMode)0);
			characterFilters.Add((CharacterModel)(object)customCharacter, filter);
			CardPoolModel pool = ((CharacterModel)customCharacter).CardPool;
			filtering.Add(filter, (CardModel c) => pool.AllCardIds.Contains(((AbstractModel)c).Id));
			((GodotObject)filter).Connect(SignalName.Toggled, updateFilter, 0u);
			((GodotObject)filter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
			{
				library._lastHoveredControl = (Control)(object)filter;
			}), 0u);
		}
	}

	private static NCardPoolFilter GenerateFilter(CustomCharacterModel character)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		NCardPoolFilter val = new NCardPoolFilter
		{
			Name = StringName.op_Implicit("FILTER-" + (object)((AbstractModel)character).Id),
			Size = new Vector2(64f, 64f),
			CustomMinimumSize = new Vector2(64f, 64f)
		};
		Texture2D iconTexture = ((CharacterModel)character).IconTexture;
		TextureRect val2 = new TextureRect
		{
			Name = StringName.op_Implicit("Image"),
			Texture = iconTexture,
			ExpandMode = (ExpandModeEnum)1,
			StretchMode = (StretchModeEnum)5,
			Size = new Vector2(56f, 56f),
			Position = new Vector2(4f, 4f),
			Scale = new Vector2(0.9f, 0.9f),
			PivotOffset = new Vector2(28f, 28f),
			Material = (Material)(object)ShaderUtils.GenerateHsv(1f, 1f, 1f)
		};
		TextureRect val3 = new TextureRect
		{
			Name = StringName.op_Implicit("Shadow"),
			Texture = iconTexture,
			ExpandMode = (ExpandModeEnum)1,
			StretchMode = (StretchModeEnum)5,
			Size = new Vector2(56f, 56f),
			Position = new Vector2(4f, 3f),
			PivotOffset = new Vector2(28f, 28f)
		};
		((Node)val2).AddChild((Node)(object)val3, false, (InternalMode)0);
		NSelectionReticle val4 = PreloadManager.Cache.GetScene(SceneHelper.GetScenePath("ui/selection_reticle")).Instantiate<NSelectionReticle>((GenEditState)0);
		((Node)val4).Name = StringName.op_Implicit("SelectionReticle");
		((Node)val4).UniqueNameInOwner = true;
		((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
		((Node)val2).Owner = (Node)(object)val;
		((Node)val).AddChild((Node)(object)val4, false, (InternalMode)0);
		((Node)val4).Owner = (Node)(object)val;
		return val;
	}

	[HarmonyPostfix]
	private static void AdjustFilterScales(NCardLibrary __instance)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		Control parentControl = ((Control)__instance._poolFilters.First().Key).GetParentControl();
		GridContainer val = (GridContainer)(object)((parentControl is GridContainer) ? parentControl : null);
		if (val == null)
		{
			throw new Exception("Failed to find grid container for PoolFilters");
		}
		int childCount = ((Node)val).GetChildCount(false);
		Vector2 one = Vector2.One;
		int num = 4;
		float num2 = 64f * one.Y * MathF.Ceiling((float)childCount / (float)num);
		float num3 = 192f;
		while (num2 > num3)
		{
			num++;
			one = Vector2.One * (4f / (float)num);
			num2 = 64f * one.Y * MathF.Ceiling((float)childCount / (float)num);
		}
		one = Vector2.One * (4f / (float)num);
		foreach (Node child2 in ((Node)val).GetChildren(false))
		{
			NCardPoolFilter val2 = (NCardPoolFilter)(object)((child2 is NCardPoolFilter) ? child2 : null);
			if (val2 == null)
			{
				continue;
			}
			((Control)val2).CustomMinimumSize = ((Control)val2).CustomMinimumSize * one;
			((Control)val2).Size = ((Control)val2).Size * one;
			((Control)val2).PivotOffset = ((Control)val2).PivotOffset * one;
			Control image = val2._image;
			image.CustomMinimumSize *= one;
			Control image2 = val2._image;
			image2.Size *= one;
			Control image3 = val2._image;
			image3.PivotOffset *= one;
			val2._image.Position = (((Control)val2).Size - val2._image.Size) * 0.5f;
			if (((Node)val2._image).GetChildCount(false) > 0)
			{
				Node child = ((Node)val2._image).GetChild(0, false);
				Control val3 = (Control)(object)((child is Control) ? child : null);
				if (val3 != null)
				{
					val3.CustomMinimumSize *= one;
					val3.Size *= one;
					val3.PivotOffset *= one;
				}
			}
			NSelectionReticle controllerSelectionReticle = val2._controllerSelectionReticle;
			((Control)controllerSelectionReticle).CustomMinimumSize = ((Control)controllerSelectionReticle).CustomMinimumSize * one;
			NSelectionReticle controllerSelectionReticle2 = val2._controllerSelectionReticle;
			((Control)controllerSelectionReticle2).Size = ((Control)controllerSelectionReticle2).Size * one;
			NSelectionReticle controllerSelectionReticle3 = val2._controllerSelectionReticle;
			((Control)controllerSelectionReticle3).PivotOffset = ((Control)controllerSelectionReticle3).PivotOffset * one;
			NSelectionReticle controllerSelectionReticle4 = val2._controllerSelectionReticle;
			((Control)controllerSelectionReticle4).Position = ((Control)controllerSelectionReticle4).Position * one;
		}
		val.Columns = num;
	}
}
