using System;
using System.Collections.Generic;

namespace BaseLib.Patches.Content;

public static class CustomEnums
{
	private class KeyGenerator
	{
		private static readonly Dictionary<Type, Func<object, object>> Incrementers = new Dictionary<Type, Func<object, object>>
		{
			{
				typeof(byte),
				(object val) => (byte)val + 1
			},
			{
				typeof(sbyte),
				(object val) => (sbyte)val + 1
			},
			{
				typeof(short),
				(object val) => (short)val + 1
			},
			{
				typeof(ushort),
				(object val) => (ushort)val + 1
			},
			{
				typeof(int),
				(object val) => (int)val + 1
			},
			{
				typeof(uint),
				(object val) => (uint)val + 1
			},
			{
				typeof(long),
				(object val) => (long)val + 1
			},
			{
				typeof(ulong),
				(object val) => (ulong)val + 1
			}
		};

		private object _nextKey;

		private readonly Func<object, object> _increment;

		public KeyGenerator(Type t)
		{
			if (!t.IsEnum)
			{
				_increment = (object o) => o;
				throw new ArgumentException("Attempted to construct KeyGenerator with non-enum type " + t.FullName);
			}
			Array enumValuesAsUnderlyingType = t.GetEnumValuesAsUnderlyingType();
			Type underlyingType = Enum.GetUnderlyingType(t);
			_nextKey = Convert.ChangeType(0, underlyingType);
			_increment = Incrementers[underlyingType];
			if (enumValuesAsUnderlyingType.Length > 0)
			{
				foreach (object item in enumValuesAsUnderlyingType)
				{
					if (((IComparable)item).CompareTo(_nextKey) >= 0)
					{
						_nextKey = _increment(item);
					}
				}
			}
			MainFile.Logger.Info($"Generated KeyGenerator for enum {t.FullName} with starting value {_nextKey}", 1);
		}

		public object GetKey()
		{
			object nextKey = _nextKey;
			_nextKey = _increment(_nextKey);
			return nextKey;
		}
	}

	private static readonly Dictionary<Type, KeyGenerator> KeyGenerators = new Dictionary<Type, KeyGenerator>();

	public static object GenerateKey(Type enumType)
	{
		if (!KeyGenerators.TryGetValue(enumType, out KeyGenerator value))
		{
			KeyGenerators.Add(enumType, value = new KeyGenerator(enumType));
		}
		return value.GetKey();
	}
}
