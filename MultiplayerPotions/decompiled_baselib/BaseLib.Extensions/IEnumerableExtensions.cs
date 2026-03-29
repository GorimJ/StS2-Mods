using System.Collections.Generic;
using System.Text;

namespace BaseLib.Extensions;

public static class IEnumerableExtensions
{
	public static string AsReadable<T>(this IEnumerable<T> enumerable, string separator = ",")
	{
		return string.Join(separator, enumerable);
	}

	public static string NumberedLines<T>(this IEnumerable<T> enumerable)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		foreach (T item in enumerable)
		{
			stringBuilder.Append(num).Append(": ").Append(item)
				.AppendLine();
			num++;
		}
		return stringBuilder.ToString();
	}
}
