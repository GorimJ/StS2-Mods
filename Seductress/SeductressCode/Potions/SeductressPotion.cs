using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Seductress.SeductressCode.Character;
using Seductress.SeductressCode.Extensions;

namespace Seductress.SeductressCode.Potions;

[Pool(typeof(SeductressPotionPool))]
public abstract class SeductressPotion : CustomPotionModel
{
	public override string? CustomPackedImagePath =>
		$"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();
	public override string? CustomPackedOutlinePath =>
		$"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionOutlineImagePath();
}