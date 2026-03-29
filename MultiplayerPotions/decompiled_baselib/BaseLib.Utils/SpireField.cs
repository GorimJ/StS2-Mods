using System;
using System.Runtime.CompilerServices;

namespace BaseLib.Utils;

public class SpireField<TKey, TVal> where TKey : class where TVal : class
{
	private readonly ConditionalWeakTable<TKey, TVal?> _table = new ConditionalWeakTable<TKey, TVal>();

	private readonly Func<TKey, TVal?> _defaultVal;

	public TVal? this[TKey obj]
	{
		get
		{
			return Get(obj);
		}
		set
		{
			Set(obj, value);
		}
	}

	public SpireField(Func<TVal?> defaultVal)
	{
		_defaultVal = (TKey _) => defaultVal();
	}

	public SpireField(Func<TKey, TVal?> defaultVal)
	{
		_defaultVal = defaultVal;
	}

	public TVal? Get(TKey obj)
	{
		if (_table.TryGetValue(obj, out TVal value))
		{
			return value;
		}
		_table.Add(obj, value = _defaultVal(obj));
		return value;
	}

	public void Set(TKey obj, TVal? val)
	{
		_table.AddOrUpdate(obj, val);
	}
}
