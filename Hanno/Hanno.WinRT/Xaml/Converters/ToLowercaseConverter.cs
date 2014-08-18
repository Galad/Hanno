using System;
using Windows.UI.Xaml.Data;

namespace Hanno
{
	public class ToLowercaseConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null)
			{
				return value;
			}
			return ((string)value).ToLowerInvariant();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}