using System;
using System.Collections.Generic;

namespace System.Linq
{	
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (action == null) throw new ArgumentNullException("action");
			return source.Select(i =>
			{
				action(i);
				return i;
			});
		}
	}
}