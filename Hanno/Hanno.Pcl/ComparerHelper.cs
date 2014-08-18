using System.Collections.Generic;

namespace Hanno
{
	public class ComparerHelper
	{
		public static T Max<T>(T value1, T value2)
		{
			var comparisonResult = Comparer<T>.Default.Compare(value1, value2);
			return comparisonResult >= 0 ? value1 : value2;
		}

		public static T Min<T>(T value1, T value2)
		{
			var comparisonResult = Comparer<T>.Default.Compare(value1, value2);
			return comparisonResult <= 0 ? value1 : value2;
		} 
	}
}