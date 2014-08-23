using System;
using Windows.UI.Xaml.Data;

namespace Hanno.Xaml.Converters
{
	public sealed class ReverseBoolConverter : IValueConverter
	{
		public bool NullValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return ReverseBool(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return ReverseBool(value);
		}

		private static object ReverseBool(object value)
		{
			if (value == null)
			{
				return false;
			}
			return !((bool) value);
		}
	}
}