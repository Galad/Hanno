using System;
using Windows.UI.Xaml.Data;

namespace Hanno.Xaml.Converters
{
	public sealed class LazyConverter : IValueConverter
	{
		private readonly Lazy<IValueConverter> _converter;
		public LazyConverter(Func<IValueConverter> converter)
		{
			_converter = new Lazy<IValueConverter>(converter);
		}

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return _converter.Value.Convert(value, targetType, parameter, language);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return _converter.Value.ConvertBack(value, targetType, parameter, language);
		}
	}
}
