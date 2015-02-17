using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Navigation
{
	public interface IRequestNavigation
	{
		/// <summary>
		/// Starts a navigation
		/// </summary>		
		Task Navigate(CancellationToken ct, INavigationRequest request);
	}
}