using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Hanno.Xaml.Converters
{
	public class BoolToVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			bool b;
			if (value == null)
			{
				b = false;
			}
			else
			{
				b = (bool) value;
			}
			var inverse = parameter != null && parameter.ToString().Equals("i", StringComparison.OrdinalIgnoreCase);
			var visibility = b ? Visibility.Visible : Visibility.Collapsed;
			if (!inverse)
			{
				return visibility;
			}
			return VisibilityHelper.InverseVisibility(visibility);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}