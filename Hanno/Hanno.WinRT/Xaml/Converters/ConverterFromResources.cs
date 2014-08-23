using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Hanno.Xaml.Converters
{
	/// <summary>
	/// A converter instanciable in XAML, which call the referenced converter.
	/// </summary>
	public sealed class ConverterFromResources : IValueConverter
	{
		public IValueConverter Converter { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return Converter.Convert(value, targetType, parameter, language);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return Converter.ConvertBack(value, targetType, parameter, language);
		}
	}
}
