using System;
using System.Collections.Generic;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

public abstract class CustomRelicPoolModel : RelicPoolModel, ICustomModel
{
	public virtual bool IsShared => false;

	public CustomRelicPoolModel()
	{
		if (IsShared)
		{
			ModelDbSharedRelicPoolsPatch.Register(this);
		}
	}

	public override IEnumerable<RelicModel> GenerateAllRelics()
	{
		return Array.Empty<RelicModel>();
	}
}
