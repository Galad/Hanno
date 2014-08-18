using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Hanno.Xaml.Converters
{
	public class StringFormatConverter : IValueConverter
	{
		public string Format { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null || parameter == null & string.IsNullOrEmpty(Format))
			{
				return value;
			}
			var format = string.IsNullOrEmpty(Format) ? (string) parameter : Format;

			return string.Format(new CultureInfo(language), format, value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}