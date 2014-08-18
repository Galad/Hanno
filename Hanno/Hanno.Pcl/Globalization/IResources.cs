using System.Globalization;

namespace Hanno.Globalization
{
	public interface IResources
	{
		string Get(string name);
		string Get(string name, CultureInfo cultureInfo);
	}
}