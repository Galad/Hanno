using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Hanno.Xaml.Converters
{
	public class NullToVisibility : IValueConverter
	{
		public Visibility NullVisibility { get; set; }
		public Visibility NotNullVisibility { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return value == null ? NullVisibility : NotNullVisibility;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}