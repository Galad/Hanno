using System;
using System.Diagnostics;

namespace Hanno.Diagnostics
{
	public static class ObjectExtensions
	{
		public static T DebugWriteline<T>(this T source)
		{
			Debug.WriteLine(source);
			return source;
		}

		public static T DebugWriteline<T, TValue>(this T source, Func<T, TValue> selector)
		{
			Debug.WriteLine(selector(source));
			return source;
		}

		public static T DebugWriteline<T>(this T source, string format)
		{
			Debug.WriteLine(format, source);
			return source;
		}
	}
}