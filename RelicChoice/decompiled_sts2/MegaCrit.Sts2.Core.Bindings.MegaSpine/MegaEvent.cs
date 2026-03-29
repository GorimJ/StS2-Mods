using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class MegaEvent : MegaSpineBinding
{
	protected override string SpineClassName => "SpineEvent";

	protected override IEnumerable<string> SpineMethods => new global::_003C_003Ez__ReadOnlySingleElementList<string>("get_data");

	public MegaEvent(Variant native)
		: base(native)
	{
	}

	public MegaEventData GetData()
	{
		return new MegaEventData(Call("get_data"));
	}
}
