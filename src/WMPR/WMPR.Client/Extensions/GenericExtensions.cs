using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace WMPR.Client.Extensions
{
	public static class GenericExtensions
	{
		public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
		{
			lock (source)
			{
				foreach (var item in items)
				{
					source.Add(item);
				}
			}
		}

		public static int IndexOf<T>(this IList<T> source, Predicate<T> condition)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (condition == null) throw new ArgumentNullException(nameof(condition));

			for (int i = 0; i < source.Count; i++)
			{
				if (condition(source[i]))
					return i;
			}

			return -1;
		}

		public static bool In<T>(this T source, params T[] options)
		{
			var comparer = EqualityComparer<T>.Default;
			foreach (var option in options)
			{
				if (comparer.Equals(option, source))
				{
					return true;
				}
			}

			return false;
		}

		public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> source, Predicate<TIn> where, Func<TIn, TOut> select)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (@where == null) throw new ArgumentNullException(nameof(@where));
			if (@select == null) throw new ArgumentNullException(nameof(@select));

			foreach (var @in in source)
			{
				if (where(@in))
					yield return select(@in);
			}
		}
	}
}