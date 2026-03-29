using System;

namespace BaseLib.Extensions;

public static class TypePrefix
{
	public const char PrefixSplitChar = '-';

	public static string GetPrefix(this Type t)
	{
		if (t.Namespace != null)
		{
			return $"{t.Namespace.Substring(0, t.Namespace.IndexOf('.')).ToUpperInvariant()}{45}";
		}
		return "";
	}
}
