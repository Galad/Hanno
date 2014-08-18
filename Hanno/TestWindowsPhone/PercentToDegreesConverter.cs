using System;
using System.Globalization;
using System.Windows.Data;
#if !NETFX_CORE

#else
using System.Globalization;
using Windows.UI.Xaml.Data;
using GenericCulture = System.String;
#endif

namespace TestWindowsPhone
{
	public class PercentToDegreesConverter : IValueConverter
	{
		public double OneHundredPercentValue { get; set; }
		public object Convert(object value, Type targetType, object parameter, CultureInfo language)
		{
			var percent = (double)value;
			if (double.IsNaN(percent))
			{
				percent = 0d;
			}
			//from 0 to 359
			return (double)(359 * percent / OneHundredPercentValue);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
		{
			throw new NotImplementedException();
		}
	}
}
