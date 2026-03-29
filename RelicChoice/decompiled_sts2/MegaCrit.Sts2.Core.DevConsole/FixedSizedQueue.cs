using System.Collections.Generic;

namespace MegaCrit.Sts2.Core.DevConsole;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class FixedSizedQueue<T> : List<T>
{
	private readonly int _limit;

	public FixedSizedQueue(int limit)
	{
		_limit = limit;
	}

	public void Enqueue(T obj)
	{
		if (base.Count >= _limit)
		{
			RemoveAt(base.Count - 1);
		}
		Insert(0, obj);
	}
}
