namespace BaseLib.Extensions;

public static class StringExtensions
{
	public static string RemovePrefix(this string id)
	{
		int num = id.IndexOf('-') + 1;
		return id.Substring(num, id.Length - num);
	}
}
