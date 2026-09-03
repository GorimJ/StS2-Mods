using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using CharacterTemplate.CharacterTemplateCode.Character;
using CharacterTemplate.CharacterTemplateCode.Extensions;

namespace CharacterTemplate.CharacterTemplateCode.Potions;

[Pool(typeof(CharacterTemplatePotionPool))]
public abstract class CharacterTemplatePotion : CustomPotionModel
{
	public override string? CustomPackedImagePath =>
		$"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();
	public override string? CustomPackedOutlinePath =>
		$"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionOutlineImagePath();
}