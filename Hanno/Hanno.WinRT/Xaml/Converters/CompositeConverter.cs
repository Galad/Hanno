using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Hanno.Xaml.Converters
{
	public class CompositeConverter : List<IValueConverter>, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return this.Aggregate(value, (v, converter) => converter.Convert(v, targetType, parameter, language));
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}