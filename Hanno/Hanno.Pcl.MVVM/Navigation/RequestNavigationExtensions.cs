using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Navigation
{
	public static class RequestNavigationExtensions
	{
		public static Task Navigate(this IRequestNavigation requestNavigation, CancellationToken ct, string viewName)
		{
			return requestNavigation.Navigate(ct, viewName, Enumerable.Empty<KeyValuePair<string, string>>());
		}

		public static Task Navigate(this IRequestNavigation requestNavigation, CancellationToken ct, string viewName, IEnumerable<KeyValuePair<string, string>> parameters, object entity = null)
		{
			return requestNavigation.Navigate(ct, new NavigationRequest(viewName, parameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), entity));
		}
	}
}