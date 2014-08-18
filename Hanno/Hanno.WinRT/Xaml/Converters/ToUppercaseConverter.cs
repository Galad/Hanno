using System;
using Windows.UI.Xaml.Data;

namespace Hanno
{
	public class ToUppercaseConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null)
			{
				return value;
			}
			return ((string) value).ToUpperInvariant();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}