using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ModelDbCustomCharacters
{
	public static readonly List<CustomCharacterModel> CustomCharacters = new List<CustomCharacterModel>();

	[HarmonyPostfix]
	public static IEnumerable<CharacterModel> AddCustomPools(IEnumerable<CharacterModel> __result)
	{
		List<CharacterModel> list = new List<CharacterModel>();
		list.AddRange(__result);
		foreach (CustomCharacterModel customCharacter in CustomCharacters)
		{
			list.Add((CharacterModel)(object)customCharacter);
		}
		return new _003C_003Ez__ReadOnlyList<CharacterModel>(list);
	}

	public static void Register(CustomCharacterModel character)
	{
		CustomCharacters.Add(character);
	}
}
