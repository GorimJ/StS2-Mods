using System;

namespace MegaCrit.Sts2.Core.Modding;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ModInitializerAttribute : Attribute
{
	public string initializerMethod;

	public ModInitializerAttribute(string initializerMethod)
	{
		this.initializerMethod = initializerMethod;
	}
}
