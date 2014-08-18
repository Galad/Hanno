using System;
using Windows.UI.Xaml.Data;

namespace Hanno.Xaml.Converters
{
	public class BoolToValueConverter : IValueConverter
	{
		public object TrueValue { get; set;}
		public object FalseValue { get; set; }
		/// <summary>
		/// Optional. Use FalseValue by default when the value is null
		/// </summary>
		public object NullValue { get; set;} 

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null)
			{
				return NullValue ?? FalseValue;
			}
			var b = (bool) value;
			return b ? TrueValue : FalseValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}