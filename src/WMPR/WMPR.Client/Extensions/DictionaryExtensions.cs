using System.Collections.Generic;

namespace WMPR.Client.Extensions
{
	public static class DictionaryExtensions
	{
		public static void AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
		{
			if (source.ContainsKey(key))
			{
				source[key] = value;
			}
			else
			{
				source.Add(key, value);
			}
		}
	}
}