using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

public abstract class CustomRelicModel : RelicModel, ICustomModel
{
	public CustomRelicModel(bool autoAdd = true)
	{
		if (autoAdd)
		{
			CustomContentDictionary.AddModel(((object)this).GetType());
		}
	}

	public virtual RelicModel? GetUpgradeReplacement()
	{
		return null;
	}
}
