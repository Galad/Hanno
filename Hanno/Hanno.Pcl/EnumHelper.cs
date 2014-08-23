using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanno
{
	public static class EnumHelper
	{
		public static IEnumerable<T> GetValues<T>() where T : struct
		{
			return Enum.GetValues(typeof(T))
					   .Cast<T>();
		}
	}
}