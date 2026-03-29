using System;
using System.Linq;
using System.Reflection;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Patches.Content;

[HarmonyPatch(typeof(ModelDb), "Init")]
internal class GenEnumValues
{
	[HarmonyPrefix]
	private static void FindAndGenerate()
	{
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		Type[] modTypes = ReflectionHelper.ModTypes;
		foreach (Type type in modTypes)
		{
			foreach (FieldInfo item in from field in type.GetFields()
				where Attribute.IsDefined(field, typeof(CustomEnumAttribute))
				select field)
			{
				if (!item.FieldType.IsEnum)
				{
					throw new Exception($"Field {item.DeclaringType?.FullName}.{item.Name} should be an enum type for CustomEnum");
				}
				if (!item.IsStatic)
				{
					throw new Exception($"Field {item.DeclaringType?.FullName}.{item.Name} should be static for CustomEnum");
				}
				if (item.DeclaringType == null)
				{
					continue;
				}
				CustomEnumAttribute customAttribute = item.GetCustomAttribute<CustomEnumAttribute>();
				object obj = CustomEnums.GenerateKey(item.FieldType);
				item.SetValue(null, obj);
				if (item.FieldType == typeof(CardKeyword))
				{
					string value = item.DeclaringType.GetPrefix() + (customAttribute?.Name ?? item.Name).ToUpperInvariant();
					CustomKeywords.KeywordIDs.Add((int)obj, value);
				}
				if (!(item.FieldType != typeof(PileType)) && type.IsAssignableTo(typeof(CustomPile)))
				{
					ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, Array.Empty<Type>()) ?? throw new Exception("CustomPile " + type.FullName + " with custom PileType does not have an accessible no-parameter constructor");
					PileType? val = (PileType?)item.GetValue(null);
					if (!val.HasValue)
					{
						throw new Exception("Failed to be set up custom PileType in " + type.FullName);
					}
					CustomPiles.RegisterCustomPile(val.Value, () => (CustomPile)constructor.Invoke(null));
				}
			}
		}
	}
}
