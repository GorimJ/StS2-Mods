using System;
using System.Collections.Generic;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

public abstract class CustomPotionPoolModel : PotionPoolModel, ICustomModel
{
	public virtual bool IsShared => false;

	public CustomPotionPoolModel()
	{
		if (IsShared)
		{
			ModelDbSharedPotionPoolsPatch.Register(this);
		}
	}

	public override IEnumerable<PotionModel> GenerateAllPotions()
	{
		return Array.Empty<PotionModel>();
	}
}
